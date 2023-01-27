//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License. 

using System.Threading.Tasks;

namespace XFP.ICora.Controls.Basic
{
    /// <summary>
    /// LoginFormMihoyo.xaml 的交互逻辑
    /// </summary>

    public partial class LoginFormMihoyo : UserControl
    {
        DataProvider data = new();

        public LoginFormMihoyo()
        {
            InitializeComponent();
            Loaded += LoginFormMihoyo_Loaded;

            Growl.Clear();
            Growl.Info("由于存储方式的问题 目前只支持单次存储 所以目前只能保存一个米哈游账户 我们会想办法解决这个问题的！");

            if (!Directory.Exists(data.HoyolabAccountData))
            {
                Directory.CreateDirectory(data.HoyolabAccountData);
            }
        }

        private async void LoginFormMihoyo_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _WebView2.EnsureCoreWebView2Async();
                var manager = _WebView2.CoreWebView2.CookieManager;
                var cookies = await manager.GetCookiesAsync("https://user.mihoyo.com/#/login/captcha");
                foreach (var item in cookies)
                {
                    manager.DeleteCookie(item);
                }
                _WebView2.CoreWebView2.Navigate("https://user.mihoyo.com/#/login/captcha");
            }
            catch (Exception ex)
            {
                Growl.Error($"初始化页面失败：{ex.Message}");
            }
        }

        private async void Logined_Click(object sender, RoutedEventArgs e)
        {
            await FinishAndAddCookie().ConfigureAwait(false);
        }

        public async Task FinishAndAddCookie(string str = null, bool FormHoyolabAccount = false)
        {
            try
            {
                if (!FormHoyolabAccount)
                {
                    var manager = _WebView2?.CoreWebView2.CookieManager;
                    var cookies = await manager.GetCookiesAsync("https://user.mihoyo.com/#/account/home");
                    str = string.Join(";", cookies.Select(x => $"{x.Name}={x.Value}"));
                    Growl.Success("正在验证Cookie");

                    if (str == string.Empty)
                    {
                        Growl.Clear();
                        Growl.Error("你的cookie为空 请重新验证");
                        return;
                    }
                }

                _WebView2.Source = new Uri("https://gashbyte.github.io/ICoraIndex");

                var user = await new HoyolabClient().GetHoyolabUserInfoAsync(str);
                var roles = await new HoyolabClient().GetGenshinRoleInfoListAsync(str);

                HoyolabUserInfo hoyolabUserInfo = user;
                GenshinRoleInfo genshinRoleInfo = roles.FirstOrDefault();

                string userMaskId = user.Uid.ToString().Substring(0, 3)
                    + "***" + user.Uid.ToString().Substring(user.Uid.ToString().Length - 3);
                string RoleMaskId = genshinRoleInfo.Uid.ToString().Substring(0, 3)
                    + "***" + genshinRoleInfo.Uid.ToString().Substring(genshinRoleInfo.Uid.ToString().Length - 3);

                Growl.Clear();
                Growl.Success($"载入成功！\n米游社通行证ID: {userMaskId}\n玩家Uid: {RoleMaskId}");

                Properties.Settings.Default.UserCookie
                    = genshinRoleInfo.Cookie;
                Properties.Settings.Default.LastUid
                    = user.Uid;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();

                if (FormHoyolabAccount)
                {
                    new HoyolabAccount().InitializeUserInfo();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                {
                    Growl.Error("无法验证此Cookie 请重新登录");
                    /*
                    Properties.Settings.Default.UserCookie
                        = string.Empty;
                    Properties.Settings.Default.LastUid
                        = 0;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Upgrade();
                    */
                    _WebView2.Source = new Uri("https://user.mihoyo.com/#/login/captcha");
                    return;
                }
                Growl.Error(ex.Message);

                _WebView2.Source = new Uri("https://user.mihoyo.com/#/login/captcha");
            }
        }
    }
}
