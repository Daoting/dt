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
    /// Describes a data bar conditional formatting rule.
    /// </summary>
    public class ExcelDataBarRule : IExcelDataBarRule, IExcelConditionalFormatRule
    {
        private byte _maxDataBarLength;
        private byte _minDatabarLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelDataBarRule" /> class.
        /// </summary>
        public ExcelDataBarRule()
        {
            this.Minimum = new ExcelConditionalFormatValueObject();
            this.Maximum = new ExcelConditionalFormatValueObject();
            this.Type = ExcelConditionalFormatType.DataBar;
            this._minDatabarLength = 10;
            this._maxDataBarLength = 90;
        }

        /// <summary>
        /// Gets or sets the color of the data bar.
        /// </summary>
        /// <value>The color of the data bar.</value>
        public IExcelColor Color { get; set; }

        /// <summary>
        /// The values of the maximum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The values of the maximum of the interpolation points in a gradient scale.
        /// </value>
        public IExcelConditionalFormatValueObject Maximum { get; set; }

        /// <summary>
        /// The maximum length of the data bar, as a percentage of the cell width.
        /// </summary>
        /// <value>The maximum length of the data bar, as a percentage of the cell width.</value>
        public byte MaximumDataBarLength
        {
            get { return  this._maxDataBarLength; }
            set
            {
                if ((value < 0) || (value > 100))
                {
                    throw new ArgumentOutOfRangeException("MaximumDataBarLength", "value must be between 0 and 100");
                }
                this._maxDataBarLength = value;
            }
        }

        /// <summary>
        /// The values of the minimum of the interpolation points in a gradient scale.
        /// </summary>
        /// <value>
        /// The values of the minimum of the interpolation points in a gradient scale.
        /// </value>
        public IExcelConditionalFormatValueObject Minimum { get; set; }

        /// <summary>
        /// The minimum length of the data bar, as a percentage of the cell width.
        /// </summary>
        /// <value>The minimum length of the data bar, as a percentage of the cell width.</value>
        public byte MinimumDataBarLength
        {
            get { return  this._minDatabarLength; }
            set
            {
                if ((value < 0) || (value > 100))
                {
                    throw new ArgumentOutOfRangeException("MinimumDataBarLength", "value must be between 0 and 100");
                }
                this._minDatabarLength = value;
            }
        }

        /// <summary>
        /// The priority of the conditional formatting rule. The value is used to determine which format
        /// should be evaluated and rendered. Lower numeric values are higher priority than higher numeric values. where '1' is the highest priority.
        /// </summary>
        /// <value>The priority of the rule</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data bar grows from right to left.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the data bar grows from right to left; otherwise, <see langword="false" />.
        /// </value>
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Indicates whether to show the values of the cells on which this data bar is applied.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show the values of the cells on which this data bar is applied; otherwise, <see langword="false" />.
        /// </value>
        public bool ShowValue { get; set; }

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
        public ExcelConditionalFormatType Type { get; private set; }
    }
}

