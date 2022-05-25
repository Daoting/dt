#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: AtSys合并到Kit
* 日志: 2021-06-08 改名
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 方便使用Stub内部方法
    /// </summary>
    public partial class Kit
    {
        #region 登录注销主页
        /// <summary>
        /// 显示登录页面，参数：是否为弹出式
        /// </summary>
        public static Action<bool> ShowLogin => Stub.Inst.ShowLogin;

        /// <summary>
        /// 注销后重新登录
        /// </summary>
        public static Action Logout => Stub.Inst.Logout;

        /// <summary>
        /// 加载根内容 Desktop/Frame 和主页
        /// </summary>
        public static Action ShowHome => Stub.Inst.ShowHome;

        /// <summary>
        /// 当前登录页面类型，未设置时采用 DefaultLogin
        /// </summary>
        public static Type LoginPageType => Stub.Inst.LoginPageType;

        /// <summary>
        /// 主页类型
        /// </summary>
        public static Type HomePageType => Stub.Inst.HomePageType;
        #endregion

        #region 窗口对话框
        /// <summary>
        /// 显示确认对话框
        /// </summary>
        /// <param name="p_content">消息内容</param>
        /// <param name="p_title">标题</param>
        /// <returns>true表确认</returns>
        public static Task<bool> Confirm(string p_content, string p_title = null)
        {
            return Stub.Inst.Confirm(p_content, string.IsNullOrEmpty(p_title) ? "请确认" : p_title);
        }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="p_content">消息内容</param>
        /// <param name="p_title">标题</param>
        public static void Error(string p_content, string p_title = null)
        {
            Stub.Inst.Error(p_content, string.IsNullOrEmpty(p_title) ? "出错提示" : p_title);
        }

        /// <summary>
        /// 根据窗口/视图类型和参数激活旧窗口、打开新窗口 或 自定义启动(IView)
        /// </summary>
        /// <param name="p_type">窗口/视图类型</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">初始参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenWin(
            Type p_type,
            string p_title = null,
            Icons p_icon = Icons.None,
            object p_params = null)
        {
            return Stub.Inst.OpenWin(p_type, p_title, p_icon, p_params);
        }

        /// <summary>
        /// 根据视图名称激活旧窗口或打开新窗口
        /// </summary>
        /// <param name="p_viewName">窗口视图名称</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">启动参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenView(
            string p_viewName,
            string p_title = null,
            Icons p_icon = Icons.None,
            object p_params = null)
        {
            return Stub.Inst.OpenView(p_viewName, p_title, p_icon, p_params);
        }

        /// <summary>
        /// 获取视图类型
        /// </summary>
        /// <param name="p_typeName">类型名称</param>
        /// <returns>返回类型</returns>
        public static Type GetViewType(string p_typeName)
        {
            Type tp;
            if (!string.IsNullOrEmpty(p_typeName) && Stub.Inst.ViewTypes.TryGetValue(p_typeName, out tp))
                return tp;
            return null;
        }
        #endregion

        #region 系统
        /// <summary>
        /// 系统标题
        /// </summary>
        public static string Title => Stub.Inst.Title;

        /// <summary>
        /// 按默认流程开始运行，加载根页面。初次启动或刷新到根页面时调用
        /// <para>1. 记录主页和登录页的类型，以备登录、注销、自动登录、中途登录时用</para>
        /// <para>2. 不使用dt服务时，直接显示主页</para>
        /// <para>3. 已登录过，先自动登录</para>
        /// <para>4. 未登录或登录失败时，根据 p_loginFirst 显示登录页或主页</para>
        /// </summary>
        /// <param name="p_homePageType">主页类型，null时采用默认主页 DefaultHome</param>
        /// <param name="p_loginFirst">是否强制先登录，默认true</param>
        /// <param name="p_loginPageType">登录页类型，null时采用默认登录页 DefaultLogin</param>
        /// <returns></returns>
        public static Task StartRun(Type p_homePageType = null, bool p_loginFirst = true, Type p_loginPageType = null)
        {
            return Stub.Inst.StartRun(p_homePageType, p_loginFirst, p_loginPageType);
        }

        /// <summary>
        /// 注册接收服务器推送
        /// </summary>
        public static Action RegisterSysPush => Stub.Inst.RegisterSysPush;

        /// <summary>
        /// 主动停止接收推送
        /// </summary>
        public static Action StopSysPush => Stub.Inst.StopSysPush;

        /// <summary>
        /// 显示系统日志窗口
        /// </summary>
        public static Action ShowTraceBox => Stub.Inst.ShowTraceBox;
        #endregion

        #region 选择文件
        /// <summary>
        /// 选择单个图片
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickImage()
        {
            return Stub.Inst.PickImage();
        }

        /// <summary>
        /// 选择多个图片
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickImages()
        {
            return Stub.Inst.PickImages();
        }

        /// <summary>
        /// 选择单个视频
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickVideo()
        {
            return Stub.Inst.PickVideo();
        }

        /// <summary>
        /// 选择多个视频
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickVideos()
        {
            return Stub.Inst.PickVideos();
        }

        /// <summary>
        /// 选择单个音频文件
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickAudio()
        {
            return Stub.Inst.PickAudio();
        }

        /// <summary>
        /// 选择多个音频文件
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickAudios()
        {
            return Stub.Inst.PickAudios();
        }

        /// <summary>
        /// 选择单个媒体文件
        /// </summary>
        /// <returns></returns>
        public static Task<FileData> PickMedia()
        {
            return Stub.Inst.PickMedia();
        }

        /// <summary>
        /// 选择多个媒体文件
        /// </summary>
        /// <returns></returns>
        public static Task<List<FileData>> PickMedias()
        {
            return Stub.Inst.PickMedias();
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
            return Stub.Inst.PickFile(p_fileTypes);
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
            return Stub.Inst.PickFiles(p_fileTypes);
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
            return Stub.Inst.TakePhoto(p_options);
        }

        /// <summary>
        /// 录像
        /// </summary>
        /// <param name="p_options">选项</param>
        /// <returns>视频文件信息，失败或放弃时返回null</returns>
        public static Task<FileData> TakeVideo(CaptureVideoOptions p_options = null)
        {
            return Stub.Inst.TakeVideo(p_options);
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="p_target">计时对话框居中的目标</param>
        /// <returns>录音文件信息，失败或放弃时返回null</returns>
        public static Task<FileData> TakeAudio(FrameworkElement p_target)
        {
            return Stub.Inst.TakeAudio(p_target);
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
            return Stub.Inst.LoadImage(p_path, p_img);
        }
        #endregion
    }
}