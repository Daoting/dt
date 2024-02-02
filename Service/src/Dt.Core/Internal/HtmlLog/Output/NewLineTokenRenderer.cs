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
    class NewLineTokenRenderer : OutputRenderer
    {
        readonly Alignment? _alignment;

        public NewLineTokenRenderer(Alignment? alignment)
        {
            _alignment = alignment;
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            if (_alignment.HasValue)
                Padding.Apply(output, Environment.NewLine, _alignment.Value.Widen(Environment.NewLine.Length));
            else
                output.WriteLine();
        }
    }
}
