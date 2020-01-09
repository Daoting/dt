#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 我的
    /// </summary>
    public sealed partial class MyMain : UserControl
    {
        public MyMain()
        {
            InitializeComponent();
        }

        void OnEditInfo(object sender, RoutedEventArgs e)
        {
            AtApp.OpenWin(typeof(MyInfoEdit));
        }

        void OnFollow(object sender, RoutedEventArgs e)
        {
            AtApp.OpenWin(typeof(FollowList));
        }

        void OnOrder(object sender, RoutedEventArgs e)
        {
            AtApp.OpenWin(typeof(OrderList));
        }

        void OnAddress(object sender, RoutedEventArgs e)
        {
            AtApp.OpenWin(typeof(AddressList));
        }

        void OnInit(object sender, RoutedEventArgs e)
        {
            AtLocal.DeleteCookie("ShowGuide");
        }
    }
}
