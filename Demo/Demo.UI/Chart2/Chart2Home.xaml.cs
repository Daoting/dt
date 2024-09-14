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
                new Nav("图表预览", typeof(BaseChart2), Icons.汉堡) { Desc =  "图表类型、X轴、Y轴、调色板、图例等设置" },
                
            };
        }
    }
}
