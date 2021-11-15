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

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// 
    /// </summary>
    public class FloatingObjectExtent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.FloatingObjectExtent" /> class.
        /// </summary>
        /// <param name="names">The names.</param>
        public FloatingObjectExtent(params string[] names)
        {
            Names = names;
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public string[] Names { get; private set; }
    }
}

