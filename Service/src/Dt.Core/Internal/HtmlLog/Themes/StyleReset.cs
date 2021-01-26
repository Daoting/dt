#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
#endregion

namespace Dt.Core.HtmlLog
{
    struct StyleReset : IDisposable
    {
        readonly HtmlTheme _theme;
        readonly TextWriter _output;
        readonly HtmlThemeStyle _style;

        public StyleReset(HtmlTheme theme, TextWriter output, HtmlThemeStyle style)
        {
            _theme = theme;
            _output = output;
            _style = style;
        }

        public void Dispose()
        {
            _theme.Reset(_output, _style);
        }
    }
}
