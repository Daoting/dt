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
    /// PartsLayoutData
    /// </summary>
    internal class PartsLayoutData
    {
        PartLayoutData columnFooterHeights;
        PartLayoutData columnHeaderHeights;
        PartLayoutData columnWidths;
        PartLayoutData rowFooterWidths;
        PartLayoutData rowHeaderWidths;
        PartLayoutData rowHeights;

        /// <summary>
        /// Gets or sets the column footer heights.
        /// </summary>
        /// <value>The column footer heights.</value>
        public PartLayoutData ColumnFooterHeights
        {
            get { return  this.columnFooterHeights; }
            set { this.columnFooterHeights = value; }
        }

        /// <summary>
        /// Gets or sets the column header heights.
        /// </summary>
        /// <value>The column header heights.</value>
        public PartLayoutData ColumnHeaderHeights
        {
            get { return  this.columnHeaderHeights; }
            set { this.columnHeaderHeights = value; }
        }

        /// <summary>
        /// Gets or sets the column widths.
        /// </summary>
        /// <value>The column widths.</value>
        public PartLayoutData ColumnWidths
        {
            get { return  this.columnWidths; }
            set { this.columnWidths = value; }
        }

        /// <summary>
        /// Gets or sets the row footer widths.
        /// </summary>
        /// <value>The row footer widths.</value>
        public PartLayoutData RowFooterWidths
        {
            get { return  this.rowFooterWidths; }
            set { this.rowFooterWidths = value; }
        }

        /// <summary>
        /// Gets or sets the row header widths.
        /// </summary>
        /// <value>The row header widths.</value>
        public PartLayoutData RowHeaderWidths
        {
            get { return  this.rowHeaderWidths; }
            set { this.rowHeaderWidths = value; }
        }

        /// <summary>
        /// Gets or sets the row heights.
        /// </summary>
        /// <value>The row heights.</value>
        public PartLayoutData RowHeights
        {
            get { return  this.rowHeights; }
            set { this.rowHeights = value; }
        }
    }
}

