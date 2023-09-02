#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections;
using System.Collections.Specialized;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 选择相关
    /// </summary>
    public partial class Lv
    {
        #region 静态内容
        public readonly static DependencyProperty SelectionModeProperty = DependencyProperty.Register(
            "SelectionMode",
            typeof(SelectionMode),
            typeof(Lv),
            new PropertyMetadata(SelectionMode.Single, OnSelectionModeChanged));

        public static readonly DependencyProperty HasSelectedProperty = DependencyProperty.Register(
            "HasSelected",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(false));

        static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Lv lv = (Lv)d;
            if (lv._panel != null)
            {
                foreach (var row in lv.Rows)
                {
                    row.ClearValue(LvItem.IsSelectedProperty);
                    row.ValueChanged = null;
                }

                if (lv._selectedLvItems.Count > 0)
                {
                    try
                    {
                        lv._selectedLvItems.CollectionChanged -= lv.OnSelectedItemsChanged;
                        lv._selectedLvItems.Clear();
                        lv.HasSelected = false;
                    }
                    finally
                    {
                        lv._selectedLvItems.CollectionChanged += lv.OnSelectedItemsChanged;
                    }
                }
                lv.ReloadPanelContent();
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置选择模式，默认Single
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// 获取当前选择的行列表
        /// </summary>
        public IEnumerable<object> SelectedItems
        {
            get
            {
                return from row in _selectedLvItems
                       select row.Data;
            }
        }

        /// <summary>
        /// 获取设置当前选定行，设置null时清空选择，多选时为返回最后选择行
        /// </summary>
        public object SelectedItem
        {
            get
            {
                var row = _selectedLvItems.LastOrDefault();
                if (row != null)
                    return row.Data;
                return null;
            }
            set
            {
                // 清空选择
                if (value == null)
                {
                    if (_selectedLvItems.Count > 0)
                        _selectedLvItems.Clear();
                    return;
                }

                // 行是否存在
                var selectedRow = (from row in _rows
                                   where row.Data == value
                                   select row).FirstOrDefault();
                if (selectedRow == null)
                    return;

                // 挑出取消选择的行
                bool exist = false;
                List<object> removes = new List<object>();
                foreach (var row in _selectedLvItems)
                {
                    if (row != selectedRow)
                    {
                        row.IsSelected = false;
                        removes.Add(row.Data);
                    }
                    else
                    {
                        exist = true;
                    }
                }
                // 无变化
                if (removes.Count == 0 && exist)
                    return;

                try
                {
                    _selectedLvItems.CollectionChanged -= OnSelectedItemsChanged;
                    if (_selectedLvItems.Count > 0)
                        _selectedLvItems.Clear();
                    _selectedLvItems.Add(selectedRow);
                    selectedRow.IsSelected = true;
                    HasSelected = true;
                }
                finally
                {
                    _selectedLvItems.CollectionChanged += OnSelectedItemsChanged;
                }

                if (SelectionChanged != null)
                {
                    List<object> adds = new List<object>();
                    if (!exist)
                        adds.Add(value);
                    SelectionChanged(this, new SelectionChangedEventArgs(removes, adds));
                }
            }
        }

        /// <summary>
        /// 获取设置选定行的索引，-1无选定行，设置-1清空选择
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                var row = _selectedLvItems.LastOrDefault();
                if (row != null)
                    return _rows.IndexOf(row);
                return -1;
            }
            set
            {
                // 清空选择
                if (value == -1)
                {
                    if (_selectedLvItems.Count > 0)
                        _selectedLvItems.Clear();
                    return;
                }

                // 超出范围
                if (value < 0 || value >= _rows.Count)
                    return;

                // 挑出取消选择的行
                bool exist = false;
                var selectedRow = _rows[value];
                List<object> removes = new List<object>();
                foreach (var row in _selectedLvItems)
                {
                    if (row != selectedRow)
                    {
                        row.IsSelected = false;
                        removes.Add(row.Data);
                    }
                    else
                    {
                        exist = true;
                    }
                }
                // 无变化
                if (removes.Count == 0 && exist)
                    return;

                try
                {
                    _selectedLvItems.CollectionChanged -= OnSelectedItemsChanged;
                    if (_selectedLvItems.Count > 0)
                        _selectedLvItems.Clear();
                    _selectedLvItems.Add(selectedRow);
                    selectedRow.IsSelected = true;
                    HasSelected = true;
                }
                finally
                {
                    _selectedLvItems.CollectionChanged += OnSelectedItemsChanged;
                }

                if (SelectionChanged != null)
                {
                    List<object> adds = new List<object>();
                    if (!exist)
                        adds.Add(selectedRow.Data);
                    SelectionChanged(this, new SelectionChangedEventArgs(removes, adds));
                }
            }
        }

        /// <summary>
        /// 获取选择的行数
        /// </summary>
        public int SelectedCount
        {
            get { return _selectedLvItems.Count; }
        }

        /// <summary>
        /// 获取当前选择的Row列表
        /// </summary>
        public IEnumerable<Row> SelectedRows
        {
            get
            {
                return from row in _selectedLvItems
                       select (Row)row.Data;
            }
        }

        /// <summary>
        /// 获取当前选定的Row
        /// </summary>
        public Row SelectedRow
        {
            get { return SelectedItem as Row; }
        }

        /// <summary>
        /// 获取是否有选择行
        /// </summary>
        public bool HasSelected
        {
            get { return (bool)GetValue(HasSelectedProperty); }
            set { SetValue(HasSelectedProperty, value); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (SelectionMode != SelectionMode.Multiple)
                return;

            List<object> adds = new List<object>();
            try
            {
                _selectedLvItems.CollectionChanged -= OnSelectedItemsChanged;
                foreach (var row in _rows)
                {
                    if (!row.IsSelected)
                    {
                        adds.Add(row.Data);
                        _selectedLvItems.Add(row);
                        row.IsSelected = true;
                    }
                }
            }
            finally
            {
                _selectedLvItems.CollectionChanged += OnSelectedItemsChanged;
            }

            if (adds.Count > 0)
            {
                HasSelected = true;
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(new List<object>(), adds));
            }
        }

        /// <summary>
        /// 清除所有选择行的选择状态
        /// </summary>
        public void ClearSelection()
        {
            if (_selectedLvItems.Count > 0)
                _selectedLvItems.Clear();
        }

        /// <summary>
        /// 增加选择行
        /// </summary>
        /// <param name="p_ls"></param>
        public void Select(IList p_ls)
        {
            if (p_ls == null || p_ls.Count == 0)
                return;

            List<object> adds = new List<object>();
            try
            {
                _selectedLvItems.CollectionChanged -= OnSelectedItemsChanged;
                foreach (var row in _rows)
                {
                    if (!row.IsSelected && p_ls.Contains(row.Data))
                    {
                        adds.Add(row.Data);
                        _selectedLvItems.Add(row);
                        row.IsSelected = true;
                    }
                }
            }
            finally
            {
                _selectedLvItems.CollectionChanged += OnSelectedItemsChanged;
            }

            if (adds.Count > 0 && SelectionChanged != null)
                SelectionChanged(this, new SelectionChangedEventArgs(new List<object>(), adds));
        }

        /// <summary>
        /// 取消列表中行的选择状态
        /// </summary>
        /// <param name="p_ls"></param>
        public void RemoveSelection(IList p_ls)
        {
            if (p_ls == null || p_ls.Count == 0)
                return;

            List<object> removes = new List<object>();
            try
            {
                _selectedLvItems.CollectionChanged -= OnSelectedItemsChanged;
                foreach (var data in p_ls)
                {
                    var item = (from row in _selectedLvItems
                                where row.Data == data
                                select row).FirstOrDefault();
                    if (item != null)
                    {
                        removes.Add(data);
                        _selectedLvItems.Remove(item);
                        item.IsSelected = false;
                    }
                }
            }
            finally
            {
                _selectedLvItems.CollectionChanged += OnSelectedItemsChanged;
            }

            if (removes.Count > 0 && SelectionChanged != null)
                SelectionChanged(this, new SelectionChangedEventArgs(removes, new List<object>()));
        }

        /// <summary>
        /// 删除所有选择行
        /// </summary>
        /// <returns></returns>
        public void DeleteSelection()
        {
            var data = Data;
            if (data != null && _selectedLvItems.Count > 0)
            {
                data.RemoveRange((from row in _selectedLvItems
                                  select row.Data).ToList());
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 选择行集合变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // HasSelected状态
            bool hasSelected = HasSelected;
            if (_selectedLvItems.Count == 0 && hasSelected)
                ClearValue(HasSelectedProperty);
            else if (_selectedLvItems.Count > 0 && !hasSelected)
                HasSelected = true;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // 增加
                if (e.NewItems != null && e.NewItems.Count > 0)
                {
                    List<object> adds = new List<object>();
                    foreach (var row in e.NewItems.OfType<LvItem>())
                    {
                        row.IsSelected = true;
                        adds.Add(row.Data);
                    }
                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(new List<object>(), adds));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // 删除
                if (e.OldItems != null && e.OldItems.Count > 0)
                {
                    List<object> removals = new List<object>();
                    foreach (var row in e.OldItems.OfType<LvItem>())
                    {
                        row.IsSelected = false;
                        removals.Add(row.Data);
                    }
                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(removals, new List<object>()));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // 清空
                var removals = new List<object>();
                foreach (var row in _rows)
                {
                    if (row.IsSelected)
                    {
                        removals.Add(row.Data);
                        row.IsSelected = false;
                    }
                }
                if (removals.Count > 0 && SelectionChanged != null)
                    SelectionChanged(this, new SelectionChangedEventArgs(removals, new List<object>()));
            }
        }

        /// <summary>
        /// 单选模式点击时切换选择
        /// </summary>
        /// <param name="p_vr"></param>
        internal void OnToggleSelected(LvItem p_vr)
        {
            List<object> removes = new List<object>();
            try
            {
                _selectedLvItems.CollectionChanged -= OnSelectedItemsChanged;
                if (_selectedLvItems.Count > 0)
                {
                    var row = _selectedLvItems[0];
                    row.IsSelected = false;
                    removes.Add(row.Data);
                    _selectedLvItems.Clear();
                }

                _selectedLvItems.Add(p_vr);
                p_vr.IsSelected = true;
                HasSelected = true;
            }
            finally
            {
                _selectedLvItems.CollectionChanged += OnSelectedItemsChanged;
            }
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(removes, new List<object> { p_vr.Data }));
        }
        #endregion
    }
}