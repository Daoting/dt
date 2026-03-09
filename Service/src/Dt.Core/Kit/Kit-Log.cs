#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-03-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 日志相关，自动添加：文件名:行号
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// Information日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="messageTemplate">日志内容</param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogInfo(
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Information, messageTemplate);
        }

        /// <summary>
        /// Information日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogInfo(
            Exception exception,
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Information, exception, messageTemplate);
        }

        /// <summary>
        /// Debug日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="messageTemplate">日志内容</param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogDebug(
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Debug, messageTemplate);
        }

        /// <summary>
        /// Debug日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogDebug(
            Exception exception,
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Debug, exception, messageTemplate);
        }

        /// <summary>
        /// Warning日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="messageTemplate">日志内容</param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogWarning(
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Warning, messageTemplate);
        }

        /// <summary>
        /// Warning日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogWarning(
            Exception exception,
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Warning, exception, messageTemplate);
        }

        /// <summary>
        /// Error日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="messageTemplate">日志内容</param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogError(
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Error, messageTemplate);
        }

        /// <summary>
        /// Error日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogError(
            Exception exception,
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Error, exception, messageTemplate);
        }

        /// <summary>
        /// Fatal日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="messageTemplate">日志内容</param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogFatal(
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Fatal, messageTemplate);
        }

        /// <summary>
        /// Fatal日志，自动添加：文件名:行号
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="p_file"></param>
        /// <param name="p_line"></param>
        public static void LogFatal(
            Exception exception,
            string messageTemplate,
            [CallerFilePath] string p_file = null,
            [CallerLineNumber] int p_line = 0)
        {
            Log.Logger
                .ForContext("src", $"{System.IO.Path.GetFileName(p_file)}:{p_line}")
                .Write(LogEventLevel.Fatal, exception, messageTemplate);
        }
    }
}
