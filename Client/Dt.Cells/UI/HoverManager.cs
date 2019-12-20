#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    internal class HoverManager
    {
        private HitTestInformation _oldHi;
        private SheetView _view;

        public HoverManager(SheetView view)
        {
            this._view = view;
            this._oldHi = null;
        }

        private void DoColumnHeaderHover(HitTestInformation hi)
        {
            this.IsMouseOverColumnHeaders = true;
            this._view.MouseOverColumnIndex = hi.HeaderInfo.Column;
            if (hi.HeaderInfo.InColumnResize)
            {
                this._view.MouseOverColumnIndex = -1;
            }
            this._view.UpdateColumnHeaderCellsState(this._view.Worksheet.ColumnHeader.RowCount - 1, hi.HeaderInfo.Column, 1, 1);
        }

        private void DoCornerHeaderHover(HitTestInformation hi)
        {
            this.IsMouseOverCornerHeaders = true;
            this._view.UpdateCornerHeaderCellState();
        }

        public void DoHover(HitTestInformation hi)
        {
            if (!object.ReferenceEquals(this._oldHi, hi))
            {
                this.IsMouseOverCornerHeaders = false;
                this.IsMouseOverRowHeaders = false;
                this.IsMouseOverColumnHeaders = false;
                this.IsMouseOverViewports = false;
                if (this._oldHi != null)
                {
                    switch (this._oldHi.HitTestType)
                    {
                        case HitTestType.RowHeader:
                            this._view.UpdateRowHeaderCellsState(this._oldHi.HeaderInfo.Row, this._view.Worksheet.RowHeader.ColumnCount - 1, 1, 1);
                            break;

                        case HitTestType.ColumnHeader:
                            this._view.UpdateColumnHeaderCellsState(this._view.Worksheet.ColumnHeader.RowCount - 1, this._oldHi.HeaderInfo.Column, 1, 1);
                            break;

                        case HitTestType.Corner:
                            this._view.UpdateCornerHeaderCellState();
                            break;
                    }
                }
                switch (hi.HitTestType)
                {
                    case HitTestType.Corner:
                        this.DoCornerHeaderHover(hi);
                        break;

                    case HitTestType.RowHeader:
                        this.DoRowHeaderHover(hi);
                        break;

                    case HitTestType.ColumnHeader:
                        this.DoColumnHeaderHover(hi);
                        break;

                    case HitTestType.Viewport:
                        this.IsMouseOverViewports = true;
                        break;
                }
                this._oldHi = hi;
            }
        }

        private void DoRowHeaderHover(HitTestInformation hi)
        {
            this.IsMouseOverRowHeaders = true;
            this._view.MouseOverRowIndex = hi.HeaderInfo.Row;
            if (hi.HeaderInfo.InRowResize)
            {
                this._view.MouseOverRowIndex = -1;
            }
            this._view.UpdateRowHeaderCellsState(hi.HeaderInfo.Row, this._view.Worksheet.RowHeader.ColumnCount - 1, 1, 1);
        }

        internal bool IsMouseOverColumnHeaders { get; set; }

        internal bool IsMouseOverCornerHeaders { get; set; }

        internal bool IsMouseOverRowHeaders { get; set; }

        internal bool IsMouseOverViewports { get; set; }
    }
}

