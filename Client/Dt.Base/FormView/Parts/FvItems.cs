#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-02-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Fv单元格集合，列表 + 字典 + 集合更改事件
    /// </summary>
    public class FvItems : IList<FrameworkElement>, IEnumerable<FrameworkElement>
    {
        // 所有元素
        readonly IList<FrameworkElement> _list = new List<FrameworkElement>();
        // 所有含ID的格元素
        readonly Dictionary<string, FvCell> _cells = new Dictionary<string, FvCell>(StringComparer.OrdinalIgnoreCase);
        int _updating;

        /// <summary>
        /// 集合更改事件
        /// </summary>
        public event EventHandler<ItemListChangedArgs> ItemsChanged;

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
        /// 返回指定索引的单元格
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        public FrameworkElement this[int p_index]
        {
            get { return _list[p_index]; }
            set
            {
                var originalValue = _list[p_index];
                if (!ReferenceEquals(originalValue, value))
                {
                    _list[p_index] = value;

                    if (originalValue is FvCell cell && !string.IsNullOrEmpty(cell.ID))
                        _cells.Remove(cell.ID);
                    if (value is FvCell fc && !string.IsNullOrEmpty(fc.ID))
                        _cells[fc.ID] = fc;

                    RaiseVectorChanged(CollectionChange.ItemChanged, p_index);
                }
            }
        }

        /// <summary>
        /// 获取具有指定id的格
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public FvCell this[string p_id]
        {
            get
            {
                FvCell cell;
                if (_cells.TryGetValue(p_id, out cell))
                    return cell;
                return null;
            }
        }

        /// <summary>
        /// 获取所有FvCell字典
        /// </summary>
        public IReadOnlyDictionary<string, FvCell> Cells
        {
            get { return _cells; }
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
        /// <param name="p_item"></param>
        public void Add(FrameworkElement p_item)
        {
            _list.Add(p_item);
            if (p_item is FvCell cell && !string.IsNullOrEmpty(cell.ID))
                _cells[cell.ID] = cell;
            RaiseVectorChanged(CollectionChange.ItemInserted, _list.Count - 1);
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            _cells.Clear();
            RaiseVectorChanged(CollectionChange.Reset, 0);
        }

        /// <summary>
        /// 是否包含指定项
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        public bool Contains(FrameworkElement p_item) => _list.Contains(p_item);

        /// <summary>
        /// 将集合复制到新列表
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(FrameworkElement[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<FrameworkElement> GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        /// 获取项的索引
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        public int IndexOf(FrameworkElement p_item) => _list.IndexOf(p_item);

        /// <summary>
        /// 在指定位置插入新项
        /// </summary>
        /// <param name="index"></param>
        /// <param name="p_item"></param>
        public void Insert(int index, FrameworkElement p_item)
        {
            _list.Insert(index, p_item);
            if (p_item is FvCell cell && !string.IsNullOrEmpty(cell.ID))
                _cells[cell.ID] = cell;
            RaiseVectorChanged(CollectionChange.ItemInserted, index);
        }

        /// <summary>
        /// 移除指定项
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        public bool Remove(FrameworkElement p_item)
        {
            var index = _list.IndexOf(p_item);
            if (index != -1)
            {
                RemoveAt(index);
                if (p_item is FvCell cell && !string.IsNullOrEmpty(cell.ID))
                    _cells.Remove(cell.ID);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除指定索引处的项
        /// </summary>
        /// <param name="p_index"></param>
        public void RemoveAt(int p_index)
        {
            if (p_index >= 0 && p_index < _list.Count)
            {
                var item = _list[p_index];
                _list.RemoveAt(p_index);
                if (item is FvCell cell && !string.IsNullOrEmpty(cell.ID))
                    _cells.Remove(cell.ID);
                RaiseVectorChanged(CollectionChange.ItemRemoved, p_index);
            }
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
                ItemsChanged(this, new ItemListChangedArgs(change, index));
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
            FvItems _rows;

            public Deferral(FvItems parent)
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
}
