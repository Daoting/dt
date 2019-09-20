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
        /// 保存调试级别日志
        /// </summary>
        /// <param name="p_msg"></param>
        public static void Debug(string p_msg)
        {
            Save(LogEventLevel.Debug, p_msg, null);
        }

        /// <summary>
        /// 保存调试级别日志
        /// </summary>
        /// <param name="p_ex"></param>
        /// <param name="p_msg"></param>
        public static void Debug(Exception p_ex, string p_msg = null)
        {
            Save(LogEventLevel.Debug, p_msg, p_ex);
        }

        /// <summary>
        /// 保存警告级别日志
        /// </summary>
        /// <param name="p_msg"></param>
        public static void Warning(string p_msg)
        {
            Save(LogEventLevel.Warning, p_msg, null);
        }

        /// <summary>
        /// 保存警告级别日志
        /// </summary>
        /// <param name="p_ex"></param>
        /// <param name="p_msg"></param>
        public static void Warning(Exception p_ex, string p_msg = null)
        {
            Save(LogEventLevel.Warning, p_msg, p_ex);
        }

        /// <summary>
        /// 保存错误级别日志
        /// </summary>
        /// <param name="p_msg"></param>
        public static void Error(string p_msg)
        {
            Save(LogEventLevel.Error, p_msg, null);
        }

        /// <summary>
        /// 保存错误级别日志
        /// </summary>
        /// <param name="p_ex"></param>
        /// <param name="p_msg"></param>
        public static void Error(Exception p_ex, string p_msg = null)
        {
            Save(LogEventLevel.Error, p_msg, p_ex);
        }

        /// <summary>
        /// 保存客户端日志
        /// </summary>
        /// <param name="p_level"></param>
        /// <param name="p_msg"></param>
        /// <param name="p_ex"></param>
        static void Save(LogEventLevel p_level, string p_msg, Exception p_ex)
        {
            ClientLog log = new ClientLog
            {
                Level = p_level,
                CTime = DateTime.Now
            };
            string msg = p_msg == null ? "" : p_msg + "\r\n";
            if (p_ex != null)
                msg += p_ex.Message;
            log.Content = msg;
            AtLocal.Insert(log);
        }
    }
}