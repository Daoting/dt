#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端日志，用法和服务端Serilog一致
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// 输出调试级别日志，不保存
        /// </summary>
        /// <param name="p_msg"></param>
        public static void Debug(string p_msg)
        {
#if UWP
            System.Diagnostics.Debug.WriteLine(p_msg);
#else
            Console.WriteLine(p_msg);
#endif
        }

        /// <summary>
        /// 输出调试级别日志，不保存
        /// </summary>
        /// <param name="p_ex"></param>
        /// <param name="p_msg"></param>
        public static void Debug(Exception p_ex, string p_msg = null)
        {
            string msg = p_msg == null ? "" : p_msg + "\r\n";
            if (p_ex != null)
                msg += p_ex.Message;
#if UWP
            System.Diagnostics.Debug.WriteLine(msg);
#else
            Console.WriteLine(msg);
#endif
        }

        /// <summary>
        /// 输出并保存警告级别日志
        /// </summary>
        /// <param name="p_msg"></param>
        public static void Warning(string p_msg)
        {
            OutputAndSave(LogEventLevel.Warning, p_msg, null);
        }

        /// <summary>
        /// 输出并保存警告级别日志
        /// </summary>
        /// <param name="p_ex"></param>
        /// <param name="p_msg"></param>
        public static void Warning(Exception p_ex, string p_msg = null)
        {
            OutputAndSave(LogEventLevel.Warning, p_msg, p_ex);
        }

        /// <summary>
        /// 输出并保存错误级别日志
        /// </summary>
        /// <param name="p_msg"></param>
        public static void Error(string p_msg)
        {
            OutputAndSave(LogEventLevel.Error, p_msg, null);
        }

        /// <summary>
        /// 输出并保存错误级别日志
        /// </summary>
        /// <param name="p_ex"></param>
        /// <param name="p_msg"></param>
        public static void Error(Exception p_ex, string p_msg = null)
        {
            OutputAndSave(LogEventLevel.Error, p_msg, p_ex);
        }

        /// <summary>
        /// 输出并保存客户端日志
        /// </summary>
        /// <param name="p_level"></param>
        /// <param name="p_msg"></param>
        /// <param name="p_ex"></param>
        static void OutputAndSave(LogEventLevel p_level, string p_msg, Exception p_ex)
        {
            string msg = p_msg == null ? "" : p_msg + "\r\n";
            if (p_ex != null)
                msg += p_ex.Message;

#if UWP
            System.Diagnostics.Debug.WriteLine(msg);
#else
            Console.Error.WriteLine(msg);
#endif

            ClientLog log = new ClientLog(
                Level: p_level,
                Content: msg,
                Ctime: DateTime.Now);
            _ = AtState.Save(log, false);
        }
    }
}