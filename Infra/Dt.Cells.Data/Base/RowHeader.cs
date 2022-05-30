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
    /// Represents a row header for the sheet.
    /// </summary>
    public sealed class RowHeader : HeaderBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new row header for the sheet.
        /// </summary>
        /// <param name="worksheet">The worksheet that contains the new row header.</param>
        internal RowHeader(Worksheet worksheet) : base(worksheet)
        {
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the header displays letters or numbers
        /// or is blank.
        /// </summary>
        /// <value>
        /// A value that indicates what the header displays.
        /// The default value is <see cref="T:Dt.Cells.Data.HeaderAutoText">Numbers</see>.
        /// </value>
        [DefaultValue(1)]
        public override HeaderAutoText AutoText
        {
            get
            {
                if (base.Sheet != null)
                {
                    return base.Sheet.RowHeaderAutoText;
                }
                return HeaderAutoText.Letters;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.RowHeaderAutoText = value;
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
                    return base.Sheet.RowHeaderAutoTextIndex;
                }
                return 1;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.RowHeaderAutoTextIndex = value;
                    base.RaisePropertyChanged("AutoTextIndex");
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of columns in the row header.
        /// </summary>
        /// <value>
        /// The number of columns in the row header.
        /// The default value is 1.
        /// </value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The specified column count is less than 0 or greater than 256.
        /// </exception>
        [DefaultValue(1)]
        public int ColumnCount
        {
            get { return  base.Sheet.RowHeaderColumnCount; }
            set
            {
                if (base.Sheet.RowHeaderColumnCount != value)
                {
                    if ((0 > value) || (value > 0x100))
                    {
                        throw new ArgumentOutOfRangeException("value", string.Format(ResourceStrings.WorksheetInvalidRowHeaderColumnCount, (object[]) new object[] { ((int) 0x100) }));
                    }
                    base.Sheet.RowHeaderColumnCount = value;
                    base.RaisePropertyChanged("ColumnCount");
                }
            }
        }

        /// <summary>
        /// Gets or sets the default column width for the row header, in pixels.
        /// </summary>
        /// <value>
        /// The default column width for the row header.
        /// The default value is 20 pixels.
        /// </value>
        [DefaultValue(20)]
        public double DefaultColumnWidth
        {
            get
            {
                if (base.Sheet != null)
                {
                    return base.Sheet.DefaultRowHeaderColumnWidth;
                }
                return 0.0;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.DefaultRowHeaderColumnWidth = value;
                    base.RaisePropertyChanged("DefaultColumnWidth");
                }
            }
        }

        /// <summary>
        /// Gets or sets the default style information for the cells in the header.
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
                    return base.Sheet.RowHeaderDefaultStyle;
                }
                return null;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.RowHeaderDefaultStyle = value;
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
                    return base.Sheet.RowHeaderVisible;
                }
                return true;
            }
            set
            {
                if (base.Sheet != null)
                {
                    base.Sheet.RowHeaderVisible = value;
                    base.RaisePropertyChanged("IsVisible");
                }
            }
        }

        /// <summary>
        /// Gets the sheet area for the header.
        /// </summary>
        /// <value>
        /// The sheet area is always set to <see cref="P:Dt.Cells.Data.RowHeader.SheetArea">SheetArea.RowHeader</see>.
        /// </value>
        [DefaultValue(2)]
        protected override Dt.Cells.Data.SheetArea SheetArea
        {
            get { return  (Dt.Cells.Data.SheetArea.CornerHeader | Dt.Cells.Data.SheetArea.RowHeader); }
        }
    }
}

