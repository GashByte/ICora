//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.

using HandyControl.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using XFP.Impact_Ultimate.Utils;
using MessageBox = HandyControl.Controls.MessageBox;

namespace XFP.Impact_Ultimate.Controls.Basic
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
            FileInfo fi = new FileInfo(Environment.CurrentDirectory + "\\XFP.Impact_Ultimate.exe");
            try
            {
                UserICoraID.Text = "ICoraID" + getMNum() + " 设备ID在用户绑定数据库数据时起到重要关键";
                ICoraVersion.Text = new DataProvider().ModeVersion + " V " + new DataProvider().Version;
                LastUpdate.Text = fi.LastWriteTime.ToString();
            }
            catch { }
        }

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

        private void CheckICoraID_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"您的设备ID{getMNum()}");
        }

        /// <summary>
        /// 取得设备硬盘的卷标号
        /// </summary>
        /// <returns></returns>
        public string GetDiskVolumeSerialNumber()
        {
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        /// <summary>
        /// 获得CPU的序列号
        /// </summary>
        /// <returns></returns>
        public string getCpu()
        {
            string strCpu = null;
            ManagementClass myCpu = new("win32_Processor");
            ManagementObjectCollection myCpuConnection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuConnection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
                break;
            }
            return strCpu;
        }

        /// <summary>
        /// 生成机器码
        /// </summary>
        /// <returns></returns>
        public string getMNum()
        {
            string strNum = getCpu() + GetDiskVolumeSerialNumber();
            string strMNum = strNum.Substring(0, 24);
            return strMNum;
        }

        private void JoinDevPlan_Click(object sender, RoutedEventArgs e)
            => Growl.Info("更多咨询请联系作者或访问主页加入QQ群聊\nHomePage : https://gashbyte.github.io/ICoraIndex");

        private void CopyICoraID_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(getMNum());
            Growl.Clear();
            Growl.Success("复制成功");
        }
    }
}
