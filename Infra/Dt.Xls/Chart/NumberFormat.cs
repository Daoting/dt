#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls.Chart
{
    internal class NumberFormat
    {
        public NumberFormat()
        {
            this.NumberFormatCode = "General";
            this.LinkToSource = true;
        }

        internal bool LinkToSource { get; set; }

        internal string NumberFormatCode { get; set; }
    }
}

