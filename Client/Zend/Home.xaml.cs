#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Zend
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Home : NaviWin
    {
        public Home()
        {
            InitializeComponent();
        }

        void OnNavi(object sender, RoutedEventArgs e)
        {
            IWin win = ((BtnItem)sender).GetClsObj() as IWin;
            if (win != null)
                LoadWin(win);
        }

        void OnNaviData(object sender, RoutedEventArgs e)
        {
            //NaviData = new List<NaviRow>
            //{
            //    new NaviRow(Icons.备注, "普通窗口", typeof(WinDemo), "右侧加载普通窗口内容"),
            //    new NaviRow(Icons.个人信息, "页面窗口", typeof(PageWinDemo), "右侧加载页面窗口内容"),
            //};
        }
    }
}