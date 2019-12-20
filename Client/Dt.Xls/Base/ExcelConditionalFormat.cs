#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent a conditional formatting conditions
    /// </summary>
    public class ExcelConditionalFormat : IExcelConditionalFormat
    {
        private List<IRange> _ranges;
        private List<IExcelConditionalFormatRule> _rules;

        /// <summary>
        /// Gets the conditional formatting rules.
        /// </summary>
        /// <value>The conditional formatting rules.</value>
        public List<IExcelConditionalFormatRule> ConditionalFormattingRules
        {
            get
            {
                if (this._rules == null)
                {
                    this._rules = new List<IExcelConditionalFormatRule>();
                }
                return this._rules;
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public short Identifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it's a office 2007 new added conditional format type.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is office 2007 new added conditional format; otherwise, <see langword="false" />.
        /// </value>
        public bool IsOffice2007ConditionalFormat { get; set; }

        /// <summary>
        /// Gets or sets range over which these conditional formatting rules apply.
        /// </summary>
        /// <value>The conditional format scope.</value>
        public List<IRange> Ranges
        {
            get
            {
                if (this._ranges == null)
                {
                    this._ranges = new List<IRange>();
                }
                return this._ranges;
            }
            set { this._ranges = value; }
        }
    }
}

