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
    public partial class ComplexChart : Win
    {
        Random _rnd = new Random();
        public ComplexChart()
        {
            InitializeComponent();
            _chart.Data.ItemNames = new string[] { "Cat.1", "Cat.2", "Cat.3", "Cat.4", "Cat.5" };
            CreateData();
        }

        void CreateData()
        {
            _chart.Data.Children.Clear();

            for (int i = 0; i < 10; i++)
            {
                DataSeries ds = new DataSeries() { ValuesSource = CreateRandomArray(5), Label = "series " + i };

                BarColumnOptions.SetStackGroup(ds, i % 2);
                _chart.Data.Children.Add(ds);
            }

            _chart.Data.Children.Add(new DataSeries()
            {
                ChartType = ChartType.LineSymbols,
                ValuesSource = CreateRandomArray(5),
                ConnectionStrokeThickness = 5,
                Label = "series 10"
            });
        }

        double[] CreateRandomArray(int cnt)
        {
            double[] vals = new double[cnt];

            for (int i = 0; i < cnt; i++)
                vals[i] = _rnd.Next(10, 100);

            return vals;
        }
    }
}