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
        static ITextFormatter _ftMsg = new MessageTemplateTextFormatter("{Message:lj}{NewLine}{Exception}");
        static ITextFormatter _ftInfo = new MessageTemplateTextFormatter("[{Timestamp:HH:mm:ss.fff} {Level:u3}] {src}");
        string _info;
        string _msg;

        /// <summary>
        /// 日志项
        /// </summary>
        public LogEvent Log { get; set; }

        /// <summary>
        /// 日志描述信息
        /// </summary>
        public string Info
        {
            get
            {
                if (_info == null)
                {
                    using (var buffer = new StringWriter())
                    {
                        _ftInfo.Format(Log, buffer);
                        _info = buffer.ToString().Trim();
                    }
                }
                return _info;
            }
        }

        /// <summary>
        /// 日志详细内容
        /// </summary>
        public string Msg
        {
            get
            {
                if (_msg == null)
                {
                    using (var buffer = new StringWriter())
                    {
                        _ftMsg.Format(Log, buffer);
                        _msg = buffer.ToString().Trim();
                    }
                }
                return _msg;
            }
        }
    }
}