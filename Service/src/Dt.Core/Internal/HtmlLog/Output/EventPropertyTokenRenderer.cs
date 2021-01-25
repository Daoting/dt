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
using System.IO;
#endregion

namespace Dt.Core.HtmlLog
{
    class EventPropertyTokenRenderer : OutputRenderer
    {
        readonly HtmlTheme _theme;
        readonly PropertyToken _token;

        public EventPropertyTokenRenderer(HtmlTheme theme, PropertyToken token)
        {
            _theme = theme;
            _token = token;
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            // If a property is missing, don't render anything (message templates render the raw token here).
            if (!logEvent.Properties.TryGetValue(_token.PropertyName, out var propertyValue))
            {
                Padding.Apply(output, string.Empty, _token.Alignment);
                return;
            }

            using (_theme.Apply(output, HtmlThemeStyle.SecondaryText))
            {
                var writer = _token.Alignment.HasValue ? new StringWriter() : output;

                // If the value is a scalar string, support some additional formats: 'u' for uppercase
                // and 'w' for lowercase.
                if (propertyValue is ScalarValue sv && sv.Value is string literalString)
                {
                    var cased = Casing.Format(literalString, _token.Format);
                    writer.Write(cased);
                }
                else
                {
                    propertyValue.Render(writer, _token.Format);
                }

                if (_token.Alignment.HasValue)
                {
                    var str = writer.ToString();
                    Padding.Apply(output, str, _token.Alignment);
                }
            }
        }
    }
}
