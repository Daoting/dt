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
using System.IO;
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
    public partial class StackedPie : Win
    {
        public StackedPie()
        {
            InitializeComponent();

            var data = new CSVData();
            data.Read(ResKit.GetResource("browsers.csv"), false);

            int len = data.Length;
            var vdata = new VersionInfo[len];

            for (int i = 0; i < len; i++)
            {
                vdata[i] = new VersionInfo()
                {
                    Name = data[i, 0],
                    Version = data[i, 1],
                    Value = double.Parse(data[i, 2])
                };
            }

            _chart.BeginUpdate();
            _chart.Data.ItemsSource = vdata;
            Binding bindingName = new Binding();
            bindingName.Path = new PropertyPath("Name");
            _chart.Data.ItemNameBinding = bindingName;

            _chart.Aggregate = Aggregate.Sum;

            // first series - total by browser
            Binding valueBinding = new Binding();
            valueBinding.Path = new PropertyPath("Value");
            var ds1 = new DataSeries()
            {
                ValueBinding = valueBinding,
                PointLabelTemplate = (DataTemplate)Resources["lbl"],
            };
            ds1.PlotElementLoaded += (PlotElementLoaded);
            Canvas.SetZIndex(ds1, 1);
            _chart.Data.Children.Add(ds1);

            // second series - browser versions
            Binding valueBinding2 = new Binding();
            valueBinding2.Path = new PropertyPath("Value");
            var ds2 = new DataSeries()
            {
                ItemsSource = vdata, // own data source(no aggregates)
                ValueBinding = valueBinding2,
                PointLabelTemplate = (DataTemplate)Resources["lbl1"],
            };
            ds2.PlotElementLoaded += (PlotElementLoaded);
            _chart.Data.Children.Add(ds2);

            // _chart type and direction
            _chart.ChartType = ChartType.PieStacked;
            PieOptions.SetDirection(_chart, SweepDirection.Counterclockwise);

            // set palette
            _palette.Add("Internet Explorer", Color.FromArgb(255, 214, 239, 255));
            _palette.Add("Firefox", Color.FromArgb(255, 123, 211, 56));
            _palette.Add("Chrome", Color.FromArgb(255, 239, 21, 123));
            _palette.Add("Safari", Color.FromArgb(255, 255, 186, 0));
            _palette.Add("Opera", Color.FromArgb(255, 0, 174, 222));

            // find max version usage by browser
            foreach (var key in _palette.Keys)
                _maxs[key] = (from item in vdata where item.Name == key select item.Value).Max();

            _chart.EndUpdate();
        }

        // palette colors
        Dictionary<string, Color> _palette = new Dictionary<string, Color>();
        // max values(by browser)
        Dictionary<string, double> _maxs = new Dictionary<string, double>();

        void PlotElementLoaded(object sender, EventArgs e)
        {
            var pe = (PlotElement)sender;
            var dp = pe.DataPoint;

            pe.Stroke = new SolidColorBrush(Colors.White);
            pe.StrokeThickness = 1;

            // set slice color
            if (dp.SeriesIndex == 0)
                pe.Fill = new SolidColorBrush(_palette[dp.Name]);
            else
            {
                var name = ((VersionInfo)dp.DataObject).Name;
                pe.Fill = new SolidColorBrush(_palette[name])
                {
                    // opacity depends on value(less usage -> more transparent)
                    Opacity = 0.3 + 0.7 * dp.Value / _maxs[name]
                };
            }
        }
    }

    public class VersionInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public double Value { get; set; }
    }

    public class CSVData
    {
        List<string> _fields = new List<string>();
        List<string[]> _records = new List<string[]>();

        List<string> Fields
        {
            get { return _fields; }
        }

        List<string[]> Records
        {
            get { return _records; }
        }

        public int Length
        {
            get { return _records.Count; }
        }

        public string this[int i, int j]
        {
            get
            {
                string[] ss = Records[i];
                if (j < 0 || j >= ss.Length)
                    throw new ArgumentException("j");

                return ss[j].Trim('"');
            }
        }

        public string this[int i, string name]
        {
            get
            {
                string[] ss = Records[i];
                int j = Fields.IndexOf(name);
                if (j < 0 || j >= ss.Length)
                    throw new ArgumentException("name");

                return ss[j];
            }
        }

        public void Read(Stream stream, bool readNames)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Fields.Clear();
            Records.Clear();

            if (stream != null)
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    int i = 0;
                    while (sr.Peek() > -1)
                    {
                        string line = sr.ReadLine();

                        if (line.StartsWith("#"))
                            continue;

                        string[] ss = line.Split(',');

                        if (readNames && Fields.Count == 0)
                            Fields.AddRange(ss);
                        else
                        {
                            Records.Add(ss);
                            i++;
                        }
                    }
                }
            }
        }
    }
}