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
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class EditorInfo
    {
        SheetView _sheetView;

        internal EditorInfo(SheetView sheetView)
        {
            _sheetView = sheetView;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ColumnIndex
        {
            get
            {
                if (_sheetView.EditorConnector.IsInOtherSheet)
                {
                    return _sheetView.EditorConnector.ColumnIndex;
                }
                return _sheetView.Worksheet.ActiveColumnIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RowIndex
        {
            get
            {
                if (_sheetView.EditorConnector.IsInOtherSheet)
                {
                    return _sheetView.EditorConnector.RowIndex;
                }
                return _sheetView.Worksheet.ActiveRowIndex;
            }
        }

        /// <summary>
        /// Gets the sheet.
        /// </summary>
        /// <value>
        /// The sheet.
        /// </value>
        public Worksheet Sheet
        {
            get
            {
                if (_sheetView.EditorConnector.IsInOtherSheet)
                {
                    return _sheetView.Worksheet.Workbook.Sheets[_sheetView.EditorConnector.SheetIndex];
                }
                return _sheetView.Worksheet.Workbook.ActiveSheet;
            }
        }
    }
}

