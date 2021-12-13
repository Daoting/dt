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
    internal class DataValidationListButtonInfo
    {
        public DataValidationListButtonInfo(DataValidator validator)
        {
            RowViewportIndex = -2;
            ColumnViewportIndex = -2;
            Validator = validator;
        }

        public DataValidationListButtonInfo(DataValidator validator, int row, int column, Dt.Cells.Data.SheetArea sheetArea)
        {
            Validator = validator;
            Row = row;
            Column = column;
            SheetArea = sheetArea;
            RowViewportIndex = -2;
            ColumnViewportIndex = -2;
        }

        public int Column { get; set; }

        public int ColumnViewportIndex { get; set; }

        public int DisplayColumn { get; set; }

        public int Row { get; set; }

        public int RowViewportIndex { get; set; }

        public Dt.Cells.Data.SheetArea SheetArea { get; set; }

        public DataValidator Validator { get; set; }
    }
}

