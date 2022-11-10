#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: AtSys合并到Kit
* 日志: 2021-06-08 改名
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 方便使用Stub内部方法
    /// </summary>
    public partial class Kit
    {
        #region 提示信息
        /// <summary>
        /// 属性，不缓存对象
        /// </summary>
        static INotify _notify => Stub.Inst.SvcProvider.GetRequiredService<INotify>();

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
        public static NotifyInfo Msg(string p_content, int p_delaySeconds = 3)
        {
            return _notify.Msg(p_content, p_delaySeconds);
        }

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
        public static NotifyInfo Warn(string p_content, int p_delaySeconds = 5)
        {
            return _notify.Warn(p_content, p_delaySeconds);
        }

        /// <summary>
        /// 发布消息提示
        /// </summary>
        /// <param name="p_notify">消息提示实例</param>
        public static void Notify(NotifyInfo p_notify)
        {
            _notify.Notify(p_notify);
        }

        /// <summary>
        /// 关闭消息提示，通常在连接按钮中执行关闭
        /// </summary>
        /// <param name="p_notify"></param>
        public static void CloseNotify(NotifyInfo p_notify)
        {
            _notify.CloseNotify(p_notify);
        }

        /// <summary>
        /// 获取提示信息列表
        /// </summary>
        public static IList<NotifyInfo> NotifyList => _notify.NotifyList;
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
        /// 根据视图名称激活旧窗口、打开新窗口 或 自定义启动(IView)
        /// </summary>
        /// <param name="p_viewAlias">视图别名</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">启动参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenView(
            string p_viewAlias,
            string p_title = null,
            Icons p_icon = Icons.None,
            object p_params = null)
        {
            Type tp = GetViewTypeByAlias(p_viewAlias);
            if (tp == null)
            {
                Msg(string.Format("【{0}】视图未找到！", p_viewAlias));
                return null;
            }
            return OpenWin(tp, p_title, p_icon, p_params);
        }

        /// <summary>
        /// 根据别名获取视图类型
        /// </summary>
        /// <param name="p_alias">类型别名</param>
        /// <returns>返回类型</returns>
        public static Type GetViewTypeByAlias(string p_alias)
        {
            return Stub.Inst.GetTypesByAlias(typeof(ViewAttribute), p_alias).FirstOrDefault();
        }

        /// <summary>
        /// 根据别名获取视图类型
        /// </summary>
        /// <param name="p_enumAlias">别名取枚举成员名称</param>
        /// <returns>返回类型</returns>
        public static Type GetViewTypeByAlias(Enum p_enumAlias)
        {
            return Stub.Inst.GetTypesByAlias(typeof(ViewAttribute), p_enumAlias.ToString()).FirstOrDefault();
        }

        /// <summary>
        /// 根据类型别名获取所有类型列表
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <returns>返回类型</returns>
        public static List<Type> GetAllTypesByAlias(Type p_attrType, string p_alias)
        {
            return Stub.Inst.GetTypesByAlias(p_attrType, p_alias);
        }

        /// <summary>
        /// 根据类型别名和方法名获取方法定义，取列表中第一个匹配项
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_flags">要包含BindingFlags.Public，否则返回null，静态的方法还要有BindingFlags.Static</param>
        /// <returns></returns>
        public static MethodInfo GetMethodByAlias(Type p_attrType, string p_alias, string p_methodName, BindingFlags p_flags)
        {
            var ls = Stub.Inst.GetTypesByAlias(p_attrType, p_alias);
            return (from tp in ls
                    let mi = tp.GetMethod(p_methodName, p_flags)
                    where mi != null
                    select mi).FirstOrDefault();
        }
        #endregion

        #region 系统
        /// <summary>
        /// 系统标题
        /// </summary>
        public static string Title => Stub.Inst.Title;

        /// <summary>
        /// 加载根内容，支持任意类型的UIElement，特殊类型有：
        /// <para>Win：PhoneUI模式加载Frame、导航到窗口主页、再导航到自启动窗口主页；Win模式加载桌面、打开窗口、再打开自启动窗口</para>
        /// <para>Page：加载Frame，导航到页面</para>
        /// <para>其余可视元素直接加载</para>
        /// </summary>
        /// <param name="p_elementType">类型：Win Page 或 任意可视元素UIElement</param>
        public static void ShowRoot(Type p_elementType)
        {
            Stub.Inst.ShowRoot(p_elementType);
        }

        /// <summary>
        /// 加载根内容，视图别名对应的视图类型可以为任意类型的 UIElement，特殊类型有：
        /// <para>Win：PhoneUI模式加载Frame、导航到窗口主页、再导航到自启动窗口主页；Win模式加载桌面、打开窗口、再打开自启动窗口</para>
        /// <para>Page：加载Frame，导航到页面</para>
        /// <para>其余可视元素直接加载</para>
        /// </summary>
        /// <param name="p_viewAlias">视图别名</param>
        public static void ShowRoot(string p_viewAlias)
        {
            Stub.Inst.ShowRoot(GetViewTypeByAlias(p_viewAlias));
        }

        /// <summary>
        /// 加载根内容，视图别名对应的视图类型可以为任意类型的 UIElement，特殊类型有：
        /// <para>Win：PhoneUI模式加载Frame、导航到窗口主页、再导航到自启动窗口主页；Win模式加载桌面、打开窗口、再打开自启动窗口</para>
        /// <para>Page：加载Frame，导航到页面</para>
        /// <para>其余可视元素直接加载</para>
        /// </summary>
        /// <param name="p_viewEnumAlias">视图别名</param>
        public static void ShowRoot(Enum p_viewEnumAlias)
        {
            Stub.Inst.ShowRoot(GetViewTypeByAlias(p_viewEnumAlias));
        }

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