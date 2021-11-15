#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Tools;
using Dt.Core;
using System;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 桌面分隔栏
    /// </summary>
    public partial class Splitter : Control
    {
        #region 成员变量
        const double _minSideSize = 250;
        Point _lastPos;
        bool _isDragging;
        #endregion

        #region 构造方法
        public Splitter()
        {
            DefaultStyleKey = typeof(Splitter);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 关闭左侧区域事件
        /// </summary>
        public event EventHandler CloseLeft;

        /// <summary>
        /// 关闭右侧区域事件
        /// </summary>
        public event EventHandler CloseRight;
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button btn = (Button)GetTemplateChild("CloseLeftButton");
            btn.Click += (s, e) => CloseLeft?.Invoke(this, EventArgs.Empty);

            btn = (Button)GetTemplateChild("CloseRightButton");
            btn.Click += (s, e) => CloseRight?.Invoke(this, EventArgs.Empty);

            var pnl = (StackPanel)GetTemplateChild("DragPanel");
            pnl.PointerEntered += OnPointerEntered;
            pnl.PointerPressed += OnPointerPressed;
            pnl.PointerMoved += OnPointerMoved;
            pnl.PointerReleased += OnPointerReleased;
            pnl.PointerExited += OnPointerExited;
        }
        #endregion

        #region 拖拽过程
        void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeWestEast, 0);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (((StackPanel)sender).CapturePointer(e.Pointer))
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, "Dragging", true);
                _lastPos = e.GetCurrentPoint((Grid)Parent).Position;
                _isDragging = true;
            }
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDragging)
                return;

            Grid grid = (Grid)Parent;
            var curPos = e.GetCurrentPoint(grid).Position;
            var deltaX = curPos.X - _lastPos.X;

            int colIndex = Grid.GetColumn(this);
            var colLeft = grid.ColumnDefinitions[colIndex - 1];
            var colRight = grid.ColumnDefinitions[colIndex + 1];

            if (colLeft.ActualWidth + deltaX > _minSideSize
                && colRight.ActualWidth - deltaX > _minSideSize)
            {
                if (colLeft.Width.IsAbsolute)
                {
                    colLeft.Width = new GridLength(colLeft.ActualWidth + deltaX);
                }
                else if (colRight.Width.IsAbsolute)
                {
                    colRight.Width = new GridLength(colRight.ActualWidth - deltaX);
                }
                _lastPos = curPos;
            }
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDragging)
                return;

            e.Handled = true;
            _isDragging = false;
            ((StackPanel)sender).ReleasePointerCapture(e.Pointer);
            VisualStateManager.GoToState(this, "Normal", true);
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDragging)
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, "Normal", true);
            }
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }
        #endregion
    }
}