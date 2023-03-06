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
    public partial class TabNaviItem : Tab
    {
        public TabNaviItem()
        {
            InitializeComponent();
        }

        protected override void OnFirstLoaded()
        {
            if (NaviParams != null)
                Kit.Msg($"输入参数：{NaviParams}");

            Result = new Random().Next(100);
            Title = "Tab " + Result.ToString();
        }

        async void OnForward(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            object input = (bool)_cbInput.IsChecked ? (object)rand.Next(1000) : null;
            var tab = new TabNaviItem();
            if ((bool)_cbHideTitle.IsChecked)
                tab.HideTitleBar = true;

            if ((bool)_cbResult.IsChecked)
            {
                var ret = await Forward<string>(tab, input, (bool)_cbModal.IsChecked);
                Kit.Msg($"返回参数：{ret}");
            }
            else
            {
                Forward(tab, input, (bool)_cbModal.IsChecked);
            }
        }

        void OnBackward(object sender, RoutedEventArgs e)
        {
            Backward();
        }

        void OnBackToHome(object sender, RoutedEventArgs e)
        {
            BackToHome();
        }

        void OnToggle(object sender, RoutedEventArgs e)
        {
            Toggle(new TabNaviItem());
        }

        void OnSingle(object sender, RoutedEventArgs e)
        {
            var dlg = new Dlg { IsPinned = true, Title = "内嵌Tab" };
            if ((bool)_cbDlgTitle.IsChecked)
                dlg.HideTitleBar = true;
            dlg.LoadTab(new TabNaviItem());
            dlg.Show();
        }

        void OnMulti(object sender, RoutedEventArgs e)
        {
            var dlg = new Dlg { Title = "内嵌多个Tab" };
            if ((bool)_cbDlgTitle.IsChecked)
                dlg.HideTitleBar = true;

            if (!Kit.IsPhoneUI)
            {
                dlg.Width = Kit.ViewWidth / 4;
                dlg.Height = Kit.ViewHeight - 100;
            }
            dlg.LoadTabs(new List<Tab>
            { 
                new TabNaviItem(),
                new TabNaviItem(),
                new Tab { Title = "内嵌3", Content = new TextBlock { Text = "标准Tab", Margin = new Thickness(10) }},
            });
            dlg.Show();
        }
    }
}