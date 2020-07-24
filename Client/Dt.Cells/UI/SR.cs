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
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.UI
{
    internal class SR : SR<ResourceStrings>
    {
        static readonly string _resourcePrefix = "ms-appx:///Dt.Cells/Icons/";

        public static BitmapImage GetImage(string resourceId)
        {
            BitmapImage image = new BitmapImage();
            Uri uri = new Uri(_resourcePrefix + resourceId);
            image.UriSource = uri;
            return image;
        }
    }
}

