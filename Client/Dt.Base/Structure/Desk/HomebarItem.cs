﻿#region 文件描述
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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 主页按钮
    /// </summary>
    public partial class HomebarItem : Control
    {
        #region 成员变量
        Win _win;
        Menu _menu;
        #endregion

        #region 构造方法
        public HomebarItem()
        {
            DefaultStyleKey = typeof(HomebarItem);
        }
        #endregion

        internal void SetWin(Win p_win)
        {
            _win = p_win;
            _win.IsActivedChanged += OnIsActivedChanged;
            ToggleSelectedState();
        }

        #region 重写方法
#if WIN
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var btn = (Button)GetTemplateChild("BtnMenu");
            if (btn != null)
            {
                btn.Tapped -= BtnMenuTapped;
                btn.Tapped += BtnMenuTapped;
            }
            ToggleSelectedState();
        }

        void BtnMenuTapped(object sender, TappedRoutedEventArgs e)
        {
            if (_win is IStartMenu sm)
            {
                e.Handled = true;
                sm.ShowStartMenu();
            }
        }
#endif

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            VisualStateManager.GoToState(this, _win.IsActived ? "Selected" : ((_win is IStartMenu) ? "PointerOverMenu" : "PointerOver"), true);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed
                && CapturePointer(e.Pointer))
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, _win.IsActived ? "Selected" : "Pressed", true);
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            ReleasePointerCapture(e.Pointer);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Desktop.Inst.MainWin = _win;
        }

        protected override void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            ShowMenu(e.GetPosition(UITree.RootContent));
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            ToggleSelectedState();
        }
        #endregion

        #region 上下文菜单
        void ShowMenu(Point p_pos)
        {
            if (_menu == null)
            {
                _menu = new Menu { IsContextMenu = true };
                var item = new Mi { ID = "系统", Icon = Icons.设置 };
                item.Click += (a) => SysTrace.ShowSysBox();
                _menu.Items.Add(item);

#if WIN //&& !DEBUG
                if (Kit.IsUsingSvc)
                {
                    item = new Mi { ID = "检查更新", Icon = Icons.刷新卡片 };
                    item.Click += (a) => WinPkgUpdate.CheckUpdate(true);
                    _menu.Items.Add(item);
                }
#endif
            }
            _ = _menu.OpenContextMenu(p_pos);
        }
        #endregion

        #region 内部方法
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
