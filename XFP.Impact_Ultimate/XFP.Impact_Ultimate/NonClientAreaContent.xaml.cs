//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

using System;
using System.Windows.Media.Imaging;

namespace XFP.Impact_Ultimate.Controls
{
    /// <summary>
    /// NonClientAreaContent.xaml 的交互逻辑
    /// </summary>
    public partial class NonClientAreaContent
    {
        public NonClientAreaContent()
        {
            InitializeComponent();
            Icon.Source = new BitmapImage(new Uri("https://img.icons8.com/nolan/512/genshin-impact-logo.png"));
        }
    }
}
