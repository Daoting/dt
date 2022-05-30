#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// PartsSpanLayoutData
    /// </summary>
    internal class PartsSpanLayoutData
    {
        SpanLayoutData bottomLeftCornerSpans;
        SpanLayoutData bottomRightCornerSpans;
        SpanLayoutData cellSpans;
        SpanLayoutData columnFooterSpans;
        SpanLayoutData columnHeaderSpans;
        SpanLayoutData rowFooterSpans;
        SpanLayoutData rowHeaderSpans;
        SpanLayoutData topLeftCornerSpans;
        SpanLayoutData topRightCornerSpans;

        /// <summary>
        /// Gets the span layout data.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        public SpanLayoutData GetSpanLayoutData(SheetArea area)
        {
            switch (area)
            {
                case SheetArea.CornerHeader:
                    return this.topLeftCornerSpans;

                case SheetArea.Cells:
                    return this.cellSpans;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return this.rowHeaderSpans;

                case SheetArea.ColumnHeader:
                    return this.columnHeaderSpans;
            }
            throw new ArgumentOutOfRangeException("area");
        }

        /// <summary>
        /// Gets or sets the bottom left corner spans.
        /// </summary>
        /// <value>The bottom left corner spans.</value>
        public SpanLayoutData BottomLeftCornerSpans
        {
            get { return  this.bottomLeftCornerSpans; }
            set { this.bottomLeftCornerSpans = value; }
        }

        /// <summary>
        /// Gets or sets the bottom right corner spans.
        /// </summary>
        /// <value>The bottom right corner spans.</value>
        public SpanLayoutData BottomRightCornerSpans
        {
            get { return  this.bottomRightCornerSpans; }
            set { this.bottomRightCornerSpans = value; }
        }

        /// <summary>
        /// Gets or sets the cell spans.
        /// </summary>
        /// <value>The cell spans.</value>
        public SpanLayoutData CellSpans
        {
            get { return  this.cellSpans; }
            set { this.cellSpans = value; }
        }

        /// <summary>
        /// Gets or sets the column footer spans.
        /// </summary>
        /// <value>The column footer spans.</value>
        public SpanLayoutData ColumnFooterSpans
        {
            get { return  this.columnFooterSpans; }
            set { this.columnFooterSpans = value; }
        }

        /// <summary>
        /// Gets or sets the column header spans.
        /// </summary>
        /// <value>The column header spans.</value>
        public SpanLayoutData ColumnHeaderSpans
        {
            get { return  this.columnHeaderSpans; }
            set { this.columnHeaderSpans = value; }
        }

        /// <summary>
        /// Gets or sets the row footer spans.
        /// </summary>
        /// <value>The row footer spans.</value>
        public SpanLayoutData RowFooterSpans
        {
            get { return  this.rowFooterSpans; }
            set { this.rowFooterSpans = value; }
        }

        /// <summary>
        /// Gets or sets the row header spans.
        /// </summary>
        /// <value>The row header spans.</value>
        public SpanLayoutData RowHeaderSpans
        {
            get { return  this.rowHeaderSpans; }
            set { this.rowHeaderSpans = value; }
        }

        /// <summary>
        /// Gets or sets the top left corner spans.
        /// </summary>
        /// <value>The top left corner spans.</value>
        public SpanLayoutData TopLeftCornerSpans
        {
            get { return  this.topLeftCornerSpans; }
            set { this.topLeftCornerSpans = value; }
        }

        /// <summary>
        /// Gets or sets the top right corner spans.
        /// </summary>
        /// <value>The top right corner spans.</value>
        public SpanLayoutData TopRightCornerSpans
        {
            get { return  this.topRightCornerSpans; }
            set { this.topRightCornerSpans = value; }
        }
    }
}

