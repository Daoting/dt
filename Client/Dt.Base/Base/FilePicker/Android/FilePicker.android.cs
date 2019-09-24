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
    public static class FilePicker
    {
        static int _requestId;
        static TaskCompletionSource<List<FileData>> _completionSource;

        /// <summary>
        /// 选择单个照片
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickPhoto()
        {
            return PickFile(new string[] { "image/*" });
        }

        /// <summary>
        /// 选择多个照片
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickPhotos()
        {
            return PickFiles(true, new string[] { "image/*" });
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickVideo()
        {
            return PickFile(new string[] { "video/*" });
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickVideos()
        {
            return PickFiles(true, new string[] { "video/*" });
        }

        /// <summary>
        /// 选择单个照片或视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickMedia()
        {
            return PickFile(new string[] { "image/*", "video/*" });
        }

        /// <summary>
        /// 选择多个照片或视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickMedias()
        {
            return PickFiles(true, new string[] { "image/*", "video/*" });
        }

        /// <summary>
        /// 选择单个文件
        /// </summary>
        /// <param name="p_allowedTypes">
        /// 文件过滤类型，null时不过滤文件类型，各平台格式不同：
        /// uwp：如.png .docx
        /// android：image/png image/*
        /// ios：UTType.Image
        /// </param>
        /// <returns></returns>
        public static async Task<FileData> PickFile(string[] p_allowedTypes)
        {
            var ls = await PickFiles(false, p_allowedTypes);
            if (ls != null && ls.Count > 0)
                return ls[0];
            return null;
        }

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <param name="p_allowedTypes">
        /// 文件过滤类型，null时不过滤文件类型，各平台格式不同：
        /// uwp：如.png .docx
        /// android：image/png image/*
        /// ios：UTType.Image
        /// </param>
        /// <returns></returns>
        public static Task<List<FileData>> PickFiles(string[] p_allowedTypes)
        {
            return PickFiles(true, p_allowedTypes);
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
        /// Android implementation of saving a picked file to the external storage directory.
        /// </summary>
        /// <param name="fileToSave">picked file data for file to save</param>
        /// <returns>true when file was saved successfully, false when not</returns>
        public static Task<bool> SaveFile(FileData fileToSave)
        {
            try
            {
                var myFile = new File(Android.OS.Environment.ExternalStorageDirectory, fileToSave.FileName);

                if (myFile.Exists())
                {
                    return Task.FromResult(true);
                }

                var fos = new FileOutputStream(myFile.Path);

                fos.Write(fileToSave.GetBytes());
                fos.Close();

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult(false);
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