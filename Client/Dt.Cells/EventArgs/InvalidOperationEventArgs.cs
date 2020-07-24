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
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the event for an invalid operation.
    /// </summary>
    public class InvalidOperationEventArgs : EventArgs
    {
        internal InvalidOperationEventArgs(string message) : this(message, null, null)
        {
        }

        internal InvalidOperationEventArgs(string message, string operation, object context)
        {
            Message = message;
            Operation = operation;
            Context = context;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public object Context { get; private set; }

        /// <summary>
        /// Gets the description of the invalid operation.
        /// </summary>
        /// <value>The description of the invalid operation.</value>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public string Operation { get; private set; }
    }
}

