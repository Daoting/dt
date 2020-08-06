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
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a header for the sheet area base.
    /// </summary>
    public abstract class HeaderBase : ICellsSupport, INotifyPropertyChanged
    {
        /// <summary>
        /// cells of sheet.
        /// </summary>
        Dt.Cells.Data.Cells cells;
        /// <summary>
        /// columns of sheet.
        /// </summary>
        Dt.Cells.Data.Columns columns;
        /// <summary>
        /// rows of sheet.
        /// </summary>
        Dt.Cells.Data.Rows rows;
        /// <summary>
        /// Sheet Owner
        /// </summary>
        Worksheet worksheet;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new header for the sheet area base.
        /// </summary>
        /// <param name="worksheet">The worksheet that contains the new header.</param>
        internal HeaderBase(Worksheet worksheet)
        {
            this.worksheet = worksheet;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        internal void RaisePropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null) && ((this.worksheet == null) || !this.worksheet.IsEventSuspend()))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the header displays letters or numbers
        /// or is blank.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.HeaderAutoText" /> enumeration that indicates what the header displays.</value>
        public abstract HeaderAutoText AutoText { get; set; }

        /// <summary>
        /// Gets or sets which header row displays the automatic text
        /// when there are multiple header rows.
        /// </summary>
        /// <value>The row index of the header that displays the automatic text.</value>
        public abstract int AutoTextIndex { get; set; }

        /// <summary>
        /// Gets the cells in the sheet header.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.Cell" /> object for the cells in the sheet header.</value>
        public Dt.Cells.Data.Cells Cells
        {
            get
            {
                if (this.cells == null)
                {
                    this.cells = new Dt.Cells.Data.Cells(this.worksheet, this.SheetArea);
                }
                return this.cells;
            }
        }

        /// <summary>
        /// Gets the columns in the sheet header.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.Column" /> object for the columns in the sheet header.</value>
        public Dt.Cells.Data.Columns Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new Dt.Cells.Data.Columns(this.worksheet, this.SheetArea);
                }
                return this.columns;
            }
        }

        /// <summary>
        /// Gets or sets the default style information for the cells in the header.
        /// </summary>
        /// <value>The default style information for the cells in the header.</value>
        public abstract StyleInfo DefaultStyle { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the header is visible.
        /// </summary>
        /// <value><c>true</c> if the header is visible; otherwise, <c>false</c>.</value>
        public abstract bool IsVisible { get; set; }

        /// <summary>
        /// Gets the rows in the sheet header.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.Row" /> object for the rows in the sheet header.</value>
        public Dt.Cells.Data.Rows Rows
        {
            get
            {
                if (this.rows == null)
                {
                    this.rows = new Dt.Cells.Data.Rows(this.worksheet, this.SheetArea);
                }
                return this.rows;
            }
        }

        /// <summary>
        /// Gets the worksheet object.
        /// </summary>
        /// <value>The worksheet object.</value>
        protected Worksheet Sheet
        {
            get { return  this.worksheet; }
        }

        /// <summary>
        /// Gets the sheet area for the header.
        /// </summary>
        /// <value>A <see cref="P:Dt.Cells.Data.HeaderBase.SheetArea" /> enumeration that indicates the sheet area of the header.</value>
        protected abstract Dt.Cells.Data.SheetArea SheetArea { get; }
    }
}

