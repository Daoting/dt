#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal partial class SpreadChartContainer : SpreadCharBaseContainer
    {
        public SpreadChartContainer(SpreadChart spreadChart, Chart c1Chart, CellsPanel parentViewport) : base(spreadChart, c1Chart, parentViewport)
        {
        }

        internal override SpreadChartBaseView CreateView(SpreadChartBase spreadChart, Control c1Chart)
        {
            return new SpreadChartView(spreadChart as SpreadChart, c1Chart as Chart);
        }

        public SpreadChartView SpreadChartView
        {
            get { return  (base._chartBaseView as SpreadChartView); }
        }
    }
}

