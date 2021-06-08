#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 跨平台工具集：选择图片、视频、音频文件，拍照、录像、录音
    /// </summary>
    internal partial class DefaultCallback : ICallback
    {
        #region 选择文件
        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickImage()
        {
            return new FilePicker().PickImage();
        }

        /// <summary>
        /// 选择多个图片
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickImages()
        {
            return new FilePicker().PickImages();
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickVideo()
        {
            return new FilePicker().PickVideo();
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickVideos()
        {
            return new FilePicker().PickVideos();
        }

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickAudio()
        {
            return new FilePicker().PickAudio();
        }

        /// <summary>
        /// 选择多个音频文件
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickAudios()
        {
            return new FilePicker().PickAudios();
        }

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        public Task<FileData> PickMedia()
        {
            return new FilePicker().PickMedia();
        }

        /// <summary>
        /// 选择多个媒体文件
        /// </summary>
        /// <returns></returns>
        public Task<List<FileData>> PickMedias()
        {
            return new FilePicker().PickMedias();
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
        public Task<FileData> PickFile(string[] p_fileTypes = null)
        {
            return new FilePicker().PickFile(p_fileTypes);
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
        public Task<List<FileData>> PickFiles(string[] p_fileTypes = null)
        {
            return new FilePicker().PickFiles(p_fileTypes);
        }
        #endregion

        #region 拍照录像录音
        // 参见 https://github.com/jamesmontemagno/MediaPlugin

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="p_options">选项</param>
        /// <returns>照片文件信息，失败或放弃时返回null</returns>
        public Task<FileData> TakePhoto(CapturePhotoOptions p_options = null)
        {
            return new CameraCapture().TakePhoto(p_options);
        }

        /// <summary>
        /// 录像
        /// </summary>
        /// <param name="p_options">选项</param>
        /// <returns>视频文件信息，失败或放弃时返回null</returns>
        public Task<FileData> TakeVideo(CaptureVideoOptions p_options = null)
        {
            return new CameraCapture().TakeVideo(p_options);
        }

        AudioRecorder _audioRecorder;

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="p_target">计时对话框居中的目标</param>
        /// <returns>录音文件信息，失败或放弃时返回null</returns>
        public async Task<FileData> TakeAudio(FrameworkElement p_target)
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
    }
}