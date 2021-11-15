#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.System;
#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// Sketch输入处理
    /// </summary>
    internal class SketchInputManager
    {
        #region 成员变量
        Sketch _owner;
        Grid _grid;
        Rectangle _rect;
        bool _isInit = false;

        bool _isCaptured;
        uint? _pointerId;
        bool _isDragging;
        Point _ptStart;
        Point _ptOld;
        Point _selectionStart;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_owner"></param>
        public SketchInputManager(Sketch p_owner)
        {
            _owner = p_owner;
        }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="p_grid"></param>
        /// <param name="p_rect"></param>
        public void Init(Grid p_grid, Rectangle p_rect)
        {
            _grid = p_grid;
            _rect = p_rect;
            _isInit = true;
        }

        /// <summary>
        /// 附加事件
        /// </summary>
        public void AttachEvent()
        {
            if (_isInit)
            {
                _grid.PointerPressed += OnPointerPressed;
                _grid.PointerMoved += OnPointerMoved;
                _grid.PointerReleased += OnPointerReleased;
                _owner.KeyDown += OnKeyDown;
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void DetachEvent()
        {
            if (_isInit)
            {
                _grid.PointerPressed -= OnPointerPressed;
                _grid.PointerMoved -= OnPointerMoved;
                _grid.PointerReleased -= OnPointerReleased;
                _owner.KeyDown -= OnKeyDown;
                _owner.SelectionClerk.Clear();
            }
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isCaptured = _grid.CapturePointer(e.Pointer);
            if (!_isCaptured)
                return;

            FrameworkElement hit;
            var clerk = _owner.SelectionClerk;
            _owner.Focus(FocusState.Programmatic);
            _pointerId = e.Pointer.PointerId;
            _ptStart = e.GetCurrentPoint(_grid).Position;
            _ptOld = _ptStart;

            if (clerk.IsInSelectionRect(_ptStart) && !InputManager.IsCtrlPressed)
            {
                // 在多选矩形内部
                if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
                {
                    e.Handled = true;
                    _isDragging = true;
                    clerk.ShowTipLines();
                    _selectionStart = clerk.GetCurrentPos();
                }
            }
            else if ((hit = _owner.GetItemByPosition(_ptStart)) != null)
            {
                // 在图元内部
                if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
                {
                    e.Handled = true;
                    _isDragging = true;
                    clerk.Select(hit, InputManager.IsCtrlPressed);
                    clerk.ShowTipLines();
                    _selectionStart = clerk.GetCurrentPos();
                }
                else
                {
                    clerk.Select(hit, InputManager.IsCtrlPressed);
                }
            }
            else
            {
                // 空白区域
                _isDragging = false;
                _rect.Visibility = Visibility.Visible;
                if (!InputManager.IsCtrlPressed)
                    clerk.Clear();
            }
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isCaptured || _pointerId != e.Pointer.PointerId)
                return;

            e.Handled = true;
            Point pt = e.GetCurrentPoint(_grid).Position;
            if (_isDragging)
            {
                _owner.SelectionClerk.Move(pt.X - _ptOld.X, pt.Y - _ptOld.Y);
                _owner.SelectionClerk.MoveTipLines();
                _ptOld = pt;
            }
            else
            {
                Canvas.SetLeft(_rect, Math.Min(pt.X, _ptStart.X));
                Canvas.SetTop(_rect, Math.Min(pt.Y, _ptStart.Y));
                _rect.Width = Math.Abs(pt.X - _ptStart.X);
                _rect.Height = Math.Abs(pt.Y - _ptStart.Y);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_isCaptured || _pointerId != e.Pointer.PointerId)
                return;

            var clerk = _owner.SelectionClerk;
            if (_isDragging)
            {
                clerk.AlignGrid();
                _owner.CalcPageSize();
                _owner.HideTipLines();
                // 记录移动历史
                if (clerk.Selection.Count > 0)
                {
                    Point pos = clerk.GetCurrentPos();
                    if (pos.X != _selectionStart.X || pos.Y != _selectionStart.Y)
                    {
                        List<FrameworkElement> ls = new List<FrameworkElement>();
                        ls.AddRange(clerk.Selection);
                        SketchMoveCmdArgs args = new SketchMoveCmdArgs(ls, pos.X - _selectionStart.X, pos.Y - _selectionStart.Y);
                        _owner.His.RecordAction(new CmdAction(_owner.CmdMove, args));
                    }
                }
            }
            else
            {
                double left = Canvas.GetLeft(_rect);
                double top = Canvas.GetTop(_rect);
                var ls = _owner.GetItemsByRegion(left, top, left + _rect.Width, top + _rect.Height);
                if (ls.Count > 0)
                    clerk.Select(ls);
                else
                    _owner.OnSelectionChanged();
                HideSelRect();
            }
            _grid.ReleasePointerCapture(e.Pointer);
            e.Handled = true;
            _isCaptured = false;
            _isDragging = false;
            _pointerId = null;
        }

        /// <summary>
        /// 键盘输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (_owner.SelectionClerk.Selection.Count == 0)
                return;

            switch (e.Key)
            {
                case VirtualKey.Left:
                    MoveStep(-20, 0);
                    break;
                case VirtualKey.Right:
                    MoveStep(20, 0);
                    break;
                case VirtualKey.Up:
                    MoveStep(0, -20);
                    break;
                case VirtualKey.Down:
                    MoveStep(0, 20);
                    break;
                case VirtualKey.Delete:
                    _owner.DeleteSelection();
                    break;
            }
        }

        /// <summary>
        /// 方向键移动位置
        /// </summary>
        /// <param name="p_x"></param>
        /// <param name="p_y"></param>
        void MoveStep(double p_x, double p_y)
        {
            List<FrameworkElement> ls = new List<FrameworkElement>();
            ls.AddRange(_owner.SelectionClerk.Selection);
            SketchMoveCmdArgs args = new SketchMoveCmdArgs(ls, p_x, p_y);
            _owner.CmdMove.Execute(args);
        }

        void HideSelRect()
        {
            Canvas.SetLeft(_rect, 0);
            Canvas.SetTop(_rect, 0);
            _rect.Width = 0;
            _rect.Height = 0;
            _rect.Visibility = Visibility.Collapsed;
        }
    }
}
