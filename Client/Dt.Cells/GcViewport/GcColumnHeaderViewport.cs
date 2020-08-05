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
#endregion

namespace Dt.Cells.UI
{
    internal partial class GcColumnHeaderViewport : GcViewport
    {
        List<RowPresenter> _recycledColHeaderRows;

        public GcColumnHeaderViewport(SheetView sheet) : base(sheet, SheetArea.ColumnHeader, false)
        {
            _recycledColHeaderRows = new List<RowPresenter>();
            base._sheetArea = SheetArea.ColumnHeader;
        }

        internal override RowPresenter GenerateNewRow()
        {
            return new ColumnHeaderRowPresenter(this);
        }

        internal override CellLayoutModel GetCellLayoutModel()
        {
            return base.Sheet.GetColumnHeaderCellLayoutModel(base.ColumnViewportIndex);
        }

        internal override ICellsSupport GetDataContext()
        {
            return base.Sheet.Worksheet.ColumnHeader;
        }

        internal override RowLayoutModel GetRowLayoutModel()
        {
            return base.Sheet.GetColumnHeaderRowLayoutModel();
        }

        internal override SheetSpanModelBase GetSpanModel()
        {
            return base.Sheet.Worksheet.ColumnHeaderSpanModel;
        }

        internal override Windows.Foundation.Size GetViewportSize(Windows.Foundation.Size availableSize)
        {
            SheetLayout sheetLayout = base.Sheet.GetSheetLayout();
            double viewportWidth = sheetLayout.GetViewportWidth(base.ColumnViewportIndex);
            double headerHeight = sheetLayout.HeaderHeight;
            viewportWidth = Math.Min(viewportWidth, availableSize.Width);
            return new Windows.Foundation.Size(viewportWidth, Math.Min(headerHeight, availableSize.Height));
        }

        internal override List<RowPresenter> RecycledRows
        {
            get
            {
                if (_recycledColHeaderRows == null)
                {
                    _recycledColHeaderRows = new List<RowPresenter>();
                }
                return _recycledColHeaderRows;
            }
        }

        internal override bool SupportCellOverflow
        {
            get { return  false; }
        }

        protected override bool SupportSelection
        {
            get { return  false; }
        }
    }
}

