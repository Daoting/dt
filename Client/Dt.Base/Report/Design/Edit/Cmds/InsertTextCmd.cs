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
    /// 添加文本
    /// </summary>
    internal class InsertTextCmd : InsertCmdBase
    {
        public override object Execute(object p_args)
        {
            InsertCmdArgs args = (InsertCmdArgs)p_args;
            RptText txt = args.RptItem as RptText;
            CellRange range = args.CellRange;
            txt.Row = range.Row;
            txt.Col = range.Column;
            txt.RowSpan = range.RowCount;
            txt.ColSpan = range.ColumnCount;
            txt.Val = "文本";
            txt.LeftStyle = BorderLineStyle.None;
            txt.TopStyle = BorderLineStyle.None;
            txt.RightStyle = BorderLineStyle.None;
            txt.BottomStyle = BorderLineStyle.None;
            txt.Part.Items.Add(txt);
            return txt;
        }
    }
}
