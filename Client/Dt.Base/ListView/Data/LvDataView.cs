#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 数据视图
    /// </summary>
    internal class LvDataView
    {
        #region 成员变量
        Lv _owner;
        INotifyList _data;
        #endregion

        #region 构造方法
        public LvDataView(Lv p_owner, INotifyList p_data)
        {
            _owner = p_owner;
            _data = p_data;
            _data.CollectionChanged += OnCollectionChanged;
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 刷新视图列表
        /// </summary>
        public void Refresh()
        {
            // 若延时刷新，不处理
            if (_owner.Updating > 0)
            {
                _owner.DeferRefreshData = true;
                return;
            }

            IList rows;
            if (_data.Count > 0
                && (!string.IsNullOrEmpty(_owner.Where)
                    || (_owner.SortDesc != null && !string.IsNullOrEmpty(_owner.SortDesc.ID))
                    || _owner.Filter != null
                    || _owner.ExistDefaultFilterCfg))
            {
                rows = GetTransformedList();
            }
            else
            {
                rows = _data;
            }

            if (rows.Count == 0)
            {
                _owner.ClearAllRows();
                return;
            }

            if (rows[0] is IList)
            {
                // 自带分组的数据源，不需要构造分组，如：Nl<GroupData<OmMenu>>
                _owner.LoadGroupRows(rows);
            }
            else if (!string.IsNullOrEmpty(_owner.GroupName))
            {
                // 按指定列或属性分组
                var groupRows = BuildGroups(rows);
                _owner.LoadGroupRows(groupRows);
            }
            else
            {
                _owner.LoadRows(rows);
            }
        }

        /// <summary>
        /// 卸载数据
        /// </summary>
        public void Unload()
        {
            _data.CollectionChanged -= OnCollectionChanged;
        }
        #endregion

        #region 集合变化
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            // 新增或删除 且 无排序过滤分组时直接操作，高效
            if ((args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Remove)
                && string.IsNullOrEmpty(_owner.Where)
                && _owner.SortDesc == null
                && string.IsNullOrEmpty(_owner.GroupName)
                && _owner.Filter == null
                && !_owner.ExistDefaultFilterCfg)
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    _owner.BatchInsertRows(_data, args.NewStartingIndex, args.NewItems.Count);
                }
                else if (args.OldStartingIndex < 0)
                {
                    // 批量删除时，参数为从小到大排序的索引列表
                    _owner.BatchRemoveRows(args.OldItems);
                }
                else
                {
                    // 删除一个时
                    _owner.BatchRemoveRows(new List<int> { args.OldStartingIndex });
                }
            }
            else
            {
                Refresh();
            }
        }
        #endregion

        #region 分组过滤排序
        /// <summary>
        /// 获得过滤排序后的列表
        /// </summary>
        /// <returns></returns>
        IList GetTransformedList()
        {
            var query = _data.AsQueryable();

            // 1. 先执行where linq 过滤
            if (!string.IsNullOrEmpty(_owner.Where))
            {
                // 只有转成实际类型，才能调用属性和方法
                query = query.Cast(_data[0].GetType()).Where(_owner.Where);
            }

            var ls = query.Cast<object>();
            // 2. 再执行过滤回调
            if (_owner.Filter != null)
            {
                ls = from item in ls
                     where _owner.Filter(item)
                     select item;
            }

            // 3. 最后筛选框过滤
            if (_owner.ExistDefaultFilterCfg)
            {
                ls = from item in ls
                     where _owner.FilterCfg.DoDefaultFilter(item)
                     select item;
            }

            // 排序
            if (_owner.SortDesc != null
                && !string.IsNullOrEmpty(_owner.SortDesc.ID))
            {
                if (_data is Table tbl)
                {
                    ls = ls.Order(new RowComparer(_owner.SortDesc, null));
                }
                else
                {
                    var pi = _data[0].GetType().GetProperty(_owner.SortDesc.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    ls = ls.Order(new RowComparer(_owner.SortDesc, pi));
                }
            }

            return ls.ToList();
        }

        /// <summary>
        /// 构造分组
        /// </summary>
        /// <param name="p_rows"></param>
        /// <returns></returns>
        GroupDataList BuildGroups(IList p_rows)
        {
            var groupRows = new GroupDataList();
            PropertyInfo pi = null;
            if (!(_data is Table))
            {
                // 分组属性
                pi = p_rows[0].GetType().GetProperty(_owner.GroupName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi == null)
                    return groupRows;
            }

            GroupData<object> group;
            string name;
            foreach (object row in p_rows)
            {
                if (pi == null)
                {
                    // Row
                    name = ((Row)row).Str(_owner.GroupName);
                }
                else
                {
                    // 普通对象
                    object obj = pi.GetValue(row);
                    name = (obj == null) ? "" : obj.ToString();
                }

                if (groupRows.Contains(name))
                {
                    group = groupRows[name];
                }
                else
                {
                    group = new GroupData<object>();
                    group.Title = name;
                    groupRows.Add(group);
                }
                group.Add(row);
            }
            return groupRows;
        }
        #endregion

        #region RowComparer
        class RowComparer : IComparer<object>
        {
            readonly IComparer _comparer = Comparer<object>.Default;
            SortDescription _desc;
            PropertyInfo _pi;

            internal RowComparer(SortDescription p_desc, PropertyInfo p_pi)
            {
                _desc = p_desc;
                _pi = p_pi;
            }

            /// <summary>
            /// IComparer方法
            /// </summary>
            /// <param name="p_rowX"></param>
            /// <param name="p_rowY"></param>
            /// <returns></returns>
            public int Compare(object p_rowX, object p_rowY)
            {
                int num = 0;
                if (_pi == null)
                    num = _comparer.Compare(((Row)p_rowX)[_desc.ID], ((Row)p_rowY)[_desc.ID]);
                else
                    num = _comparer.Compare(_pi.GetValue(p_rowX), _pi.GetValue(p_rowY));
                if (num != 0)
                {
                    if (_desc.Direction != ListSortDirection.Ascending)
                    {
                        return -num;
                    }
                    return num;
                }
                return 0;
            }
        }
        #endregion
    }
}
