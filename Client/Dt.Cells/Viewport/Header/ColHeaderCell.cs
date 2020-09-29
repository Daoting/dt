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
    internal partial class ColHeaderCell : HeaderCellItem
    {
        public ColHeaderCell(HeaderItem p_rowItem)
            : base(p_rowItem)
        { 
        }

        protected override bool IsHightlighted
        {
            get
            {
                var excel = OwnRow.Owner.Excel;
                if (!excel.ShowSelection)
                {
                    return false;
                }
                if (excel.HasSelectedFloatingObject())
                {
                    return false;
                }
                Worksheet sheet = excel.ActiveSheet;
                bool flag3 = sheet.IsAnyCellInColumnSelected(Column);
                bool flag4 = Column == sheet.ActiveColumnIndex;
                bool flag5 = Enumerable.FirstOrDefault<CellLayout>(from cellLayout in excel.GetViewportCellLayoutModel(0, 0) select cellLayout, delegate (CellLayout cellLayout)
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
                var excel = OwnRow.Owner.Excel;
                Worksheet worksheet = excel.ActiveSheet;
                return ((excel.HoverManager.IsMouseOverColumnHeaders && (excel.MouseOverColumnIndex == Column)) && ((Row == (worksheet.ColumnHeader.RowCount - 1)) && !excel.IsWorking));
            }
        }

        protected override bool IsSelected
        {
            get
            {
                Worksheet worksheet = OwnRow.Owner.Excel.ActiveSheet;
                // hdt 唐忠宝 注释掉下面语句及修改linq，增加where条件
                // CellRange columnRange = new CellRange(-1, Column, -1, 1);
                return (from range in worksheet.Selections
                        where range.Column <= Column && range.Column + range.ColumnCount > Column
                        select range).Any();
            }
        }
    }
}
