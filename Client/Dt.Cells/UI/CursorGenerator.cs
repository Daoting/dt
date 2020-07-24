#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.UI
{
    internal static class CursorGenerator
    {
        static Dictionary<CursorType, ImageSource> _cache = new Dictionary<CursorType, ImageSource>();

        internal static ImageSource GetCursor(CursorType cursorType)
        {
            ImageSource source;
            if (!_cache.TryGetValue(cursorType, out source))
            {
                bool flag = cursorType.ToString().StartsWith("Resize");
                string str = cursorType.ToString() + (((Application.Current.RequestedTheme == ApplicationTheme.Dark) && flag) ? "_dark" : "") + ".png";
                source = new BitmapImage(new Uri("ms-appx:///Dt.Cells/Icons/" + str));
                _cache.Add(cursorType, source);
            }
            return source;
        }
    }
}

