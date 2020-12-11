#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 类似StackPanel布局，增加附加依赖项属性
    /// 内部子元素为 Tabs 或 Pane
    /// </summary>
    public partial class TabItemPanel : Panel
    {
        #region 静态内容
        internal static readonly DependencyProperty SplitterChangeProperty = DependencyProperty.RegisterAttached(
            "SplitterChange", 
            typeof(double), 
            typeof(TabItemPanel), 
            null);

        internal static double GetSplitterChange(DependencyObject obj)
        {
            return (double)obj.GetValue(SplitterChangeProperty);
        }

        internal static void SetSplitterChange(DependencyObject obj, double value)
        {
            obj.SetValue(SplitterChangeProperty, value);
        }

        /// <summary>
        /// 子元素的排序方式
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(TabItemPanel),
            new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TabItemPanel).InvalidateMeasure();
        }
        #endregion

        readonly List<ChildSize> _children = new List<ChildSize>();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            _children.Clear();
            int end = Children.Count;
            for (int i = 0; i < end; i++)
            {
                FrameworkElement element = Children[i] as FrameworkElement;
                if (element != null)
                {
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        _children.Add(new ChildSize(element, IsHorizontal));
                    }
                    else
                    {
                        element.Measure(availableSize);
                    }
                }
            }

            Size measureSize = availableSize;
            double withoutChange = 0.0;
            double withChangeSet = 0.0;
            double changesSum = 0.0;
            foreach (var child in _children)
            {
                double change = GetSplitterChange(child.Element);
                if (change == 0.0)
                {
                    withoutChange += child.Length;
                }
                else
                {
                    changesSum += change;
                    withChangeSet += child.Length;
                }
            }

            Size desiredSize = new Size();
            double availableLength = IsHorizontal ? availableSize.Width : availableSize.Height;
            bool isAvailableInfinity = double.IsInfinity(availableLength);
            foreach (var child in _children)
            {
                double length;
                double change = GetSplitterChange(child.Element);
                if (change == 0.0)
                {
                    if (isAvailableInfinity)
                    {
                        length = child.Length;
                    }
                    else
                    {
                        length = child.Length *(availableLength / (withoutChange + withChangeSet));
                    }
                }
                else if (isAvailableInfinity)
                {
                    length = child.Length;
                }
                else
                {
                    length = ((change * withChangeSet) / changesSum) * (availableLength / (withoutChange + withChangeSet));
                }

                if (IsHorizontal)
                {
                    measureSize.Width = length;
                }
                else
                {
                    measureSize.Height = length;
                }

                child.Element.Measure(measureSize);

                if (IsHorizontal)
                {
                    desiredSize.Width += length;
                    desiredSize.Height = Math.Max(desiredSize.Height, child.Element.DesiredSize.Height);
                }
                else
                {
                    desiredSize.Height += length;
                    desiredSize.Width = Math.Max(desiredSize.Width, child.Element.DesiredSize.Width);
                }
            }

            availableLength = IsHorizontal ? availableSize.Width : availableSize.Height;
            if (!isAvailableInfinity)
            {
                withoutChange = IsHorizontal ? desiredSize.Width : desiredSize.Height;
                double diff = availableLength - withoutChange;
                if (diff <= 0.0)
                {
                    return desiredSize;
                }
                if (IsHorizontal)
                {
                    desiredSize.Width = availableSize.Width;
                    return desiredSize;
                }
                desiredSize.Height = availableSize.Height;
            }
            return desiredSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double changesSum = 0.0;
            double relativeLengthSum = 0.0;
            double relativeLengthWithChangeSet = 0.0;
            double finalLength = IsHorizontal ? finalSize.Width : finalSize.Height;
            Rect finalRect = new Rect(new Point(), finalSize);
            foreach (var child in _children)
            {
                double change = GetSplitterChange(child.Element);
                relativeLengthSum += child.Length;
                if (change != 0.0)
                {
                    changesSum += change;
                    relativeLengthWithChangeSet += child.Length;
                }
            }

            double coef = finalLength / relativeLengthSum;
            double coef2 = relativeLengthWithChangeSet / changesSum;
            foreach (var child in _children)
            {
                double length;
                double change = GetSplitterChange(child.Element);
                
                if (change == 0.0)
                {
                    length = child.Length * coef;
                }
                else
                {
                    length = (change * coef2) * coef;
                }

                if (IsHorizontal)
                {
                    finalRect.Width = length;
                }
                else
                {
                    finalRect.Height = length;
                }
                child.Element.Arrange(finalRect);

                if (IsHorizontal)
                {
                    finalRect.X += child.Element.RenderSize.Width;
                }
                else
                {
                    finalRect.Y += child.Element.RenderSize.Height;
                }
            }
            return finalSize;
        }
        
        /// <summary>
        /// 获取设置子元素的排序方式
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        internal Pane Owner { get; set; }

        bool IsHorizontal
        {
            get { return (Orientation == Orientation.Horizontal); }
        }

        /// <summary>
        /// 获取元素在不同排序方式下的大小
        /// </summary>
        /// <param name="p_child"></param>
        /// <param name="p_isHor"></param>
        /// <returns></returns>
        internal static double GetLength(FrameworkElement p_child, bool p_isHor)
        {
            double length = 0;
            Tabs sect;
            Pane dockItem;
            if ((sect = p_child as Tabs) != null)
            {
                length = p_isHor ? sect.InitWidth : sect.InitHeight;
            }
            else if ((dockItem = p_child as Pane) != null)
            {
                length = p_isHor ? dockItem.InitWidth : dockItem.InitHeight;
            }
            return length;
        }

        class ChildSize
        {
            public ChildSize(FrameworkElement p_child, bool p_isHor)
            {
                Element = p_child;
                Length = GetLength(p_child, p_isHor);
            }

            public FrameworkElement Element { get; set; }

            public double Length { get; set; }
        }
    }
}
