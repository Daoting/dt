#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Core;
using Serilog.Events;
#endregion

namespace Dt.Core
{
    class TraceSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            TraceLogs.AddItem(logEvent);
        }
    }
}