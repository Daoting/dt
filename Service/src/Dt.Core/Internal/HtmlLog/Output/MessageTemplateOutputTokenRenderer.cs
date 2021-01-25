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
#endregion

namespace Dt.Core.HtmlLog
{
    class MessageTemplateOutputTokenRenderer : OutputRenderer
    {
        readonly HtmlTheme _theme;
        readonly PropertyToken _token;
        readonly ThemedMessageTemplateRenderer _renderer;

        public MessageTemplateOutputTokenRenderer(HtmlTheme theme, PropertyToken token)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
            _token = token ?? throw new ArgumentNullException(nameof(token));
            bool isLiteral = false, isJson = false;

            if (token.Format != null)
            {
                for (var i = 0; i < token.Format.Length; ++i)
                {
                    if (token.Format[i] == 'l')
                        isLiteral = true;
                    else if (token.Format[i] == 'j')
                        isJson = true;
                }
            }

            var valueFormatter = isJson
                ? (ThemedValueFormatter)new ThemedJsonValueFormatter(theme)
                : new ThemedDisplayValueFormatter(theme);

            _renderer = new ThemedMessageTemplateRenderer(theme, valueFormatter, isLiteral);
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            _renderer.Render(logEvent.MessageTemplate, logEvent.Properties, output);
        }
    }
}
