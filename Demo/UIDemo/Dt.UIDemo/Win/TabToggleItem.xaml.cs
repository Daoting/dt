#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
#endregion

namespace Dt.UIDemo
{
    public partial class TabToggleItem : Tab
    {
        public TabToggleItem()
        {
            InitializeComponent();
            Title = "切换" + new Random().Next(100);
        }

        void OnToggle1(object sender, RoutedEventArgs e)
        {
            _win.LeftTab.Toggle(new TabToggleItem());
        }

        void OnBackToHome(object sender, RoutedEventArgs e)
        {
            _win.LeftTab.BackToHome();
        }

        void OnToggle2(object sender, RoutedEventArgs e)
        {
            _win.LeftTab.Toggle(new TabToggleItem(), true);
        }

        TabToggleItem _inst;
        void OnToggle3(object sender, RoutedEventArgs e)
        {
            if (_inst == null)
                _inst = new TabToggleItem();
            _win.LeftTab.Toggle(_inst);
        }

        TabNavi _win => (TabNavi)OwnWin;
    }
}