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
using System.Net;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
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
                parent = parent.GetParent() as FrameworkElement;
            }
            return null;
        }

        /// <summary>
        /// 在UI元素第一次Loaded事件后调用Action方法，只调用一次
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_action"></param>
        public static void FirstLoaded(this FrameworkElement source, Action p_action)
        {
            if (source == null || p_action == null)
                return;

            RoutedEventHandler handler = null;
            handler = (sender, e) =>
            {
                FrameworkElement elem = sender as FrameworkElement;
                if (elem != null)
                {
                    elem.Loaded -= handler;
                    if (elem.Dispatcher.HasThreadAccess)
                        p_action();
                    else
                        _ = elem.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(p_action));
                }
            };
            source.Loaded += handler;
        }

        /// <summary>
        /// 判断某点是否在元素区域内部
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_point">Point</param>
        /// <returns>true 表包含该点</returns>
        public static bool ContainPoint(this FrameworkElement source, Point p_point)
        {
            if (source == null)
                return false;

            try
            {
                bool result = false;
                MatrixTransform trans = source.TransformToVisual(null) as MatrixTransform;
                if (trans != null)
                {
                    double offsetX = trans.Matrix.OffsetX;
                    double offsetY = trans.Matrix.OffsetY;
                    if (p_point.X >= offsetX
                        && p_point.X <= offsetX + source.ActualWidth
                        && p_point.Y >= offsetY
                        && p_point.Y <= offsetY + source.ActualHeight)
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
        /// 设置绑定，简化写法
        /// </summary>
        /// <param name="target">目标元素</param>
        /// <param name="p_dp">目标依赖属性</param>
        /// <param name="p_source">绑定源</param>
        /// <param name="p_path">源路径</param>
        public static void Bind(this FrameworkElement target, DependencyProperty p_dp, object p_source, string p_path)
        {
            if (target != null)
                target.SetBinding(p_dp, new Binding { Path = new PropertyPath(p_path), Source = p_source });
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
        /// 获取当前FrameworkElement相对于relativeTo的边界矩形
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_relativeTo">基准元素</param>
        /// <returns>矩形</returns>
        public static Rect GetBounds(this FrameworkElement source, FrameworkElement p_relativeTo = null)
        {
            try
            {
                var trans = source.TransformToVisual(p_relativeTo ?? SysVisual.RootContent) as MatrixTransform;
                if (trans != null)
                    return new Rect(trans.Matrix.OffsetX, trans.Matrix.OffsetY, source.ActualWidth, source.ActualHeight);
            }
            catch { }
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
