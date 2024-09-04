#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides data for the FloatingObjectSelectionChanged event.
    /// </summary>
    public class FloatingObjectSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.FloatingObjectSelectionChangedEventArgs" /> class.
        /// </summary>
        /// <param name="floatingObject">The floating object.</param>
        public FloatingObjectSelectionChangedEventArgs(Dt.Cells.Data.FloatingObject floatingObject)
        {
            this.FloatingObject = floatingObject;
        }

        /// <summary>
        /// Gets the floating object.
        /// </summary>
        /// <value>
        /// The floating object.
        /// </value>
        public Dt.Cells.Data.FloatingObject FloatingObject { get; private set; }
    }
}

