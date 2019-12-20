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
    /// Defines properties related to alignment settings.
    /// </summary>
    public class AlignmentBlock : IAlignmentBlock
    {
        /// <summary>
        /// Clone and create a new <see cref="T:Dt.Xls.IAlignmentBlock" /> instance.
        /// </summary>
        /// <returns></returns>
        public IAlignmentBlock Clone()
        {
            return new AlignmentBlock { HorizontalAlignment = this.HorizontalAlignment, VerticalAlignment = this.VerticalAlignment, IsTextWrapped = this.IsTextWrapped, TextRotation = this.TextRotation, TextDirection = this.TextDirection, IsJustifyLastLine = this.IsJustifyLastLine, IsShrinkToFit = this.IsShrinkToFit, IndentationLevel = this.IndentationLevel };
        }

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelHorizontalAlignment" /> property value
        /// </summary>
        /// <value>The horizontal alignment setting.</value>
        public ExcelHorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the indent level of the alignment.
        /// </summary>
        /// <value>The indent level of the alignment</value>
        public byte IndentationLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the justify distributed option is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        public bool IsJustifyLastLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Shrink to fit of  the text control is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        public bool IsShrinkToFit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Wrap text of the text control is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        public bool IsTextWrapped { get; set; }

        /// <summary>
        /// Gets or sets the text direction of the alignment.
        /// </summary>
        /// <value>The test direction of the alignment.</value>
        public Dt.Xls.TextDirection TextDirection { get; set; }

        /// <summary>
        /// Gets or sets the text orientation.
        /// </summary>
        /// <value>The orientation of the text.</value>
        public byte TextRotation { get; set; }

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelVerticalAlignment" /> property value
        /// </summary>
        /// <value>The vertical alignment setting.</value>
        public ExcelVerticalAlignment VerticalAlignment { get; set; }
    }
}

