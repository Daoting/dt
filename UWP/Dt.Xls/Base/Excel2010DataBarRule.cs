#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents new excel 2010 DataBar rule.
    /// </summary>
    public class Excel2010DataBarRule : ExcelDataBarRule, IExcel2010DataBarRule, IExcelDataBarRule, IExcelConditionalFormatRule
    {
        /// <summary>
        /// Specifies the axis color of the data bar.
        /// </summary>
        public IExcelColor AxisColor { get; set; }

        /// <summary>
        /// Specifies the axis position for the data bar.
        /// </summary>
        public DataBarAxisPosition AxisPosition { get; set; }

        /// <summary>
        /// Specifies the border color of the data bar.
        /// </summary>
        public IExcelColor BorderColor { get; set; }

        /// <summary>
        /// Specifies the direction for the data bar.
        /// </summary>
        public DataBarDirection Direction { get; set; }

        /// <summary>
        /// Specifies whether the data bar has gradient fill.
        /// </summary>
        public bool IsGradientDatabar { get; set; }

        /// <summary>
        /// Specifies whether the data bar has a negative bar color that is different from the positive bar color.
        /// </summary>
        public bool NegativeBarColorAsPositive { get; set; }

        /// <summary>
        /// Specifies the negative border color of the data bar.
        /// </summary>
        public IExcelColor NegativeBorderColor { get; set; }

        /// <summary>
        /// Specifies whether the data bar has a negative border color that is different from the positive border color.
        /// </summary>
        public bool NegativeBorderColorSameAsPositive { get; set; }

        /// <summary>
        /// Specifies the negative fill color of the data bar.
        /// </summary>
        public IExcelColor NegativeFillColor { get; set; }

        /// <summary>
        /// Specifies whether the data bar has a border.
        /// </summary>
        public bool ShowBorder { get; set; }
    }
}

