using Dt.Cells.Data;

namespace Dt.Base.Report
{
    /// <summary>
    /// 平移对象
    /// </summary>
    internal class MoveItemsCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            var args = (MoveItemsCmdArgs)p_args;
            args.ClearOldRange();
            foreach (var item in args.Items)
            {
                item.Col = item.Col + args.X < 0 ? 0 : item.Col + args.X;
                item.Row = item.Row + args.Y < 0 ? 0 : item.Row + args.Y;
                item.Update(false);
            }
            return null;
        }

        public override void Undo(object p_args)
        {
            var args = (MoveItemsCmdArgs)p_args;
            args.ClearNewRange();
            foreach (var item in args.Items)
            {
                item.Col = item.Col - args.X < 0 ? 0 : item.Col - args.X;
                item.Row = item.Row - args.Y < 0 ? 0 : item.Row - args.Y;
                item.Update(false);
            }
        }
    }

    internal class MoveItemsCmdArgs
    {
        CellRange _range { get; }
        RptDesignHome _owner { get; }
        
        public MoveItemsCmdArgs(List<RptItem> p_items, int p_x, int p_y, CellRange p_range, RptDesignHome p_owner)
        {
            Items = p_items;
            X = p_x;
            Y = p_y;
            _range = p_range;
            _owner = p_owner;
        }

        public List<RptItem> Items { get; }

        public int X { get; }

        public int Y { get; }
        
        public void ClearOldRange()
        {
            _owner.ClearBodyRange(_range);
        }

        public void ClearNewRange()
        {
            var range = new CellRange(
                _range.Row + Y < 0 ? 0 : _range.Row + Y,
                _range.Column + X < 0 ? 0 : _range.Column + X,
                _range.RowCount,
                _range.ColumnCount);
            _owner.ClearBodyRange(range);
        }
    }
}
