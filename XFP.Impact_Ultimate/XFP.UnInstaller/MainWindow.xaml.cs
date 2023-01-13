using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XFP.UnInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (MessageBox.Show
                ("您确定要卸载您的ICora 这是完全不可逆的操作？", "防误触提示",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var count = Directory.GetFiles(Environment.CurrentDirectory).Length;
                    for (int i = 0; i < count; i++)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
                        if (dir.Exists)
                        {
                            DirectoryInfo[] childs = dir.GetDirectories();
                            foreach (DirectoryInfo child in childs)
                            {
                                child.Delete(true);
                            }
                            dir.Delete(true);
                        }
                        count--;
                    }
                }
                catch
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                    key.CreateSubKey("Impact_Ultimate");
                    key.DeleteSubKey("Impact_Ultimate");

                    MessageBox.Show("卸载成功 如果ICora依然存在 请手动打开 UnInstaller");
                    ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 1000 > Nul & Del " + Environment.CurrentDirectory + "\\XFP.UnInstaller.exe");
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.CreateNoWindow = true;
                    Process.Start(psi);
                    ProcessStartInfo psi2 = new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 1000 > Nul & Del " + Environment.CurrentDirectory + "\\XFP.UnInstaller.dll");
                    psi2.WindowStyle = ProcessWindowStyle.Hidden;
                    psi2.CreateNoWindow = true;
                    Process.Start(psi2);
                    Environment.Exit(0);
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
