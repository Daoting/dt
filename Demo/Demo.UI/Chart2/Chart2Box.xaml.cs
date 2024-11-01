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
    public partial class Chart2Box : Win
    {

        public Chart2Box()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Box box = new()
                {
                    Position = 5,
                    BoxMin = 81,
                    BoxMax = 93,
                    WhiskerMin = 76,
                    WhiskerMax = 107,
                    BoxMiddle = 84,
                };

                _c.Add.Box(box);

                _c.Axes.SetLimits(0, 10, 70, 110);
            }
        }

        void OnGroup(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                List<ScottPlot.Box> boxes1 = new() {
                    Generate.RandomBox(1),
                    Generate.RandomBox(2),
                    Generate.RandomBox(3),
                };

                List<ScottPlot.Box> boxes2 = new() {
                    Generate.RandomBox(5),
                    Generate.RandomBox(6),
                    Generate.RandomBox(7),
                };

                var bp1 = _c.Add.Boxes(boxes1);
                bp1.LegendText = "Group 1";

                var bp2 = _c.Add.Boxes(boxes2);
                bp2.LegendText = "Group 2";

                _c.ShowLegend(Alignment.UpperRight);
            }
        }
    }
}