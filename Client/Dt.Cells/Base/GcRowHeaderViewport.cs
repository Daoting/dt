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
    internal partial class GcRowHeaderViewport : GcViewport
    {
        List<RowPresenter> _recycledRowHeaderRows;

        public GcRowHeaderViewport(SheetView sheet) : base(sheet, SheetArea.CornerHeader | SheetArea.RowHeader, false)
        {
            _recycledRowHeaderRows = new List<RowPresenter>();
            base._sheetArea = SheetArea.CornerHeader | SheetArea.RowHeader;
        }

        internal override RowPresenter GenerateNewRow()
        {
            return new RowHeaderRowPresenter(this);
        }

        internal override CellLayoutModel GetCellLayoutModel()
        {
            return base.Sheet.GetRowHeaderCellLayoutModel(base.RowViewportIndex);
        }

        internal override ICellsSupport GetDataContext()
        {
            return base.Sheet.Worksheet.RowHeader;
        }

        internal override RowLayoutModel GetRowLayoutModel()
        {
            return base.Sheet.GetViewportRowLayoutModel(base.RowViewportIndex);
        }

        internal override SheetSpanModelBase GetSpanModel()
        {
            return base.Sheet.Worksheet.RowHeaderSpanModel;
        }

        internal override Windows.Foundation.Size GetViewportSize(Windows.Foundation.Size availableSize)
        {
            SheetLayout sheetLayout = base.Sheet.GetSheetLayout();
            double headerWidth = sheetLayout.HeaderWidth;
            double viewportHeight = sheetLayout.GetViewportHeight(base.RowViewportIndex);
            headerWidth = Math.Min(headerWidth, availableSize.Width);
            return new Windows.Foundation.Size(headerWidth, Math.Min(viewportHeight, availableSize.Height));
        }

        internal override List<RowPresenter> RecycledRows
        {
            get
            {
                if (_recycledRowHeaderRows == null)
                {
                    _recycledRowHeaderRows = new List<RowPresenter>();
                }
                return _recycledRowHeaderRows;
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

