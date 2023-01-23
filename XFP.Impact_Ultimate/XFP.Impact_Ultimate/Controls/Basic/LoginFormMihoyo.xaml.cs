//Copyright (c) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT License. 

using HandyControl.Controls;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using XFP.Impact_Ultimate.Hoyolab;
using XFP.Impact_Ultimate.Hoyolab.Account;
using XFP.Impact_Ultimate.Utils;
using XFP.Impact_Ultimate.Utlis.Model.Files;
using MessageBox = System.Windows.MessageBox;

namespace XFP.Impact_Ultimate.Controls.Basic
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

        private void Logined_Click(object sender, RoutedEventArgs e)
        {
            FinishAndAddCookie();
        }

        private async Task FinishAndAddCookie()
        {
            try
            {
                var manager = _WebView2?.CoreWebView2.CookieManager;
                var cookies = await manager.GetCookiesAsync("https://user.mihoyo.com/#/account/home");
                var str = string.Join(";", cookies.Select(x => $"{x.Name}={x.Value}"));
                Growl.Success("正在验证Cookie");

                if (str == string.Empty)
                {
                    Growl.Clear();
                    Growl.Error("你的cookie为空 请重新验证");
                    return;
                }

                _WebView2.Source = new Uri("https://gashbyte.github.io/ICoraIndex");

                var user = await new HoyolabClient().GetHoyolabUserInfoAsync(str);
                var roles = await new HoyolabClient().GetGenshinRoleInfoListAsync(str);

                HoyolabUserInfo? hoyolabUserInfo = user;
                GenshinRoleInfo? genshinRoleInfo = roles.FirstOrDefault();

                string userMaskId = user.Uid.ToString().Substring(0, 3)
                    + "***" + user.Uid.ToString().Substring(user.Uid.ToString().Length - 3);
                string RoleMaskId = genshinRoleInfo.Uid.ToString().Substring(0, 3)
                    + "***" + genshinRoleInfo.Uid.ToString().Substring(genshinRoleInfo.Uid.ToString().Length - 3);

                Growl.Clear();
                Growl.Success($"载入成功！\n米游社通行证ID: {userMaskId}\n玩家Uid: {RoleMaskId}");

                if (!Directory.Exists(data.HoyolabAccountData + $"\\{user.Nickname}"))
                {
                    Directory.CreateDirectory(data.HoyolabAccountData + $"\\{user.Nickname}");
                }
                if (File.Exists(data.HoyolabAccountData + $"\\{user.Nickname}\\{user.Nickname}.Data"))
                {
                    File.Delete(data.HoyolabAccountData + $"\\{user.Nickname}\\{user.Nickname}.Data");
                }
                File.Create(data.HoyolabAccountData + $"\\{user.Nickname}\\{user.Nickname}.Data").Close();

                IWrapper wrapper = new IWrapper()
                {
                    Uid = genshinRoleInfo.Uid.ToString(),
                    Cookie = genshinRoleInfo.Cookie.ToString(),
                    RegionName = genshinRoleInfo.RegionName,
                };

                var Data = new DocumentSerializer().Serializer(wrapper);

                using (StreamWriter sw = new(
                    data.HoyolabAccountData +
                    $"\\{user.Nickname}\\{user.Nickname}.Data", false))
                {
                    sw.WriteLine(Data);
                }
            }
            catch (Exception ex)
            {

                if (ex.Message == "Object reference not set to an instance of an object")
                {
                    Growl.Error("没有抓到你的cookie 请确保登录了");
                    _WebView2.Source = new Uri("https://user.mihoyo.com/#/login/captcha");
                    return;
                }
                Growl.Error(ex.Message);

                _WebView2.Source = new Uri("https://user.mihoyo.com/#/login/captcha");
            }
        }
    }
}
