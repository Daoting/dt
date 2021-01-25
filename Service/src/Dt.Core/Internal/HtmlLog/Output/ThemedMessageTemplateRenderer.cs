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
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Core.HtmlLog
{
    class ThemedMessageTemplateRenderer
    {
        readonly HtmlTheme _theme;
        readonly ThemedValueFormatter _valueFormatter;
        readonly bool _isLiteral;
        static readonly HtmlTheme NoTheme = new EmptyHtmlTheme();
        readonly ThemedValueFormatter _unthemedValueFormatter;

        public ThemedMessageTemplateRenderer(HtmlTheme theme, ThemedValueFormatter valueFormatter, bool isLiteral)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
            _valueFormatter = valueFormatter;
            _isLiteral = isLiteral;
            _unthemedValueFormatter = valueFormatter.SwitchTheme(NoTheme);
        }

        public int Render(MessageTemplate template, IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output)
        {
            var count = 0;
            foreach (var token in template.Tokens)
            {
                if (token is TextToken tt)
                {
                    count += RenderTextToken(tt, output);
                }
                else
                {
                    var pt = (PropertyToken)token;
                    count += RenderPropertyToken(pt, properties, output);
                }
            }
            return count;
        }

        int RenderTextToken(TextToken tt, TextWriter output)
        {
            using (_theme.Apply(output, HtmlThemeStyle.Text))
                output.Write(tt.Text);
            return 0;
        }

        int RenderPropertyToken(PropertyToken pt, IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output)
        {
            if (!properties.TryGetValue(pt.PropertyName, out var propertyValue))
            {
                using (_theme.Apply(output, HtmlThemeStyle.Invalid))
                    output.Write(pt.ToString());
                return 0;
            }

            if (!pt.Alignment.HasValue)
            {
                return RenderValue(_theme, _valueFormatter, propertyValue, output, pt.Format);
            }

            return RenderAlignedPropertyTokenUnbuffered(pt, output, propertyValue);
        }

        int RenderAlignedPropertyTokenUnbuffered(PropertyToken pt, TextWriter output, LogEventPropertyValue propertyValue)
        {
            var valueOutput = new StringWriter();
            RenderValue(NoTheme, _unthemedValueFormatter, propertyValue, valueOutput, pt.Format);

            var valueLength = valueOutput.ToString().Length;
            // ReSharper disable once PossibleInvalidOperationException
            if (valueLength >= pt.Alignment.Value.Width)
            {
                return RenderValue(_theme, _valueFormatter, propertyValue, output, pt.Format);
            }

            if (pt.Alignment.Value.Direction == AlignmentDirection.Left)
            {
                var invisible = RenderValue(_theme, _valueFormatter, propertyValue, output, pt.Format);
                Padding.Apply(output, string.Empty, pt.Alignment.Value.Widen(-valueLength));
                return invisible;
            }

            Padding.Apply(output, string.Empty, pt.Alignment.Value.Widen(-valueLength));
            return RenderValue(_theme, _valueFormatter, propertyValue, output, pt.Format);
        }

        int RenderValue(HtmlTheme theme, ThemedValueFormatter valueFormatter, LogEventPropertyValue propertyValue, TextWriter output, string format)
        {
            if (_isLiteral && propertyValue is ScalarValue sv && sv.Value is string)
            {
                using (theme.Apply(output, HtmlThemeStyle.String))
                    output.Write(sv.Value);
                return 0;
            }

            return valueFormatter.Format(propertyValue, output, format, _isLiteral);
        }
    }
}
