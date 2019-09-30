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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// IOS版文件选择
    /// </summary>
    public static class FileKit
    {
        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        public static async Task<FileData> PickImage()
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
        public static Task<List<FileData>> PickImages()
        {
            return new MediaPickerForIOS().PickFiles(true, FileFilter.IOSImage);
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public static async Task<FileData> PickVideo()
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
        public static Task<List<FileData>> PickVideos()
        {
            return new MediaPickerForIOS().PickFiles(true, FileFilter.IOSVideo);
        }

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        public static async Task<FileData> PickAudio()
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
        public static Task<List<FileData>> PickAudios()
        {
            return new MediaPickerForIOS().PickFiles(true, FileFilter.IOSAudio);
        }

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        public static async Task<FileData> PickMedia()
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
        public static Task<List<FileData>> PickMedias()
        {
            return new MediaPickerForIOS().PickFiles(true, FileFilter.IOSMedia);
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
            var ls = await new FilePickerForIOS().PickFiles(false, p_iosFileTypes);
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
            return new FilePickerForIOS().PickFiles(true, p_iosFileTypes);
        }

        /// <summary>
        /// iOS implementation of saving a picked file to the iOS "my documents" directory.
        /// </summary>
        /// <param name="fileToSave">picked file data for file to save</param>
        /// <returns>true when file was saved successfully, false when not</returns>
        public static async Task<bool> SaveFile(FileData fileToSave)
        {
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fileName = Path.Combine(documents, fileToSave.FileName);

                File.WriteAllBytes(fileName, await fileToSave.GetBytes());

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
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
    }
}
#endif