#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
#endregion

namespace Dt.Core.HtmlLog
{
    class OutputFormatter : ITextFormatter
    {
        readonly OutputRenderer[] _renderers;

        public OutputFormatter(string outputTemplate)
        {
            if (outputTemplate is null) throw new ArgumentNullException(nameof(outputTemplate));
            var template = new MessageTemplateParser().Parse(outputTemplate);

            var theme = new HtmlTheme();
            var renderers = new List<OutputRenderer>();
            foreach (var token in template.Tokens)
            {
                if (token is TextToken tt)
                {
                    renderers.Add(new TextTokenRenderer(theme, tt.Text));
                    continue;
                }

                var pt = (PropertyToken)token;
                if (pt.PropertyName == OutputProperties.LevelPropertyName)
                {
                    renderers.Add(new LevelTokenRenderer(theme, pt));
                }
                else if (pt.PropertyName == OutputProperties.NewLinePropertyName)
                {
                    renderers.Add(new NewLineTokenRenderer(pt.Alignment));
                }
                else if (pt.PropertyName == OutputProperties.ExceptionPropertyName)
                {
                    renderers.Add(new ExceptionTokenRenderer(theme, pt));
                }
                else if (pt.PropertyName == OutputProperties.MessagePropertyName)
                {
                    renderers.Add(new MessageTemplateOutputTokenRenderer(theme, pt));
                }
                else if (pt.PropertyName == OutputProperties.TimestampPropertyName)
                {
                    renderers.Add(new TimestampTokenRenderer(theme, pt));
                }
                else if (pt.PropertyName == "Properties")
                {
                    renderers.Add(new PropertiesTokenRenderer(theme, pt, template));
                }
                else
                {
                    renderers.Add(new EventPropertyTokenRenderer(theme, pt));
                }
            }

            _renderers = renderers.ToArray();
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent is null) throw new ArgumentNullException(nameof(logEvent));
            if (output is null) throw new ArgumentNullException(nameof(output));

            foreach (var renderer in _renderers)
                renderer.Render(logEvent, output);
        }
    }
}
