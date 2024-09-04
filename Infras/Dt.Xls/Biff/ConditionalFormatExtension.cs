#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls.Biff
{
    internal class ConditionalFormatExtension
    {
        internal byte ComparisionOperator { get; set; }

        internal IDifferentialFormatting DXF { get; set; }

        internal bool HasDXF { get; set; }

        internal short Identifier { get; set; }

        internal ushort Priority { get; set; }

        internal short RuleId { get; set; }

        internal bool stopIfTrue { get; set; }

        internal byte TemplateIndex { get; set; }
    }
}

