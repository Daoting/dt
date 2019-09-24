#if UWP
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Uwp版文件选择
    /// </summary>
    public static class FilePicker
    {
        /// <summary>
        /// 选择单个照片
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickPhoto()
        {
            var picker = CreatePicker(PickerLocationId.PicturesLibrary, AtKit.ImageFormat.ToArray());
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFile(picker);
        }

        /// <summary>
        /// 选择多个照片
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickPhotos()
        {
            var picker = CreatePicker(PickerLocationId.PicturesLibrary, AtKit.ImageFormat.ToArray());
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFiles(picker);
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickVideo()
        {
            var picker = CreatePicker(PickerLocationId.VideosLibrary, AtKit.VideoFormat.ToArray());
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFile(picker);
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickVideos()
        {
            var picker = CreatePicker(PickerLocationId.VideosLibrary, AtKit.VideoFormat.ToArray());
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFiles(picker);
        }

        /// <summary>
        /// 选择单个照片或视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickMedia()
        {
            List<string> ls = new List<string>();
            ls.AddRange(AtKit.ImageFormat);
            ls.AddRange(AtKit.VideoFormat);
            var picker = CreatePicker(PickerLocationId.DocumentsLibrary, ls.ToArray());
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFile(picker);
        }

        /// <summary>
        /// 选择多个照片或视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickMedias()
        {
            List<string> ls = new List<string>();
            ls.AddRange(AtKit.ImageFormat);
            ls.AddRange(AtKit.VideoFormat);
            var picker = CreatePicker(PickerLocationId.DocumentsLibrary, ls.ToArray());
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFiles(picker);
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
        public static Task<FileData> PickFile(string[] p_allowedTypes)
        {
            return PickFile(CreatePicker(PickerLocationId.DocumentsLibrary, p_allowedTypes));
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
            return PickFiles(CreatePicker(PickerLocationId.DocumentsLibrary, p_allowedTypes));
        }

        /// <summary>
        /// Android implementation of saving a picked file to the external storage directory.
        /// </summary>
        /// <param name="fileToSave">picked file data for file to save</param>
        /// <returns>true when file was saved successfully, false when not</returns>
        public static async Task<bool> SaveFile(FileData fileToSave)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    fileToSave.FileName,
                    CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteBytesAsync(file, fileToSave.GetBytes());

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Android implementation of OpenFile(), opening a file already stored on external
        /// storage.
        /// </summary>
        /// <param name="fileToOpen">relative filename of file to open</param>
        public static async void OpenFile(string fileToOpen)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileToOpen);

                if (file != null)
                {
                    await Launcher.LaunchFileAsync(file);
                }
            }
            catch (FileNotFoundException)
            {
                // ignore exceptions
            }
            catch (Exception)
            {
                // ignore exceptions
            }
        }

        /// <summary>
        /// Android implementation of OpenFile(), opening a picked file in an external viewer. The
        /// picked file is saved to external storage before opening.
        /// </summary>
        /// <param name="fileToOpen">picked file data</param>
        public static async void OpenFile(FileData fileToOpen)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileToOpen.FileName);

                if (file != null)
                {
                    await Launcher.LaunchFileAsync(file);
                }
            }
            catch (FileNotFoundException)
            {
                await SaveFile(fileToOpen);
                OpenFile(fileToOpen);
            }
            catch (Exception)
            {
                // ignore exceptions
            }
        }

        #region 内部方法
        static async Task<FileData> PickFile(FileOpenPicker p_picker)
        {
            var file = await p_picker.PickSingleFileAsync();
            if (file == null)
                return null;

            string id = StorageApplicationPermissions.FutureAccessList.Add(file);
            return new FileData(id, file.Name);
        }

        static async Task<List<FileData>> PickFiles(FileOpenPicker p_picker)
        {
            var files = await p_picker.PickMultipleFilesAsync();
            if (files == null || files.Count == 0)
                return null;

            List<FileData> ls = new List<FileData>();
            foreach (var file in files)
            {
                string id = StorageApplicationPermissions.FutureAccessList.Add(file);
                ls.Add(new FileData(id, file.Name));
            }
            return ls;
        }

        static FileOpenPicker CreatePicker(PickerLocationId p_locationstring, string[] p_allowedTypes)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = p_locationstring,
            };

            var hasAtleastOneType = false;
            if (p_allowedTypes != null)
            {
                foreach (var type in p_allowedTypes)
                {
                    if (type.StartsWith("."))
                    {
                        picker.FileTypeFilter.Add(type);
                        hasAtleastOneType = true;
                    }
                }
            }
            if (!hasAtleastOneType)
                picker.FileTypeFilter.Add("*");
            return picker;
        }
        #endregion
    }
}
#endif