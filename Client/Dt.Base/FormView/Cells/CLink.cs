#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-03-05 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
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
        public event Action<CLink> Click;

        /// <summary>
        /// 点击事件，无事件参数，方便复用处理方法
        /// </summary>
        public event Action Call;
        #endregion

        #region 重写方法
        /// <summary>
        /// 切换内容
        /// </summary>
        protected override void LoadContent()
        {
            Grid root = (Grid)GetTemplateChild("RootGrid");
            if (root == null)
                return;

            // 为uno节省一级ContentPresenter！
            if (root.Children.Count > 2)
                root.Children.RemoveAt(2);

            string title = !string.IsNullOrEmpty(Title) ? Title : Content as string;
            if (!string.IsNullOrEmpty(title))
            {
                TextBlock tb = new TextBlock
                {
                    Text = title,
                    TextWrapping = TextWrapping.NoWrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 10, 0)
                };
                root.Children.Add(tb);
            }
            else if (Content is FrameworkElement con)
            {
                // 默认边距
                var margin = con.Margin;
                con.Margin = new Thickness(margin.Left + 10, margin.Top + 1, margin.Right, margin.Bottom);
                root.Children.Add(con);
            }
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            var props = e.GetCurrentPoint(null).Properties;
            if (props.IsLeftButtonPressed && Owner != null)
            {
                VisualStateManager.GoToState(this, "Pressed", true);
                Owner.OnCellClick(this, e);
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            Click?.Invoke(this);
            Call?.Invoke();
        }
        #endregion
    }
}