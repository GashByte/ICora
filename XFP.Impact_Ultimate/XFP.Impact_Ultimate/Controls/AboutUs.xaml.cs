using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace XFP.Impact_Ultimate.Controls
{
    /// <summary>
    /// AboutUs.xaml 的交互逻辑
    /// </summary>
    public partial class AboutUs : UserControl
    {
        public AboutUs()
        {
            InitializeComponent();
        }

        private void GetICora_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://gitee.com/XFP-Group/Impact_Ultimate/");
        }

        private void GetICora_Github_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "");
        }
    }
}
