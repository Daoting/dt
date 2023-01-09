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

namespace Dt.Sample
{
    /// <summary>
    /// 日志设置
    /// </summary>
    class LogSetting : ILogSetting
    {
        /// <summary>
        /// 是否将日志输出到Console
        /// </summary>
        public bool ConsoleEnabled => true;

        /// <summary>
        /// 是否将日志保存到文件
        /// </summary>
        public bool FileEnabled => true;

        /// <summary>
        /// 是否将日志输出到Trace
        /// </summary>
        public bool TraceEnabled => true;

        /// <summary>
        /// 日志输出级别
        /// </summary>
        public LogEventLevel LogLevel => LogEventLevel.Debug;
    }
}