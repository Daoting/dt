#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents the warnings arising from loading or saving Excel files.
    /// </summary>
    public class ExcelWarning
    {
        private ExcelWarningCode code;
        private int column;
        private Exception innerException;
        private string message;
        private int row;
        private int sheet;

        /// <summary>
        /// Constructor -- internal use only
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="code">The error code used to specify error category</param>
        public ExcelWarning(string message, ExcelWarningCode code) : this(message, code, -1, -1, -1, null)
        {
        }

        /// <summary>
        /// Constructor -- internal use only
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="code">The error code used to specify error category</param>
        /// <param name="sheet">The zero based sheet index.</param>
        /// <param name="row">The zero based row index</param>
        /// <param name="column">The zero based column index</param>
        public ExcelWarning(string message, ExcelWarningCode code, int sheet, int row, int column) : this(message, code, sheet, row, column, null)
        {
        }

        /// <summary>
        /// Constructor -- internal use only
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="code">The error code used to specify error category</param>
        /// <param name="sheet">The zero based sheet index.</param>
        /// <param name="row">The zero based row index</param>
        /// <param name="column">The zero based column index</param>
        /// <param name="exception">the actual exception occurred in Excel IE component.</param>
        public ExcelWarning(string message, ExcelWarningCode code, int sheet, int row, int column, Exception exception)
        {
            this.sheet = -1;
            this.row = -1;
            this.column = -1;
            this.message = message;
            this.code = code;
            this.sheet = sheet;
            this.row = row;
            this.column = column;
            this.innerException = exception;
        }

        /// <summary>
        /// Gets the Excel warning code.
        /// </summary>
        public ExcelWarningCode Code
        {
            get { return  this.code; }
        }

        /// <summary>
        /// Gets the index of the column where the warning pertains.
        /// </summary>
        public int Column
        {
            get { return  this.column; }
        }

        /// <summary>
        /// Gets the actual exception occurred in Excel IE component.
        /// </summary>
        public Exception InnerException
        {
            get { return  this.innerException; }
        }

        /// <summary>
        /// Gets the string description of the warning.
        /// </summary>
        public string Message
        {
            get { return  this.message; }
        }

        /// <summary>
        /// Gets the index of the row where the warning pertains.
        /// </summary>
        public int Row
        {
            get { return  this.row; }
        }

        /// <summary>
        /// Gets the index of the applicable sheet where the warning pertains.
        /// </summary>
        public int Sheet
        {
            get { return  this.sheet; }
        }

        /// <summary>
        /// Represents the unsupported biff records in biff file
        /// </summary>
        public List<UnsupportedBiffRecord> UnsupportedBiffRecords { get; internal set; }

        /// <summary>
        /// Represents the unsupported xml content in open xml file
        /// </summary>
        public string UnsupportedOpenXmlRecords { get; internal set; }
    }
}

