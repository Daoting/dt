﻿#region 文件描述
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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 任务栏按扭
    /// </summary>
    public partial class TaskbarItem : Control
    {
        #region 成员变量
        static Menu _menu;
        static TaskbarItem _currentItem;
        Win _win;
        bool _isDragging = false;
        #endregion

        #region 构造方法
        public TaskbarItem(Win p_win)
        {
            DefaultStyleKey = typeof(TaskbarItem);
            _win = p_win;
            _win.IsActivedChanged += OnIsActivedChanged;
            DataContext = p_win;
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button btn = (Button)GetTemplateChild("CloseButton");
            btn.Click += OnCloseItem;
            btn.PointerCaptureLost += OnBtnPointerCaptureLost;

            ToggleSelectedState();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (!_isDragging)
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, _win.IsActived ? "Selected" : "PointerOver", true);
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
            {
                if (CapturePointer(e.Pointer))
                {
                    e.Handled = true;
                    VisualStateManager.GoToState(this, _win.IsActived ? "Selected" : "Pressed", true);
                    _isDragging = true;
                }
            }
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (_isDragging)
                Desktop.Inst.DoSwap(this, e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            ReleasePointerCapture(e.Pointer);
            _isDragging = false;
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            e.Handled = true;
            _isDragging = false;
            Desktop.Inst.ActiveWin(_win);
        }

        protected override void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            _currentItem = this;
            ShowMenu(e.GetPosition(UITree.RootContent));
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            ToggleSelectedState();
        }
        #endregion

        #region 上下文菜单
        static async void ShowMenu(Point p_pos)
        {
            if (_menu == null)
            {
                _menu = new Menu { IsContextMenu = true };
                var item = new Mi { ID = "取消自启动", Icon = Icons.圈停止 };
                item.Click += (a) => AutoStartKit.DelAutoStart();
                _menu.Items.Add(item);
                item = new Mi { ID = "设置自启动", Icon = Icons.圈播放 };
                item.Click += SetAutoStart;
                _menu.Items.Add(item);
                item = new Mi { ID = "恢复窗口默认布局", Icon = Icons.预览链接 };
                item.Click += ResetWinLayout;
                _menu.Items.Add(item);
                item = new Mi { ID = "关闭其他", Icon = Icons.关闭面板 };
                item.Click += CloseOtherWin;
                _menu.Items.Add(item);
                item = new Mi { ID = "停靠在左侧", Icon = Icons.停靠左侧 };
                item.Click += DockLeft;
                _menu.Items.Add(item);
                item = new Mi { ID = "停靠在右侧", Icon = Icons.停靠右侧 };
                item.Click += DockRight;
                _menu.Items.Add(item);


                item = new Mi { ID = "复制类名", Icon = Icons.地址 };
                item.Click += OnCopyWinName;
                _menu.Items.Add(item);
#if WIN
                item = new Mi { ID = "预览Xaml", Icon = Icons.Html };
                item.Click += OnWinXaml;
                _menu.Items.Add(item);
#endif
            }

            var autoStart = await CookieX.GetAutoStart();
            Win win = _currentItem._win;
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
                    Tabs tabs = (Tabs)win.GetValue(Win.MainTabsProperty);
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

            _menu.Items[3].Visibility = (Desktop.Inst.Items.Count > 1) ? Visibility.Visible : Visibility.Collapsed;
            _menu.Items[4].Visibility = !win.IsActived ? Visibility.Visible : Visibility.Collapsed;
            _menu.Items[5].Visibility = !win.IsActived ? Visibility.Visible : Visibility.Collapsed;

            _ = _menu.OpenContextMenu(p_pos);
        }

        static void SetAutoStart(Mi e)
        {
            AutoStartKit.SetAutoStart(_currentItem._win);
        }

        static void ResetWinLayout(Mi e)
        {
            Win win = _currentItem._win;

            // 主区内容为Win时，先恢复
            Tabs tabs = (Tabs)win.GetValue(Win.MainTabsProperty);
            if (tabs != null
                && tabs.Items.Count > 0
                && tabs.Items[0].Content is Win cw
                && cw.AllowResetLayout)
                cw.LoadDefaultLayout();

            if (win.AllowResetLayout)
                win.LoadDefaultLayout();
        }

        static void CloseOtherWin(Mi e)
        {
            Desktop.Inst.CloseExcept(_currentItem._win);
        }

        static void DockLeft(Mi e)
        {
            Desktop.Inst.LeftWin = _currentItem._win;
        }

        static void DockRight(Mi e)
        {
            Desktop.Inst.RightWin = _currentItem._win;
        }

        static void OnCopyWinName(Mi e)
        {
            Type tp = _currentItem._win.GetType();
            Kit.CopyToClipboard(tp.FullName, true);
        }

#if WIN
        static void OnWinXaml(Mi e)
        {
            Type tp = _currentItem._win.GetType();
            if (tp == typeof(Win))
            {
                Kit.Warn("标准的Win，无Xaml内容！");
                return;
            }

            string res = Docking.TabHeader.GetSourcePath(tp);
            if (res == null)
            {
                Kit.Warn($"未找到 [{tp.FullName}] 的Xaml内容！");
            }
            else
            {
                Docking.TabHeader.ShowXamlDlg(res, tp);
            }
        }
#endif
        #endregion

        #region 内部方法
        void OnCloseItem(object sender, RoutedEventArgs e)
        {
            ((Button)sender).PointerCaptureLost -= OnBtnPointerCaptureLost;
            _win.Close();
        }

        void OnBtnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            ToggleSelectedState();
        }

        void OnIsActivedChanged(object sender, EventArgs e)
        {
            ToggleSelectedState();
        }

        void ToggleSelectedState()
        {
            if (_win.IsActived)
                VisualStateManager.GoToState(this, "Selected", true);
            else
                VisualStateManager.GoToState(this, "UnSelected", true);
        }
        #endregion
    }
}
