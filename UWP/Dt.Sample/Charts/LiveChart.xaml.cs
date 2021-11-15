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
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Sample
{
    public partial class LiveChart : Win
    {
        ObservableCollection<Point> _pts = new ObservableCollection<Point>();
        int _counter = 0;
        Random _rnd = new Random();
        DispatcherTimer _dt;
        int _nMaxPoints = 60;
        int _nAddPoints = 1;

        public LiveChart()
        {
            InitializeComponent();

            _chart.ChartType = ChartType.Line;

            XYDataSeries ds = new XYDataSeries()
            {
                XValueBinding = new Binding() { Path = new PropertyPath("X") },
                ValueBinding = new Binding() { Path = new PropertyPath("Y") },
                ConnectionStrokeThickness = 2,
                Label = "raw",
            };
            _chart.Data.Children.Add(ds);
            _chart.Data.ItemsSource = _pts;

            _chart.View.AxisY.Min = -1000;
            _chart.View.AxisY.Max = 1000;

            _dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.2) };
            _dt.Tick += (s, e) => Update();

            _chart.Loaded += LiveChart_Loaded;
            _chart.Unloaded += LiveChart_Unloaded;
        }

        void LiveChart_Loaded(object sender, RoutedEventArgs e)
        {
            _chart.Loaded -= LiveChart_Loaded;
            _dt.Start();
            btnTimer.Content = "Stop";
        }

        void LiveChart_Unloaded(object sender, RoutedEventArgs e)
        {
            _dt.Stop();
            btnTimer.Content = "Start";
        }

        void Update()
        {
            _chart.BeginUpdate();

            int cnt = _nAddPoints;
            for (int i = 0; i < cnt; i++)
            {
                double r = _rnd.NextDouble();
                double y = (10 * r * Math.Sin(0.1 * _counter) * Math.Sin(0.6 * _rnd.NextDouble() * _counter));
                _pts.Add(new Point(_counter++, y * 100));
            }

            int ndel = _pts.Count - _nMaxPoints;
            if (ndel > 0)
                for (int i = 0; i < ndel; i++)
                    _pts.RemoveAt(0);

            _chart.EndUpdate();
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (_dt.IsEnabled)
            {
                _dt.Stop();
                btn.Content = "Start";
            }
            else
            {
                _dt.Start();
                btn.Content = "Stop";
            }
        }

        internal void StopTimer()
        {
            if (_dt.IsEnabled)
            {
                _dt.Stop();
                btnTimer.Content = "Start";
            }
        }
    }
}