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
using Serilog.Formatting;
using Serilog.Formatting.Display;
#endregion

namespace Dt.Core
{
    class ConsoleSink : ILogEventSink
    {
        readonly ITextFormatter _formatter = new MessageTemplateTextFormatter("[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        public void Emit(LogEvent logEvent)
        {
            using (var buffer = new StringWriter())
            {
                _formatter.Format(logEvent, buffer);
#if WASM || SKIA
                Console.WriteLine(buffer.ToString().Trim());
#else
                System.Diagnostics.Debug.WriteLine(buffer.ToString().Trim());
#endif
            }
        }
    }
}