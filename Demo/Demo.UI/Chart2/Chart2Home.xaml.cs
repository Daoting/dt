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
                new Nav("图表样式及布局", typeof(Chart2Style), Icons.汉堡) { Desc =  "图表样式设置" },
                new Nav("基础图元", typeof(BaseChart2), Icons.汉堡) { Desc =  "注释、箭头、轴线、轴跨度设置" },
                
            };
        }
    }
}
