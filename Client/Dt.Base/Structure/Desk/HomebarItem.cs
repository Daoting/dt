#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-13 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
#if UWP
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ToggleSelectedState();
        }
#endif

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            VisualStateManager.GoToState(this, _win.IsActived ? "Selected" : "PointerOver", true);
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
            e.Handled = true;

            Point pt = e.GetCurrentPoint(null).Position;
            if (this.ContainPoint(pt) && !_win.IsActived)
            {
                // 释放时鼠标在内部
                Desktop.Inst.MainWin = _win;
            }
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            ToggleSelectedState();
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
