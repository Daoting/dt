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
    /// 
    /// </summary>
    public class ValidationPastingEventArgs : EventArgs
    {
        internal ValidationPastingEventArgs(Worksheet fromsheet, CellRange fromRange, Worksheet toSheet, CellRange toRange, CellRange pastingRange, bool isCutting)
        {
            FromSheet = fromsheet;
            FromRange = fromRange;
            ToWorksheet = toSheet;
            ToRange = toRange;
            PastingRange = pastingRange;
            IsCutting = isCutting;
        }

        /// <summary>
        /// Gets the source range.
        /// </summary>
        /// <value>
        /// The source range.
        /// </value>
        public CellRange FromRange { get; private set; }

        /// <summary>
        /// Gets the source sheet.
        /// </summary>
        /// <value>
        /// The source sheet.
        /// </value>
        public Worksheet FromSheet { get; private set; }

        /// <summary>
        /// Gets or sets whether the event is handled.
        /// </summary>
        /// <value>
        /// <c>true</c> if [handle]; otherwise, <c>false</c>.
        /// </value>
        public bool Handle { get; set; }

        /// <summary>
        /// Gets or sets the invalid message.
        /// </summary>
        /// <value>
        /// The invalid message.
        /// </value>
        public string InvalidMessage { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the data is cut.
        /// </summary>
        /// <value>
        /// <c>true</c> if [is cutting]; otherwise, <c>false</c>.
        /// </value>
        public bool IsCutting { get; private set; }

        /// <summary>
        /// Gets or sets whether the value is invalid.
        /// </summary>
        /// <value>
        /// <c>true</c> if [is invalid]; otherwise, <c>false</c>.
        /// </value>
        public bool IsInvalid { get; set; }

        /// <summary>
        /// Gets the pasting range.
        /// </summary>
        /// <value>
        /// The pasting range.
        /// </value>
        public CellRange PastingRange { get; private set; }

        /// <summary>
        /// Gets the cell range when pasting.
        /// </summary>
        /// <value>The pasted cell range.</value>
        public CellRange ToRange { get; private set; }

        /// <summary>
        /// Gets the worksheet.
        /// </summary>
        /// <value>
        /// The worksheet.
        /// </value>
        public Worksheet ToWorksheet { get; private set; }
    }
}

