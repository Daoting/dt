﻿#region 文件描述
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
    public sealed partial class ExcelHome : NaviWin
    {
        public ExcelHome()
        {
            InitializeComponent();
            NaviData = new List<NaviRow>
            {
                new NaviRow(Icons.汉堡, "基本操作", typeof(BaseExcel), "Excel基本编辑功能及设置"),
                new NaviRow(Icons.分组, "用例", typeof(UseCase), "常见使用用例"),
                new NaviRow(Icons.详细, "数据操作", typeof(DataExcel), "Excel数据源"),
                new NaviRow(Icons.日历, "图表", typeof(ChartExcel), "内嵌图表"),
                new NaviRow(Icons.修改, "迷你图", typeof(Sparkline), ""),
                new NaviRow(Icons.划卡, "过滤", typeof(FilterExcel), ""),
                new NaviRow(Icons.排列, "分组排序", typeof(RangeGroup), ""),
                new NaviRow(Icons.录音, "浮动对象", typeof(FloatingObject), ""),
                new NaviRow(Icons.公告, "公式框", typeof(FormulaTextBox), ""),
                new NaviRow(Icons.详细, "格式化", typeof(Formatter), ""),
                new NaviRow(Icons.日历, "样式", typeof(ExcelTheme), ""),
                new NaviRow(Icons.修改, "图片", typeof(ImgExcel), ""),
                new NaviRow(Icons.划卡, "图表图片", typeof(ChartPicture), ""),
                new NaviRow(Icons.打印, "打印", typeof(PrintExcel), "只支持Windows"),
            };
        }
    }
}
