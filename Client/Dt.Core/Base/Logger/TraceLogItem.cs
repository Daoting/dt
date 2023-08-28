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
        string _msg;
        string _detial;

        /// <summary>
        /// 日志项
        /// </summary>
        public LogEvent Log { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Time => Log.Timestamp.ToString("HH:mm:ss");

        /// <summary>
        /// 日志源和级别
        /// </summary>
        public string LevelAndSource
        {
            get
            {
                string title;
                if (Log.Properties.TryGetValue("SourceContext", out var val))
                {
                    // 含日志来源，不显示命名空间，后缀为级别
                    title = val.ToString("l", null);
                    int index = title.LastIndexOf('.');
                    if (index > -1)
                        title = title.Substring(index + 1);

                    title = $"{title} — {Log.Level}";
                }
                else
                {
                    title = Log.Level.ToString();
                }
                return title;
            }
        }

        /// <summary>
        /// Title属性或消息内容
        /// </summary>
        public string Message
        {
            get
            {
                if (_msg == null)
                {
                    if (Log.Properties.TryGetValue("Title", out var vtitle)
                        && vtitle.ToString("l", null) is string msg)
                    {
                        // 内置标题属性
                        _msg = msg;
                    }
                    else
                    {
                        using (var buffer = new StringWriter())
                        {
                            _ftMessage.Format(Log, buffer);
                            _msg = buffer.ToString().Trim();
                            if (_msg.Length > 100)
                            {
                                _msg = _msg.Substring(0, 100) + "...";
                            }
                        }
                    }
                }
                return _msg;
            }
        }

        /// <summary>
        /// 日志项的详细内容
        /// </summary>
        public string Detial
        {
            get
            {
                if (_detial == null)
                {
                    using (var buffer = new StringWriter())
                    {
                        _ftAll.Format(Log, buffer);
                        _detial = buffer.ToString().Trim();
                    }
                }
                return _detial;
            }
        }
    }
}