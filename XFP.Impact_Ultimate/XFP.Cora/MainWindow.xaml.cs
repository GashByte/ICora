//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License. 

namespace XFP.ICora
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
            if (key.GetValue("UserAgree") == null)
            {
                if (HandyControl.Controls.MessageBox.Show("你好旅行者！欢迎来到ICora\nICora是一款免费使用的原神工具箱 如果您是付费买来的 您已经被骗了\n由于ICora内置Dll注入的模块 所以可能会遭受到一定程度的倒卖 如果您是免费使用的 不妨查看一下下面的内容\nICora提供服务的QQ群：\n\t原神交流一群 811979687 || 原神交流二群 590566763\nICora用户隐私协议：https://gitee.com/MasterGashByte/impact_ultimate_issues/blob/master/PrivacyPolicy.md\nICora用户使用协议：https://gitee.com/MasterGashByte/impact_ultimate_issues/blob/master/UserUseAgreement.md\n点击 [是] 则代表同意以上条款并继续使用ICora\n\t\t(此弹窗不会弹出第二次)", "欢迎你 旅行者！", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    Environment.Exit(0);
                }
                else
                {
                    key.SetValue("UserAgree", "True");
                }
            }

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
                Name = "角色练度",
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
                Name = "米哈游账户",
                GroupName = ControlType.更多功能.ToString(),
            });
            MenuList.Add(new MenuInfo()
            {
                Name = "自定义便签",
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
                Name = "友商论坛",
                GroupName = ControlType.关于.ToString(),
            });
            #endregion

            #region other
            settingsMenu.Add(new MenuInfo()
            {
                Name = "设置",
            });
            settingsMenu.Add(new MenuInfo()
            {
                Name = "登录",
            });
            #endregion

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
                                Growl.InfoGlobal($"ICora找到新版本啦！\n发现新版本啦，快去更新吧~(版本号：{ServerVesion})");
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
                    case "友商论坛":
                        Growl.Clear();
                        Growl.SuccessGlobal("即将访问入梦论坛~");
                        Process.Start("explorer.exe", "https://www.rmlt.xyz/");
                        break;
                    case "关于我们":
                        ControlPanel.Content = new AboutUs();
                        break;
                    case "祈愿记录":
                        ControlPanel.Content = new WishExport();
                        break;
                    case "自定义便签":
                        ControlPanel.Content = new CustomNote();
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
