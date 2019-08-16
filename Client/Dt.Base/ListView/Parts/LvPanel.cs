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
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
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
        /*********************************************************************************************************/
        // MeasureOverride中尽可能不增删Children元素，uno中每增删一个元素会重复一次MeasureOverride，严重时死循环！！！
        // UWP和uno的调用顺序不同！
        // UWP：MeasureOverride > _owner.SizeChanged > SizeChanged > Loaded
        // uno：Loaded > MeasureOverride > SizeChanged > _owner.SizeChanged
        /*********************************************************************************************************/

        #region 成员变量
        protected const double PanelMaxHeight = 5000;
        protected static Rect _rcEmpty = new Rect();

        protected Lv _owner;
        protected Func<LvItem, FrameworkElement> _createLvRow;
        protected readonly List<FrameworkElement> _dataRows = new List<FrameworkElement>();
        protected bool _initVirRow;
        protected double _rowHeight;
        protected double _pageHeight;
        protected GroupHeader _groupHeader;
        #endregion

        #region 构造方法
        public LvPanel(Lv p_owner)
        {
            _owner = p_owner;
            Background = AtRes.TransparentBrush;

            // 为高效
            DefineCreateRowFunc();

            // Children中的元素顺序：数据行、分组行、列头，后面的元素部署在上层！
            // 虚拟行时，初次只生成分组和列头，第一次测量时才添加虚拟行
            // 真实行时，初次生成所有行
            if (_owner.IsVir)
                LoadGroupRows();
            else
                LoaAllRows();

            _owner.Scroll.ViewChanged += OnScrollViewChanged;
            if (AtSys.System == TargetSystem.Windows)
            {
                // 屏蔽鼠标滚轮引起的抖动
                PointerWheelChanged += OnPointerWheelChanged;
                // 处理大小变化，uno上Loaded后触发！
                _owner.SizeChanged += OnSizeChanged;
                KeyDown += OnKeyDown;
            }
        }
        #endregion

        /// <summary>
        /// 通过Lv获取有效高度，因在ScrollViewer内！
        /// </summary>
        internal Size AvailableSize { get; set; }

        /// <summary>
        /// 切换模板、调整自动行高时重新加载
        /// </summary>
        internal void Reload()
        {
            // 清空旧元素
            Children.Clear();
            _dataRows.Clear();
            ClearOtherRows();
            _initVirRow = false;

            DefineCreateRowFunc();
            if (_owner.IsVir)
                LoadGroupRows();
            else
                LoaAllRows();
        }

        /// <summary>
        /// ViewMode切换时卸载所有元素
        /// </summary>
        internal void Unload()
        {
            Children.Clear();
            _dataRows.Clear();
            _owner.Scroll.ViewChanged -= OnScrollViewChanged;
            if (AtSys.System == TargetSystem.Windows)
            {
                PointerWheelChanged -= OnPointerWheelChanged;
                _owner.SizeChanged -= OnSizeChanged;
                KeyDown -= OnKeyDown;
            }
        }

        /// <summary>
        /// 切换数据源时，调整所有分组行和数据行
        /// </summary>
        /// <param name="p_existGroup">是否有分组行</param>
        internal void OnRowsChanged(bool p_existGroup)
        {
            if (_owner.IsVir)
            {
                // 含分组时需清除
                if (p_existGroup)
                    ClearVirRows();
                InvalidateMeasure();
            }
            else
            {
                Children.Clear();
                _dataRows.Clear();
                LoaAllRows();
            }
        }

        /// <summary>
        /// 批量插入数据行
        /// </summary>
        /// <param name="p_index"></param>
        /// <param name="p_count"></param>
        internal void OnInsertRows(int p_index, int p_count)
        {
            if (_owner.IsVir)
            {
                InvalidateMeasure();
                return;
            }

            for (int i = 0; i < p_count; i++)
            {
                int index = p_index + i;
                FrameworkElement row = _createLvRow(_owner.Rows[index]);
                Children.Insert(index, row);
                _dataRows.Insert(index, row);
            }
        }

        /// <summary>
        /// 滚动到指定的数据行
        /// </summary>
        /// <param name="p_index">-1 表示最后</param>
        internal void ScrollInto(int p_index)
        {
            // 最后
            if (p_index < 0 || p_index >= _owner.Rows.Count - 1)
            {
                _owner.Scroll.ChangeView(null, _owner.Scroll.ScrollableHeight, null);
                return;
            }

            double height = GetRowVerPos(p_index);
            double offset = Math.Max(height - _owner.Scroll.ViewportHeight, 0);
            _owner.Scroll.ChangeView(null, offset, null);
        }

        /// <summary>
        /// 获取数据行的垂直位置
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        internal double GetRowVerPos(int p_index)
        {
            double height = 0;
            if (_owner.IsVir)
            {
                // 虚拟行，等高
                height += (p_index + 1) * _rowHeight;
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
                    for (int i = 0; i <= p_index; i++)
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

                        if (iDataRow > p_index)
                            break;
                    }
                }
            }
            return height;
        }

        /// <summary>
        /// 设置为输入焦点
        /// </summary>
        internal virtual void ReceiveFocus()
        {
            if (Children.Count > 0 && Children[Children.Count - 1] is Control con)
                con.Focus(FocusState.Programmatic);
        }

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

            double maxWidth = double.IsInfinity(AvailableSize.Width) ? AtUI.ViewWidth : AvailableSize.Width;
            double maxHeight = double.IsInfinity(AvailableSize.Height) ? AtUI.ViewHeight : AvailableSize.Height;
            // 虚拟行/真实行
            return _owner.IsVir ? MeasureVirRows(maxWidth, maxHeight) : MeasureRealRows(maxWidth, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

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
            return finalSize;
        }

        protected abstract Size MeasureVirRows(double p_maxWidth, double p_maxHeight);

        protected abstract void ArrangeVirRows(Size p_finalSize);

        protected abstract void ArrangeGroupVirRows(Size p_finalSize);

        protected abstract Size MeasureRealRows(double p_maxWidth, double p_maxHeight);

        protected abstract void ArrangeRealRows(Size p_finalSize);

        protected abstract void ArrangeGroupRealRows(Size p_finalSize);
        #endregion

        #region 增删元素
        /// <summary>
        /// 清除所有行，重新加入列头、分组行
        /// </summary>
        void ClearVirRows()
        {
            Children.Clear();
            _dataRows.Clear();
            LoadGroupRows();
            _initVirRow = false;
        }

        /// <summary>
        /// 自动行高时生成所有行
        /// </summary>
        void LoaAllRows()
        {
            // 添加元素顺序：数据行、分组行、列头，后面的元素部署在上层！
            // 数据行
            if (_owner.Rows.Count > 0)
            {
                FrameworkElement elem;
                foreach (var row in _owner.Rows)
                {
                    elem = _createLvRow(row);
                    Children.Add(elem);
                    _dataRows.Add(elem);
                }
            }

            // 列头、分组行
            LoadGroupRows();
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
            LoadOtherRows();
        }

        /// <summary>
        /// 生成其他行，如列头
        /// </summary>
        protected virtual void LoadOtherRows()
        { }

        /// <summary>
        /// 清除其他行，如列头
        /// </summary>
        protected virtual void ClearOtherRows()
        { }
        #endregion

        #region 创建行界面
        /// <summary>
        /// 确定创建界面行的方法，高效，省去循环中的冗余判断
        /// </summary>
        void DefineCreateRowFunc()
        {
            if (_owner.View is Cols cols)
            {
                // 由列定义生成
                if (_owner.ViewMode == ViewMode.Table)
                    _createLvRow = CreateTableRow;
                else if (_owner.ViewMode == ViewMode.List)
                    _createLvRow = CreateListFormRow;
                else
                    _createLvRow = CreateTileFormItem;
            }
            else if (_owner.View is DataTemplate)
            {
                // 由模板生成
                if (_owner.ViewMode == ViewMode.List)
                    _createLvRow = CreateListRowByTemplate;
                else
                    _createLvRow = CreateTileItemByTemplate;
            }
            else if (_owner.View is DataTemplateSelector)
            {
                // 由模板选择器生成
                if (_owner.ViewMode == ViewMode.List)
                    _createLvRow = CreateListRowBySelector;
                else
                    _createLvRow = CreateTileItemBySelector;
            }
        }

        /// <summary>
        /// 表格行
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        FrameworkElement CreateTableRow(LvItem p_item)
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
        FrameworkElement CreateListFormRow(LvItem p_item)
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
        FrameworkElement CreateTileFormItem(LvItem p_item)
        {
            var row = new TileFormItem(_owner);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 由模板生成行内容
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        FrameworkElement CreateListRowByTemplate(LvItem p_item)
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
        FrameworkElement CreateTileItemByTemplate(LvItem p_item)
        {
            var row = new TileItem(_owner, (DataTemplate)_owner.View);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }

        /// <summary>
        /// 由模板选择器生成行内容
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        FrameworkElement CreateListRowBySelector(LvItem p_item)
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
        FrameworkElement CreateTileItemBySelector(LvItem p_item)
        {
            var temp = ((DataTemplateSelector)_owner.View).SelectTemplate(p_item);
            if (temp == null)
                throw new Exception("未指定行模板！");

            var row = new TileItem(_owner, temp);
            if (p_item != null)
                row.SetViewRow(p_item, false);
            return row;
        }
        #endregion

        #region 事件处理
        async void OnScrollViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var page = _owner.PageData;
            if (page != null)
            {
                if (page.InsertTop && _owner.Scroll.VerticalOffset == 0)
                {
                    // 插入顶部
                    _owner.Scroll.ViewChanged -= OnScrollViewChanged;
                    await page.GotoNextPage();
                    double height = GetRowVerPos(page.PageSize);
                    _owner.Scroll.ChangeView(null, height, null, true);
                    _owner.Scroll.ViewChanged += OnScrollViewChanged;
                }
                else if (!page.InsertTop && _owner.Scroll.VerticalOffset == _owner.Scroll.ScrollableHeight)
                {
                    // 插入底部
                    await page.GotoNextPage();
                }
            }
            InvalidateArrange();
        }

        /// <summary>
        /// 面板大小变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 过滤初次加载或从可视树卸载后再加载的情况，造成切换任务栏时非常慢！！！
            // 频繁调整大小时会异常！！！修改Dlg调整大小的方式
            if (e.PreviousSize.Width > 0 && e.PreviousSize.Height > 0)
            {
                if (_owner.IsVir)
                    ClearVirRows();
                else
                    InvalidateArrange();
            }
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