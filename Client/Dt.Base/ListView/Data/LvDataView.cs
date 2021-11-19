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
            Refresh();
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 刷新视图列表
        /// </summary>
        public void Refresh()
        {
            IList rows;
            if ((_owner.SortDesc != null && !string.IsNullOrEmpty(_owner.SortDesc.ID))
                || _owner.Filter != null)
                rows = GetTransformedList();
            else
                rows = _data;

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
                && _owner.SortDesc == null
                && string.IsNullOrEmpty(_owner.GroupName)
                && _owner.Filter == null)
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
        List<object> GetTransformedList()
        {
            List<object> list = new List<object>();
            // 过滤
            if (_owner.Filter != null)
            {
                foreach (var row in _data)
                {
                    if (_owner.Filter(row))
                        list.Add(row);
                }
            }
            else
            {
                list.AddRange(_data.Cast<object>());
            }

            // 排序
            if (list.Count > 0
                && _owner.SortDesc != null
                && !string.IsNullOrEmpty(_owner.SortDesc.ID))
            {
                // 使用RowComparer的Compare方法排序
                if (_data is Table)
                {
                    list.Sort(new RowComparer(_owner.SortDesc, null));
                }
                else
                {
                    var pi = list[0].GetType().GetProperty(_owner.SortDesc.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                        list.Sort(new RowComparer(_owner.SortDesc, pi));
                }
            }
            return list;
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
