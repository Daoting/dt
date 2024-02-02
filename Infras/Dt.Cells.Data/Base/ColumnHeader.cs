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
    /// <summary>
    /// Represents a column header for the sheet.
    /// </summary>
    public sealed class ColumnHeader : HeaderBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new column header for the sheet.
        /// </summary>
        /// <param name="worksheet">The worksheet that contains the new column header.</param>
        internal ColumnHeader(Worksheet worksheet) : base(worksheet)
        {
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the header displays letters or numbers
        /// or is blank.
        /// </summary>
        /// <value>
        /// A value that indicates what the header displays.
        /// The default value is <see cref="T:Dt.Cells.Data.HeaderAutoText">Letters</see>.
        /// </value>
        [DefaultValue(2)]
        public override HeaderAutoText AutoText
        {
            get
            {
                if (base.Sheet != null)
                {
                    return base.Sheet.ColumnHeaderAutoText;
                }
                return HeaderAutoText.Letters;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.ColumnHeaderAutoText = value;
                    base.RaisePropertyChanged("AutoText");
                }
            }
        }

        /// <summary>
        /// Gets or sets which header row displays the automatic text
        /// when there are multiple header rows.
        /// </summary>
        /// <value>
        /// The row index of the header that displays the automatic text.
        /// The default value is 0.
        /// </value>
        [DefaultValue(0)]
        public override int AutoTextIndex
        {
            get
            {
                if (base.Sheet != null)
                {
                    return base.Sheet.ColumnHeaderAutoTextIndex;
                }
                return 1;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.ColumnHeaderAutoTextIndex = value;
                    base.RaisePropertyChanged("AutoTextIndex");
                }
            }
        }

        /// <summary>
        /// Gets or sets the default row height for the column header, in pixels.
        /// </summary>
        /// <value>
        /// The default row height for the column header.
        /// The default value is 20 pixels.
        /// </value>
        [DefaultValue(20)]
        public double DefaultRowHeight
        {
            get
            {
                if (base.Sheet != null)
                {
                    return base.Sheet.DefaultColumnHeaderRowHeight;
                }
                return 0.0;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.DefaultColumnHeaderRowHeight = value;
                    base.RaisePropertyChanged("DefaultRowHeight");
                }
            }
        }

        /// <summary>
        /// Gets or sets the default style information for the header cells.
        /// </summary>
        /// <value>
        /// The default style information for the cells in the header.
        /// The default value is null.
        /// </value>
        [DefaultValue((string) null)]
        public override StyleInfo DefaultStyle
        {
            get
            {
                if (base.Sheet != null)
                {
                    return base.Sheet.ColumnHeaderDefaultStyle;
                }
                return null;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.ColumnHeaderDefaultStyle = value;
                    base.RaisePropertyChanged("DefaultStyle");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the header is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if the header is visible; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public override bool IsVisible
        {
            get
            {
                if (base.Sheet != null)
                {
                    return base.Sheet.ColumnHeaderVisible;
                }
                return true;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.ColumnHeaderVisible = value;
                    base.RaisePropertyChanged("IsVisible");
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of rows in the column header.
        /// </summary>
        /// <value>
        /// The number of rows in the column header.
        /// The default value is 1.
        /// </value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The specified row count is less than 0 or greater than 256.
        /// </exception>
        [DefaultValue(1)]
        public int RowCount
        {
            get
            {
                if (base.Sheet != null)
                {
                    return base.Sheet.GetRowCount(this.SheetArea);
                }
                return 0;
            }
            set
            {
                if (base.Sheet.GetRowCount(this.SheetArea) != value)
                {
                    if ((0 > value) || (value > 0x100))
                    {
                        throw new ArgumentOutOfRangeException("value", string.Format(ResourceStrings.WorksheetInvalidRowHeaderColumnCount, (object[]) new object[] { ((int) 0x100) }));
                    }
                    base.Sheet.SetRowCount(this.SheetArea, value);
                    base.RaisePropertyChanged("RowCount");
                }
            }
        }

        /// <summary>
        /// Gets the sheet area for the header.
        /// </summary>
        /// <value>
        /// The sheet area is always set to <see cref="P:Dt.Cells.Data.ColumnHeader.SheetArea">SheetArea.ColumnHeader</see>.
        /// </value>
        [DefaultValue(4)]
        protected override Dt.Cells.Data.SheetArea SheetArea
        {
            get { return  Dt.Cells.Data.SheetArea.ColumnHeader; }
        }
    }
}

