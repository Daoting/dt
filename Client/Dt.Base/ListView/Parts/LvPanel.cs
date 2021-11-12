#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// Lv布局面板基类
    /// </summary>
    public abstract partial class LvPanel : Panel
    {
        /************************************************************************************************************************************/
        // 平台调用顺序不同：
        // UWP：父OnApplyTemplate > 父MeasureOverride > 子MeasureOverride > 父ArrangeOverride > 子ArrangeOverride > 父SizeChanged > 子SizeChanged > 父Loaded > 子Loaded
        // Adr：父OnApplyTemplate > 父Loaded > 子Loaded > 父MeasureOverride > 子MeasureOverride > 父ArrangeOverride > 子ArrangeOverride > 子SizeChanged > 父SizeChanged
        // iOS：父OnApplyTemplate > 子Loaded > 父Loaded > 父MeasureOverride > 子MeasureOverride > 父SizeChanged > 子SizeChanged > 父ArrangeOverride > 子ArrangeOverride
        //
        // uwp的OnApplyTemplate时控件已在可视树上，可查询父元素；uno此时不在可视树上，只能在Loaded时查询父元素！！！
        // 
        // 在MeasureOverride中尽可能不增删Children元素，uno中每增删一个元素会重复一次MeasureOverride，严重时死循环！！！
        // 采用虚拟行模式时，需要根据可视区大小确定生成的虚拟行行数，可视区大小在MeasureOverride时才能确定，故解决方法：
        // 在Lv.MeasureOverride时准确获取可见区大小，若大小变化则重新生成虚拟行，添加虚拟行会造成多次MeasureOverride，未发现其他好方法！！！
        // 若放在SizeChanged中生成虚拟行时uno会警告 requestLayout() improperly called by xxx: posting in next frame！！！
        //
        // 重新生成虚拟行的场景：
        // 1. 可视区大小变化时 LvPanel.SetMaxSize
        // 2. View CellEx ItemHeight等属性变化时的重新加载 LvPanel.Reload
        // 3. 切换数据源时 LvPanel.OnRowsChanged
        /************************************************************************************************************************************/

        #region 成员变量
        public const double PanelMaxHeight = 5000;
        /// <summary>
        /// 分组行上部的间隔高度
        /// </summary>
        public const double GroupSeparatorHeight = 16;
        protected static Rect _rcEmpty = new Rect();

        protected Lv _owner;
        protected Func<LvItem, LvRow> _createLvRow;
        protected GroupHeader _groupHeader;

        /// <summary>
        /// 是否已生成虚拟行
        /// </summary>
        bool _initVirRow;

        /// <summary>
        /// 虚拟行时：能填充可视区域的UI行列表(可看作一页)，真实行时：与数据行一一对应的UI行列表，
        /// </summary>
        protected readonly List<LvRow> _dataRows = new List<LvRow>();

        /// <summary>
        /// 采用虚拟行时使用，_dataRows的所有行总高度，看作页面高度
        /// </summary>
        protected double _pageHeight;

        /// <summary>
        /// 虚拟行的行高
        /// </summary>
        protected double _rowHeight;

        /// <summary>
        /// 面板最大尺寸，宽高始终不为无穷大！
        /// </summary>
        protected Size _maxSize = Size.Empty;

        /// <summary>
        /// 以滚动栏为参照物，面板与滚动栏的水平距离，面板在右侧时为正数
        /// </summary>
        protected double _deltaX;

        /// <summary>
        /// 以滚动栏为参照物，面板与滚动栏的垂直距离，面板在下方时为正数
        /// </summary>
        protected double _deltaY;

        /// <summary>
        /// 工具栏高度
        /// </summary>
        protected double _toolbarHeight;

        /// <summary>
        /// 当前是否正在滚动中
        /// </summary>
        protected bool _isScrolling;
        #endregion

        #region 构造方法
        public LvPanel(Lv p_owner)
        {
            _owner = p_owner;
            Background = Res.TransparentBrush;

            // 为高效
            DefineCreateRowFunc();

            // 真实行时，初次生成所有行
            if (!_owner.IsVir)
                LoaRealRows();

            _owner.Scroll.ViewChanged += OnScrollViewChanged;
            SizeChanged += OnSizeChanged;

#if UWP
            // 屏蔽鼠标滚轮引起的抖动
            PointerWheelChanged += OnPointerWheelChanged;
            KeyDown += OnKeyDown;
#endif
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 设置Lv面板的最大尺寸，宽高始终不为无穷大！
        /// 在Lv.MeasureOverride时获取，已处理父元素为ScrollViewer StackPanel时造成的无穷大情况！
        /// </summary>
        internal void SetMaxSize(Size p_size)
        {
            if (_maxSize.Width != p_size.Width || _maxSize.Height != p_size.Height)
            {
                _maxSize = p_size;
                if (_owner.IsVir)
                    LoadVirRows();
            }
        }

        /// <summary>
        /// View CellEx ItemHeight ShowGroupHeader ShowItemBorder GroupTemplate SelectionMode 或上下文菜单 变化时重新加载
        /// </summary>
        internal void Reload()
        {
            DefineCreateRowFunc();
            ClearColHeader();

            if (_owner.IsVir)
                LoadVirRows();
            else
                LoaRealRows();
        }

        /// <summary>
        /// 切换数据源时，调整所有分组行和数据行
        /// </summary>
        internal void OnRowsChanged()
        {
            if (_owner.IsVir)
            {
                // 为避免布局错误统一重绘！
                LoadVirRows();

                //if (p_existGroup || !_initVirRow)
                //{
                //    // 含分组 或 第一次加载数据源时需重绘
                //    LoadVirRows();
                //}
                //else
                //{
                //    // 无分组时只需重新测量布局
                //    InvalidateMeasure();
                //}
            }
            else
            {
                LoaRealRows();
                // 确保数据变化后可立即访问行UI
                UpdateLayout();
            }
        }

        /// <summary>
        /// 批量插入数据行，无排序过滤分组时！
        /// </summary>
        /// <param name="p_index">开始插入位置</param>
        /// <param name="p_count">共插入行数</param>
        internal void OnInsertRows(int p_index, int p_count)
        {
            if (_owner.IsVir)
            {
                // 为避免布局错误统一重绘！
                LoadVirRows();

                //if (!_initVirRow)
                //{
                //    // 第一次加载数据源时需重绘
                //    LoadVirRows();
                //}
                //else
                //{
                //    // 只需重新测量布局
                //    InvalidateMeasure();
                //}
            }
            else
            {
                for (int i = 0; i < p_count; i++)
                {
                    int index = p_index + i;
                    var row = _createLvRow(_owner.Rows[index]);
                    Children.Insert(index, row);
                    _dataRows.Insert(index, row);
                }
                // 确保数据变化后可立即访问行UI
                UpdateLayout();
            }
        }

        /// <summary>
        /// 批量删除数据行，无排序过滤分组！
        /// </summary>
        /// <param name="p_items">所有删除项的索引列表，索引已按从小到大排序</param>
        internal void OnRemoveRows(IList p_items)
        {
            if (_owner.IsVir)
            {
                // 为避免布局错误统一重绘！
                LoadVirRows();
            }
            else
            {
                // 从后向前删除
                for (int i = p_items.Count - 1; i >= 0; i--)
                {
                    int index = (int)p_items[i];
                    Children.RemoveAt(index);
                    _dataRows.RemoveAt(index);
                }
                // 确保数据变化后可立即访问行UI
                UpdateLayout();
            }
        }

        /// <summary>
        /// 从可视树卸载，不可重复使用！ViewMode切换时卸载旧面板，其它无需卸载
        /// </summary>
        internal void Unload()
        {
            ClearAllRows();
            _owner.Scroll.ViewChanged -= OnScrollViewChanged;
            SizeChanged -= OnSizeChanged;

#if UWP
            PointerWheelChanged -= OnPointerWheelChanged;
            KeyDown -= OnKeyDown;
#endif
        }

        /// <summary>
        /// 获取行UI，不支持虚拟行的情况
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        internal FrameworkElement GetLvRow(int p_index)
        {
            if (p_index >= 0 && p_index < _dataRows.Count)
                return _dataRows[p_index];
            return null;
        }

        internal Size GetMaxSize()
        {
            return _maxSize;
        }

        /// <summary>
        /// 切换 SortDesc 属性时调整排序图标
        /// </summary>
        internal virtual void OnSortDescChanged()
        {
            SyncToolbarSortIcon();
        }
        #endregion

        #region 滚动到
        /// <summary>
        /// 滚动到指定的数据行
        /// </summary>
        /// <param name="p_index">-1 表示最后</param>
        internal async void ScrollInto(int p_index)
        {
            if (p_index < 0 || p_index > _owner.Rows.Count - 1)
            {
                // 最后
                p_index = _owner.Rows.Count - 1;
            }

            double offset;
            if (_owner.IsInnerScroll)
            {
                offset = (p_index == _owner.Rows.Count - 1) ? _owner.Scroll.ScrollableHeight : GetRowVerPos(p_index);
            }
            else
            {
                double rowHeight = GetRowVerPos(p_index);
                // 计算与滚动栏的相对位置
                var pt = TransformToVisual(_owner.Scroll).TransformPoint(new Point());
                offset = _owner.Scroll.VerticalOffset + pt.Y + rowHeight;
            }

            // 不使用Dispatcher初次加载数据后滚出无效！
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                new DispatchedHandler(() => _owner.Scroll.ChangeView(null, offset, null)));
        }

        /// <summary>
        /// 获取数据行的垂直位置，只适用于列表和表格模式
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        internal virtual double GetRowVerPos(int p_index)
        {
            double height = 0;
            if (_owner.IsVir)
            {
                // 虚拟行，等高
                height = p_index * _rowHeight;
                if (_owner.MapRows != null)
                {
                    // 等高有分组
                    int cnt = 0, iGrpRow = 0;
                    for (int i = 0; i < _owner.MapRows.Count; i++)
                    {
                        if (_owner.MapRows[i])
                            height += _owner.GroupRows[iGrpRow++].DesiredSize.Height;
                        else
                            cnt++;

                        if (cnt > p_index)
                            break;
                    }
                }
            }
            else
            {
                // 真实行
                if (_owner.MapRows == null)
                {
                    // 无分组
                    for (int i = 0; i < p_index; i++)
                    {
                        height += _dataRows[i].DesiredSize.Height;
                    }
                }
                else
                {
                    // 有分组
                    int iDataRow = 0, iGrpRow = 0;
                    for (int i = 0; i < _owner.MapRows.Count; i++)
                    {
                        if (_owner.MapRows[i])
                            height += _owner.GroupRows[iGrpRow++].DesiredSize.Height;
                        else
                            height += _dataRows[iDataRow++].DesiredSize.Height;

                        if (iDataRow >= p_index)
                            break;
                    }
                }
            }

            if (_groupHeader != null)
                height -= _groupHeader.DesiredSize.Height;
            return Math.Max(height, 0);
        }

        /// <summary>
        /// 设置为输入焦点
        /// </summary>
        internal virtual void ReceiveFocus()
        {
            if (Children.Count > 0 && Children[Children.Count - 1] is Control con)
                con.Focus(FocusState.Programmatic);
        }
        #endregion

        #region 测量布局
        /// <summary>
        /// 面板水平Stretch，因需要屏蔽鼠标滚轮引起的抖动，实际内容宽度_finalWidth
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // 未指定视图模板
            if (_owner.View == null)
                return new Size();

            //Log.Debug($"{_owner.BaseUri} LvPanel MeasureOverride");
            // 虚拟行/真实行
            Size size = _owner.IsVir ? MeasureVirRows() : MeasureRealRows();

            // 工具栏
            if (_toolbar != null)
            {
                _toolbar.Measure(_maxSize);
                _toolbarHeight = _toolbar.DesiredSize.Height;
                size = new Size(size.Width, size.Height + _toolbarHeight);
            }
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            //Log.Debug($"{_owner.BaseUri} LvPanel ArrangeOverride");
            if (!_owner.IsInnerScroll)
            {
                // 面板与ScrollViewer的相对距离，以滚动栏为参照物，面板在右下方时为正数
#if UWP
                if (_owner.Scroll.ActualHeight > 0)
                {
                    // 当切换win时，再次显示Scroll时ActualHeight为0，计算相对位置错误！采用切换前的相对位置
                    var pt = TransformToVisual(_owner.Scroll).TransformPoint(new Point());
                    _deltaX = pt.X;
                    _deltaY = pt.Y;
                }
#else
                // uno中面板与Scroll的相对距离始终为滚动栏未移动时之间的距离！
                var pt = TransformToVisual(_owner.Scroll).TransformPoint(new Point());
                _deltaX = pt.X - _owner.Scroll.HorizontalOffset;
                _deltaY = pt.Y - _owner.Scroll.VerticalOffset;
#endif
            }
            else
            {
                // 内置滚动栏时，垂直距离始终 <= 0
                _deltaX = -_owner.Scroll.HorizontalOffset;
                _deltaY = -_owner.Scroll.VerticalOffset;
            }

            if (_owner.IsVir)
            {
                // 虚拟行
                if (_owner.MapRows != null)
                    ArrangeGroupVirRows(finalSize);
                else
                    ArrangeVirRows(finalSize);
            }
            else
            {
                // 真实行
                if (_owner.MapRows != null)
                    ArrangeGroupRealRows(finalSize);
                else
                    ArrangeRealRows(finalSize);
            }

            _toolbar?.Arrange(new Rect(-_deltaX, _deltaY > 0 ? 0 : -_deltaY, finalSize.Width, _toolbarHeight));
            return finalSize;
        }

        protected abstract Size MeasureVirRows();

        /*********************************************************************************************************/
        // 虚拟行布局
        // 1. 整个面板不在滚动栏可视区(_deltaY >= _maxSize.Height 或 _deltaY <= -p_finalSize.Height）时，布局到空区域
        // 2. 面板可见且在滚动栏下方(0 <= _deltaY < _maxSize.Height)时，按虚拟行顺序布局，下方超出的行布局到空区域
        // 3. 面板顶部超出滚动栏 并且 没有整个面板都超出(-p_finalSize.Height < _deltaY < 0)时，按分页算法布局所有虚拟行
        /*********************************************************************************************************/

        protected abstract void ArrangeVirRows(Size p_finalSize);

        protected abstract void ArrangeGroupVirRows(Size p_finalSize);

        protected abstract Size MeasureRealRows();

        protected abstract void ArrangeRealRows(Size p_finalSize);

        protected abstract void ArrangeGroupRealRows(Size p_finalSize);
        #endregion

        #region 增删元素
        /// <summary>
        /// 加载虚拟模式的所有行：虚拟数据行、分组行、列头
        /// </summary>
        void LoadVirRows()
        {
            ClearAllRows();

            // 按顺序添加，后面的元素部署在上层！
            _initVirRow = CreateVirRows();
            LoadGroupRows();
            LoadColHeader();
            LoadToolbar();

            //if (_initVirRow)
            //    Log.Debug($"{_owner.BaseUri} 生成{_dataRows.Count}个虚拟行");
        }

        /// <summary>
        /// 加载真实模式的所有行：数据行、分组行、列头
        /// </summary>
        void LoaRealRows()
        {
            ClearAllRows();

            // 添加元素顺序：数据行、分组行、列头，后面的元素部署在上层！
            // 数据行
            if (_owner.Rows.Count > 0)
            {
                foreach (var row in _owner.Rows)
                {
                    var elem = _createLvRow(row);
                    Children.Add(elem);
                    _dataRows.Add(elem);
                }
            }

            LoadGroupRows();
            LoadColHeader();
            LoadToolbar();
        }

        /// <summary>
        /// 清除所有UI行
        /// </summary>
        void ClearAllRows()
        {
            if (Children.Count > 0)
                Children.Clear();
            _dataRows.Clear();
            RemoveToolbar();
        }

        /// <summary>
        /// 生成分组行及其他
        /// </summary>
        void LoadGroupRows()
        {
            // 分组行
            _groupHeader = null;
            if (_owner.GroupRows != null)
            {
                foreach (var grp in _owner.GroupRows)
                {
                    Children.Add(grp);
                }
                // 分组导航头
                if (_owner.ShowGroupHeader)
                {
                    _groupHeader = new GroupHeader(_owner);
                    Children.Add(_groupHeader);
                }
            }
        }

        /// <summary>
        /// 生成虚拟行
        /// </summary>
        protected abstract bool CreateVirRows();

        /// <summary>
        /// 加载表格的列头
        /// </summary>
        protected virtual void LoadColHeader()
        { }

        /// <summary>
        /// 清除表格的列头
        /// </summary>
        protected virtual void ClearColHeader()
        { }
        #endregion

        #region 创建行界面
        /// <summary>
        /// 确定创建界面行的方法，高效，省去循环中的冗余判断
        /// </summary>
        void DefineCreateRowFunc()
        {
            var mode = _owner.CurrentViewMode;
            if (_owner.View is Cols cols)
            {
                // 由列定义生成
                if (mode == ViewMode.Table)
                    _createLvRow = CreateTableRow;
                else if (mode == ViewMode.List)
                    _createLvRow = CreateListFormRow;
                else
                    _createLvRow = CreateTileFormItem;
            }
            else if (_owner.View is DataTemplate)
            {
                // 由模板生成
                if (mode == ViewMode.List)
                    _createLvRow = CreateListRowByTemplate;
                else
                    _createLvRow = CreateTileItemByTemplate;
            }
            else if (_owner.View is DataTemplateSelector)
            {
                // 由模板选择器生成
                if (mode == ViewMode.List)
                    _createLvRow = CreateListRowBySelector;
                else
                    _createLvRow = CreateTileItemBySelector;
            }
            else if (_owner.View is IRowView)
            {
                // 动态创建行UI
                if (mode == ViewMode.List)
                    _createLvRow = CreateListRowByRowView;
                else
                    _createLvRow = CreateTileItemByRowView;
            }
        }

        /// <summary>
        /// 表格行
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        LvRow CreateTableRow(LvItem p_item)
        {
            var row = new TableRow(_owner);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 表单列表的行
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        LvRow CreateListFormRow(LvItem p_item)
        {
            var row = new ListFormRow(_owner);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 磁贴的项
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        LvRow CreateTileFormItem(LvItem p_item)
        {
            var row = new TileFormRow(_owner);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 由模板生成行内容
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        LvRow CreateListRowByTemplate(LvItem p_item)
        {
            var row = new ListRow(_owner, (DataTemplate)_owner.View);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 由模板生成磁贴项内容
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        LvRow CreateTileItemByTemplate(LvItem p_item)
        {
            var row = new TileRow(_owner, (DataTemplate)_owner.View);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 由模板选择器生成行内容
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        LvRow CreateListRowBySelector(LvItem p_item)
        {
            var temp = ((DataTemplateSelector)_owner.View).SelectTemplate(p_item);
            if (temp == null)
                throw new Exception("未指定行模板！");

            var row = new ListRow(_owner, temp);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 由模板选择器生成磁贴项内容
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        LvRow CreateTileItemBySelector(LvItem p_item)
        {
            var temp = ((DataTemplateSelector)_owner.View).SelectTemplate(p_item);
            if (temp == null)
                throw new Exception("未指定行模板！");

            var row = new TileRow(_owner, temp);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 动态创建行内容
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        LvRow CreateListRowByRowView(LvItem p_item)
        {
            var row = new ListRow(_owner, (IRowView)_owner.View, p_item);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 动态创建磁贴项内容
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        LvRow CreateTileItemByRowView(LvItem p_item)
        {
            var row = new TileRow(_owner, (IRowView)_owner.View, p_item);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }
        #endregion

        #region Toolbar
        Menu _toolbar;

        void LoadToolbar()
        {
            _toolbar = _owner.Toolbar;
            if (_toolbar != null)
            {
                _toolbar.Background = Res.浅灰1;
                _toolbar.BorderBrush = Res.浅灰2;
                _toolbar.BorderThickness = new Thickness(0, 0, 0, 1);
                Children.Add(_toolbar);
                _toolbar.ItemClick += OnToolbarClick;
            }
        }

        void RemoveToolbar()
        {
            if (_toolbar != null)
            {
                _toolbar.ItemClick -= OnToolbarClick;
                _toolbar = null;
            }
            _toolbarHeight = 0;
        }

        void OnToolbarClick(object sender, Mi e)
        {
            string str = e.CmdParam as string;
            if (string.IsNullOrEmpty(str))
                return;

            if (str.Equals("#Filter", StringComparison.OrdinalIgnoreCase))
            {
                _owner.ShowFilterDlg();
                return;
            }

            var cols = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var old = _owner.SortDesc;
            var sort = new SortDescription { ID = cols[0] };
            if (old != null && old.ID == cols[0])
            {
                sort.Direction = old.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else if (cols.Length == 2)
            {
                sort.Direction = "desc".Equals(cols[1], StringComparison.OrdinalIgnoreCase) ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            _owner.SortDesc = sort;
        }

        void SyncToolbarSortIcon()
        {
            if (_toolbar == null)
                return;

            var sort = _owner.SortDesc;
            foreach (var mi in _toolbar.Items)
            {
                string str = mi.CmdParam as string;
                if (string.IsNullOrEmpty(str) || str.Equals("#Filter", StringComparison.OrdinalIgnoreCase))
                    continue;

                var cols = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (cols[0].Equals(sort.ID, StringComparison.OrdinalIgnoreCase))
                {
                    mi.ShowInPhone = VisibleInPhone.IconAndID;
                    mi.Icon = (sort.Direction == ListSortDirection.Ascending) ? Icons.向上 : Icons.向下;
                }
                else
                {
                    mi.Icon = Icons.None;
                }
            }
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 表格：始终刷新布局，已重写方法
        /// 列表：虚拟行始终刷新布局，真实行当有分组或工具栏时只在开始、结束时刷新，其他情况不刷新
        /// 磁贴：同列表
        /// 本方法适用于列表和磁贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnScrollViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_owner.IsVir)
            {
                // 虚拟行始终刷新布局
                _isScrolling = e.IsIntermediate;
                InvalidateArrange();
            }
            else if (_owner.MapRows != null || _toolbar != null)
            {
                // 开始滚动或结束滚动时刷新布局，为了隐藏/显示分组导航头
                if (!e.IsIntermediate)
                {
                    _isScrolling = false;
                    InvalidateArrange();
                }
                else if (!_isScrolling)
                {
                    _isScrolling = true;
                    InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// 面板大小变化时处理 自动滚到底部、分页插入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var pd = _owner.PageData;
            if (pd == null || pd.State != PageDataState.Loading)
            {
                // 未使用分页 或 未加载普通页数据时，自动滚动到底部才有效
                if (_owner.AutoScrollBottom)
                {
                    await Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        new DispatchedHandler(() => _owner.Scroll.ChangeView(null, _owner.Scroll.ScrollableHeight, null)));
                }
            }
            else if (pd.State == PageDataState.Loading && pd.InsertTop)
            {
                // 插入顶部时，滚动到当前行的位置
                await Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    new DispatchedHandler(() =>
                    {
                        double height = GetRowVerPos(pd.LastPageCount);
                        _owner.Scroll.ChangeView(null, height, null, true);
                    }));
            }

            // 避免首页加载时先重置状态造成无后续页！
            if (pd != null && _owner.Data != null)
                pd.State = PageDataState.Normal;
        }

        /// <summary>
        /// 屏蔽鼠标滚轮引起的抖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            // 屏蔽ScrollViewer的滚动
            e.Handled = true;
            int delta = e.GetCurrentPoint(null).Properties.MouseWheelDelta;
            // 不启用动画！
            _owner.Scroll.ChangeView(null, _owner.Scroll.VerticalOffset - delta, null, true);
        }

        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            int index;
            switch (e.Key)
            {
                case VirtualKey.Down:
                    index = _owner.SelectedIndex;
                    if (index == -1)
                        index = 0;
                    else
                        index = (index == _owner.Rows.Count - 1) ? 0 : index + 1;
                    _owner.SelectedIndex = index;
                    ScrollInto(index);
                    e.Handled = true;
                    return;

                case VirtualKey.Up:
                    index = _owner.SelectedIndex;
                    if (index == -1)
                        index = 0;
                    else
                        index = (index == 0) ? _owner.Rows.Count - 1 : index - 1;
                    _owner.SelectedIndex = index;
                    ScrollInto(index);
                    e.Handled = true;
                    return;

                case VirtualKey.Enter:
                    object data = _owner.SelectedItem;
                    if (data != null)
                        _owner.OnItemClick(data, data);
                    return;

                case VirtualKey.End:
                    index = _owner.Rows.Count - 1;
                    _owner.SelectedIndex = index;
                    ScrollInto(index);
                    e.Handled = true;
                    return;

                case VirtualKey.Home:
                    _owner.SelectedIndex = 0;
                    ScrollInto(0);
                    e.Handled = true;
                    return;

                case VirtualKey.C:
                    // 复制

                    return;

                case VirtualKey.A:
                    if (InputManager.IsCtrlPressed && _owner.SelectionMode == SelectionMode.Multiple)
                    {
                        // 全选
                        _owner.SelectAll();
                        e.Handled = true;
                    }
                    return;

                default:
                    return;
            }
        }
        #endregion
    }
}