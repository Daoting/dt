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
    /// Defines properties related to alignment settings.
    /// </summary>
    public interface IAlignmentBlock
    {
        /// <summary>
        /// Clone and create a new <see cref="T:Dt.Xls.IAlignmentBlock" /> instance.
        /// </summary>
        /// <returns></returns>
        IAlignmentBlock Clone();

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelHorizontalAlignment" /> property value
        /// </summary>
        /// <value>The horizontal alignment setting.</value>
        ExcelHorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the indent level of the alignment.
        /// </summary>
        /// <value>The indent level of the alignment</value>
        byte IndentationLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the justify distributed option is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        bool IsJustifyLastLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Shrink to fit of  the text control is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        bool IsShrinkToFit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Wrap text of the text control is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        bool IsTextWrapped { get; set; }

        /// <summary>
        /// Gets or sets the text direction of the alignment.
        /// </summary>
        /// <value>The test direction of the alignment.</value>
        Dt.Xls.TextDirection TextDirection { get; set; }

        /// <summary>
        /// Gets or sets the text orientation.
        /// </summary>
        /// <value>The orientation of the text.</value>
        byte TextRotation { get; set; }

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelVerticalAlignment" /> property value
        /// </summary>
        /// <value>The vertical alignment setting.</value>
        ExcelVerticalAlignment VerticalAlignment { get; set; }
    }
}

