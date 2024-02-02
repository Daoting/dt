#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
using Serilog.Parsing;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Core.HtmlLog
{
    class TimestampTokenRenderer : OutputRenderer
    {
        readonly HtmlTheme _theme;
        readonly PropertyToken _token;

        public TimestampTokenRenderer(HtmlTheme theme, PropertyToken token)
        {
            _theme = theme;
            _token = token;
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            // We need access to ScalarValue.Render() to avoid this alloc; just ensures
            // that custom format providers are supported properly.
            var sv = new ScalarValue(logEvent.Timestamp);

            using (_theme.Apply(output, HtmlThemeStyle.SecondaryText))
            {
                if (_token.Alignment is null)
                {
                    sv.Render(output, _token.Format);
                }
                else
                {
                    var buffer = new StringWriter();
                    sv.Render(buffer, _token.Format);
                    var str = buffer.ToString();
                    Padding.Apply(output, str, _token.Alignment);
                }
            }
        }
    }
}
