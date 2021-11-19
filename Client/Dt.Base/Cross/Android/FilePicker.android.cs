#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Android.App;
using Android.Content;
using Android.Runtime;
using Dt.Core;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Android版文件选择
    /// </summary>
    [Preserve(AllMembers = true)]
    class FilePicker
    {
        TaskCompletionSource<List<FileData>> _tcs;

        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickImage()
        {
            return PickFile(FileFilter.AndroidImage);
        }

        /// <summary>
        /// 选择多个图片
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickImages()
        {
            return PickFiles(true, FileFilter.AndroidImage);
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickVideo()
        {
            return PickFile(FileFilter.AndroidVideo);
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickVideos()
        {
            return PickFiles(true, FileFilter.AndroidVideo);
        }

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickAudio()
        {
            return PickFile(FileFilter.AndroidAudio);
        }

        /// <summary>
        /// 选择多个音频文件
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickAudios()
        {
            return PickFiles(true, FileFilter.AndroidAudio);
        }

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickMedia()
        {
            return PickFile(FileFilter.AndroidMedia);
        }

        /// <summary>
        /// 选择多个媒体文件
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickMedias()
        {
            return PickFiles(true, FileFilter.AndroidMedia);
        }

        /// <summary>
        /// 选择单个文件
        /// </summary>
        /// <param name="p_fileTypes">android文件过滤类型，如 image/png image/*，null时不过滤</param>
        /// <returns></returns>
        public async Task<FileData> PickFile(string[] p_fileTypes)
        {
            var ls = await PickFiles(false, p_fileTypes);
            if (ls != null && ls.Count > 0)
                return ls[0];
            return null;
        }

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <param name="p_fileTypes">android文件过滤类型，如 image/png image/*，null时不过滤</param>
        /// <returns></returns>
        public Task<List<FileData>> PickFiles(string[] p_fileTypes)
        {
            return PickFiles(true, p_fileTypes);
        }

        Task<List<FileData>> PickFiles(bool p_allowMultiple, string[] p_allowedTypes)
        {
            var ntcs = new TaskCompletionSource<List<FileData>>();
            var previousTcs = Interlocked.Exchange(ref _tcs, ntcs);
            if (previousTcs != null)
                previousTcs.TrySetResult(null);

            try
            {
                var pickerIntent = new Intent(Application.Context, typeof(FilePickerActivity));
                pickerIntent.SetFlags(ActivityFlags.NewTask);

                pickerIntent.PutExtra(FilePickerActivity.ExtraAllowedTypes, p_allowedTypes);
                // 多选
                if (p_allowMultiple)
                    pickerIntent.PutExtra(Intent.ExtraAllowMultiple, true);
                Application.Context.StartActivity(pickerIntent);

                EventHandler<List<FileData>> handler = null;
                handler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref _tcs, null);
                    FilePickerActivity.FilePicked -= handler;
                    tcs?.SetResult(e);
                };
                FilePickerActivity.FilePicked += handler;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                _tcs.SetException(ex);
            }

            return _tcs.Task;
        }
    }
}
#endif