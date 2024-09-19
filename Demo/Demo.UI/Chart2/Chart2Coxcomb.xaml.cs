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
    public partial class Chart2Coxcomb : Win
    {

        public Chart2Coxcomb()
        {
            InitializeComponent();

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