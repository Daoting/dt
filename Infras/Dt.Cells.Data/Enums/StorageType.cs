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
    /// <summary>
    /// Specifies the type of storage data.
    /// </summary>
    [Flags]
    public enum StorageType
    {
        /// <summary>
        /// Indicates the storage data type is the axis information.
        /// </summary>
        Axis = 0x20,
        /// <summary>
        /// Indicates the storage data type is pure value. 
        /// </summary>
        Data = 1,
        /// <summary>
        /// Indicates the storage data type is sparkline.
        /// </summary>
        Sparkline = 0x10,
        /// <summary>
        /// Indicates the storage data type is style.
        /// </summary>
        Style = 2,
        /// <summary>
        /// Indicates the storage data type is tag.
        /// </summary>
        Tag = 8
    }
}

