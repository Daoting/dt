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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Dispatching;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// Lv的行基类
    /// </summary>
    public partial class LvRow : Panel
    {
        #region 成员变量
        protected const double _flagWidth = 40;

        protected Lv _owner;
        protected LvItem _row;
        protected Rectangle _rcPointer;
        bool _menuOpened;
        Point _ptStart;
        #endregion
        
        public LvRow(Lv p_owner)
        {
            _owner = p_owner;
        }

        /// <summary>
        /// 设置对应的视图行
        /// </summary>
        /// <param name="p_row"></param>
        public void SetViewRow(LvItem p_row)
        {
            if (_row == p_row)
                return;

            // 虚拟行时需要重置
            if (_row != null)
                _row.ValueChanged = null;

            _row = p_row;
            if (_row != null)
            {
                // 只一次初始化
                _row.Init();
                // 值变化时通过切换DataContext更新
                _row.ValueChanged = OnValueChanged;
            }

            // 停止异步，uno上影响速度
            DataContext = _row;
            //if (p_isAsync)
            //    SetDataContextAsync();
            //else
            //    DataContext = _row;
        }

        /// <summary>
        /// 卸载行
        /// </summary>
        public void Unload()
        {
            Children.Clear();
            if (_row != null)
            {
                _row.ValueChanged = null;
                _row = null;
                DataContext = null;
            }
            
            PointerPressed -= OnPointerPressed;
            PointerReleased -= OnPointerReleased;
            PointerEntered -= OnPointerEntered;
            PointerExited -= OnPointerExited;
            PointerCaptureLost -= OnPointerCaptureLost;
            DoubleTapped -= OnDoubleTapped;

            Tapped -= OnTapped;
            RightTapped -= OnRightTapped;
            OnUnload();
        }

        protected virtual void OnUnload()
        {
        }
        
        /// <summary>
        /// Col属性复制到Dot
        /// </summary>
        /// <param name="p_col"></param>
        /// <param name="p_dot"></param>
        protected void CopyColToDot(Col p_col, Dot p_dot)
        {
            p_dot.ID = p_col.ID;

            // 将Col中的已设置属性值复制到Dot
            if (p_col.ReadLocalValue(Col.CallProperty) != DependencyProperty.UnsetValue)
                p_dot.Call = p_col.Call;
            
            if (p_col.ReadLocalValue(Col.FormatProperty) != DependencyProperty.UnsetValue)
                p_dot.Format = p_col.Format;

            if (p_col.ReadLocalValue(Col.ForegroundProperty) != DependencyProperty.UnsetValue)
                p_dot.Foreground = p_col.Foreground;

            if (p_col.ReadLocalValue(Col.BackgroundProperty) != DependencyProperty.UnsetValue)
                p_dot.Background = p_col.Background;

            if (p_col.ReadLocalValue(Col.FontWeightProperty) != DependencyProperty.UnsetValue)
                p_dot.FontWeight = p_col.FontWeight;

            if (p_col.ReadLocalValue(Col.FontStyleProperty) != DependencyProperty.UnsetValue)
                p_dot.FontStyle = p_col.FontStyle;

            if (p_col.ReadLocalValue(Col.FontSizeProperty) != DependencyProperty.UnsetValue)
                p_dot.FontSize = p_col.FontSize;

            // 内容为空时不自动隐藏，因其负责画右下边线！
            p_dot.AutoHide = false;
        }

        /// <summary>
        /// 附加交互事件
        /// </summary>
        protected void AttachEvent()
        {
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;
            PointerCaptureLost += OnPointerCaptureLost;
            // 参见 Dt.Core\Note.txt 中的事件顺序
            // _btnMenu附加Click事件时，点击Button仍能接收到Tapped事件！
            //Tapped += (s, e) => _row.OnClick();
            DoubleTapped += OnDoubleTapped;

            // 新版uno在PointerCaptureLost中处理
            // android上快速滑动时未触发PointerMoved！
            //#if ANDROID
            //            _owner.Scroll.ViewChanged += (s, e) => _rcPointer.Fill = null;
            //#endif
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // 右键无效
            if (e.IsRightButton())
                return;

            _rcPointer.Fill = _owner.PressedBrush;
            if (CapturePointer(e.Pointer))
            {
                e.Handled = true;
                _ptStart = e.GetCurrentPoint(this).Position;
                // 确保键盘操作有效：上、下、头、尾、复制、全选等，LvPanel.OnKeyUp
                _owner.Focus(FocusState.Programmatic);
            }
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _rcPointer.Fill = null;

            // 放在 Tapped 事件处理受 _btnMenu 点击影响！
            var pt = e.GetCurrentPoint(this).Position;
            if (Math.Abs(pt.X - _ptStart.X) < 6
                && Math.Abs(pt.Y - _ptStart.Y) < 6)
            {
                _row.OnClick();
            }
            
            ReleasePointerCapture(e.Pointer);
        }

        void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!_menuOpened
                && e.IsMouse()
                && (_owner.SelectionMode == SelectionMode.None || !_row.IsSelected))
                _rcPointer.Fill = _owner.EnteredBrush;
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!_menuOpened)
                _rcPointer.Fill = null;
        }

        void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            //Log.Debug("OnPointerCaptureLost");
            _rcPointer.Fill = null;
        }

        void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            _row.OnDoubleClick();
        }

        void OnValueChanged()
        {
            if (_owner.View is IRowView rv)
            {
                // 动态创建的行内容无法通过切换 DataContext 实现界面刷新
                ReloadRowView();
            }
            else
            {
                // 切换行数据上下文
                DataContext = null;
                DataContext = _row;
            }
        }

        protected virtual void ReloadRowView()
        {
        }

        #region 上下文菜单
        /// <summary>
        /// 附加上下文菜单触发事件
        /// </summary>
        /// <param name="p_menu"></param>
        /// <returns></returns>
        protected Button AttachContextMenu(Menu p_menu)
        {
            Button _btnMenu = null;
            var trigger = p_menu.TriggerEvent;
            if (Kit.IsPhoneUI)
            {
                if (!p_menu.ExistLocalValue(Menu.TriggerEventProperty)
                    || trigger == TriggerEvent.Custom)
                {
                    // 因 长按 和 ItemClick 同时触发无法区分，phone模式默认为按钮
                    _btnMenu = CreateMenuButton(p_menu);
                }
                else if (trigger == TriggerEvent.RightTapped)
                {
                    RightTapped += OnRightTapped;
                }
                else if (trigger == TriggerEvent.LeftTapped)
                {
                    Tapped += OnTapped;
                }
            }
            else
            {
                if (trigger == TriggerEvent.RightTapped)
                    RightTapped += OnRightTapped;
                else if (trigger == TriggerEvent.LeftTapped)
                    Tapped += OnTapped;
                else
                    _btnMenu = CreateMenuButton(p_menu);
            }
            return _btnMenu;
        }

        protected void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            OpenContextMenu(e.GetPosition(null));
        }

        protected void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            OpenContextMenu(e.GetPosition(null));
        }

        protected void OnMenuBtnClick(object sender, RoutedEventArgs e)
        {
            OpenContextMenu(new Point(), (Button)sender);
        }

        /// <summary>
        /// 显示上下文菜单
        /// </summary>
        /// <param name="p_pos"></param>
        /// <param name="p_tgt"></param>
        async void OpenContextMenu(Point p_pos, FrameworkElement p_tgt = null)
        {
            Menu menu = Ex.GetMenu(_owner);
            if (menu == null)
                return;

            menu.TargetData = _row.Data;
            menu.Closed += OnMenuClosed;
            if (await menu.OpenContextMenu(p_pos, p_tgt))
            {
                _menuOpened = true;
                _rcPointer.Fill = Res.深黄遮罩;
            }
        }

        Button CreateMenuButton(Menu p_menu)
        {
            // 自定义按钮触发
            var btn = new Button { Content = "\uE178", Style = Res.字符按钮, Foreground = Res.中灰1 };
            if (!Kit.IsPhoneUI)
                p_menu.Placement = MenuPosition.OuterLeftTop;
            return btn;
        }

        void OnMenuClosed(Menu obj)
        {
            // 关闭上下文菜单时移除行醒目颜色
            obj.Closed -= OnMenuClosed;
            _rcPointer.Fill = null;
            _menuOpened = false;
        }
        #endregion

        /// <summary>
        /// 滚动时异步设置DataContext，提高流畅性，停用！
        /// </summary>
        void SetDataContextAsync()
        {
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () => DataContext = _row);
        }
    }
}
