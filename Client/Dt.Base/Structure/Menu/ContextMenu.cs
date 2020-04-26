#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 上下文菜单相关
    /// </summary>
    public partial class Menu : Control
    {
        #region 静态内容
        public static readonly DependencyProperty ContextTargetProperty = DependencyProperty.Register(
            "ContextTarget",
            typeof(FrameworkElement),
            typeof(Menu),
            new PropertyMetadata(null));

        public static readonly DependencyProperty WinPlacementProperty = DependencyProperty.Register(
            "WinPlacement",
            typeof(MenuPosition),
            typeof(Menu),
            new PropertyMetadata(MenuPosition.Default));

        public static readonly DependencyProperty MouseTriggerProperty = DependencyProperty.Register(
            "MouseTrigger",
            typeof(MouseTriggerEvent),
            typeof(Menu),
            new PropertyMetadata(MouseTriggerEvent.RightTapped, OnMouseTriggerEventChanged));

        public static readonly DependencyProperty TouchTriggerProperty = DependencyProperty.Register(
            "TouchTrigger",
            typeof(TouchTriggerEvent),
            typeof(Menu),
            new PropertyMetadata(TouchTriggerEvent.Custom, OnTouchTriggerEventChanged));

        public static readonly DependencyProperty TargetDataProperty = DependencyProperty.Register(
            "TargetData",
            typeof(object),
            typeof(Menu),
            new PropertyMetadata(null));

        internal static readonly DependencyProperty IsContextMenuProperty = DependencyProperty.Register(
            "IsContextMenu",
            typeof(bool),
            typeof(Menu),
            new PropertyMetadata(false));

        static void OnMouseTriggerEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!AtSys.IsTouchMode)
                ((Menu)d).OnTriggerEventChanged();
        }

        static void OnTouchTriggerEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (AtSys.IsTouchMode)
                ((Menu)d).OnTriggerEventChanged();
        }
        #endregion

        #region 成员变量
        Dlg _dlg;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置上下文菜单的目标元素
        /// </summary>
        public FrameworkElement ContextTarget
        {
            get { return (FrameworkElement)GetValue(ContextTargetProperty); }
            set { SetValue(ContextTargetProperty, value); }
        }

        /// <summary>
        /// 获取设置win模式时上下文菜单的显示位置，默认为指定位置
        /// </summary>
        public MenuPosition WinPlacement
        {
            get { return (MenuPosition)GetValue(WinPlacementProperty); }
            set { SetValue(WinPlacementProperty, value); }
        }

        /// <summary>
        /// 获取设置鼠标模式触发上下文菜单的事件种类，默认RightTapped
        /// </summary>
        public MouseTriggerEvent MouseTrigger
        {
            get { return (MouseTriggerEvent)GetValue(MouseTriggerProperty); }
            set { SetValue(MouseTriggerProperty, value); }
        }

        /// <summary>
        /// 获取设置触摸模式触发上下文菜单的事件种类，默认Custom
        /// </summary>
        public TouchTriggerEvent TouchTrigger
        {
            get { return (TouchTriggerEvent)GetValue(TouchTriggerProperty); }
            set { SetValue(TouchTriggerProperty, value); }
        }

        /// <summary>
        /// 获取上下文菜单的目标数据
        /// </summary>
        public object TargetData
        {
            get { return GetValue(TargetDataProperty); }
            internal set { SetValue(TargetDataProperty, value); }
        }

        /// <summary>
        /// 是否为上下文菜单
        /// </summary>
        public bool IsContextMenu
        {
            get { return (bool)GetValue(IsContextMenuProperty); }
            set { SetValue(IsContextMenuProperty, value); }
        }
        #endregion

        /// <summary>
        /// 打开上下文菜单
        /// </summary>
        /// <param name="p_tgt">相对目标元素，win模式有效，通过WinPlacement控制相对位置</param>
        public void OpenContextMenu(FrameworkElement p_tgt)
        {
            OpenContextMenu(default, p_tgt);
        }

        /// <summary>
        /// 打开上下文菜单
        /// </summary>
        /// <param name="p_pos">在指定位置显示，win模式有效</param>
        /// <param name="p_tgtPlacement">相对目标元素，win模式有效</param>
        public async void OpenContextMenu(Point p_pos = default, FrameworkElement p_tgtPlacement = null)
        {
            if (!IsContextMenu || (_dlg != null && _dlg.IsOpened))
                return;

            // 取消或无菜单项时不显示
            if (Opening != null)
            {
                var args = new AsyncCancelEventArgs();
                Opening(this, args);
                await args.EnsureAllCompleted();
                if (args.Cancel || Items.Count == 0)
                    return;
            }

            if (_dlg == null)
            {
                _dlg = new Dlg
                {
                    HideTitleBar = true,
                    Resizeable = false,
                    Content = this,
                    Background = AtRes.浅灰背景,
                    MinWidth = 160,
                };
                if (AtSys.IsPhoneUI)
                    _dlg.PhonePlacement = DlgPlacement.FromBottom;
                _dlg.Closed += (s, e) => Closed?.Invoke(this, EventArgs.Empty);
            }

            // 相对位置显示
            if (!AtSys.IsPhoneUI && WinPlacement != MenuPosition.Default)
            {
                _dlg.PlacementTarget = p_tgtPlacement ?? ContextTarget;
                _dlg.WinPlacement = (DlgPlacement)WinPlacement + 5;
            }
            _dlg.Show();

            if (!AtSys.IsPhoneUI && WinPlacement == MenuPosition.Default)
            {
                // 计算显示位置
                double width = _dlg.DesiredSize.Width;
                double height = _dlg.DesiredSize.Height;
                _dlg.Left = (p_pos.X + width > SysVisual.ViewWidth) ? Math.Floor(p_pos.X - width) : p_pos.X;
                _dlg.Top = (p_pos.Y + height > SysVisual.ViewHeight) ? Math.Floor(p_pos.Y - height) : p_pos.Y;
            }

            Focus(FocusState.Programmatic);
            Opened?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 附加事件
        /// </summary>
        /// <param name="p_target"></param>
        internal void InitContextMenu(FrameworkElement p_target)
        {
            IsContextMenu = true;
            ContextTarget = p_target;
            p_target.Unloaded += OnTargetUnloaded;
            AttachTriggerEvent();
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        internal void UnloadContextMenu()
        {
            var tgt = ContextTarget;
            ClearValue(ContextTargetProperty);
            tgt.Unloaded -= OnTargetUnloaded;
            DetachTriggerEvent();
        }

        void OnTriggerEventChanged()
        {
            if (ContextTarget == null)
                return;

            if (ContextTarget is IMenuHost cm)
            {
                cm.UpdateContextMenu();
                return;
            }

            DetachTriggerEvent();
            AttachTriggerEvent();
        }

        void AttachTriggerEvent()
        {
            var tgt = ContextTarget;
            if (tgt == null || tgt is IMenuHost)
                return;

            if (tgt is Button btn)
            {
                // uno中Button不触发Tapped事件！
                btn.Click += OnButtonClick;
                return;
            }

            if (AtSys.IsTouchMode)
            {
                if (TouchTrigger == TouchTriggerEvent.Holding)
                {
                    tgt.AddHandler(HoldingEvent, (HoldingEventHandler)OnTargetHolding, true);
                    // win上触摸模式使用鼠标时不触发Holding事件！
                    if (AtSys.System == TargetSystem.Windows)
                        tgt.AddHandler(RightTappedEvent, (RightTappedEventHandler)OnTargetRightTapped, true);
                }
                else if (TouchTrigger == TouchTriggerEvent.Tapped)
                {
                    tgt.AddHandler(TappedEvent, (TappedEventHandler)OnTargetTapped, true);
                }
            }
            else
            {
                if (MouseTrigger == MouseTriggerEvent.RightTapped)
                    tgt.AddHandler(RightTappedEvent, (RightTappedEventHandler)OnTargetRightTapped, true);
                else if (MouseTrigger == MouseTriggerEvent.LeftTapped)
                    tgt.AddHandler(TappedEvent, (TappedEventHandler)OnTargetTapped, true);
            }
        }

        void DetachTriggerEvent()
        {
            var tgt = ContextTarget;
            if (tgt == null || tgt is IMenuHost)
                return;

            if (tgt is Button btn)
            {
                btn.Click -= OnButtonClick;
                return;
            }

            tgt.RemoveHandler(TappedEvent, (TappedEventHandler)OnTargetTapped);
            tgt.RemoveHandler(RightTappedEvent, (RightTappedEventHandler)OnTargetRightTapped);
            tgt.RemoveHandler(HoldingEvent, (HoldingEventHandler)OnTargetHolding);
        }

        void OnTargetRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!AtSys.IsPhoneUI && WinPlacement == MenuPosition.Default)
                OpenContextMenu(e.GetPosition(null));
            else
                OpenContextMenu();
        }

        void OnTargetTapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!AtSys.IsPhoneUI && WinPlacement == MenuPosition.Default)
                OpenContextMenu(e.GetPosition(null));
            else
                OpenContextMenu();
        }

        void OnTargetHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
            {
                e.Handled = true;
                if (!AtSys.IsPhoneUI && WinPlacement == MenuPosition.Default)
                    OpenContextMenu(e.GetPosition(null));
                else
                    OpenContextMenu();
            }
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (!AtSys.IsPhoneUI && WinPlacement == MenuPosition.Default)
                OpenContextMenu(((Button)sender).GetAbsolutePosition());
            else
                OpenContextMenu();
        }

        void OnTargetUnloaded(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
