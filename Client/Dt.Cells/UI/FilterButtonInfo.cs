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
            this.RowViewportIndex = -2;
            this.ColumnViewportIndex = -2;
            this.RowFilter = rowFilter;
        }

        public FilterButtonInfo(HideRowFilter rowFilter, int row, int column, Dt.Cells.Data.SheetArea sheetArea)
        {
            this.RowFilter = rowFilter;
            this.Row = row;
            this.Column = column;
            this.SheetArea = sheetArea;
            this.RowViewportIndex = -2;
            this.ColumnViewportIndex = -2;
        }

        public SortState GetSortState()
        {
            if ((this.RowFilter != null) && (this.RowFilter.GetSorttedColumnIndex() == this.Column))
            {
                return this.RowFilter.GetColumnSortState();
            }
            return SortState.None;
        }

        public bool IsFiltered()
        {
            return ((this.RowFilter != null) && this.RowFilter.IsColumnFiltered(this.Column));
        }

        public int Column { get; set; }

        public int ColumnViewportIndex { get; set; }

        public int Row { get; set; }

        public HideRowFilter RowFilter { get; set; }

        public int RowViewportIndex { get; set; }

        public Dt.Cells.Data.SheetArea SheetArea { get; set; }
    }
}

