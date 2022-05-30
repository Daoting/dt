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

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the formatting for the text.
    /// </summary>
    public interface IExcelTextFormat
    {
        /// <summary>
        /// Specifies the bottom inset of the bounding rectangle.
        /// </summary>
        double BottomInset { get; set; }

        /// <summary>
        /// Specifies the centering of the text box.
        /// </summary>
        bool IsTextBoxAnchorCenter { get; set; }

        /// <summary>
        /// Specifies the left inset of the bounding rectangle.
        /// </summary>
        double LeftInset { get; set; }

        /// <summary>
        /// Specifies the Right inset of the bounding rectangle.
        /// </summary>
        double RightInset { get; set; }

        /// <summary>
        /// Specifies the rotation that is being applied to the text within the bounding box.
        /// </summary>
        double Rotation { get; set; }

        /// <summary>
        /// Gets or sets the type of the text anchoring.
        /// </summary>
        /// <value>
        /// The type of the text anchoring.
        /// </value>
        TextAnchoringTypes TextAnchoringType { get; set; }

        /// <summary>
        /// Determines whether the text can flow out of the bounding box horizontally.
        /// </summary>
        TextOverflowTypes TextHorizontalOverflow { get; set; }

        /// <summary>
        /// Specifies the text paragraph settings.
        /// </summary>
        List<TextParagraph> TextParagraphs { get; }

        /// <summary>
        /// Determines whether the text can flow out of the bounding box vertically.
        /// </summary>
        TextOverflowTypes TextVerticalOverflow { get; set; }

        /// <summary>
        /// Specifies the wrapping options to be used for this text body.
        /// </summary>
        TextWrappingTypes TextWrappingType { get; set; }

        /// <summary>
        /// Specifies the top inset of the bounding rectangle.
        /// </summary>
        double TopInset { get; set; }

        /// <summary>
        /// Specifies whether text should remain upright, regardless of the transform applied to it.
        /// </summary>
        bool UpRight { get; set; }

        /// <summary>
        /// Determines if the text within the given text body should be displayed vertically.
        /// </summary>
        TextVerticalTypes VerticalText { get; set; }
    }
}

