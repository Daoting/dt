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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 具有集合变化通知的泛型列表，NotifyList缩写
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Nl<T> : ObservableCollection<T>, INotifyList
    {
        #region 构造方法
        /// <summary>
        /// 具有集合变化通知的泛型列表
        /// </summary>
        public Nl()
        { }

        /// <summary>
        /// 初始化时复制列表
        /// </summary>
        /// <param name="list"></param>
        public Nl(List<T> list)
            : base(list)
        { }

        /// <summary>
        /// 初始化时复制列表
        /// </summary>
        /// <param name="collection"></param>
        public Nl(IEnumerable<T> collection)
            : base(collection)
        { }
        #endregion

        #region INotifyList
        /// <summary>
        /// 在末尾批量添加数据，统一触发 NotifyCollectionChangedAction.Add
        /// </summary>
        /// <param name="p_items"></param>
        public void AddRange(IList p_items)
        {
            InsertRange(Count, p_items);
        }

        /// <summary>
        /// 在指定的索引处批量插入数据，统一触发 NotifyCollectionChangedAction.Add
        /// </summary>
        /// <param name="p_index">插入位置，范围在0到当前总数之间</param>
        /// <param name="p_items">数据行</param>
        public void InsertRange(int p_index, IList p_items)
        {
            if (p_items == null || p_items.Count == 0)
                return;

            if (p_index < 0 || p_index > Count)
                throw new Exception("数据待插入的索引超出范围！");

            using (Defer())
            {
                _collectionChangedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, p_items, p_index);
                for (int i = 0; i < p_items.Count; i++)
                {
                    if (p_items[i] is T item)
                        base.Insert(i + p_index, item);
                    else
                        throw new Exception("插入数据类型应为" + typeof(T).FullName);
                }
            }
        }

        /// <summary>
        /// 批量删除给定的行，统一触发 NotifyCollectionChangedAction.Remove
        /// </summary>
        /// <param name="p_items">数据行</param>
        /// <returns>实际删除的行数</returns>
        public int RemoveRange(IList p_items)
        {
            if (p_items == null || p_items.Count == 0)
                return 0;

            // 支持外部嵌套Defer()
            int old = _updating;
            _updating = 1;
            int cnt = 0;
            foreach (var row in p_items.OfType<T>())
            {
                if (base.Remove(row))
                    cnt++;
            }
            _updating = old;

            if (cnt > 0)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return cnt;
        }

        /// <summary>
        /// 批量删除指定范围的行，统一触发 NotifyCollectionChangedAction.Remove
        /// </summary>
        /// <param name="p_index">起始索引</param>
        /// <param name="p_count">行数</param>
        public void RemoveRange(int p_index, int p_count)
        {
            if (p_index < 0
                || p_index >= Count
                || p_count <= 0
                || p_index + p_count >= Count)
                throw new Exception("删除范围的索引超出范围！");

            using (Defer())
            {
                for (int i = 0; i < p_count; i++)
                {
                    base.RemoveAt(p_index);
                }
            }
        }

        /// <summary>
        /// 延迟触发CollectionChanged事件
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// using (tbl.Defer())
        /// {
        ///     foreach (var row in data)
        ///     {
        ///         tbl.Add(row);
        ///     }
        /// }
        /// </code>
        /// </example>
        public IDisposable Defer()
        {
            _collectionChangedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            return new InternalCls(this);
        }

        int _updating;
        NotifyCollectionChangedEventArgs _collectionChangedArgs;

        /// <summary>
        /// 通过Defer实现延时更新
        /// </summary>
        int Updating
        {
            get { return _updating; }
            set
            {
                _updating = value;
                if (_updating == 0)
                    OnCollectionChanged(_collectionChangedArgs);
            }
        }

        /// <summary>
        /// 重新触发CollectionChanged的方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_updating <= 0)
            {
                // 符合更新条件，触发基类事件，否则延迟更新
                base.OnCollectionChanged(e);
            }
        }

        class InternalCls : IDisposable
        {
            Nl<T> _owner;

            public InternalCls(Nl<T> p_owner)
            {
                _owner = p_owner;
                _owner.Updating = _owner.Updating + 1;
            }

            public void Dispose()
            {
                _owner.Updating = _owner.Updating - 1;
            }
        }
        #endregion
    }

    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class NlEx
    {
        /// <summary>
        /// 转换成Nl对象
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Nl<TSource> ToNl<TSource>(this IEnumerable<TSource> source)
        {
            return new Nl<TSource>(source);
        }
    }
}
