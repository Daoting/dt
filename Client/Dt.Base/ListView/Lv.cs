#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列表控件
    /// </summary>
    [ContentProperty(Name = nameof(View))]
    public partial class Lv : Control, IViewItemHost, IMenuHost
    {
        #region 静态内容
        public readonly static DependencyProperty ViewProperty = DependencyProperty.Register(
            "View",
            typeof(object),
            typeof(Lv),
            new PropertyMetadata(null, OnViewChanged));

        public readonly static DependencyProperty ViewModeProperty = DependencyProperty.Register(
            "ViewMode",
            typeof(ViewMode),
            typeof(Lv),
            new PropertyMetadata(ViewMode.List, OnViewModeChanged));

        public readonly static DependencyProperty ItemStyleProperty = DependencyProperty.Register(
            "ItemStyle",
            typeof(Action<ItemStyleArgs>),
            typeof(Lv),
            new PropertyMetadata(null));

        public readonly static DependencyProperty PhoneViewModeProperty = DependencyProperty.Register(
            "PhoneViewMode",
            typeof(ViewMode?),
            typeof(Lv),
            new PropertyMetadata(null, OnPhoneViewModeChanged));

        public readonly static DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            "ItemHeight",
            typeof(double),
            typeof(Lv),
            new PropertyMetadata(0d, OnItemHeightChanged));

        public static readonly DependencyProperty ShowItemBorderProperty = DependencyProperty.Register(
            "ShowItemBorder",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(true, OnReload));

        public static readonly DependencyProperty EnteredBrushProperty = DependencyProperty.Register(
            "EnteredBrush",
            typeof(Brush),
            typeof(Lv),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x19, 0xff, 0xff, 0x00))));

        public static readonly DependencyProperty PressedBrushProperty = DependencyProperty.Register(
            "PressedBrush",
            typeof(Brush),
            typeof(Lv),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x19, 0x00, 0x00, 0x00))));

        public static readonly DependencyProperty AutoScrollBottomProperty = DependencyProperty.Register(
            "AutoScrollBottom",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(false));

        public static readonly DependencyProperty AutoFocusProperty = DependencyProperty.Register(
            "AutoFocus",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(true));

        public static readonly DependencyProperty AutoCreateColProperty = DependencyProperty.Register(
            "AutoCreateCol",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(false, OnAutoCreateColChanged));

        static void OnAutoCreateColChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Lv lv = (Lv)d;
            if ((bool)e.NewValue)
            {
                // 初始化空列
                lv.ViewMode = ViewMode.Table;
                lv.View = new Cols();
            }
        }

        public readonly static DependencyProperty MinItemWidthProperty = DependencyProperty.Register(
            "MinItemWidth",
            typeof(double),
            typeof(Lv),
            new PropertyMetadata(160d, OnReload));

        public readonly static DependencyProperty ToolbarProperty = DependencyProperty.Register(
            "Toolbar",
            typeof(Menu),
            typeof(Lv),
            new PropertyMetadata(null, OnReload));

        static void OnViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Cols cols = e.NewValue as Cols;
            if (cols != null)
                cols.FixWidth();

            if (e.OldValue == null)
            {
                // 初次设置View
                ((Lv)d).LoadPanel();
            }
            else
            {
                ((Lv)d).ReloadPanelContent();
            }
        }

        static void OnViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Lv)d).LoadPanel();
        }

        static void OnPhoneViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Kit.IsPhoneUI)
                ((Lv)d).LoadPanel();
        }

        static void OnReload(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Lv)d).ReloadPanelContent();
        }

        static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Lv)d).OnItemHeightChanged();
        }
        #endregion

        #region 成员变量
        Border _root;
        LvPanel _panel;
        LvDataView _dataView;
        readonly List<LvItem> _rows;
        readonly ObservableCollection<LvItem> _selectedLvItems;
        SizedPresenter _sizedPresenter;
        #endregion

        #region 构造方法
        public Lv()
        {
            DefaultStyleKey = typeof(Lv);

            _rows = new List<LvItem>();
            _selectedLvItems = new ObservableCollection<LvItem>();
            _selectedLvItems.CollectionChanged += OnSelectedItemsChanged;
            Loaded += OnLoaded;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 单击行/项目事件
        /// </summary>
        public event EventHandler<ItemClickArgs> ItemClick;

        /// <summary>
        /// 双击行/项目事件，慎用：
        /// <para>iOS始终不触发</para>
        /// <para>该事件之前始终触发ItemClick，若两个事件都有操作容易无法区分</para>
        /// </summary>
        public event EventHandler<object> ItemDoubleClick;

        /// <summary>
        /// 选择变化事件
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// 切换数据源事件
        /// </summary>
        public event EventHandler<INotifyList> DataChanged;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置行视图，DataTemplate、DataTemplateSelector、Cols列定义 或 IRowView
        /// </summary>
        public object View
        {
            get { return GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        /// <summary>
        /// 获取设置视图类型：列表、表格、磁贴，默认List
        /// </summary>
        public ViewMode ViewMode
        {
            get { return (ViewMode)GetValue(ViewModeProperty); }
            set { SetValue(ViewModeProperty, value); }
        }

        /// <summary>
        /// 获取设置自定义行样式的回调方法
        /// </summary>
        public Action<ItemStyleArgs> ItemStyle
        {
            get { return (Action<ItemStyleArgs>)GetValue(ItemStyleProperty); }
            set { SetValue(ItemStyleProperty, value); }
        }

        /// <summary>
        /// 获取设置Phone模式下的视图类型，null时Win,Phone两模式统一采用ViewMode，默认null
        /// </summary>
        public ViewMode? PhoneViewMode
        {
            get { return (ViewMode?)GetValue(PhoneViewModeProperty); }
            set { SetValue(PhoneViewModeProperty, value); }
        }

        /// <summary>
        /// 获取设置行/项目高度，0时以第一项高度为准，NaN时自动调整高度(性能差)，默认0
        /// </summary>
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示行/项目分割线，默认true
        /// </summary>
        public bool ShowItemBorder
        {
            get { return (bool)GetValue(ShowItemBorderProperty); }
            set { SetValue(ShowItemBorderProperty, value); }
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
        /// 获取设置数据变化时是否自动滚动到最底端，默认false
        /// </summary>
        public bool AutoScrollBottom
        {
            get { return (bool)GetValue(AutoScrollBottomProperty); }
            set { SetValue(AutoScrollBottomProperty, value); }
        }

        /// <summary>
        /// 获取设置加载后是否自动为输入焦点，默认true
        /// </summary>
        public bool AutoFocus
        {
            get { return (bool)GetValue(AutoFocusProperty); }
            set { SetValue(AutoFocusProperty, value); }
        }

        /// <summary>
        /// 获取设置是否根据数据源自动生成列
        /// </summary>
        public bool AutoCreateCol
        {
            get { return (bool)GetValue(AutoCreateColProperty); }
            set { SetValue(AutoCreateColProperty, value); }
        }

        /// <summary>
        /// 获取设置项目的最小宽度，默认160，只磁贴视图有效！
        /// </summary>
        public double MinItemWidth
        {
            get { return (double)GetValue(MinItemWidthProperty); }
            set { SetValue(MinItemWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置顶部的工具栏
        /// </summary>
        public Menu Toolbar
        {
            get { return (Menu)GetValue(ToolbarProperty); }
            set { SetValue(ToolbarProperty, value); }
        }

        /// <summary>
        /// 获取Table数据源
        /// </summary>
        public Table Table
        {
            get { return GetValue(DataProperty) as Table; }
        }
        #endregion

        #region 内部属性
        /// <summary>
        /// 所有视图行
        /// </summary>
        internal List<LvItem> Rows
        {
            get { return _rows; }
        }

        /// <summary>
        /// 所有分组行
        /// </summary>
        internal List<GroupRow> GroupRows { get; set; }

        /// <summary>
        /// 包含分组时，按顺序排列的所有分组行和视图行，true 分组行，false视图行
        /// </summary>
        internal List<bool> MapRows { get; set; }

        /// <summary>
        /// 是否为表格视图
        /// </summary>
        internal bool IsTableView
        {
            get { return View is Cols && CurrentViewMode == ViewMode.Table; }
        }

        /// <summary>
        /// 是否采用虚拟行
        /// </summary>
        internal bool IsVir
        {
            get { return !(View is DataTemplateSelector || double.IsNaN(ItemHeight) || View is IRowView); }
        }

        internal ViewMode CurrentViewMode
        {
            get { return Kit.IsPhoneUI ? (PhoneViewMode.HasValue ? PhoneViewMode.Value : ViewMode) : ViewMode; }
        }

        internal ScrollViewer Scroll { get; set; }

        /// <summary>
        /// 滚动栏是否在内部
        /// </summary>
        internal bool IsInnerScroll => _root != null && (_root.Child is ScrollViewer || _root.Child is RefreshContainer);

        internal Cols Cols
        {
            get { return View as Cols; }
        }

        internal IList<LvItem> SelectedLvItems
        {
            get { return _selectedLvItems; }
        }

        /// <summary>
        /// 是否存在默认筛选框过滤
        /// </summary>
        internal bool ExistDefaultFilterCfg => FilterCfg != null && FilterCfg.MyFilter == null;
        #endregion

        #region 外部方法
        /// <summary>
        /// 刷新数据视图，通常在动态过滤时调用
        /// </summary>
        public void Refresh()
        {
            _dataView?.Refresh();
        }

        /// <summary>
        /// 切换视图，同时调整多属性时只刷新一次，性能高！
        /// </summary>
        /// <param name="p_view">null时不切换</param>
        /// <param name="p_viewMode">null时不切换</param>
        public void ChangeView(object p_view, ViewMode? p_viewMode)
        {
            using (Defer())
            {
                if (p_view != null)
                    View = p_view;

                if (p_viewMode.HasValue && ViewMode != p_viewMode.Value)
                    ViewMode = p_viewMode.Value;
            }
        }

        /// <summary>
        /// 设置为输入焦点
        /// </summary>
        public void SetFocus()
        {
            if (_panel != null)
                _panel.ReceiveFocus();
        }

        /// <summary>
        /// 获取行UI，不支持虚拟行的情况！使用场景少
        /// </summary>
        /// <param name="p_index">行索引</param>
        /// <returns></returns>
        public FrameworkElement GetRowUI(int p_index)
        {
            if (!IsVir && _panel != null)
                return _panel.GetLvRow(p_index);
            return null;
        }

        /// <summary>
        /// 显示默认数据筛选对话框，对本地数据源过滤，列名为 ICell.ID，主要提供给内部管理员使用
        /// </summary>
        public void ShowFilterDlg()
        {
            if (Data == null || Data.Count == 0)
                Kit.Warn("数据源为空，不需要筛选！");
            else
                new DefFilterDlg().ShowDlg(this);
        }

        /// <summary>
        /// 复制选择行数据
        /// </summary>
        public void CopySelection()
        {
            if (SelectionMode == SelectionMode.None || _selectedLvItems.Count == 0)
                return;

            StringBuilder sb = new StringBuilder();
            if (ViewMode == ViewMode.Table && View is Cols cols)
            {
                foreach (var item in _selectedLvItems)
                {
                    foreach (var col in cols)
                    {
                        var val = item[col.ID];
                        if (val != null)
                            sb.Append(val);
                        // Tab复制到Excel自动分列
                        sb.Append(" ");
                    }
                    sb.AppendLine();
                }
            }
            else if (_selectedLvItems[0].Data is Row)
            {
                foreach (var item in _selectedLvItems)
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
                var props = _selectedLvItems[0].Data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.DeclaredOnly);
                foreach (var item in _selectedLvItems)
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

        #region 滚动到可视
        /// <summary>
        /// 滚动到最顶端
        /// </summary>
        public void ScrollTop()
        {
            if (_panel != null)
                _panel.ScrollInto(0);
        }

        /// <summary>
        /// 滚动到最底端
        /// </summary>
        public void ScrollBottom()
        {
            if (_panel != null)
                _panel.ScrollInto(Rows.Count - 1);
        }

        /// <summary>
        /// 将指定行滚动到可视区域
        /// </summary>
        /// <param name="p_index">行索引</param>
        public void ScrollInto(int p_index)
        {
            if (_panel != null)
                _panel.ScrollInto(p_index);
        }

        /// <summary>
        /// 滚动到指定的数据行
        /// </summary>
        /// <param name="p_row"></param>
        public void ScrollInto(object p_row)
        {
            if (_panel != null)
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    if (_rows[i].Data == p_row)
                    {
                        _panel.ScrollInto(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 点击分组导航头链接，滚动到指定的分组
        /// </summary>
        /// <param name="p_group"></param>
        internal void ScrollIntoGroup(GroupRow p_group)
        {
            if (IsInnerScroll)
            {
                // 启用动画会界面抖动！
                // 16为分组行上部的间隔高度
                Scroll.ChangeView(null, p_group.IsFirst ? 0 : p_group.Top + LvPanel.GroupSeparatorHeight, null, true);
            }
            else
            {
                // 不能用p_group计算相对位置，因不可见时被布局在空区域
                var pt = _panel.TransformToVisual(Scroll).TransformPoint(new Point());
                double y = Scroll.VerticalOffset + pt.Y + p_group.Top;
                if (!p_group.IsFirst)
                    y += LvPanel.GroupSeparatorHeight;
                Scroll.ChangeView(null, y, null, true);
            }
        }
        #endregion

        #region IViewItemHost
        bool IViewItemHost.IsCustomItemStyle => ItemStyle != null;

        void IViewItemHost.SetItemStyle(ViewItem p_item)
        {
            ItemStyle?.Invoke(new ItemStyleArgs(p_item));
        }
        #endregion

        #region IMenuHost
        /// <summary>
        /// 切换上下文菜单或修改触发事件种类时通知宿主刷新
        /// </summary>
        void IMenuHost.UpdateContextMenu()
        {
            ReloadPanelContent();
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
            ItemClick?.Invoke(this, new ItemClickArgs(p_data, p_oldData));
        }

        /// <summary>
        /// 触发双击行事件
        /// </summary>
        /// <param name="p_data"></param>
        internal void OnItemDoubleClick(object p_data)
        {
            ItemDoubleClick?.Invoke(this, p_data);
        }

        /// <summary>
        /// 触发切换数据源事件
        /// </summary>
        void OnDataChanged()
        {
            DataChanged?.Invoke(this, Data);
        }
        #endregion
    }
}