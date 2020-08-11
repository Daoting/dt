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
    internal partial class RowHeaderPanel : CellsPanel
    {
        List<RowItem> _recycledRowHeaderRows;

        public RowHeaderPanel(SheetView sheet) : base(sheet, SheetArea.CornerHeader | SheetArea.RowHeader)
        {
            _recycledRowHeaderRows = new List<RowItem>();
        }

        internal override RowItem GenerateNewRow()
        {
            return new RowHeaderItem(this);
        }

        internal override CellLayoutModel GetCellLayoutModel()
        {
            return Sheet.GetRowHeaderCellLayoutModel(RowViewportIndex);
        }

        internal override ICellsSupport GetDataContext()
        {
            return Sheet.ActiveSheet.RowHeader;
        }

        internal override RowLayoutModel GetRowLayoutModel()
        {
            return Sheet.GetViewportRowLayoutModel(RowViewportIndex);
        }

        internal override SheetSpanModelBase GetSpanModel()
        {
            return Sheet.ActiveSheet.RowHeaderSpanModel;
        }

        internal override Size GetViewportSize(Size availableSize)
        {
            SheetLayout sheetLayout = Sheet.GetSheetLayout();
            double headerWidth = sheetLayout.HeaderWidth;
            double viewportHeight = sheetLayout.GetViewportHeight(RowViewportIndex);
            headerWidth = Math.Min(headerWidth, availableSize.Width);
            return new Size(headerWidth, Math.Min(viewportHeight, availableSize.Height));
        }

        internal override List<RowItem> RecycledRows
        {
            get
            {
                if (_recycledRowHeaderRows == null)
                {
                    _recycledRowHeaderRows = new List<RowItem>();
                }
                return _recycledRowHeaderRows;
            }
        }

        internal override bool SupportCellOverflow
        {
            get { return  false; }
        }
    }
}

