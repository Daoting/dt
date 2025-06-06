#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.TreeViews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 树控件
    /// </summary>
    [ContentProperty(Name = nameof(View))]
    public partial class Tv : DtControl, IViewItemHost, IMenuHost
    {
        #region 静态内容
        public readonly static DependencyProperty ViewProperty = DependencyProperty.Register(
            "View",
            typeof(object),
            typeof(Tv),
            new PropertyMetadata(null, OnViewChanged));

        public readonly static DependencyProperty ItemStyleProperty = DependencyProperty.Register(
            "ItemStyle",
            typeof(Action<TvItemStyleArgs>),
            typeof(Tv),
            new PropertyMetadata(null));

        public readonly static DependencyProperty IsVirtualizedProperty = DependencyProperty.Register(
            "IsVirtualized",
            typeof(bool),
            typeof(Tv),
            new PropertyMetadata(true, OnIsVirtualizedChanged));

        public readonly static DependencyProperty SelectionModeProperty = DependencyProperty.Register(
            "SelectionMode",
            typeof(SelectionMode),
            typeof(Tv),
            new PropertyMetadata(SelectionMode.Single, OnSelectionModeChanged));

        public static readonly DependencyProperty ShowRowLineProperty = DependencyProperty.Register(
            "ShowRowLine",
            typeof(bool),
            typeof(Tv),
            new PropertyMetadata(true, OnReloadAllRows));

        public static readonly DependencyProperty IndentProperty = DependencyProperty.Register(
            "Indent",
            typeof(int),
            typeof(Tv),
            new PropertyMetadata(20));

        public static readonly DependencyProperty EnteredBrushProperty = DependencyProperty.Register(
            "EnteredBrush",
            typeof(Brush),
            typeof(Tv),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x19, 0xff, 0xff, 0x00))));

        public static readonly DependencyProperty PressedBrushProperty = DependencyProperty.Register(
            "PressedBrush",
            typeof(Brush),
            typeof(Tv),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x19, 0x00, 0x00, 0x00))));

        public static readonly DependencyProperty IsDynamicLoadingProperty = DependencyProperty.Register(
            "IsDynamicLoading",
            typeof(bool),
            typeof(Tv),
            new PropertyMetadata(false));

        public static readonly DependencyProperty HasSelectedProperty = DependencyProperty.Register(
            "HasSelected",
            typeof(bool),
            typeof(Tv),
            new PropertyMetadata(false));

        static void OnReloadAllRows(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Tv tv = (Tv)d;
            if (tv._isLoaded)
                tv._panel.Reload();
        }

        static void OnViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Tv tv = (Tv)d;
            if (e.NewValue != null)
            {
                if (tv.IsVirtualized && e.NewValue is DataTemplateSelector)
                    tv.IsVirtualized = false;
                else if (tv._isLoaded)
                    tv._panel.Reload();
            }
        }

        static void OnIsVirtualizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Tv tv = (Tv)d;
            if (tv._isLoaded)
            {
                tv.Scroll.ViewChanged -= tv.OnScrollViewChanged;
                if ((bool)e.NewValue)
                    tv.Scroll.ViewChanged += tv.OnScrollViewChanged;
                tv._panel.Reload();
            }
        }

        static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Tv tv = (Tv)d;
            if (tv._isLoaded)
            {
                foreach (var row in tv.RootItems.GetAllItems())
                {
                    row.ClearValue(TvItem.IsSelectedProperty);
                    row.ValueChanged = null;
                }

                if (tv._selectedRows.Count > 0)
                {
                    try
                    {
                        tv._selectedRows.CollectionChanged -= tv.OnSelectedItemsChanged;
                        tv._selectedRows.Clear();
                        tv.HasSelected = false;
                    }
                    finally
                    {
                        tv._selectedRows.CollectionChanged += tv.OnSelectedItemsChanged;
                    }
                }
                tv._panel.Reload();
            }
        }
        #endregion

        #region 成员变量
        TvDataView _dataView;
        TvPanel _panel;
        bool _isLoaded;
        readonly ObservableCollection<TvItem> _selectedRows;
        #endregion

        #region 构造方法
        public Tv()
        {
            DefaultStyleKey = typeof(Tv);
            RootItems = new TvRootItems(this);
            _selectedRows = new ObservableCollection<TvItem>();
            _selectedRows.CollectionChanged += OnSelectedItemsChanged;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 单击行事件
        /// </summary>
        public event Action<ItemClickArgs> ItemClick;

        /// <summary>
        /// 双击行事件
        /// </summary>
        public event Action<object> ItemDoubleClick;

        /// <summary>
        /// 加载子节点事件
        /// </summary>
        public event Action<LoadingChildArgs> LoadingChild;

        /// <summary>
        /// 选择变化事件
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// 切换数据源事件
        /// </summary>
        public event Action<object> DataChanged;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置节点模板或模板选择器
        /// </summary>
        public object View
        {
            get { return GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        /// <summary>
        /// 获取设置自定义行样式的回调方法
        /// </summary>
        public Action<TvItemStyleArgs> ItemStyle
        {
            get { return (Action<TvItemStyleArgs>)GetValue(ItemStyleProperty); }
            set { SetValue(ItemStyleProperty, value); }
        }

        /// <summary>
        /// 获取设置选择模式，默认Single
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示行分割线，默认true
        /// </summary>
        public bool ShowRowLine
        {
            get { return (bool)GetValue(ShowRowLineProperty); }
            set { SetValue(ShowRowLineProperty, value); }
        }

        /// <summary>
        /// 获取设置是否采用虚拟化，默认true
        /// </summary>
        public bool IsVirtualized
        {
            get { return (bool)GetValue(IsVirtualizedProperty); }
            set { SetValue(IsVirtualizedProperty, value); }
        }

        /// <summary>
        /// 获取设置每层节点的缩进值
        /// </summary>
        public int Indent
        {
            get { return (int)GetValue(IndentProperty); }
            set { SetValue(IndentProperty, value); }
        }

        /// <summary>
        /// 获取设置鼠标进入行/项目时的背景色
        /// </summary>
        public Brush EnteredBrush
        {
            get { return (Brush)GetValue(EnteredBrushProperty); }
            set { SetValue(EnteredBrushProperty, value); }
        }

        /// <summary>
        /// 获取设置点击行/项目时的背景色
        /// </summary>
        public Brush PressedBrush
        {
            get { return (Brush)GetValue(PressedBrushProperty); }
            set { SetValue(PressedBrushProperty, value); }
        }

        /// <summary>
        /// 获取设置是否动态加载节点，默认false
        /// </summary>
        public bool IsDynamicLoading
        {
            get { return (bool)GetValue(IsDynamicLoadingProperty); }
            set { SetValue(IsDynamicLoadingProperty, value); }
        }

        /// <summary>
        /// 获取是否有选择行
        /// </summary>
        public bool HasSelected
        {
            get { return (bool)GetValue(HasSelectedProperty); }
            set { SetValue(HasSelectedProperty, value); }
        }

        /// <summary>
        /// 获取当前选择的节点列表
        /// </summary>
        public IEnumerable<object> SelectedItems
        {
            get
            {
                return from item in _selectedRows
                       select item.Data;
            }
        }

        /// <summary>
        /// 获取设置当前选定行，并自动展开滚动到当前位置，设置null时清空选择，多选时为返回最后选择行
        /// </summary>
        public object SelectedItem
        {
            get
            {
                var row = _selectedRows.LastOrDefault();
                if (row != null)
                    return row.Data;
                return null;
            }
            set
            {
                // 清空选择
                if (value == null)
                {
                    if (_selectedRows.Count > 0)
                        _selectedRows.Clear();
                    return;
                }

                // 行是否存在
                var selectedRow = (from row in RootItems.GetAllItems()
                                   where row.Data == value
                                   select row).FirstOrDefault();
                if (selectedRow == null)
                    return;

                // 挑出取消选择的行
                bool exist = false;
                List<object> removes = new List<object>();
                foreach (var row in _selectedRows)
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
                    _selectedRows.CollectionChanged -= OnSelectedItemsChanged;
                    if (_selectedRows.Count > 0)
                        _selectedRows.Clear();
                    _selectedRows.Add(selectedRow);
                    selectedRow.IsSelected = true;
                    if (_panel != null)
                        _panel.ScrollIntoItem(selectedRow);
                    HasSelected = true;
                }
                finally
                {
                    _selectedRows.CollectionChanged += OnSelectedItemsChanged;
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
        /// 获取选择的行数
        /// </summary>
        public int SelectedCount
        {
            get { return _selectedRows.Count; }
        }

        /// <summary>
        /// 获取当前选定的Row
        /// </summary>
        public Row SelectedRow
        {
            get { return SelectedItem as Row; }
        }
        #endregion

        #region 内部属性
        /// <summary>
        /// 获取根节点集合
        /// </summary>
        internal TvRootItems RootItems { get; set; }

        internal ScrollViewer Scroll { get; set; }

        internal TvPanel Panel
        {
            get { return _panel; }
        }

        internal IList<TvItem> SelectedRows
        {
            get { return _selectedRows; }
        }

        /// <summary>
        /// 滚动栏是否在内部
        /// </summary>
        internal bool IsInnerScroll
        {
            get { return Scroll.Content as Panel == _panel; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 获取当前选定的实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns></returns>
        public TEntity Selected<TEntity>()
        where TEntity : Entity
        {
            return SelectedItem as TEntity;
        }

        /// <summary>
        /// 删除数据对应的节点，若节点为选择状态，则删除后选择同层后一节点或父节点
        /// </summary>
        /// <param name="p_item"></param>
        public void DeleteItem(object p_item)
        {
            var item = (from row in RootItems.GetAllItems()
                        where row.Data == p_item
                        select row).FirstOrDefault();
            if (item == null)
                return;

            int index;
            if (item.Parent == null)
            {
                index = RootItems.IndexOf(item);
                if (index > -1)
                {
                    RootItems.RemoveAt(index);
                    if ((bool)item.IsSelected)
                    {
                        if (index < RootItems.Count)
                            SelectedItem = RootItems[index].Data;
                        else if (RootItems.Count > 0)
                            SelectedItem = RootItems[RootItems.Count - 1].Data;
                        else
                            SelectedItem = null;
                    }
                }
            }
            else
            {
                var ls = item.Parent.Children;
                index = ls.IndexOf(item);
                if (index > -1)
                {
                    ls.RemoveAt(index);
                    if ((bool)item.IsSelected)
                    {
                        if (index < ls.Count)
                            SelectedItem = ls[index].Data;
                        else if (ls.Count > 0)
                            SelectedItem = ls[ls.Count - 1].Data;
                        else
                            SelectedItem = item.Parent.Data;
                    }
                }
            }
            RootItems.Invalidate();
        }

        /// <summary>
        /// 获取同层上面的节点
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        public object GetTopBrother(object p_item)
        {
            var item = (from row in RootItems.GetAllItems()
                        where row.Data == p_item
                        select row).FirstOrDefault();
            if (item == null)
                return null;

            int index;
            if (item.Parent == null)
            {
                index = RootItems.IndexOf(item);
                if (index > 0)
                    return RootItems[index - 1].Data;
            }
            else
            {
                index = item.Parent.Children.IndexOf(item);
                if (index > 0)
                    return item.Parent.Children[index - 1].Data;
            }
            return null;
        }

        /// <summary>
        /// 获取同层下面的节点
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        public object GetFollowingBrother(object p_item)
        {
            var item = (from row in RootItems.GetAllItems()
                        where row.Data == p_item
                        select row).FirstOrDefault();
            if (item == null)
                return null;

            int index;
            if (item.Parent == null)
            {
                index = RootItems.IndexOf(item);
                if (index >= 0 && index < RootItems.Count - 1)
                    return RootItems[index + 1].Data;
            }
            else
            {
                index = item.Parent.Children.IndexOf(item);
                if (index >= 0 && index < item.Parent.Children.Count - 1)
                    return item.Parent.Children[index + 1].Data;
            }
            return null;
        }

        /// <summary>
        /// 根据数据项设置对应节点的状态
        /// </summary>
        /// <param name="p_item">数据项</param>
        /// <param name="p_status"></param>
        public void SetItemSelectionStatus(object p_item, int p_status)
        {
            TvItem item = RootItems.GetAllItems().Where(w => w.Data == p_item).FirstOrDefault();
            if (item == null)
            {
                return;
            }

            TvItem tvItem = _selectedRows.Where(w => w.Data == item).FirstOrDefault();
            if (tvItem == null)
            {
                if (p_status == 0)
                {
                    if (item.IsSelected == false)
                    {
                        return;
                    }
                }

                if (p_status == 2)
                {
                    if (item.IsSelected == null)
                    {
                        return;
                    }
                }
            }
            else
            {
                if (p_status == 1)
                {
                    return;
                }
            }

            try
            {
                _selectedRows.CollectionChanged -= OnSelectedItemsChanged;
                switch (p_status)
                {
                    case 0:
                        if (_selectedRows.Contains(item))
                        {
                            _selectedRows.Remove(item);
                        }
                        item.IsSelected = false;
                        HasSelected = false;
                        break;
                    case 1:
                        _selectedRows.Add(item);
                        item.IsSelected = true;
                        HasSelected = true;
                        break;
                    case 2:
                        if (_selectedRows.Contains(item))
                        {
                            _selectedRows.Remove(item);
                        }
                        item.IsSelected = null;
                        HasSelected = false;
                        break;
                }
                if (_panel != null)
                {
                    _panel.ScrollIntoItem(item);
                }
            }
            finally
            {
                _selectedRows.CollectionChanged += OnSelectedItemsChanged;
            }
        }

        /// <summary>
        /// 复制选择行数据
        /// </summary>
        public void CopySelection()
        {
            if (SelectionMode == SelectionMode.None || _selectedRows.Count == 0)
                return;

            StringBuilder sb = new StringBuilder();
            if (_selectedRows[0].Data is Row)
            {
                foreach (var item in _selectedRows)
                {
                    Row row = item.Data as Row;
                    foreach (var cell in row.Cells)
                    {
                        if (cell.Val != null)
                            sb.Append(cell.Val);
                        sb.Append(" ");
                    }
                    sb.AppendLine();
                }
            }
            else
            {
                var props = _selectedRows[0].Data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.DeclaredOnly);
                foreach (var item in _selectedRows)
                {
                    foreach (var p in props)
                    {
                        var val = p.GetValue(item.Data);
                        if (val != null)
                            sb.Append(val);
                        sb.Append(" ");
                    }
                    sb.AppendLine();
                }
            }

            DataPackage data = new DataPackage();
            data.SetText(sb.ToString());
            Clipboard.SetContent(data);
            Kit.Msg("已成功复制到剪贴板！");
        }
        #endregion

        #region 展开/折叠
        /// <summary>
        /// 递归展开所有节点，动态加载时只展开已有节点
        /// </summary>
        public void ExpandAll()
        {
            SetAllExpandState(true);
        }

        /// <summary>
        /// 递归折叠所有节点
        /// </summary>
        public void CollapseAll()
        {
            SetAllExpandState(false);
        }

        void SetAllExpandState(bool p_isExpanded)
        {
            foreach (var item in RootItems.GetAllItems())
            {
                item.SetExpandState(p_isExpanded);
            }
            RootItems.Invalidate();
        }
        #endregion

        #region 滚动到
        /// <summary>
        /// 滚动到最顶端
        /// </summary>
        public void ScrollTop()
        {
            if (_panel == null)
                return;

            if (Scroll.Content as Panel == _panel)
            {
                // 内部滚动栏
                Scroll.ChangeView(null, 0, null);
            }
            else
            {
                // 外部滚动栏
                Point pt = _panel.TransformToVisual(Scroll).TransformPoint(new Point());
                Scroll.ChangeView(null, Scroll.VerticalOffset + pt.Y, null);
            }
        }

        /// <summary>
        /// 滚动到最底端
        /// </summary>
        public void ScrollBottom()
        {
            if (_panel == null)
                return;

            if (Scroll.Content as Panel == _panel)
            {
                // 内部滚动栏
                Scroll.ChangeView(null, Scroll.ScrollableHeight, null);
            }
            else
            {
                // 外部滚动栏
                Point pt = _panel.TransformToVisual(Scroll).TransformPoint(new Point());
                Scroll.ChangeView(null, Scroll.VerticalOffset + pt.Y + _panel.ActualHeight - Scroll.ViewportHeight, null);
            }
        }

        /// <summary>
        /// 滚动到指定的节点
        /// </summary>
        /// <param name="p_item"></param>
        public void ScrollInto(object p_item)
        {
            if (_panel != null)
            {
                var item = (from row in RootItems.GetAllItems()
                            where row.Data == p_item
                            select row).FirstOrDefault();
                if (item != null)
                    _panel.ScrollIntoItem(item);
            }
        }
        #endregion

        #region 重写方法
        protected override Size MeasureOverride(Size availableSize)
        {
            // 准确获取高度
            if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
            {
                // 外部无ScrollViewer StackPanel的情况
                _panel.SetMaxSize(availableSize);
            }
            else
            {
                // 和Lv相似，参见win.xaml：win模式在Tabs定义，phone模式在Tab定义
                var pre = Scroll.FindParentInWin<SizedPresenter>();
                if (pre != null)
                {
                    _panel.SetMaxSize(pre.AvailableSize);
                }
                else
                {
                    // 无有效大小时以窗口大小为准
                    double width = double.IsInfinity(availableSize.Width) ? Kit.ViewWidth : availableSize.Width;
                    double height = double.IsInfinity(availableSize.Height) ? Kit.ViewHeight : availableSize.Height;
                    _panel.SetMaxSize(new Size(width, height));
                }
            }
            return base.MeasureOverride(availableSize);
        }
        #endregion

        #region 加载过程
        protected override void OnLoadTemplate()
        {
            _panel = new TvPanel(this);
            var root = (Border)GetTemplateChild("Border");

            // win模式查询范围限制在Tabs内，phone模式限制在Tab内
            var scroll = this.FindParentInWin<ScrollViewer>();
            if (scroll == null)
            {
                // 内部滚动栏
                scroll = new ScrollViewer();
                scroll.Content = _panel;
                root.Child = scroll;
            }
            else
            {
                // 外部滚动栏
                root.Child = _panel;
            }
            scroll.HorizontalScrollMode = ScrollMode.Auto;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.VerticalScrollMode = ScrollMode.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            if (IsVirtualized)
                scroll.ViewChanged += OnScrollViewChanged;
            Scroll = scroll;

            _isLoaded = true;
        }

        protected override void OnFirstLoaded()
        {
            Focus(FocusState.Programmatic);
            KeyDown += OnKeyDown;
        }

        void OnScrollViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // 虚拟化滚动时重新布局
            _panel.InvalidateArrange();
        }
        #endregion

        #region 选择
        /// <summary>
        /// 选择行集合变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // HasSelected状态
            bool hasSelected = HasSelected;
            if (_selectedRows.Count == 0 && hasSelected)
                ClearValue(HasSelectedProperty);
            else if (_selectedRows.Count > 0 && !hasSelected)
                HasSelected = true;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // 增加
                if (e.NewItems != null && e.NewItems.Count > 0)
                {
                    List<object> adds = new List<object>();
                    foreach (var row in e.NewItems.OfType<TvItem>())
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
                    foreach (var row in e.OldItems.OfType<TvItem>())
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
                foreach (var row in RootItems.GetAllItems())
                {
                    if (row.IsSelected.HasValue && row.IsSelected.Value)
                        removals.Add(row.Data);
                    row.IsSelected = false;
                }
                if (removals.Count > 0 && SelectionChanged != null)
                    SelectionChanged(this, new SelectionChangedEventArgs(removals, new List<object>()));
            }
        }

        /// <summary>
        /// 单选模式点击时切换选择
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="p_fireEvent">是否触发选择变化事件</param>
        internal void OnToggleSelected(TvItem p_item, bool p_fireEvent = true)
        {
            List<object> removes = new List<object>();
            try
            {
                _selectedRows.CollectionChanged -= OnSelectedItemsChanged;
                if (_selectedRows.Count > 0)
                {
                    var row = _selectedRows[0];
                    row.IsSelected = false;
                    removes.Add(row.Data);
                    _selectedRows.Clear();
                }

                _selectedRows.Add(p_item);
                p_item.IsSelected = true;
                HasSelected = true;
            }
            finally
            {
                _selectedRows.CollectionChanged += OnSelectedItemsChanged;
            }
            
            if (p_fireEvent && SelectionChanged != null)
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(removes, new List<object> { p_item.Data }));
        }

        /// <summary>
        /// 触发清空选择事件
        /// </summary>
        internal void OnClearSelection()
        {
            if (SelectionChanged != null)
            {
                var removals = new List<object>();
                SelectionChanged(this, new SelectionChangedEventArgs(removals, removals));
            }
        }
        
        /// <summary>
        /// 多选模式时级联调整选择状态
        /// </summary>
        /// <param name="p_item"></param>
        internal void ToggleSelectedCascade(TvItem p_item)
        {
            _selectedRows.CollectionChanged -= OnSelectedItemsChanged;
            var adds = new List<object>();
            var removes = new List<object>();
            try
            {
                if (!p_item.IsSelected.HasValue || p_item.IsSelected.Value)
                    UnselectCascade(p_item, removes);
                else
                    SelectCascade(p_item, adds);
            }
            finally
            {
                _selectedRows.CollectionChanged += OnSelectedItemsChanged;
            }

            if (SelectionChanged != null && (adds.Count > 0 || removes.Count > 0))
                SelectionChanged(this, new SelectionChangedEventArgs(removes, adds));
        }

        /// <summary>
        /// 级联选择
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="p_data"></param>
        void SelectCascade(TvItem p_item, List<object> p_data)
        {
            p_item.IsSelected = true;
            _selectedRows.Add(p_item);
            p_data.Add(p_item.Data);

            if (p_item.Parent != null)
                UpdateParentSelect(p_item.Parent, p_data);
            UpdateChildSelect(p_item, p_data);
        }

        /// <summary>
        /// 向上递归更新选择状态
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="p_data"></param>
        void UpdateParentSelect(TvItem p_item, List<object> p_data)
        {
            bool isAll = true;
            foreach (var item in p_item.Children)
            {
                if (!item.IsSelected.HasValue || !item.IsSelected.Value)
                {
                    isAll = false;
                    break;
                }
            }
            if (isAll)
            {
                // 子节点全选
                p_item.IsSelected = true;
                _selectedRows.Add(p_item);
                p_data.Add(p_item.Data);
            }
            else
            {
                // 部分选，未知状态
                p_item.IsSelected = null;
            }

            p_item = p_item.Parent;
            while (p_item != null)
            {
                UpdateParentSelect(p_item, p_data);
                p_item = p_item.Parent;
            }
        }

        void UpdateChildSelect(TvItem p_item, List<object> p_data)
        {
            if (p_item.Children.Count == 0)
                return;

            foreach (var item in p_item.Children)
            {
                if (!item.IsSelected.HasValue || !item.IsSelected.Value)
                {
                    item.IsSelected = true;
                    _selectedRows.Add(p_item);
                    p_data.Add(p_item.Data);
                }
                UpdateChildSelect(item, p_data);
            }
        }

        void UnselectCascade(TvItem p_item, List<object> p_data)
        {
            if (p_item.IsSelected.HasValue && p_item.IsSelected.Value)
            {
                _selectedRows.Remove(p_item);
                p_data.Add(p_item.Data);
            }
            p_item.IsSelected = false;

            if (p_item.Parent != null)
                UpdateParentUnselect(p_item.Parent, p_data);
            UpdateChildUnselect(p_item, p_data);
        }

        void UpdateParentUnselect(TvItem p_item, List<object> p_data)
        {
            bool isAll = true;
            foreach (var item in p_item.Children)
            {
                if (!item.IsSelected.HasValue || item.IsSelected.Value)
                {
                    isAll = false;
                    break;
                }
            }
            if (isAll)
            {
                // 子节点全未选
                if (p_item.IsSelected.HasValue && p_item.IsSelected.Value)
                {
                    _selectedRows.Remove(p_item);
                    p_data.Add(p_item.Data);
                }
                p_item.IsSelected = false;
            }
            else
            {
                // 部分选，未知状态
                p_item.IsSelected = null;
            }

            p_item = p_item.Parent;
            while (p_item != null)
            {
                UpdateParentUnselect(p_item, p_data);
                p_item = p_item.Parent;
            }
        }

        void UpdateChildUnselect(TvItem p_item, List<object> p_data)
        {
            if (p_item.Children.Count == 0)
                return;

            foreach (var item in p_item.Children)
            {
                if (item.IsSelected.HasValue && item.IsSelected.Value)
                {
                    _selectedRows.Remove(p_item);
                    p_data.Add(p_item.Data);
                }
                item.IsSelected = false;
                UpdateChildUnselect(item, p_data);
            }
        }
        #endregion

        #region IViewItemHost
        bool IViewItemHost.IsCustomItemStyle => ItemStyle != null;

        void IViewItemHost.SetItemStyle(ViewItem p_item)
        {
            ItemStyle?.Invoke(new TvItemStyleArgs(p_item as TvItem));
        }
        #endregion

        #region IMenuHost
        /// <summary>
        /// 切换上下文菜单或修改触发事件种类时通知宿主刷新
        /// </summary>
        void IMenuHost.UpdateContextMenu()
        {
            if (_isLoaded)
                _panel.Reload();
        }
        #endregion

        #region 键盘操作
        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (SelectionMode == SelectionMode.Multiple || RootItems.Count == 0)
                return;

            TvItem row;
            switch (e.Key)
            {
                case VirtualKey.Down:
                    row = SelectedRows.LastOrDefault();
                    if (row == null)
                    {
                        OnToggleSelected(RootItems[0]);
                    }
                    else
                    {
                        using (var ls = RootItems.GetExpandedItems().GetEnumerator())
                        {
                            while (ls.MoveNext())
                            {
                                if (ls.Current == row)
                                {
                                    if (ls.MoveNext())
                                        OnToggleSelected(ls.Current);
                                    else
                                        OnToggleSelected(RootItems[0]);
                                    break;
                                }
                            }
                        }
                    }
                    e.Handled = true;
                    return;

                case VirtualKey.Up:
                    row = SelectedRows.LastOrDefault();
                    if (row == null)
                    {
                        OnToggleSelected(RootItems[0]);
                    }
                    else
                    {
                        using (var ls = RootItems.GetExpandedItems().GetEnumerator())
                        {
                            if (ls.MoveNext())
                            {
                                if (ls.Current == row)
                                {
                                    SelectedItem = null;
                                }
                                else
                                {
                                    while (true)
                                    {
                                        TvItem lastRow = ls.Current;
                                        if (ls.MoveNext())
                                        {
                                            if (ls.Current == row)
                                            {
                                                OnToggleSelected(lastRow);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    e.Handled = true;
                    return;

                case VirtualKey.Right:
                    row = SelectedRows.LastOrDefault();
                    if (row != null)
                        row.IsExpanded = true;
                    e.Handled = true;
                    return;

                case VirtualKey.Left:
                    row = SelectedRows.LastOrDefault();
                    if (row != null)
                        row.IsExpanded = false;
                    e.Handled = true;
                    return;

                case VirtualKey.Enter:
                    row = SelectedRows.LastOrDefault();
                    if (row != null)
                        row.OnClick();
                    return;

                default:
                    return;
            }
        }
        #endregion

        #region 内部方法
        internal void ApplyFilterFlag()
        {
            if (_dataView != null)
                _dataView.ApplyFilterFlag();
        }
        #endregion
        
        #region 触发事件
        /// <summary>
        /// 触发单击行事件
        /// </summary>
        /// <param name="p_data"></param>
        /// <param name="p_oldData"></param>
        internal void OnItemClick(object p_data, object p_oldData)
        {
            ItemClick?.Invoke(new ItemClickArgs(p_data, p_oldData));
        }

        /// <summary>
        /// 触发双击行事件
        /// </summary>
        /// <param name="p_data"></param>
        internal void OnItemDoubleClick(object p_data)
        {
            ItemDoubleClick?.Invoke(p_data);
        }

        /// <summary>
        /// 触发加载子节点事件
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        internal async Task OnLoadingChild(TvItem p_item)
        {
            if (LoadingChild == null)
                return;

            var args = new LoadingChildArgs(p_item);
            LoadingChild(args);
            await args.EnsureAllCompleted();
            if (args.Children != null)
            {
                foreach (var item in args.Children)
                {
                    TvItem ti = new TvItem(this, item, p_item);
                    ti.ExpandedState = TvItemExpandedState.NotExpanded;
                    p_item.Children.Add(ti);
                }
                RootItems.Invalidate();
            }
            p_item.HasLoadedChildren = true;
        }

        /// <summary>
        /// 触发切换数据源事件
        /// </summary>
        void OnDataChanged()
        {
            DataChanged?.Invoke(Data);
        }
        #endregion
    }
}