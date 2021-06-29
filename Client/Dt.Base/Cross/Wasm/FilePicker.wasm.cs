#if WASM
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
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Uwp版文件选择
    /// </summary>
    class FilePicker
    {
        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickImage()
        {
            var picker = CreatePicker(PickerLocationId.PicturesLibrary, FileFilter.UwpImage);
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFile(picker);
        }

        /// <summary>
        /// 选择多个图片
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickImages()
        {
            var picker = CreatePicker(PickerLocationId.PicturesLibrary, FileFilter.UwpImage);
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFiles(picker);
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickVideo()
        {
            var picker = CreatePicker(PickerLocationId.VideosLibrary, FileFilter.UwpVideo);
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFile(picker);
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickVideos()
        {
            var picker = CreatePicker(PickerLocationId.VideosLibrary, FileFilter.UwpVideo);
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFiles(picker);
        }

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickAudio()
        {
            return PickFile(CreatePicker(PickerLocationId.MusicLibrary, FileFilter.UwpAudio));
        }

        /// <summary>
        /// 选择多个音频文件
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickAudios()
        {
            return PickFiles(CreatePicker(PickerLocationId.MusicLibrary, FileFilter.UwpAudio));
        }

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickMedia()
        {
            var picker = CreatePicker(PickerLocationId.DocumentsLibrary, FileFilter.UwpMedia);
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFile(picker);
        }

        /// <summary>
        /// 选择多个媒体文件
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickMedias()
        {
            var picker = CreatePicker(PickerLocationId.DocumentsLibrary, FileFilter.UwpMedia);
            picker.ViewMode = PickerViewMode.Thumbnail;
            return PickFiles(picker);
        }

        /// <summary>
        /// 选择单个文件
        /// </summary>
        /// <param name="p_fileTypes">uwp文件过滤类型，如 .png .docx，null时不过滤</param>
        /// <returns></returns>
        public Task<FileData> PickFile(string[] p_fileTypes)
        {
            return PickFile(CreatePicker(PickerLocationId.DocumentsLibrary, p_fileTypes));
        }

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <param name="p_fileTypes">uwp文件过滤类型，如 .png .docx，null时不过滤</param>
        /// <returns></returns>
        public Task<List<FileData>> PickFiles(string[] p_fileTypes)
        {
            return PickFiles(CreatePicker(PickerLocationId.DocumentsLibrary, p_fileTypes));
        }

        #region 内部方法
        static async Task<FileData> PickFile(FileOpenPicker p_picker)
        {
            var file = await p_picker.PickSingleFileAsync();
            if (file != null)
                return await GetFileData(file);
            return null;
        }

        static async Task<List<FileData>> PickFiles(FileOpenPicker p_picker)
        {
            var files = await p_picker.PickMultipleFilesAsync();
            if (files == null || files.Count == 0)
                return null;

            List<FileData> ls = new List<FileData>();
            foreach (var file in files)
            {
                ls.Add(await GetFileData(file));
            }
            return ls;
        }

        static async Task<FileData> GetFileData(StorageFile p_file)
        {
            string id = StorageApplicationPermissions.FutureAccessList.Add(p_file);
            var fd = new FileData(id, p_file.Name, (await p_file.GetBasicPropertiesAsync()).Size);
            string ext = fd.Ext;
            bool existThumb = false;

            // 详细描述
            if (FileFilter.UwpImage.Contains(ext))
            {
                var prop = await p_file.Properties.GetImagePropertiesAsync();
                fd.Desc = $"{prop.Width} x {prop.Height} ({ext})";
                existThumb = (prop.Width > FileData.ThumbSize || prop.Height > FileData.ThumbSize);
            }
            else if (FileFilter.UwpVideo.Contains(ext))
            {
                var prop = await p_file.Properties.GetVideoPropertiesAsync();
                fd.Desc = string.Format("{0:HH:mm:ss} ({1} x {2})", new DateTime(prop.Duration.Ticks), prop.Width, prop.Height);
                existThumb = true;
            }
            else if (FileFilter.UwpAudio.Contains(ext))
            {
                var prop = await p_file.Properties.GetMusicPropertiesAsync();
                fd.Desc = string.Format("{0:mm:ss}", new DateTime(prop.Duration.Ticks));
            }

            // 生成缩略图
            if (existThumb)
            {
                fd.ThumbPath = Path.Combine(Kit.CachePath, Kit.NewGuid + "-t.jpg");
                using (var fs = File.Create(fd.ThumbPath))
                {
                    // 默认根据DPI调整缩略图大小
                    var fl = await p_file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, FileData.ThumbSize, ThumbnailOptions.ResizeThumbnail);
                    await fl.AsStreamForRead().CopyToAsync(fs);
                }
            }
            return fd;
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