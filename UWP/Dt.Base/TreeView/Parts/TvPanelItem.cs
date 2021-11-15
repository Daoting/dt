#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
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
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.TreeViews
{
    /// <summary>
    /// 树节点面板
    /// </summary>
    public partial class TvPanelItem : Panel
    {
        #region 成员变量
        const double _btnWidth = 40;
        const double _cbWidth = 25;
        TreeView _owner;
        TvItem _row;
        Rectangle _rcPointer;
        double _indent;
        Button _btnMenu;
        bool _menuOpened;
        #endregion

        #region 构造方法
        public TvPanelItem(TreeView p_owner)
        {
            _owner = p_owner;
            LoadContent();
        }

        public TvPanelItem(TreeView p_owner, TvItem p_row)
        {
            _owner = p_owner;
            _row = p_row;
            _indent = _row.Depth * _owner.Indent;
            LoadContent();
            DataContext = _row;
        }
        #endregion

        /// <summary>
        /// 设置对应的视图行
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="p_isAsync">是否异步设置DataContext</param>
        public void SetItem(TvItem p_item, bool p_isAsync)
        {
            if (_row == p_item)
                return;

            // 虚拟行时需要重置
            if (_row != null)
                _row.ValueChanged = null;

            _row = p_item;
            if (_row != null)
            {
                // 值变化时通过切换DataContext更新
                _row.ValueChanged = OnValueChanged;
                SetIndent(_row.Depth * _owner.Indent);
            }

            if (p_isAsync)
                SetDataContextAsync();
            else
                DataContext = _row;
        }

        #region 重写方法
        protected override Size MeasureOverride(Size availableSize)
        {
            // 行最小高度41
            double height = Res.RowOuterHeight;
            double usedWidth = 0;
            int index = 0;

            // 展开/折叠按钮
            UIElement elem = (UIElement)Children[index++];
            elem.Measure(new Size(_btnWidth, availableSize.Height));
            usedWidth += _btnWidth;

            // 多选框
            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                elem = (UIElement)Children[index++];
                elem.Measure(new Size(_cbWidth, availableSize.Height));
                usedWidth += _cbWidth;
            }

            // 内容
            elem = (UIElement)Children[index++];
            if (_btnMenu != null)
                usedWidth += _btnWidth;
            elem.Measure(new Size(Math.Max(availableSize.Width - usedWidth, 0), availableSize.Height));
            if (elem.DesiredSize.Height > height)
                height = elem.DesiredSize.Height;

            // 上下文菜单
            if (_btnMenu != null)
            {
                index++;
                _btnMenu.Measure(new Size(_btnWidth, _btnWidth));
            }

            // 选择背景
            elem = (UIElement)Children[index++];
            elem.Measure(availableSize);

            // 交互背景
            _rcPointer.Measure(availableSize);
            return new Size(availableSize.Width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double left = _indent;
            int index = 0;

            // 展开/折叠按钮
            UIElement elem = (UIElement)Children[index++];
            elem.Arrange(new Rect(left, 0, _btnWidth, finalSize.Height));
            left += _btnWidth;

            // 多选框
            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                elem = (UIElement)Children[index++];
                elem.Arrange(new Rect(left, 0, _cbWidth, finalSize.Height));
                left += _cbWidth;
            }

            // 内容
            double right = (_btnMenu != null) ? _btnWidth : 0;
            elem = (UIElement)Children[index++];
            elem.Arrange(new Rect(left, 0, Math.Max(finalSize.Width - left - right, 0), finalSize.Height));

            // 上下文菜单，垂直居中
            if (_btnMenu != null)
            {
                index++;
                _btnMenu.Arrange(new Rect(finalSize.Width - _btnWidth, (finalSize.Height - _btnWidth) / 2, _btnWidth, _btnWidth));
            }

            // 选择背景
            Rect rc = new Rect(new Point(), finalSize);
            elem = (UIElement)Children[index++];
            elem.Arrange(rc);

            // 交互背景
            _rcPointer.Arrange(rc);
            return finalSize;
        }
        #endregion

        void LoadContent()
        {
            // 背景
            SetBinding(BackgroundProperty, new Binding { Path = new PropertyPath("Background") });

            // 展开/折叠按钮
            ContentPresenter pre = new ContentPresenter { Width = _btnWidth };
            pre.SetBinding(ContentPresenter.ContentProperty, new Binding { Path = new PropertyPath("ExpandedUI") });
            Children.Add(pre);

            // 多选框
            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                var tbCheck = new TextBlock { VerticalAlignment = VerticalAlignment.Center, FontFamily = Res.IconFont };
                tbCheck.SetBinding(TextBlock.TextProperty, new Binding
                {
                    Path = new PropertyPath("IsSelected"),
                    Converter = new IsSelectedIconConverter(),
                });
                Children.Add(tbCheck);
            }

            // 内容
            UIElement ui = null;
            if (_owner.View is DataTemplate template)
            {
                ui = template.LoadContent() as UIElement;
            }
            else if (_owner.View is DataTemplateSelector selector && _row != null)
            {
                // 模板选择器
                var temp = selector.SelectTemplate(_row);
                if (temp != null)
                    ui = temp.LoadContent() as UIElement;
            }
            if (ui != null)
                Children.Add(ui);

            // 上下文菜单
            Menu menu = Ex.GetMenu(_owner);
            if (menu != null)
                AttachContextMenu(menu);

            // 分割线及选择背景
            Border bd;
            if (_owner.ShowRowLine)
                bd = new Border { BorderThickness = new Thickness(0, 0, 0, 1), BorderBrush = Res.浅灰2, IsHitTestVisible = false };
            else
                bd = new Border { IsHitTestVisible = false };
            if (_owner.SelectionMode != SelectionMode.None)
            {
                bd.SetBinding(Border.BackgroundProperty, new Binding
                {
                    Path = new PropertyPath("IsSelected"),
                    Converter = new SelectedBackgroundConverter(),
                });
            }
            Children.Add(bd);

            // 交互背景
            _rcPointer = new Rectangle { IsHitTestVisible = false };
            Children.Add(_rcPointer);

            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;
            PointerCaptureLost += OnPointerCaptureLost;
            Tapped += (s, e) => _row.OnClick();
            DoubleTapped += (s, e) => _row.OnDoubleTapped();
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
                && _row.IsSelected.HasValue
                && !_row.IsSelected.Value)
                _rcPointer.Fill = _owner.EnteredBrush;
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!_menuOpened)
                _rcPointer.Fill = null;
        }

        void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            _rcPointer.Fill = null;
        }

        /// <summary>
        /// 缩进不同时必须重新布局，不然有时不对！
        /// </summary>
        /// <param name="p_indent"></param>
        void SetIndent(double p_indent)
        {
            if (p_indent != _indent)
            {
                _indent = p_indent;
                InvalidateArrange();
            }
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

        #region 上下文菜单
        /// <summary>
        /// 附加上下文菜单触发事件
        /// </summary>
        /// <param name="p_menu"></param>
        void AttachContextMenu(Menu p_menu)
        {
            if (p_menu.TriggerEvent == TriggerEvent.RightTapped)
                RightTapped += (s, e) => OpenContextMenu(e.GetPosition(null));
            else if (p_menu.TriggerEvent == TriggerEvent.LeftTapped)
                Tapped += (s, e) => OpenContextMenu(e.GetPosition(null));
            else
                CreateMenuButton(p_menu);
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

        void CreateMenuButton(Menu p_menu)
        {
            // 自定义按钮触发
            _btnMenu = new Button { Content = "\uE03F", Style = Res.字符按钮, Foreground = Res.深灰2 };
            _btnMenu.Click += (s, e) => OpenContextMenu(new Point(), (Button)s);
            if (!Kit.IsPhoneUI)
                p_menu.Placement = MenuPosition.OuterLeftTop;
            Children.Add(_btnMenu);
        }

        void OnMenuClosed(object sender, EventArgs e)
        {
            // 关闭上下文菜单时移除行醒目颜色
            ((Menu)sender).Closed -= OnMenuClosed;
            _rcPointer.Fill = null;
            _menuOpened = false;
        }
        #endregion
    }
}
