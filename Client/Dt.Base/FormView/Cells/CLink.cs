#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-03-05 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 链接格
    /// </summary>
    public partial class CLink : CBar
    {
        #region 构造方法
        public CLink()
        {
            DefaultStyleKey = typeof(CLink);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 点击链接事件
        /// </summary>
#if ANDROID
        new
#endif
        public event EventHandler<EventArgs> Click;
        #endregion

        #region 重写方法
        /// <summary>
        /// 切换内容
        /// </summary>
        protected override void LoadContent()
        {
            if (_root == null)
                return;

            // 为uno节省一级ContentPresenter！
            if (_root.Children.Count > 2)
                _root.Children.RemoveAt(2);

            if (Content is string title)
            {
                TextBlock tb = new TextBlock
                {
                    Text = title,
                    TextWrapping = TextWrapping.NoWrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 10, 0)
                };
                _root.Children.Add(tb);
            }
            else if (Content is FrameworkElement con)
            {
                // 默认边距
                var margin = con.Margin;
                con.Margin = new Thickness(margin.Left + 10, margin.Top + 1, margin.Right, margin.Bottom);
                _root.Children.Add(con);
            }
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            var props = e.GetCurrentPoint(null).Properties;
            if (props.IsLeftButtonPressed)
            {
                if (CapturePointer(e.Pointer))
                {
                    e.Handled = true;
                    VisualStateManager.GoToState(this, "Pressed", true);
                }
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            ReleasePointerCapture(e.Pointer);
            e.Handled = true;

            var pt = e.GetCurrentPoint(null).Position;
            if (this.ContainPoint(pt))
            {
                VisualStateManager.GoToState(this, "PointerOver", true);
                Click?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            e.Handled = true;
            VisualStateManager.GoToState(this, "Normal", true);
        }
        #endregion
    }
}