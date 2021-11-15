#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 跨平台工具集：选择图片、视频、音频文件，拍照、录像、录音
    /// </summary>
    public partial class Kit
    {
        #region 选择文件
        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickImage()
        {
            return Callback.PickImage();
        }

        /// <summary>
        /// 选择多个图片
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickImages()
        {
            return Callback.PickImages();
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickVideo()
        {
            return Callback.PickVideo();
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickVideos()
        {
            return Callback.PickVideos();
        }

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickAudio()
        {
            return Callback.PickAudio();
        }

        /// <summary>
        /// 选择多个音频文件
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickAudios()
        {
            return Callback.PickAudios();
        }

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickMedia()
        {
            return Callback.PickMedia();
        }

        /// <summary>
        /// 选择多个媒体文件
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickMedias()
        {
            return Callback.PickMedias();
        }

        /// <summary>
        /// 选择单个文件
        /// </summary>
        /// <param name="p_fileTypes">
        /// uwp文件过滤类型，如 .png .docx，null时不过滤
        /// android文件过滤类型，如 image/png image/*，null时不过滤
        /// ios文件过滤类型，如 UTType.Image，null时不过滤
        /// </param>
        /// <returns></returns>
        public static Task<FileData> PickFile(string[] p_fileTypes = null)
        {
            return Callback.PickFile(p_fileTypes);
        }

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <param name="p_fileTypes">
        /// uwp文件过滤类型，如 .png .docx，null时不过滤
        /// android文件过滤类型，如 image/png image/*，null时不过滤
        /// ios文件过滤类型，如 UTType.Image，null时不过滤
        /// </param>
        /// <returns></returns>
        public static Task<List<FileData>> PickFiles(string[] p_fileTypes = null)
        {
            return Callback.PickFiles(p_fileTypes);
        }
        #endregion

        #region 拍照录像录音
        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="p_options">选项</param>
        /// <returns>照片文件信息，失败或放弃时返回null</returns>
        public static Task<FileData> TakePhoto(CapturePhotoOptions p_options = null)
        {
            return Callback.TakePhoto(p_options);
        }

        /// <summary>
        /// 录像
        /// </summary>
        /// <param name="p_options">选项</param>
        /// <returns>视频文件信息，失败或放弃时返回null</returns>
        public static Task<FileData> TakeVideo(CaptureVideoOptions p_options = null)
        {
            return Callback.TakeVideo(p_options);
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="p_target">计时对话框居中的目标</param>
        /// <returns>录音文件信息，失败或放弃时返回null</returns>
        public static Task<FileData> TakeAudio(FrameworkElement p_target)
        {
            return Callback.TakeAudio(p_target);
        }
        #endregion

        #region 打开文件
        /// <summary>
        /// 默认关联程序打开文件
        /// </summary>
        /// <param name="p_filePath">文件完整路径</param>
        public static async void OpenFile(string p_filePath)
        {
            if (File.Exists(p_filePath))
            {
                // 默认关联程序打开
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(p_filePath)
                });
            }
        }
        #endregion

        #region 加载图片
        /// <summary>
        /// 加载文件服务的图片，优先加载缓存，支持路径 或 FileList中json格式
        /// </summary>
        /// <param name="p_path">路径或FileList中json格式</param>
        /// <param name="p_img"></param>
        public static Task LoadImage(string p_path, Image p_img = null)
        {
            return Callback.LoadImage(p_path, p_img);
        }
        #endregion

        #region 系统通知
        /// <summary>
        /// 显示系统通知，iOS只有app在后台或关闭时才显示！其他平台始终显示
        /// </summary>
        /// <param name="p_title">标题</param>
        /// <param name="p_content">内容</param>
        /// <param name="p_startInfo">点击通知的启动参数</param>
        public static void Toast(string p_title, string p_content, AutoStartInfo p_startInfo = null)
        {
#if !WASM
            BgJob.Toast(p_title, p_content, p_startInfo);
#endif
        }

        /// <summary>
        /// 更新磁贴内容，最多支持四行信息
        /// </summary>
        /// <param name="p_msgs"></param>
        public static void Tile(params string[] p_msgs)
        {
#if UWP
            // 最多支持四行信息！
            int cnt = p_msgs.Length > 4 ? 4 : p_msgs.Length;
            if (cnt == 0)
                return;

            Windows.Data.Xml.Dom.XmlDocument xml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text03);
            Windows.Data.Xml.Dom.XmlNodeList nodes = xml.GetElementsByTagName("text");
            for (uint i = 0; i < cnt; i++)
            {
                nodes.Item(i).InnerText = p_msgs[i];
            }
            TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(xml));
#elif IOS
            throw new NotImplementedException();
#elif ANDROID
            throw new NotImplementedException();
#elif WASM
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// 更新磁贴数字
        /// </summary>
        /// <param name="p_num"></param>
        public static void Tile(double p_num)
        {
#if UWP
            Windows.Data.Xml.Dom.XmlDocument xml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Block);
            Windows.Data.Xml.Dom.XmlNodeList nodes = xml.GetElementsByTagName("text");
            nodes.Item(0).InnerText = p_num.ToString();
            TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(xml));
#elif IOS
            throw new NotImplementedException();
#elif ANDROID
            throw new NotImplementedException();
#elif WASM
            throw new NotImplementedException();
#endif
        }
#endregion
    }
}