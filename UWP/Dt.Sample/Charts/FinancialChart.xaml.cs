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
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Sample
{
    public partial class FinancialChart : Win
    {
        public FinancialChart()
        {
            InitializeComponent();

            _chart.Data.Children[1].ChartType = ChartType.Column;
            _chart.View.AxisX.Scale = 0.25;
            _chart.View.AxisX.ScrollBar = new AxisScrollBar();

            StackPanel spy = new StackPanel();
            spy.Orientation = Orientation.Horizontal;
            spy.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            Button btnPrice = new Button();
            btnPrice.Content = "Standard";
            btnPrice.Click += btnPrice_Click;
            spy.Children.Add(btnPrice);
            TextBlock txtPrice = new TextBlock();
            txtPrice.Text = "Price";
            txtPrice.TextAlignment = Windows.UI.Xaml.TextAlignment.Right;
            txtPrice.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
            spy.Children.Add(txtPrice);
            _chart.View.AxisY.Min = 500;
            _chart.View.AxisY.Title = spy;

            Axis Volume = new Axis();
            Volume.Name = "Volume";
            Volume.Max = 200;
            Volume.Position = AxisPosition.Far;
            Volume.AxisType = AxisType.Y;
            StackPanel spv = new StackPanel();
            TextBlock txtVol = new TextBlock();
            txtVol.Text = "Volume";
            txtVol.Width = 80;
            spv.Orientation = Orientation.Horizontal;
            spv.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            spv.Children.Add(txtVol);
            Button btnVol = new Button();
            btnVol.Content = "Area";
            btnVol.Click += btnVol_Click;
            spv.Children.Add(btnVol);
            Volume.Title = spv;
            _chart.View.Axes.Add(Volume);

            _chart.GestureSlide = GestureSlideAction.Translate;
            _chart.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            _chart.ActionUpdateDelay = 0;

            NewData();
        }

        void NewData()
        {
            long ticks = DateTime.Now.Ticks;
            int len = 100;
            List<Quotation> data = SampleFinancialData.Create(len);
            _chart.BeginUpdate();
            _chart.Data.ItemsSource = data;
            _chart.View.AxisX.Min = TicksToOADate(data[0].Time.Ticks) - 0.5;
            _chart.View.AxisX.Max = TicksToOADate(data[len - 1].Time.Ticks) + 0.5;
            _chart.EndUpdate();
        }

        void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NewData();
        }

        void btnPrice_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if ((string)btn.Content == "Standard")
            {
                btn.Content = "Candle";
                _chart.ChartType = ChartType.HighLowOpenClose;
            }
            else
            {
                btn.Content = "Standard";
                _chart.ChartType = ChartType.Candle;
            }
        }

        void btnVol_Click(object sender, RoutedEventArgs e)
        {
            _chart.BeginUpdate();
            Button btn = (Button)sender;
            if ((string)btn.Content == "Bar")
            {
                btn.Content = "Area";
                _chart.Data.Children[1].ChartType = ChartType.Bar;
            }
            else
            {
                btn.Content = "Bar";
                _chart.Data.Children[1].ChartType = ChartType.Area;
            }
            _chart.EndUpdate();
        }

        static double TicksToOADate(long value)
        {
            if (value == 0L)
            {
                return 0.0;
            }
            if (value < 0xc92a69c000L)
            {
                value += 0x85103c0cb83c000L;
            }
            if (value < 0x6efdddaec64000L)
            {
                throw new OverflowException("Arg_OleAutDateInvalid");
            }
            long num = (value - 0x85103c0cb83c000L) / 0x2710L;
            if (num < 0L)
            {
                long num2 = num % 0x5265c00L;
                if (num2 != 0L)
                {
                    num -= (0x5265c00L + num2) * 2L;
                }
            }
            return (((double)num) / 86400000.0);
        }
    }

    public class Quotation
    {
        public DateTime Time
        {
            get;
            set;
        }

        public double Open
        {
            get;
            set;
        }

        public double Close
        {
            get;
            set;
        }

        public double High
        {
            get;
            set;
        }

        public double Low
        {
            get;
            set;
        }

        public double Volume
        {
            get;
            set;
        }
    }

    public class SampleFinancialData
    {
        static Random rnd = new Random();
        public static List<Quotation> Create(int npts)
        {
            List<Quotation> data = new List<Quotation>();
            DateTime dt = DateTime.Today.AddDays(0);

            for (int i = 0; i < npts; i++)
            {
                Quotation q = new Quotation();

                q.Time = dt.AddDays(i);

                if (i > 0)
                    q.Open = data[i - 1].Close;
                else
                    q.Open = 1000;

                q.High = q.Open + rnd.Next(50);
                q.Low = q.Open - rnd.Next(50);

                q.Close = rnd.Next((int)q.Low, (int)q.High);

                q.Volume = rnd.Next(0, 100);

                data.Add(q);
            }

            return data;
        }
    }
}