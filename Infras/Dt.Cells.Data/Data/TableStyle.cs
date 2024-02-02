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
    /// Represents table style settings.
    /// </summary>
    public sealed class TableStyle : ICloneable
    {
        TableStyleInfo firstColumn;
        TableStyleInfo firstColumnStrip;
        int firstColumnStripSize;
        TableStyleInfo firstFooterCell;
        TableStyleInfo firstHeaderCell;
        TableStyleInfo firstRowStrip;
        int firstRowStripSize;
        TableStyleInfo footerRow;
        TableStyleInfo headerRow;
        TableStyleInfo lastColumn;
        TableStyleInfo lastFooterCell;
        TableStyleInfo lastHeaderCell;
        string name;
        TableStyleInfo secondColumnStrip;
        int secondColumnStripSize;
        TableStyleInfo secondRowStrip;
        int secondRowStripSize;
        TableStyleInfo wholeTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.TableStyle" /> class.
        /// </summary>
        public TableStyle()
        {
            this.Init();
        }

        /// <summary>
        /// Clones the table style.
        /// </summary>
        /// <returns>The cloned TableStyle instance.</returns>
        public object Clone()
        {
            return new TableStyle { 
                name = this.name, headerRow = this.Clone(this.headerRow), footerRow = this.Clone(this.footerRow), wholeTable = this.Clone(this.wholeTable), lastColumn = this.Clone(this.lastColumn), firstColumn = this.Clone(this.firstColumn), firstRowStrip = this.Clone(this.firstRowStrip), secondRowStrip = this.Clone(this.secondRowStrip), firstColumnStrip = this.Clone(this.firstColumnStrip), secondColumnStrip = this.Clone(this.secondColumnStrip), firstHeaderCell = this.Clone(this.firstHeaderCell), lastHeaderCell = this.Clone(this.lastHeaderCell), firstFooterCell = this.Clone(this.firstFooterCell), lastFooterCell = this.Clone(this.lastFooterCell), firstRowStripSize = this.firstRowStripSize, secondRowStripSize = this.secondRowStripSize, 
                firstColumnStripSize = this.firstColumnStripSize, secondColumnStripSize = this.secondColumnStripSize
             };
        }

        TableStyleInfo Clone(TableStyleInfo styleInfo)
        {
            if (styleInfo != null)
            {
                return (styleInfo.Clone() as TableStyleInfo);
            }
            return null;
        }

        void Init()
        {
            this.name = string.Empty;
            this.headerRow = null;
            this.footerRow = null;
            this.wholeTable = null;
            this.lastColumn = null;
            this.firstColumn = null;
            this.firstRowStrip = null;
            this.secondRowStrip = null;
            this.firstColumnStrip = null;
            this.secondColumnStrip = null;
            this.firstHeaderCell = null;
            this.lastHeaderCell = null;
            this.firstFooterCell = null;
            this.lastFooterCell = null;
            this.firstRowStripSize = 1;
            this.secondRowStripSize = 1;
            this.firstColumnStripSize = 1;
            this.secondColumnStripSize = 1;
        }

        /// <summary>
        /// The size of the first alternating column.
        /// </summary>
        public int FirstColumnStripSize
        {
            get { return  this.firstColumnStripSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.firstColumnStripSize = value;
            }
        }

        /// <summary>
        /// The style of the first alternating column.
        /// </summary>
        public TableStyleInfo FirstColumnStripStyle
        {
            get { return  this.firstColumnStrip; }
            set { this.firstColumnStrip = value; }
        }

        /// <summary>
        /// The style of the first footer cell.
        /// </summary>
        public TableStyleInfo FirstFooterCellStyle
        {
            get { return  this.firstFooterCell; }
            set { this.firstFooterCell = value; }
        }

        /// <summary>
        /// The style of the first header cell.
        /// </summary>
        public TableStyleInfo FirstHeaderCellStyle
        {
            get { return  this.firstHeaderCell; }
            set { this.firstHeaderCell = value; }
        }

        /// <summary>
        /// The size of the first alternating row.
        /// </summary>
        public int FirstRowStripSize
        {
            get { return  this.firstRowStripSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.firstRowStripSize = value;
            }
        }

        /// <summary>
        /// The first alternating row style.
        /// </summary>
        public TableStyleInfo FirstRowStripStyle
        {
            get { return  this.firstRowStrip; }
            set { this.firstRowStrip = value; }
        }

        /// <summary>
        /// The default style of the footer area.
        /// </summary>
        public TableStyleInfo FooterRowStyle
        {
            get { return  this.footerRow; }
            set { this.footerRow = value; }
        }

        /// <summary>
        /// The default style of the header area.
        /// </summary>
        public TableStyleInfo HeaderRowStyle
        {
            get { return  this.headerRow; }
            set { this.headerRow = value; }
        }

        /// <summary>
        /// The style of the first column.
        /// </summary>
        public TableStyleInfo HighlightFirstColumnStyle
        {
            get { return  this.firstColumn; }
            set { this.firstColumn = value; }
        }

        /// <summary>
        /// The style of the last column.
        /// </summary>
        public TableStyleInfo HighlightLastColumnStyle
        {
            get { return  this.lastColumn; }
            set { this.lastColumn = value; }
        }

        /// <summary>
        /// The style of the last footer cell.
        /// </summary>
        public TableStyleInfo LastFooterCellStyle
        {
            get { return  this.lastFooterCell; }
            set { this.lastFooterCell = value; }
        }

        /// <summary>
        /// The style of the last header cell.
        /// </summary>
        public TableStyleInfo LastHeaderCellStyle
        {
            get { return  this.lastHeaderCell; }
            set { this.lastHeaderCell = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return  this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// The size of the second alternating column.
        /// </summary>
        public int SecondColumnStripSize
        {
            get { return  this.secondColumnStripSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.secondColumnStripSize = value;
            }
        }

        /// <summary>
        /// The style of the second alternating column.
        /// </summary>
        public TableStyleInfo SecondColumnStripStyle
        {
            get { return  this.secondColumnStrip; }
            set { this.secondColumnStrip = value; }
        }

        /// <summary>
        /// The size of the second alternating row.
        /// </summary>
        public int SecondRowStripSize
        {
            get { return  this.secondRowStripSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.secondRowStripSize = value;
            }
        }

        /// <summary>
        /// The second alternating row style.
        /// </summary>
        public TableStyleInfo SecondRowStripStyle
        {
            get { return  this.secondRowStrip; }
            set { this.secondRowStrip = value; }
        }

        /// <summary>
        /// The default style of the data area.
        /// </summary>
        public TableStyleInfo WholeTableStyle
        {
            get { return  this.wholeTable; }
            set { this.wholeTable = value; }
        }
    }
}

