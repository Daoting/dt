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
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the event data for the ClipboardPasting event for the GcSpreadSheet component; occurs when the user invokes the Clipboard paste action. 
    /// </summary>
    public class ClipboardPastingEventArgs : CancelEventArgs
    {
        internal ClipboardPastingEventArgs(Dt.Cells.Data.Worksheet sourceSheet, Dt.Cells.Data.CellRange sourceRange, Dt.Cells.Data.Worksheet worksheet, Dt.Cells.Data.CellRange cellRange, ClipboardPasteOptions pasteOption, bool isCutting)
        {
            Worksheet = worksheet;
            CellRange = cellRange;
            SourceSheet = sourceSheet;
            SourceRange = sourceRange;
            PasteOption = pasteOption;
            IsCutting = isCutting;
        }

        internal ClipboardPastingEventArgs(Dt.Cells.Data.Worksheet sourceSheet, Dt.Cells.Data.CellRange sourceRange, Dt.Cells.Data.Worksheet worksheet, Dt.Cells.Data.CellRange cellRange, ClipboardPasteOptions pasteOption, bool isCutting, bool cancel) : this(sourceSheet, sourceRange, worksheet, cellRange, pasteOption, isCutting)
        {
            base.Cancel = cancel;
        }

        /// <summary>
        /// Gets the cell range for pasting.
        /// </summary>
        /// <value>Pasted cell range.</value>
        public Dt.Cells.Data.CellRange CellRange { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is cutting.
        /// </summary>
        /// <value>
        /// <c>true</c> if is cutting; otherwise, <c>false</c>.
        /// </value>
        public bool IsCutting { get; private set; }

        /// <summary>
        /// Gets the ClipboardPasteOptions value when pasting.
        /// </summary>
        /// <value>ClipboardPasteOptions value when pasting.</value>
        public ClipboardPasteOptions PasteOption { get; set; }

        /// <summary>
        /// Gets the soruce range.
        /// </summary>
        /// <value>
        /// The soruce range.
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
        /// Gets the worksheet for patsting.
        /// </summary>
        /// <value>
        /// The worksheet.
        /// </value>
        public Dt.Cells.Data.Worksheet Worksheet { get; private set; }
    }
}

