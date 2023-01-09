using Dt.Base;
using Dt.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Dt.Sample
{
    class ChartSampleData
    {
        public ChartData GetData(ChartType ct)
        {
            if (ct == ChartType.Bubble)
                return BubbleData;
            if (ct == ChartType.Candle || ct == ChartType.HighLowOpenClose)
                return FinancialData;
            if (ct == ChartType.Gantt)
                return GanttData;
            if (ct.ToString().StartsWith("Polar"))
                return PolarData;

            return DefaultData;
        }

        /// <summary>
        /// Default data is appropriate for the most chart types.
        /// </summary>
        public ChartData DefaultData
        {
            get
            {
                ChartData data = new ChartData();

                data.ItemNames = "语文 数学 英语 物理 化学";

                DataSeries ds1 = new DataSeries() { Label = "张平" };
                ds1.ValuesSource = new double[] { 93, 82, 97, 74, 88 };
                data.Children.Add(ds1);

                DataSeries ds2 = new DataSeries() { Label = "李华强" };
                ds2.ValuesSource = new double[] { 91, 92, 93, 94, 96 };
                data.Children.Add(ds2);

                DataSeries ds3 = new DataSeries() { Label = "王力国" };
                ds3.ValuesSource = new double[] { 100, 91, 76, 62, 63 };
                data.Children.Add(ds3);
                return data;
            }
        }

        /// <summary>
        /// Data for financial charts(HighLowOpenClose and Candle).
        /// </summary>
        public ChartData FinancialData
        {
            get
            {
                ChartData data = new ChartData();

                HighLowOpenCloseSeries ds = new HighLowOpenCloseSeries() { Label = "s1" };

                ds.SymbolStrokeThickness = 3; 
                ds.SymbolSize = new Size(20, 20);
                ds.XValuesSource = new DateTime[] 
                    { 
                        new DateTime(2008,10,1), new DateTime(2008,10,2), new DateTime(2008,10,3),
                        new DateTime(2008,10,6), new DateTime(2008,10,7), new DateTime(2008,10,8) 
                     };
                ds.OpenValuesSource = new double[] { 100, 102, 104, 100, 107, 102 };
                ds.CloseValuesSource = new double[] { 102, 104, 100, 107, 102, 100 };
                ds.HighValuesSource = new double[] { 102, 105, 105, 108, 109, 105 };
                ds.LowValuesSource = new double[] { 99, 95, 95, 100, 96, 99, 98 };
                data.Children.Add(ds);

                return data;
            }
        }

        /// <summary>
        /// Data for Bubble chart.
        /// </summary>
        public ChartData BubbleData
        {
            get
            {
                ChartData data = new ChartData();

                BubbleSeries ds1 = new BubbleSeries() { Label = "s1" };
                ds1.ValuesSource = new double[] { 3, 4, 7, 5, 8 };
                ds1.SizeValuesSource = new double[] { 1, 5, 7, 4, 9 };
                data.Children.Add(ds1);

                BubbleSeries ds2 = new BubbleSeries() { Label = "s2" };
                ds2.ValuesSource = new double[] { 1, 2, 3, 4, 6 };
                ds2.SizeValuesSource = new double[] { 10, 3, 6, 8, 9 };
                data.Children.Add(ds2);

                return data;
            }
        }

        /// <summary>
        /// Data for Gantt chart.
        /// </summary>
        public ChartData GanttData
        {
            get
            {
                ChartData data = new ChartData();

                data.ItemNames = new string[] { "Task1", "Task2", "Task3", "Task4" };

                HighLowSeries ds1 = new HighLowSeries() { Label = "Task1" };
                ds1.XValuesSource = new double[] { 0 };
                ds1.LowValuesSource = new DateTime[] { new DateTime(2008, 10, 1) };
                ds1.HighValuesSource = new DateTime[] { new DateTime(2008, 10, 5) };
                data.Children.Add(ds1);

                HighLowSeries ds2 = new HighLowSeries() { Label = "Task2" };
                ds2.XValuesSource = new double[] { 1 };
                ds2.LowValuesSource = new DateTime[] { new DateTime(2008, 10, 3) };
                ds2.HighValuesSource = new DateTime[] { new DateTime(2008, 10, 4) };
                data.Children.Add(ds2);

                HighLowSeries ds3 = new HighLowSeries() { Label = "Task3" };
                ds3.XValuesSource = new double[] { 2 };
                ds3.LowValuesSource = new DateTime[] { new DateTime(2008, 10, 2) };
                ds3.HighValuesSource = new DateTime[] { new DateTime(2008, 10, 8) };
                data.Children.Add(ds3);

                HighLowSeries ds4 = new HighLowSeries() { Label = "Task4" };
                ds4.XValuesSource = new double[] { 3 };
                ds4.LowValuesSource = new DateTime[] { new DateTime(2008, 10, 4) };
                ds4.HighValuesSource = new DateTime[] { new DateTime(2008, 10, 7) };
                data.Children.Add(ds4);

                foreach (DataSeries ds in data.Children)
                {
                    ds.SymbolSize = new Size(30, 30);
                }
                return data;
            }
        }

        /// <summary>
        /// Data for polar chart.
        /// </summary>
        public ChartData PolarData
        {
            get
            {
                int cnt = 61;
                double[] x = new double[cnt];
                double[] y = new double[cnt];
                for (int i = 0; i < cnt; i++)
                {
                    x[i] = i * 6;
                    y[i] = 100 * Math.Abs(Math.Cos(9 * i * Math.PI / 180));
                }
                XYDataSeries ds = new XYDataSeries() { XValuesSource = x, ValuesSource = y };
                ChartData data = new ChartData();
                data.Children.Add(ds);
                return data;
            }
        }

        static Random rnd = new Random();

        public static void CreateData(Chart chart)
        {
            chart.BeginUpdate();

            chart.Data.Children.Clear();

            var ds = CreateDataSeries(100);
            chart.Data.Children.Add(ds);
            chart.EndUpdate();
        }

        public static DataSeries CreateDataSeries(int npts, bool invert = false)
        {
            var ds = new XYDataSeries();// { SymbolSize = new Size(5, 5), ConnectionStrokeThickness = 1 };

            int cnt = npts;
            var x = new double[cnt];
            var y = new double[cnt];

            for (int i = 0; i < cnt; i++)
            {
                x[i] = i; y[i] = rnd.NextDouble();
            }

            if (invert)
            {
                ds.XValuesSource = y; ds.ValuesSource = x;
            }
            else
            {
                ds.XValuesSource = x; ds.ValuesSource = y;
            }
            return ds;
        }

        public static DataSeries CreateDataSeries2(int npts, bool invert = false)
        {
            var ds = new XYDataSeries() { SymbolSize = new Size(5, 5), ConnectionStrokeThickness = 1 };

            int cnt = npts;
            var x = new double[2 * cnt];
            var y = new double[2 * cnt];

            for (int i = 0; i < cnt; i++)
            {
                x[i] = i; y[i] = rnd.NextDouble();
            }

            for (int i = 0; i < cnt; i++)
            {
                x[cnt + i] = cnt - i - 1; y[cnt + i] = rnd.NextDouble();
            }

            if (invert)
            {
                ds.XValuesSource = y; ds.ValuesSource = x;
            }
            else
            {
                ds.XValuesSource = x; ds.ValuesSource = y;
            }
            return ds;
        }
    }
}
