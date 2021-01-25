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
    class LevelTokenRenderer : OutputRenderer
    {
        readonly HtmlTheme _theme;
        readonly PropertyToken _levelToken;

        static readonly Dictionary<LogEventLevel, HtmlThemeStyle> Levels = new Dictionary<LogEventLevel, HtmlThemeStyle>
        {
            { LogEventLevel.Verbose, HtmlThemeStyle.LevelVerbose },
            { LogEventLevel.Debug, HtmlThemeStyle.LevelDebug },
            { LogEventLevel.Information, HtmlThemeStyle.LevelInformation },
            { LogEventLevel.Warning, HtmlThemeStyle.LevelWarning },
            { LogEventLevel.Error, HtmlThemeStyle.LevelError },
            { LogEventLevel.Fatal, HtmlThemeStyle.LevelFatal },
        };

        public LevelTokenRenderer(HtmlTheme theme, PropertyToken levelToken)
        {
            _theme = theme;
            _levelToken = levelToken;
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            var moniker = LevelOutputFormat.GetLevelMoniker(logEvent.Level, _levelToken.Format);
            if (!Levels.TryGetValue(logEvent.Level, out var levelStyle))
                levelStyle = HtmlThemeStyle.Invalid;

            using (_theme.Apply(output, levelStyle))
                Padding.Apply(output, moniker, _levelToken.Alignment);
        }
    }
}
