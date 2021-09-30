#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 基础功能杂项
    /// </summary>
    public partial class Kit
    {
        #region 提示信息
        /// <summary>
        /// 发布消息提示
        /// </summary>
        /// <param name="p_content">显示内容</param>
        /// <param name="p_delaySeconds">几秒后自动关闭，默认3秒，0表示不自动关闭</param>
        public static void Msg(string p_content, int p_delaySeconds = 3)
        {
            if (string.IsNullOrEmpty(p_content))
                return;

            NotifyInfo notify = new NotifyInfo();
            notify.Message = p_content;
            notify.NotifyType = NotifyType.Information;
            notify.DelaySeconds = p_delaySeconds;
            RunAsync(() => SysVisual.NotifyList.Add(notify));
        }

        /// <summary>
        /// 警告提示
        /// </summary>
        /// <param name="p_content">显示内容</param>
        /// <param name="p_delaySeconds">几秒后自动关闭，默认5秒，0表示不自动关闭</param>
        public static void Warn(string p_content, int p_delaySeconds = 5)
        {
            if (string.IsNullOrEmpty(p_content))
                return;

            NotifyInfo notify = new NotifyInfo();
            notify.Message = p_content;
            notify.NotifyType = NotifyType.Warning;
            notify.DelaySeconds = p_delaySeconds;
            RunAsync(() => SysVisual.NotifyList.Add(notify));
        }

        /// <summary>
        /// 发布消息提示
        /// </summary>
        /// <param name="p_notify">消息提示实例</param>
        public static void Notify(NotifyInfo p_notify)
        {
            if (p_notify != null && !string.IsNullOrEmpty(p_notify.Message))
                RunAsync(() => SysVisual.NotifyList.Add(p_notify));
        }

        /// <summary>
        /// 关闭消息提示，通常在连接按钮中执行关闭
        /// </summary>
        /// <param name="p_notify"></param>
        public static void CloseNotify(NotifyInfo p_notify)
        {
            if (p_notify != null)
                RunAsync(() => SysVisual.NotifyList.Remove(p_notify));
        }
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
            return Callback.Confirm(p_content, string.IsNullOrEmpty(p_title) ? "请确认" : p_title);
        }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="p_content">消息内容</param>
        /// <param name="p_title">标题</param>
        public static void Error(string p_content, string p_title = null)
        {
            Callback.Error(p_content, string.IsNullOrEmpty(p_title) ? "出错提示" : p_title);
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
            return Callback.OpenWin(p_type, p_title, p_icon, p_params);
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
            return Callback.OpenView(p_viewName, p_title, p_icon, p_params);
        }

        /// <summary>
        /// 获取视图类型
        /// </summary>
        /// <param name="p_typeName">类型名称</param>
        /// <returns>返回类型</returns>
        public static Type GetViewType(string p_typeName)
        {
            Type tp;
            if (!string.IsNullOrEmpty(p_typeName) && Stub.ViewTypes.TryGetValue(p_typeName, out tp))
                return tp;
            return null;
        }
        #endregion

        #region UI线程调用
        /// <summary>
        /// 确保在UI线程异步调用给定方法
        /// </summary>
        /// <param name="p_action"></param>
        public static async void RunAsync(Action p_action)
        {
            await SysVisual.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(p_action));
        }

        /// <summary>
        /// 确保在UI线程异步调用给定方法，返回可等待任务
        /// </summary>
        /// <param name="p_action"></param>
        /// <returns></returns>
        public static Task RunTask(Action p_action)
        {
            return SysVisual.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(p_action)).AsTask();
        }

        /// <summary>
        /// 确保在UI线程同步调用给定方法
        /// </summary>
        /// <param name="p_action"></param>
        public static void RunSync(Action p_action)
        {
            if (SysVisual.Dispatcher.HasThreadAccess)
                p_action();
            else
                WindowsRuntimeSystemExtensions.AsTask(SysVisual.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(p_action))).Wait();
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 获取新Guid，小写无连字符'-'
        /// </summary>
        public static string NewGuid => Guid.NewGuid().ToString("N");

        /// <summary>
        /// 将字节长度转成描述信息
        /// </summary>
        /// <param name="p_size"></param>
        /// <returns></returns>
        public static string GetFileSizeDesc(ulong p_size)
        {
            if (p_size < KB)
                return string.Format("{0} 字节", p_size);
            if (p_size < MB)
                return string.Format("{0} KB", Math.Round(p_size / (float)KB, 2));
            if (p_size < GB)
                return string.Format("{0} MB", Math.Round(p_size / (float)MB, 2));
            return string.Format("{0} GB", Math.Round(p_size / (float)GB, 2));
        }

        /// <summary>
        /// 计算字符串在oracle中占的长度
        /// </summary>
        /// <param name="p_content">要计算的字符串 </param>
        /// <returns></returns>
        public static int GetVarcharLength(string p_content)
        {
            if (string.IsNullOrEmpty(p_content))
                return 0;

            int length = 0;
            byte[] content = Encoding.Unicode.GetBytes(p_content);
            foreach (byte item in content)
            {
                if (item != 0)
                {
                    length++;
                }
            }
            return length;
        }

        /// <summary>
        /// 转换特殊字符 导出Xaml时用
        /// </summary>
        /// <param name="p_txt"></param>
        /// <returns></returns>
        public static string ConvertSpecialStr(string p_txt)
        {
            p_txt = Regex.Replace(p_txt, "&", "&amp;");
            p_txt = Regex.Replace(p_txt, ">", "&gt;");
            p_txt = Regex.Replace(p_txt, "<", "&lt;");
            p_txt = Regex.Replace(p_txt, "\"", "&quot;");
            p_txt = Regex.Replace(p_txt, "'", "&apos;");
            p_txt = Regex.Replace(p_txt, "{", "{}{");
            return p_txt;
        }

        /// <summary>
        /// 转换对象的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_val"></param>
        /// <returns></returns>
        public static T ConvertType<T>(object p_val)
        {
            if (p_val != null)
            {
                if (typeof(T) == p_val.GetType())
                {
                    // 结果对象与给定对象类型相同时
                    return (T)p_val;
                }

                object val = null;
                try
                {
                    val = Convert.ChangeType(p_val, typeof(T));
                }
                catch
                {
                    throw new Exception(string.Format("无法将【{0}】转换到【{1}】类型！", val, typeof(T)));
                }
                return (T)val;
            }
            return default(T);
        }

        /// <summary>
        /// 获得给定字符串的字节个数
        /// </summary>
        /// <param name="p_txt"></param>
        /// <returns></returns>
        public static int GetByteCount(string p_txt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(p_txt);
            int count = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i % 2 == 0)
                    count++;
                else if (bytes[i] > 0)
                    count++;
            }
            return count;
        }

        #endregion

        #region 可视区域
        /// <summary>
        /// 可视区域宽度
        /// 手机：页面宽度
        /// PC上：除标题栏和外框的窗口内部宽度
        /// </summary>
        public static double ViewWidth => Window.Current.Bounds.Width;

        /// <summary>
        /// 可视区域高度
        /// 手机：不包括状态栏的高度
        /// PC上：除标题栏和外框的窗口内部高度
        /// </summary>
        public static double ViewHeight => Window.Current.Bounds.Height - SysVisual.StatusBarHeight;
        #endregion

        #region 常量
        /// <summary>
        /// 1GB
        /// </summary>
        public const int GB = 1024 * 1024 * 1024;

        /// <summary>
        /// 1MB
        /// </summary>
        public const int MB = 1024 * 1024;

        /// <summary>
        /// 1KB
        /// </summary>
        public const int KB = 1024;
        #endregion
    }
}