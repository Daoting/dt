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
    public class FloatingObjectPastedEventArgs : EventArgs
    {
        internal FloatingObjectPastedEventArgs(Dt.Cells.Data.Worksheet worksheet, FloatingObject pastedObject)
        {
            Worksheet = worksheet;
            PastedObject = pastedObject;
        }

        /// <summary>
        /// Gets the pasted object.
        /// </summary>
        /// <value>
        /// The pasted object.
        /// </value>
        public FloatingObject PastedObject { get; private set; }

        /// <summary>
        /// Gets the worksheet.
        /// </summary>
        /// <value>
        /// The worksheet.
        /// </value>
        public Dt.Cells.Data.Worksheet Worksheet { get; private set; }
    }
}

