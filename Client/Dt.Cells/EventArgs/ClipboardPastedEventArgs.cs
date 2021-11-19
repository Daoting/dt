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
    /// <summary>
    /// Represents the event data for the ClipboardPasting event for the GcSpreadSheet component; occurs when the user invokes the Clipboard paste action. 
    /// </summary>
    public class ClipboardPastedEventArgs : EventArgs
    {
        internal ClipboardPastedEventArgs(Dt.Cells.Data.Worksheet sourceSheet, Dt.Cells.Data.CellRange sourceRange, Dt.Cells.Data.Worksheet worksheet, Dt.Cells.Data.CellRange cellRange, ClipboardPasteOptions pasteOption)
        {
            SourceSheet = sourceSheet;
            SourceRange = sourceRange;
            Worksheet = worksheet;
            CellRange = cellRange;
            PasteOption = pasteOption;
        }

        /// <summary>
        /// Gets the cell range when pasting.
        /// </summary>
        /// <value>The pasted cell range.</value>
        public Dt.Cells.Data.CellRange CellRange { get; private set; }

        /// <summary>
        /// Gets the ClipboardPasteOptions value when pasting.
        /// </summary>
        /// <value>The ClipboardPasteOptions value when pasting.</value>
        public ClipboardPasteOptions PasteOption { get; private set; }

        /// <summary>
        /// Gets the source range.
        /// </summary>
        /// <value>
        /// The source range.
        /// </value>
        public Dt.Cells.Data.CellRange SourceRange { get; private set; }

        /// <summary>
        /// Gets the source sheet.
        /// </summary>
        /// <value>
        /// The source sheet.
        /// </value>
        public Dt.Cells.Data.Worksheet SourceSheet { get; private set; }

        /// <summary>
        /// Gets the worksheet.
        /// </summary>
        /// <value>
        /// The worksheet.
        /// </value>
        public Dt.Cells.Data.Worksheet Worksheet { get; private set; }
    }
}

