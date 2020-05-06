#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Foundation;
using MobileCoreServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// IOS版文件选择
    /// </summary>
    class FilePicker
    {
        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        public async Task<FileData> PickImage()
        {
            var ls = await new MediaPickerForIOS().PickFiles(false, FileFilter.IOSImage);
            if (ls != null && ls.Count > 0)
                return ls[0];
            return null;
        }

        /// <summary>
        /// 选择多个图片
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickImages()
        {
            return new MediaPickerForIOS().PickFiles(true, FileFilter.IOSImage);
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public async Task<FileData> PickVideo()
        {
            var ls = await new MediaPickerForIOS().PickFiles(false, FileFilter.IOSVideo);
            if (ls != null && ls.Count > 0)
                return ls[0];
            return null;
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickVideos()
        {
            return new MediaPickerForIOS().PickFiles(true, FileFilter.IOSVideo);
        }

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        public async Task<FileData> PickAudio()
        {
            var ls = await new MediaPickerForIOS().PickFiles(false, FileFilter.IOSAudio);
            if (ls != null && ls.Count > 0)
                return ls[0];
            return null;
        }

        /// <summary>
        /// 选择多个音频文件
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickAudios()
        {
            return new MediaPickerForIOS().PickFiles(true, FileFilter.IOSAudio);
        }

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        public async Task<FileData> PickMedia()
        {
            var ls = await new MediaPickerForIOS().PickFiles(false, FileFilter.IOSMedia);
            if (ls != null && ls.Count > 0)
                return ls[0];
            return null;
        }

        /// <summary>
        /// 选择多个媒体文件
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickMedias()
        {
            return new MediaPickerForIOS().PickFiles(true, FileFilter.IOSMedia);
        }

        /// <summary>
        /// 选择单个文件
        /// </summary>
        /// <param name="p_fileTypes">ios文件过滤类型，如 UTType.Image，null时不过滤</param>
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
        /// <param name="p_fileTypes">ios文件过滤类型，如 UTType.Image，null时不过滤</param>
        /// <returns></returns>
        public Task<List<FileData>> PickFiles(string[] p_fileTypes)
        {
            return PickFiles(true, p_fileTypes);
        }

        #region 内部方法
        TaskCompletionSource<List<FileData>> _tcs;

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="p_allowMultiple">是否多选</param>
        /// <param name="p_allowedTypes"></param>
        /// <returns></returns>
        Task<List<FileData>> PickFiles(bool p_allowMultiple, string[] p_allowedTypes)
        {
            var allowedUtis = (p_allowedTypes != null) ? p_allowedTypes : new string[]
            {
                UTType.Content,
                UTType.Item,
                "public.data"
            };

            // NOTE: Importing (UIDocumentPickerMode.Import) makes a local copy of the document,
            // while opening (UIDocumentPickerMode.Open) opens the document directly. We do the
            // first, so the user has to read the file immediately.
            var documentPicker = new UIDocumentPickerViewController(allowedUtis, UIDocumentPickerMode.Import);
            documentPicker.DidPickDocument += DocumentPicker_DidPickDocument;
            documentPicker.WasCancelled += DocumentPicker_WasCancelled;
            documentPicker.DidPickDocumentAtUrls += DocumentPicker_DidPickDocumentAtUrls;
            // 多选
            if (p_allowMultiple)
                documentPicker.AllowsMultipleSelection = true;

            UIViewController viewController = GetActiveViewController();
            viewController.PresentViewController(documentPicker, true, null);

            _tcs = new TaskCompletionSource<List<FileData>>();
            return _tcs.Task;
        }

        /// <summary>
        /// Finds active view controller to use to present document picker
        /// </summary>
        /// <returns>view controller to use</returns>
        UIViewController GetActiveViewController()
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            UIViewController viewController = window.RootViewController;

            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            return viewController;
        }

        /// <summary>
        /// Callback method called by document picker when file has been picked; this is called
        /// starting from iOS 11.
        /// </summary>
        /// <param name="sender">sender object (document picker)</param>
        /// <param name="args">event args</param>
        void DocumentPicker_DidPickDocumentAtUrls(object sender, UIDocumentPickedAtUrlsEventArgs args)
        {
            try
            {
                List<FileData> ls = new List<FileData>();
                NSFileManager mg = new NSFileManager();
                foreach (var url in args.Urls)
                {
                    url.StartAccessingSecurityScopedResource();

                    var doc = new UIDocument(url);
                    string name = doc.LocalizedName;
                    string path = doc.FileUrl?.Path;
                    // iCloud drive can return null for LocalizedName.
                    if (name == null && path != null)
                        name = Path.GetFileName(path);
                    ls.Add(new FileData(path, name, mg.GetAttributes(path).Size.Value));

                    url.StopAccessingSecurityScopedResource();
                }

                var tcs = Interlocked.Exchange(ref _tcs, null);
                tcs?.SetResult(ls);
            }
            catch (Exception ex)
            {
                // pass exception to task so that it doesn't get lost in the UI main loop
                var tcs = Interlocked.Exchange(ref _tcs, null);
                tcs?.SetException(ex);
            }
        }

        /// <summary>
        /// Callback method called by document picker when file has been picked; this is called
        /// up to iOS 10.
        /// </summary>
        /// <param name="sender">sender object (document picker)</param>
        /// <param name="args">event args</param>
        void DocumentPicker_DidPickDocument(object sender, UIDocumentPickedEventArgs args)
        {
            try
            {
                var securityEnabled = args.Url.StartAccessingSecurityScopedResource();
                var doc = new UIDocument(args.Url);

                string name = doc.LocalizedName;
                string path = doc.FileUrl?.Path;

                args.Url.StopAccessingSecurityScopedResource();

                // iCloud drive can return null for LocalizedName.
                if (name == null && path != null)
                {
                    name = Path.GetFileName(path);
                }

                NSFileManager mg = new NSFileManager();
                List<FileData> ls = new List<FileData>();
                ls.Add(new FileData(path, name, mg.GetAttributes(path).Size.Value));
                var tcs = Interlocked.Exchange(ref _tcs, null);
                tcs?.SetResult(ls);
            }
            catch (Exception ex)
            {
                // pass exception to task so that it doesn't get lost in the UI main loop
                var tcs = Interlocked.Exchange(ref _tcs, null);
                tcs.SetException(ex);
            }
        }

        /// <summary>
        /// Handles when the file picker was cancelled. Either in the
        /// popup menu or later on.
        /// </summary>
        /// <param name="sender">sender object (document picker)</param>
        /// <param name="args">event args</param>
        void DocumentPicker_WasCancelled(object sender, EventArgs args)
        {
            var tcs = Interlocked.Exchange(ref _tcs, null);
            tcs.SetResult(null);
        }
        #endregion
    }
}
#endif