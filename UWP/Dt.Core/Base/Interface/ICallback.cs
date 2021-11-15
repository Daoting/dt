#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统回调接口
    /// </summary>
    public interface ICallback
    {
        /// <summary>
        /// 显示登录页面
        /// </summary>
        /// <param name="p_isPopup">是否为弹出式</param>
        void Login(bool p_isPopup);

        /// <summary>
        /// 注销后重新登录
        /// </summary>
        /// <returns></returns>
        void Logout();

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        /// <param name="p_content">消息内容</param>
        /// <param name="p_title">标题</param>
        /// <returns>true表确认</returns>
        Task<bool> Confirm(string p_content, string p_title);

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="p_content">消息内容</param>
        /// <param name="p_title">标题</param>
        void Error(string p_content, string p_title);

        /// <summary>
        /// 根据窗口/视图类型和参数激活旧窗口、打开新窗口 或 自定义启动(IView)
        /// </summary>
        /// <param name="p_type">窗口/视图类型</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">初始参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        object OpenWin(Type p_type, string p_title, Icons p_icon, object p_params);

        /// <summary>
        /// 根据视图名称激活旧窗口或打开新窗口
        /// </summary>
        /// <param name="p_viewName">窗口视图名称</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">启动参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        object OpenView(string p_viewName, string p_title, Icons p_icon, object p_params);

        /// <summary>
        /// 显示监视窗口
        /// </summary>
        void ShowTraceBox();

        /// <summary>
        /// 挂起时的处理，必须耗时小！
        /// 手机或PC平板模式下不占据屏幕时触发，此时不确定被终止还是可恢复
        /// </summary>
        /// <returns></returns>
        Task OnSuspending();

        /// <summary>
        /// 恢复会话时的处理，手机或PC平板模式下再次占据屏幕时触发
        /// </summary>
        void OnResuming();

        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        Task<FileData> PickImage();

        /// <summary>
        /// 选择多个图片
        /// </summary>
        /// <returns></returns>
        Task<List<FileData>> PickImages();

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        Task<FileData> PickVideo();

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        Task<List<FileData>> PickVideos();

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        Task<FileData> PickAudio();

        /// <summary>
        /// 选择多个音频文件
        /// </summary>
        /// <returns></returns>
        Task<List<FileData>> PickAudios();

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        Task<FileData> PickMedia();

        /// <summary>
        /// 选择多个媒体文件
        /// </summary>
        /// <returns></returns>
        Task<List<FileData>> PickMedias();

        /// <summary>
        /// 选择单个文件
        /// </summary>
        /// <param name="p_fileTypes">
        /// uwp文件过滤类型，如 .png .docx，null时不过滤
        /// android文件过滤类型，如 image/png image/*，null时不过滤
        /// ios文件过滤类型，如 UTType.Image，null时不过滤
        /// </param>
        /// <returns></returns>
        Task<FileData> PickFile(string[] p_fileTypes);

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <param name="p_fileTypes">
        /// uwp文件过滤类型，如 .png .docx，null时不过滤
        /// android文件过滤类型，如 image/png image/*，null时不过滤
        /// ios文件过滤类型，如 UTType.Image，null时不过滤
        /// </param>
        /// <returns></returns>
        Task<List<FileData>> PickFiles(string[] p_fileTypes);

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="p_options">选项</param>
        /// <returns>照片文件信息，失败或放弃时返回null</returns>
        Task<FileData> TakePhoto(CapturePhotoOptions p_options);

        /// <summary>
        /// 录像
        /// </summary>
        /// <param name="p_options">选项</param>
        /// <returns>视频文件信息，失败或放弃时返回null</returns>
        Task<FileData> TakeVideo(CaptureVideoOptions p_options);

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="p_target">计时对话框居中的目标</param>
        /// <returns>录音文件信息，失败或放弃时返回null</returns>
        Task<FileData> TakeAudio(FrameworkElement p_target);

        /// <summary>
        /// 加载文件服务的图片，优先加载缓存，支持路径 或 FileList中json格式
        /// </summary>
        /// <param name="p_path">路径或FileList中json格式</param>
        /// <param name="p_img"></param>
        Task LoadImage(string p_path, Image p_img);
    }
}