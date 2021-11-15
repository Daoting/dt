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

namespace Dt.Xls
{
    /// <summary>
    /// Represents the font underline style
    /// </summary>
    public enum UnderLineStyle : byte
    {
        /// <summary>
        /// Double
        /// </summary>
        Double = 2,
        /// <summary>
        /// DoubleAccounting
        /// </summary>
        DoubleAccounting = 0x22,
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Single
        /// </summary>
        Single = 1,
        /// <summary>
        /// SingleAccounting
        /// </summary>
        SingleAccounting = 0x21
    }
}

