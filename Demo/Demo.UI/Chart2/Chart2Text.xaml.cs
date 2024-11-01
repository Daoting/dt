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
using ScottPlot.Plottables;
#endregion

namespace Demo.UI
{
    public partial class Chart2Text : Win
    {
        public Chart2Text()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.Add.Text("Hello, World", 25, 0.5);
            }
        }

        void OnFormat(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var text = _c.Add.Text("Hello, World", 42, 69);
                text.LabelFontSize = 26;
                text.LabelBold = true;
                text.LabelRotation = -45;
                text.LabelFontColor = Colors.Yellow;
                text.LabelBackgroundColor = Colors.Navy.WithAlpha(.5);
                text.LabelBorderColor = Colors.Magenta;
                text.LabelBorderWidth = 3;
                text.LabelPadding = 10;
                text.LabelAlignment = Alignment.MiddleCenter;
            }
        }

        void OnHeight(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var label1 = _c.Add.Text($"line\nheight", 0, 0);
                label1.LabelLineSpacing = 0;
                label1.LabelFontColor = Colors.Red;
                label1.LabelBorderColor = Colors.Black;

                var label2 = _c.Add.Text($"can\nbe", 1, 0);
                label2.LabelLineSpacing = 10;
                label2.LabelFontColor = Colors.Orange;
                label2.LabelBorderColor = Colors.Black;

                var label3 = _c.Add.Text($"automatic\nor", 2, 0);
                label3.LabelLineSpacing = null;
                label3.LabelFontColor = Colors.Green;
                label3.LabelBorderColor = Colors.Black;

                var label4 = _c.Add.Text($"set\nmanually", 3, 0);
                label4.LabelLineSpacing = 15;
                label4.LabelFontColor = Colors.Blue;
                label4.LabelBorderColor = Colors.Black;

                _c.HideGrid();
                _c.Axes.SetLimitsX(-.5, 4);
            }
        }

        void OnOffset(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                for (int i = 0; i < 25; i += 5)
                {
                    // place a marker at the point
                    var marker = _c.Add.Marker(i, 1);

                    // place a styled text label at the point
                    var txt = _c.Add.Text($"{i}", i, 1);
                    txt.LabelFontSize = 16;
                    txt.LabelBorderColor = Colors.Black;
                    txt.LabelBorderWidth = 1;
                    txt.LabelPadding = 2;
                    txt.LabelBold = true;
                    txt.LabelBackgroundColor = marker.Color.WithAlpha(.5);

                    // offset the text label by the given number of pixels
                    txt.OffsetX = i;
                    txt.OffsetY = i;
                }

                _c.Axes.SetLimitsX(-5, 30);
            }
        }
    }
}