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
        /// 日志的消息内容，Lv中显示
        /// </summary>
        public string Message
        {
            get
            {
                if (_msg == null)
                {
                    using (var buffer = new StringWriter())
                    {
                        _ftMessage.Format(Log, buffer);
                        _msg = buffer.ToString().Trim();
                    }
                }
                return _msg;
            }
        }

        /// <summary>
        /// 日志项是否存在详细内容
        /// </summary>
        public bool ExistDetial => Log.Exception != null || Log.Properties.ContainsKey("Detail");

        /// <summary>
        /// 日志项的详细内容
        /// </summary>
        public string Detial
        {
            get
            {
                if (_detial == null)
                {
                    if (Log.Properties.TryGetValue("Detail", out var val))
                    {
                        if (Log.Properties.TryGetValue("Kind", out var vkind))
                        {
                            var kind = vkind.ToString("l", null);
                            if (kind == "Call" || kind == "Recv")
                            {
                                _detial = TraceLogs.GetRpcJson(val.ToString("l", null));
                            }
                            else if (kind == "Sqlite")
                            {
                                _detial = "Sqlite";
                            }
                        }

                        if (_detial == null)
                        {
                            // 普通项直接输出
                            _detial = val.ToString("l", null);
                        }
                    }
                    else if (Log.Exception != null)
                    {
                        // 消息 + 异常
                        using (var buffer = new StringWriter())
                        {
                            _ftAll.Format(Log, buffer);
                            _detial = buffer.ToString().Trim();
                        }
                    }
                }

                if (_detial == null)
                    _detial = "空";
                return _detial;
            }
        }
    }
}