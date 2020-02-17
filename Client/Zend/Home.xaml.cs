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
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Zend
{
    [View("主页")]
    public partial class Home : NaviWin
    {
        public Home()
        {
            InitializeComponent();
            RefreshInfo();
        }

        void OnNavi(object sender, RoutedEventArgs e)
        {
            IWin win = ((BtnItem)sender).GetClsObj() as IWin;
            if (win != null)
                LoadWin(win);
        }

        void RefreshInfo()
        {
            _btnPwd.Desc = AtLocal.GetScalar<string>("select count(1) from pwdlog");
        }

        void OnRefresh(object sender, Mi e)
        {
            RefreshInfo();
        }
    }
}