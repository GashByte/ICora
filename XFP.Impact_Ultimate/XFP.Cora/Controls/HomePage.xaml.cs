//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

namespace XFP.ICora.Controls
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>

    public partial class HomePage : UserControl
    {
        #region Initialze
        DataProvider data = new();
        DeveloperOption option = new();
        LogWriter log = new();
        KeySetter key = new();
        #endregion

        #region define
        private bool IsGreedG = false;
        private string OneTimeID;
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
        #endregion

        #region MainMethod
        public HomePage()
        {
            InitializeComponent();

            try
            {
                var myReg = Registry.LocalMachine.OpenSubKey(
                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", RegistryKeyPermissionCheck.ReadWriteSubTree,
                        RegistryRights.FullControl);
                if (myReg.GetValue("XFP.Impact_Ultiamte").ToString() != string.Empty)
                {
                    AutoStart.Content = "已启用";
                }
            }
            catch { }
        }
        #endregion

        #region Method
        /// <summary>
        /// 启用开发者按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeveloperOptions_Click(object sender, RoutedEventArgs e)
        {
            if (data.ModeVersion != ModuleVersion.DevelopmentEdition)
            {
                MessageBox.Show("您的版本不是开发者版本");
                return;
            }
            else
            {
                option.UseDeveloperOptions = true;
                DeveloperOptions.Content = "已启用";
            }
        }

        /// <summary>
        /// 开机自启动按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoStart_Click(object sender, RoutedEventArgs e)
        {
            if (AutoStart.Content.ToString() != "已启用")
            {
                AutoStart.Content = "已启用";
                AdminAutoStart();
            }
            else
            {
                AutoStart.Content = "启用";
                CancelAdminAutoStart();
            }
        }

        /// <summary>
        /// 用于创建开机自启动的方法
        /// </summary>
        private void AdminAutoStart()
        {
            var starupPath = GetType().Assembly.Location;
            try
            {
                var fileName = Environment.CurrentDirectory + $"\\XFP.ICora.exe";
                var shortFileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                var myReg = Registry.LocalMachine.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", RegistryKeyPermissionCheck.ReadWriteSubTree,
                    RegistryRights.FullControl);
                if (myReg == null)
                {
                    myReg = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                if (myReg != null && myReg.GetValue(shortFileName) != null)
                {
                    myReg.DeleteValue(shortFileName);
                    myReg.SetValue(shortFileName, fileName);
                }
                else if (myReg != null && myReg.GetValue(shortFileName) == null)
                {
                    myReg.SetValue(shortFileName, fileName);
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// 用于取消开机自启动的方法
        /// </summary>
        private void CancelAdminAutoStart()
        {
            var starupPath = GetType().Assembly.Location;
            try
            {
                var fileName = starupPath;
                var shortFileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                var myReg = Registry.LocalMachine.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", RegistryKeyPermissionCheck.ReadWriteSubTree,
                    RegistryRights.FullControl);
                if (myReg == null)
                {
                    myReg = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                if (myReg != null && myReg.GetValue(shortFileName) != null)
                {
                    myReg.DeleteValue(shortFileName);
                }
                else if (myReg != null && myReg.GetValue(shortFileName) == null)
                {
                    return;
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// 反馈按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FeedBack_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", "https://gitee.com/MasterGashByte/impact_ultimate_issues/issues");

        /// <summary>
        /// 清除所有日志按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            if (HandyControl.Controls.MessageBox.Show
                ("您确定要这么做？", "防误触提示",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                File.Delete(data.DataLog);
                File.Delete(data.ErrorLog);
                File.Delete(data.TempLog);
                File.Delete(data.SettingsData);
                Growl.Clear();
                Growl.Success("清除成功");
            }
        }

        /// <summary>
        /// 卸载ICora按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    Growl.Clear();
                    Growl.Error("您的ICora仿佛不齐全 请前往群中重新下载");
                    log.ErrorLog("DetectionSystem: ICora is incomplete", -0, "您的ICora不是完整的 您可以前往群中获取完整的Impact_Ultiamte");
                }
            }
        }

        /// <summary>
        /// 打开日志目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDataDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Environment.CurrentDirectory + "\\DataBase");
        }

        /// <summary>
        /// 打开用户账户数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenUserDataDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Environment.CurrentDirectory + "\\UserData");
        }

        /// <summary>
        /// 清除WebView的Cookie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearWebCookie_Click(object sender, RoutedEventArgs e)
        {
            Growl.Clear();
            Growl.Warning("接下来会进行很多次的删除操作\n如果出现闪退是因为线程卡死了");
            if (Directory.Exists(Environment.CurrentDirectory + "\\XFP.ICora.exe.WebView2"))
            {
                new Thread(() =>
                {
                    try
                    {
                        ClearWebView2(Environment.CurrentDirectory + "\\XFP.ICora.exe.WebView2");
                    }
                    catch
                    {
                        ClearWebView2(Environment.CurrentDirectory + "\\XFP.ICora.exe.WebView2");
                    }
                    Directory.Delete(Environment.CurrentDirectory + "\\XFP.ICora.exe.WebView2");
                    Growl.Clear();
                    Growl.Success("清理完成");
                }).Start();
            }
            else
            {
                Growl.Clear();
                Growl.Success("WebView 仿佛没有留下任何数据");
            }
        }

        private void ClearWebView2(string path)
        {
            try
            {
                DirectoryInfo info = new(path);
                var filesinfo = info.GetFileSystemInfos();
                foreach (var file in filesinfo)
                {
                    if (file is DirectoryInfo)
                    {
                        ClearWebView2(file.FullName);
                        Directory.Delete(file.FullName);
                    }
                    else
                    {
                        File.Delete(file.FullName);
                    }
                }
            }
            catch { return; }
        }

        /// <summary>
        /// 查看版本日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckVersionData_Click(object sender, RoutedEventArgs e)
            => HandyControl.Controls.MessageBox.Show(
                    "当前版本：" + data.Version + "\n版本类型：" + data.ModeVersion + "\n数据验证密钥：" + GenerateId() + "\n" +
                    "数据库连接状态: " + IsCanConnect() + "\n\nCopyright(C) XFP Group 2022-2023"
                    , "ICora V" + data.Version,
                    MessageBoxButton.YesNo, MessageBoxImage.Information);

        /// <summary>
        /// 加入QQ群聊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JoinQQGroup_Click(object sender, RoutedEventArgs e)
        {
            Growl.Clear();
            Growl.Info("请手动添加QQ群：\n一群：811979687\n二群：590566763\n劳烦你了 其实本来是有自动跳转的\n不过qq的连接实在是打不开 所以手动添加一下吧\nQQ群已经复制到粘贴板咯");
            Clipboard.SetDataObject("一群: 811979687 | 二群: 590566763");
        }

        /// <summary>
        /// 通讯判断
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string IsCanConnect()
        {
            string url = "https://gashbyte.github.io/ICoraIndex";
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            string ReturnStr = "连接成功";
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                res = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception)
            {
                ReturnStr = "连接失败";
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
            return ReturnStr;
        }
        #endregion
    }
}
