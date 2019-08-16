#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 
    /// </summary>
    internal static class DragDropHelper
    {
        /// <summary>
        /// 开始拖动
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        public static void StartDrag(this UIElement target, PointerRoutedEventArgs e)
        {
            target.RaiseEvent(new DragInfoEventArgs(Tab.DragStartedEvent, e));
        }

        public static void DragDelta(this UIElement target, PointerRoutedEventArgs e)
        {
            target.RaiseEvent(new DragInfoEventArgs(Tab.DragDeltaEvent, e));
        }

        public static void EndDrag(this UIElement target, PointerRoutedEventArgs e)
        {
            target.RaiseEvent(new DragInfoEventArgs(Tab.DragCompletedEvent, e));
        }

        static UIElement GetDragRoot(this UIElement target)
        {
            for (UIElement root = target; root != null; root = VisualTreeHelper.GetParent(root) as UIElement)
            {
                target = root;
                if (GetIsPopupDragRoot(target))
                {
                    return target;
                }
            }
            return target;
        }

        static readonly DependencyProperty IsPopupDragRootProperty = DependencyProperty.RegisterAttached("IsPopupDragRoot", typeof(bool), typeof(DragDropHelper), null);

        internal static bool GetIsPopupDragRoot(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPopupDragRootProperty);
        }

        internal static void SetIsPopupDragRoot(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPopupDragRootProperty, value);
        }
    }
}

