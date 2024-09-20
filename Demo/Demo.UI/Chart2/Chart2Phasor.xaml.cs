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
    public partial class Chart2Phasor : Win
    {
        public Chart2Phasor()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis(30);
                polarAxis.Circles.ForEach(x => x.LinePattern = LinePattern.Dotted);
                polarAxis.Spokes.ForEach(x => x.LinePattern = LinePattern.Dotted);

                // A Phasor may be added with predefined points
                PolarCoordinates[] points1 = [
                    new(10, Angle.FromDegrees(15)),
                    new(20, Angle.FromDegrees(120)),
                    new(30, Angle.FromDegrees(240)),
                ];
                _c.Add.Phasor(points1);

                // Points on a Phasor may be added or modified after it is created
                var phaser2 = _c.Add.Phasor();
                phaser2.Points.Add(new PolarCoordinates(20, Angle.FromDegrees(35)));
                phaser2.Points.Add(new PolarCoordinates(25, Angle.FromDegrees(140)));
                phaser2.Points.Add(new PolarCoordinates(20, Angle.FromDegrees(260)));
            }
        }

        void OnLineStyle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis(30);
                polarAxis.Circles.ForEach(x => x.LinePattern = LinePattern.Dotted);
                polarAxis.Spokes.ForEach(x => x.LinePattern = LinePattern.Dotted);

                // create a phasor plot and points in coordinate space
                var phaser = _c.Add.Phasor();
                phaser.Points.Add(new PolarCoordinates(20, Angle.FromDegrees(35)));
                phaser.Points.Add(new PolarCoordinates(25, Angle.FromDegrees(140)));
                phaser.Points.Add(new PolarCoordinates(20, Angle.FromDegrees(260)));

                // add labels for points
                phaser.Labels.Add("Alpha");
                phaser.Labels.Add("Beta");
                phaser.Labels.Add("Gamma");

                // style the labels
                phaser.LabelStyle.FontSize = 24;
                phaser.LabelStyle.ForeColor = Colors.Black;
                phaser.LabelStyle.FontName = Fonts.Monospace;
                phaser.LabelStyle.Bold = true;
            }
        }
    }
}