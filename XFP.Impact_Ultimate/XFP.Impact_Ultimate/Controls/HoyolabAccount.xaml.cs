//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using XFP.Impact_Ultimate.Hoyolab.Account;
using XFP.Impact_Ultimate.Utils;
using MessageBox = System.Windows.MessageBox;

namespace XFP.Impact_Ultimate.Controls
{
    /// <summary>
    /// HoyolabAccount.xaml 的交互逻辑
    /// </summary>
    public partial class HoyolabAccount
    {

        DataProvider data = new();

        public HoyolabAccount()
        {
            InitializeComponent();
        }

        #region Normal Method

        #endregion

        #region Control Method

        private void UChooseAccount_DropDownClosed(object sender, EventArgs e)
        {
            if (UChooseAccount.SelectedIndex > -1)
            {
                UChooseHoyolabAccount.Text = "当前账户：" + UChooseAccount.Text;
                UChooseHoyolabAccount_Copy.Text = UChooseHoyolabAccount.Text;
            }
            if (UChooseAccount.Text == string.Empty)
            {
                UChooseHoyolabAccount.Text = "当前账户：未添加(选择)账户";
                UChooseHoyolabAccount_Copy.Text = UChooseHoyolabAccount.Text;
            }
        }

        private void RefreshAccountList_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
