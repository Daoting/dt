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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

#endregion

namespace Dt.Sample
{
    public sealed partial class DlgDemo : PageWin
    {
        Random _rnd = new Random();
        Dlg _dlg;
        public DlgDemo()
        {
            InitializeComponent();

            _cbPlacement.ItemsSource = EnumDataSource.FromType<DlgPlacement>();
            if (AtSys.IsPhoneUI)
                _cbPlacement.SelectedIndex = 1;
            else
                _cbPlacement.SelectedIndex = 0;

            Closed += OnClosed;
            GenShapeOps();
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
            //GetDlg().ShowAt(_rnd.Next(0, 1000), _rnd.Next(0, 800));
        }

        async void OnClicked1(object sender, RoutedEventArgs e)
        {
            if (await AtKit.Confirm("确认?"))
                AtKit.Msg("Yes");
            else
                AtKit.Msg("No");
        }

        void OnClicked2(object sender, RoutedEventArgs e)
        {
            AtKit.Error("错误消息内容！");
        }

        async void OnClicked3(object sender, RoutedEventArgs e)
        {
            string result = await AtKit.Option("选择哪项？", "选项一", "选项二", "选项三", "选项四", "选项五");
            AtKit.Msg(result);
        }

        async void OnClicked4(object sender, RoutedEventArgs e)
        {
            if (_dlg == null)
                CreateDlg();

            await _dlg.ShowAsync();
        }

        void OnTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            _dlg.Close();
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dlg == null) return;
            Path path = (Path)_dlg.Content;
            if (path == null) return;
            path.Data = GenPathData();
        }

        void OnClosed(object sender, EventArgs e)
        {
            if (_dlg != null)
                _dlg.Close();
        }

        Dlg1 GetDlg()
        {
            Dlg1 dlg = new Dlg1();
            var placement = (DlgPlacement)((EnumMember)_cbPlacement.SelectedItem).Value;
            if (AtSys.IsPhoneUI)
                dlg.PhonePlacement = placement;
            else
                dlg.WinPlacement = placement;
            dlg.PlacementTarget = _tbTarget;
            if ((bool)_cbPin.IsChecked)
                dlg.IsPinned = true;
            if ((bool)_cbHideBar.IsChecked)
                dlg.HideTitleBar = true;
            if ((bool)_cbResize.IsChecked)
                dlg.Resizeable = false;
            return dlg;
        }

        void GenShapeOps()
        {
            List<string> shape = new List<string> { "方形", "椭圆", };

            _shapes.ItemsSource = shape;
            _shapes.SelectedIndex = 0;
            _shapes.SelectionChanged += OnSelectionChanged;
        }

        Geometry GenPathData()
        {
            if (_shapes.SelectedIndex == 0)
            {
                return new RectangleGeometry { Rect = new Windows.Foundation.Rect { Width = 160, Height = 100 } };
            }
            else
            {
                return new EllipseGeometry { RadiusX = 70, RadiusY = 100 };
            }
        }

        void CreateDlg()
        {
            _dlg = new Dlg
            {
                IsPinned = true,
                Background = null,
                Resizeable = false,
                HideTitleBar = true,
                BorderThickness = new Thickness(0),
                PhonePlacement = DlgPlacement.CenterScreen,
                WinPlacement = DlgPlacement.CenterScreen,
            };

            // 通过data描述的方式构建发杂path的方式如下行注释掉的方式调用
            //Path path = AtRes.ParsePath("M30,15 C30,23.2843 23.2843,30 15,30 C6.71573,30 0,23.2843 0,15 C0,6.71573 6.71573,0 15,0 C23.2843,0 30,6.71573 30,15 z");
            Path path = new Path
            {
                Data = GenPathData(),
                StrokeThickness = 4,
                IsHitTestVisible = true,
                Fill = new SolidColorBrush(Colors.Green),
                Stroke = new SolidColorBrush(Colors.Red),
            };

            path.Tapped += OnTapped;
            _dlg.Content = path;
        }
    }
}
