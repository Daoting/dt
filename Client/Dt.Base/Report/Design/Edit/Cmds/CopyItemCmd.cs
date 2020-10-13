using Dt.Cells.Data;

namespace Dt.Base.Report
{
    /// <summary>
    /// 拷贝对象
    /// </summary>
    internal class CopyItemCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            CopyItemCmdArgs args = (CopyItemCmdArgs)p_args;
            RptItem item = args.Item as RptItem;
            CellRange range = args.Range;
            item.Row = range.Row;
            item.Col = range.Column;
            if(item is RptText || item is RptChart)
            {
                item.RowSpan = range.RowCount;
                item.ColSpan = range.ColumnCount;
            }
            item.Part.Items.Add(item);

            return item;
        }

        public override void Undo(object p_args)
        {
            RptItem rptItem = ((CopyItemCmdArgs)p_args).Item;
            rptItem.Part.Items.Remove(rptItem);
        }
    }

    internal class CopyItemCmdArgs
    {
        public CopyItemCmdArgs(RptItem p_item, CellRange p_range)
        {
            Item = p_item;
            Range = p_range;
        }

        public RptItem Item { get; }

        public CellRange Range { get; }
    }
}
