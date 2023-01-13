using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using XFP.Impact_Ultimate.Utlis;
using XFP.Impact_Ultimate.Utlis.Log;
using XFP.Impact_Ultimate.Utlis.Model;
using XFP.Impact_Ultimate.Utlis.Model.Settings;
using MessageBox = System.Windows.MessageBox;

namespace XFP.Impact_Ultimate.Controls
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : UserControl
    {
        DataProvider data = new();
        DeveloperOption option = new();
        LogWriter log = new();
        UserSettings setter = new();
        SetterWriter setterWriter = new();
        KeySetter key = new();

        public bool IsGreedG = false;
        public string OneTimeID;

        private string GenerateId() 
        {
            if (IsGreedG == false)
            {
                long i = 1;
                foreach (byte b in Guid.NewGuid().ToByteArray())
                {
                    i *= b + 1;
                }
                IsGreedG = true;
                OneTimeID = string.Format("{0:x}", i - DateTime.Now.Ticks);
                return OneTimeID;
            }
            else
            { 
                return OneTimeID;
            }
        }

        public bool _UseDarkModel = false;

        public HomePage()
        {
            InitializeComponent();

            setterWriter.read();
        }

        private void DeveloperOptions_Click(object sender, RoutedEventArgs e)
        {
            if (data.ModuleVersion != 3)
            {
                System.Windows.MessageBox.Show("您的版本不是开发者版本");
                return;
            }
            else
            {
                option.UseDeveloperOptions = true;
                DeveloperOptions.Content = "已启用";
            }
        }

        private void AutoStart_Click(object sender, RoutedEventArgs e)
        {
            if (setter.AutoStart == false)
            {
                AutoStart.Content = "已启用";
                AutoStart.Opacity = 0.5;
                setter.AutoStart = true;

                #region Set
                string path = "\"" + Environment.CurrentDirectory + "\\XFP.Impact_Ultimate.exe\"";
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                rk2.SetValue("ICoraShutdown", path);
                rk2.Close();
                rk.Close();

                Growl.Info("这容易被杀毒软件杀掉\n若未自启动成功 极有可能是被杀毒软件所杀");
                #endregion
            }
            else
            {
                AutoStart.Content = "启用";
                AutoStart.Opacity = 1;
                setter.AutoStart = false;

                #region Set
                string path = Environment.CurrentDirectory + "\\XFP.Impact_Ultimate.exe";
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                rk2.SetValue("ICoraShutdown", false);
                rk2.Close();
                rk.Close();
                #endregion
            }
        }

        private void FeedBack_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://gitee.com/MasterGashByte/impact_ultimate_issues/issues");
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            if(HandyControl.Controls.MessageBox.Show
                ("您确定要这么做？", "防误触提示", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) 
            {
                File.Delete(data.DataLog);
                File.Delete(data.ErrorLog);
                File.Delete(data.TempLog);
                File.Delete(data.SettingsData);
                Growl.Success("清除成功");
            }
        }

        private void UnInstall_Click(object sender, RoutedEventArgs e)
        {
            if (HandyControl.Controls.MessageBox.Show
                ("您确定要这么做？", "防误触提示",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (File.Exists(Environment.CurrentDirectory + "\\XFP.UnInstaller.exe")
                    || File.Exists(Environment.CurrentDirectory + "\\XFP.UnInstaller.dll")
                    || File.Exists(Environment.CurrentDirectory + "\\XFP.UnInstaller.deps.json")
                    || File.Exists(Environment.CurrentDirectory + "\\XFP.UnInstaller.runtimeconfig.json"))
                {
                    Process.Start("explorer.exe", Environment.CurrentDirectory + "\\XFP.UnInstaller.exe");
                    Environment.Exit(0);
                }
                else
                {
                    Growl.Error("您的ICora仿佛不齐全 请前往群中重新下载");
                    log.ErrorLog("DetectionSystem: ICora is incomplete", -0, "您的ICora不是完整的 您可以前往群中获取完整的Impact_Ultiamte");
                }
            }
        }

        private void OpenDataDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Environment.CurrentDirectory + "\\DataBase");
        }

        private void OpenUserDataDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Environment.CurrentDirectory + "\\UserData");
        }

        private void ClearWebCookie_Click(object sender, RoutedEventArgs e)
        {
            Growl.Success("WebView 仿佛没有留下任何数据");
        }

        private void CheckVersionData_Click(object sender, RoutedEventArgs e)
        {
#pragma warning disable CA1416
            HandyControl.Controls.MessageBox.Show
                            (
                "当前版本：1.2.0\n版本类型：常规版\nICoraOne-TimeID：" + GenerateId() + "\n" +
                "ICora授权码：None"
                , "ICora V1.2.0",
                MessageBoxButton.YesNo, MessageBoxImage.Information);
#pragma warning restore CA1416
        }

        private void JoinQQGroup_Click(object sender, RoutedEventArgs e)
        {
            Growl.Info("请手动添加QQ群：\n一群：811979687\n二群：590566763\n劳烦你了 其实本来是有自动跳转的\n不过qq的连接实在是打不开 所以手动添加一下吧\nQQ群已经复制到粘贴板咯");
            Clipboard.SetDataObject("一群: 811979687 | 二群: 590566763");
        }
    }
}
