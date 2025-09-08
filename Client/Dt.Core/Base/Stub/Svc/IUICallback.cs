#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 提示信息相关
    /// </summary>
    public interface IUICallback
    {
        #region 提示信息
        /// <summary>
        /// 获取提示信息列表
        /// </summary>
        ItemList<NotifyInfo> NotifyList { get; }

        /// <summary>
        /// 发布消息提示
        /// </summary>
        /// <param name="p_content">显示内容</param>
        /// <param name="p_delaySeconds">
        /// 几秒后自动关闭，默认3秒
        /// <para>大于0：启动定时器自动关闭，点击也关闭</para>
        /// <para>0：不自动关闭，但点击关闭</para>
        /// <para>小于0：始终不关闭，只有程序控制关闭</para>
        /// </param>
        NotifyInfo Msg(string p_content, int p_delaySeconds);

        /// <summary>
        /// 警告提示
        /// </summary>
        /// <param name="p_content">显示内容</param>
        /// <param name="p_delaySeconds">
        /// 几秒后自动关闭，默认5秒
        /// <para>大于0：启动定时器自动关闭，点击也关闭</para>
        /// <para>0：不自动关闭，但点击关闭</para>
        /// <para>小于0：始终不关闭，只有程序控制关闭</para>
        /// </param>
        NotifyInfo Warn(string p_content, int p_delaySeconds);

        /// <summary>
        /// 发布消息提示
        /// </summary>
        /// <param name="p_notify">消息提示实例</param>
        void Notify(NotifyInfo p_notify);

        /// <summary>
        /// 关闭消息提示，通常在连接按钮中执行关闭
        /// </summary>
        /// <param name="p_notify"></param>
        void CloseNotify(NotifyInfo p_notify);
        #endregion

        #region 托盘通知
        /// <summary>
        /// 获取托盘通知列表
        /// </summary>
        Table AllTrayMsg { get; }

        /// <summary>
        /// 发布新托盘通知
        /// </summary>
        /// <param name="p_content">通知内容</param>
        /// <param name="p_isWarning">是否为警告通知</param>
        void TrayMsg(string p_content, bool p_isWarning);

        /// <summary>
        /// 发布新托盘通知
        /// </summary>
        /// <param name="p_notify">消息提示实例</param>
        void TrayMsg(NotifyInfo p_notify);
        #endregion
        
        #region 窗口对话框
        /// <summary>
        /// 加载根内容，支持任意类型的UIElement，特殊类型有：
        /// <para>Win：PhoneUI模式加载Frame、导航到窗口主页、再导航到自启动窗口主页；Win模式加载桌面、打开窗口、再打开自启动窗口</para>
        /// <para>Page：加载Frame，导航到页面</para>
        /// <para>其余可视元素直接加载</para>
        /// </summary>
        /// <param name="p_elementType">类型：Win Page 或 任意可视元素UIElement</param>
        void ShowRoot(Type p_elementType);

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
        /// 显示系统日志窗口
        /// </summary>
        void ShowLogBox();

        /// <summary>
        /// UI模式切换的回调方法，Phone UI 与 PC UI 切换
        /// </summary>
        void OnUIModeChanged();

        /// <summary>
        /// 主题颜色，logo图标、启动页背景色，在app项目.csprj中设置
        /// </summary>
        Brush ThemeBrush { get; }
        
        /// <summary>
        /// 标题，在app项目.csprj中设置 ApplicationTitle
        /// </summary>
        string Title { get; }
        #endregion

        #region 选择文件
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
        #endregion

        #region 拍照录像录音
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
        #endregion
    }
}