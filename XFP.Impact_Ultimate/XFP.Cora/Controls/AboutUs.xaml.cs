//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.

namespace XFP.ICora.Controls
{
    /// <summary>
    /// AboutUs.xaml 的交互逻辑
    /// </summary>
    public partial class AboutUs : UserControl
    {
        public AboutUs()
        {
            InitializeComponent();
        }

        private void GetICora_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://gitee.com/XFP-Group/Impact_Ultimate/");
        }

        private void GetICora_Github_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/GashByte/ICora");
        }

        private void ContentUs_Click(object sender, RoutedEventArgs e)
        {
            Growl.Info("加入群聊获取资讯！群聊QQ号已经被复制到粘贴板上咯");
            Clipboard.SetDataObject("811979687");
        }

        private void PrivacyPolicy_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://gitee.com/MasterGashByte/impact_ultimate_issues/blob/master/PrivacyPolicy.md");
        }

        private void UserUseAgreement_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://gitee.com/MasterGashByte/impact_ultimate_issues/blob/master/UserUseAgreement.md");
        }

        private void FeedBackQ_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://gitee.com/MasterGashByte/impact_ultimate_issues/issues");
        }

        private void ICoraIndex_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://gashbyte.github.io/ICoraIndex/");
        }

        private void afadian_Click(object sender, RoutedEventArgs e)
        {
            Growl.Clear();
            Growl.Success("唔噜！UwU!");
            Process.Start("explorer.exe", "https://afdian.net/a/XFP-Group");
        }

        private void GithubLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/GashByte/ICora");
        }

        private void VisitWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://gashbyte.github.io/ICoraIndex/");
        }
    }
}
