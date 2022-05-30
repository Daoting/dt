#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UndoRedo;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the data for the CellChanged event.
    /// </summary>
    public class ValidationErrorEventArgs : EventArgs
    {
        int column;
        int row;
        DataValidator validator;

        internal ValidationErrorEventArgs(int row, int column, DataValidator validator)
        {
            this.row = row;
            this.column = column;
            this.validator = validator;
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        /// <value>The column index.</value>
        public int Column
        {
            get { return  column; }
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        /// <value>The row index.</value>
        public int Row
        {
            get { return  row; }
        }

        /// <summary>
        /// Gets or sets the policy that the user can set to determine how to process the error.
        /// </summary>
        public DataValidationResult ValidationResult { get; set; }

        /// <summary>
        /// Gets the validator which caused this error. 
        /// This validator is a copy of the real validator, so any modifications to this validator do not take effect.
        /// </summary>
        public DataValidator Validator
        {
            get { return  validator; }
        }
    }
}

