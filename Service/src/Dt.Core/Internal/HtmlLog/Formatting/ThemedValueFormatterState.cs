#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using System.IO;
#endregion

namespace Dt.Core.HtmlLog
{
    struct ThemedValueFormatterState
    {
        public TextWriter Output;
        public string Format;
        public bool IsTopLevel;

        public ThemedValueFormatterState Nest() => new ThemedValueFormatterState { Output = Output };
    }
}
