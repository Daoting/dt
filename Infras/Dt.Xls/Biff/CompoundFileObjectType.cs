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
    /// <summary>
    /// Represents the entry object type of the compound file.
    /// </summary>
    internal enum CompoundFileObjectType : byte
    {
        RootStorage = 5,
        Storage = 1,
        Stream = 2,
        Unknown = 0
    }
}

