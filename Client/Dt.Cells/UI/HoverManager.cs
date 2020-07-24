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
        HitTestInformation _oldHi;
        SheetView _view;

        public HoverManager(SheetView view)
        {
            _view = view;
            _oldHi = null;
        }

        void DoColumnHeaderHover(HitTestInformation hi)
        {
            IsMouseOverColumnHeaders = true;
            _view.MouseOverColumnIndex = hi.HeaderInfo.Column;
            if (hi.HeaderInfo.InColumnResize)
            {
                _view.MouseOverColumnIndex = -1;
            }
            _view.UpdateColumnHeaderCellsState(_view.Worksheet.ColumnHeader.RowCount - 1, hi.HeaderInfo.Column, 1, 1);
        }

        void DoCornerHeaderHover(HitTestInformation hi)
        {
            IsMouseOverCornerHeaders = true;
            _view.UpdateCornerHeaderCellState();
        }

        public void DoHover(HitTestInformation hi)
        {
            if (!object.ReferenceEquals(_oldHi, hi))
            {
                IsMouseOverCornerHeaders = false;
                IsMouseOverRowHeaders = false;
                IsMouseOverColumnHeaders = false;
                IsMouseOverViewports = false;
                if (_oldHi != null)
                {
                    switch (_oldHi.HitTestType)
                    {
                        case HitTestType.RowHeader:
                            _view.UpdateRowHeaderCellsState(_oldHi.HeaderInfo.Row, _view.Worksheet.RowHeader.ColumnCount - 1, 1, 1);
                            break;

                        case HitTestType.ColumnHeader:
                            _view.UpdateColumnHeaderCellsState(_view.Worksheet.ColumnHeader.RowCount - 1, _oldHi.HeaderInfo.Column, 1, 1);
                            break;

                        case HitTestType.Corner:
                            _view.UpdateCornerHeaderCellState();
                            break;
                    }
                }
                switch (hi.HitTestType)
                {
                    case HitTestType.Corner:
                        DoCornerHeaderHover(hi);
                        break;

                    case HitTestType.RowHeader:
                        DoRowHeaderHover(hi);
                        break;

                    case HitTestType.ColumnHeader:
                        DoColumnHeaderHover(hi);
                        break;

                    case HitTestType.Viewport:
                        IsMouseOverViewports = true;
                        break;
                }
                _oldHi = hi;
            }
        }

        void DoRowHeaderHover(HitTestInformation hi)
        {
            IsMouseOverRowHeaders = true;
            _view.MouseOverRowIndex = hi.HeaderInfo.Row;
            if (hi.HeaderInfo.InRowResize)
            {
                _view.MouseOverRowIndex = -1;
            }
            _view.UpdateRowHeaderCellsState(hi.HeaderInfo.Row, _view.Worksheet.RowHeader.ColumnCount - 1, 1, 1);
        }

        internal bool IsMouseOverColumnHeaders { get; set; }

        internal bool IsMouseOverCornerHeaders { get; set; }

        internal bool IsMouseOverRowHeaders { get; set; }

        internal bool IsMouseOverViewports { get; set; }
    }
}

