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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// Table数据视图
    /// </summary>
    internal class DataTableView : ILvDataView
    {
        #region 成员变量
        Lv _owner;
        Table _data;
        #endregion

        #region 构造方法
        public DataTableView(Lv p_owner, Table p_data)
        {
            _owner = p_owner;
            _data = p_data;
            _data.CollectionChanged += OnDataCollectionChanged;
            Refresh();
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 刷新视图列表
        /// </summary>
        public void Refresh()
        {
            IList<Row> rows;
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

            if (!string.IsNullOrEmpty(_owner.GroupName))
            {
                // 按列分组
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

            // 不触发OnDataCollectionChanged中的Add
            _data.CollectionChanged -= OnDataCollectionChanged;
            int i = 0;
            foreach (var row in p_rows.OfType<Row>())
            {
                _data.Insert(i + start, row);
                i++;
            }
            _owner.BatchInsertRows(_data, start, i);
            _data.CollectionChanged += OnDataCollectionChanged;
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

            int cnt = 0;
            _data.CollectionChanged -= OnDataCollectionChanged;
            var all = (ObservableCollection<Row>)_data;
            foreach (var row in p_rows.OfType<Row>())
            {
                if (all.Remove(row))
                    cnt++;
            }
            _data.CollectionChanged += OnDataCollectionChanged;

            if (cnt > 0)
                Refresh();
        }

        /// <summary>
        /// 卸载数据
        /// </summary>
        public void Unload()
        {
            if (_data != null)
                _data.CollectionChanged -= OnDataCollectionChanged;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获得过滤排序后的列表
        /// </summary>
        /// <returns></returns>
        List<Row> GetTransformedList()
        {
            List<Row> list = new List<Row>();
            // 过滤
            if (_owner.Filter != null)
            {
                foreach (Row row in _data)
                {
                    if (_owner.Filter(row))
                        list.Add(row);
                }
            }
            else
            {
                list.AddRange(_data);
            }

            // 排序
            if (list.Count > 0
                && _owner.SortDesc != null
                && !string.IsNullOrEmpty(_owner.SortDesc.ID))
            {
                // 使用RowComparer的Compare方法排序
                list.Sort(new RowComparer(_owner.SortDesc));
                return list;
            }
            return list;
        }

        /// <summary>
        /// 构造分组
        /// </summary>
        /// <param name="p_rows"></param>
        /// <returns></returns>
        GroupDataList BuildGroups(IList<Row> p_rows)
        {
            GroupDataList groupRows = new GroupDataList();
            GroupData<object> group;
            foreach (Row row in p_rows)
            {
                string name = row.Str(_owner.GroupName);
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

        void OnDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Refresh();
        }
        #endregion

        #region RowComparer
        /// <summary>
        /// 行比较器
        /// </summary>
        class RowComparer : IComparer<Row>
        {
            readonly IComparer _comparer = Comparer<object>.Default;
            SortDescription _desc;

            internal RowComparer(SortDescription p_desc)
            {
                _desc = p_desc;
            }

            /// <summary>
            /// IComparer方法
            /// </summary>
            /// <param name="p_rowX"></param>
            /// <param name="p_rowY"></param>
            /// <returns></returns>
            public int Compare(Row p_rowX, Row p_rowY)
            {
                int num = 0;
                num = _comparer.Compare(p_rowX[_desc.ID], p_rowY[_desc.ID]);
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