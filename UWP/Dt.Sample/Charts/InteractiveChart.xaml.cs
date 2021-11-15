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
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Sample
{
    public partial class InteractiveChart : Win
    {
        Random _rnd = new Random();

        public InteractiveChart()
        {
            InitializeComponent();

            rbX.Checked += rb_Checked;
            rbY.Checked += rb_Checked;
            rbXY.Checked += rb_Checked;

            _chart.View.AxisX.ScrollBar = new AxisScrollBar() { Visibility = Visibility.Collapsed };
            _chart.View.AxisY.ScrollBar = new AxisScrollBar() { Visibility = Visibility.Collapsed };

            _chart.ActionLeave += new EventHandler(Actions_Leave);

            CreataSampleData(3, 200);

            _chart.View.AxisX.Scale = 0.5;
            UpdateScrollbars();

            _chart.ActionUpdateDelay = 0;
            _chart.ManipulationMode = ManipulationModes.Scale |
              ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;

            _chart.GestureDoubleTap = GestureDoubleTapAction.Scale;
            _chart.GesturePinch = GesturePinchAction.Scale;
            _chart.GestureSlide = GestureSlideAction.Translate;
        }

        void Actions_Leave(object sender, EventArgs e)
        {
            UpdateScrollbars();
        }

        void CreataSampleData(int nser, int npts)
        {
            _chart.BeginUpdate();
            DataSeriesCollection sc = _chart.Data.Children;
            sc.Clear();

            for (int iser = 0; iser < nser; iser++)
            {
                double phase = 0.05 + 0.1 * _rnd.NextDouble();
                double amp = 1 + 5 * _rnd.NextDouble();

                DataSeries ds = new DataSeries();
                double[] vals = new double[npts];
                for (int i = 0; i < npts; i++)
                    vals[i] = amp * Math.Sin(i * phase);

                ds.ValuesSource = vals;
                ds.Label = "S " + iser.ToString();
                ds.ConnectionStrokeThickness = 2;

                sc.Add(ds);
            }
            _chart.EndUpdate();
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            _chart.BeginUpdate();
            _chart.View.AxisX.Scale = 1;
            _chart.View.AxisX.Value = 0.5;
            _chart.View.AxisY.Scale = 1;
            _chart.View.AxisY.Value = 0.5;

            UpdateScrollbars();
            _chart.EndUpdate();
        }

        void rb_Checked(object sender, RoutedEventArgs e)
        {
            if (_chart == null)
                return;

            _chart.BeginUpdate();

            if (sender == rbX)
            {
                // limit scale of Y-axis
                _chart.View.AxisY.MinScale = 1;
                _chart.View.AxisY.Scale = 1;
                _chart.View.AxisY.Value = 0.5;
                _chart.View.AxisX.MinScale = 0.001;
            }
            else if (sender == rbY)
            {
                // limit scale of X-axis
                _chart.View.AxisX.MinScale = 1;
                _chart.View.AxisX.Scale = 1;
                _chart.View.AxisX.Value = 0.5;
                _chart.View.AxisY.MinScale = 0.001;
            }
            else
            {
                // allow smaller scales for both axes
                _chart.View.AxisX.MinScale = 0.001;
                _chart.View.AxisY.MinScale = 0.001;
            }

            UpdateScrollbars();
            _chart.EndUpdate();
        }

        void UpdateScrollbars()
        {
            double sx = _chart.View.AxisX.Scale;
            AxisScrollBar sbx = (AxisScrollBar)_chart.View.AxisX.ScrollBar;
            if (sx >= 1.0)
                sbx.Visibility = Visibility.Collapsed;
            else
                sbx.Visibility = Visibility.Visible;

            double sy = _chart.View.AxisY.Scale;
            AxisScrollBar sby = (AxisScrollBar)_chart.View.AxisY.ScrollBar;
            if (sy >= 1.0)
                sby.Visibility = Visibility.Collapsed;
            else
                sby.Visibility = Visibility.Visible;
        }
    }
}