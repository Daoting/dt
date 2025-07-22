#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 日志设置
    /// </summary>
    public class LogSetting
    {
        /// <summary>
        /// 处理日志的最小级别，低于最小级别的不处理
        /// </summary>
        public LogEventLevel MinimumLevel { get; set; }

        /// <summary>
        /// 是否将日志输出到Console
        /// </summary>
        public bool ConsoleEnabled { get; set; }

        /// <summary>
        /// 输出到Console的级别
        /// </summary>
        public LogEventLevel ConsoleLogLevel { get; set; }

        /// <summary>
        /// 是否将日志保存到文件
        /// </summary>
        public bool FileEnabled { get; set; }

        /// <summary>
        /// 日志输出到文件的级别
        /// </summary>
        public LogEventLevel FileLogLevel { get; set; }

        /// <summary>
        /// 是否将日志输出到Trace
        /// </summary>
        public bool TraceEnabled { get; set; }

        /// <summary>
        /// 日志到Trace的级别
        /// </summary>
        public LogEventLevel TraceLogLevel { get; set; }
    }
}