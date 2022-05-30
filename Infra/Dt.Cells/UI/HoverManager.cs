#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    internal class HoverManager
    {
        HitTestInformation _oldHi;
        Excel _excel;

        public HoverManager(Excel p_excel)
        {
            _excel = p_excel;
        }

        void DoColumnHeaderHover(HitTestInformation hi)
        {
            IsMouseOverColumnHeaders = true;
            _excel.MouseOverColumnIndex = hi.HeaderInfo.Column;
            if (hi.HeaderInfo.InColumnResize)
            {
                _excel.MouseOverColumnIndex = -1;
            }
            _excel.UpdateColumnHeaderCellsState(_excel.ActiveSheet.ColumnHeader.RowCount - 1, hi.HeaderInfo.Column, 1, 1);
        }

        void DoCornerHeaderHover(HitTestInformation hi)
        {
            IsMouseOverCornerHeaders = true;
            _excel.UpdateCornerHeaderCellState();
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
                            _excel.UpdateRowHeaderCellsState(_oldHi.HeaderInfo.Row, _excel.ActiveSheet.RowHeader.ColumnCount - 1, 1, 1);
                            break;

                        case HitTestType.ColumnHeader:
                            _excel.UpdateColumnHeaderCellsState(_excel.ActiveSheet.ColumnHeader.RowCount - 1, _oldHi.HeaderInfo.Column, 1, 1);
                            break;

                        case HitTestType.Corner:
                            _excel.UpdateCornerHeaderCellState();
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
            _excel.MouseOverRowIndex = hi.HeaderInfo.Row;
            if (hi.HeaderInfo.InRowResize)
            {
                _excel.MouseOverRowIndex = -1;
            }
            _excel.UpdateRowHeaderCellsState(hi.HeaderInfo.Row, _excel.ActiveSheet.RowHeader.ColumnCount - 1, 1, 1);
        }

        internal bool IsMouseOverColumnHeaders { get; set; }

        internal bool IsMouseOverCornerHeaders { get; set; }

        internal bool IsMouseOverRowHeaders { get; set; }

        internal bool IsMouseOverViewports { get; set; }
    }
}

