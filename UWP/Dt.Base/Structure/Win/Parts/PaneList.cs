#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-03 创建
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

namespace Dt.Base.Docking
{
    /// <summary>
    /// Win中使用的子项集合，含集合更改事件
    /// </summary>
    public class PaneList : IList<FrameworkElement>, IEnumerable<FrameworkElement>
    {
        // 所有元素
        readonly IList<FrameworkElement> _list = new List<FrameworkElement>();

        /// <summary>
        /// 集合更改事件
        /// </summary>
        public event EventHandler<ItemListChangedArgs> ItemsChanged;

        /// <summary>
        /// 返回指定索引的子项
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        public FrameworkElement this[int p_index]
        {
            get { return _list[p_index]; }
            set
            {
                if (!ReferenceEquals(_list[p_index], value))
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
        /// <param name="p_item"></param>
        public void Add(FrameworkElement p_item)
        {
            _list.Add(p_item);
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
            // 符合更新条件，触发基类事件，否则延迟更新
            ItemsChanged?.Invoke(this, new ItemListChangedArgs(change, index));
        }
    }
}
