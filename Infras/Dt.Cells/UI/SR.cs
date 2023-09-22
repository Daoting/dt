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
using Microsoft.UI.Xaml.Media.Imaging;
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
#if WIN
            // win将图片嵌入在pri中
            var rm = new Microsoft.Windows.ApplicationModel.Resources.ResourceManager();
            var data = rm.MainResourceMap.GetValue("Files/Dt.Cells/Icons/" + resourceId).ValueAsBytes;
            using (var stream = new MemoryStream(data))
            {
                await bmp.SetSourceAsync(stream.AsRandomAccessStream());
            }
#else
            using (var stream = typeof(SR).Assembly.GetManifestResourceStream("Dt.Cells.Dt.Cells.Icons." + resourceId))
            {
                await bmp.SetSourceAsync(stream);
            }
#endif
            _caches[resourceId] = bmp;
            return bmp;
        }
    }
}

