#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents metadata for a line object.
    /// </summary>
    public class LineItem : IThemeContextSupport
    {
        List<Rect> _bounds;

        IThemeSupport IThemeContextSupport.GetContext()
        {
            return null;
        }

        void IThemeContextSupport.SetContext(IThemeSupport context)
        {
            this.SetContext(context);
        }

        internal void SetContext(IThemeSupport context)
        {
            if (this.Line != null)
            {
                ((IThemeContextSupport) this.Line).SetContext(context);
            }
            if (this.PreviousLine != null)
            {
                ((IThemeContextSupport) this.PreviousLine).SetContext(context);
            }
            if (this.NextLine != null)
            {
                ((IThemeContextSupport) this.NextLine).SetContext(context);
            }
            if (this.PreviousBreaker1 != null)
            {
                ((IThemeContextSupport) this.PreviousBreaker1).SetContext(context);
            }
            if (this.PreviousBreaker2 != null)
            {
                ((IThemeContextSupport) this.PreviousBreaker2).SetContext(context);
            }
            if (this.NextBreaker1 != null)
            {
                ((IThemeContextSupport) this.NextBreaker1).SetContext(context);
            }
            if (this.NextBreaker2 != null)
            {
                ((IThemeContextSupport) this.NextBreaker2).SetContext(context);
            }
        }

        /// <summary>
        /// Gets the boundary of the line.
        /// </summary>
        /// <remarks>
        /// This list consists of the boundary of all the cells this line covers.
        /// </remarks>
        public List<Rect> Bounds
        {
            get
            {
                if (this._bounds == null)
                {
                    this._bounds = new List<Rect>();
                }
                return this._bounds;
            }
        }

        /// <summary>
        /// Gets or sets the end column of this LineItem object.
        /// </summary>
        /// <value>The end column of this LineItem object.</value>
        public int ColumnEnd { get; set; }

        /// <summary>
        /// Gets or sets the column from which this LineItem object starts.
        /// </summary>
        /// <value>The column.</value>
        public int ColumnFrom { get; set; }

        /// <summary>
        /// Gets or sets the direction of this LineItem object.
        /// </summary>
        /// <value>The direction of this LineItem object.</value>
        public int Direction { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this instance is a grid line.
        /// </summary>
        /// <value><c>true</c> if this instance is a grid line; otherwise, <c>false</c>.</value>
        public bool IsGridLine { get; set; }

        /// <summary>
        /// Gets or sets the main line of this LineItem object.
        /// </summary>
        public BorderLine Line { get; set; }

        /// <summary>
        /// Gets or sets the next breaker1 item.
        /// </summary>
        /// <value>The next breaker1.</value>
        public BorderLine NextBreaker1 { get; set; }

        /// <summary>
        /// Gets or sets the next breaker2 item.
        /// </summary>
        /// <value>The next breaker2.</value>
        public BorderLine NextBreaker2 { get; set; }

        /// <summary>
        /// Gets or sets the next line of this LineItem object.
        /// </summary>
        public BorderLine NextLine { get; set; }

        /// <summary>
        /// Gets or sets the previous breaker1 item.
        /// </summary>
        /// <value>The previous breaker1.</value>
        public BorderLine PreviousBreaker1 { get; set; }

        /// <summary>
        /// Gets or sets the previous breaker2 item.
        /// </summary>
        /// <value>The previous breaker2.</value>
        public BorderLine PreviousBreaker2 { get; set; }

        /// <summary>
        /// Gets or sets the previous line of this LineItem object.
        /// </summary>
        /// <value>The previous line.</value>
        public BorderLine PreviousLine { get; set; }

        /// <summary>
        /// Gets or sets the end row of this LineItem object.
        /// </summary>
        /// <value>The row end row of this LineItem object.</value>
        public int RowEnd { get; set; }

        /// <summary>
        /// Gets or sets the row from which this LineItem object starts.
        /// </summary>
        /// <value>The row this LineItem object starts from.</value>
        public int RowFrom { get; set; }
    }
}

