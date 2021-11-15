#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 列头单元格
    /// </summary>
    public partial class ColHeaderCell : Control
    {
        #region 静态内容
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(ColHeaderCell),
            new PropertyMetadata(null));

        public readonly static DependencyProperty SortStateProperty = DependencyProperty.Register(
            "SortState",
            typeof(string),
            typeof(ColHeaderCell),
            new PropertyMetadata(null));
        #endregion

        #region 成员变量
        // 调整列宽行高的有效边距
        const double _resizePadding = 6.0;
        ColHeader _owner;
        uint? _pointerID;
        bool _isDragging;
        Col _resizingCol;
        Col _dragTgtCol;
        Point _ptLast;
        #endregion

        #region 构造方法
        public ColHeaderCell(Col p_col, ColHeader p_owner)
        {
            DefaultStyleKey = typeof(ColHeaderCell);
            Col = p_col;
            _owner = p_owner;
            Title = p_col.Title;
            Loaded += OnLoaded;
        }
        #endregion

        /// <summary>
        /// 获取设置标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置排序标志
        /// </summary>
        public string SortState
        {
            get { return (string)GetValue(SortStateProperty); }
            set { SetValue(SortStateProperty, value); }
        }

        internal Col Col { get; }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            // uno中重写事件方法无效！iOS暂不支持Tapped事件！
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;
            PointerCaptureLost += OnPointerCaptureLost;
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!CapturePointer(e.Pointer))
                return;

            e.Handled = true;
            if (e.IsTouch())
            {
                // 触摸模式只支持列排序
                _pointerID = e.Pointer.PointerId;
                return;
            }

            // 鼠标模式
            Point pt = e.GetCurrentPoint(this).Position;
            if (pt.X < _resizePadding)
            {
                // 左侧调列宽
                _resizingCol = GetLeftCol();
                if (_resizingCol == null)
                {
                    _isDragging = true;
                    SetCursor(CoreCursorType.Arrow);
                    VisualStateManager.GoToState(this, "Pressed", true);
                }
                else
                {
                    _ptLast = e.GetCurrentPoint(null).Position;
                    VisualStateManager.GoToState(this, "Normal", true);
                }
            }
            else if (pt.X > Col.Width - _resizePadding)
            {
                // 右侧调列宽
                _resizingCol = Col;
                _ptLast = e.GetCurrentPoint(null).Position;
                VisualStateManager.GoToState(this, "Normal", true);
            }
            else
            {
                // 开始拖拽
                _isDragging = true;
                VisualStateManager.GoToState(this, "Pressed", true);
            }
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.IsTouch())
            {
                // 触摸模式只支持列排序
                if (_pointerID == e.Pointer.PointerId)
                {
                    ReleasePointerCapture(e.Pointer);
                    _pointerID = null;
                }
                return;
            }

            // 鼠标未按下时的滑过
            if (!_isDragging && _resizingCol == null)
            {
                Point pt = e.GetCurrentPoint(this).Position;
                if (pt.X >= _resizePadding && Col.Width - pt.X >= _resizePadding)
                {
                    SetCursor(CoreCursorType.Arrow);
                    VisualStateManager.GoToState(this, "PointerOver", true);
                }
                else
                {
                    SetCursor(CoreCursorType.SizeWestEast);
                    VisualStateManager.GoToState(this, "Normal", true);
                }
                return;
            }

            // 调整列宽
            if (_resizingCol != null)
            {
                Point cur = e.GetCurrentPoint(null).Position;
                double width = _resizingCol.Width + cur.X - _ptLast.X;
                if (width > 35)
                {
                    // 最小宽度能显示一个字
                    _resizingCol.Width = width;
                    _owner.Lv.Cols.Invalidate();
                    _ptLast = cur;
                }
                return;
            }

            // 拖拽列
            _dragTgtCol = _owner.GetDragTargetCol(Col, e.GetCurrentPoint(_owner).Position.X);
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.IsTouch())
            {
                // 触摸模式只支持列排序
                if (_pointerID == e.Pointer.PointerId)
                {
                    ReleasePointerCapture(e.Pointer);
                    _pointerID = null;
                    ChangedSortState();
                }
                return;
            }

            if (_isDragging)
            {
                if (_dragTgtCol != null)
                {
                    // 拖拽结束
                    _owner.FinishedDrag();
                    Cols cols = _owner.Lv.Cols;
                    int index = cols.IndexOf(_dragTgtCol);
                    cols.Remove(Col);
                    cols.Insert(index, Col);
                    cols.Invalidate();
                }
                else
                {
                    // 列排序
                    ChangedSortState();
                }
            }
            ResetMouseState();
        }

        void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.IsMouse() && !_isDragging && _resizingCol == null)
            {
                Point pt = e.GetCurrentPoint(this).Position;
                if (pt.X >= _resizePadding && Col.Width - pt.X >= _resizePadding)
                {
                    SetCursor(CoreCursorType.Arrow);
                    VisualStateManager.GoToState(this, "PointerOver", true);
                }
                else
                {
                    SetCursor(CoreCursorType.SizeWestEast);
                    VisualStateManager.GoToState(this, "Normal", true);
                }
            }
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.IsMouse() && !_isDragging && _resizingCol == null)
            {
                SetCursor(CoreCursorType.Arrow);
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (e.IsMouse())
                ResetMouseState();
        }

        void ChangedSortState()
        {
            // 允许排序
            if (!_owner.Lv.Cols.AllowSorting || !Col.AllowSorting)
                return;

            ListSortDirection dir = ListSortDirection.Ascending;
            SortDescription desc = _owner.Lv.SortDesc;
            if (desc != null
                && !string.IsNullOrEmpty(desc.ID)
                && desc.ID.Equals(Col.ID, StringComparison.OrdinalIgnoreCase)
                && desc.Direction == ListSortDirection.Ascending)
            {
                dir = ListSortDirection.Descending;
            }
            _owner.Lv.SortDesc = new SortDescription(Col.ID, dir);
        }

        internal void SyncSortIcon()
        {
            SortDescription desc = _owner.Lv.SortDesc;
            if (desc != null
                && !string.IsNullOrEmpty(desc.ID)
                && desc.ID.Equals(Col.ID, StringComparison.OrdinalIgnoreCase))
            {
                SortState = (desc.Direction == ListSortDirection.Ascending) ? "\uE017" : "\uE018";
            }
            else
            {
                ClearValue(SortStateProperty);
            }
        }

        Col GetLeftCol()
        {
            int index = 0;
            Cols cols = _owner.Lv.Cols;
            for (int i = 0; i < cols.Count; i++)
            {
                if (Col == cols[i])
                {
                    index = i;
                    break;
                }
            }

            if (index == 0)
                return null;
            return cols[index - 1];
        }

        void ResetMouseState()
        {
            if (_isDragging || _resizingCol != null)
            {
                ReleasePointerCaptures();
                _isDragging = false;
                _dragTgtCol = null;
                _resizingCol = null;
            }
            SetCursor(CoreCursorType.Arrow);
            VisualStateManager.GoToState(this, "Normal", true);
        }

        void SetCursor(CoreCursorType p_cursor)
        {
            CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(p_cursor, 0);
        }
    }
}
