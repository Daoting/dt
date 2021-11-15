#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.Data
{
    internal static class Utility
    {
        public static FontFamily DefaultFontFamily = new FontFamily(NameConstans.DEFAULT_FONT_FAMILY);
        static PropertyInfo[] infos = null;

        public static MemoryStream CreateMemStream(Stream source)
        {
            if (!source.CanRead)
                return null;

            MemoryStream stream = new MemoryStream();
            byte[] buffer = new byte[0xff];
            int count = 0;
            while ((count = source.Read(buffer, 0, 0xff)) > 0)
            {
                stream.Write(buffer, 0, count);
            }
            stream.Seek(0L, SeekOrigin.Begin);
            return stream;
        }

        internal static string GetFontWeightString(FontWeight fontWeight)
        {
            if (infos == null)
            {
                // hdt
                infos = typeof(FontWeights).GetRuntimeProperties().ToArray<PropertyInfo>();
            }
            for (int i = 0; i < infos.Length; i++)
            {
                object obj2 = infos[i].GetValue(null);
                if ((obj2 != null) && (((FontWeight)obj2).Weight == fontWeight.Weight))
                {
                    return infos[i].Name;
                }
            }
            return "Normal";
        }

        public static ImageFormat GetImageFormat(Uri uriSource)
        {
            if (uriSource != null)
            {
                string str = uriSource.IsAbsoluteUri ? uriSource.LocalPath : uriSource.OriginalString;
                if (!string.IsNullOrEmpty(str))
                {
                    switch (Path.GetExtension(str).ToLower())
                    {
                        case ".bmp":
                            return Dt.Cells.Data.ImageFormat.Bmp;

                        case ".gif":
                            return Dt.Cells.Data.ImageFormat.Gif;

                        case ".jpg":
                        case ".jpeg":
                            return Dt.Cells.Data.ImageFormat.Jpeg;

                        case ".png":
                            return Dt.Cells.Data.ImageFormat.Png;

                        case ".tiff":
                            return Dt.Cells.Data.ImageFormat.Tiff;

                        case ".wmp":
                            return Dt.Cells.Data.ImageFormat.Wmp;

                        case ".unknown":
                            return Dt.Cells.Data.ImageFormat.Unkown;
                    }
                }
            }
            return Dt.Cells.Data.ImageFormat.Unkown;
        }

        public static Stream GetImageStream(ImageSource image, ImageFormat imageFormat, PictureSerializationMode mode)
        {
            //try
            //{
            //    BitmapImage bmp = image as BitmapImage;
            //    if (bmp == null)
            //        return null;

            //    Stream stream = null;
            //    Task loadImageTask = null;
            //    UIAdaptor.InvokeSync(() => loadImageTask = LoadImgAsync(bmp.UriSource, stream));
            //    while (true)
            //    {
            //        if ((loadImageTask.IsCompleted || loadImageTask.IsCanceled) || loadImageTask.IsFaulted)
            //        {
            //            break;
            //        }
            //        new ManualResetEvent(false).WaitOne(50);
            //    }
            //    return stream;
            //}
            //catch { }
            //return null;

            // 功能未用到，hdt
            throw new NotImplementedException("未实现链接地址图片的序列化功能！");
        }

        /// <summary>
        /// hdt 新增，提供报表中图表的导出
        /// </summary>
        /// <param name="p_bmp"></param>
        /// <returns></returns>
        public static Stream GetBmpStream(RenderTargetBitmap p_bmp)
        {
            Task<IBuffer> taskBuf = taskBuf = p_bmp.GetPixelsAsync().AsTask();
            taskBuf.Wait();
            byte[] data = taskBuf.Result.ToArray();

            InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
            Task<BitmapEncoder> taskEncoder = BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms).AsTask();
            taskEncoder.Wait();

            BitmapEncoder encoder = taskEncoder.Result;
            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)p_bmp.PixelWidth,
                    (uint)p_bmp.PixelHeight,
                    dpi,
                    dpi,
                    data);
            encoder.FlushAsync().AsTask().Wait();

            return WindowsRuntimeStreamExtensions.AsStream(ms);
        }

        public static async void InitImageSource(BitmapImage image, Stream imageStream)
        {
            // hdt
            imageStream.Seek(0L, SeekOrigin.Begin);
            MemoryStream ms = CreateMemStream(imageStream);
            InMemoryRandomAccessStream random = new InMemoryRandomAccessStream();
            IOutputStream output = random.GetOutputStreamAt(0L);
            await RandomAccessStream.CopyAsync(ms.AsInputStream(), output);
            image.SetSource(random);
        }

        public static bool IsNumber(double x)
        {
            return (!double.IsInfinity(x) && !double.IsNaN(x));
        }

        async static Task LoadImgAsync(Uri uri, Stream p_stream)
        {
            string originalString = uri.OriginalString;
            if (originalString.StartsWith("ms-appx:/"))
            {
                string relativePath = originalString.Replace("ms-appx:/", "");
                StorageFile storageFile = await GetLocalResourceAsync(relativePath);
                if (storageFile == null)
                    throw new FileNotFoundException("Could not load image.", relativePath);
                p_stream = await storageFile.OpenStreamForReadAsync();
            }
            throw new FileNotFoundException("暂不支持Web路径图片！");
        }

        /// <summary>
        /// hdt  唐忠宝修改
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        static async Task<StorageFile> GetLocalResourceAsync(string relativePath)
        {
            string path = string.Format("Files/{0}", (object[])new object[] { relativePath });
            ResourceMap rm = ResourceManager.Current.MainResourceMap;
            if (rm.ContainsKey(path))
                return await rm.GetValue(path, ResourceContext.GetForCurrentView()).GetValueAsFileAsync();
            return null;
        }
    }
}

