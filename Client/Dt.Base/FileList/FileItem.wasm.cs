#if WASM
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FileLists;
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    public partial class FileItem
    {
        /// <summary>
        /// 打开文件
        /// <para>预览图片、播放音视频</para>
        /// <para>其他文件类型提示下载</para>
        /// </summary>
        public async Task OpenFile()
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
                return;

            switch (FileType)
            {
                case FileItemType.Image:
                    AtKit.Msg("未实现图片浏览功能");
                    break;

                case FileItemType.Video:
                    //Grid grid = (Grid)GetTemplateChild("ContentGrid");
                    //if (grid != null
                    //    && !(grid.Children[grid.Children.Count - 1] is MediaPlayerElement))
                    //{
                    //    var mediaPlayer = new MediaPlayerElement();
                    //    mediaPlayer.AutoPlay = true;

                    //    var player = mediaPlayer.MediaPlayer;
                    //    if (player == null)
                    //    {
                    //        player = new Windows.Media.Playback.MediaPlayer();
                    //        mediaPlayer.SetMediaPlayer(player);
                    //    }

                    //    mediaPlayer.Height = ActualHeight;
                    //    grid.Children.Add(mediaPlayer);
                    //    mediaPlayer.Source = MediaSource.CreateFromUri(new Uri($"{AtSys.Stub.ServerUrl}/fsm/{ID}"));
                    //}
                    AtKit.Msg("wasm版未实现MediaPlayerElement");
                    break;

                case FileItemType.Sound:
                    AtKit.Msg("wasm版未实现MediaPlayerElement");
                    break;

                default:
                    if (await AtKit.Confirm($"要下载《{Title}》吗？"))
                        DownloadFile();
                    break;
            }
            _owner.OnOpenedFile(this);
        }

        /// <summary>
        /// 共享文件
        /// </summary>
        public async Task ShareFile()
        {
            string fileName = Path.Combine(AtLocal.CachePath, GetFileName());
            if (!File.Exists(fileName))
            {
                // 先下载
                bool suc = await Download();
                if (!suc)
                    return;
            }

            string title;
            switch (FileType)
            {
                case FileItemType.Image:
                    title = "分享图片";
                    break;
                case FileItemType.Video:
                    title = "分享视频";
                    break;
                case FileItemType.Sound:
                    title = "分享音乐";
                    break;
                default:
                    title = "分享文件";
                    break;
            }

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = title,
                File = new ShareFile(fileName)
            });
        }

        /// <summary>
        /// 文件另存为，直接下载文件
        /// </summary>
        public void SaveAs()
        {
            DownloadFile();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <returns></returns>
        public void DownloadFile()
        {
            AtWasm.Download($"{AtSys.Stub.ServerUrl}/fsm/{ID}", Title);
        }

        Task LoadImage()
        {
            var path = new Uri($"{AtSys.Stub.ServerUrl}/fsm/{ID}{ThumbPostfix}");
            Bitmap = new BitmapImage(path);
            return Task.CompletedTask;
        }
    }
}
#endif