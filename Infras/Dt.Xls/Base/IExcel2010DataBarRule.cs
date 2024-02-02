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
    /// Represents new excel 2010 DataBar rule.
    /// </summary>
    public interface IExcel2010DataBarRule : IExcelDataBarRule, IExcelConditionalFormatRule
    {
        /// <summary>
        /// Specifies the axis color of the data bar.
        /// </summary>
        IExcelColor AxisColor { get; set; }

        /// <summary>
        /// Specifies the axis position for the data bar.
        /// </summary>
        DataBarAxisPosition AxisPosition { get; set; }

        /// <summary>
        /// Specifies the border color of the data bar.
        /// </summary>
        IExcelColor BorderColor { get; set; }

        /// <summary>
        /// Specifies the direction for the data bar.
        /// </summary>
        DataBarDirection Direction { get; set; }

        /// <summary>
        /// Specifies whether the data bar has gradient fill.
        /// </summary>
        bool IsGradientDatabar { get; set; }

        /// <summary>
        /// Specifies whether the data bar has a negative bar color that is different from the positive bar color.
        /// </summary>
        bool NegativeBarColorAsPositive { get; set; }

        /// <summary>
        /// Specifies the negative border color of the data bar.
        /// </summary>
        IExcelColor NegativeBorderColor { get; set; }

        /// <summary>
        /// Specifies whether the data bar has a negative border color that is different from the positive border color.
        /// </summary>
        bool NegativeBorderColorSameAsPositive { get; set; }

        /// <summary>
        /// Specifies the negative fill color of the data bar.
        /// </summary>
        IExcelColor NegativeFillColor { get; set; }

        /// <summary>
        /// Specifies whether the data bar has a border.
        /// </summary>
        bool ShowBorder { get; set; }
    }
}

