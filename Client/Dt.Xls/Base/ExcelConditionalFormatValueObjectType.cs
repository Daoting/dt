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
    /// This simple type expresses the type of the conditional formatting value object (cfvo). 
    /// In general the cfvo specifies one value used in the gradated scale 
    /// (max, min, midpoint, etc).
    /// </summary>
    public enum ExcelConditionalFormatValueObjectType
    {
        /// <summary>
        /// The larger of zero or the maximum value from the range of cells that the conditional formatting rule applies to.
        /// </summary>
        AutoMax = 9,
        /// <summary>
        /// The smaller of zero or the minimum value from the range of cells that the conditional formatting rule applies to.
        /// </summary>
        AutoMin = 8,
        /// <summary>
        /// The minimum/ midpoint / maximum value for the gradient is determined by a formula.
        /// </summary>
        Formula = 7,
        /// <summary>
        /// Indicates that the maximum value in the range shall be
        /// used as the maximum value for the gradient.
        /// </summary>
        Max = 3,
        /// <summary>
        /// Indicates that the minimum value in the range shall be
        /// used as the minimum value for the gradient.
        /// </summary>
        Min = 2,
        /// <summary>
        /// Indicates that the minimum / midpoint / maximum
        /// value for the gradient is specified by a constant  numeric value.
        /// </summary>
        Num = 1,
        /// <summary>
        /// Value indicates a percentage between the minimum and maximum values in the range shall be used as the
        /// minimum / midpoint / maximum value for the gradient.
        /// </summary>
        Percent = 4,
        /// <summary>
        /// Value indicates a percentile ranking in the range shall be used as the minimum / midpoint / maximum value for the gradient
        /// </summary>
        Percentile = 5,
        /// <summary>
        /// Indicates the item represents the "standard deviation" aggregate function.
        /// </summary>
        Stddev = 6
    }
}

