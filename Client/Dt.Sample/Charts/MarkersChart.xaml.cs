#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Charts;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Sample
{
    public partial class MarkersChart : Win
    {
        public MarkersChart()
        {
            InitializeComponent();
            ChartSampleData.CreateData(_chart);
            _chart.ManipulationMode = ManipulationModes.All;

            var pnl = new ChartPanel();
            var vmarker = CreateMarker(false);
            pnl.Children.Add(vmarker);
            vmarker.Action = ChartPanelAction.MouseMove;

            var hmarker = CreateMarker(true);
            pnl.Children.Add(hmarker);
            hmarker.Action = ChartPanelAction.MouseMove;

            _chart.View.Layers.Add(pnl);

            _cbAttach.ItemsSource = EnumDataSource.FromType<ChartPanelAttach>();
            _cbAttach.SelectedIndex = 0;
            _cbAttach.SelectionChanged += (s, e) =>
            {
                hmarker.Attach = vmarker.Attach =
                  (ChartPanelAttach)((EnumMember)_cbAttach.SelectedItem).Value;
            };

            _cbType.ItemsSource = new List<string> { "Column", "LineSymbols"};
            _cbType.SelectionChanged += (s, e) =>
            {
                _chart.ChartType = (ChartType)Enum.Parse(typeof(ChartType), _cbType.SelectedItem.ToString(), false);
            };
            _cbType.SelectedIndex = 0;
        }

        ChartPanelObject CreateMarker(bool isHorizontal)
        {
            var obj = new ChartPanelObject();
            var bdr = new Border()
            {
                BorderBrush = Background = new SolidColorBrush(Colors.Red),
                Padding = new Thickness(2),
            };
            var tb = new TextBlock() { Foreground = new SolidColorBrush(Colors.Black) };
            var bind = new Binding();
            bind.Source = obj;

            if (isHorizontal)
            {
                bdr.BorderThickness = new Thickness(0, 2, 0, 0);
                bdr.Margin = new Thickness(0, -1, 0, 0);
                obj.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                obj.DataPoint = new Point(double.NaN, 0.5);
                //bind.StringFormat = "y={0:#.#}";
                bind.Path = new PropertyPath("DataPoint");
            }
            else
            {
                bdr.BorderThickness = new Thickness(2, 0, 0, 0);
                bdr.Margin = new Thickness(-1, 0, 0, 0);
                obj.VerticalContentAlignment = VerticalAlignment.Stretch;
                obj.DataPoint = new Point(0.5, double.NaN);
                //bind.StringFormat = "x={0:#.#}";
                bind.Path = new PropertyPath("DataPoint");
            }

            tb.SetBinding(TextBlock.TextProperty, bind);
            bdr.Child = tb;

            bdr.IsHitTestVisible = false;

            obj.Content = bdr;
            obj.DataPointChanged += obj_DataPointChanged;

            return obj;
        }

        void obj_DataPointChanged(object sender, EventArgs e)
        {
            var cpobj = (ChartPanelObject)sender;
            if (double.IsNaN(cpobj.DataPoint.X))
                ((TextBlock)((Border)(cpobj).Content).Child).Text = cpobj.DataPoint.Y.ToString("F2");
            else
                ((TextBlock)((Border)(cpobj).Content).Child).Text = cpobj.DataPoint.X.ToString("F1");
        }
    }
}