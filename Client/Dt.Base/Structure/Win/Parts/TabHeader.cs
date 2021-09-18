#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 面板标题
    /// </summary>
    public partial class TabHeader : Control
    {
        #region 静态内容
        public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
            "Owner",
            typeof(object), 
            typeof(TabHeader), 
            null);
        #endregion

        double _minOffset = 12;
        bool _dragging;
        Point _start;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TabHeader()
        {
            DefaultStyleKey = typeof(TabHeader);
        }

        /// <summary>
       /// 获取所属Tabs
       /// </summary>
        public object Owner
        {
            get { return GetValue(OwnerProperty); }
            internal set { SetValue(OwnerProperty, value); }
        }

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button btn = GetTemplateChild("BackButton") as Button;
            if (btn != null)
                btn.Click += OnBackClick;
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (CanFloat() && e.IsLeftButton())
            {
                Focus(FocusState.Programmatic);
                _dragging = CapturePointer(e.Pointer);
                if (_dragging)
                    _start = e.GetCurrentPoint(null).Position;
            }
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_dragging)
            {
                Point position = e.GetCurrentPoint(null).Position;
                double offsetX = position.X - _start.X;
                double offsetY = position.Y - _start.Y;
                if (Math.Abs(offsetX) > _minOffset || Math.Abs(offsetY) > _minOffset)
                {
                    _dragging = false;
                    ReleasePointerCapture(e.Pointer);
                    if (Owner is Tabs tabs && tabs.SelectedItem is Tab tab)
                        tab.OwnWin?.OnDragStarted(this, e);
                }
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_dragging)
            {
                _dragging = false;
                ReleasePointerCapture(e.Pointer);
            }
        }
        #endregion

        #region 右键菜单
        static Menu _menu;
        
        protected override async void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            base.OnRightTapped(e);

            if (!(Owner is Tabs tabs 
                && tabs.SelectedItem is Tab tab
                && tab.CanUserPin
                && !tab.IsInCenter
                && !tab.IsFloating))
                return;
            
            if (_menu == null)
            {
                _menu = new Menu { IsContextMenu = true };
                var item = new Mi { ID = "自动隐藏" };
                item.Click += OnAutoHide;
                _menu.Items.Add(item);
            }
            _menu.DataContext = tab;
            await _menu.OpenContextMenu(e.GetPosition(null));
        }

        static void OnAutoHide(object sender, Mi e)
        {
            if (_menu.DataContext is Tab tab)
                tab.IsPinned = false;
        }
        #endregion

        #region 内部方法
        void OnBackClick(object sender, RoutedEventArgs e)
        {
            if (Owner is Tabs tabs && tabs.SelectedItem is Tab tab)
                tab.Backward();
            else if (Owner is AutoHideTab autoTabs && autoTabs.SelectedItem is Tab autoTab)
                autoTab.Backward();
        }

        bool CanFloat()
        {
            Tabs tabs;
            if (Owner == null || (tabs = Owner as Tabs) == null)
                return false;

            return (from pane in tabs.Items
                    where pane is Tab
                    select pane).All((item) => (item as Tab).CanFloat);
        }
        #endregion
    }
}

