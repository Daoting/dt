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

        bool _isDragging = false;

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
        /// <summary>
        /// 应用模板
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button btn = (Button)GetTemplateChild("CloseButton");
            if (btn != null)
            {
                btn.Click -= OnCloseItem;
                btn.Click += OnCloseItem;
                btn.PointerCaptureLost -= OnBtnPointerCaptureLost;
                btn.PointerCaptureLost += OnBtnPointerCaptureLost;
            }
            ToggleSelectedState();
        }

        /// <summary>
        /// 鼠标进入
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (!_isDragging)
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, IsActive ? "Selected" : "PointerOver", true);
            }
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            var props = e.GetCurrentPoint(null).Properties;
            if (props.IsLeftButtonPressed)
            {
                if (CapturePointer(e.Pointer))
                {
                    e.Handled = true;
                    VisualStateManager.GoToState(this, IsActive ? "Selected" : "Pressed", true);
                    _isDragging = true;
                }
            }
            else if (props.IsRightButtonPressed)
            {
#if UWP
                MenuFlyout menu = ContextFlyout as MenuFlyout;
                if (menu == null)
                {
                    menu = new MenuFlyout();
                    var item = new MenuFlyoutItem { Text = "取消自启动" };
                    item.Click += (s, a) => LaunchManager.DelAutoStart();
                    menu.Items.Add(item);
                    item = new MenuFlyoutItem { Text = "设置自启动" };
                    item.Click += (s, a) => LaunchManager.SetAutoStart((Win)DataContext);
                    menu.Items.Add(item);
                    item = new MenuFlyoutItem { Text = "恢复窗口默认布局" };
                    item.Click += ResetWinLayout;
                    menu.Items.Add(item);
                    item = new MenuFlyoutItem { Text = "关闭其他" };
                    item.Click += CloseOtherWin;
                    menu.Items.Add(item);
                    item = new MenuFlyoutItem { Text = "停靠在左侧" };
                    item.Click += DockLeft;
                    menu.Items.Add(item);
                    item = new MenuFlyoutItem { Text = "停靠在右侧" };
                    item.Click += DockRight;
                    menu.Items.Add(item);
                    ContextFlyout = menu;
                }

                var autoStart = AtLocal.GetAutoStart();
                Win win = (Win)DataContext;
                if (autoStart != null
                    && win != null
                    && autoStart.WinType == win.GetType().AssemblyQualifiedName
                    && (win.Params == null || autoStart.Params == JsonSerializer.Serialize(win.Params, JsonOptions.UnsafeSerializer)))
                {
                    menu.Items[0].Visibility = Visibility.Visible;
                    menu.Items[1].Visibility = Visibility.Collapsed;
                }
                else
                {
                    menu.Items[0].Visibility = Visibility.Collapsed;
                    menu.Items[1].Visibility = Visibility.Visible;
                }

                if (win != null)
                {
                    if (win.AllowResetLayout)
                    {
                        menu.Items[2].Visibility = Visibility.Visible;
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
                            menu.Items[2].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            menu.Items[2].Visibility = Visibility.Collapsed;
                        }
                    }
                }
                else
                {
                    menu.Items[2].Visibility = Visibility.Collapsed;
                }

                menu.Items[3].Visibility = (Taskbar.Inst.TaskItems.Count > 1) ? Visibility.Visible : Visibility.Collapsed;
                menu.Items[4].Visibility = !IsActive ? Visibility.Visible : Visibility.Collapsed;
                menu.Items[5].Visibility = !IsActive ? Visibility.Visible : Visibility.Collapsed;
#endif
            }
        }

        /// <summary>
        /// 拖动过程中
        /// </summary>
        /// <param name="e"></param>
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

        #region 内部方法
        void ResetWinLayout(object sender, RoutedEventArgs e)
        {
            Win win = (Win)DataContext;

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

        async void CloseOtherWin(object sender, RoutedEventArgs e)
        {
            var ls = await Desktop.Inst.CloseExcept((Win)DataContext);
            if (ls != null && ls.Count > 0)
            {
                var items = Taskbar.Inst.TaskItems;
                if (ls.Count == items.Count - 1)
                {
                    // 其他全部都可关闭
                    items.Clear();
                    items.Add(this);
                }
                else
                {
                    // 只部分
                    foreach (var win in ls)
                    {
                        Taskbar.Inst.RemoveTaskItem(win);
                    }
                }
                IsActive = true;
            }
        }

        void DockLeft(object sender, RoutedEventArgs e)
        {
            Desktop.SetLeftWin((Win)DataContext);
        }

        void DockRight(object sender, RoutedEventArgs e)
        {
            Desktop.SetRightWin((Win)DataContext);
        }

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
