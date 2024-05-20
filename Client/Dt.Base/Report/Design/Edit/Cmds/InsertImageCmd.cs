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
    /// 添加图片
    /// </summary>
    internal class InsertImageCmd : InsertCmdBase
    {
        public override object Execute(object p_args)
        {
            InsertCmdArgs args = (InsertCmdArgs)p_args;
            var img = args.RptItem as RptImage;
            CellRange range = args.CellRange;
            img.Row = range.Row;
            img.Col = range.Column;
            img.RowSpan = range.RowCount;
            img.ColSpan = range.ColumnCount;
            img.Part.Items.Add(img);
            return img;
        }
    }
}
