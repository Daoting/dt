#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation.Collections;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 泛型集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemList<T> : IList<T>, IEnumerable<T>
    {
        readonly IList<T> _list = new List<T>();
        int _updating;

        /// <summary>
        /// 集合更改事件
        /// </summary>
        public event ItemListChangedHandler ItemsChanged;

        /// <summary>
        /// 延迟触发ItemsChanged事件
        /// using (_items.Defer())
        /// {
        ///     _items.Clear();
        ///     foreach (var col in p_cols)
        ///     {
        ///         _items.Add(col);
        ///     }
        /// }
        /// </summary>
        /// <returns></returns>
        public IDisposable Defer()
        {
            return new Deferral(this);
        }

        /// <summary>
        /// 返回指定索引的项
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        public T this[int p_index]
        {
            get { return _list[p_index]; }
            set
            {
                var originalValue = _list[p_index];
                if (!ReferenceEquals(originalValue, value))
                {
                    _list[p_index] = value;
                    RaiseVectorChanged(CollectionChange.ItemChanged, p_index);
                }
            }
        }

        /// <summary>
        /// 集合元素总数
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// 集合是否为只读
        /// </summary>
        public bool IsReadOnly => _list.IsReadOnly;

        /// <summary>
        /// 添加新项
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            _list.Add(item);
            RaiseVectorChanged(CollectionChange.ItemInserted, _list.Count - 1);
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            RaiseVectorChanged(CollectionChange.Reset, 0);
        }

        /// <summary>
        /// 是否包含指定项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item) => _list.Contains(item);

        /// <summary>
        /// 将集合复制到新列表
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        /// 获取项的索引
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item) => _list.IndexOf(item);

        /// <summary>
        /// 在指定位置插入新项
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            RaiseVectorChanged(CollectionChange.ItemInserted, index);
        }

        /// <summary>
        /// 移除指定项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            var index = _list.IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除指定索引处的项
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            RaiseVectorChanged(CollectionChange.ItemRemoved, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        /// 触发集合更改事件
        /// </summary>
        /// <param name="change"></param>
        /// <param name="index"></param>
        void RaiseVectorChanged(CollectionChange change, int index)
        {
            if (_updating <= 0 && ItemsChanged != null)
            {
                // 符合更新条件，触发基类事件，否则延迟更新
                ItemsChanged.Invoke(this, new ItemListChangedArgs(change, index));
            }
        }

        #region 延时更新
        /// <summary>
        /// 通过Defer()实现延时更新
        /// </summary>
        int Updating
        {
            get { return _updating; }
            set
            {
                _updating = value;
                if (_updating == 0)
                    RaiseVectorChanged(CollectionChange.Reset, 0);
            }
        }

        class Deferral : IDisposable
        {
            ItemList<T> _rows;

            public Deferral(ItemList<T> parent)
            {
                _rows = parent;
                _rows.Updating = _rows.Updating + 1;
            }

            public void Dispose()
            {
                _rows.Updating = _rows.Updating - 1;
            }
        }
        #endregion
    }

    /// <summary>
    /// 集合更改事件原型
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ItemListChangedHandler(object sender, ItemListChangedArgs e);

    /// <summary>
    /// 集合改变参数
    /// </summary>
    public class ItemListChangedArgs
    {
        public ItemListChangedArgs(CollectionChange p_change, int p_index)
        {
            CollectionChange = p_change;
            Index = p_index;
        }

        /// <summary>
        /// 集合更改类型
        /// </summary>
        public CollectionChange CollectionChange { get; }

        /// <summary>
        /// 集合更改发生的位置
        /// </summary>
        public int Index { get; }
    }
}