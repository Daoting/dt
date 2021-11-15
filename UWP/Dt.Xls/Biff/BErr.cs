#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls.Biff
{
    internal enum BErr : byte
    {
        DIV0 = 7,
        NA = 0x2a,
        NAME = 0x1d,
        NULL = 0,
        NUM = 0x24,
        REF = 0x17,
        VALUE = 15
    }
}

