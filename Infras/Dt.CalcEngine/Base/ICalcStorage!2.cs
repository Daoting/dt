#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represent a storage for <typeparamref name="TValue" />.
    /// </summary>
    /// <typeparam name="TKey">Indicates the key for a stored item</typeparam>
    /// <typeparam name="TValue">Indicates the type for stored item.</typeparam>
    public interface ICalcStorage<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable where TKey: IEqualityComparer<TKey>
    {
        /// <summary>
        /// Removes items at the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        void RemoveAt(TKey id);

        /// <summary>
        /// Get count of items.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or sets the <typeparamref name="TValue" /> at the specified <see cref="T:Dt.CalcEngine.CalcIdentity">id</see>.
        /// </summary>
        /// <value>A <see cref="T:Dt.CalcEngine.CalcIdentity" /> indicates the data address.</value>
        /// <remarks>
        /// When get value, if item which <param name="id" /> is not existed, return <see langword="null" />.
        /// When set value, if <paramref name="id" /> is not existed, means add a new item to storage.
        /// </remarks>
        TValue this[TKey id] { get; set; }
    }
}

