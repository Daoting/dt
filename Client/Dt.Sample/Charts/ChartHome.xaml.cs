#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class ChartHome : NaviWin
    {
        public ChartHome()
        {
            InitializeComponent();
            NaviData = new List<NaviRow>
            {
                new NaviRow(Icons.汉堡, "图表预览", typeof(BaseChart), "图表类型、X轴、Y轴、调色板、图例等设置"),
                new NaviRow(Icons.分组, "数据图表", typeof(DataChart), "Table数据图表"),
                new NaviRow(Icons.详细, "标注", typeof(MarkersChart), "跟踪鼠标，显示标注"),
                new NaviRow(Icons.日历, "组合图表", typeof(ComplexChart), "多图表组合方式"),
                new NaviRow(Icons.修改, "聚合", typeof(AggregateChart), "支持多种聚合方法"),
                new NaviRow(Icons.划卡, "标签", typeof(ChartLabel), "自定义数据标签"),
                new NaviRow(Icons.排列, "饼图", typeof(StackedPie), "csv统计结果"),
                new NaviRow(Icons.录音, "交互", typeof(InteractiveChart), "图表交互"),
                new NaviRow(Icons.公告, "实时", typeof(LiveChart), "实时图表"),
                new NaviRow(Icons.详细, "财务", typeof(FinancialChart), ""),
                new NaviRow(Icons.日历, "甘特图", typeof(GanttChart), ""),
                new NaviRow(Icons.修改, "多轴", typeof(PlotAreas), ""),
                new NaviRow(Icons.划卡, "动画", typeof(LoadAnimation), ""),
            };
        }
    }
}
