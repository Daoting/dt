#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Hatches;
#endregion

namespace Demo.UI
{
    public partial class Chart2Financial : Win
    {
        public Chart2Financial()
        {
            InitializeComponent();
        }

        void OnCandlestick(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var prices = Generate.RandomOHLCs(30);
                _c.Add.Candlestick(prices);
                _c.Axes.DateTimeTicksBottom();
            }
        }

        void OnOHLC(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var prices = Generate.RandomOHLCs(30);
                _c.Add.OHLC(prices);
                _c.Axes.DateTimeTicksBottom();

            }
        }

        void OnFinance(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                DateTime timeOpen = new(1985, 09, 24, 9, 30, 0); // 9:30 AM
                DateTime timeClose = new(1985, 09, 24, 16, 0, 0); // 4:00 PM
                TimeSpan timeSpan = TimeSpan.FromMinutes(10); // 10 minute bins

                List<OHLC> prices = new();
                for (DateTime dt = timeOpen; dt <= timeClose; dt += timeSpan)
                {
                    double open = Generate.RandomNumber(20, 40) + prices.Count;
                    double close = Generate.RandomNumber(20, 40) + prices.Count;
                    double high = Math.Max(open, close) + Generate.RandomNumber(5);
                    double low = Math.Min(open, close) - Generate.RandomNumber(5);
                    prices.Add(new OHLC(open, high, low, close, dt, timeSpan));
                }

                _c.Add.Candlestick(prices);
                _c.Axes.DateTimeTicksBottom();
            }
        }

        void OnRight(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var prices = Generate.RandomOHLCs(30);
                var candles = _c.Add.Candlestick(prices);

                // configure the candlesticks to use the plot's right axis
                candles.Axes.YAxis = _c.Axes.Right;
                candles.Axes.YAxis.Label.Text = "Price";

                // style the bottom axis to display date
                _c.Axes.DateTimeTicksBottom();
            }
        }

        void OnMoving(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var prices = Generate.RandomOHLCs(75);
                _c.Add.Candlestick(prices);
                _c.Axes.DateTimeTicksBottom();

                // calculate SMA and display it as a scatter plot
                int[] windowSizes = { 3, 8, 20 };
                foreach (int windowSize in windowSizes)
                {
                    ScottPlot.Finance.SimpleMovingAverage sma = new(prices, windowSize);
                    var sp = _c.Add.Scatter(sma.Dates, sma.Means);
                    sp.LegendText = $"SMA {windowSize}";
                    sp.MarkerSize = 0;
                    sp.LineWidth = 3;
                    sp.Color = Colors.Navy.WithAlpha(1 - windowSize / 30.0);
                }

                _c.ShowLegend();

            }
        }

        void OnBollinger(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var prices = Generate.RandomOHLCs(100);
                _c.Add.Candlestick(prices);
                _c.Axes.DateTimeTicksBottom();

                // calculate Bollinger Bands
                ScottPlot.Finance.BollingerBands bb = new(prices, 20);

                // display center line (mean) as a solid line
                var sp1 = _c.Add.Scatter(bb.Dates, bb.Means);
                sp1.MarkerSize = 0;
                sp1.Color = Colors.Navy;

                // display upper bands (positive variance) as a dashed line
                var sp2 = _c.Add.Scatter(bb.Dates, bb.UpperValues);
                sp2.MarkerSize = 0;
                sp2.Color = Colors.Navy;
                sp2.LinePattern = LinePattern.Dotted;

                // display lower bands (positive variance) as a dashed line
                var sp3 = _c.Add.Scatter(bb.Dates, bb.LowerValues);
                sp3.MarkerSize = 0;
                sp3.Color = Colors.Navy;
                sp3.LinePattern = LinePattern.Dotted;
            }
        }

        void OnCandlestickNoGap(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var prices = Generate.RandomOHLCs(31);
                var candlePlot = _c.Add.Candlestick(prices);

                // enable sequential mode to place candles at X = 0, 1, 2, ...
                candlePlot.Sequential = true;

                // determine a few candles to display ticks for
                int tickCount = 5;
                int tickDelta = prices.Count / tickCount;
                DateTime[] tickDates = prices
                    .Where((x, i) => i % tickDelta == 0)
                    .Select(x => x.DateTime)
                    .ToArray();

                // By default, horizontal tick labels will be numbers (1, 2, 3...)
                // We can use a manual tick generator to display dates on the horizontal axis
                double[] tickPositions = Generate.Consecutive(tickDates.Length, tickDelta);
                string[] tickLabels = tickDates.Select(x => x.ToString("MM/dd")).ToArray();
                ScottPlot.TickGenerators.NumericManual tickGen = new(tickPositions, tickLabels);
                _c.Axes.Bottom.TickGenerator = tickGen;
            }
        }

        void OnOHLCNoGap(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var prices = Generate.RandomOHLCs(31);
                var ohlcPlot = _c.Add.OHLC(prices);

                // enable sequential mode to place OHLCs at X = 0, 1, 2, ...
                ohlcPlot.Sequential = true;

                // determine a few OHLCs to display ticks for
                int tickCount = 5;
                int tickDelta = prices.Count / tickCount;
                DateTime[] tickDates = prices
                    .Where((x, i) => i % tickDelta == 0)
                    .Select(x => x.DateTime)
                    .ToArray();

                // By default, horizontal tick labels will be numbers (1, 2, 3...)
                // We can use a manual tick generator to display dates on the horizontal axis
                double[] tickPositions = Generate.Consecutive(tickDates.Length, tickDelta);
                string[] tickLabels = tickDates.Select(x => x.ToString("MM/dd")).ToArray();
                ScottPlot.TickGenerators.NumericManual tickGen = new(tickPositions, tickLabels);
                _c.Axes.Bottom.TickGenerator = tickGen;
            }
        }
    }
}