#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-04-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Specialized;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 具有集合变化通知的列表
    /// </summary>
    public interface INotifyList : INotifyCollectionChanged, IList
    {
        /// <summary>
        /// 在末尾批量添加数据，统一触发 NotifyCollectionChangedAction.Add
        /// </summary>
        /// <param name="p_items"></param>
        void AddRange(IList p_items);

        /// <summary>
        /// 在指定的索引处批量插入数据，统一触发 NotifyCollectionChangedAction.Add
        /// </summary>
        /// <param name="p_index">插入位置，范围在0到当前总数之间</param>
        /// <param name="p_items">数据行</param>
        void InsertRange(int p_index, IList p_items);

        /// <summary>
        /// 批量删除给定的行，统一触发 NotifyCollectionChangedAction.Remove
        /// </summary>
        /// <param name="p_items">数据行</param>
        /// <returns>实际删除的行数</returns>
        int RemoveRange(IList p_items);

        /// <summary>
        /// 批量删除指定范围的行，统一触发 NotifyCollectionChangedAction.Remove
        /// </summary>
        /// <param name="p_index">起始索引</param>
        /// <param name="p_count">行数</param>
        void RemoveRange(int p_index, int p_count);

        /// <summary>
        /// 获取延迟触发CollectionChanged事件的控制对象，控制对象Dispose后触发 NotifyCollectionChangedAction.Reset
        /// </summary>
        /// <returns></returns>
        IDisposable Defer();
    }
}
