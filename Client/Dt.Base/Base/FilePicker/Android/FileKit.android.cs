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
    public static class FileKit
    {
        static int _requestId;
        static TaskCompletionSource<List<FileData>> _completionSource;

        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickImage()
        {
            return PickFile(p_androidFileTypes: FileFilter.AndroidImage);
        }

        /// <summary>
        /// 选择多个图片
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickImages()
        {
            return PickFiles(true, FileFilter.AndroidImage);
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickVideo()
        {
            return PickFile(p_androidFileTypes: FileFilter.AndroidVideo);
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickVideos()
        {
            return PickFiles(true, FileFilter.AndroidVideo);
        }

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickAudio()
        {
            return PickFile(p_androidFileTypes: FileFilter.AndroidAudio);
        }

        /// <summary>
        /// 选择多个音频文件
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickAudios()
        {
            return PickFiles(true, FileFilter.AndroidAudio);
        }

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickMedia()
        {
            return PickFile(p_androidFileTypes: FileFilter.AndroidMedia);
        }

        /// <summary>
        /// 选择多个媒体文件
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickMedias()
        {
            return PickFiles(true, FileFilter.AndroidMedia);
        }

        /// <summary>
        /// 选择单个文件
        /// </summary>
        /// <param name="p_uwpFileTypes">uwp文件过滤类型，如 .png .docx，null时不过滤</param>
        /// <param name="p_androidFileTypes">android文件过滤类型，如 image/png image/*，null时不过滤</param>
        /// <param name="p_iosFileTypes">ios文件过滤类型，如 UTType.Image，null时不过滤</param>
        /// <returns></returns>
        public static async Task<FileData> PickFile(string[] p_uwpFileTypes = null, string[] p_androidFileTypes = null, string[] p_iosFileTypes = null)
        {
            var ls = await PickFiles(false, p_androidFileTypes);
            if (ls != null && ls.Count > 0)
                return ls[0];
            return null;
        }

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <param name="p_uwpFileTypes">uwp文件过滤类型，如 .png .docx，null时不过滤</param>
        /// <param name="p_androidFileTypes">android文件过滤类型，如 image/png image/*，null时不过滤</param>
        /// <param name="p_iosFileTypes">ios文件过滤类型，如 UTType.Image，null时不过滤</param>
        /// <returns></returns>
        public static Task<List<FileData>> PickFiles(string[] p_uwpFileTypes = null, string[] p_androidFileTypes = null, string[] p_iosFileTypes = null)
        {
            return PickFiles(true, p_androidFileTypes);
        }

        static Task<List<FileData>> PickFiles(bool p_allowMultiple, string[] p_allowedTypes)
        {
            int id = GetRequestId();
            var ntcs = new TaskCompletionSource<List<FileData>>(id);

            var previousTcs = Interlocked.Exchange(ref _completionSource, ntcs);
            if (previousTcs != null)
            {
                previousTcs.TrySetResult(null);
            }

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
                    var tcs = Interlocked.Exchange(ref _completionSource, null);
                    FilePickerActivity.FilePicked -= handler;
                    tcs?.SetResult(e);
                };
                FilePickerActivity.FilePicked += handler;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                _completionSource.SetException(ex);
            }

            return _completionSource.Task;
        }

        /// <summary>
        /// 将选择的文件保存到.doc目录
        /// </summary>
        /// <param name="p_file">待另存的文件信息</param>
        /// <returns>文件完整路径</returns>
        public static Task<string> SaveFile(FileData p_file)
        {
            try
            {
                var tempPath = System.IO.Path.Combine(AtSys.DocPath, AtKit.NewID + p_file.Ext);
                System.IO.File.Copy(p_file.FilePath, tempPath);
                return Task.FromResult(tempPath);
            }
            catch (Exception)
            {
                return Task.FromResult("");
            }
        }

        /// <summary>
        /// Android implementation of opening a file by using ActionView intent.
        /// </summary>
        /// <param name="fileToOpen">file to open in viewer</param>
        public static void OpenFile(File fileToOpen)
        {
            var uri = Android.Net.Uri.FromFile(fileToOpen);
            var intent = new Intent();
            var mime = IOUtil.GetMimeType(uri.ToString());

            intent.SetAction(Intent.ActionView);
            intent.SetDataAndType(uri, mime);
            intent.SetFlags(ActivityFlags.NewTask);

            Application.Context.StartActivity(intent);
        }

        /// <summary>
        /// Android implementation of OpenFile(), opening a file already stored on external
        /// storage.
        /// </summary>
        /// <param name="fileToOpen">relative filename of file to open</param>
        public static void OpenFile(string fileToOpen)
        {
            var myFile = new File(Android.OS.Environment.ExternalStorageDirectory, fileToOpen);

            OpenFile(myFile);
        }

        /// <summary>
        /// Android implementation of OpenFile(), opening a picked file in an external viewer. The
        /// picked file is saved to external storage before opening.
        /// </summary>
        /// <param name="fileToOpen">picked file data</param>
        public static async void OpenFile(FileData fileToOpen)
        {
            var myFile = new File(Android.OS.Environment.ExternalStorageDirectory, fileToOpen.FileName);

            if (!myFile.Exists())
            {
                await SaveFile(fileToOpen);
            }

            OpenFile(myFile);
        }

        /// <summary>
        /// Returns a new request ID for a new call to PickFile()
        /// </summary>
        /// <returns>new request ID</returns>
        static int GetRequestId()
        {
            int id = _requestId;

            if (_requestId == int.MaxValue)
            {
                _requestId = 0;
            }
            else
            {
                _requestId++;
            }

            return id;
        }
    }
}
#endif