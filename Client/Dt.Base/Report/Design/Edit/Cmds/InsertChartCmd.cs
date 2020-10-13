#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 添加图表
    /// </summary>
    internal class InsertChartCmd : InsertCmdBase
    {
        public override object Execute(object p_args)
        {
            InsertCmdArgs args = (InsertCmdArgs)p_args;
            RptChart chart = args.RptItem as RptChart;
            CellRange range = args.CellRange;
            chart.Row = range.Row;
            chart.Col = range.Column;
            chart.RowSpan = range.RowCount;
            chart.ColSpan = range.ColumnCount;
            chart.Part.Items.Add(chart);
            return chart;
        }
    }
}
