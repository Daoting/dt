#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a suspend state.
    /// </summary>
    internal sealed class WorkingState
    {
        /// <summary>
        /// ref count
        /// </summary>
        short countRef;

        /// <summary>
        /// Adds a counter. 
        /// </summary>
        public void AddRef()
        {
            this.countRef++;
        }

        /// <summary>
        /// Releases the counter.  
        /// </summary>
        public void Release()
        {
            this.countRef--;
            if (this.countRef <= 0)
            {
                this.countRef = 0;
            }
        }

        /// <summary>
        /// Resets the counter.
        /// </summary>
        public void Reset()
        {
            this.countRef = 0;
        }

        /// <summary>
        /// Gets a value that indicates whether the counter is greater than 0.
        /// </summary>
        public bool IsWorking
        {
            get { return  (this.countRef > 0); }
        }
    }
}

