#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Linq;
using System.Text.Json;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 任务栏按扭
    /// </summary>
    public partial class TaskbarItem : Control
    {
        #region 静态成员
        /// <summary>
        /// 是否为激活项
        /// </summary>
        public readonly static DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive",
            typeof(bool),
            typeof(TaskbarItem),
            new PropertyMetadata(false, OnIsActiveChanged));

        static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TaskbarItem)d).ToggleSelectedState();
        }
        #endregion

        #region 成员变量
        static Menu _menu;
        static TaskbarItem _currentItem;
        bool _isDragging = false;
        #endregion

        #region 构造方法
        public TaskbarItem()
        {
            DefaultStyleKey = typeof(TaskbarItem);
        }
        #endregion

        /// <summary>
        /// 获取设置是否为激活项
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button btn = (Button)GetTemplateChild("CloseButton");
            if (btn != null)
            {
                btn.Click += OnCloseItem;
                btn.PointerCaptureLost += OnBtnPointerCaptureLost;
            }
            ToggleSelectedState();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (!_isDragging)
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, IsActive ? "Selected" : "PointerOver", true);
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
            {
                if (CapturePointer(e.Pointer))
                {
                    e.Handled = true;
                    VisualStateManager.GoToState(this, IsActive ? "Selected" : "Pressed", true);
                    _isDragging = true;
                }
            }
        }

        protected override void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            _currentItem = this;
            ShowMenu(e.GetPosition(SysVisual.RootContent));
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            e.Handled = true;
            _currentItem = this;
            ShowMenu(e.GetPosition(SysVisual.RootContent));
        }

        // 容易与拖拽混
        //protected override void OnHolding(HoldingRoutedEventArgs e)
        //{
        //    if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
        //    {
        //        e.Handled = true;
        //        ShowMenu(e.GetPosition(SysVisual.RootContent));
        //    }
        //}

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (!_isDragging)
                return;

            Point pt = e.GetCurrentPoint(null).Position;
            var items = Taskbar.Inst.TaskItems;
            TaskbarItem tgt = items.FirstOrDefault(itm => itm.ContainPoint(pt));
            if (tgt != null && tgt != this)
            {
                // 交换位置的最小移动控制
                pt = e.GetCurrentPoint(tgt).Position;
                double delta = tgt.ActualWidth / 2;
                if ((pt.X < delta && pt.X > 20)
                    || (pt.X > delta && pt.X < tgt.ActualWidth - 20))
                {
                    int index = items.IndexOf(this);
                    items.Remove(tgt);
                    items.Insert(index, tgt);
                }
            }
        }

        /// <summary>
        /// 抬起时激活窗口
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (!_isDragging)
            {
                ToggleSelectedState();
                return;
            }

            ReleasePointerCapture(e.Pointer);
            e.Handled = true;
            _isDragging = false;

            Point pt = e.GetCurrentPoint(null).Position;
            if (this.ContainPoint(pt) && !IsActive)
            {
                // 释放时鼠标在内部
                Win win = (Win)DataContext;
                if (Desktop.Inst.ActiveWin(win))
                    Taskbar.Inst.ActiveTaskItem(this);
            }
        }

        /// <summary>
        /// 鼠标移出
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            ToggleSelectedState();
        }
        #endregion

        #region 上下文菜单
        static void ShowMenu(Point p_pos)
        {
            if (_menu == null)
            {
                _menu = new Menu { IsContextMenu = true };
                var item = new Mi { ID = "取消自启动" };
                item.Click += (s, a) => LaunchManager.DelAutoStart();
                _menu.Items.Add(item);
                item = new Mi { ID = "设置自启动" };
                item.Click += (s, a) => LaunchManager.SetAutoStart((Win)_currentItem.DataContext);
                _menu.Items.Add(item);
                item = new Mi { ID = "恢复窗口默认布局" };
                item.Click += ResetWinLayout;
                _menu.Items.Add(item);
                item = new Mi { ID = "关闭其他" };
                item.Click += CloseOtherWin;
                _menu.Items.Add(item);
                item = new Mi { ID = "停靠在左侧" };
                item.Click += DockLeft;
                _menu.Items.Add(item);
                item = new Mi { ID = "停靠在右侧" };
                item.Click += DockRight;
                _menu.Items.Add(item);
            }

            var autoStart = AtLocal.GetAutoStart();
            Win win = (Win)_currentItem.DataContext;
            if (autoStart != null
                && win != null
                && autoStart.WinType == win.GetType().AssemblyQualifiedName
                && (win.Params == null || autoStart.Params == JsonSerializer.Serialize(win.Params, JsonOptions.UnsafeSerializer)))
            {
                _menu.Items[0].Visibility = Visibility.Visible;
                _menu.Items[1].Visibility = Visibility.Collapsed;
            }
            else
            {
                _menu.Items[0].Visibility = Visibility.Collapsed;
                _menu.Items[1].Visibility = Visibility.Visible;
            }

            if (win != null)
            {
                if (win.AllowResetLayout)
                {
                    _menu.Items[2].Visibility = Visibility.Visible;
                }
                else
                {
                    // 主区内容为Win
                    Tabs tabs = (Tabs)win.GetValue(Win.CenterTabsProperty);
                    if (tabs != null
                        && tabs.Items.Count > 0
                        && tabs.Items[0].Content is Win cw
                        && cw.AllowResetLayout)
                    {
                        _menu.Items[2].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _menu.Items[2].Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                _menu.Items[2].Visibility = Visibility.Collapsed;
            }

            _menu.Items[3].Visibility = (Taskbar.Inst.TaskItems.Count > 1) ? Visibility.Visible : Visibility.Collapsed;
            _menu.Items[4].Visibility = !_currentItem.IsActive ? Visibility.Visible : Visibility.Collapsed;
            _menu.Items[5].Visibility = !_currentItem.IsActive ? Visibility.Visible : Visibility.Collapsed;

            _menu.OpenContextMenu(p_pos);
        }

        static void ResetWinLayout(object sender, Mi e)
        {
            Win win = (Win)_currentItem.DataContext;

            // 主区内容为Win时，先恢复
            Tabs tabs = (Tabs)win.GetValue(Win.CenterTabsProperty);
            if (tabs != null
                && tabs.Items.Count > 0
                && tabs.Items[0].Content is Win cw
                && cw.AllowResetLayout)
                cw.LoadDefaultLayout();

            if (win.AllowResetLayout)
                win.LoadDefaultLayout();
        }

        static async void CloseOtherWin(object sender, Mi e)
        {
            var ls = await Desktop.Inst.CloseExcept((Win)_currentItem.DataContext);
            if (ls != null && ls.Count > 0)
            {
                var items = Taskbar.Inst.TaskItems;
                if (ls.Count == items.Count - 1)
                {
                    // 其他全部都可关闭
                    items.Clear();
                    items.Add(_currentItem);
                }
                else
                {
                    // 只部分
                    foreach (var win in ls)
                    {
                        Taskbar.Inst.RemoveTaskItem(win);
                    }
                }
                _currentItem.IsActive = true;
            }
        }

        static void DockLeft(object sender, Mi e)
        {
            Desktop.SetLeftWin((Win)_currentItem.DataContext);
        }

        static void DockRight(object sender, Mi e)
        {
            Desktop.SetRightWin((Win)_currentItem.DataContext);
        }
        #endregion

        #region 内部方法
        void OnCloseItem(object sender, RoutedEventArgs e)
        {
            ((Button)sender).PointerCaptureLost -= OnBtnPointerCaptureLost;
            ((Win)DataContext).Close();
        }

        void OnBtnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            ToggleSelectedState();
        }

        void ToggleSelectedState()
        {
            if (IsActive)
                VisualStateManager.GoToState(this, "Selected", true);
            else
                VisualStateManager.GoToState(this, "UnSelected", true);
        }
        #endregion
    }
}
