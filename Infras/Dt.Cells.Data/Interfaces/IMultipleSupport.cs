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
    /// Interface that supports operations on ranges of items. 
    /// </summary>
    internal interface IMultipleSupport
    {
        /// <summary>
        /// Adds items after the specified item.
        /// </summary>
        /// <param name="index">The item index at which to start adding items.</param>
        /// <param name="count">The number of items to add.</param>
        void Add(int index, int count);
        /// <summary>
        /// Clears items, starting at and including the specified position.
        /// </summary>
        /// <param name="from">Item index at which to start clearing.</param>
        /// <param name="count">Number of items to clear.</param>
        void Clear(int from, int count);
        /// <summary>
        /// Copies items, starting at and including the specified position.
        /// </summary>
        /// <param name="from">The item index from which to copy.</param>
        /// <param name="to">The item index of the copy destination.</param>
        /// <param name="count">The number of items to copy.</param>
        void Copy(int from, int to, int count);
        /// <summary>
        /// Moves items, starting at and including the specified position.
        /// </summary>
        /// <param name="from">The item index at which to start to move items.</param>
        /// <param name="to">The item index of the move destination.</param>
        /// <param name="count">The number of items to move.</param>
        void Move(int from, int to, int count);
        /// <summary>
        /// Removes items, starting at the specified position.
        /// </summary>
        /// <param name="index">The item index at which to start removing items.</param>
        /// <param name="count">The number of items to remove.</param>
        void Remove(int index, int count);
        /// <summary>
        /// Swaps items at the specified positions.
        /// </summary>
        /// <param name="from">The item index from which to swap.</param>
        /// <param name="to">The item index of the swap destination.</param>
        /// <param name="count">The number of items to swap.</param>
        void Swap(int from, int to, int count);

        /// <summary>
        /// Gets the number of items.
        /// </summary>
        /// <value>The number of items.</value>
        int Count { get; }
    }
}

