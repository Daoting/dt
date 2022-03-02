#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Trace的日志项
    /// </summary>
    public class TraceLogItem
    {
        static ITextFormatter _ftMessage = new MessageTemplateTextFormatter("{Message:lj}");
        static ITextFormatter _ftAll = new MessageTemplateTextFormatter("{Message:lj}{NewLine}{Exception}");

        /// <summary>
        /// 日志项
        /// </summary>
        public LogEvent Log { get; set; }

        /// <summary>
        /// 日志的消息内容
        /// </summary>
        public string Message
        {
            get
            {
                using (var buffer = new StringWriter())
                {
                    _ftMessage.Format(Log, buffer);
                    return buffer.ToString().Trim();
                }
            }
        }

        /// <summary>
        /// 消息 + 异常
        /// </summary>
        public string ExceptionMsg
        {
            get
            {
                using (var buffer = new StringWriter())
                {
                    _ftAll.Format(Log, buffer);
                    return buffer.ToString().Trim();
                }
            }
        }
    }
}