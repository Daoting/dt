#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System.Linq;
#endregion

namespace Dt.Cells.UI
{
    public partial class ColHeaderCell : HeaderCellItem
    {
        protected override bool IsHightlighted
        {
            get
            {
                SheetView view = Owner.Owner.Sheet;
                if (view.HideSelectionWhenPrinting)
                {
                    return false;
                }
                if (view.HasSelectedFloatingObject())
                {
                    return false;
                }
                Worksheet sheet = view.ActiveSheet;
                bool flag3 = sheet.IsAnyCellInColumnSelected(Column);
                bool flag4 = Column == sheet.ActiveColumnIndex;
                bool flag5 = Enumerable.FirstOrDefault<CellLayout>(from cellLayout in view.GetViewportCellLayoutModel(0, 0) select cellLayout, delegate (CellLayout cellLayout)
                {
                    return (Column >= cellLayout.Column) && (Column < (cellLayout.Column + cellLayout.ColumnCount));
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
                SheetView sheet = Owner.Owner.Sheet;
                Worksheet worksheet = sheet.ActiveSheet;
                return ((sheet.HoverManager.IsMouseOverColumnHeaders && (sheet.MouseOverColumnIndex == Column)) && ((Row == (worksheet.ColumnHeader.RowCount - 1)) && !sheet.IsWorking));
            }
        }

        protected override bool IsSelected
        {
            get
            {
                Worksheet worksheet = Owner.Owner.Sheet.ActiveSheet;
                // hdt 唐忠宝 注释掉下面语句及修改linq，增加where条件
                // CellRange columnRange = new CellRange(-1, Column, -1, 1);
                return (from range in worksheet.Selections
                        where range.Column <= Column && range.Column + range.ColumnCount > Column
                        select range).Any();
            }
        }
    }
}
