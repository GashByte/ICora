//Copyright(c) XFP Group and Contributors. All rights reserved. All rights reserved.
//Licensed under the MIT License.

namespace XFP.ICora.Controls
{
    /// <summary>
    /// Binding DataList
    /// </summary>
    public class LocalDataList
    {
        #region Bindings

        public string Name { get; set; }

        #endregion
    }

    /// <summary>
    /// AkebiPage.xaml 的交互逻辑
    /// </summary>

    public partial class AkebiPage
    {
        #region Initializtion
        KeySetter key = new();
        LogWriter log = new();
        INIFiles ini = new();

        public ObservableCollection<LocalDataList> UDataList { get; } = new ObservableCollection<LocalDataList>();
        #endregion

        #region define
        private string CLPath = Environment.CurrentDirectory + "\\ICora\\CLibrary.dll";
        private string AkebiPath = Environment.CurrentDirectory + "\\ICora";
        private string UserData = Environment.CurrentDirectory + "\\UserData";
        private string GenshinServiceDir = Environment.CurrentDirectory + "\\GenshinService";

        private bool InAdd = false;
        private bool InModify = false;
        #endregion

        #region Main Method

        /// <summary>
        /// 构建开始游戏页面
        /// </summary>
        public AkebiPage()
        {
            InitializeComponent();

            #region 文件夹判断
            if (!Directory.Exists(AkebiPath))
                Directory.CreateDirectory(AkebiPath);
            if (!Directory.Exists(UserData))
                Directory.CreateDirectory(UserData);
            #endregion
            // 服务器选择的List
            List<string> GenshinService = new List<string>
            {
                "官方服 | 天空岛",
                "渠道服 | 世界树",
                "国际服 | Global"
            };

            if (!Directory.Exists(AkebiPath))
            {
                Directory.CreateDirectory(AkebiPath);
            }

            UChooseService.ItemsSource = GenshinService;
            UChooseAccount.ItemsSource = UDataList;

            RefreshList();
            #region 基础设置
            try
            {
                #region 读取配置文件
                UGenshinImpactPath.Text = key.gk("Genshin Impact Path") == string.Empty ? "没有找到你的原神" : key.gk("Genshin Impact Path");
                UChooseDll.Text = key.gk("UChooseDll") == string.Empty ? "默认路径" : key.gk("UChooseDll");
                if (UGenshinImpactPath.Text == string.Empty)
                {
                    UGenshinImpactPath.Text = "没有找到你的原神路径";
                }
                if (UChooseDll.Text == string.Empty)
                {
                    UChooseDll.Text = "默认路径";
                }
                UScreenWidth.Text = key.gk("Screen Width");
                UScreenHeight.Text = key.gk("Screen Height");
                UChooseAccount.SelectedItem = key.gk("Account");
                UAccountChange.Text = UChooseAccount.Text;

                if (key.gk("Start Game Model") == "注入DLL")
                {
                    UGameStartModel.Text = "注入DLL";
                    UStartModel.Content = "注入DLL";
                    UChooseDllPath.IsEnabled = true;
                    UChooseDll.Opacity = 1;
                }
                else
                {
                    UGameStartModel.Text = "默认模式";
                    UStartModel.Content = "默认模式";
                    UChooseDllPath.IsEnabled = false;
                    UChooseDll.Opacity = 0.5;
                }

                if (key.gk("Mult Start") == "True")
                    MultStart.Content = "已启用";
                else
                    MultStart.Content = "启用";

                if (key.gk("Is Full Screen") == "True")
                    UIsFullScreen.Content = "已启用";
                else
                    UIsFullScreen.Content = "启用";

                if (key.gk("Border less") == "True")
                    UBorderless.Content = "已启用";
                else
                    UBorderless.Content = "启用";

                if (key.gk("Check CLibrary") == "True")
                    CheckCL.Content = "已启用";
                else
                    CheckCL.Content = "启用";
                #endregion

                #region 服务器判定
                if (UGenshinImpactPath.Text != string.Empty)
                {
                    if (File.Exists(UGenshinImpactPath.Text))
                    {
                        string GenshinName = Path.GetFileName(UGenshinImpactPath.Text);
                        if (GenshinName == "YuanShen.exe")
                        {
                            DirectoryInfo info = new DirectoryInfo(UGenshinImpactPath.Text);
                            string YuanShenDir = info.Parent.FullName;
                            string YuanShenChannel = ini.INIRead("General", "channel", YuanShenDir + "\\config.ini");
                            UGameService.Text = YuanShenChannel == "1" ? "当前服务器：官方服 | 天空岛"
                                : YuanShenChannel == "14" ? "当前服务器：渠道服 | 世界树"
                                : string.Empty;
                        }
                        else if (GenshinName == "GenshinImpact.exe")
                        {
                            UGameService.Text = "当前服务器：国际服 | Global";
                        }
                        else
                        {
                            UGameService.Text = "当前服务器：出错了捏？！";
                        }
                    }
                }
                else
                {
                    UGameService.Text = "当前服务器：没有获取到当前服务器";
                }

                #region 选择判断
                UChooseService.SelectedItem = UGameService.Text == "当前服务器：官方服 | 天空岛" ? "官方服 | 天空岛"
                    : UGameService.Text == "当前服务器：国际服 | Global" ? "国际服 | Global"
                    : UGameService.Text == "当前服务器：渠道服 | 世界树" ? "渠道服 | 世界树"
                    : UChooseService.SelectedItem = null;
                #endregion
                #endregion
            }
            catch { }
            
            // 进行全局检查
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            bool isAdministrator = currentUser.IsSystem;
            if (!isAdministrator)
            {
                Growl.Clear();
                Growl.Warning("未获取提权 部分功能将无法使用");
                UStartModel.Content = "默认模式";
                UStartModel.IsEnabled = false;
                UChooseDllPath.IsEnabled = false;
                UChooseDll.Opacity = 0.5;
                UGameStartModel.Text = UStartModel.Content.ToString();
                key.sk("Start Game Model", UGameStartModel.Text);
                UChooseService.IsEnabled = false;
            }
            
            #endregion
        }

        #endregion

        #region Normal Method

        /// <summary>
        /// 刷新List
        /// </summary>
        private void RefreshList()
        {
            UDataList.Clear();
            DirectoryInfo root = new DirectoryInfo(UserData);
            FileInfo[] files = root.GetFiles();
            foreach (FileInfo file in files)
            {
                UDataList.Add(new LocalDataList()
                {
                    Name = file.Name
                });
            }

            if (UChooseAccount.Items.Count > 0)
            {
                DelAccount.IsEnabled = true;
                ModifyAccount.IsEnabled = true;
                splitButton.Visibility = Visibility.Visible;
            }
            else
            {
                DelAccount.IsEnabled = false;
                ModifyAccount.IsEnabled = false;
                splitButton.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 切换账户
        /// </summary>
        private void Switch()
        {
            Thread.Sleep(100);
            YSAccount acct = YSAccount.ReadFromDisk(UChooseAccount.Text);
            acct.WriteToRegedit();

            key.sk("Account", UChooseAccount.Text);
            UAccountChange.Text = UChooseAccount.Text;

            Growl.Clear();
            Growl.Success($"切换成功 当前账户:{UChooseAccount.Text}");
        }

        /// <summary>
        /// 启动游戏
        /// </summary>
        public void StartGame()
        {
            try
            {
                if (string.IsNullOrEmpty(UGenshinImpactPath.Text))
                {
                    Growl.Clear();
                    Growl.Error("您的原神路径是空的");
                }
                else
                {
                    if (YuanShenIsRunning() && MultStart.Content.ToString() == "启用")
                    {
                        Growl.Clear();
                        Growl.Error("原神已经启动 请关闭后再试");
                        return;
                    }
                    try
                    {
                        bool TokenRet = DllUtils.OpenProcessToken(DllUtils.GetCurrentProcess(), 0xF00FF, out IntPtr hToken);
                        var si = new DllUtils.STARTUPINFOEX();
                        si.StartupInfo.cb = Marshal.SizeOf(si);
                        if (hToken == IntPtr.Zero)
                        {
                            Growl.Clear();
                            Growl.Warning("提权失败 正常启动");
                            StartGameNormal();
                            return;
                        }
                        var pExporer = Process.GetProcessesByName("explorer")[0];
                        if (pExporer == null)
                        {
                            Growl.Clear();
                            Growl.Warning("Explorer未找到 正常启动");
                            StartGameNormal();
                            return;
                        }
                        IntPtr handle = DllUtils.OpenProcess(0xF0000 | 0x100000 | 0xFFFF, false, (uint)pExporer.Id);
                        var lpSize = IntPtr.Zero;
                        DllUtils.InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref lpSize);
                        si.lpAttributeList = Marshal.AllocHGlobal(lpSize);
                        DllUtils.InitializeProcThreadAttributeList(si.lpAttributeList, 1, 0, ref lpSize);
                        if (DllUtils.UpdateProcThreadAttribute(si.lpAttributeList, 0, 0x00020004, handle, IntPtr.Size, IntPtr.Zero, IntPtr.Zero))
                        {
                            Growl.Clear();
                            Growl.Error("更新线程失败");
                        }
                        DirectoryInfo path = new DirectoryInfo(UGenshinImpactPath.Text);
                        var pi = new DllUtils.PROCESS_INFORMATION();

                        string args = null;
                        if (key.gk("Is Full Screen") == "True")
                        {
                            args += new CommandLineBuilder()
                                .AppendIf("-screen-fullscreen", true)
                                .ToString();
                        }
                        if (key.gk("Border less") == "True")
                        {
                            args += new CommandLineBuilder()
                                .AppendIf("-popupwindow", true)
                                .ToString();
                        }
                        args += new CommandLineBuilder()
                            .Append("-screen-height", UScreenHeight.Text.ToString())
                            .Append("-screen-width", UScreenWidth.Text.ToString())
                            .ToString();
                        var result = DllUtils.CreateProcessAsUser(hToken, Path.Combine(UGenshinImpactPath.Text).ToString(),
                            RunParameters.Text == string.Empty ? args : RunParameters.Text, IntPtr.Zero, IntPtr.Zero, false, 0x00080000 | 0x00000004,
                            IntPtr.Zero, Path.Combine(path.Parent.Parent.FullName, "Genshin Impact Game").ToString(), ref si.StartupInfo, out pi);
                        if (!result)
                        {
                            Growl.Clear();
                            Growl.Warning("启动暂停线程失败 正常启动");
                            StartGameNormal();
                            return;
                        }
                        DllUtils.DeleteProcThreadAttributeList(si.lpAttributeList);
                        new Thread(() =>
                        {
                            InjectDll(pi.hProcess);
                            Thread.Sleep(2000);
                            DllUtils.ResumeThread(pi.hThread);
                            DllUtils.CloseHandle(pi.hProcess);
                        }).Start();
                        Growl.Clear();
                        Growl.Success("启动成功 祝你游玩愉快！\nDll注入暂时不支持统计游玩时间 我找不到方法统计它");
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Clear();
                Growl.Error("在启动发现了异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 正常启动游戏
        /// </summary>
        private void StartGameNormal()
        {
            Stopwatch sw = new();
            string args = "";
            ProcessStartInfo startInfo = new();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Path.Combine(UGenshinImpactPath.Text);
            startInfo.Verb = "RunAs";
            if (key.gk("Is Full Screen") == "False"
                && key.gk("Border less") == "False")
            {
                args = new CommandLineBuilder()
                    .Append("-screen-height", UScreenHeight.Text.ToString())
                    .Append("-screen-width", UScreenWidth.Text.ToString())
                    .ToString();
                startInfo.Arguments = RunParameters.Text + args;
            }
            else
            {
                if (key.gk("Is Full Screen") == "True")
                {
                    args += new CommandLineBuilder()
                        .AppendIf("-screen-fullscreen", true)
                        .ToString();
                }
                if (key.gk("Border less") == "True")
                {
                    args += new CommandLineBuilder()
                        .AppendIf("-popupwindow", true)
                        .ToString();
                }
                args += new CommandLineBuilder()
                    .Append("-screen-height", UScreenHeight.Text.ToString())
                    .Append("-screen-width", UScreenWidth.Text.ToString())
                    .ToString();
                startInfo.Arguments = args;
            }

            new Thread(() =>
            {
                sw.Start();
                Process GenshinProc = Process.Start(startInfo);

                GenshinProc.EnableRaisingEvents = true;
                GenshinProc.Exited += (sender, e) =>
                {
                    sw.Stop();
                    TimeSpan ts = sw.Elapsed;
                    Growl.Clear();
                    Growl.Success($"游戏结束了 您游玩了: {ts.Hours}小时," +
                        $" {ts.Minutes}分钟, {ts.Seconds}秒\n希望它是一段美妙的路途");

                };
            }).Start();
        }

        /// <summary>
        /// 使用Exe启动游戏
        /// </summary>
        private void StartGameFormExe()
        {
            Stopwatch sw = new();
            ProcessStartInfo startInfo = new();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Path.Combine(UChooseDll.Text);
            startInfo.Verb = "runas";
            new Thread(() =>
            {
                sw.Start();
                Process GenshinProc = Process.Start(startInfo);

                GenshinProc.EnableRaisingEvents = true;
                GenshinProc.Exited += (sender, e) =>
                {
                    sw.Stop();
                    TimeSpan ts = sw.Elapsed;
                    Growl.Clear();
                    Growl.Success($"游戏结束了 您游玩了: {ts.Hours}小时," +
                        $" {ts.Minutes}分钟, {ts.Seconds}秒\n希望它是一段美妙的路途");

                };
            }).Start();
        }

        /// <summary>
        /// 判断原神是否启动
        /// </summary>
        /// <returns>True:已启动 False:未启动</returns>
        private bool YuanShenIsRunning()
        {
            var pros = Process.GetProcessesByName("YuanShen");
            if (pros.Any())
            {
                return true;
            }
            else
            {
                pros = Process.GetProcessesByName("GenshinImpact");
                return pros.Any();
            }
        }

        /// <summary>
        /// DLL注入
        /// </summary>
        /// <param name="hProc"></param>
        /// <returns></returns>
        private bool InjectDll(IntPtr hProc)
        {
            IntPtr hKernel = DllUtils.GetModuleHandle("kernel32.dll");
            if (hKernel == IntPtr.Zero)
            {
                MessageBox.Show("kernel32.dll模块地址寻找失败");
                return false;
            }
            IntPtr pLoadLibrary = DllUtils.GetProcAddress(hKernel, "LoadLibraryA");
            if (pLoadLibrary == IntPtr.Zero)
            {
                MessageBox.Show("LoadLibraryA地址获取失败");
                return false;
            }
            IntPtr pDllPath = DllUtils.VirtualAllocEx(hProc, IntPtr.Zero,
                (uint)((CLPath.Length + 1) * Marshal.SizeOf(typeof(char))), 0x1000 | 0x2000, 0x4);
            if (pDllPath == IntPtr.Zero)
            {
                MessageBox.Show(string.Format("申请内存地址失败(VirtualArrocEx Failed!) : {0}", Marshal.GetLastWin32Error()));
                return false;
            }
            bool writeResult = DllUtils.WriteProcessMemory(hProc, pDllPath,
                Encoding.Default.GetBytes(CLPath), (uint)((CLPath.Length + 1) * Marshal.SizeOf(typeof(char))), out _);
            if (!writeResult)
            {
                MessageBox.Show(string.Format("写进程内存失败(Wtire Process Memory Failed!) : {0}", DllUtils.GetLastError()));
                return false;
            }
            IntPtr hThread = DllUtils.CreateRemoteThread(hProc, IntPtr.Zero, 0, pLoadLibrary, pDllPath, 0, IntPtr.Zero);
            if (hThread == IntPtr.Zero)
            {
                MessageBox.Show(string.Format("创建远程线程失败(Create Remote Thread Failed!) : {0}", Marshal.GetLastWin32Error()));
                DllUtils.VirtualFreeEx(hProc, pDllPath, 0, 0x8000);
                return false;
            }
            if (DllUtils.WaitForSingleObject(hThread, 2000) == IntPtr.Zero)
            {
                DllUtils.VirtualFreeEx(hProc, pDllPath, 0, 0x8000);
            }
            DllUtils.CloseHandle(hThread);
            return true;
        }

        /// <summary>
        /// CL下载
        /// </summary>
        private void CLDownloader(bool UseCLDownloader)
        {
            if (UChooseDll.Text != "默认路径")
            {
                Growl.Clear();
                Growl.Warning("暂时无法获取你的Dll信息");
                return;
            }
            if (UseCLDownloader)
            {
                var localpath = Environment.CurrentDirectory;
                if (File.Exists(localpath + "\\CLDownloader.exe")
                    && File.Exists(localpath + "\\CLDownloader.deps.json")
                    && File.Exists(localpath + "\\CLDownloader.dll")
                    && File.Exists(localpath + "\\CLDownloader.runtimeconfig.json"))
                {
                    Process.Start(localpath + "\\CLDownloader.exe");
                }
                else
                {
                    Growl.Error("您的ICora仿佛不完整 请前往群中获取完整的ICora");
                    log.ErrorLog("DetectionSystem: Impact_Ultimate is incomplete", -0, "您的Impact_Ultimate不是完整的 您可以前往群中获取完整的Impact_Ultiamte");
                }
            }
            else
            {
                try
                {
                    HttpWebRequest request = WebRequest.CreateHttp(
                           "https://gitee.com/MasterGashByte/download/releases/download/CLibrary/CLibrary.dll");
                    HttpWebResponse? response = request.GetResponse() as HttpWebResponse;
                    Stream responseStream = response.GetResponseStream();
                    Stream stream = new FileStream(CLPath, FileMode.Create);
                    byte[] bArr = new byte[1024];
                    int size = responseStream.Read(bArr, 0, bArr.Length);
                    long totalBytesRead = 0;
                    do
                    {
                        stream.Write(bArr, 0, size);
                        size = responseStream.Read(bArr, 0, bArr.Length);
                    } while (size > 0);
                    stream.Close();
                    responseStream.Close();

                    Growl.Success("下载成功 以及尝试为您开始游戏");

                    StartGame();
                }
                catch (Exception ex)
                {
                    Growl.Error($"出现异常: {ex.Message}");
                    log.ErrorLog(ex.Message, -1);
                }
            }
        }

        #endregion

        #region Controls Method

        /// <summary>
        /// 选择游戏路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseGamePath_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                // 文件后缀
                Filter = "Exe Files (*.exe)|*.exe"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                UGenshinImpactPath.Text = openFileDialog.FileName;
                key.sk("Genshin Impact Path", openFileDialog.FileName);
            }
        }

        /// <summary>
        /// 选择游戏启动模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UStartModel_Click(object sender, RoutedEventArgs e)
        {
            if (UStartModel.Content.ToString() == "默认模式")
            {
                UStartModel.Content = "注入DLL";
                UChooseDllPath.IsEnabled = true;
                UChooseDll.Opacity = 1;
            }
            else
            {
                UStartModel.Content = "默认模式";
                UChooseDllPath.IsEnabled = false;
                UChooseDll.Opacity = 0.5;
            }
            UGameStartModel.Text = UStartModel.Content.ToString();
            key.sk("Start Game Model", UGameStartModel.Text);
        }

        /// <summary>
        /// 账户选择逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!InModify)
            {
                if (AccountName.Visibility == Visibility.Hidden)
                {
                    InAdd = true;
                    AccountName.Visibility = Visibility.Visible;
                    Growl.Clear();
                    Growl.Info("输入完成后按下回车确定, ESC退出");
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// AccountName TextBox回车判断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                AccountName.Visibility = Visibility.Hidden;
                AccountName.Text = string.Empty;
                InAdd = false;
                return;
            }
            if (AccountName.Text.Length > 15)
            {
                Growl.Clear();
                Growl.Warning("名称最高只能有15个字符哦");
                AccountName.Text = AccountName.Text.Substring(0, AccountName.Text.Length - 1);
                return;
            }
            if (AccountName.Text == string.Empty)
            {
                Growl.Clear();
                Growl.Warning("请输入账户名称");
                return;
            }
            if (e.Key == Key.Enter)
            {
                try
                {
                    UDataList.Add(new LocalDataList()
                    {
                        Name = AccountName.Text
                    });

                    YSAccount acct = YSAccount.ReadFromRegedit(true);
                    acct.Name = AccountName.Text;
                    acct.WriteToDisk();

                    key.sk("Account", AccountName.Text);

                    UChooseAccount.ItemsSource = UDataList;

                    AccountName.Visibility = Visibility.Hidden;
                    AccountName.Text = string.Empty;
                    InAdd = false;

                    RefreshList();

                    Growl.Clear();
                    Growl.Success($"添加成功 账户{AccountName.Text}");
                }
                catch (Exception ex)
                {
                    Growl.Clear();
                    Growl.Error(ex.Message);
                    log.ErrorLog(ex.Message, -1);
                }
            }
        }

        /// <summary>
        /// 赋值的异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UChooseAccount_DropDownClosed(object sender, EventArgs e)
        {
            if (UChooseAccount.Text != UAccountChange.Text)
            {
                Switch();
            }
        }

        /// <summary>
        /// 删除账户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelAccount_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("你确定要这么做？这是不可逆的操作", "防误触提醒", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
            {
                if (UChooseAccount.SelectedIndex > -1)
                {
                    string name = UChooseAccount.Text;
                    YSAccount.DeleteFromDisk(name);

                    UAccountChange.Text = string.Empty;

                    key.sk("Accout", string.Empty);

                    Growl.Clear();
                    Growl.Success($"您跟此账户说拜拜了 账户: {name}");

                    RefreshList();
                }
                else
                {
                    Growl.Clear();
                    Growl.Warning("您未选择某项");
                    return;
                }
            }
        }

        /// <summary>
        /// 修改账户名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!InAdd)
            {
                if (UChooseAccount.SelectedIndex > -1)
                {
                    ModifAccountName.Visibility = Visibility.Visible;
                    Growl.Clear();
                    Growl.Info("输入完成后按下回车确定, ESC退出");
                    InModify = true;
                }
                else
                {
                    Growl.Clear();
                    Growl.Warning("您未选择某项");
                    return;
                }
            }

        }

        /// <summary>
        /// ModifyAccountName TextBox回车判断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifAccountName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ModifAccountName.Text = string.Empty;
                ModifAccountName.Visibility = Visibility.Hidden;
                InModify = false;
                return;
            }
            if (ModifAccountName.Text.Length > 15)
            {
                Growl.Clear();
                Growl.Warning("名称最高只能有15个字符哦");
                ModifAccountName.Text = ModifAccountName.Text.Substring(0, ModifAccountName.Text.Length - 1);
                return;
            }
            if (ModifAccountName.Text == string.Empty)
            {
                Growl.Clear();
                Growl.Warning("请输入账户名称");
                return;
            }
            if (e.Key == Key.Enter)
            {
                try
                {
                    File.Move(UserData + "\\" + UAccountChange.Text, UserData + "\\" + ModifAccountName.Text);
                    key.sk("Account", ModifAccountName.Text);
                    UAccountChange.Text = ModifAccountName.Text;

                    RefreshList();

                    ModifAccountName.Text = string.Empty;
                    ModifAccountName.Visibility = Visibility.Hidden;

                    Growl.Clear();
                    Growl.Success("修改成功！");
                    InModify = false;
                }
                catch (Exception ex)
                {
                    Growl.Clear();
                    Growl.Error(ex.Message);
                    log.ErrorLog(ex.Message, -1);
                }
            }
        }

        /// <summary>
        /// 开始游戏的按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(UGenshinImpactPath.Text))
                {
                    Growl.Clear();
                    Growl.Error("没有找到你的原神 已经帮你打开启动器修复！");

                    DirectoryInfo info = new DirectoryInfo(UGenshinImpactPath.Text);
                    var LauncherPath = info.Parent.Parent.FullName;
                    if (File.Exists(LauncherPath + "\\launcher.exe"))
                    {
                        Process.Start(LauncherPath + "\\launcher.exe");
                        return;
                    }
                    Growl.Error("没有找到你的启动器");
                    return;
                }
                if (UGameStartModel.Text == "默认模式")
                {
                    StartGameNormal();
                    Growl.Clear();
                    Growl.Success("启动成功 祝你游玩愉快！");
                }
                else if (UGameStartModel.Text == "注入DLL")
                {
                    if (UGameService.Text == "国际服 | Global")
                    {
                        Growl.Clear();
                        Growl.Warning("由于支持方问题 ICora暂时不支持国际服Dll注入 望理解！\n游戏已正常启动");
                        StartGameNormal();
                        return;
                    }
                    string UPath = Environment.CurrentDirectory;
                    Regex regex = new Regex("[\u4e00-\u9fa5]+");
                    if (regex.IsMatch(UPath))
                    {
                        Growl.Clear();
                        Growl.Error($"我们发现了你的路径含有中文或非法字符 请你删除它后再试");
                        return;
                    }

                    if (UChooseDll.Text != "默认路径")
                    {
                        FileInfo UserChoosePath = new(UChooseDll.Text);
                        Regex r = new Regex("\\.(.*)$");
                        Match match = r.Match(UserChoosePath.Name);
                        if (match.Success)
                        {
                            string content = match.Groups[1].Value;
                            if (content == "exe"
                                || content == "lnk")
                            {
                                if (File.Exists(UChooseDll.Text))
                                {
                                    StartGameFormExe();
                                    Growl.Clear();
                                    Growl.Success("Game Started! Happy Hacking!");
                                    return;
                                }
                                else
                                {
                                    Growl.Clear();
                                    Growl.Error("没有找到你的文件");
                                    return;
                                }
                            }
                            if (content == "dll")
                            {
                                if (File.Exists(UChooseDll.Text))
                                {
                                    CLPath = UChooseDll.Text;
                                    StartGame();
                                    return;
                                }
                                else
                                {
                                    Growl.Clear();
                                    Growl.Error("没有找到你的文件");
                                    return;
                                }
                            }
                        }
                        Growl.Clear();
                        Growl.Error("未知问题");
                        return;
                    }
                    try
                    {

                        if (File.Exists(CLPath))
                        {
                            if (CheckCL.Content == "启用")
                            {
                                StartGame();
                                return;
                            }
                            // CL大小校验
                            FileInfo fileInfo = new FileInfo(CLPath);
                            long localsize = fileInfo.Length;
                            HttpWebRequest request = (HttpWebRequest)
                                WebRequest.Create("https://gitee.com/MasterGashByte/download/releases/download/CLibrary/CLibrary.dll");
                            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                            if (localsize != response.ContentLength)
                            {
                                if (MessageBox.Show(
                                    System.Windows.Window.GetWindow(this),
                                    "您的Dll大小不正确 这也许是服务器返回了不正确的大小 是否要尝试启动？选择否则下载服务器中的Dll"
                                    , "", MessageBoxButton.YesNoCancel, MessageBoxImage.Error) == MessageBoxResult.Yes)
                                {
                                    Growl.Clear();
                                    Growl.Warning("如果启动失败(原神未启动)则是Dll不正确导致的\n您可以下载服务器中的Dll或者下载群中的Akebi 然后手动导入Dll\n" +
                                        "您若需要手动导入Dll 需要将Dll放置到下面这个文件夹：\n" + AkebiPath);
                                    StartGame();
                                }
                                else
                                {
                                    Growl.Clear();
                                    Growl.Info("正在下载\n什么？没有反应 下拉开始按钮 点击下载Clibrary");
                                    File.Delete(CLPath);
                                    CLDownloader(true);
                                }
                            }
                            else
                            {
                                StartGame();
                            }
                        }
                        else
                        {
                            Growl.Clear();
                            Growl.Error("您的CLibrary不存在 正在为您下载");
                            CLDownloader(true);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Growl.Clear();
                        Growl.Warning("请以管理员模式打开ICora后再试");
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Clear();
                Growl.Error(ex.Message);
            }
        }

        /// <summary>
        /// 内置方法下载Clibrary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadClibrary_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("这也许会出现短暂的卡顿 确定要这么做？",
                "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (File.Exists(CLPath))
                    {
                        FileInfo fileInfo = new FileInfo(CLPath);
                        long localsize = fileInfo.Length;
                        HttpWebRequest request = WebRequest.CreateHttp(
                                    "https://gitee.com/MasterGashByte/download/releases/download/CLibrary/CLibrary.dll");
                        HttpWebResponse? response = request.GetResponse() as HttpWebResponse;

                        if (localsize != response.ContentLength)
                        {
                            CLDownloader(false);
                        }
                        else
                        {
                            Growl.Success("您的CLibrary看起来很正常");
                        }
                    }
                    else
                    {
                        CLDownloader(false);
                    }
                }
                catch (Exception ex)
                {
                    Growl.Error($"出现异常：{ex.Message}");
                    log.ErrorLog(ex.Message, -1);
                }
            }
        }

        /// <summary>
        /// 服务器选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UChooseService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                if ("当前服务器：" + UChooseService.SelectedItem == UGameService.Text)
                {
                    return;
                }
                try
                {
                    if (!Directory.Exists(GenshinServiceDir))
                    {
                        Growl.Clear();
                        Growl.Error("没有找到转服包资源 请前往群中连接下载\n下载后请保存至\n"
                            + Environment.CurrentDirectory + "\\GenshinService文件夹 \n已经为您打开");
                        Directory.CreateDirectory(GenshinServiceDir);
                        Process.Start("explorer.exe", Environment.CurrentDirectory + "\\GenshinService");
                        return;
                    }
                    #region 写入配置
                    DirectoryInfo info = new DirectoryInfo(UGenshinImpactPath.Text);
                    string YuanShenDir = info.Parent.FullName;
                    string configPath = YuanShenDir + "\\config.ini";
                    if (UChooseService.SelectedItem.ToString() == "官方服 | 天空岛")
                    {
                        if (MessageBox.Show("是否这么做？这样也许会导致ICora进入长时间的卡顿\n若出现无法打开原神 请去启动器校验文件完整性", ""
                            , MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
                        {
                            if (UGameService.Text == "当前服务器：国际服 | Global")
                            {
                                if (File.Exists(Environment.CurrentDirectory + "\\GenshinService\\Initial_file_v3.4.0.zip"))
                                {
                                    GameConverter game = new();
                                    game.Converter();
                                }
                                else
                                {
                                    Growl.Clear();
                                    Growl.Error("没有找到游戏转服资源包 请前往下载");
                                    return;
                                }
                            }

                            UGenshinImpactPath.Text = YuanShenDir + "\\YuanShen.exe";
                            ini.INIWrite("General", "channel", "1", configPath);
                            ini.INIWrite("General", "cps", "mihoyo", configPath);
                        }
                    }
                    if (UChooseService.SelectedItem.ToString() == "国际服 | Global")
                    {
                        if (MessageBox.Show("是否这么做？这样也许会导致ICora进入长时间的卡顿\n若出现无法打开原神 请去启动器校验文件完整性", ""
                            , MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
                        {
                            if (File.Exists(Environment.CurrentDirectory + "\\GenshinService\\Replace_file_v3.4.0.zip"))
                            {
                                GameConverter game = new();
                                game.Converter();
                            }
                            else
                            {
                                Growl.Clear();
                                Growl.Error("没有找到游戏转服资源包 请前往下载");
                                return;
                            }

                            ini.INIWrite("General", "channel", "1", configPath);
                            ini.INIWrite("General", "cps", "mihoyo", configPath);
                            UGenshinImpactPath.Text = YuanShenDir + "\\GenshinImpact.exe";
                        }
                    }
                    if (UChooseService.SelectedItem.ToString() == "渠道服 | 世界树")
                    {
                        if (MessageBox.Show("是否这么做？这样也许会导致ICora进入长时间的卡顿\n若出现无法打开原神 请去启动器校验文件完整性", ""
                            , MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
                        {
                            ini.INIWrite("General", "channel", "14", configPath);
                            ini.INIWrite("General", "cps", "bilibili", configPath);
                            UGenshinImpactPath.Text = YuanShenDir + "\\YuanShen.exe";
                            if (UGameService.Text == "当前服务器：国际服 | Global")
                            {
                                if (File.Exists(Environment.CurrentDirectory + "\\GenshinService\\Initial_file_v3.4.0.zip"))
                                {
                                    GameConverter game = new();
                                    game.Converter();
                                }
                                else
                                {
                                    Growl.Clear();
                                    Growl.Error("没有找到游戏转服资源包 请前往下载");
                                    return;
                                }
                            }
                            Growl.Clear();
                            Growl.Success("转服成功 当前服务器：渠道服 | 世界树\n若出现无法进入 还是官方服的问题 请反馈");
                        }
                    }
                    UGameService.Text = "当前服务器：" + UChooseService.SelectedItem;
                    #endregion
                }
                catch (DirectoryNotFoundException)
                {
                    Growl.Clear();
                    Growl.Warning("找不到游戏配置文件 config.ini");
                }
                catch (UnauthorizedAccessException)
                {
                    Growl.Clear();
                    Growl.Warning("无法读取或保存配置文件 请以管理员模式重启ICora然后重试");
                    Growl.Info("服务器未切换 当前服务器：" + UGameService.Text);
                }
            }
        }

        /// <summary>
        /// 选择Dll位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UChooseDllPath_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                // 文件后缀
                Filter = "(*.exe or *.dll or *.lnk) | *.exe;*.dll;*.lnk"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (openFileDialog.FileName == CLPath)
                {
                    UChooseDll.Text = "默认路径";
                    key.sk("UChooseDll", "默认路径");
                    return;
                }
                UChooseDll.Text = openFileDialog.FileName;
                key.sk("UChooseDll", UChooseDll.Text);
            }
        }

        #region 更多设置

        private void MultStart_Click(object sender, RoutedEventArgs e)
        {
            if (MultStart.Content.ToString() == "启用")
            {
                MultStart.Content = "已启用";
                key.sk("Mult Start", "True");
            }
            else
            {
                MultStart.Content = "启用";
                key.sk("Mult Start", "False");
            }
        }

        private bool _UIsFullScreen;
        private void UIsFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (_UIsFullScreen == false)
            {
                UIsFullScreen.Content = "已启用";
                key.sk("Is Full Screen", "True");
                _UIsFullScreen = true;
            }
            else
            {
                UIsFullScreen.Content = "启用";
                key.sk("Is Full Screen", "False");
                _UIsFullScreen = false;
            }
        }

        private bool _UBorderless;
        private void UBorderless_Click(object sender, RoutedEventArgs e)
        {
            if (_UBorderless == false)
            {
                UBorderless.Content = "已启用";
                key.sk("Border less", "True");
                _UBorderless = true;
            }
            else
            {
                UBorderless.Content = "启用";
                key.sk("Border less", "False");
                _UBorderless = false;
            }
        }

        private bool _UScreenWidthChanged;
        private void UScreenWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_UScreenWidthChanged)
            {
                if (UScreenWidth.Text != string.Empty)
                {
                    if (Regex.IsMatch(UScreenWidth.Text, @"^-?[1-9]\d*$|^0$"))
                    {
                        if (int.Parse(UScreenWidth.Text) < 10000)
                        {

                            key.sk("Screen Width", UScreenWidth.Text);
                            return;
                        }
                        else
                        {
                            Growl.Clear();
                            Growl.Warning("请输入正确的宽度");
                            UScreenWidth.Text = "1920";
                            return;
                        }
                    }
                    else
                    {
                        Growl.Clear();
                        Growl.Warning("请输入正确的宽度");
                        UScreenWidth.Text = string.Empty;
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            _UScreenWidthChanged = true;
        }

        private bool _UScreenHeightChanged;
        private void UScreenHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_UScreenHeightChanged)
            {
                if (UScreenHeight.Text != string.Empty)
                {
                    if (Regex.IsMatch(UScreenHeight.Text, @"^-?[1-9]\d*$|^0$"))
                    {
                        if (int.Parse(UScreenHeight.Text) < 10000)
                        {

                            key.sk("Screen Height", UScreenHeight.Text);
                            return;
                        }
                        else
                        {
                            Growl.Warning("请输入正确的宽度");
                            UScreenHeight.Text = "1080";
                            return;
                        }
                    }
                    else
                    {
                        Growl.Clear();
                        Growl.Warning("请输入正确的宽度");
                        UScreenHeight.Text = string.Empty;
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            _UScreenHeightChanged = true;
        }

        private bool _UCheckCL;
        private void CheckCL_Click(object sender, RoutedEventArgs e)
        {
            if (_UCheckCL == false)
            {
                CheckCL.Content = "启用";
                key.sk("Check CLibrary", "False");
            }
            else
            {
                CheckCL.Content = "已启用";
                key.sk("Check CLibrary", "True");
            }
        }

        #endregion

        #endregion
    }
}
