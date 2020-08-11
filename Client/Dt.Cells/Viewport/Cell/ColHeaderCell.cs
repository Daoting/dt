using Dt.Cells.Data;
using System.Linq;

namespace Dt.Cells.UI
{
    public partial class ColHeaderCell : HeaderCellItem
    {
        public ColHeaderCell()
        {
            DefaultStyleKey = typeof(ColHeaderCell);
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
                Worksheet sheet = view.ActiveSheet;
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
                Worksheet worksheet = sheet.ActiveSheet;
                return ((sheet.HoverManager.IsMouseOverColumnHeaders && (sheet.MouseOverColumnIndex == base.Column)) && ((base.Row == (worksheet.ColumnHeader.RowCount - 1)) && !sheet.IsWorking));
            }
        }

        protected override bool IsSelected
        {
            get
            {
                Worksheet worksheet = OwningRow.OwningPresenter.Sheet.ActiveSheet;
                // hdt 唐忠宝 注释掉下面语句及修改linq，增加where条件
                // CellRange columnRange = new CellRange(-1, base.Column, -1, 1);
                return (from range in worksheet.Selections
                        where range.Column <= Column && range.Column + range.ColumnCount > Column
                        select range).Any();
            }
        }
    }
}
