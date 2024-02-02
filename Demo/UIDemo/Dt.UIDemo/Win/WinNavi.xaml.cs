#region 文件描述
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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
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

        void OnClosWin(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}