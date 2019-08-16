#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-01-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 普通集合视图
    /// </summary>
    internal class DataListView : ILvDataView
    {
        #region 成员变量
        Lv _owner;
        IList _data;
        #endregion

        #region 构造方法
        public DataListView(Lv p_owner, IList p_data)
        {
            _owner = p_owner;
            _data = p_data;
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
                // 自带分组的数据源
                _owner.LoadGroupRows(rows);
            }
            else if (!string.IsNullOrEmpty(_owner.GroupName))
            {
                // 按指定属性分组
                var groupRows = BuildGroups(rows);
                _owner.LoadGroupRows(groupRows);
            }
            else
            {
                _owner.LoadRows(rows);
            }
        }

        /// <summary>
        /// 在指定的索引处批量插入数据，未参加过滤排序和分组！
        /// </summary>
        /// <param name="p_rows">外部数据行</param>
        /// <param name="p_index">插入位置，-1表示添加到最后</param>
        /// <returns>插入位置</returns>
        public int Insert(IEnumerable p_rows, int p_index)
        {
            if (p_rows == null)
                return -1;

            int start = p_index;
            if (p_index < 0 || p_index > _owner.Rows.Count)
                start = _owner.Rows.Count;

            int i = 0;
            foreach (var row in p_rows)
            {
                _data.Insert(i + start, row);
                i++;
            }
            _owner.BatchInsertRows(_data, start, i);
            return start;
        }

        /// <summary>
        /// 批量删除给定的行，统一更新
        /// </summary>
        /// <param name="p_rows"></param>
        public void Delete(IEnumerable p_rows)
        {
            if (p_rows == null)
                return;

            foreach (var row in p_rows)
            {
                _data.Remove(row);
            }
            Refresh();
        }

        /// <summary>
        /// 卸载数据
        /// </summary>
        public void Unload()
        {
        }
        #endregion

        #region 内部方法
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
                var pi = list[0].GetType().GetProperty(_owner.SortDesc.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                    list.Sort(new RowComparer(_owner.SortDesc, pi));
                return list;
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
            GroupData<object> group;
            PropertyInfo pi = null;
            foreach (object row in p_rows)
            {
                if (pi == null)
                {
                    pi = row.GetType().GetProperty(_owner.GroupName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi == null)
                        break;
                }

                object obj = pi.GetValue(row);
                string name = (obj == null) ? "" : obj.ToString();
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