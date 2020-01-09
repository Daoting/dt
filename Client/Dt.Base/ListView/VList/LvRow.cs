#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// Lv的行基类
    /// </summary>
    public partial class LvRow : Panel
    {
        protected const double _flagWidth = 40;

        protected Lv _owner;
        protected LvItem _row;
        protected Rectangle _rcPointer;
        uint? _pointerID;
        Point _ptLast;
        bool _menuOpened;

        public LvRow(Lv p_owner)
        {
            _owner = p_owner;
        }

        /// <summary>
        /// 设置对应的视图行
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_isAsync">是否异步设置DataContext</param>
        public void SetViewRow(LvItem p_row, bool p_isAsync)
        {
            if (_row == p_row)
                return;

            // 虚拟行时需要重置
            if (_row != null)
                _row.ValueChanged = null;

            _row = p_row;
            if (_row != null)
            {
                // 值变化时通过切换DataContext更新
                _row.ValueChanged = OnValueChanged;
            }

            if (p_isAsync)
                SetDataContextAsync();
            else
                DataContext = _row;
        }

        /// <summary>
        /// 附加交互事件
        /// </summary>
        protected void AttachEvent()
        {
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;

            // android上快速滑动时未触发PointerMoved！
#if ANDROID
            _owner.Scroll.ViewChanged += (s, e) => _rcPointer.Fill = null;
#endif
        }

        /// <summary>
        /// 附加上下文菜单触发事件
        /// </summary>
        /// <param name="p_menu"></param>
        /// <returns></returns>
        protected Button AttachContextMenu(Menu p_menu)
        {
            Button _btnMenu = null;
            if (AtSys.IsPhoneUI)
            {
                // PhoneUI模式
                if (p_menu.TouchTrigger == TouchTriggerEvent.Custom)
                {
                    _btnMenu = CreateMenuButton(p_menu);
                }
                else if (p_menu.TouchTrigger == TouchTriggerEvent.Holding)
                {
                    Holding += (s, e) =>
                    {
                        if (e.HoldingState == HoldingState.Started)
                            OpenContextMenu(default);
                    };
                    // win上触摸模式使用鼠标时不触发Holding事件！
                    if (AtSys.System == TargetSystem.Windows)
                        RightTapped += (s, e) => OpenContextMenu(e.GetPosition(null));
                }
                else
                {
                    Tapped += (s, e) => OpenContextMenu(default);
                }
            }
            else
            {
                // WindowsUI模式
                if (p_menu.MouseTrigger == MouseTriggerEvent.RightTapped)
                    RightTapped += (s, e) => OpenContextMenu(e.GetPosition(null));
                else if (p_menu.MouseTrigger == MouseTriggerEvent.LeftTapped)
                    Tapped += (s, e) => OpenContextMenu(e.GetPosition(null));
                else
                    _btnMenu = CreateMenuButton(p_menu);
            }
            return _btnMenu;
        }

        /// <summary>
        /// 显示上下文菜单
        /// </summary>
        /// <param name="p_pos"></param>
        /// <param name="p_tgt"></param>
        protected void OpenContextMenu(Point p_pos, FrameworkElement p_tgt = null)
        {
            Menu menu = Ex.GetMenu(_owner);
            if (menu == null)
                return;

            _menuOpened = true;
            _rcPointer.Fill = AtRes.深黄遮罩;
            menu.TargetData = _row.Data;
            menu.Closed += OnMenuClosed;
            menu.OpenContextMenu(p_pos, p_tgt);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // 右键无效
            if (e.IsRightButton())
                return;

            _rcPointer.Fill = AtRes.暗遮罩;
            if (CapturePointer(e.Pointer))
            {
                e.Handled = true;
                _pointerID = e.Pointer.PointerId;
                _ptLast = e.GetCurrentPoint(null).Position;
            }
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_pointerID != e.Pointer.PointerId)
                return;

            // 允许有短距离移动
            e.Handled = true;
            Point cur = e.GetCurrentPoint(null).Position;
            if (Math.Abs(cur.X - _ptLast.X) > 4 || Math.Abs(cur.Y - _ptLast.Y) > 4)
            {
                ReleasePointerCapture(e.Pointer);
                _pointerID = null;
                if (e.IsTouch())
                    _rcPointer.Fill = null;
            }
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_pointerID != e.Pointer.PointerId)
                return;

            e.Handled = true;
            _rcPointer.Fill = null;
            ReleasePointerCapture(e.Pointer);
            _pointerID = null;
            _row.OnClick();
        }

        void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!_menuOpened
                && e.IsMouse()
                && (_owner.SelectionMode == SelectionMode.None || !_row.IsSelected))
                _rcPointer.Fill = AtRes.黄遮罩;
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!_menuOpened)
                _rcPointer.Fill = null;
        }

        Button CreateMenuButton(Menu p_menu)
        {
            // 自定义按钮触发
            var _btnMenu = new Button { Content = "\uE0DC", Style = AtRes.字符按钮, Foreground = AtRes.深灰边框 };
            _btnMenu.Click += (s, e) => OpenContextMenu(new Point(), (Button)s);
            if (AtSys.System == TargetSystem.Windows)
                p_menu.WinPlacement = MenuPosition.OuterLeftTop;
            return _btnMenu;
        }

        void OnMenuClosed(object sender, EventArgs e)
        {
            // 关闭上下文菜单时移除行醒目颜色
            ((Menu)sender).Closed -= OnMenuClosed;
            _rcPointer.Fill = null;
            _menuOpened = false;
        }

        void OnValueChanged()
        {
            DataContext = null;
            DataContext = _row;
        }

        /// <summary>
        /// 滚动时异步设置DataContext，提高流畅性
        /// </summary>
        async void SetDataContextAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, new DispatchedHandler(() => DataContext = _row));
        }
    }
}
