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
        public static DependencyObject GetKeyboardFocusedElement()
        {
            return (FocusManager.GetFocusedElement() as DependencyObject);
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
    }
}

