#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
using System;
using System.IO;
using System.Threading;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 头像视图扩展
    /// </summary>
    public class MemberIconViewEx
    {
        public static Image Icon(ViewItem p_item)
        {
            Image img = new Image();
            //string fileName = p_item.Row.Str("id") + ".png";
            //if (File.Exists(Path.Combine(AtLocal.CachePath, fileName)))
            //    img.Source = new BitmapImage(new Uri("ms-appdata:///local/.doc/" + fileName));
            //else
            //    img.Source = new BitmapImage(new Uri("https://10.10.1.16/baisui/fsm/.d/v0/94/95/59100539833753600.png"));

            CreateImage(p_item, img);
            return img;
        }

        static async void CreateImage(ViewItem p_item, Image p_img)
        {
            string path = Path.Combine(AtLocal.CachePath, p_item.Row.Str("id") + ".png");
            //if (!File.Exists(path))
            {
                FileStream stream = File.Create(path);
                DownloadInfo info = new DownloadInfo
                {
                    Path = "v0/94/95/59100539833753600.png",
                    TgtStream = stream,
                };

                bool suc = false;
                try
                {
                    suc = await Downloader.Handle(info, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "下载出错");
                }
                finally
                {
                    stream.Close();
                }

                if (!suc)
                {
                    // 未成功，删除缓存文件，避免打开时出错
                    try
                    {
                        // mono中 FileInfo 的 Exists 状态不同步！
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }
                    catch { }
                    return;
                }

                if (suc)
                    p_img.Source = new BitmapImage(new Uri("ms-appdata:///local/.doc/" + p_item.Row.Str("id") + ".png"));
            }

//            BitmapImage bmp = new BitmapImage();
//#if UWP
//            StorageFile sf = await StorageFile.GetFileFromPathAsync(path);
//            if (sf != null)
//            {
//                using (var stream = await sf.OpenAsync(FileAccessMode.Read))
//                {
//                    await bmp.SetSourceAsync(stream);
//                }
//            }
//#elif ANDROID
//            using (var stream = System.IO.File.OpenRead(path))
//            {
//                await bmp.SetSourceAsync(stream);
//            }
//#elif IOS
//            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
//            {
//                await bmp.SetSourceAsync(stream);
//            }
//#endif
//            p_img.Source = bmp;

            //AtKit.RunAsync(async () =>
            //{
                
            //});
        }
    }
}
