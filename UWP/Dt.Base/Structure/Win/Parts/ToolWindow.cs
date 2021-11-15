#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 在Win中浮动的窗口容器
    /// </summary>
    public partial class ToolWindow : ContentControl
    {
        #region 静态成员
        /// <summary>
        /// 水平偏移量
        /// </summary>
        public readonly static DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(
            "HorizontalOffset",
            typeof(double),
            typeof(ToolWindow),
            new PropertyMetadata(0.0, OnHorizontalOffsetChanged));

        /// <summary>
        /// 垂直偏移量
        /// </summary>
        public readonly static DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
            "VerticalOffset",
            typeof(double),
            typeof(ToolWindow),
            new PropertyMetadata(0.0, OnVerticalOffsetChanged));

        static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolWindow window = d as ToolWindow;
            Canvas.SetLeft(window, window.HorizontalOffset);
        }

        static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolWindow window = d as ToolWindow;
            Canvas.SetTop(window, window.VerticalOffset);
        }
        #endregion

        #region 成员变量
        Win _owner;
        Canvas _panel;
        UIElement _headerElement;
        UIElement _rootGrid;
        bool _isHeadPressed;
        Point _startPoint;
        bool _isResizing;
        ResizeDirection _resizeDirection;
        Rect _initRect;
        #endregion

        #region 构造方法
        public ToolWindow(Win p_win, Canvas p_panel)
        {
            DefaultStyleKey = typeof(ToolWindow);
            _owner = p_win;
            _panel = p_panel;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置水平偏移量
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// 获取设置垂直偏移量
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// 获取窗口内的所有项是否可停靠在中部区域
        /// </summary>
        internal bool CanDockInCenter
        {
            get
            {
                Pane dockItem = Content as Pane;
                if (dockItem == null)
                    return false;

                return dockItem.GetAllTabItems().All((item) => item.CanDockInCenter);
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 直接设置标题为按下状态
        /// </summary>
        /// <param name="e"></param>
        internal void StartDrag(PointerRoutedEventArgs e)
        {
            OnheaderElementPointerPressed(this, e);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        internal void Show()
        {
            if (!_panel.Children.Contains(this))
                _panel.Children.Add(this);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        internal void Close()
        {
            if (_panel.Children.Contains(this))
                _panel.Children.Remove(this);
        }
        #endregion

        #region 重写
        /// <summary>
        /// 应用模板
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_headerElement != null)
            {
                _headerElement.PointerPressed -= OnheaderElementPointerPressed;
            }
            _headerElement = (UIElement)base.GetTemplateChild("HeaderElement");
            _headerElement.PointerPressed += OnheaderElementPointerPressed;

            if (_rootGrid != null)
            {
                _rootGrid.PointerPressed -= OnRootGridPointerPressed;
                _rootGrid.PointerMoved -= OnRootGridPointerMoved;
                _rootGrid.PointerReleased -= OnRootGridPointerReleased;
                _rootGrid.PointerEntered -= OnRootGridPointerEntered;
            }
            _rootGrid = (UIElement)base.GetTemplateChild("RootGrid");
            _rootGrid.PointerPressed += OnRootGridPointerPressed;
            _rootGrid.PointerMoved += OnRootGridPointerMoved;
            _rootGrid.PointerReleased += OnRootGridPointerReleased;
            _rootGrid.PointerEntered += OnRootGridPointerEntered;
        }

        /// <summary>
        /// 窗口内容变化
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            Pane oldItem = oldContent as Pane;
            Pane newItem = newContent as Pane;
            if (oldItem != null)
                oldItem.IsInWindow = false;

            if (newItem != null)
                newItem.IsInWindow = true;

            if (newContent == null)
                Close();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            BringToFront(this);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            BringToFront(this);
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_isHeadPressed)
            {
                PointerPoint position = e.GetCurrentPoint(_panel);
                HorizontalOffset = position.Position.X - _startPoint.X;
                VerticalOffset = position.Position.Y - _startPoint.Y;
                _owner.OnDragDelta(this, e);
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_isHeadPressed)
            {
                _isHeadPressed = false;
                ReleasePointerCapture(e.Pointer);
            }
            _owner.OnDragCompleted(this);
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 点击标题栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnheaderElementPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Point pt1 = e.GetCurrentPoint(_panel).Position;
            if (GetResizeDirection(pt1) == ResizeDirection.None && CapturePointer(e.Pointer))
            {
                _isHeadPressed = true;
                _startPoint = new Point(pt1.X - HorizontalOffset, pt1.Y - VerticalOffset);
            }
        }

        /// <summary>
        /// 点击RootGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRootGridPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Point mousePosition = e.GetCurrentPoint(_panel).Position;
            if (GetResizeDirection(mousePosition) != ResizeDirection.None && _rootGrid.CapturePointer(e.Pointer))
            {
                _isHeadPressed = false;
                Point offset = TransformToVisual(_panel).TransformPoint(new Point(0.0, 0.0));
                _isResizing = true;
                _resizeDirection = GetResizeDirection(mousePosition);
                _startPoint = mousePosition;
                _initRect = new Rect(offset.X, offset.Y, ActualWidth, ActualHeight);
                UpdateMouseCursor(_resizeDirection);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 移动RootGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRootGridPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isResizing)
            {
                UpdateMouseCursor(GetResizeDirection(e.GetCurrentPoint(_panel).Position));
            }
            else
            {
                double newWidth;
                double newHeight;
                double minWidth = double.IsNaN(MinWidth) ? 0.0 : MinWidth;
                double minHeight = double.IsNaN(MinHeight) ? 0.0 : MinHeight;
                double maxWidth = double.IsNaN(MaxWidth) ? double.PositiveInfinity : MaxWidth;
                double maxHeight = double.IsNaN(MaxHeight) ? double.PositiveInfinity : MaxHeight;
                e.Handled = true;
                switch (_resizeDirection)
                {
                    case ResizeDirection.Left:
                        newWidth = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + _startPoint.X) - e.GetCurrentPoint(_panel).Position.X));
                        Width = newWidth;
                        HorizontalOffset = _initRect.Right - newWidth;
                        return;

                    case ResizeDirection.TopLeft:
                        newWidth = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + _startPoint.X) - e.GetCurrentPoint(_panel).Position.X));
                        Width = newWidth;
                        HorizontalOffset = _initRect.Right - newWidth;
                        newHeight = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + _startPoint.Y) - e.GetCurrentPoint(_panel).Position.Y));
                        Height = newHeight;
                        VerticalOffset = _initRect.Bottom - newHeight;
                        return;

                    case ResizeDirection.Top:
                        newHeight = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + _startPoint.Y) - e.GetCurrentPoint(_panel).Position.Y));
                        Height = newHeight;
                        VerticalOffset = _initRect.Bottom - newHeight;
                        return;

                    case ResizeDirection.TopRight:
                        newHeight = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + _startPoint.Y) - e.GetCurrentPoint(_panel).Position.Y));
                        Height = newHeight;
                        VerticalOffset = _initRect.Bottom - newHeight;
                        Width = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + e.GetCurrentPoint(_panel).Position.X) - _startPoint.X));
                        return;

                    case ResizeDirection.Right:
                        Width = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + e.GetCurrentPoint(_panel).Position.X) - _startPoint.X));
                        return;

                    case ResizeDirection.BottomRight:
                        Height = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + e.GetCurrentPoint(_panel).Position.Y) - _startPoint.Y));
                        Width = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + e.GetCurrentPoint(_panel).Position.X) - _startPoint.X));
                        return;

                    case ResizeDirection.Bottom:
                        Height = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + e.GetCurrentPoint(_panel).Position.Y) - _startPoint.Y));
                        return;

                    case ResizeDirection.BottomLeft:
                        newWidth = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + _startPoint.X) - e.GetCurrentPoint(_panel).Position.X));
                        Width = newWidth;
                        HorizontalOffset = _initRect.Right - newWidth;
                        Height = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + e.GetCurrentPoint(_panel).Position.Y) - _startPoint.Y));
                        return;

                    case ResizeDirection.None:
                        return;
                }
            }
        }

        /// <summary>
        /// 取消点击RootGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRootGridPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isResizing)
            {
                e.Handled = true;
                UpdateMouseCursor(ResizeDirection.None);
                _isResizing = false;
                _rootGrid.ReleasePointerCapture(e.Pointer);
            }
        }

        /// <summary>
        /// 鼠标移入，用于解决Moved事件有时鼠标不变图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRootGridPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!_isResizing)
            {
                UpdateMouseCursor(GetResizeDirection(e.GetCurrentPoint(_panel).Position));
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取当前位置状态
        /// </summary>
        /// <param name="pointerPosition"></param>
        /// <returns></returns>
        ResizeDirection GetResizeDirection(Point pointerPosition)
        {
            Point offset = TransformToVisual(_panel).TransformPoint(new Point(0.0, 0.0));
            double resizerSize = 7.0;
            pointerPosition = new Point(Math.Abs(pointerPosition.X - offset.X), Math.Abs(pointerPosition.Y - offset.Y));
            if ((pointerPosition.X >= 0.0) && (pointerPosition.X < resizerSize))
            {
                if ((pointerPosition.Y >= 0.0) && (pointerPosition.Y < resizerSize))
                {
                    return ResizeDirection.TopLeft;
                }
                if ((pointerPosition.Y > (ActualHeight - resizerSize)) && (pointerPosition.Y <= ActualHeight))
                {
                    return ResizeDirection.BottomLeft;
                }
                return ResizeDirection.Left;
            }
            if ((pointerPosition.X > (ActualWidth - resizerSize)) && (pointerPosition.X <= ActualWidth))
            {
                if ((pointerPosition.Y >= 0.0) && (pointerPosition.Y < resizerSize))
                {
                    return ResizeDirection.TopRight;
                }
                if ((pointerPosition.Y > (ActualHeight - resizerSize)) && (pointerPosition.Y <= ActualHeight))
                {
                    return ResizeDirection.BottomRight;
                }
                return ResizeDirection.Right;
            }
            if ((pointerPosition.Y >= 0.0) && (pointerPosition.Y < resizerSize))
            {
                return ResizeDirection.Top;
            }
            if ((pointerPosition.Y > (ActualHeight - resizerSize)) && (pointerPosition.Y <= ActualHeight))
            {
                return ResizeDirection.Bottom;
            }
            return ResizeDirection.None;
        }

        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="p_window"></param>
        static void BringToFront(ToolWindow p_window)
        {
            Panel pnl = p_window.Parent as Panel;
            if (pnl != null)
            {
                IEnumerable<UIElement> toolWindows = pnl.FindChildrenByType<ToolWindow>(false);
                int maxZIndex = toolWindows.Max(itm => Canvas.GetZIndex(itm));
                if (Canvas.GetZIndex(p_window) <= maxZIndex)
                {
                    Canvas.SetZIndex(p_window, maxZIndex + 1);
                }
            }
        }

        /// <summary>
        /// 切换鼠标样式
        /// </summary>
        /// <param name="direction"></param>
        void UpdateMouseCursor(ResizeDirection direction)
        {
            switch (direction)
            {
                case ResizeDirection.Left:
                case ResizeDirection.Right:
                    this.SetCursor(Windows.UI.Core.CoreCursorType.SizeWestEast);
                    break;

                case ResizeDirection.TopLeft:
                case ResizeDirection.BottomRight:
                    this.SetCursor(Windows.UI.Core.CoreCursorType.SizeNorthwestSoutheast);
                    break;

                case ResizeDirection.Top:
                case ResizeDirection.Bottom:
                    this.SetCursor(Windows.UI.Core.CoreCursorType.SizeNorthSouth);
                    break;

                case ResizeDirection.TopRight:
                case ResizeDirection.BottomLeft:
                    this.SetCursor(Windows.UI.Core.CoreCursorType.SizeNortheastSouthwest);
                    break;

                default:
                    this.SetCursor(Windows.UI.Core.CoreCursorType.Arrow);
                    break;
            }
        }
        #endregion
    }

    internal enum ResizeDirection
    {
        Left,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        None
    }
}

