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
using Serilog.Parsing;
using System;
using System.IO;
using System.Net;
#endregion

namespace Dt.Core.HtmlLog
{
    class TextTokenRenderer : OutputRenderer
    {
        readonly HtmlTheme _theme;
        readonly string _text;

        public TextTokenRenderer(HtmlTheme theme, string text)
        {
            _theme = theme;
            _text = text;
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            using (_theme.Apply(output, HtmlThemeStyle.TertiaryText))
                output.Write(_text);
        }
    }
}
