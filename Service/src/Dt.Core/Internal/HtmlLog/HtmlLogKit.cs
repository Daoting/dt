#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.HtmlLog;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.Net;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 参照 https://github.com/serilog/serilog-sinks-console
    /// </summary>
    public static class HtmlLogKit
    {
        static readonly object DefaultSyncRoot = new object();
        const string DefaultConsoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// 扩展方法 WriteTo.Html()
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="outputTemplate"></param>
        /// <returns></returns>
        public static LoggerConfiguration Html(
            this LoggerSinkConfiguration sinkConfiguration,
            string outputTemplate = DefaultConsoleOutputTemplate)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputTemplate is null) throw new ArgumentNullException(nameof(outputTemplate));

            var formatter = new OutputFormatter(outputTemplate);
            return sinkConfiguration.Sink(new HtmlSink(formatter, DefaultSyncRoot), LevelAlias.Minimum, null);
        }
    }
}
