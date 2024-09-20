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
                new Nav("条形图", typeof(Chart2Bar), Icons.汉堡),
                new Nav("箱形图", typeof(Chart2Box), Icons.汉堡),
                new Nav("饼图", typeof(Chart2Pie), Icons.汉堡),
                new Nav("椭圆", typeof(Chart2Ellipse), Icons.汉堡),
                new Nav("误差线", typeof(Chart2ErrorBars), Icons.汉堡),
                new Nav("Y填充图", typeof(Chart2FillY), Icons.汉堡),
                new Nav("财务图", typeof(Chart2Financial), Icons.汉堡),
                new Nav("函数线图", typeof(Chart2Func), Icons.汉堡),
                new Nav("热图", typeof(Chart2Heatmap), Icons.汉堡),
                new Nav("线图", typeof(Chart2Line), Icons.汉堡),
                new Nav("棒棒糖图", typeof(Chart2Lollipop), Icons.汉堡),
                new Nav("标记", typeof(Chart2Marker), Icons.汉堡),
                new Nav("极坐标图", typeof(Chart2Polar), Icons.汉堡),
                new Nav("总体图", typeof(Chart2Population), Icons.汉堡),
                new Nav("雷达图", typeof(Chart2Radar), Icons.汉堡),

            };
        }
    }
}
