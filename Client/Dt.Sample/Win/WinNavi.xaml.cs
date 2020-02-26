﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class WinNavi : Win
    {
        public WinNavi()
        {
            InitializeComponent();
        }

        void OnNavi(object sender, RoutedEventArgs e)
        {
            NaviTo((string)((Button)sender).Content);
        }
    }
}