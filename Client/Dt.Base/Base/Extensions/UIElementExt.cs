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
                    var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(AtSys.Stub.Title, CreationCollisionOption.OpenIfExists);
                    saveFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                }
                catch
                {
                    AtKit.Error("无访问图片库权限，保存失败！");
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
        /// 获取控件测量大小的整数值尺寸！（不同于DesiredSize）
        /// </summary>
        /// <param name="source"></param>
        /// <returns>尺寸值</returns>
        public static Size GetDesiredSize(this UIElement source)
        {
            return GetVisualSize(source.DesiredSize);
        }

        /// <summary>
        /// 获取控件最终呈现大小的整数值尺寸！（不同于RenderSize）
        /// </summary>
        /// <param name="source"></param>
        /// <returns>尺寸值</returns>
        public static Size GetRenderSize(this UIElement source)
        {
            return GetVisualSize(source.RenderSize);
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

        /// <summary>
        /// 转换原有的double值到最小整数值尺寸
        /// </summary>
        /// <param name="size">原尺寸</param>
        /// <returns>转换后的尺寸</returns>
        static Size GetVisualSize(Size size)
        {
            return new Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
        }
    }
}
