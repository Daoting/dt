#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    [Flags]
    internal enum BindingFlags
    {
        CreateInstance = 0x200,
        DeclaredOnly = 2,
        Default = 0,
        ExactBinding = 0x10000,
        FlattenHierarchy = 0x40,
        GetField = 0x400,
        GetProperty = 0x1000,
        IgnoreCase = 1,
        IgnoreReturn = 0x1000000,
        Instance = 4,
        InvokeMethod = 0x100,
        NonPublic = 0x20,
        OptionalParamBinding = 0x40000,
        Public = 0x10,
        PutDispProperty = 0x4000,
        PutRefDispProperty = 0x8000,
        SetField = 0x800,
        SetProperty = 0x2000,
        Static = 8,
        SuppressChangeType = 0x20000
    }
}

