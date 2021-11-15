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
                    await new ImageFileView().ShowDlg(_owner, this);
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
                    //    mediaPlayer.Source = MediaSource.CreateFromUri(new Uri($"{Kit.Stub.ServerUrl}/fsm/{ID}"));
                    //}
                    Kit.Warn("wasm版未实现MediaPlayerElement");
                    break;

                case FileItemType.Sound:
                    Kit.Warn("wasm版未实现MediaPlayerElement");
                    break;

                default:
                    if (await Kit.Confirm($"要下载《{Title}》吗？"))
                        DownloadFile();
                    break;
            }
            _owner.OnOpenedFile(this);
        }

        /// <summary>
        /// 共享文件
        /// </summary>
        public Task ShareFile()
        {
            Kit.Warn("wasm版未实现分享功能");
            return Task.CompletedTask;
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
            Kit.Download($"{Kit.Stub.ServerUrl}/fsm/{ID}", Title);
        }

        internal Task<string> EnsureFileExists()
        {
            return Task.FromResult($"{Kit.Stub.ServerUrl}/fsm/{ID}");
        }

        Task LoadImage()
        {
            var path = new Uri($"{Kit.Stub.ServerUrl}/fsm/{ID}{ThumbPostfix}");
            Bitmap = new BitmapImage(path);
            return Task.CompletedTask;
        }
    }
}
#endif