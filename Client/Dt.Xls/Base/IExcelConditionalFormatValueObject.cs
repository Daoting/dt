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
    /// Describes the values of the interpolation points in a gradient scale.
    /// </summary>
    public interface IExcelConditionalFormatValueObject
    {
        /// <summary>
        /// Flag indicate whether the Value is formula or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this value is formula; otherwise, <see langword="false" />.
        /// </value>
        bool IsFormula { get; set; }

        /// <summary>
        /// The type of this conditional formatting value object
        /// </summary>
        /// <value>The type of this conditional formatting value object</value>
        ExcelConditionalFormatValueObjectType Type { get; set; }

        /// <summary>
        /// The value of this conditional formatting value object.
        /// </summary>
        /// <value>The value of this conditional formatting value object.</value>
        string Value { get; set; }
    }
}

