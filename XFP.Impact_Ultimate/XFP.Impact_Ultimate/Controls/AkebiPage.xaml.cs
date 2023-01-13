using HandyControl.Controls;
using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Windows.Devices.HumanInterfaceDevice;
using XFP.Impact_Ultimate.Controls.Game.Utils;
using XFP.Impact_Ultimate.Model;
using XFP.Impact_Ultimate.Utils;
using XFP.Impact_Ultimate.Utlis;
using XFP.Impact_Ultimate.Utlis.Log;
using XFP.Impact_Ultimate.Utlis.Model.Files;
using ZdfFlatUI;
using MessageBox = HandyControl.Controls.MessageBox;
using Path = System.IO.Path;

namespace XFP.Impact_Ultimate.Controls
{
    /// <summary>
    /// AkebiPage.xaml 的交互逻辑
    /// </summary>
    public partial class AkebiPage
    {
        #region 实例化
        KeySetter key = new();
        LogWriter log = new();
        INIFiles ini = new();
        #endregion

        #region define
        private string CLPath = Environment.CurrentDirectory + "\\Impact_Ultimate\\CLibrary.dll";
        private string AkebiPath = Environment.CurrentDirectory + "\\Impact_Ultimate";
        private string UserData = Environment.CurrentDirectory + "\\UserData";
        private string GenshinServiceDir = Environment.CurrentDirectory + "\\GenshinService";
        #endregion

        /// <summary>
        /// 构建开始游戏页面
        /// </summary>
        public AkebiPage()
        {
            InitializeComponent();

            #region 文件夹判断
            if (!Directory.Exists(AkebiPath))
                Directory.CreateDirectory(AkebiPath);
            if(!Directory.Exists(UserData))
                Directory.CreateDirectory(UserData);
            #endregion
            // 服务器选择的List
            List<string> GenshinService = new List<string>
            {
                "官方服 | 天空岛",
                "渠道服 | 世界树",
                "国际服 | Global" 
            };

            UChooseService.ItemsSource = GenshinService;

            #region 基础设置
            try
            {
                #region 读取配置文件
                UGenshinImpactPath.Text = key.gk("Genshin Impact Path") == string.Empty ? "没有找到你的原神" : key.gk("Genshin Impact Path");
                UScreenWidth.Text = key.gk("Screen Width") == string.Empty ? "1920" : key.gk("Screen Width");
                UScreenHeight.Text = key.gk("Screen Height") == string.Empty ? "1080" : key.gk("Screen Height");

                if (key.gk("Start Game Model") == "注入DLL")
                {
                    UGameStartModel.Text = "注入DLL";
                    UStartModel.Content = "注入DLL";
                }
                else
                {
                    UGameStartModel.Text = "默认模式";
                    UStartModel.Content = "默认模式";
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
                    UBorderless.Content = "已启用";
                else
                    UBorderless.Content = "启用";
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
            #endregion
        }

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
            }
            else
            {
                UStartModel.Content = "默认模式";
            }
            UGameStartModel.Text = UStartModel.Content.ToString();
            key.sk("Start Game Model", UGameStartModel.Text);
        }

        private void ChooseAccount_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 开始游戏的按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (UGameStartModel.Text == "默认模式")
            {
                StartGameNomal();
                Growl.Success("启动成功 祝你游玩愉快！");
            }
            else if (UGameStartModel.Text == "注入DLL")
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
                        if (ZMessageBox.Show(
                            System.Windows.Window.GetWindow(this),
                            "您的Dll大小不正确 这也许是服务器返回了不正确的大小 是否要尝试启动？选择否则下载服务器中的Dll"
                            , "", MessageBoxButton.YesNoCancel, EnumPromptType.Error) == MessageBoxResult.Yes)
                        {
                            Growl.Warning("如果启动失败(原神未启动)则是Dll不正确导致的\n您可以下载服务器中的Dll或者下载群中的Akebi 然后手动导入Dll\n" +
                                "您若需要手动导入Dll 需要将Dll放置到下面这个文件夹：\n" + AkebiPath);
                            StartGame();
                        }
                        else
                        {
                            Growl.Info("正在下载");
                            File.Delete(CLPath);
                            CLDownloader();
                        }
                    }
                    else
                    {
                        StartGame();
                    }
                }
                else
                {
                    Growl.Error("您的CLibrary不存在 正在为您下载");
                    CLDownloader();
                }
            }
        }

        /// <summary>
        /// 启动游戏
        /// </summary>
        public void StartGame()
        {
            if (string.IsNullOrEmpty(UGenshinImpactPath.Text))
            {
                Growl.Error("您的原神路径是空的");
            }
            else
            {
                if (YuanShenIsRunning() && MultStart.Content.ToString() == "启用")
                {
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
                        Growl.Warning("提权失败 正常启动");
                        StartGameNomal();
                        return;
                    }
                    var pExporer = Process.GetProcessesByName("explorer")[0];
                    if (pExporer == null)
                    {
                        Growl.Warning("Explorer未找到 正常启动");
                        StartGameNomal();
                        return;
                    }
                    IntPtr handle = DllUtils.OpenProcess(0xF0000 | 0x100000 | 0xFFFF, false, (uint)pExporer.Id);
                    var lpSize = IntPtr.Zero;
                    DllUtils.InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref lpSize);
                    si.lpAttributeList = Marshal.AllocHGlobal(lpSize);
                    DllUtils.InitializeProcThreadAttributeList(si.lpAttributeList, 1, 0, ref lpSize);
                    if (DllUtils.UpdateProcThreadAttribute(si.lpAttributeList, 0, (IntPtr)0x00020004, handle, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero))
                    {
                        Growl.Error("更新线程失败");
                    }
                    DirectoryInfo path = new DirectoryInfo(UGenshinImpactPath.Text);
                    var pi = new DllUtils.PROCESS_INFORMATION();
                    var result = DllUtils.CreateProcessAsUser(hToken, Path.Combine(UGenshinImpactPath.Text).ToString(),
                        RunParameters.Text, IntPtr.Zero, IntPtr.Zero, false, 0x00080000 | 0x00000004,
                        IntPtr.Zero, Path.Combine(path.Parent.Parent.FullName, "Genshin Impact Game").ToString(), ref si.StartupInfo, out pi);
                    if (!result)
                    {
                        Growl.Warning("启动暂停线程失败 正常启动");
                        StartGameNomal();
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
                    Growl.Success("启动成功 祝你游玩愉快！");
                }
                catch
                {

                }
            }
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
        private void CLDownloader()
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

        /// <summary>
        /// 正常启动游戏
        /// </summary>
        private void StartGameNomal()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Path.Combine(UGenshinImpactPath.Text);
            if (key.gk("Is Full Screen") == "False"
                && key.gk("Border less") == "False")
            {
                string args = new CommandLineBuilder()
                    .Append("-screen-height", UScreenHeight.Text.ToString())
                    .Append("-screen-width", UScreenWidth.Text.ToString())
                    .ToString();
                startInfo.Arguments = RunParameters.Text + args;
            }
            else
            {
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
                startInfo.Arguments = args;
            }
            startInfo.Verb = "runas";
            Process.Start(startInfo);
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

        private bool _UIsFullScreen = false;
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

        private bool _UBorderless = false;
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

        private void UScreenWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UScreenWidth.Text != string.Empty)
            {
                if (int.Parse(UScreenWidth.Text) < 10000)
                {
                    key.sk("Screen Width", UScreenWidth.Text);
                    return;
                }
                else
                {
                    Growl.Warning("请输入正确的宽度");
                    UScreenWidth.Text = "1920";
                    return;
                }
            }
            else
            {
                UScreenWidth.Text = "1920";
                return;
            }
        }

        private void UScreenHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UScreenHeight.Text != string.Empty)
            {
                if (int.Parse(UScreenHeight.Text) < 10000)
                {
                    key.sk("Screen Height", UScreenHeight.Text);
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
                UScreenHeight.Text = "1080";
                return;
            }
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
                    if (UChooseService.SelectedItem == "官方服 | 天空岛")
                    {
                        if (MessageBox.Show("是否这么做？这样也许会导致ICora进入长时间的卡顿\n若出现无法打开原神 请去启动器校验文件完整性", ""
                            , MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
                        {
                            if (UGameService.Text == "当前服务器：国际服 | Global")
                            {
                                GameConverter game = new();
                                game.Converter();
                            }

                            UGenshinImpactPath.Text = YuanShenDir + "\\YuanShen.exe";
                            ini.INIWrite("General", "channel", "1", configPath);
                            ini.INIWrite("General", "cps", "mihoyo", configPath);
                        }
                    }
                    if (UChooseService.SelectedItem == "国际服 | Global")
                    {
                        if (MessageBox.Show("是否这么做？这样也许会导致ICora进入长时间的卡顿\n若出现无法打开原神 请去启动器校验文件完整性", ""
                            , MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
                        {
                            GameConverter game = new();
                            game.Converter();

                            ini.INIWrite("General", "channel", "1", configPath);
                            ini.INIWrite("General", "cps", "mihoyo", configPath);
                            UGenshinImpactPath.Text = YuanShenDir + "\\GenshinImpact.exe";
                        }
                    }
                    if (UChooseService.SelectedItem == "渠道服 | 世界树")
                    {
                        if (MessageBox.Show("是否这么做？这样也许会导致ICora进入长时间的卡顿\n若出现无法打开原神 请去启动器校验文件完整性", ""
                            , MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
                        {
                            ini.INIWrite("General", "channel", "14", configPath);
                            ini.INIWrite("General", "cps", "bilibili", configPath);
                            UGenshinImpactPath.Text = YuanShenDir + "\\YuanShen.exe";
                            if (UGameService.Text == "当前服务器：国际服 | Global")
                            {
                                GameConverter game = new();
                                game.Converter();
                            }
                            Growl.Success("转服成功 当前服务器：渠道服 | 世界树\n若出现无法进入 还是官方服的问题 请反馈");
                        }
                    }
                    UGameService.Text = "当前服务器：" + UChooseService.SelectedItem;
                    #endregion
                }
                catch (DirectoryNotFoundException)
                {
                    Growl.Warning("找不到游戏配置文件 config.ini");
                }
                catch (UnauthorizedAccessException)
                {
                    Growl.Warning("无法读取或保存配置文件 请以管理员模式重启ICora然后重试");
                    Growl.Info("服务器未切换 当前服务器：" + UGameService.Text);
                }
            }
        }
    }
}
