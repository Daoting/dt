#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
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
            return PickFile(new string[] { UTType.Image });
        }

        /// <summary>
        /// 选择多个照片
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickPhotos()
        {
            return PickFiles(true, new string[] { UTType.Image });
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickVideo()
        {
            return PickFile(new string[] { UTType.Video });
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickVideos()
        {
            return PickFiles(true, new string[] { UTType.Video });
        }

        /// <summary>
        /// 选择单个照片或视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickMedia()
        {
            return PickFile(new string[] { UTType.Image, UTType.Video });
        }

        /// <summary>
        /// 选择多个照片或视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickMedias()
        {
            return PickFiles(true, new string[] { UTType.Image, UTType.Video });
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

        static Task<List<FileData>> PickFiles(bool p_allowMultiple, string[] allowedTypes)
        {
            var id = GetRequestId();
            var ntcs = new TaskCompletionSource<List<FileData>>(id);
            if (Interlocked.CompareExchange(ref _completionSource, ntcs, null) != null)
            {
                throw new InvalidOperationException("Only one operation can be active at a time");
            }

            var allowedUtis = new string[]
            {
                UTType.Content,
                UTType.Item,
                "public.data"
            };

            if (allowedTypes != null)
            {
                allowedUtis = allowedTypes;
            }

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
            return _completionSource.Task;
        }

        /// <summary>
        /// iOS implementation of saving a picked file to the iOS "my documents" directory.
        /// </summary>
        /// <param name="fileToSave">picked file data for file to save</param>
        /// <returns>true when file was saved successfully, false when not</returns>
        public static Task<bool> SaveFile(FileData fileToSave)
        {
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fileName = Path.Combine(documents, fileToSave.FileName);

                File.WriteAllBytes(fileName, fileToSave.GetBytes());

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// iOS implementation of opening a file by using a UIDocumentInteractionController.
        /// </summary>
        /// <param name="fileUrl">file Url to open in viewer</param>
        public static void OpenFile(NSUrl fileUrl)
        {
            var docController = UIDocumentInteractionController.FromUrl(fileUrl);

            var window = UIApplication.SharedApplication.KeyWindow;
            var subViews = window.Subviews;
            var lastView = subViews.Last();
            var frame = lastView.Frame;

            docController.PresentOpenInMenu(frame, lastView, true);
        }

        /// <summary>
        /// iOS implementation of OpenFile(), opening a file already stored on iOS "my documents"
        /// directory.
        /// </summary>
        /// <param name="fileToOpen">relative filename of file to open</param>
        public static void OpenFile(string fileToOpen)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var fileName = Path.Combine(documents, fileToOpen);

            if (NSFileManager.DefaultManager.FileExists(fileName))
            {
                var url = new NSUrl(fileName, true);
                OpenFile(url);
            }
        }

        /// <summary>
        /// iOS implementation of OpenFile(), opening a picked file in an external viewer. The
        /// picked file is saved to iOS "my documents" directory before opening.
        /// </summary>
        /// <param name="fileToOpen">picked file data</param>
        public static async void OpenFile(FileData fileToOpen)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var fileName = Path.Combine(documents, fileToOpen.FileName);

            if (NSFileManager.DefaultManager.FileExists(fileName))
            {
                var url = new NSUrl(fileName, true);

                OpenFile(url);
            }
            else
            {
                await SaveFile(fileToOpen);
                OpenFile(fileToOpen);
            }
        }

        /// <summary>
        /// Finds active view controller to use to present document picker
        /// </summary>
        /// <returns>view controller to use</returns>
        static UIViewController GetActiveViewController()
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
        static void DocumentPicker_DidPickDocumentAtUrls(object sender, UIDocumentPickedAtUrlsEventArgs args)
        {
            try
            {
                List<FileData> ls = new List<FileData>();
                foreach (var url in args.Urls)
                {
                    url.StartAccessingSecurityScopedResource();

                    var doc = new UIDocument(url);
                    string name = doc.LocalizedName;
                    string path = doc.FileUrl?.Path;
                    // iCloud drive can return null for LocalizedName.
                    if (name == null && path != null)
                        name = Path.GetFileName(path);
                    ls.Add(new FileData(path, name));

                    url.StopAccessingSecurityScopedResource();
                }

                var tcs = Interlocked.Exchange(ref _completionSource, null);
                tcs?.SetResult(ls);
            }
            catch (Exception ex)
            {
                // pass exception to task so that it doesn't get lost in the UI main loop
                var tcs = Interlocked.Exchange(ref _completionSource, null);
                tcs?.SetException(ex);
            }
        }

        /// <summary>
        /// Callback method called by document picker when file has been picked; this is called
        /// up to iOS 10.
        /// </summary>
        /// <param name="sender">sender object (document picker)</param>
        /// <param name="args">event args</param>
        static void DocumentPicker_DidPickDocument(object sender, UIDocumentPickedEventArgs args)
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

                List<FileData> ls = new List<FileData>();
                ls.Add(new FileData(path, name));
                var tcs = Interlocked.Exchange(ref _completionSource, null);
                tcs?.SetResult(ls);
            }
            catch (Exception ex)
            {
                // pass exception to task so that it doesn't get lost in the UI main loop
                var tcs = Interlocked.Exchange(ref _completionSource, null);
                tcs.SetException(ex);
            }
        }

        /// <summary>
        /// Handles when the file picker was cancelled. Either in the
        /// popup menu or later on.
        /// </summary>
        /// <param name="sender">sender object (document picker)</param>
        /// <param name="args">event args</param>
        static void DocumentPicker_WasCancelled(object sender, EventArgs args)
        {
            var tcs = Interlocked.Exchange(ref _completionSource, null);
            tcs.SetResult(null);
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