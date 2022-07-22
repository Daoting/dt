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
    /// A class implements <see cref="T:Dt.Xls.IExcelDataValidation" /> used to represents the data validation settings used in Excel. 
    /// </summary>
    public class ExcelDataValidation : IExcelDataValidation
    {
        private List<IRange> _ranges;

        /// <summary>
        /// Gets or sets a value indicating whether ignore blank during the data validation.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ignore blank; otherwise, <see langword="false" />.
        /// </value>
        public bool AllowBlank { get; set; }

        /// <summary>
        /// Gets or sets the data validation compare operator.
        /// </summary>
        /// <value>The compare operator.</value>
        public ExcelDataValidationOperator CompareOperator { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message</value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error alert style of the data validation.
        /// </summary>
        /// <value>The error alert style.</value>
        public ExcelDataValidationErrorStyle ErrorStyle { get; set; }

        /// <summary>
        /// Gets or sets the error title.
        /// </summary>
        /// <value>The error title.</value>
        public string ErrorTitle { get; set; }

        /// <summary>
        /// Gets or sets the first formula in the data validation dropdown. It is used as bounds for 'between' and
        /// 'notBetween' relation operators, and the only formula used for other relational operators (equal, notEqual, lessThan,
        /// lessThanOrEqual, greaterThan, greaterThanOrEqual), or for custom or list type data validation. The content can be
        /// a formula or a constant or a list of series (comma separated values).
        /// </summary>
        /// <value>The first formula.</value>
        public string FirstFormula { get; set; }

        /// <summary>
        /// Gets or sets the prompt window message.
        /// </summary>
        /// <value>The prompt window message.</value>
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the title bar text of input prompt.
        /// </summary>
        /// <value>The title bar text of input prompt.</value>
        public string PromptTitle { get; set; }

        /// <summary>
        /// Gets or sets the ranges which data validation is applied.
        /// </summary>
        /// <value>The data validation scope.</value>
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

        /// <summary>
        /// Gets or sets the second formula in the data validation dropdown. It is used as a bounds for 'between' and 'notBetween' relational
        /// operators only.
        /// </summary>
        /// <value>The second formula.</value>
        public string SecondFormula { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show error alert box
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show error alert box; otherwise, <see langword="false" />.
        /// </value>
        public bool ShowErrorBox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show input message when cell is selected.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show input message; otherwise, <see langword="false" />.
        /// </value>
        public bool ShowInputMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show the prompt window.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show prompt window; otherwise, <see langword="false" />.
        /// </value>
        public bool ShowPromptBox { get; set; }

        /// <summary>
        /// Gets or sets the type of data validation..
        /// </summary>
        /// <value>The type of data validation.</value>
        public ExcelDataValidationType Type { get; set; }
    }
}

