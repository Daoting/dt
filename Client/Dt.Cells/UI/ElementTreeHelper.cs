#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal static class ElementTreeHelper
    {
        public static bool FocusChild(DependencyObject parent)
        {
            if (parent != null)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < childrenCount; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null)
                    {
                        Control @this = child as Control;
                        if ((@this != null) && @this.IsTabStop)
                        {
                            return @this.Focus(FocusState.Programmatic);
                        }
                    }
                    if (FocusChild(child))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static DependencyObject GetKeyboardFocusedElement()
        {
            return (FocusManager.GetFocusedElement() as DependencyObject);
        }

        public static DependencyObject GetLogicFocusedElement(UIElement container)
        {
            return GetKeyboardFocusedElement();
        }

        public static DependencyObject GetLogicFocusedElement(UIElement container, out DependencyObject scope)
        {
            scope = null;
            return GetKeyboardFocusedElement();
        }

        public static DependencyObject GetParent(DependencyObject element)
        {
            TextElement element2 = element as TextElement;
            if (element2 != null)
            {
                if (element2.ElementStart == null)
                {
                    return null;
                }
                return element2.ElementStart.Parent;
            }
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if ((parent == null) && (element is FrameworkElement))
            {
                DependencyObject obj3 = ((FrameworkElement) element).Parent;
                if (obj3 is Windows.UI.Xaml.Controls.Primitives.Popup)
                {
                    return obj3;
                }
            }
            return parent;
        }

        public static T GetParentOrSelf<T>(DependencyObject element) where T: class
        {
            T local = default(T);
            while ((local == null) && (element != null))
            {
                local = element as T;
                element = GetParent(element);
            }
            return local;
        }

        public static IEnumerable<T> GetVisualChildren<T>(DependencyObject element) where T: DependencyObject
        {
            List<T> children = new List<T>();
            GetVisualChildren<T>(element, children);
            return (IEnumerable<T>) children;
        }

        static void GetVisualChildren<T>(DependencyObject element, List<T> children) where T: DependencyObject
        {
            if (element != null)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < childrenCount; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(element, i);
                    if (child is T)
                    {
                        children.Add((T) child);
                    }
                    else
                    {
                        GetVisualChildren<T>(child, children);
                    }
                }
            }
        }

        public static T GetVisualParentOrSelf<T>(DependencyObject element)
            where T: class
        {
            T local = default(T);
            while ((local == null) && (element != null))
            {
                local = element as T;
                element = VisualTreeHelper.GetParent(element);
            }
            return local;
        }

        public static T HitTest<T>(UIElement container, ref Windows.Foundation.Point position) where T: UIElement
        {
            position = container.TransformToVisual(Window.Current.Content).TransformPoint(position);
            T visual = Enumerable.FirstOrDefault<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(position, container), delegate (UIElement e) {
                return e is T;
            }) as T;
            position = Window.Current.Content.TransformToVisual(visual).TransformPoint(position);
            return visual;
        }

        public static bool IsAncestorOf(this UIElement @this, DependencyObject element)
        {
            while (element != null)
            {
                if (element == @this)
                {
                    return true;
                }
                element = GetParent(element);
            }
            return false;
        }

        public static bool IsFocused(Control targetElement)
        {
            return (FocusManager.GetFocusedElement() == targetElement);
        }

        public static bool IsKeyboardFocusWithin(UIElement targetElement)
        {
            for (DependencyObject obj2 = GetKeyboardFocusedElement(); obj2 != null; obj2 = GetParent(obj2))
            {
                if (object.ReferenceEquals(obj2, targetElement))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsLogicFocusWithin(UIElement targetElement)
        {
            return IsKeyboardFocusWithin(targetElement);
        }

        public static bool IsLogicFocusWithin(UIElement targetElement, out DependencyObject scope)
        {
            scope = null;
            return IsKeyboardFocusWithin(targetElement);
        }

        public static void TransferFocusToDescendent(UIElement container, Control target)
        {
            if (GetKeyboardFocusedElement() == container)
            {
                target.Focus(FocusState.Programmatic);
            }
        }
    }
}

