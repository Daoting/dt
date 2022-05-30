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

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the display unit is one of the built in values
    /// </summary>
    public enum BuiltInDisplayUnitValue : long
    {
        /// <summary>
        /// Specifies the value on the chart shall be divided by 1000000000
        /// </summary>
        Billions = 0x3b9aca00L,
        /// <summary>
        /// Specifies the value on the chart shall be divided by 100000000
        /// </summary>
        HundredMillions = 0x5f5e100L,
        /// <summary>
        /// Specifies the value on the chart shall be divided by 100
        /// </summary>
        Hundreds = 100L,
        /// <summary>
        /// Specifies the value on the chart shall be divided by 100000
        /// </summary>
        HundredThousands = 0x186a0L,
        /// <summary>
        /// Specifies the value on the chart shall be divided by 1000000
        /// </summary>
        Millions = 0xf4240L,
        /// <summary>
        /// Specifies the value on the chart shall be displayed directly.
        /// </summary>
        None = 0L,
        /// <summary>
        /// Specifies the value on the chart shall be divided by 10000000
        /// </summary>
        TenMillions = 0x989680L,
        /// <summary>
        /// Specifies the value on the chart shall be divided by 10000
        /// </summary>
        TenThousands = 0x2710L,
        /// <summary>
        /// Specifies the value on the chart shall be divided by  1000
        /// </summary>
        Thousands = 0x3e8L,
        /// <summary>
        /// Specifies the value on the chart shall be divided by 1000000000000
        /// </summary>
        Trillions = 0xe8d4a51000L
    }
}

