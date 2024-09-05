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
using System;
using System.IO;
using System.Linq;
#endregion

namespace Dt.Core.HtmlLog
{
    class PropertiesTokenRenderer : OutputRenderer
    {
        readonly MessageTemplate _outputTemplate;
        readonly HtmlTheme _theme;
        readonly PropertyToken _token;
        readonly ThemedValueFormatter _valueFormatter;

        public PropertiesTokenRenderer(HtmlTheme theme, PropertyToken token, MessageTemplate outputTemplate)
        {
            _outputTemplate = outputTemplate;
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
            _token = token ?? throw new ArgumentNullException(nameof(token));
            var isJson = false;

            if (token.Format != null)
            {
                for (var i = 0; i < token.Format.Length; ++i)
                {
                    if (token.Format[i] == 'j')
                        isJson = true;
                }
            }

            _valueFormatter = isJson
                ? (ThemedValueFormatter)new ThemedJsonValueFormatter(theme)
                : new ThemedDisplayValueFormatter(theme);
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            var included = logEvent.Properties
                .Where(p => !TemplateContainsPropertyName(logEvent.MessageTemplate, p.Key) &&
                            !TemplateContainsPropertyName(_outputTemplate, p.Key))
                .Select(p => new LogEventProperty(p.Key, p.Value));

            var value = new StructureValue(included);
            _valueFormatter.Format(value, output, null);
        }

        static bool TemplateContainsPropertyName(MessageTemplate template, string propertyName)
        {
            foreach (var token in template.Tokens)
            {
                if (token is PropertyToken namedProperty
                    && namedProperty.PropertyName == propertyName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
