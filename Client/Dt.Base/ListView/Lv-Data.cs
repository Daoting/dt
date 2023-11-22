#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 数据源相关
    /// </summary>
    public partial class Lv
    {
        #region 静态内容
        public readonly static DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(INotifyList),
            typeof(Lv),
            new PropertyMetadata(null, OnDataChanged));

        public readonly static DependencyProperty PageDataProperty = DependencyProperty.Register(
            "PageData",
            typeof(PageData),
            typeof(Lv),
            new PropertyMetadata(null, OnPageDataChanged));

        public readonly static DependencyProperty FilterProperty = DependencyProperty.Register(
            "Filter",
            typeof(Predicate<object>),
            typeof(Lv),
            new PropertyMetadata(null, OnDataViewPropertyChanged));

        public readonly static DependencyProperty WhereProperty = DependencyProperty.Register(
            "Where",
            typeof(string),
            typeof(Lv),
            new PropertyMetadata(null, OnDataViewPropertyChanged));

        public static readonly DependencyProperty FilterCfgProperty = DependencyProperty.Register(
            "FilterCfg",
            typeof(FilterCfg),
            typeof(Lv),
            new PropertyMetadata(null, OnFilterCfgChanged));

        public readonly static DependencyProperty SortDescProperty = DependencyProperty.Register(
            "SortDesc",
            typeof(SortDescription),
            typeof(Lv),
            new PropertyMetadata(null, OnSortDescChanged));

        static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Lv)d).LoadDataSource();
        }

        static void OnPageDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Lv lv = (Lv)d;
            if (lv.Scroll != null)
                lv.Scroll.ViewChanged -= lv.OnScrollViewChanged;

            var pd = (PageData)e.NewValue;
            if (pd != null)
            {
                pd.SetOwner(lv);
                pd.GotoFirstPage();

                if (lv.Scroll != null)
                    lv.Scroll.ViewChanged += lv.OnScrollViewChanged;
            }
        }

        static void OnDataViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Lv lv = (Lv)d;
            if (lv._dataView != null)
                lv._dataView.Refresh();
        }

        static void OnSortDescChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Lv lv = (Lv)d;
            if (lv._panel != null)
                lv._panel.OnSortDescChanged();
            if (lv._dataView != null)
                lv._dataView.Refresh();
        }

        static void OnFilterCfgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Lv lv = (Lv)d;
            if (lv._dataView != null)
                lv._dataView.Refresh();
            lv.ReloadPanelContent();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置数据源对象，需实现INotifyList接口，Table 和 Nl 为常用类型
        /// </summary>
        public INotifyList Data
        {
            get { return (INotifyList)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// 获取设置分页数据源对象
        /// </summary>
        public PageData PageData
        {
            get { return (PageData)GetValue(PageDataProperty); }
            set { SetValue(PageDataProperty, value); }
        }

        /// <summary>
        /// 获取设置过滤回调
        /// </summary>
        public Predicate<object> Filter
        {
            get { return (Predicate<object>)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        /// <summary>
        /// 获取设置linq过滤串，过滤串规则：
        /// <para>1. it是当前数据对象，调用对象的属性和方法时可以省略，如：it.Name 等同 Name</para>
        /// <para>2. 可以访问it的任意公共属性，但访问方法时需要在类型上添加DynamicLinqType标签</para>
        /// <para>3. it的属性为简单类型时，可调用其属性和方法，如：Name.StartsWith() Name.Contains()</para>
        /// <para>4. it的属性为自定义类型时，访问其属性和方法也需要在其类型上添加DynamicLinqType标签</para>
        /// <para>5. it为Row时写法复杂些，推荐使用Enity</para>
        /// </summary>
        public string Where
        {
            get { return (string)GetValue(WhereProperty); }
            set { SetValue(WhereProperty, value); }
        }

        /// <summary>
        /// 获取设置筛选框配置，默认null
        /// </summary>
        public FilterCfg FilterCfg
        {
            get { return (FilterCfg)GetValue(FilterCfgProperty); }
            set { SetValue(FilterCfgProperty, value); }
        }

        /// <summary>
        /// 获取设置排序条件
        /// </summary>
        public SortDescription SortDesc
        {
            get { return (SortDescription)GetValue(SortDescProperty); }
            set { SetValue(SortDescProperty, value); }
        }
        #endregion

        #region 切换数据源
        void LoadDataSource()
        {
            // 重新指定数据源时，清除分页数据源
            if (PageData != null && PageData.State == PageDataState.Normal)
                ClearValue(PageDataProperty);

            if (_dataView != null)
                _dataView.Unload();

            var data = Data;
            if (data == null)
            {
                _dataView = null;
                ClearAllRows();
            }
            else if (data is Table tbl)
            {
                if (AutoCreateCol)
                {
                    ClearAllRows();
                    OnAutoCreateCol(tbl);
                }
                _dataView = new LvDataView(this, tbl);
            }
            else if (data is INotifyList ls)
            {
                if (AutoCreateCol)
                {
                    ClearAllRows();
                    if (ls.Count > 0)
                        OnAutoCreateProp(ls[0].GetType());
                }
                _dataView = new LvDataView(this, ls);
            }

            _dataView?.Refresh();
            OnDataChanged();
        }
        #endregion

        #region 生成视图行
        /// <summary>
        /// 根据数据行生成视图行列表
        /// </summary>
        /// <param name="p_rows"></param>
        internal void LoadRows(IList p_rows)
        {
            _rows.Clear();
            if (_selectedLvItems.Count > 0)
                _selectedLvItems.Clear();

            bool existGroup = false;
            if (GroupRows != null)
            {
                // 清空原有分组行
                GroupRows.Clear();
                GroupRows = null;
                MapRows = null;
                existGroup = true;
            }

            int i = 1;
            foreach (var row in p_rows)
            {
                _rows.Add(new LvItem(this, row, i++));
            }

            _panel?.OnRowsChanged(existGroup);
        }

        /// <summary>
        /// 根据数据行和分组行生成视图行列表
        /// </summary>
        /// <param name="p_groups"></param>
        internal void LoadGroupRows(IList p_groups)
        {
            _rows.Clear();
            if (_selectedLvItems.Count > 0)
                _selectedLvItems.Clear();
            int i = 1;

            MapRows = new List<bool>();
            GroupRows = new List<GroupRow>();
            foreach (var group in p_groups.OfType<IList>())
            {
                GroupRows.Add(new GroupRow(this, group));
                MapRows.Add(true);
                foreach (var row in group)
                {
                    _rows.Add(new LvItem(this, row, i++));
                    MapRows.Add(false);
                }
            }
            if (GroupRows.Count > 0)
                GroupRows[0].IsFirst = true;

            _panel?.OnRowsChanged(true);
        }

        /// <summary>
        /// 批量插入数据行，无排序过滤分组时直接插入！
        /// </summary>
        /// <param name="p_tbl"></param>
        /// <param name="p_start">开始插入位置</param>
        /// <param name="p_count">共插入行数</param>
        internal void BatchInsertRows(IList p_tbl, int p_start, int p_count)
        {
            if (p_start < 0 || p_start > _rows.Count)
                return;

            int index;
            if (p_start == _rows.Count)
            {
                // 添加到末尾
                index = _rows.Count + 1;
                for (int i = 0; i < p_count; i++)
                {
                    _rows.Add(new LvItem(this, p_tbl[i + p_start], index++));
                }
            }
            else
            {
                // 插入
                index = p_start;
                for (int i = 0; i < p_count; i++)
                {
                    _rows.Insert(index, new LvItem(this, p_tbl[index], index + 1));
                    index++;
                }

                // 更新后续行号
                for (int i = index; i < _rows.Count; i++)
                {
                    _rows[i].Index = i + 1;
                }
            }
            _panel?.OnInsertRows(p_start, p_count);
        }

        /// <summary>
        /// 批量删除数据行，无排序过滤分组！
        /// </summary>
        /// <param name="p_items">所有删除项的索引列表，索引已按从小到大排序</param>
        internal void BatchRemoveRows(IList p_items)
        {
            if (p_items == null || p_items.Count == 0)
                return;

            // 从后向前删除
            for (int i = p_items.Count - 1; i >= 0; i--)
            {
                _rows.RemoveAt((int)p_items[i]);
            }
            // 更新后续行号
            for (int i = (int)p_items[0]; i < _rows.Count; i++)
            {
                _rows[i].Index = i + 1;
            }
            _panel?.OnRemoveRows(p_items);
        }

        /// <summary>
        /// 清空所有行
        /// </summary>
        internal void ClearAllRows()
        {
            _rows.Clear();
            if (_selectedLvItems.Count > 0)
                _selectedLvItems.Clear();

            bool existGroup = false;
            if (GroupRows != null)
            {
                GroupRows.Clear();
                GroupRows = null;
                MapRows = null;
                existGroup = true;
            }
            _panel?.OnRowsChanged(existGroup);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 滚动到顶部或底部时添加分页数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnScrollViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var page = PageData;
            if (page.HasMorePages
                && page.State == PageDataState.Normal
                && ((page.InsertTop && Scroll.VerticalOffset == 0)
                    || (!page.InsertTop && Scroll.VerticalOffset > Scroll.ScrollableHeight * 0.9)))
            {
                page.GotoNextPage();
            }
        }

        /// <summary>
        /// 自动生成列
        /// </summary>
        /// <param name="p_tbl"></param>
        void OnAutoCreateCol(Table p_tbl)
        {
            Cols cols = new Cols();
            foreach (var c in p_tbl.Columns)
            {
                cols.Add(new Col { ID = c.ID, Title = c.ID, Width = "200" });
            }
            View = cols;
        }

        void OnAutoCreateProp(Type p_type)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ItemHeight变化
        bool _itemHeightNoReload;

        void OnItemHeightChanged()
        {
            if (!_itemHeightNoReload)
                ReloadPanelContent();
        }

        internal void SetItemHeightNoReload(double p_itemHeight)
        {
            _itemHeightNoReload = true;
            ItemHeight = p_itemHeight;
            _itemHeightNoReload = false;
        }
        #endregion
    }
}