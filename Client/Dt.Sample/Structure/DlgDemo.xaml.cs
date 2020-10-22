﻿#region 文件描述
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

#endregion

namespace Dt.Sample
{
    public sealed partial class DlgDemo : Win
    {
        Random _rnd = new Random();
        Dlg _dlg;

        public DlgDemo()
        {
            InitializeComponent();

            Table tbl = new Table
            {
                { "Placement", typeof(DlgPlacement) },
                { "Target" },
                { "Pin", typeof(bool) },
                { "HideBar", typeof(bool) },
                { "Resize", typeof(bool) },
                { "ShowWinVeil", typeof(bool) },
            };
            _fv.Data = tbl.AddRow(new { Placement = DlgPlacement.CenterScreen });
            Closed += OnClosed;
        }

        void OnShow(object sender, RoutedEventArgs e)
        {
            GetDlg().Show();
        }

        async void OnShowAsync(object sender, RoutedEventArgs e)
        {
            var dlg = GetDlg();
            await dlg.ShowAsync();
            AtKit.Msg($"返回值：{dlg.Result}");
        }

        void OnShowPos(object sender, RoutedEventArgs e)
        {
            var dlg = GetDlg();
            if (AtSys.IsPhoneUI)
            {
                dlg.PhonePlacement = DlgPlacement.CenterScreen;
                dlg.Top = _rnd.Next(0, 500);
                dlg.Left = _rnd.Next(0, 400);
            }
            else
            {
                dlg.WinPlacement = DlgPlacement.CenterScreen;
                dlg.Top = _rnd.Next(0, 1000);
                dlg.Left = _rnd.Next(0, 800);
            }
            dlg.Show();
        }

        async void OnClicked1(object sender, RoutedEventArgs e)
        {
            if (await AtKit.Confirm("确认要删除所有数据吗?"))
                AtKit.Msg("Yes");
            else
                AtKit.Msg("No");
        }

        void OnClicked2(object sender, RoutedEventArgs e)
        {
            AtKit.Error("错误消息内容！");
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
            if (_dlg == null)
            {
                _dlg = new Dlg
                {
                    Background = null,
                    Resizeable = false,
                    HideTitleBar = true,
                    BorderThickness = new Thickness(0),
                    PhonePlacement = DlgPlacement.CenterScreen,
                    WinPlacement = DlgPlacement.CenterScreen,
                };

                // 通过data描述的方式构建发杂path的方式如下行注释掉的方式调用
                //Path path = AtRes.ParsePath("M30,15 C30,23.2843 23.2843,30 15,30 C6.71573,30 0,23.2843 0,15 C0,6.71573 6.71573,0 15,0 C23.2843,0 30,6.71573 30,15 z");
                Ellipse path = new Ellipse
                {
                    Height = 200,
                    Width = 140,
                    StrokeThickness = 4,
                    IsHitTestVisible = true,
                    Fill = new SolidColorBrush(Colors.Green),
                    Stroke = new SolidColorBrush(Colors.Red),
                };
                _dlg.Content = path;
            }
            _dlg.Show();
        }

        void OnNaviClick(object sender, RoutedEventArgs e)
        {
            Dlg dlg = new Dlg();
            dlg.Content = new TabNav1();
            if (!AtSys.IsPhoneUI)
            {
                dlg.Width = 300;
                dlg.Height = 300;
            }
            dlg.Show();
        }

        void OnClosed(object sender, EventArgs e)
        {
            if (_dlg != null)
                _dlg.Close();
        }

        Dlg1 GetDlg()
        {
            Dlg1 dlg = new Dlg1();
            Row row = _fv.Row;
            var placement = (DlgPlacement)row["Placement"];
            if (AtSys.IsPhoneUI)
                dlg.PhonePlacement = placement;
            else
                dlg.WinPlacement = placement;
            dlg.PlacementTarget = _tgt;
            dlg.IsPinned = row.Bool("Pin");
            dlg.HideTitleBar = row.Bool("HideBar");
            dlg.Resizeable = row.Bool("Resize");
            dlg.ShowWinVeil = row.Bool("ShowWinVeil");
            return dlg;
        }
    }
}
