#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;

#endregion

namespace Dt.UIDemo
{
    public sealed partial class SysDlgDemo : Win
    {
        public SysDlgDemo()
        {
            InitializeComponent();
        }

        async void OnClicked1(object sender, RoutedEventArgs e)
        {
            if (await Kit.Confirm("确认要删除所有数据吗?"))
                Kit.Msg("Yes");
            else
                Kit.Msg("No");
        }

        void OnClicked2(object sender, RoutedEventArgs e)
        {
            Kit.Error("错误消息内容！");
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_tb.Text.Contains("4"))
                _tb.Warn("文本框内容不可以含有4！");
        }

        void OnMsgClick(object sender, RoutedEventArgs e)
        {
            _btn.Msg("在目标元素上部显示提示信息，提示信息内容过长时换行");
        }

        void OnClicked4(object sender, RoutedEventArgs e)
        {
            var dlg = new Dlg
            {
                Background = null,
                Resizeable = false,
                HideTitleBar = true,
                BorderThickness = new Thickness(0),
                PhonePlacement = DlgPlacement.CenterScreen,
                WinPlacement = DlgPlacement.CenterScreen,
            };

            // 通过data描述的方式构建发杂path的方式如下行注释掉的方式调用
            //Path path = Res.ParsePath("M30,15 C30,23.2843 23.2843,30 15,30 C6.71573,30 0,23.2843 0,15 C0,6.71573 6.71573,0 15,0 C23.2843,0 30,6.71573 30,15 z");
            Ellipse path = new Ellipse
            {
                Height = 200,
                Width = 140,
                StrokeThickness = 4,
                IsHitTestVisible = true,
                Fill = new SolidColorBrush(Colors.Green),
                Stroke = new SolidColorBrush(Colors.Red),
            };
            dlg.Content = path;
            dlg.Show();
        }

        void OnSingle(object sender, RoutedEventArgs e)
        {
            var dlg = new Dlg { IsPinned = true, Title = "内嵌Tab" };
            dlg.LoadTab(new TabNaviItem());
            dlg.Show();
        }

        void OnMulti(object sender, RoutedEventArgs e)
        {
            var dlg = new Dlg { Title = "内嵌多个Tab" };
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
