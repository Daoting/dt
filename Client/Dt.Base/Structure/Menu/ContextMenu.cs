#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.MenuView;
using Dt.Core;
using System;
using System.Threading.Tasks;
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
    public partial class Menu : DtControl
    {
        #region 静态内容
        public static readonly DependencyProperty ContextTargetProperty = DependencyProperty.Register(
            "ContextTarget",
            typeof(FrameworkElement),
            typeof(Menu),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(
            "Placement",
            typeof(MenuPosition),
            typeof(Menu),
            new PropertyMetadata(MenuPosition.Default));

        public static readonly DependencyProperty TriggerEventProperty = DependencyProperty.Register(
            "TriggerEvent",
            typeof(TriggerEvent),
            typeof(Menu),
            new PropertyMetadata(TriggerEvent.RightTapped, OnTriggerEventChanged));

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

        static void OnTriggerEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
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
        /// 获取设置上下文菜单的显示位置，默认为Default，win模式在指定位置显示，phone模式为FromBottom
        /// </summary>
        public MenuPosition Placement
        {
            get { return (MenuPosition)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary>
        /// 获取设置触发上下文菜单的事件种类，默认RightTapped(鼠标右键，触摸时长按)
        /// </summary>
        public TriggerEvent TriggerEvent
        {
            get { return (TriggerEvent)GetValue(TriggerEventProperty); }
            set { SetValue(TriggerEventProperty, value); }
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
        public Task<bool> OpenContextMenu(FrameworkElement p_tgt)
        {
            return OpenContextMenu(default, p_tgt);
        }

        /// <summary>
        /// 打开上下文菜单
        /// </summary>
        /// <param name="p_pos">在指定位置显示，win模式有效</param>
        /// <param name="p_tgtPlacement">相对目标元素，win模式有效</param>
        /// <returns>false表菜单已取消显示</returns>
        public async Task<bool> OpenContextMenu(Point p_pos = default, FrameworkElement p_tgtPlacement = null)
        {
            if (!IsContextMenu || (_dlg != null && _dlg.IsOpened))
                return false;

            // 取消或无菜单项时不显示
            if (Opening != null)
            {
                var args = new AsyncCancelEventArgs();
                Opening(this, args);
                await args.EnsureAllCompleted();
                if (args.Cancel || Items.Count == 0)
                    return false;
            }

            if (_dlg == null)
            {
                _dlg = new Dlg
                {
                    HideTitleBar = true,
                    Resizeable = false,
                    Content = this,
                    Background = Res.浅灰1,
                    MinWidth = 160,

                    // 不向下层对话框传递Press事件
                    AllowRelayPress = false,
                };
                _dlg.Closed += (s, e) => Closed?.Invoke(this, EventArgs.Empty);
            }

            // 相对位置显示
            if (Kit.IsPhoneUI)
            {
                if (Placement == MenuPosition.Default)
                {
                    _dlg.PhonePlacement = DlgPlacement.FromBottom;
                }
                else
                {
                    _dlg.PlacementTarget = p_tgtPlacement ?? ContextTarget;
                    _dlg.PhonePlacement = (DlgPlacement)Placement + 5;
                }
            }
            else if (Placement != MenuPosition.Default)
            {
                _dlg.PlacementTarget = p_tgtPlacement ?? ContextTarget;
                _dlg.WinPlacement = (DlgPlacement)Placement + 5;
            }
            _dlg.Show();

            if (!Kit.IsPhoneUI && Placement == MenuPosition.Default)
            {
                // 计算显示位置
                double width = _dlg.DesiredSize.Width;
                double height = _dlg.DesiredSize.Height;
                _dlg.Left = (p_pos.X + width > Kit.ViewWidth) ? Math.Floor(p_pos.X - width) : p_pos.X;
                _dlg.Top = (p_pos.Y + height > Kit.ViewHeight) ? Math.Floor(p_pos.Y - height) : p_pos.Y;
            }

            Focus(FocusState.Programmatic);
            Opened?.Invoke(this, EventArgs.Empty);
            return true;
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

            if (TriggerEvent == TriggerEvent.RightTapped)
                tgt.AddHandler(RightTappedEvent, (RightTappedEventHandler)OnTargetRightTapped, true);
            else if (TriggerEvent == TriggerEvent.LeftTapped)
                tgt.AddHandler(TappedEvent, (TappedEventHandler)OnTargetTapped, true);
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
        }

        void OnTargetRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!Kit.IsPhoneUI && Placement == MenuPosition.Default)
                _ = OpenContextMenu(e.GetPosition(null));
            else
                _ = OpenContextMenu();
        }

        void OnTargetTapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!Kit.IsPhoneUI && Placement == MenuPosition.Default)
                _ = OpenContextMenu(e.GetPosition(null));
            else
                _ = OpenContextMenu();
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (!Kit.IsPhoneUI && Placement == MenuPosition.Default)
                _ = OpenContextMenu(((Button)sender).GetAbsolutePosition());
            else
                _ = OpenContextMenu();
        }

        void OnTargetUnloaded(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
