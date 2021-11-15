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
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.UI
{
    internal class SR : SR<ResourceStrings>
    {
        static Dictionary<string, BitmapImage> _caches = new Dictionary<string, BitmapImage>();

        public static async Task<BitmapImage> GetImage(string resourceId)
        {
            BitmapImage bmp;
            if (_caches.TryGetValue(resourceId, out bmp))
                return bmp;

            bmp = new BitmapImage();
            using (var stream = typeof(SR).Assembly.GetManifestResourceStream("Dt.Cells.Icons." + resourceId))
            {
#if UWP
                await bmp.SetSourceAsync(stream.AsRandomAccessStream());
#else
                await bmp.SetSourceAsync(stream);
#endif
            }
            _caches[resourceId] = bmp;
            return bmp;
        }
    }
}

