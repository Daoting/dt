#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.UI
{
    internal static class CursorGenerator
    {
        static ConcurrentDictionary<CursorType, BitmapImage> _cache = new ConcurrentDictionary<CursorType, BitmapImage>();

        internal static async Task<BitmapImage> GetCursor(CursorType cursorType)
        {
            BitmapImage source;
            if (!_cache.TryGetValue(cursorType, out source))
            {
                bool flag = cursorType.ToString().StartsWith("Resize");
                string str = cursorType.ToString() + (((Application.Current.RequestedTheme == ApplicationTheme.Dark) && flag) ? "_dark" : "") + ".png";
                source = new BitmapImage();
#if WIN
                // win将图片嵌入在pri中
                var rm = new Microsoft.Windows.ApplicationModel.Resources.ResourceManager();
                var data = rm.MainResourceMap.GetValue("Files/Icons/" + str).ValueAsBytes;
                using (var stream = new MemoryStream(data))
                {
                    await source.SetSourceAsync(stream.AsRandomAccessStream());
                }
#else
                using (var stream = typeof(CursorGenerator).Assembly.GetManifestResourceStream("Dt.Cells.Icons." + str))
                {
                    await source.SetSourceAsync(stream);
                }
#endif
                _cache[cursorType] = source;
            }
            return source;
        }
    }
}

