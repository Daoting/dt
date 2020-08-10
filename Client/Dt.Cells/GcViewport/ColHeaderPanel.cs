#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    internal partial class ColHeaderPanel : CellsPanel
    {
        List<RowItem> _recycledColHeaderRows;

        public ColHeaderPanel(SheetView sheet) : base(sheet, SheetArea.ColumnHeader)
        {
            _recycledColHeaderRows = new List<RowItem>();
        }

        internal override RowItem GenerateNewRow()
        {
            return new ColHeaderItem(this);
        }

        internal override CellLayoutModel GetCellLayoutModel()
        {
            return Sheet.GetColumnHeaderCellLayoutModel(base.ColumnViewportIndex);
        }

        internal override ICellsSupport GetDataContext()
        {
            return Sheet.ActiveSheet.ColumnHeader;
        }

        internal override RowLayoutModel GetRowLayoutModel()
        {
            return Sheet.GetColumnHeaderRowLayoutModel();
        }

        internal override SheetSpanModelBase GetSpanModel()
        {
            return Sheet.ActiveSheet.ColumnHeaderSpanModel;
        }

        internal override Size GetViewportSize(Size availableSize)
        {
            SheetLayout sheetLayout = Sheet.GetSheetLayout();
            double viewportWidth = sheetLayout.GetViewportWidth(base.ColumnViewportIndex);
            double headerHeight = sheetLayout.HeaderHeight;
            viewportWidth = Math.Min(viewportWidth, availableSize.Width);
            return new Size(viewportWidth, Math.Min(headerHeight, availableSize.Height));
        }

        internal override List<RowItem> RecycledRows
        {
            get
            {
                if (_recycledColHeaderRows == null)
                {
                    _recycledColHeaderRows = new List<RowItem>();
                }
                return _recycledColHeaderRows;
            }
        }

        internal override bool SupportCellOverflow
        {
            get { return  false; }
        }
    }
}

