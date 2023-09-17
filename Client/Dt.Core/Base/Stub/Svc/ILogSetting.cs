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
    public interface ILogSetting
    {
        /// <summary>
        /// 是否将日志输出到Console
        /// </summary>
        bool ConsoleEnabled { get; }

        /// <summary>
        /// 输出到Console的级别
        /// </summary>
        LogEventLevel ConsoleLogLevel { get; }

        /// <summary>
        /// 是否将日志保存到文件
        /// </summary>
        bool FileEnabled { get; }

        /// <summary>
        /// 日志输出到文件的级别
        /// </summary>
        LogEventLevel FileLogLevel { get; }

        /// <summary>
        /// 是否将日志输出到Trace
        /// </summary>
        bool TraceEnabled { get; }

        /// <summary>
        /// 日志到Trace的级别
        /// </summary>
        LogEventLevel TraceLogLevel { get; }
    }
}