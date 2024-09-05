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
    class ExceptionTokenRenderer : OutputRenderer
    {
        const string StackFrameLinePrefix = "   ";

        readonly HtmlTheme _theme;

        public ExceptionTokenRenderer(HtmlTheme theme, PropertyToken pt)
        {
            _theme = theme;
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            // Padding is never applied by this renderer.

            if (logEvent.Exception is null)
                return;

            var lines = new StringReader(logEvent.Exception.ToString());
            string nextLine;
            while ((nextLine = lines.ReadLine()) != null)
            {
                var style = nextLine.StartsWith(StackFrameLinePrefix) ? HtmlThemeStyle.SecondaryText : HtmlThemeStyle.Text;
                using (_theme.Apply(output, style))
                    output.WriteLine(nextLine);
            }
        }
    }
}
