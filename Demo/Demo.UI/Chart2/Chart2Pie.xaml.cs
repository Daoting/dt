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
#endregion

namespace Demo.UI
{
    public partial class Chart2Pie : Win
    {

        public Chart2Pie()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                double[] values = { 5, 2, 8, 4, 8 };
                var pie = _c.Add.Pie(values);
                pie.ExplodeFraction = .1;
            }
        }

        void OnList(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                List<PieSlice> slices = new()
                {
                    new PieSlice() { Value = 5, FillColor = Colors.Red, Label = "Red" },
                    new PieSlice() { Value = 2, FillColor = Colors.Orange, Label = "Orange" },
                    new PieSlice() { Value = 8, FillColor = Colors.Gold, Label = "Yellow" },
                    new PieSlice() { Value = 4, FillColor = Colors.Green, Label = "Green" },
                    new PieSlice() { Value = 8, FillColor = Colors.Blue, Label = "Blue" },
                };

                var pie = _c.Add.Pie(slices);
                pie.ExplodeFraction = .1;

                _c.ShowLegend();
            }
        }

        void OnDonutFraction(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                List<PieSlice> slices = new()
                {
                    new PieSlice() { Value = 5, FillColor = Colors.Red, Label = "Red" },
                    new PieSlice() { Value = 2, FillColor = Colors.Orange, Label = "Orange" },
                    new PieSlice() { Value = 8, FillColor = Colors.Gold, Label = "Yellow" },
                    new PieSlice() { Value = 4, FillColor = Colors.Green, Label = "Green" },
                    new PieSlice() { Value = 8, FillColor = Colors.Blue, Label = "Blue" },
                };

                var pie = _c.Add.Pie(slices);
                pie.DonutFraction = .5;

                _c.ShowLegend();
            }
        }

        void OnRotation(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                double[] values = { 5, 2, 8, 4, 8 };
                var pie = _c.Add.Pie(values);
                pie.ExplodeFraction = .1;
                pie.Rotation = Angle.FromDegrees(90);

            }
        }

        void OnLabel(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                PieSlice slice1 = new() { Value = 5, FillColor = Colors.Red, Label = "alpha" };
                PieSlice slice2 = new() { Value = 2, FillColor = Colors.Orange, Label = "beta" };
                PieSlice slice3 = new() { Value = 8, FillColor = Colors.Gold, Label = "gamma" };
                PieSlice slice4 = new() { Value = 4, FillColor = Colors.Green, Label = "delta" };
                PieSlice slice5 = new() { Value = 8, FillColor = Colors.Blue, Label = "epsilon" };

                List<PieSlice> slices = new() { slice1, slice2, slice3, slice4, slice5 };

                // setup the pie to display slice labels
                var pie = _c.Add.Pie(slices);
                pie.ExplodeFraction = .1;
                pie.SliceLabelDistance = 1.3;

                // color each label's text to match the slice
                slices.ForEach(x => x.LabelFontColor = x.FillColor);

                // styling can be customized for individual slices
                slice5.LabelStyle.FontSize = 22;
                slice5.LabelStyle.Bold = true;
                slice5.LabelStyle.Italic = true;
            }
        }

        void OnCoxcomb(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                List<PieSlice> slices = new()
                {
                    new() { Value = 5, Label = "Red", FillColor = Colors.Red },
                    new() { Value = 2, Label = "Orange", FillColor = Colors.Orange },
                    new() { Value = 8, Label = "Gold", FillColor = Colors.Gold },
                    new() { Value = 4, Label = "Green", FillColor = Colors.Green.WithOpacity(0.5) },
                    new() { Value = 8, Label = "Blue",  FillColor = Colors.Blue.WithOpacity(0.5) },
                };

                _c.Add.Coxcomb(slices);

                _c.Axes.Frameless();
                _c.ShowLegend();
                _c.HideGrid();
            }
        }
    }
}