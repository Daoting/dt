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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// UIElement扩展类
    /// </summary>
    public static class UIElementExt
    {
        #region 查询子元素
        /// <summary>
        /// 查询给定类型的第一个子元素
        /// </summary>
        /// <typeparam name="T">要查询的子元素类型</typeparam>
        /// <param name="source">要查询的起点元素</param>
        /// <param name="p_checkItself">是事包含当前元素</param>
        /// <returns>第一个符合类型的子元素</returns>
        public static T FindChildByType<T>(this UIElement source, bool p_checkItself = false)
            where T : class
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
        public static IEnumerable<T> FindChildrenByType<T>(this UIElement source, bool p_checkItself = false)
            where T : class
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
        public static UIElement FindChildByName(this UIElement source, string p_name, bool p_checkItself = false)
        {
            return (from item in source.FindChildren(p_checkItself)
                    where item is FrameworkElement elem && elem.Name.Equals(p_name)
                    select item).FirstOrDefault();
        }

        /// <summary>
        /// 根据给定子元素的Tag串查询
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <param name="p_tag">子元素的Tag串</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <returns>返回第一个符合条件的子元素</returns>
        public static UIElement FindChildByTag(this UIElement source, string p_tag, bool p_checkItself = false)
        {
            if (string.IsNullOrEmpty(p_tag))
                return null;

            return (from item in source.FindChildren(p_checkItself)
                    where item is FrameworkElement elem && p_tag.Equals(elem.Tag)
                    select item).FirstOrDefault();
        }

        /// <summary>
        /// 获取给定元素的子元素列表，为提高效率只查询出VisualTreeHelper给出的子元素
        /// 转为非递归调用
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <returns>返回所有子元素列表</returns>
        public static IEnumerable<UIElement> FindChildren(this UIElement source, bool p_checkItself = false)
        {
            if (p_checkItself)
                yield return source;

            Queue<UIElement> qu = new Queue<UIElement>();
            qu.Enqueue(source);
            while (qu.Count > 0)
            {
                var item = qu.Dequeue();
                int childrenCount = VisualTreeHelper.GetChildrenCount(item);
                if (childrenCount > 0)
                {
                    // 通过系统的可视树查询
                    for (int childIndex = 0; childIndex < childrenCount; childIndex++)
                    {
                        var child = VisualTreeHelper.GetChild(item, childIndex) as UIElement;
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
        public static T FindParentByType<T>(this UIElement source, UIElement p_endParent = null, bool p_checkItself = false)
            where T : class
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
        public static IEnumerable<T> FindParentsByType<T>(this UIElement source, UIElement p_endParent = null, bool p_checkItself = false)
            where T : class
        {
            T tgt;
            if (p_checkItself && (tgt = source as T) != null)
                yield return tgt;

            var parent = source;
            do
            {
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
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
        /// 从父容器中移除当前元素
        /// </summary>
        /// <param name="source"></param>
        public static void ClearParent(this UIElement source)
        {
            var parent = VisualTreeHelper.GetParent(source) as UIElement;
            if (parent == null)
                return;

            if (parent is Panel panel)
            {
                panel.Children.Remove(source);
            }
            else if (parent is ContentPresenter pre)
            {
                pre.Content = null;
            }
            else if (parent is ContentControl con)
            {
                con.Content = null;
            }
        }

        /// <summary>
        /// 根据给定子元素的名称的查询，注意有部分名称在加载时空，如没被选择的TabItem内容
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <param name="p_checkItself">结果中是否包含自已</param>
        /// <param name="p_name">要查询的父元素名称</param>
        /// <returns>返回第一个符合条件的父元素</returns>
        public static UIElement FindParentByName(this UIElement source, string p_name, bool p_checkItself = false)
        {
            return (from item in source.FindParents(null, p_checkItself)
                    where item is FrameworkElement elem && elem.Name == p_name
                    select item).FirstOrDefault();
        }

        /// <summary>
        /// 获取当前元素的所有父元素
        /// </summary>
        /// <param name="source">要查询的元素</param>
        /// <param name="p_endParent">终点父元素</param>
        /// <param name="p_checkItself">结果中是否包含自己</param>
        /// <returns>返回所有父元素列表</returns>
        public static IEnumerable<UIElement> FindParents(this UIElement source, UIElement p_endParent = null, bool p_checkItself = false)
        {
            if (p_checkItself)
                yield return source;

            var parent = source;
            do
            {
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
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
        public static UIElement GetParent(this UIElement source)
        {
            if (source != null)
                return VisualTreeHelper.GetParent(source) as UIElement;
            return null;
        }

        /// <summary>
        /// 在Win内查询第一个匹配类型的父元素
        /// </summary>
        /// <typeparam name="T">父元素类型</typeparam>
        /// <param name="source"></param>
        /// <returns>找到返回父元素，否则返回 null</returns>
        public static T FindParentInWin<T>(this UIElement source)
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

        #region 焦点
        /// <summary>
        /// 判断焦点是否在内部
        /// </summary>
        /// <param name="source"></param>
        /// <returns>true 在内部</returns>
        public static bool IsFocusInside(this UIElement source)
        {
            var focusedElement = FocusManager.GetFocusedElement() as UIElement;
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
        public static bool IsFocused(this UIElement source)
        {
            return ((source != null) && (source == FocusManager.GetFocusedElement()));
        }
        #endregion

        #region 截图
        ///// <summary>
        ///// 获取当前界面元素的截图，uno不支持RenderTargetBitmap，废弃
        ///// </summary>
        ///// <param name="p_element"></param>
        ///// <returns></returns>
        //public static async Task<RenderTargetBitmap> GetSnapshot(this UIElement p_element)
        //{
        //    if (p_element == null)
        //        return null;

        //    var parent = VisualTreeHelper.GetParent(p_element);
        //    if (parent == null)
        //        SysVisual.InvisibleGrid.Children.Add(p_element);
        //    RenderTargetBitmap bmp = new RenderTargetBitmap();
        //    await bmp.RenderAsync(p_element);
        //    if (parent == null)
        //        SysVisual.InvisibleGrid.Children.Remove(p_element);
        //    return bmp;
        //}

        /// <summary>
        /// 保存当前界面元素的png截图
        /// </summary>
        /// <param name="p_element">要截图的界面元素</param>
        /// <param name="p_fileName">要保存的文件名</param>
        /// <param name="p_autoSave">false显示文件对话框，true且p_fileName不为空时自动保存</param>
        /// <param name="p_bounds">裁剪区域</param>
        /// <returns></returns>
        public static async Task<StorageFile> SaveSnapshot(
            this UIElement p_element,
            string p_fileName = null,
            bool p_autoSave = false,
            Rect p_bounds = default(Rect))
        {
            if (p_element == null)
                return null;

            string fileName = "";
            RenderTargetBitmap bmp = new RenderTargetBitmap();
            await bmp.RenderAsync(p_element);
            var pixelBuffer = await bmp.GetPixelsAsync();
            StorageFile saveFile = null;
            if (p_autoSave)
            {
                fileName = string.IsNullOrEmpty(p_fileName) ? DateTime.Now.ToString("yyMMdd_hhmmss") + "snapshot.png" : p_fileName.ToLower();
                if (fileName.IndexOf(".png") < 1)
                    fileName += ".png";

                try
                {
                    var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(Kit.Stub.Title, CreationCollisionOption.OpenIfExists);
                    saveFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                }
                catch
                {
                    Kit.Error("无访问图片库权限，保存失败！");
                    return null;
                }
            }
            else
            {
                fileName = string.IsNullOrEmpty(p_fileName) ? "snapshot.png" : p_fileName;
                var savePicker = new FileSavePicker();
                savePicker.DefaultFileExtension = ".png";
                savePicker.FileTypeChoices.Add(".png", new List<string> { ".png" });
                savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                savePicker.SuggestedFileName = fileName;
                saveFile = await savePicker.PickSaveFileAsync();

                if (saveFile == null)
                    return null;
            }

            using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var di = DisplayInformation.GetForCurrentView();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                // 含裁剪区域
                if (p_bounds.Width > 0 && p_bounds.Height > 0)
                {
                    // 换算成物理像素
                    double raw = di.RawPixelsPerViewPixel;
                    BitmapBounds bb = new BitmapBounds();
                    bb.X = (uint)Math.Ceiling(p_bounds.Left * raw);
                    bb.Y = (uint)Math.Ceiling(p_bounds.Top * raw);
                    bb.Width = (uint)Math.Floor(p_bounds.Width * raw);
                    bb.Height = (uint)Math.Floor(p_bounds.Height * raw);
                    encoder.BitmapTransform.Bounds = bb;
                }

                float dpi = di.LogicalDpi;
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)bmp.PixelWidth,
                    (uint)bmp.PixelHeight,
                    dpi,
                    dpi,
                    pixelBuffer.ToArray());
                await encoder.FlushAsync();
            }
            return saveFile;
        }
        #endregion

        /// <summary>
        /// 判断source是否为p_element的祖先元素
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_element"></param>
        /// <returns></returns>
        public static bool IsAncestorOf(this UIElement source, DependencyObject p_element)
        {
            if (p_element == null)
            {
                return false;
            }

            if (source == p_element)
            {
                return true;
            }

            DependencyObject parent = p_element;
            do
            {
                FrameworkElement feParent = parent as FrameworkElement;
                if (feParent != null && feParent.Parent != null)
                {
                    parent = feParent.Parent;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                if (parent == source)
                {
                    return true;
                }
            }
            while (parent != null);
            return false;
        }

        /// <summary>
        /// 判断p_target是否为p_element的祖先元素且p_element可视
        /// 若p_element所在的TabItem未选中则不可视
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_element"></param>
        /// <returns></returns>
        public static bool IsAncestorOfAndVisible(this UIElement source, DependencyObject p_element)
        {
            if (p_element == null)
            {
                return false;
            }

            if (source == p_element)
            {
                return true;
            }

            UIElement parent = p_element as UIElement;
            while (parent != null)
            {
                FrameworkElement feParent = parent as FrameworkElement;
                if (feParent != null && feParent.Parent != null && feParent.Parent is UIElement)
                {
                    parent = feParent.Parent as UIElement;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }

                if (parent != null && parent.Visibility == Visibility.Collapsed)
                    return false;

                if (parent == source)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取UIElement的绝对位置
        /// </summary>
        /// <param name="source"></param>
        /// <returns>坐标值</returns>
        public static Point GetAbsolutePosition(this UIElement source)
        {
            Point pt = new Point();
            MatrixTransform mat = source.TransformToVisual(null) as MatrixTransform;
            if (mat != null)
            {
                pt.X = mat.Matrix.OffsetX;
                pt.Y = mat.Matrix.OffsetY;
            }
            return pt;
        }

        /// <summary>
        /// 获取UIElement的相对位置
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_relative">相对于UIElement</param>
        /// <returns>坐标值</returns>
        public static Point GetRelativePosition(this UIElement source, UIElement p_relative)
        {
            Point pt = new Point();
            MatrixTransform mat = source.TransformToVisual(p_relative) as MatrixTransform;
            if (mat != null)
            {
                pt.X = mat.Matrix.OffsetX;
                pt.Y = mat.Matrix.OffsetY;
            }
            return pt;
        }

        /// <summary>
        /// 获取UIElement的ToolTip对象
        /// </summary>
        /// <param name="source"></param>
        /// <returns>ToolTip对象</returns>
        public static object GetToolTip(this UIElement source)
        {
            return ToolTipService.GetToolTip(source);
        }

        /// <summary>
        /// 设置UIElement的ToolTip对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_value">ToolTip对象</param>
        public static void SetToolTip(this UIElement source, object p_value)
        {
            ToolTipService.SetToolTip(source, p_value);
        }

        /// <summary>
        /// 获取UIElement是否可见
        /// </summary>
        /// <param name="source"></param>
        /// <returns>true 可见</returns>
        public static bool IsVisible(this UIElement source)
        {
            return (source.Visibility == Visibility.Visible);
        }

        /// <summary>
        /// 设置UIElement是否可见
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_visible">true 可见</param>
        public static void SetVisible(this UIElement source, bool p_visible)
        {
            source.Visibility = p_visible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取UIElement位置转换矩阵是否为默认的
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool HasDefaultRenderTransform(this UIElement source)
        {
            return ((source.RenderTransform == null)
                || ((source.RenderTransform is MatrixTransform) && ((MatrixTransform)source.RenderTransform).Matrix.IsIdentity));
        }

        /// <summary>
        /// 判断键盘焦点是否在当前元素中
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsKeyboardFocusWithin(this UIElement element)
        {
            DependencyObject focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
            return ((focusedElement != null) && element.IsAncestorOf(focusedElement));
        }

        /// <summary>
        /// 克隆Image
        /// </summary>
        static Image CloneImageElement(Image originalImage)
        {
            Image clonedImage = new Image();
            clonedImage.Width = originalImage.ActualWidth;
            clonedImage.Height = originalImage.ActualHeight;
            clonedImage.VerticalAlignment = VerticalAlignment.Stretch;
            clonedImage.Source = originalImage.Source;
            return clonedImage;
        }
    }
}
