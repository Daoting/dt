using Dt.Cells.Data;
using System.Linq;

namespace Dt.Cells.UI
{
    public partial class RowHeaderCellPresenter : HeaderCellPresenter
    {
        public RowHeaderCellPresenter()
        {
            DefaultStyleKey = typeof(RowHeaderCellPresenter);
        }

        internal override bool IsHightlighted
        {
            get
            {
                SheetView view = base.OwningRow.OwningPresenter.Sheet;
                if (view.HideSelectionWhenPrinting)
                {
                    return false;
                }
                if (view.HasSelectedFloatingObject())
                {
                    return false;
                }
                Worksheet sheet = view.Worksheet;
                bool flag3 = sheet.IsAnyCellInRowSelected(base.Row);
                bool flag4 = base.Row == sheet.ActiveRowIndex;
                bool flag5 = Enumerable.FirstOrDefault<CellLayout>(from cellLayout in view.GetViewportCellLayoutModel(view.GetActiveRowViewportIndex(), view.GetActiveColumnViewportIndex()) select cellLayout, delegate(CellLayout cellLayout)
                {
                    return (base.Row >= cellLayout.Row) && (base.Row < (cellLayout.Row + cellLayout.RowCount));
                }) != null;
                if (!flag3 && !flag4)
                {
                    return flag5;
                }
                return true;
            }
        }

        protected override bool IsMouseOver
        {
            get
            {
                SheetView sheet = base.OwningRow.OwningPresenter.Sheet;
                Worksheet worksheet = sheet.Worksheet;
                return ((sheet.HoverManager.IsMouseOverRowHeaders && (sheet.MouseOverRowIndex == base.Row)) && ((base.Column == (worksheet.RowHeader.ColumnCount - 1)) && !sheet.IsWorking));
            }
        }

        protected override bool IsSelected
        {
            get
            {
                Worksheet worksheet = base.OwningRow.OwningPresenter.Sheet.Worksheet;
                // hdt 唐忠宝 注释掉下面语句及修改linq，增加where条件
                // CellRange rowRange = new CellRange(base.Row, -1, 1, -1);
                return (from range in worksheet.Selections
                        where range.Row <= Row && range.Row + range.RowCount > Row
                        select range).Any();
            }
        }
    }
}
