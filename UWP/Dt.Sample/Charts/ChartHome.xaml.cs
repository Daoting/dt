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
    public sealed partial class ChartHome : Win
    {
        public ChartHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("图表预览", typeof(BaseChart), Icons.汉堡) { Desc =  "图表类型、X轴、Y轴、调色板、图例等设置" },
                new Nav("数据图表", typeof(DataChart), Icons.分组) { Desc =  "Table数据图表" },
                new Nav("标注", typeof(MarkersChart), Icons.全选) { Desc =  "跟踪鼠标，显示标注" },
                new Nav("组合图表", typeof(ComplexChart), Icons.日历) { Desc = "多图表组合方式" },
                new Nav("聚合", typeof(AggregateChart), Icons.修改) { Desc =  "支持多种聚合方法" },
                new Nav("标签", typeof(ChartLabel), Icons.划卡) { Desc =  "自定义数据标签" },
                new Nav("饼图", typeof(StackedPie), Icons.排列) { Desc =  "csv统计结果" },
                new Nav("交互", typeof(InteractiveChart), Icons.录音),
                new Nav("实时", typeof(LiveChart), Icons.公告),
                new Nav("财务", typeof(FinancialChart), Icons.全选),
                new Nav("甘特图", typeof(GanttChart), Icons.日历),
                new Nav("多轴", typeof(PlotAreas), Icons.修改),
                new Nav("动画", typeof(LoadAnimation), Icons.划卡) { Desc =  "有bug，暂时无动画" },
            };
        }
    }
}
