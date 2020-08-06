#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides information when an error occurs during loading or saving an excel file.
    /// </summary>
    public class ExcelErrorEventArgs : EventArgs
    {
        Dt.Xls.ExcelWarning _warning;

        internal ExcelErrorEventArgs(Dt.Xls.ExcelWarning warning)
        {
            this._warning = warning;
        }

        /// <summary>
        /// An <see cref="P:Dt.Cells.Data.ExcelErrorEventArgs.ExcelWarning" /> instance used to provide error details.
        /// </summary>
        public Dt.Xls.ExcelWarning ExcelWarning
        {
            get { return  this._warning; }
        }
    }
}

