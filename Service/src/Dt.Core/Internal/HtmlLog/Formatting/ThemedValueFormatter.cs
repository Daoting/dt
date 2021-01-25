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
using Serilog.Data;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Parsing;
using System;
using System.IO;
using System.Net;
#endregion

namespace Dt.Core.HtmlLog
{
    abstract class ThemedValueFormatter : LogEventPropertyValueVisitor<ThemedValueFormatterState, int>
    {
        readonly HtmlTheme _theme;

        protected ThemedValueFormatter(HtmlTheme theme)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        }

        protected StyleReset ApplyStyle(TextWriter output, HtmlThemeStyle style)
        {
            return _theme.Apply(output, style);
        }

        public int Format(LogEventPropertyValue value, TextWriter output, string format, bool literalTopLevel = false)
        {
            return Visit(new ThemedValueFormatterState { Output = output, Format = format, IsTopLevel = literalTopLevel }, value);
        }

        public abstract ThemedValueFormatter SwitchTheme(HtmlTheme theme);
    }
}
