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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public sealed partial class Chart2Home : Win
    {
        public Chart2Home()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("坐标轴及图例", typeof(Chart2Axes), Icons.折线图) { Desc =  "X轴、Y轴、标题、网格、图例等设置" },
                new Nav("图表样式及布局", typeof(Chart2Style), Icons.汉堡) { Desc =  "样式、背景图像、颜色插值、布局区域" },
                new Nav("基础图元", typeof(BaseChart2), Icons.汉堡) { Desc =  "注释、箭头、轴线、轴跨度、标注、十字准线" },
                new Nav("条形图", typeof(Chart2Bar), Icons.汉堡) { Desc =  "水平或垂直矩形" },
                new Nav("箱形图", typeof(Chart2Box), Icons.汉堡) { Desc =  "一目了然地显示分布" },
                new Nav("饼图", typeof(Chart2Pie), Icons.汉堡) { Desc = "将数字比例说明为圆的切片" },
                new Nav("椭圆", typeof(Chart2Ellipse), Icons.汉堡) { Desc = "具有定义中心和不同 X 和 Y 半径的曲线。圆是 X 半径等于其 Y 半径的椭圆" },
                new Nav("误差线", typeof(Chart2ErrorBars), Icons.汉堡) { Desc = "表示测量值的可能范围" },
                new Nav("Y填充图", typeof(Chart2FillY), Icons.汉堡) { Desc =  "在定义的 X 位置显示两个 Y 值之间的垂直范围" },
                new Nav("财务图", typeof(Chart2Financial), Icons.汉堡) { Desc = "显示按时间范围分箱的价格数据" },
                new Nav("函数线图", typeof(Chart2Func), Icons.汉堡) { Desc =  " Y 位置由依赖于 X 的函数定义，而不是离散数据点的集合" },
                new Nav("热图", typeof(Chart2Heatmap), Icons.汉堡) { Desc = "将 2D 数据中的值显示为具有不同强度的像元的图像" },
                new Nav("线图", typeof(Chart2Line), Icons.汉堡) { Desc =  "使用 Start、End 和可选的 LineStyle 将线图放置在坐标空间中的绘图上" },
                new Nav("棒棒糖图", typeof(Chart2Lollipop), Icons.汉堡) { Desc = "条形图的一种变体，它使用从基线延伸到标记（头部）的线条（茎）来表示数据点。与传统条形图相比，突出显示单个数据点的视觉混乱程度较低" },
                new Nav("标记", typeof(Chart2Marker), Icons.汉堡) { Desc =  "标记可以放置在坐标空间的绘图上" },
                new Nav("极坐标图", typeof(Chart2Polar), Icons.汉堡) { Desc = "创建极轴并将其添加到绘图中，以在圆形坐标系上显示数据" },
                new Nav("总体图", typeof(Chart2Population), Icons.汉堡) { Desc =  "显示单个值的集合" },
                new Nav("雷达图", typeof(Chart2Radar), Icons.汉堡) { Desc = "将多轴数据表示为围绕中心点圆周排列的轴上的 2D 形状" },
                new Nav("仪表图", typeof(Chart2Radial), Icons.汉堡) { Desc =  "将标量数据显示为圆形仪表" },
                new Nav("散点图", typeof(Chart2Scatter), Icons.汉堡) { Desc = "在坐标空间中的 X/Y 位置显示点" },
                new Nav("几何图形", typeof(Chart2Shape), Icons.汉堡) { Desc =  "可以添加到绘图的基本形状" },
                new Nav("信号图", typeof(Chart2Signal), Icons.汉堡) { Desc = "显示均匀分布的数据" },
                new Nav("SignalXY图", typeof(Chart2SignalXY), Icons.汉堡) { Desc = "一种高性能绘图类型，针对 X 值始终升序的 X/Y 对进行了优化。对于大型数据集，SignalXY 图的性能比散点图（允许无序的 X 点）高得多，但不如信号图（需要 X 点之间的固定间距）的性能" },
                new Nav("文本", typeof(Chart2Text), Icons.汉堡) { Desc = "文本标签可以放置在坐标空间的绘图上" },

            };
        }
    }
}
