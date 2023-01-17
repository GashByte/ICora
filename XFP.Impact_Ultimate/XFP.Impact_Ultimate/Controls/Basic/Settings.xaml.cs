//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.

using HandyControl.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using XFP.Impact_Ultimate.Utlis;

namespace XFP.Impact_Ultimate.Controls.Basic
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings() => InitializeComponent();

        private void FeedBackUs_Click(object sender, RoutedEventArgs e) 
            => Process.Start("explorer.exe", "https://gitee.com/MasterGashByte/impact_ultimate_issues/issues");

        private void ReadUUA_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", "https://gitee.com/MasterGashByte/impact_ultimate_issues/blob/master/UserUseAgreement.md");

        private void ReadPP_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", "https://gitee.com/MasterGashByte/impact_ultimate_issues/blob/master/PrivacyPolicy.md");

        private void CheckVersion_Click(object sender, RoutedEventArgs e)
        {
            var LocalVersion = new GetFormUrl().Get("https://gitee.com/MasterGashByte/updates/raw/master/Checker/Version");
            if (LocalVersion != new DataProvider().Version)
            {
                Growl.Clear();
                Growl.Warning($"当前版本：{new DataProvider().Version}\n发行版本：{LocalVersion}\n请前往官网或QQ群聊获取最新版\n您也可以手动打开Updater.exe来自动下载最新版");
            }
            else
            {
                Growl.Clear();
                Growl.Success($"当前版本：{new DataProvider().Version}\n发行版本：{LocalVersion}\n已经是最新版本啦！");
            }
        }
    }
}
