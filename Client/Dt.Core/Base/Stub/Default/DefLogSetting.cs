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
    /// 默认日志设置
    /// </summary>
    class DefLogSetting : ILogSetting
    {
#if DEBUG
        public LogEventLevel MinimumLevel => LogEventLevel.Debug;
        public bool ConsoleEnabled => true;
        public bool FileEnabled => true;
        public bool TraceEnabled => true;
        public LogEventLevel FileLogLevel => LogEventLevel.Information;
#else
        public LogEventLevel MinimumLevel => LogEventLevel.Warning;
        public bool ConsoleEnabled => false;
        public bool FileEnabled => true;
        public bool TraceEnabled => false;
        public LogEventLevel FileLogLevel => LogEventLevel.Warning;
#endif

        public LogEventLevel ConsoleLogLevel => LogEventLevel.Debug;
        public LogEventLevel TraceLogLevel => LogEventLevel.Debug;
    }
}