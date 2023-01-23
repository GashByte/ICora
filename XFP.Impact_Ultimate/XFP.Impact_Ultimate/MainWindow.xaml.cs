//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License. 

using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UI;
using XFP.Impact_Ultimate.Controls;
using XFP.Impact_Ultimate.Model;
using XFP.Impact_Ultimate.Utils;
using XFP.Impact_Ultimate.Utils.Log;
using MessageBox = System.Windows.MessageBox;

/*
Impact_Ultimate Beta Source
 */

namespace XFP.Impact_Ultimate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        #region initialization

        DataProvider data = new();
        LogWriter log = new();
        GetFormUrl GetFormUrl = new GetFormUrl();
        Notifiaction notifiaction = new();

        #endregion

        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software", true).CreateSubKey("Impact_Ultimate");

        private ObservableCollection<MenuInfo> _MenuList;
        public ObservableCollection<MenuInfo> MenuList
        {
            get { return _MenuList; }
            set { _MenuList = value; }
        }

        private ObservableCollection<MenuInfo> _settingsMenu;
        public ObservableCollection<MenuInfo> settingsMenu
        {
            get { return _settingsMenu; }
            set { _settingsMenu = value; }
        }

        public MainWindow() => Initialization();

        private void Initialization()
        {
            InitializeComponent();

            // 初始化数据
            new DataProvider();
            new DataProvider().Initialize();

            // 窗口居中
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // 用于绘制标题栏
            NonClientAreaContent = new NonClientAreaContent();

            // 实例化
            MenuList = new ObservableCollection<MenuInfo>();
            settingsMenu = new ObservableCollection<MenuInfo>();

            #region BackGround
            // 通过笔刷进行绘制背景图片 后期用户可以自定义背景图片
            //if (File.Exists(Environment.CurrentDirectory + "\\bg\\BackGround.png"))
            //{
            //    ImageBrush b = new ImageBrush();
            //    b.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\bg\\BackGround.png"));
            //    b.Stretch = Stretch.Fill;
            //    Background = b;
            //}
            #endregion

            #region Basic
            MenuList.Add(new MenuInfo()
            {
                Name = "主页",
                GroupName = ControlType.基础功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "开始游戏",
                GroupName = ControlType.基础功能.ToString(),
            });
            #endregion

            #region Form
            MenuList.Add(new MenuInfo()
            {
                Name = "祈愿记录",
                GroupName = ControlType.更多功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "螺旋深渊",
                GroupName = ControlType.更多功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "养成计划",
                GroupName = ControlType.更多功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "成就管理",
                GroupName = ControlType.更多功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "活动公告",
                GroupName = ControlType.更多功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "图鉴管理",
                GroupName = ControlType.更多功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "过场动画",
                GroupName = ControlType.更多功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "游戏公告",
                GroupName = ControlType.更多功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "实时便签",
                GroupName = ControlType.更多功能.ToString(),
            });
            #endregion

            #region View
            MenuList.Add(new MenuInfo()
            {
                Name = "关于我们",
                GroupName = ControlType.关于.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "米哈游账户",
                GroupName = ControlType.关于.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "论坛",
                GroupName = ControlType.关于.ToString(),
            });
            #endregion

            settingsMenu.Add(new MenuInfo()
            {
                Name = "设置",
            });
            settingsMenu.Add(new MenuInfo()
            {
                Name = "登录",
            });

            menu.ItemsSource = MenuList;
            SettingsMenu.ItemsSource = settingsMenu;

            ControlPanel.Content = new HomePage();

            menu.SelectionChanged += menu_SelectionChanged;
            SettingsMenu.SelectionChanged += SettingsMenu_SelectionChanged;

            Growl.SuccessGlobal("岁月不居 时节如流 我们再一次迎来了新的一年\n我代表XFP全体团员 在此祝您\n兔年大吉！心想事成！万事如意！财源广进！四季平安！");

            new Thread(() =>
            {
                bool _Info = false;
                var _ServerVesion = string.Empty;
                while (true)
                {
                    var ServerVesion =
                        GetFormUrl.Get("https://gitee.com/MasterGashByte/updates/raw/master/Checker/Version");
                    var version = new DataProvider().Version;
                    if (version != ServerVesion)
                    {
                        var MustUpdate =
                            GetFormUrl.Get("https://gitee.com/MasterGashByte/updates/raw/master/Checker/MustUpdate");
                        if (MustUpdate == "true")
                        {
                            MessageBox.Show("这是一个必须更新的版本 正在打开 [更新助手]");

                            var localpath = Environment.CurrentDirectory;
                            if (File.Exists(localpath + "\\Updater.exe")
                                && File.Exists(localpath + "\\Updater.deps.json")
                                && File.Exists(localpath + "\\Updater.dll")
                                && File.Exists(localpath + "\\Updater.runtimeconfig.json"))
                            {
                                Process.Start(localpath + "\\Updater.exe");
                            }
                            else
                            {
                                MessageBox.Show("您的ICora仿佛不齐全 请前往群中重新下载");
                                log.ErrorLog("DetectionSystem: ICora is incomplete", -0, "您的ICora不是完整的 您可以前往群中获取完整的ICora");
                            }

                            Environment.Exit(0);
                        }


                        if (_ServerVesion != ServerVesion)
                        {
                            if (!_Info)
                            {
                                Growl.InfoGlobal("有新版本！\n发现新版本啦，快去更新吧~");
                                _ServerVesion = ServerVesion;
                                _Info = true;
                            }
                        }
                        _Info = false;
                    }
                    Thread.Sleep(1800000);
                }
            }).Start();
        }

        private void SettingsMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettingsMenu.SelectedItem != null)
            {
                menu.SelectedItem = null;
                MenuInfo info = SettingsMenu.SelectedItem as MenuInfo;
                switch (info.Name)
                {
                    case "设置":
                        ControlPanel.Content = new Controls.Basic.Settings();
                        break;
                    case "登录":
                        ControlPanel.Content = new Controls.Basic.LoginFormMihoyo();
                        break;
                }
            }
        }

        private void menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (menu.SelectedItem != null)
            {
                SettingsMenu.SelectedItem = null;
                MenuInfo info = menu.SelectedItem as MenuInfo;
                switch (info.Name)
                {
                    case "主页":
                        ControlPanel.Content = new HomePage();
                        break;
                    case "开始游戏":
                        ControlPanel.Content = new AkebiPage();
                        break;
                    case "论坛":
                        Process.Start("explorer.exe", "https://www.rmlt.xyz/");
                        break;
                    case "关于我们":
                        ControlPanel.Content = new AboutUs();
                        break;
                    case "祈愿记录":
                        Growl.Clear();
                        Growl.Warning("此按钮将在下一个版本开放~我们已经编写完成咯！");
                        break;
                    case "米哈游账户":
                        ControlPanel.Content = new HoyolabAccount();
                        break;
                    default:
                        ControlPanel.Content = new HomePage();
                        Growl.Clear();
                        Growl.Warning("暂未开发\n请期待下个版本");
                        break;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Environment.Exit(0);
            base.OnClosed(e);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var localpath = Environment.CurrentDirectory;
            if (File.Exists(localpath + "\\Updater.exe")
                && File.Exists(localpath + "\\Updater.deps.json")
                && File.Exists(localpath + "\\Updater.dll")
                && File.Exists(localpath + "\\Updater.runtimeconfig.json"))
            {
                Process.Start("explorer.exe", localpath + "\\Updater.exe");
                Environment.Exit(0);
            }
            else
            {
                Growl.Error("您的ICora仿佛不完整");
                log.ErrorLog("DetectionSystem: Impact_Ultimate is incomplete", -0, "您的Impact_Ultimate不是完整的 您可以前往群中获取完整的Impact_Ultiamte");
            }
        }
    }

    public enum ControlType
    {
        基础功能,
        更多功能,
        关于,
    }
}
