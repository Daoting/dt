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
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Defines properties and conditions used to represents data validation used in Excel.
    /// </summary>
    public interface IExcelDataValidation
    {
        /// <summary>
        /// Gets or sets a value indicating whether ignore blank during the data validation.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if ignore blank; otherwise, <see langword="false" />.
        /// </value>
        bool AllowBlank { get; set; }

        /// <summary>
        /// Gets or sets the data validation compare operator.
        /// </summary>
        /// <value>The compare operator.</value>
        ExcelDataValidationOperator CompareOperator { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message</value>
        string Error { get; set; }

        /// <summary>
        /// Gets or sets the error alert style of the data validation.
        /// </summary>
        /// <value>The error alert style.</value>
        ExcelDataValidationErrorStyle ErrorStyle { get; set; }

        /// <summary>
        /// Gets or sets the error title.
        /// </summary>
        /// <value>The error title.</value>
        string ErrorTitle { get; set; }

        /// <summary>
        /// Gets or sets the first formula in the data validation dropdown. It is used as bounds for 'between' and 
        /// 'notBetween' relation operators, and the only formula used for other relational operators (equal, notEqual, lessThan,
        /// lessThanOrEqual, greaterThan, greaterThanOrEqual), or for custom or list type data validation. The content can be 
        /// a formula or a constant or a list of series (comma separated values).
        /// </summary>
        /// <value>The first formula.</value>
        string FirstFormula { get; set; }

        /// <summary>
        /// Gets or sets the prompt window message.
        /// </summary>
        /// <value>The prompt window message.</value>
        string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the title bar text of input prompt.
        /// </summary>
        /// <value>The title bar text of input prompt.</value>
        string PromptTitle { get; set; }

        /// <summary>
        /// Gets or sets the ranges which data validation is applied.
        /// </summary>
        /// <value>The data validation scope.</value>
        List<IRange> Ranges { get; set; }

        /// <summary>
        /// Gets or sets the second formula in the data validation dropdown. It is used as a bounds for 'between' and 'notBetween' relational
        /// operators only.
        /// </summary>
        /// <value>The second formula.</value>
        string SecondFormula { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show error alert box
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show error alert box; otherwise, <see langword="false" />.
        /// </value>
        bool ShowErrorBox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show input message when cell is selected.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show input message; otherwise, <see langword="false" />.
        /// </value>
        bool ShowInputMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show the prompt window.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show prompt window; otherwise, <see langword="false" />.
        /// </value>
        bool ShowPromptBox { get; set; }

        /// <summary>
        /// Gets or sets data validation type.
        /// </summary>
        /// <value>The data validation type.</value>
        ExcelDataValidationType Type { get; set; }
    }
}

