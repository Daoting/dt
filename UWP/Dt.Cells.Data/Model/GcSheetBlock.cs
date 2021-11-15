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
using System.Runtime.InteropServices;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// GcSheetBlock
    /// </summary>
    internal class GcSheetBlock : GcBlock
    {
        string bookmark;
        Windows.Foundation.Rect cellRect;
        int columnEndIndex;
        int columnStartIndex;
        bool footer;
        Windows.Foundation.Rect footerRect;
        bool frozen;
        Windows.Foundation.Rect frozenRect;
        bool frozenTrailing;
        Windows.Foundation.Rect frozenTrailingRect;
        bool header;
        Windows.Foundation.Rect headerRect;
        bool horizontal;
        bool recalcRects;
        bool repeat;
        Windows.Foundation.Rect repeatRect;
        int rowEndIndex;
        List<int> rowIndexs;
        int rowStartIndex;
        GcSheetSection.SheetState state;
        PartType type;
        Worksheet worksheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSheetBlock" /> class.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="state">The state.</param>
        public GcSheetBlock(double x, double y, double width, double height, Worksheet worksheet, GcSheetSection.SheetState state) : this(x, y, width, height, PartType.Cell, worksheet, state)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSheetBlock" /> class.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="type">The type.</param>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="state">The state.</param>
        public GcSheetBlock(double x, double y, double width, double height, PartType type, Worksheet worksheet, GcSheetSection.SheetState state) : base(x, y, width, height, null)
        {
            this.horizontal = true;
            this.recalcRects = true;
            this.type = type;
            this.worksheet = worksheet;
            this.state = state;
        }

        /// <summary>
        /// Checks the rectangles.
        /// </summary>
        void CheckRects()
        {
            if (this.recalcRects)
            {
                double height = this.horizontal ? base.Height : base.Width;
                double x = 0.0;
                double width = 0.0;
                if (this.Header)
                {
                    width = this.horizontal ? (this.state.HasRowHeader ? this.state.RowHeaderWidths.AllSize : 0.0) : (this.state.HasColumnHeader ? this.state.ColumnHeaderHeights.AllSize : 0.0);
                    if (this.horizontal)
                    {
                        this.headerRect = new Windows.Foundation.Rect(x, 0.0, width, height);
                    }
                    else
                    {
                        this.headerRect = new Windows.Foundation.Rect(0.0, x, height, width);
                    }
                    x += width;
                }
                if (this.Frozen)
                {
                    width = this.horizontal ? (this.state.HasFrozenColumn ? this.state.FrozenColumnWidths.AllSize : 0.0) : (this.state.HasFrozenRow ? this.state.FrozenRowHeights.AllSize : 0.0);
                    if (this.horizontal)
                    {
                        this.frozenRect = new Windows.Foundation.Rect(x, 0.0, width, height);
                    }
                    else
                    {
                        this.frozenRect = new Windows.Foundation.Rect(0.0, x, height, width);
                    }
                    x += width;
                }
                if (this.Repeat)
                {
                    width = this.horizontal ? (this.state.HasRepeatColumn ? this.state.RepeatColumnWidths.AllSize : 0.0) : (this.state.HasRepeatRow ? this.state.RepeatRowHeights.AllSize : 0.0);
                    if (this.horizontal)
                    {
                        this.repeatRect = new Windows.Foundation.Rect(x, 0.0, width, height);
                    }
                    else
                    {
                        this.repeatRect = new Windows.Foundation.Rect(0.0, x, height, width);
                    }
                    x += width;
                }
                if (this.horizontal)
                {
                    width = Utilities.GetColumnWidth(this.worksheet, this.state.ColumnWidths, SheetArea.Cells, this.ColumnStartIndex, this.ColumnCount);
                }
                else
                {
                    width = 0.0;
                    for (int i = this.RowStartIndex; i < (this.RowStartIndex + this.RowCount); i++)
                    {
                        width += Utilities.GetRowHeight(this.worksheet, this.state.RowHeights, SheetArea.Cells, this.GetAcutalRowIndex(i));
                    }
                }
                if (this.horizontal)
                {
                    this.cellRect = new Windows.Foundation.Rect(x, 0.0, width, height);
                }
                else
                {
                    this.cellRect = new Windows.Foundation.Rect(0.0, x, height, width);
                }
                x += width;
                if (this.FrozenTrailing)
                {
                    width = this.horizontal ? (this.state.HasFrozenTrailingColumn ? this.state.FrozenTrailingColumnWidths.AllSize : 0.0) : (this.state.HasFrozenTrailingRow ? this.state.FrozenTrailingRowHeights.AllSize : 0.0);
                    if (this.horizontal)
                    {
                        this.frozenTrailingRect = new Windows.Foundation.Rect(x, 0.0, width, height);
                    }
                    else
                    {
                        this.frozenTrailingRect = new Windows.Foundation.Rect(0.0, x, height, width);
                    }
                    x += width;
                }
                if (this.Footer)
                {
                    width = this.horizontal ? (this.state.HasRowFooter ? this.state.RowFooterWidths.AllSize : 0.0) : (this.state.HasColumnFooter ? this.state.ColumnFooterHeights.AllSize : 0.0);
                    if (this.horizontal)
                    {
                        this.footerRect = new Windows.Foundation.Rect(x, 0.0, width, height);
                    }
                    else
                    {
                        this.footerRect = new Windows.Foundation.Rect(0.0, x, height, width);
                    }
                }
                this.recalcRects = false;
            }
        }

        /// <summary>
        /// Gets the index of the actual row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public int GetAcutalRowIndex(int row)
        {
            if (this.RowIndexs != null)
            {
                return this.RowIndexs[row];
            }
            return row;
        }

        /// <summary>
        /// Gets the type of the area.
        /// </summary>
        /// <param name="partType">Type of the part.</param>
        /// <returns></returns>
        public SheetArea GetAreaType(PartType partType)
        {
            switch (this.type)
            {
                case PartType.Cell:
                case PartType.Frozen:
                case PartType.Repeat:
                case PartType.FrozenTrailing:
                    switch (partType)
                    {
                        case PartType.Cell:
                        case PartType.Frozen:
                        case PartType.Repeat:
                        case PartType.FrozenTrailing:
                            return SheetArea.Cells;

                        case PartType.Header:
                            if (!this.horizontal)
                            {
                                return SheetArea.ColumnHeader;
                            }
                            return (SheetArea.CornerHeader | SheetArea.RowHeader);
                    }
                    throw new ArgumentOutOfRangeException("partType");

                case PartType.Header:
                    switch (partType)
                    {
                        case PartType.Cell:
                        case PartType.Frozen:
                        case PartType.Repeat:
                        case PartType.FrozenTrailing:
                            if (!this.horizontal)
                            {
                                return (SheetArea.CornerHeader | SheetArea.RowHeader);
                            }
                            return SheetArea.ColumnHeader;

                        case PartType.Header:
                            return SheetArea.CornerHeader;
                    }
                    throw new ArgumentOutOfRangeException("partType");
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Gets the part.
        /// </summary>
        /// <param name="partType">Type of the part.</param>
        /// <param name="rs">The rs value.</param>
        /// <param name="re">The re value.</param>
        /// <param name="cs">The cs value.</param>
        /// <param name="ce">The ce value.</param>
        /// <param name="heights">The height values.</param>
        /// <param name="widths">The width  values.</param>
        /// <param name="spans">The spans.</param>
        public void GetPart(PartType partType, out int rs, out int re, out int cs, out int ce, out PartLayoutData heights, out PartLayoutData widths, out SpanLayoutData spans)
        {
            rs = -1;
            re = -1;
            cs = -1;
            ce = -1;
            heights = null;
            widths = null;
            if (this.horizontal)
            {
                rs = this.RowStartIndex;
                re = this.RowEndIndex;
            }
            else
            {
                cs = this.ColumnStartIndex;
                ce = this.ColumnEndIndex;
            }
            switch (partType)
            {
                case PartType.Cell:
                    rs = this.RowStartIndex;
                    re = this.RowEndIndex;
                    cs = this.ColumnStartIndex;
                    ce = this.ColumnEndIndex;
                    heights = this.state.RowHeights;
                    widths = this.state.ColumnWidths;
                    break;

                case PartType.Header:
                    if (this.horizontal)
                    {
                        if (!this.state.HasRowHeader)
                        {
                            throw new ArgumentOutOfRangeException("partType");
                        }
                        cs = 0;
                        ce = this.worksheet.RowHeaderColumnCount - 1;
                        widths = this.state.RowHeaderWidths;
                        break;
                    }
                    if (!this.state.HasColumnHeader)
                    {
                        throw new ArgumentOutOfRangeException("partType");
                    }
                    rs = 0;
                    re = this.worksheet.ColumnHeaderRowCount - 1;
                    heights = this.state.ColumnHeaderHeights;
                    break;

                case PartType.Frozen:
                    if (this.horizontal)
                    {
                        if (!this.state.HasFrozenColumn)
                        {
                            throw new ArgumentOutOfRangeException("partType");
                        }
                        cs = 0;
                        ce = this.state.FrozenColumnWidths.Sizes.Count - 1;
                        widths = this.state.ColumnWidths;
                        break;
                    }
                    if (!this.state.HasFrozenRow)
                    {
                        throw new ArgumentOutOfRangeException("partType");
                    }
                    rs = 0;
                    re = this.state.FrozenRowHeights.Sizes.Count - 1;
                    heights = this.state.RowHeights;
                    break;

                case PartType.Repeat:
                    if (this.horizontal)
                    {
                        if (!this.state.HasRepeatColumn)
                        {
                            throw new ArgumentOutOfRangeException("partType");
                        }
                        cs = this.state.RepeatColumnStartIndex;
                        ce = this.state.RepeatColumnEndIndex;
                        widths = this.state.ColumnWidths;
                        break;
                    }
                    if (!this.state.HasRepeatRow)
                    {
                        throw new ArgumentOutOfRangeException("partType");
                    }
                    rs = this.state.RepeatRowStartIndex;
                    re = this.state.RepeatRowEndIndex;
                    heights = this.state.RowHeights;
                    break;

                case PartType.FrozenTrailing:
                    if (this.horizontal)
                    {
                        if (!this.state.HasFrozenTrailingColumn)
                        {
                            throw new ArgumentOutOfRangeException("partType");
                        }
                        cs = this.worksheet.ColumnCount - this.worksheet.FrozenTrailingColumnCount;
                        ce = this.worksheet.ColumnCount - 1;
                        widths = this.state.ColumnWidths;
                        break;
                    }
                    if (!this.state.HasFrozenTrailingRow)
                    {
                        throw new ArgumentOutOfRangeException("partType");
                    }
                    rs = this.worksheet.RowCount - this.worksheet.FrozenTrailingRowCount;
                    re = this.worksheet.RowCount - 1;
                    heights = this.state.RowHeights;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("partType");
            }
            switch (this.type)
            {
                case PartType.Cell:
                case PartType.Frozen:
                case PartType.Repeat:
                case PartType.FrozenTrailing:
                    if (!this.horizontal)
                    {
                        widths = this.state.ColumnWidths;
                        break;
                    }
                    heights = this.state.RowHeights;
                    break;

                case PartType.Header:
                    if (!this.horizontal)
                    {
                        widths = this.state.RowHeaderWidths;
                        break;
                    }
                    heights = this.state.ColumnHeaderHeights;
                    break;

                case PartType.Footer:
                    if (!this.horizontal)
                    {
                        widths = this.state.RowFooterWidths;
                        break;
                    }
                    heights = this.state.ColumnFooterHeights;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (this.GetAreaType(partType))
            {
                case SheetArea.CornerHeader:
                    spans = this.state.TopLeftCornerSpans;
                    return;

                case SheetArea.Cells:
                    spans = this.state.CellSpans;
                    return;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    spans = this.state.RowHeaderSpans;
                    return;

                case SheetArea.ColumnHeader:
                    spans = this.state.ColumnHeaderSpans;
                    return;
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <returns></returns>
        public Windows.Foundation.Rect GetRect()
        {
            this.CheckRects();
            Windows.Foundation.Rect empty = Windows.Foundation.Rect.Empty;
            empty.Union(this.cellRect);
            empty.Union(this.headerRect);
            empty.Union(this.frozenRect);
            empty.Union(this.repeatRect);
            empty.Union(this.frozenTrailingRect);
            empty.Union(this.footerRect);
            return empty;
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <param name="part">The type of the rectangle.</param>
        /// <returns></returns>
        public Windows.Foundation.Rect GetRect(PartType part)
        {
            this.CheckRects();
            switch (part)
            {
                case PartType.Cell:
                    return this.cellRect;

                case PartType.Header:
                    return this.headerRect;

                case PartType.Frozen:
                    return this.frozenRect;

                case PartType.Repeat:
                    return this.repeatRect;

                case PartType.FrozenTrailing:
                    return this.frozenTrailingRect;

                case PartType.Footer:
                    return this.footerRect;
            }
            throw new ArgumentOutOfRangeException("part");
        }

        /// <summary>
        /// Gets the bookmark.
        /// </summary>
        /// <value>The bookmark.</value>
        public string Bookmark
        {
            get { return  this.bookmark; }
            set { this.bookmark = value; }
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>The column count.</value>
        public int ColumnCount
        {
            get { return  ((this.ColumnEndIndex - this.ColumnStartIndex) + 1); }
        }

        /// <summary>
        /// Gets or sets the end index of the column.
        /// </summary>
        /// <value>The end index of the column.</value>
        public int ColumnEndIndex
        {
            get { return  this.columnEndIndex; }
            set
            {
                if (this.columnEndIndex != value)
                {
                    this.recalcRects = true;
                }
                this.columnEndIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the start index of the column.
        /// </summary>
        /// <value>The start index of the column.</value>
        public int ColumnStartIndex
        {
            get { return  this.columnStartIndex; }
            set
            {
                if (this.columnStartIndex != value)
                {
                    this.recalcRects = true;
                }
                this.columnStartIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="T:Dt.Cells.Data.GcSheetBlock" /> is a footer.
        /// </summary>
        /// <value><c>true</c> if a footer; otherwise, <c>false</c>.</value>
        public bool Footer
        {
            get { return  this.footer; }
            set
            {
                if (this.footer != value)
                {
                    this.recalcRects = true;
                }
                this.footer = value;
            }
        }

        /// <summary>
        /// Gets the footer area.
        /// </summary>
        /// <value>The footer area.</value>
        public SheetArea FooterArea
        {
            get { return  this.GetAreaType(PartType.Footer); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="T:Dt.Cells.Data.GcSheetBlock" /> is frozen.
        /// </summary>
        /// <value><c>true</c> if frozen; otherwise, <c>false</c>.</value>
        public bool Frozen
        {
            get { return  this.frozen; }
            set
            {
                if (this.frozen != value)
                {
                    this.recalcRects = true;
                }
                this.frozen = value;
            }
        }

        /// <summary>
        /// Gets the frozen area.
        /// </summary>
        /// <value>The frozen area.</value>
        public SheetArea FrozenArea
        {
            get { return  this.GetAreaType(PartType.Frozen); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether there is a frozen trailing item.
        /// </summary>
        /// <value><c>true</c> if [frozen trailing]; otherwise, <c>false</c>.</value>
        public bool FrozenTrailing
        {
            get { return  this.frozenTrailing; }
            set
            {
                if (this.frozenTrailing != value)
                {
                    this.recalcRects = true;
                }
                this.frozenTrailing = value;
            }
        }

        /// <summary>
        /// Gets the frozen trailing area.
        /// </summary>
        /// <value>The frozen trailing area.</value>
        public SheetArea FrozenTrailingArea
        {
            get { return  this.GetAreaType(PartType.FrozenTrailing); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="T:Dt.Cells.Data.GcSheetBlock" /> is a header.
        /// </summary>
        /// <value><c>true</c> if a header; otherwise, <c>false</c>.</value>
        public bool Header
        {
            get { return  this.header; }
            set
            {
                if (this.header != value)
                {
                    this.recalcRects = true;
                }
                this.header = value;
            }
        }

        /// <summary>
        /// Gets the header area.
        /// </summary>
        /// <value>The header area.</value>
        public SheetArea HeaderArea
        {
            get { return  this.GetAreaType(PartType.Header); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="T:Dt.Cells.Data.GcSheetBlock" /> is horizontal.
        /// </summary>
        /// <value><c>true</c> if horizontal; otherwise, <c>false</c>.</value>
        public bool Horizontal
        {
            get { return  this.horizontal; }
            set
            {
                if (this.horizontal != value)
                {
                    this.recalcRects = true;
                }
                this.horizontal = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="T:Dt.Cells.Data.GcSheetBlock" /> is repeated.
        /// </summary>
        /// <value><c>true</c> if repeated; otherwise, <c>false</c>.</value>
        public bool Repeat
        {
            get { return  this.repeat; }
            set
            {
                if (this.repeat != value)
                {
                    this.recalcRects = true;
                }
                this.repeat = value;
            }
        }

        /// <summary>
        /// Gets the repeated area.
        /// </summary>
        /// <value>The repeated area.</value>
        public SheetArea RepeatArea
        {
            get { return  this.GetAreaType(PartType.Repeat); }
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <value>The row count.</value>
        public int RowCount
        {
            get { return  ((this.RowEndIndex - this.RowStartIndex) + 1); }
        }

        /// <summary>
        /// Gets or sets the end index of the row.
        /// </summary>
        /// <value>The end index of the row.</value>
        public int RowEndIndex
        {
            get { return  this.rowEndIndex; }
            set
            {
                if (this.rowEndIndex != value)
                {
                    this.recalcRects = true;
                }
                this.rowEndIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the row indexes.
        /// </summary>
        /// <value>The row indexes.</value>
        public List<int> RowIndexs
        {
            get { return  this.rowIndexs; }
            set { this.rowIndexs = value; }
        }

        /// <summary>
        /// Gets or sets the start index of the row.
        /// </summary>
        /// <value>The start index of the row.</value>
        public int RowStartIndex
        {
            get { return  this.rowStartIndex; }
            set
            {
                if (this.rowStartIndex != value)
                {
                    this.recalcRects = true;
                }
                this.rowStartIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the sheet.
        /// </summary>
        /// <value>The sheet.</value>
        public Worksheet Sheet
        {
            get { return  this.worksheet; }
            set
            {
                if (this.worksheet != value)
                {
                    this.recalcRects = true;
                }
                this.worksheet = value;
            }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public GcSheetSection.SheetState State
        {
            get { return  this.state; }
            set
            {
                if (this.state != value)
                {
                    this.recalcRects = true;
                }
                this.state = value;
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public PartType Type
        {
            get { return  this.type; }
            set
            {
                if (this.type != value)
                {
                    this.recalcRects = true;
                }
                this.type = value;
            }
        }

        /// <summary>
        /// PartType
        /// </summary>
        internal enum PartType
        {
            Cell,
            Header,
            Frozen,
            Repeat,
            FrozenTrailing,
            Footer
        }
    }
}

