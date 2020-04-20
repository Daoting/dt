#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// FrameworkElement扩展类
    /// </summary>
    public static class FrameworkElementExt
    {
        #region 查询子元素
        /// <summary>
        /// 查询给定类型的第一个子元素
        /// </summary>
        /// <typeparam name="T">要查询的子元素类型</typeparam>
        /// <param name="source">要查询的起点元素</param>
        /// <param name="p_checkItself">是事包含当前元素</param>
        /// <returns>第一个符合类型的子元素</returns>
        public static T FindChildByType<T>(this FrameworkElement source, bool p_checkItself = false)
            where T : FrameworkElement
        {
            return (from item in source.FindChildren(p_checkItself)
                    let elem = item as T
                    where elem != null
                    select elem).FirstOrDefault();
        }

        /// <summary>
        /// 查询给定类型的所有子元素
        /// </summary>
        /// <typeparam name="T">要查询的子元素类型</typeparam>
        /// <param name="source">要查询的起点元素</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <returns>所有符合类型子元素</returns>
        public static IEnumerable<T> FindChildrenByType<T>(this FrameworkElement source, bool p_checkItself = false)
            where T : FrameworkElement
        {
            return (from item in source.FindChildren(p_checkItself)
                    let elem = item as T
                    where elem != null
                    select elem).Distinct();
        }

        /// <summary>
        /// 根据给定名称的子元素，注意有部分名称在加载时空，如没被选择的TabItem内容
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <param name="p_name">要查询的子元素名称</param>
        /// <returns>返回第一个符合条件的子元素</returns>
        public static FrameworkElement FindChildByName(this FrameworkElement source, string p_name, bool p_checkItself = false)
        {
            return (from item in source.FindChildren(p_checkItself)
                    where item.Name.Equals(p_name)
                    select item).FirstOrDefault();
        }

        /// <summary>
        /// 根据给定子元素的Tag串查询
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <param name="p_tag">子元素的Tag串</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <returns>返回第一个符合条件的子元素</returns>
        public static FrameworkElement FindChildByTag(this FrameworkElement source, string p_tag, bool p_checkItself = false)
        {
            if (string.IsNullOrEmpty(p_tag))
                return null;
            return (from item in source.FindChildren(p_checkItself)
                    where p_tag.Equals(item.Tag)
                    select item).FirstOrDefault();
        }

        /// <summary>
        /// 获取给定元素的子元素列表，为提高效率只查询出VisualTreeHelper给出的子元素
        /// 转为非递归调用
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <returns>返回所有子元素列表</returns>
        public static IEnumerable<FrameworkElement> FindChildren(this FrameworkElement source, bool p_checkItself = false)
        {
            if (p_checkItself)
                yield return source;

            Queue<FrameworkElement> qu = new Queue<FrameworkElement>();
            qu.Enqueue(source);
            while (qu.Count > 0)
            {
                FrameworkElement item = qu.Dequeue();
                int childrenCount = VisualTreeHelper.GetChildrenCount(item);
                if (childrenCount > 0)
                {
                    // 通过系统的可视树查询
                    for (int childIndex = 0; childIndex < childrenCount; childIndex++)
                    {
                        FrameworkElement child = VisualTreeHelper.GetChild(item, childIndex) as FrameworkElement;
                        if (child != null)
                        {
                            qu.Enqueue(child);
                            yield return child;
                        }
                    }
                }
            }
        }
        #endregion

        #region 查询父元素
        /// <summary>
        /// 在可视树向上查询第一个匹配类型的父元素
        /// 转为非递归调用，能查询出所有的可视父元素
        /// </summary>
        /// <typeparam name="T">
        /// 父元素类型
        /// </typeparam>
        /// <param name="source">起点元素</param>
        /// <param name="p_endParent">终点父元素</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <returns>找到返回父元素，否则返回 null.</returns>
        public static T FindParentByType<T>(this FrameworkElement source, FrameworkElement p_endParent = null, bool p_checkItself = false)
            where T : FrameworkElement
        {
            return (from item in source.FindParentsByType<T>(p_endParent, p_checkItself)
                    select item).FirstOrDefault();
        }

        /// <summary>
        /// 查询给定类型的所有父元素
        /// </summary>
        /// <typeparam name="T">父元素类型</typeparam>
        /// <param name="source">起点元素</param>
        /// <param name="p_endParent">终点父元素</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <returns></returns>
        public static IEnumerable<T> FindParentsByType<T>(this FrameworkElement source, FrameworkElement p_endParent = null, bool p_checkItself = false)
            where T : FrameworkElement
        {
            T tgt;
            if (p_checkItself && (tgt = source as T) != null)
                yield return tgt;

            FrameworkElement parent = source;
            do
            {
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
                if (parent == null || parent == p_endParent)
                {
                    break;
                }

                tgt = parent as T;
                if (tgt != null)
                {
                    yield return tgt;
                }
            }
            while (true);
        }

        /// <summary>
        /// 根据给定子元素的名称的查询，注意有部分名称在加载时空，如没被选择的TabItem内容
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <param name="p_name">要查询的父元素名称</param>
        /// <returns>返回第一个符合条件的父元素</returns>
        public static FrameworkElement FindParentByName(this FrameworkElement source, string p_name, bool p_checkItself = false)
        {
            return (from item in source.FindParents(null, p_checkItself)
                    where item.Name == p_name
                    select item).FirstOrDefault();
        }

        /// <summary>
        /// 获取当前元素的所有父元素
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <param name="p_endParent">终点父元素</param>
        /// <param name="p_checkItself">结果中是否包含自己</param>
        /// <returns>返回所有父元素列表</returns>
        public static IEnumerable<FrameworkElement> FindParents(this FrameworkElement source, FrameworkElement p_endParent = null, bool p_checkItself = false)
        {
            if (p_checkItself)
                yield return source;

            FrameworkElement parent = source;
            do
            {
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
                if (parent == null || parent == p_endParent)
                    break;
                yield return parent;
            }
            while (true);
        }

        /// <summary>
        /// 获取当前元素的父元素
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <returns>找到返回父元素，否则为空</returns>
        public static FrameworkElement GetParent(this FrameworkElement source)
        {
            if (source != null)
                return VisualTreeHelper.GetParent(source) as FrameworkElement;
            return null;
        }

        /// <summary>
        /// 在Win内查询第一个匹配类型的父元素
        /// </summary>
        /// <typeparam name="T">父元素类型</typeparam>
        /// <param name="source"></param>
        /// <returns>找到返回父元素，否则返回 null</returns>
        public static T FindParentInWin<T>(this FrameworkElement source)
        {
            DependencyObject parent = source;
            while (true)
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent == null)
                    break;

                // 也可查询SizedPresenter
                if (parent is T tgt)
                    return tgt;

                // 查询范围SizedPresenter，参见win.xaml：win模式在Tabs定义，phone模式在Tab定义
                if (parent.GetType() == typeof(SizedPresenter))
                    break;
            }
            return default;
        }
        #endregion

        /// <summary>
        /// 按照名称查找和当前元素在同一xaml文件中的元素，比较准确，逼出来的方法！
        /// </summary>
        /// <param name="source">当前元素</param>
        /// <param name="p_name">待查找元素的名称</param>
        /// <returns></returns>
        public static object FindElementByName(this FrameworkElement source, string p_name)
        {
            if (source == null || source.Name == p_name)
                return source;

            // 先找到xaml文件的根元素再查找子节点
            string xaml = WebUtility.UrlDecode(source.BaseUri.Segments[source.BaseUri.Segments.Length - 1].Split('.')[0]);
            var parent = source;
            while (parent != null)
            {
                Type tp = parent.GetType();
                if (tp.Name == xaml)
                    return parent.FindName(p_name);
                parent = parent.GetParent();
            }
            return null;
        }

        /// <summary>
        /// 在UI元素的Loaded事件中调用Dispatcher.RunAsync来执行Action方法，只调用一次
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_action"></param>
        public static void AfterLoad(this FrameworkElement source, Action p_action)
        {
            if (source == null || p_action == null)
                return;

            RoutedEventHandler handler = null;
            handler = async (sender, e) =>
            {
                FrameworkElement elem = sender as FrameworkElement;
                if (elem != null)
                {
                    elem.Loaded -= handler;
                    await elem.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(p_action));
                }
            };
            source.Loaded += handler;
        }

        /// <summary>
        /// 判断焦点是否在内部
        /// </summary>
        /// <param name="source"></param>
        /// <returns>true 在内部</returns>
        public static bool IsFocusInside(this FrameworkElement source)
        {
            FrameworkElement focusedElement = FocusManager.GetFocusedElement() as FrameworkElement;
            while ((focusedElement != null) && (focusedElement != source))
            {
                focusedElement = focusedElement.GetParent();
            }
            return (focusedElement == source);
        }

        /// <summary>
        /// 判断焦点是否在当前元素上，不判断是否在内部！
        /// </summary>
        /// <param name="source"></param>
        /// <returns>true 在当前元素上</returns>
        public static bool IsFocused(this FrameworkElement source)
        {
            return ((source != null) && (source == FocusManager.GetFocusedElement()));
        }

        /// <summary>
        /// 判断某点是否在元素区域内部
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_point">Point</param>
        /// <returns>true 表包含该点</returns>
        public static bool ContainPoint(this FrameworkElement source, Point p_point)
        {
            try
            {
                bool result = false;
                MatrixTransform trans = source.TransformToVisual(null) as MatrixTransform;
                if (trans != null)
                {
                    double offsetX = trans.Matrix.OffsetX;
                    double offsetY = trans.Matrix.OffsetY;
                    if (p_point.X > offsetX
                        && p_point.X < offsetX + source.ActualWidth
                        && p_point.Y > offsetY
                        && p_point.Y < offsetY + source.ActualHeight)
                    {
                        result = true;
                    }
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取FrameworkElement的尺寸
        /// </summary>
        /// <param name="source"></param>
        /// <returns>尺寸</returns>
        public static Size GetSize(this FrameworkElement source)
        {
            return new Size(source.ActualWidth, source.ActualHeight);
        }

        /// <summary>
        /// 从父容器中移除当前元素
        /// </summary>
        /// <param name="source"></param>
        public static void ClearParent(this FrameworkElement source)
        {
            FrameworkElement parent = source.GetParent();
            if (parent == null)
                return;

            Panel panel = parent as Panel;
            if (panel != null)
            {
                panel.Children.Remove(source);
                return;
            }

            ContentPresenter pre = parent as ContentPresenter;
            if (pre != null)
            {
                pre.Content = null;
                return;
            }

            ContentControl con = parent as ContentControl;
            if (con != null)
            {
                con.Content = null;
            }
        }

        /// <summary>
        /// 获取当前FrameworkElement相对于relativeTo的边界矩形
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_relativeTo">基准元素</param>
        /// <returns>矩形</returns>
        public static Rect GetBounds(this FrameworkElement source, FrameworkElement p_relativeTo)
        {
            Point ptMap;
            if (source.TransformToVisual(p_relativeTo).TryTransform(new Point(), out ptMap))
            {
                // 
                double y = (p_relativeTo == null) ? ptMap.Y - SysVisual.StatusBarHeight : ptMap.Y;
                return new Rect(ptMap.X, y, source.ActualWidth, source.ActualHeight);
            }
            return new Rect();
        }

        const ManipulationModes _mode = ManipulationModes.System | ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;

        /// <summary>
        /// 内容加载时自动设置可水平滑屏
        /// </summary>
        /// <param name="source"></param>
        public static void AllowTranslateX(this FrameworkElement source)
        {
            if (source == null)
                return;

            // 设置可水平滑屏
            UIElement ue;
            var sv = source.FindChildByType<ScrollViewer>(true);
            if (sv != null
                && (ue = sv.Content as UIElement) != null
                && ue.ManipulationMode != _mode)
            {
                Panel pnl;
                Control con;
                ue.ManipulationMode = _mode;

                // 设置背景色确保能接收滑屏操作
                if ((pnl = ue as Panel) != null)
                {
                    if (pnl.Background == null)
                        pnl.Background = new SolidColorBrush(Colors.Transparent);
                }
                else if ((con = ue as Control) != null)
                {
                    if (con.Background == null)
                        con.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }
    }
}
