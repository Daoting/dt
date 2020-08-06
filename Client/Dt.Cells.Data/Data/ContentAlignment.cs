#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
#endregion

namespace Dt.Cells.Data
{
    internal sealed class ContentAlignment : ICloneable
    {
        TextHorizontalAlignment? horizontalAlignment;
        bool? rightToLeft;
        int? textIndent;
        Dt.Cells.Data.TextOrientation? textOrientation;
        double? textRotationAngle;
        TextVerticalAlignment? verticalAlignment;
        bool? wordWrap;

        /// <summary>
        /// Creates a new content alignment object.
        /// </summary>
        public ContentAlignment()
        {
            this.horizontalAlignment = null;
            this.verticalAlignment = null;
            this.textIndent = null;
            this.textOrientation = null;
            this.textRotationAngle = null;
            this.rightToLeft = null;
            this.wordWrap = null;
        }

        /// <summary>
        /// Creates a new content alignment object with the specified horizontal and vertical alignment type.
        /// </summary>
        /// <param name="horizontalAlignment">The horizontal alignment type.</param>
        /// <param name="verticalAlignment">The vertical alignment type.</param>
        public ContentAlignment(TextHorizontalAlignment horizontalAlignment, TextVerticalAlignment verticalAlignment)
        {
            this.horizontalAlignment = null;
            this.verticalAlignment = null;
            this.textIndent = null;
            this.textOrientation = null;
            this.textRotationAngle = null;
            this.rightToLeft = null;
            this.wordWrap = null;
            this.horizontalAlignment = new TextHorizontalAlignment?(horizontalAlignment);
            this.verticalAlignment = new TextVerticalAlignment?(verticalAlignment);
        }

        /// <summary>
        /// Creates a new content alignment object with the specified alignment types, text indent, 
        /// text orientation, rotation angle, word wrap, and right to left setting.
        /// </summary>
        /// <param name="horizontalAlignment">The horizontal alignment type.</param>
        /// <param name="verticalAlignment">The vertical alignment type.</param>
        /// <param name="textIndent">The amount in pixels to indent the text.</param>
        /// <param name="textOrientation">The text orientation type.</param>
        /// <param name="textRotationAngle">The text rotation angle.</param>
        /// <param name="wordWrap">Whether words wrap in the cell.</param>
        /// <param name="rightToLeft">Whether text displays right to left.</param>
        public ContentAlignment(TextHorizontalAlignment horizontalAlignment, TextVerticalAlignment verticalAlignment, int textIndent, Dt.Cells.Data.TextOrientation textOrientation, double textRotationAngle, bool wordWrap, bool rightToLeft)
        {
            this.horizontalAlignment = null;
            this.verticalAlignment = null;
            this.textIndent = null;
            this.textOrientation = null;
            this.textRotationAngle = null;
            this.rightToLeft = null;
            this.wordWrap = null;
            this.horizontalAlignment = new TextHorizontalAlignment?(horizontalAlignment);
            this.verticalAlignment = new TextVerticalAlignment?(verticalAlignment);
            this.textIndent = new int?(textIndent);
            this.textOrientation = new Dt.Cells.Data.TextOrientation?(textOrientation);
            this.textRotationAngle = new double?(textRotationAngle);
            this.rightToLeft = new bool?(rightToLeft);
            this.wordWrap = new bool?(wordWrap);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new ContentAlignment { horizontalAlignment = this.horizontalAlignment, textIndent = this.textIndent, textOrientation = this.textOrientation, textRotationAngle = this.textRotationAngle, verticalAlignment = this.verticalAlignment, rightToLeft = this.rightToLeft, wordWrap = this.wordWrap };
        }

        public static ContentAlignment Create(StyleInfo style)
        {
            if ((style == null) || ((!style.IsHorizontalAlignmentSet() && !style.IsVerticalAlignmentSet()) && (!style.IsTextIndentSet() && !style.IsWordWrapSet())))
            {
                return null;
            }
            return new ContentAlignment { horizontalAlignment = new TextHorizontalAlignment?(HA2THA(style.HorizontalAlignment)), verticalAlignment = new TextVerticalAlignment?(VA2TVA(style.VerticalAlignment)), textIndent = new int?(style.TextIndent), wordWrap = new bool?(style.WordWrap) };
        }

        /// <summary>
        /// Determines whether the specified ContentAlignment object is equal to the current ContentAlignment object.
        /// </summary>
        /// <param name="obj">The object to compare to the current object.</param>
        /// <returns>
        /// <c>true</c> if the objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return ((((obj is ContentAlignment) && (this.HorizontalAlignment == ((ContentAlignment) obj).HorizontalAlignment)) && ((this.TextIndent == ((ContentAlignment) obj).TextIndent) && (this.TextOrientation == ((ContentAlignment) obj).TextOrientation))) && (((this.TextRotationAngle == ((ContentAlignment) obj).TextRotationAngle) && (this.VerticalAlignment == ((ContentAlignment) obj).VerticalAlignment)) && ((this.RightToLeft == ((ContentAlignment) obj).RightToLeft) && (this.WordWrap == ((ContentAlignment) obj).WordWrap))));
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static TextHorizontalAlignment HA2THA(CellHorizontalAlignment s)
        {
            switch (s)
            {
                case CellHorizontalAlignment.Left:
                    return TextHorizontalAlignment.Left;

                case CellHorizontalAlignment.Center:
                    return TextHorizontalAlignment.Center;

                case CellHorizontalAlignment.Right:
                    return TextHorizontalAlignment.Right;

                case CellHorizontalAlignment.General:
                    return TextHorizontalAlignment.General;
            }
            return TextHorizontalAlignment.General;
        }

        public static TextVerticalAlignment VA2TVA(CellVerticalAlignment s)
        {
            switch (s)
            {
                case CellVerticalAlignment.Top:
                    return TextVerticalAlignment.Top;

                case CellVerticalAlignment.Center:
                    return TextVerticalAlignment.Center;

                case CellVerticalAlignment.Bottom:
                    return TextVerticalAlignment.Bottom;
            }
            return TextVerticalAlignment.Justify;
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of contents in a cell.
        /// </summary>
        /// <value>
        /// A value that specifies the horizontal alignment for the contents.
        /// The default value is <see cref="T:Dt.Cells.Data.TextHorizontalAlignment">General</see>.
        /// </value>
        [DefaultValue(0)]
        public TextHorizontalAlignment HorizontalAlignment
        {
            get
            {
                if (this.horizontalAlignment.HasValue)
                {
                    return this.horizontalAlignment.Value;
                }
                return TextHorizontalAlignment.General;
            }
            set { this.horizontalAlignment = new TextHorizontalAlignment?(value); }
        }

        /// <summary>
        /// Gets or sets whether the text appears from right to left, such as when using Hebrew or Arabic fonts.
        /// </summary>
        /// <value>
        /// <c>true</c> if the text appears from right to left; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool RightToLeft
        {
            get
            {
                if (this.rightToLeft.HasValue)
                {
                    return this.rightToLeft.Value;
                }
                return false;
            }
            set { this.rightToLeft = new bool?(value); }
        }

        /// <summary>
        /// Gets or sets the amount to indent the text in a cell, in pixels.
        /// </summary>
        /// <value>The amount to indent the text in a cell, in pixels. The default value is 0.</value>
        [DefaultValue(0)]
        public int TextIndent
        {
            get
            {
                if (this.textIndent.HasValue)
                {
                    return this.textIndent.Value;
                }
                return 0;
            }
            set { this.textIndent = new int?(value); }
        }

        /// <summary>
        /// Gets or sets the text orientation in a cell.
        /// </summary>
        /// <value>
        /// A value that specifies how text is oriented when painting the cell.
        /// The default value is <see cref="P:Dt.Cells.Data.ContentAlignment.TextOrientation">TextHorizontal</see>.
        /// </value>
        [DefaultValue(0)]
        public Dt.Cells.Data.TextOrientation TextOrientation
        {
            get
            {
                if (this.textOrientation.HasValue)
                {
                    return this.textOrientation.Value;
                }
                return Dt.Cells.Data.TextOrientation.TextHorizontal;
            }
            set { this.textOrientation = new Dt.Cells.Data.TextOrientation?(value); }
        }

        /// <summary>
        /// Gets or sets the rotation angle of the text for the cell.
        /// </summary>
        /// <value>The rotation angle of the text for the cell in degrees. The default value is 0.0 degrees.</value>
        [DefaultValue((double) 0.0)]
        public double TextRotationAngle
        {
            get
            {
                if (this.textRotationAngle.HasValue)
                {
                    return this.textRotationAngle.Value;
                }
                return 0.0;
            }
            set { this.textRotationAngle = new double?(value); }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of contents in a cell.
        /// </summary>
        /// <value>
        /// A value that specifies the vertical alignment for the contents.
        /// The default value is <see cref="T:Dt.Cells.Data.TextVerticalAlignment">General</see>.
        /// </value>
        [DefaultValue(0)]
        public TextVerticalAlignment VerticalAlignment
        {
            get
            {
                if (this.verticalAlignment.HasValue)
                {
                    return this.verticalAlignment.Value;
                }
                return TextVerticalAlignment.General;
            }
            set { this.verticalAlignment = new TextVerticalAlignment?(value); }
        }

        /// <summary>
        /// Gets or sets whether to wrap words.
        /// </summary>
        /// <value>
        /// <c>true</c> if the content supports word wrap; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool WordWrap
        {
            get
            {
                if (this.wordWrap.HasValue)
                {
                    return this.wordWrap.Value;
                }
                return false;
            }
            set { this.wordWrap = new bool?(value); }
        }
    }
}

