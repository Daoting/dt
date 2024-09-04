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
    /// Represents an icon set conditional formatting rule.
    /// </summary>
    public class ExcelColorScaleRule : IExcelColorScaleRule, IExcelConditionalFormatRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelColorScaleRule" /> class.
        /// </summary>
        public ExcelColorScaleRule()
        {
            this.Minimum = new ExcelConditionalFormatValueObject();
            this.Middle = new ExcelConditionalFormatValueObject();
            this.Maximum = new ExcelConditionalFormatValueObject();
            this.Type = ExcelConditionalFormatType.ColorScale;
        }

        /// <summary>
        /// Flag indicate whether it's a three points gradient scale or two point gradient scale.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this it's a three points gradient scale; otherwise, <see langword="false" />.
        /// </value>
        public bool HasMiddleNode { get; set; }

        /// <summary>
        /// The values of the maximum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The values of the maximum of the interpolation points in a gradient scale.
        /// </value>
        public IExcelConditionalFormatValueObject Maximum { get; set; }

        /// <summary>
        /// The color of the maximum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The color of the maximum of the interpolation points in a gradient scale.
        /// </value>
        public IExcelColor MaximumColor { get; set; }

        /// <summary>
        /// The values of the middle of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The values of the middle of the interpolation points in a gradient scale.
        /// </value>
        public IExcelConditionalFormatValueObject Middle { get; set; }

        /// <summary>
        /// The color of the middle of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The color of the middle of the interpolation points in a gradient scale.
        /// </value>
        public IExcelColor MiddleColor { get; set; }

        /// <summary>
        /// The values of the minimum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The values of the minimum of the interpolation points in a gradient scale.
        /// </value>
        public IExcelConditionalFormatValueObject Minimum { get; set; }

        /// <summary>
        /// The color of the minimum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The color of the minimum of the interpolation points in a gradient scale.
        /// </value>
        public IExcelColor MinimumColor { get; set; }

        /// <summary>
        /// The priority of the conditional formatting rule. The value is used to determine which format
        /// should be evaluated and rendered. Lower numeric values are higher priority than higher numeric values. where '1' is the highest priority.
        /// </summary>
        /// <value>The priority of the rule</value>
        public int Priority { get; set; }

        /// <summary>
        /// Flag indicates whether apply other lower priority rules when this rule evaluates to true.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if don't apply lower priority rules when this rule evaluates to true; otherwise, <see langword="false" />.
        /// </value>
        public bool StopIfTrue { get; set; }

        /// <summary>
        /// Type of conditional formatting rule.
        /// </summary>
        /// <value>Type of conditional formatting rule.</value>
        public ExcelConditionalFormatType Type { get; set; }
    }
}

