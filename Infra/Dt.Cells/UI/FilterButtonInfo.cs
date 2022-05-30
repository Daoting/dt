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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    internal class FilterButtonInfo
    {
        public FilterButtonInfo(HideRowFilter rowFilter)
        {
            RowViewportIndex = -2;
            ColumnViewportIndex = -2;
            RowFilter = rowFilter;
        }

        public FilterButtonInfo(HideRowFilter rowFilter, int row, int column, Dt.Cells.Data.SheetArea sheetArea)
        {
            RowFilter = rowFilter;
            Row = row;
            Column = column;
            SheetArea = sheetArea;
            RowViewportIndex = -2;
            ColumnViewportIndex = -2;
        }

        public SortState GetSortState()
        {
            if ((RowFilter != null) && (RowFilter.GetSorttedColumnIndex() == Column))
            {
                return RowFilter.GetColumnSortState();
            }
            return SortState.None;
        }

        public bool IsFiltered()
        {
            return ((RowFilter != null) && RowFilter.IsColumnFiltered(Column));
        }

        public int Column { get; set; }

        public int ColumnViewportIndex { get; set; }

        public int Row { get; set; }

        public HideRowFilter RowFilter { get; set; }

        public int RowViewportIndex { get; set; }

        public Dt.Cells.Data.SheetArea SheetArea { get; set; }
    }
}

