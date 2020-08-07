using Dt.Cells.Data;
using System.Linq;

namespace Dt.Cells.UI
{
    public partial class ColumnHeaderCellPresenter : HeaderCellPresenter
    {
        public ColumnHeaderCellPresenter()
        {
            DefaultStyleKey = typeof(ColumnHeaderCellPresenter);
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
                bool flag3 = sheet.IsAnyCellInColumnSelected(base.Column);
                bool flag4 = base.Column == sheet.ActiveColumnIndex;
                bool flag5 = Enumerable.FirstOrDefault<CellLayout>(from cellLayout in view.GetViewportCellLayoutModel(0, 0) select cellLayout, delegate (CellLayout cellLayout)
                {
                    return (base.Column >= cellLayout.Column) && (base.Column < (cellLayout.Column + cellLayout.ColumnCount));
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
                return ((sheet.HoverManager.IsMouseOverColumnHeaders && (sheet.MouseOverColumnIndex == base.Column)) && ((base.Row == (worksheet.ColumnHeader.RowCount - 1)) && !sheet.IsWorking));
            }
        }

        protected override bool IsSelected
        {
            get
            {
                Worksheet worksheet = OwningRow.OwningPresenter.Sheet.Worksheet;
                // hdt 唐忠宝 注释掉下面语句及修改linq，增加where条件
                // CellRange columnRange = new CellRange(-1, base.Column, -1, 1);
                return (from range in worksheet.Selections
                        where range.Column <= Column && range.Column + range.ColumnCount > Column
                        select range).Any();
            }
        }

        internal override bool TryUpdateVisualTree()
        {
            SheetView sheetView = base.SheetView;
            if (sheetView != null)
            {
                FilterButtonInfo info = sheetView.GetFilterButtonInfo(base.Row, base.Column, SheetArea.ColumnHeader);
                if (info != base.FilterButtonInfo)
                {
                    base.FilterButtonInfo = info;
                    base.SynFilterButton();
                    return true;
                }
            }
            return false;
        }
    }
}
