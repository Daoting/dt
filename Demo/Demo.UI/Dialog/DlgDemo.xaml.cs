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

namespace Demo.UI
{
    public sealed partial class DlgDemo : Win
    {
        Random _rnd = new Random();

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
                { "ShowVeil", typeof(bool) },
                { "CanClose", typeof(bool) },
                { "AllowRelayPress", typeof(bool) },
            };
            _fv.Data = tbl.AddRow(new { Placement = DlgPlacement.CenterScreen, Resize = true, CanClose = true, AllowRelayPress = true });
        }

        void OnShow(object sender, RoutedEventArgs e)
        {
            GetDlg().Show();
        }

        async void OnShowAsync(object sender, RoutedEventArgs e)
        {
            var dlg = GetDlg();
            await dlg.ShowAsync();
            Kit.Msg($"返回值：{dlg.Result}");
        }

        void OnShowPos(object sender, RoutedEventArgs e)
        {
            var dlg = GetDlg();
            if (Kit.IsPhoneUI)
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

        Dlg1 GetDlg()
        {
            var dlg = new Dlg1();
            Row row = _fv.Row;
            var placement = (DlgPlacement)row["Placement"];
            if (Kit.IsPhoneUI)
                dlg.PhonePlacement = placement;
            else
                dlg.WinPlacement = placement;

            if (placement == DlgPlacement.TargetOverlap)
                dlg.PlacementTarget = _fv;
            else
                dlg.PlacementTarget = _tgt;
            dlg.IsPinned = row.Bool("Pin");
            dlg.HideTitleBar = row.Bool("HideBar");
            dlg.Resizeable = row.Bool("Resize");
            dlg.ShowVeil = row.Bool("ShowVeil");
            dlg.CanClose = row.Bool("CanClose");
            dlg.AllowRelayPress = row.Bool("AllowRelayPress");
            return dlg;
        }
    }
}
