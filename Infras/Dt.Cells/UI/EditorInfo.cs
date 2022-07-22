#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class EditorInfo
    {
        Excel _excel;

        internal EditorInfo(Excel p_excel)
        {
            _excel = p_excel;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ColumnIndex
        {
            get { return _excel.ActiveSheet.ActiveColumnIndex; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int RowIndex
        {
            get { return _excel.ActiveSheet.ActiveRowIndex; }
        }

        /// <summary>
        /// Gets the sheet.
        /// </summary>
        /// <value>
        /// The sheet.
        /// </value>
        public Worksheet Sheet
        {
            get { return _excel.ActiveSheet.Workbook.ActiveSheet; }
        }
    }
}

