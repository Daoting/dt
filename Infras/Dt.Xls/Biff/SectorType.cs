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
    /// Represents the sector type of the compound file.
    /// </summary>
    internal enum SectorType : byte
    {
        Data = 0,
        DIFAT = 2,
        FAT = 1
    }
}

