#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.IO;
using System.Text;
#endregion

namespace Dt.Core.HtmlLog
{
    class HtmlSink : ILogEventSink
    {
        readonly ITextFormatter _formatter;
        readonly object _syncRoot;

        const int DefaultWriteBufferCapacity = 256;

        public HtmlSink(
            ITextFormatter formatter,
            object syncRoot)
        {
            _formatter = formatter;
            _syncRoot = syncRoot ?? throw new ArgumentNullException(nameof(syncRoot));
        }

        public void Emit(LogEvent logEvent)
        {
            lock (_syncRoot)
            {
                var sw = new StringWriter(new StringBuilder(DefaultWriteBufferCapacity));
                _formatter.Format(logEvent, sw);
                HtmlLogHub.AddLog(sw.ToString());
            }
        }
    }
}
