#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a collection that notifies listeners of dynamic changes when items are added and removed or the entire collection object is reset. 
    /// </summary>
    public class NotifyCollection<T> : NotifyCollectionBase<T>
    {
        /// <summary>
        /// Creates a new notify collection.
        /// </summary>
        public NotifyCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.NotifyCollection`1" /> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public NotifyCollection(IList<T> items)
        {
            if (items != null)
            {
                base.SuspendEvent();
                foreach (T local in items)
                {
                    this.Add(local);
                }
                base.ResumeEvent();
            }
        }
    }
}

