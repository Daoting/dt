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
    public class FloatingObjectChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.FloatingObjectChangedEventArgs" /> class.
        /// </summary>
        /// <param name="floatingObject">The floating object.</param>
        /// <param name="property">The property.</param>
        public FloatingObjectChangedEventArgs(Dt.Cells.Data.FloatingObject floatingObject, string property)
        {
            this.FloatingObject = floatingObject;
            this.Property = property;
        }

        /// <summary>
        /// Gets the floating object.
        /// </summary>
        /// <value>
        /// The floating object.
        /// </value>
        public Dt.Cells.Data.FloatingObject FloatingObject { get; private set; }

        /// <summary>
        /// Gets the Property.
        /// </summary>
        /// <value>
        /// The Property.
        /// </value>
        public string Property { get; private set; }
    }
}

