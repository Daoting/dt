#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-05-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 跨平台工具集：选择图片、视频、音频文件，拍照、录像、录音
    /// </summary>
    public static class CrossKit
    {
        #region 选择文件
        static readonly FilePicker _filePicker = new FilePicker();

        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickImage()
        {
            return _filePicker.PickImage();
        }

        /// <summary>
        /// 选择多个图片
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickImages()
        {
            return _filePicker.PickImages();
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickVideo()
        {
            return _filePicker.PickVideo();
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickVideos()
        {
            return _filePicker.PickVideos();
        }

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickAudio()
        {
            return _filePicker.PickAudio();
        }

        /// <summary>
        /// 选择多个音频文件
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickAudios()
        {
            return _filePicker.PickAudios();
        }

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickMedia()
        {
            return _filePicker.PickMedia();
        }

        /// <summary>
        /// 选择多个媒体文件
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickMedias()
        {
            return _filePicker.PickMedias();
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
            return _filePicker.PickFile(p_fileTypes);
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
            return _filePicker.PickFiles(p_fileTypes);
        }
        #endregion

        #region 拍照录像
        static readonly CameraCapture _capture = new CameraCapture();

        // 参加 https://github.com/jamesmontemagno/MediaPlugin

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="p_options">选项</param>
        /// <returns>照片文件信息，失败或放弃时返回null</returns>
        public static Task<FileData> TakePhoto(CapturePhotoOptions p_options = null)
        {
            return _capture.TakePhoto(p_options);
        }

        /// <summary>
        /// 录像
        /// </summary>
        /// <param name="p_options">选项</param>
        /// <returns>视频文件信息，失败或放弃时返回null</returns>
        public static Task<FileData> TakeVideo(CaptureVideoOptions p_options = null)
        {
            return _capture.TakeVideo(p_options);
        }
        #endregion

        #region 录音
        static AudioRecorder _audioRecorder;

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="p_target">计时对话框居中的目标</param>
        /// <returns>录音文件信息，失败或放弃时返回null</returns>
        public static async Task<FileData> StartRecording(FrameworkElement p_target)
        {
            if (_audioRecorder == null)
                _audioRecorder = new AudioRecorder();

            if (_audioRecorder.IsRecording)
            {
                Kit.Warn("已启动录音");
                return null;
            }

            if (!await _audioRecorder.CanRecordAudio)
            {
                Kit.Warn("无麦克风设备，无法录音！");
                return null;
            }

            try
            {
                await Permissions.RequestAsync<Permissions.Microphone>();
            }
            catch
            {
                Kit.Warn("设备禁止录音！");
                return null;
            }

            _audioRecorder.IsRecording = true;
            await _audioRecorder.PlatformRecordAsync();

            // 显示计时框
            var dlg = new AudioRecordDlg();
            dlg.PlacementTarget = p_target;
            bool isOk = await dlg.ShowAsync();

            // 计时框关闭，停止录音
            FileData fd = await _audioRecorder.PlatformStopAsync();
            // 录音时长
            fd.Desc = dlg.Duration;
            _audioRecorder.IsRecording = false;

            return isOk ? fd : null;
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
    }
}