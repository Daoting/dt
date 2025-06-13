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

namespace Demo
{
    /// <summary>
    /// 日志设置
    /// </summary>
    class LogSetting : ILogSetting
    {
        /// <summary>
        /// 处理日志的最小级别，低于最小级别的不处理
        /// </summary>
        public LogEventLevel MinimumLevel => LogEventLevel.Debug;
        
        /// <summary>
        /// 是否将日志输出到Console，默认true
        /// </summary>
        public bool ConsoleEnabled => true;

        /// <summary>
        /// 日志输出到Console的级别，默认Debug
        /// </summary>
        public LogEventLevel ConsoleLogLevel => LogEventLevel.Debug;

        /// <summary>
        /// 是否将日志保存到文件，默认true
        /// </summary>
        public bool FileEnabled => true;

        /// <summary>
        /// 日志输出到文件的级别，默认Info
        /// </summary>
        public LogEventLevel FileLogLevel => LogEventLevel.Information;

        /// <summary>
        /// 是否将日志输出到Trace，默认true
        /// </summary>
        public bool TraceEnabled => true;

        /// <summary>
        /// 日志到Trace的级别，默认Debug
        /// </summary>
        public LogEventLevel TraceLogLevel => LogEventLevel.Debug;
    }
}