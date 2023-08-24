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
                        }
                    }
                }
                return _msg;
            }
        }

        /// <summary>
        /// 日志项是否存在详细内容
        /// </summary>
        public bool ExistDetial =>
            Log.Exception != null
            || Log.Properties.Count > 1
            || (Log.Properties.Count == 1 && !Log.Properties.ContainsKey("SourceContext"));

        /// <summary>
        /// 日志项的详细内容
        /// </summary>
        public string Detial
        {
            get
            {
                //if (_detial == null)
                //{
                //    // 输出所有属性
                //    if (Log.Properties.Count > 0)
                //    {
                //        foreach (var item in Log.Properties)
                //        {
                //            string txt;

                //            if (item.Key == "Detail"
                //                && Log.Properties.TryGetValue("SourceContext", out var vkind)
                //                && vkind.ToString("l", null) is string kind
                //                && (kind == "Rpc" || kind == "Sqlite" || kind == "Push"))
                //            {
                //                // 内置日志项的Detail是详细内容的索引
                //                txt = TraceLogs.GetDetail(item.Value.ToString("l", null));
                //            }
                //            else if (item.Key != "SourceContext")
                //            {
                //                // 普通项直接输出
                //                txt = item.Key + "：" + item.Value.ToString();
                //            }
                //            else
                //            {
                //                continue;
                //            }

                //            if (!string.IsNullOrEmpty(txt))
                //            {
                //                if (_detial == null)
                //                    _detial = txt;
                //                else
                //                    _detial += "\r\n\r\n" + txt;
                //            }
                //        }
                //    }

                //    if (Log.Exception != null)
                //    {
                //        // 消息 + 异常
                //        using (var buffer = new StringWriter())
                //        {
                //            _ftAll.Format(Log, buffer);

                //            if (_detial == null)
                //                _detial = buffer.ToString().Trim();
                //            else
                //                _detial += "\r\n\r\n" + buffer.ToString().Trim();
                //        }
                //    }
                //}

                //if (_detial == null)
                //    _detial = "空";
                //return _detial;
                return "空";
            }
        }
    }
}