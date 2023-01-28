//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.

using GalaSoft.MvvmLight.Messaging;
using HandyControl.Properties.Langs;
using System.Windows.Media;

namespace XFP.ICora.Controls
{
    public class HoyolabUid
    {
        public string Uid { get; set; }
    }

    /// <summary>
    /// HoyolabAccount.xaml 的交互逻辑
    /// </summary>

    public partial class HoyolabAccount
    {
        public HoyolabUserInfo? HoyolabUserInfo;
        public GenshinRoleInfo? GenshinRoleInfo;

        public string UserCookie = Properties.Settings.Default.UserCookie;
        DataProvider data = new();

        public ObservableCollection<HoyolabUid> HoyolabLists { get; } = new ObservableCollection<HoyolabUid>();

        public HoyolabAccount()
        {
            InitializeComponent();

            UHeaderImage.Source = new BitmapImage(new Uri("https://th.bing.com/th/id/R.3a6f44192394c3b9e5b68ed21d2e1795?rik=ZEfzfyiGXR1yKw&pid=ImgRaw&r=0"));
            GetAreaImage();

            if (UserCookie != string.Empty)
            {
                InitializeUserInfo();
                HoyolabLoadingBorder.Visibility = Visibility.Visible;
                DoubleAnimation daV = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.6)));
                HoyolabLoadingBorder.BeginAnimation(OpacityProperty, daV);
            }
            else
            {
                Initialized();
            }

            UChooseAccount.ItemsSource = HoyolabLists;
        }

        #region Normal Method

        /// <summary>
        /// 载入用户数据
        /// </summary>
        public async void InitializeUserInfo()
        {
            try
            {
                var user = await new HoyolabClient().GetHoyolabUserInfoAsync(UserCookie);
                var role = await new HoyolabClient().GetGenshinRoleInfoListAsync(UserCookie);
                GenshinRoleInfo? genshinRoleInfo = role.FirstOrDefault();

                UHeaderImage.Source = new BitmapImage(new Uri(user.AvatarUrl));
                UserHoyolabName.Text = $"你好! {user.Nickname}";
                string RoleMaskId = genshinRoleInfo.Uid.ToString().Substring(0, 3)
                    + "***" + genshinRoleInfo.Uid.ToString().Substring(genshinRoleInfo.Uid.ToString().Length - 3);
                UserHoyolabUid.Text = $"Uid {RoleMaskId}";
                UserGenshinServer.Text = $"您在 {genshinRoleInfo.RegionName} ";
                UserGenshinLevel.Text = $"您 {genshinRoleInfo.Level} 级了";
                if (user.Pendant != string.Empty)
                {
                    HeadFrame.Source = new BitmapImage(new Uri(user.Pendant));
                }
                Intr.Text = $"简介：{user.Introduce}";
                UHoyolabAccount.Text = $"嗨！别来无恙啊！{user.Nickname} 最近生活是否如意？";
                InputCookie.IsEnabled = false;

                Properties.Settings.Default.UserCookie = genshinRoleInfo.Cookie;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();

                GetDailyNotesAsync();

                HoyolabLists.Clear();
                HoyolabLists.Add(new HoyolabUid()
                {
                    Uid = RoleMaskId
                });

                GetGameRecordByInitialze(genshinRoleInfo, RoleMaskId);

                try
                {
                    var SginInfo = await new HoyolabClient().GetSignInInfoAsync(genshinRoleInfo);
                    if (SginInfo.IsSub)
                    {
                        Growl.Clear();
                        Growl.Info("旅行者今天还没有签到哦~ 已经帮你签到了哦~");
                        await new HoyolabClient().SignInAsync(genshinRoleInfo);
                    }
                }
                catch (Exception ex)
                {
                    Growl.Error($"在签到时 {ex.Message}");
                    Initialized();
                }
            }
            catch (Exception ex)
            {
                Growl.Clear();
                if (ex.Message == "Object reference not set to an instance of an object.")
                {
                    Growl.Clear();
                    Growl.Error("米游社凭证过期 正在尝试刷新");
                    await new LoginFormMihoyo().FinishAndAddCookie(UserCookie, true);
                    Initialized();
                    return;
                }
                if (ex.Message == "The format of value '' is invalid.")
                {
                    Growl.Clear();
                    Growl.Error("没有找到有效的Cookie");
                    return;
                }
                Growl.Error($"在请求数据时出错：{ex.Message}");
                Initialized();
            }
        }

        /// <summary>
        /// 获取玩家当前Uid的所有战绩
        /// </summary>
        private async void GetGameRecordByInitialze(GenshinRoleInfo info, string uid)
        {
            try
            {
                var roleGameRecord = await new HoyolabClient().GetGameRecordAsync(info);
                Days.Text = $"{roleGameRecord.PlayerStat.ActiveDayNumber}天";
                Achievements.Text = $"{roleGameRecord.PlayerStat.AchievementNumber}个";
                RolesCount.Text = $"{roleGameRecord.PlayerStat.AvatarNumber}个";
                TPPoints.Text = $"{roleGameRecord.PlayerStat.WayPointNumber}个";
                AOcuil.Text = $"{roleGameRecord.PlayerStat.AnemoculusNumber}";
                GOcuil.Text = $"{roleGameRecord.PlayerStat.GeoculusNumber}";
                EOcuil.Text = $"{roleGameRecord.PlayerStat.ElectroculusNumber}";
                DOcuil.Text = $"{roleGameRecord.PlayerStat.DendroculusNumber}";
                Domain.Text = $"{roleGameRecord.PlayerStat.DomainNumber}个";
                SpiralAbyss.Text = $"{roleGameRecord.PlayerStat.SpiralAbyss}";
                Luxurious.Text = $"{roleGameRecord.PlayerStat.LuxuriousChestNumber}";
                Precious.Text = $"{roleGameRecord.PlayerStat.PreciousChestNumber}";
                Exquisite.Text = $"{roleGameRecord.PlayerStat.ExquisiteChestNumber}";
                Common.Text = $"{roleGameRecord.PlayerStat.CommonChestNumber}";
                Remarkable.Text = $"{roleGameRecord.PlayerStat.MagicChestNumber}";

                MondE.Text = $"{roleGameRecord.WorldExplorations[7].ExplorationPercentage.ToString().
                    Insert(roleGameRecord.WorldExplorations[7].ExplorationPercentage.ToString().Length - 1, ".")}%";
                LiyueE.Text = $"{roleGameRecord.WorldExplorations[6].ExplorationPercentage.ToString().
                    Insert(roleGameRecord.WorldExplorations[6].ExplorationPercentage.ToString().Length - 1, ".")}%";
                DragonspineE.Text = $"{roleGameRecord.WorldExplorations[5].ExplorationPercentage.ToString().
                    Insert(roleGameRecord.WorldExplorations[5].ExplorationPercentage.ToString().Length - 1, ".")}%";
                DaoqiE.Text = $"{roleGameRecord.WorldExplorations[4].ExplorationPercentage.ToString().
                    Insert(roleGameRecord.WorldExplorations[4].ExplorationPercentage.ToString().Length - 1, ".")}%";
                EnkanomiyaE.Text = $"{roleGameRecord.WorldExplorations[3].ExplorationPercentage.ToString().
                    Insert(roleGameRecord.WorldExplorations[3].ExplorationPercentage.ToString().Length - 1, ".")}%";
                ChasmE.Text = $"{roleGameRecord.WorldExplorations[2].ExplorationPercentage.ToString().
                    Insert(roleGameRecord.WorldExplorations[2].ExplorationPercentage.ToString().Length - 1, ".")}%";
                Chasm_UndergroundE.Text = $"{roleGameRecord.WorldExplorations[1].ExplorationPercentage.ToString().
                    Insert(roleGameRecord.WorldExplorations[1].ExplorationPercentage.ToString().Length - 1, ".")}%";
                XumiE.Text = $"{roleGameRecord.WorldExplorations[0].ExplorationPercentage.ToString().
                    Insert(roleGameRecord.WorldExplorations[0].ExplorationPercentage.ToString().Length - 1, ".")}%";
            }
            catch (Exception ex)
            {
                Growl.Clear();
                Growl.Error($"在获取用户战绩时出错 {ex.Message}");
                Initialized();
            }
        }

        /// <summary>
        /// 获取图标
        /// </summary>
        public async void GetAreaImage()
        {
            try
            {
                MondeIcon.Source = new BitmapImage(new Uri("https://upload-bbs.mihoyo.com/game_record/genshin/city_icon/UI_ChapterIcon_Mengde.png"));
                LiyueIcon.Source = new BitmapImage(new Uri("https://upload-bbs.mihoyo.com/game_record/genshin/city_icon/UI_ChapterIcon_Liyue.png"));
                DaoqiIcon.Source = new BitmapImage(new Uri("https://upload-bbs.mihoyo.com/game_record/genshin/city_icon/UI_ChapterIcon_Daoqi.png"));
                XumiIcon.Source = new BitmapImage(new Uri("https://upload-bbs.mihoyo.com/game_record/genshin/city_icon/UI_ChapterIcon_Xumi.png"));
                Dragonspine.Source = new BitmapImage(new Uri("https://upload-bbs.mihoyo.com/game_record/genshin/city_icon/UI_ChapterIcon_Dragonspine.png"));
                Enkanomiya.Source = new BitmapImage(new Uri("https://upload-bbs.mihoyo.com/game_record/genshin/city_icon/UI_ChapterIcon_Enkanomiya.png"));
                Chasm.Source = new BitmapImage(new Uri("https://upload-bbs.mihoyo.com/game_record/genshin/city_icon/UI_ChapterIcon_ChasmsMaw.png"));
                Chasm_Underground.Source = new BitmapImage(new Uri("https://upload-bbs.mihoyo.com/game_record/genshin/city_icon/UI_ChapterIcon_ChasmsMaw.png"));
            }
            catch (Exception ex)
            {
                Growl.Clear();
                Growl.Error($"在获取图片时出错 {ex.Message}");
                Initialized();
            }
        }

        /// <summary>
        /// 实时便签
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        public async void GetDailyNotesAsync()
        {
            try
            {
                var users = await new HoyolabClient().GetHoyolabUserInfoAsync(UserCookie);
                var roles = await new HoyolabClient().GetGenshinRoleInfoListAsync(UserCookie);
                foreach (var role in roles)
                {
                    try
                    {
                        var dailynote = await new HoyolabClient().GetDailyNoteAsync(role);
                        var travelnote = await new HoyolabClient().GetTravelNotesSummaryAsync(role);

                        try
                        {
                            int FinishedCount = 0;
                            string TipText = "";
                            Dictionary<string, Image> ImageValue = new Dictionary<string, Image>
                            {
                                { "I", AvatarSideIconI },
                                { "II", AvatarSideIconII },
                                { "III", AvatarSideIconIII },
                                { "IV", AvatarSideIconIV },
                                { "V", AvatarSideIconV }
                            };
                            Dictionary<string, TextBlock> ExpeditionsTime = new Dictionary<string, TextBlock>
                            {
                                { "I", RemainedTimeI },
                                { "II", RemainedTimeII },
                                { "III", RemainedTimeIII },
                                { "IV", RemainedTimeIV },
                                { "V", RemainedTimeV }
                            };

                            int Expeditions = dailynote.CurrentExpeditionNumber;
                            if (Expeditions != 0)
                            {
                                foreach (string key in ImageValue.Keys)
                                {
                                    if (dailynote.Expeditions[FinishedCount].AvatarSideIcon != null)
                                    { 
                                        Image avatarsideicon = ImageValue[key];
                                        avatarsideicon.Source = new BitmapImage(new Uri(dailynote.Expeditions[FinishedCount].AvatarSideIcon));
                                        TextBlock text = ExpeditionsTime[key];
                                        if (dailynote.Expeditions[FinishedCount].Status == "Ongoing")
                                        {
                                            text.Text = dailynote.Expeditions[FinishedCount].FinishedTime.ToString();
                                        }
                                        else
                                        {
                                            text.Text = "已完成";
                                        }
                                    }
                                    FinishedCount++;
                                }

                                if (FinishedCount != dailynote.CurrentExpeditionNumber)
                                {
                                    ExpeditionsStatus.Text = $"派遣完成度({FinishedCount}/{dailynote.CurrentExpeditionNumber})";
                                }
                                else
                                {
                                    ExpeditionsStatus.Text = "派遣已完成！";
                                    TipText += "派遣已完成 快点领取奖励吧！\n";
                                }
                            }

                            ResinCurrent.Text = $"{dailynote.CurrentResin}/{dailynote.MaxResin}";
                            if (dailynote.CurrentResin == dailynote.MaxResin)
                            {
                                TipText += "体力已满\n";
                            }
                            HomeCoinCurrent.Text = $"{dailynote.CurrentHomeCoin}/{dailynote.MaxHomeCoin}";
                            if (dailynote.CurrentHomeCoin == dailynote.MaxHomeCoin)
                            {
                                TipText += "洞天宝钱已满\n";
                            }
                            DailyTaskCurrent.Text = $"{dailynote.FinishedTaskNumber}/4";
                            if (dailynote.FinishedTaskNumber == 0)
                            {
                                TipText += "今日委托未刷\n";
                            }
                            ResinDiscountCurrent.Text = $"{dailynote.ResinDiscountLimitedNumber}/3";
                            if (dailynote.ResinDiscountLimitedNumber == 3)
                            {
                                TipText += "本周体力减半未使用\n";
                            }
                            if (dailynote.Transformer.RecoveryTime.Day != 0)
                            {
                                TransformerCurrent.Text = $"还有 {dailynote.Transformer.RecoveryTime.Day} 天";
                            }
                            else
                            {
                                TransformerCurrent.Text = $"已冷却";
                                TipText += "参量质变仪已冷却\n";
                            }
                            Growl.Clear();
                            Growl.Success(TipText + "快上线原神来玩玩吧！");
                            CurrentPrimogems.Text = travelnote.MonthData.LastPrimogems.ToString();
                            CurrentMora.Text = travelnote.DayData.LastMora.ToString();
                            Initialized(Properties.Settings.Default.LastUid.ToString());
                        }
                        catch {  }
                    }
                    catch (Exception ex)
                    {
                        Growl.Clear();
                        Growl.Warning("在获取实时数据时 " + ex.Message + " 请前往手机端米游社手动刷新实时便签");
                        PaimonEmo.Visibility = Visibility.Visible;
                        Initialized();
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Clear();
                Growl.Error("在获取实时数据时 " + ex.Message);
                Initialized();
            }
        }

        /// <summary>
        /// 加载结束动画
        /// </summary>
        /// <param name="Uid"></param>
        public new void Initialized(string Uid = "未添加(选择)账户")
        {
            DoubleAnimation daV = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(1.3)));
            HoyolabLoadingBorder.BeginAnimation(OpacityProperty, daV);
            daV.Completed += (sender, e) => { /*Skip Animation*/ };
            HoyolabLoadingBorder.IsHitTestVisible = false;

            UChooseHoyolabAccount.Text = "当前账户：" + Uid;

            Growl.Success("刷新成功!");
        }


        #endregion

        #region Control Method

        private void UChooseAccount_DropDownClosed(object sender, EventArgs e)
        {
            if (UChooseAccount.SelectedIndex > -1)
            {
                UChooseHoyolabAccount.Text = "当前账户：" + UChooseAccount.Text;
            }
            if (UChooseAccount.Text == string.Empty)
            {
                UChooseHoyolabAccount.Text = "当前账户：未添加(选择)账户";
            }
        }

        /// <summary>
        /// 重新请求内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshAccountList_Click(object sender, RoutedEventArgs e)
        {
            RefreshAll();
        }

        private async void RefreshAll()
        {
            try
            {
                DoubleAnimation daV = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.6)));
                HoyolabLoadingBorder.BeginAnimation(OpacityProperty, daV);
                await new LoginFormMihoyo().FinishAndAddCookie(UserCookie, true);
            }
            catch (Exception ex)
            {
                Growl.Clear();
                Growl.Error($"{ex.Message}");
                Initialized();
            }
        }

        /// <summary>
        /// 复制Cookie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyCookie_Click(object sender, RoutedEventArgs e)
        {
            if (!(UChooseAccount.SelectedIndex > -1))
            {
                Growl.Clear();
                Growl.Warning("您没有选择一个账户");
                return;
            }
            if (Properties.Settings.Default.UserCookie == string.Empty)
            {
                Growl.Clear();
                Growl.Warning($"{UChooseAccount.SelectedItem} 账户的Cookie已失效或不存在 请重新登录");
                return;
            }
            Clipboard.SetDataObject(Properties.Settings.Default.UserCookie);
            Growl.Clear();
            Growl.Success("复制成功!");
        }

        /// <summary>
        /// 删除此账户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelHoyolabAcc_Click(object sender, RoutedEventArgs e)
        {
            if (!(UChooseAccount.SelectedIndex > -1))
            {
                Growl.Clear();
                Growl.Warning("您没有选择一个账户");
                return;
            }
            if (MessageBox.Show(Window.GetWindow(this),
                "您确定要这么做？\n这是不可逆的操作", "", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
            {
                Properties.Settings.Default.UserCookie = string.Empty;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();
                Growl.Clear();
                Growl.Success("删除成功!\n请重新刷新");
                InputCookie.IsEnabled = true;
                UChooseHoyolabAccount.Text = "当前账户：未添加(选择)账户";

                new MainWindow().ControlPanel = new HomePage();
            }
        }

        /// <summary>
        /// 手动输入Cookie按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputCookie_Click(object sender, RoutedEventArgs e)
        {
            InputUCookie.Visibility = Visibility.Visible;
            InputUserCookie.Visibility = Visibility.Visible;
            Growl.Clear();
            Growl.Info("请输入带有cookie_token_v2的Cookie\n按下ESC退出输入 回车键确定");
        }

        /// <summary>
        /// 按键检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputUserCookie_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                InputUCookie.Visibility = Visibility.Hidden;
                InputUserCookie.Visibility = Visibility.Hidden;
                InputUserCookie.Text = string.Empty;
                return;
            }
            if (e.Key == Key.Enter)
            {
                InputUCookie.Visibility = Visibility.Hidden;
                InputUserCookie.Visibility = Visibility.Hidden;
                if (InputUserCookie.Text != string.Empty)
                {
                    Properties.Settings.Default.UserCookie = InputUserCookie.Text;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Upgrade();
                    InputUserCookie.Text = string.Empty;
                    Growl.Clear();
                    Growl.Success("正在验证Cookie");
                    new LoginFormMihoyo().FinishAndAddCookie(Properties.Settings.Default.UserCookie, true);
                    return;
                }
                else
                {
                    Growl.Clear();
                    Growl.Warning("您没有输入Cookie\n按下ESC退出输入");
                    InputUCookie.Visibility = Visibility.Hidden;
                    InputUserCookie.Visibility = Visibility.Hidden;
                    InputUserCookie.Text = string.Empty;
                    return;
                }
            }
        }

        public record GameRecordItem
        {
            public GameRecordItem(string description, string value)
            {
                Description = description;
                Value = value;
            }

            public string Description { get; set; }

            public string Value { get; set; }
        }

        #endregion
    }
}
