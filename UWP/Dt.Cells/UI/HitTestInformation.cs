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
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents hit test information for the data area of the spreadsheet.
    /// </summary>
    public class HitTestInformation
    {
        /// <summary>
        /// Creates a new set of hit test information.
        /// </summary>
        internal HitTestInformation()
        {
            HitTestType = HitTestType.Empty;
            RowViewportIndex = -2;
            ColumnViewportIndex = -2;
        }

        /// <summary>
        /// Gets the column viewport referred to in the spreadsheet.
        /// </summary>
        public int ColumnViewportIndex { get; internal set; }

        /// <summary>
        /// Gets the chart information.
        /// </summary>
        public ViewportFloatingObjectHitTestInformation FloatingObjectInfo { get; internal set; }

        /// <summary>
        /// Gets more detailed hit test information for the FormulaSelection.
        /// </summary>
        public ViewportFormulaSelectionHitTestInformation FormulaSelectionInfo { get; internal set; }

        /// <summary>
        /// Gets more detailed hit test information for the headers (column headers, row headers, and corner).
        /// </summary>
        public HeaderHitTestInformation HeaderInfo { get; internal set; }

        internal Point HitPoint { get; set; }

        /// <summary>
        /// Gets the general area referred to in the spreadsheet.
        /// </summary>
        public HitTestType HitTestType { get; internal set; }

        /// <summary>
        /// Gets the row viewport referred to in the spreadsheet.
        /// </summary>
        public int RowViewportIndex { get; internal set; }

        /// <summary>
        /// Gets more detailed hit test information for the viewport.
        /// </summary>
        public ViewportHitTestInformation ViewportInfo { get; internal set; }
    }
}

