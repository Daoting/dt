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
    /// 添加迷你图
    /// </summary>
    internal class InsertSparklineCmd : InsertCmdBase
    {
        public override object Execute(object p_args)
        {
            InsertCmdArgs args = (InsertCmdArgs)p_args;
            var spark = args.RptItem as RptSparkline;
            CellRange range = args.CellRange;
            spark.Row = range.Row;
            spark.Col = range.Column;
            spark.RowSpan = range.RowCount;
            spark.ColSpan = range.ColumnCount;
            spark.Part.Items.Add(spark);
            return spark;
        }
    }
}
