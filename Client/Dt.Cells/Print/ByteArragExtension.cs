#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.UI
{
    internal static class ByteArragExtension
    {
        internal static ImageSource ToImageSource(this byte[] rawImageBytes)
        {
            if (rawImageBytes == null)
                return null;

            BitmapImage img = null;
            try
            {
                using (MemoryStream stream = new MemoryStream(rawImageBytes))
                {
                    stream.Seek(0L, SeekOrigin.Begin);
                    img = new BitmapImage();
                    InMemoryRandomAccessStream streamSource = new InMemoryRandomAccessStream();
                    IOutputStream outputStreamAt = streamSource.GetOutputStreamAt(0L);
                    WindowsRuntimeSystemExtensions.AsTask<ulong, ulong>(RandomAccessStream.CopyAsync(WindowsRuntimeStreamExtensions.AsInputStream(stream), outputStreamAt)).Wait();
                    img.SetSource(streamSource);
                }
            }
            catch { }
            return img;
        }
    }
}

