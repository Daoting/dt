#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

#endregion

#if ANDROID
using View = Android.Views.View;
#elif IOS
using UIKit;
using View = UIKit.UIView;
#else
using View = Windows.UI.Xaml.UIElement;
#endif

namespace Dt.Base
{
    /// <summary>
    /// 可停靠面板
    /// </summary>
    public partial class DockPanel : Panel
    {
        #region 静态内容
        /// <summary>
        /// 停靠位置依赖项属性
        /// </summary>
        public static readonly DependencyProperty DockProperty = DependencyProperty.RegisterAttached(
            "Win",
            typeof(DockPosition),
            typeof(DockPanel),
            new PropertyMetadata(DockPosition.Left, new PropertyChangedCallback(OnDockChanged)));


        /// <summary>
        /// 获取停靠位置属性值
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static DockPosition GetDock(View element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (DockPosition)element.GetValue(DockProperty);
        }

        /// <summary>
        /// 设置停靠位置属性值
        /// </summary>
        public static void SetDock(UIElement element, DockPosition dock)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(DockProperty, dock);
        }

        static void OnDockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsValidDock(e.NewValue))
            {
                throw new ArgumentException("停靠位置只可设置为：上下左右！");
            }
            FrameworkElement reference = d as FrameworkElement;
            if (reference != null)
            {
                DockPanel parent = reference.Parent as DockPanel;
                if (parent != null)
                {
                    parent.InvalidateMeasure();
                }
            }
        }

        static bool IsValidDock(object o)
        {
            DockPosition dock = (DockPosition)o;
            if (((dock != DockPosition.Left) && (dock != DockPosition.Top)) && (dock != DockPosition.Right))
            {
                return (dock == DockPosition.Bottom);
            }
            return true;
        }

        /// <summary>
        /// 内容区域是否采用填充方式
        /// </summary>
        public static readonly DependencyProperty LastChildFillProperty = DependencyProperty.Register(
            "LastChildFill",
            typeof(bool),
            typeof(DockPanel),
            new PropertyMetadata(true, new PropertyChangedCallback(OnLastChildFillChanged)));

        static void OnLastChildFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockPanel panel = d as DockPanel;
            if (panel != null)
            {
                panel.InvalidateArrange();
            }
        }

        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public DockPanel()
        {
        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            UIElementCollection internalChildren = Children;
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            int num5 = 0;
            int count = internalChildren.Count;
            while (num5 < count)
            {
                var element = internalChildren[num5];
                if (element != null)
                {
                    Size remainingSize = new Size(Math.Max(0.0, availableSize.Width - num3), Math.Max(0.0, availableSize.Height - num4));
#if UWP
                    element.Measure(remainingSize);
                    Size desiredSize = element.DesiredSize;
#else
                    Size desiredSize = MeasureElement(element, remainingSize);
#endif
                    switch (GetDock(element))
                    {
                        case DockPosition.Left:
                        case DockPosition.Right:
                            num2 = Math.Max(num2, num4 + desiredSize.Height);
                            num3 += desiredSize.Width;
                            break;

                        case DockPosition.Top:
                        case DockPosition.Bottom:
                            num = Math.Max(num, num3 + desiredSize.Width);
                            num4 += desiredSize.Height;
                            break;
                    }
                }
                num5++;
            }
            return new Size(Math.Max(num, num3), Math.Max(num2, num4));
        }

        /// <summary>
        /// 布局
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElementCollection internalChildren = Children;
            int count = internalChildren.Count;
            int num2 = count - (LastChildFill ? 1 : 0);
            double x = 0.0;
            double y = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            for (int i = 0; i < count; i++)
            {
                var element = internalChildren[i];
                if (element != null)
                {
#if UWP
                    Size desiredSize = element.DesiredSize;
#else
                    Size desiredSize = GetElementDesiredSize(element);
#endif
                    Rect finalRect = new Rect(x, y, Math.Max(0.0, finalSize.Width - (x + num5)), Math.Max(0.0, finalSize.Height - (y + num6)));
                    if (i < num2)
                    {
                        switch (GetDock(element))
                        {
                            case DockPosition.Left:
                                x += desiredSize.Width;
                                finalRect.Width = desiredSize.Width;
                                break;

                            case DockPosition.Top:
                                y += desiredSize.Height;
                                finalRect.Height = desiredSize.Height;
                                break;

                            case DockPosition.Right:
                                num5 += desiredSize.Width;
                                finalRect.X = Math.Max((double)0.0, (double)(finalSize.Width - num5));
                                finalRect.Width = desiredSize.Width;
                                break;

                            case DockPosition.Bottom:
                                num6 += desiredSize.Height;
                                finalRect.Y = Math.Max((double)0.0, (double)(finalSize.Height - num6));
                                finalRect.Height = desiredSize.Height;
                                break;
                        }
                    }
#if UWP
                    element.Arrange(finalRect);
#else
                    ArrangeElement(element, finalRect);
#endif
                }
            }
            return finalSize;
        }

        /// <summary>
        /// 获取设置内容区域是否采用填充方式
        /// </summary>
        public bool LastChildFill
        {
            get { return (bool)GetValue(LastChildFillProperty); }
            set { SetValue(LastChildFillProperty, value); }
        }
    }
}
