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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
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
        static CellUIConverter _uiConverter = new CellUIConverter();
        protected const double _flagWidth = 40;

        protected Lv _owner;
        protected LvItem _row;
        protected Rectangle _rcPointer;
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
        /// 绑定单元格，数据源为ViewItem
        /// </summary>
        /// <param name="p_col"></param>
        /// <param name="p_pre"></param>
        protected void SetContentBinding(Col p_col, ContentPresenter p_pre)
        {
            p_pre.SetBinding(ContentPresenter.ContentProperty, new Binding { Converter = _uiConverter, ConverterParameter = p_col, Mode = BindingMode.OneTime });

            // 优先级：直接设置 > ViewItem属性，未直接设置的绑定ViewItem中行样式
            if (p_col.ReadLocalValue(Col.ForegroundProperty) == DependencyProperty.UnsetValue)
                p_pre.SetBinding(ContentPresenter.ForegroundProperty, new Binding { Path = new PropertyPath("Foreground") });
            else
                p_pre.Foreground = p_col.Foreground;

            if (p_col.ReadLocalValue(Col.BackgroundProperty) == DependencyProperty.UnsetValue)
                p_pre.SetBinding(ContentPresenter.BackgroundProperty, new Binding { Path = new PropertyPath("Background") });
            else
                p_pre.Background = p_col.Background;

            if (p_col.ReadLocalValue(Col.FontWeightProperty) == DependencyProperty.UnsetValue)
                p_pre.SetBinding(ContentPresenter.FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight") });
            else
                p_pre.FontWeight = p_col.FontWeight;

            if (p_col.ReadLocalValue(Col.FontStyleProperty) == DependencyProperty.UnsetValue)
                p_pre.SetBinding(ContentPresenter.FontStyleProperty, new Binding { Path = new PropertyPath("FontStyle") });
            else
                p_pre.FontStyle = p_col.FontStyle;

            if (p_col.ReadLocalValue(Col.FontSizeProperty) == DependencyProperty.UnsetValue)
                p_pre.SetBinding(ContentPresenter.FontSizeProperty, new Binding { Path = new PropertyPath("FontSize") });
            else
                p_pre.FontSize = p_col.FontSize;
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
            Tapped += (s, e) => _row.OnClick();
            DoubleTapped += (s, e) => _row.OnDoubleClick();

            // 新版uno在PointerCaptureLost中处理
            // android上快速滑动时未触发PointerMoved！
            //#if ANDROID
            //            _owner.Scroll.ViewChanged += (s, e) => _rcPointer.Fill = null;
            //#endif
        }

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
                    RightTapped += (s, e) => OpenContextMenu(e.GetPosition(null));
                }
                else if (trigger == TriggerEvent.LeftTapped)
                {
                    Tapped += (s, e) => OpenContextMenu(e.GetPosition(null));
                }
            }
            else
            {
                if (trigger == TriggerEvent.RightTapped)
                    RightTapped += (s, e) => OpenContextMenu(e.GetPosition(null));
                else if (trigger == TriggerEvent.LeftTapped)
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
        protected async void OpenContextMenu(Point p_pos, FrameworkElement p_tgt = null)
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

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // 右键无效
            if (e.IsRightButton())
                return;

            _rcPointer.Fill = _owner.PressedBrush;
            if (CapturePointer(e.Pointer))
                e.Handled = true;
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _rcPointer.Fill = null;
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

        Button CreateMenuButton(Menu p_menu)
        {
            // 自定义按钮触发
            var btn = new Button { Content = "\uE03F", Style = Res.字符按钮, Foreground = Res.深灰2 };
            btn.Click += (s, e) => OpenContextMenu(new Point(), (Button)s);
            if (!Kit.IsPhoneUI)
                p_menu.Placement = MenuPosition.OuterLeftTop;
            return btn;
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
