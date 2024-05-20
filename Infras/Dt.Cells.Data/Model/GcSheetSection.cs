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
using System.Xml;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a section that is used for display of the <see cref="T:Dt.Cells.Data.Worksheet" />.
    /// </summary>
    internal partial class GcSheetSection : GcMultiplePageSection, IGcAllowAppendixSection
    {
        bool flagName;
        string sheetName;
        internal const string Suffix = "ETEN2J";
        Worksheet worksheet;

        /// <summary>
        /// Creates a new section for the worksheet.
        /// </summary>
        public GcSheetSection()
        {
            this.Init();
        }

        /// <summary>
        /// Creates a new section for the specified worksheet.
        /// </summary>
        /// <param name="worksheet">The <see cref="T:Dt.Cells.Data.Worksheet" /> object.</param>
        public GcSheetSection(Worksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("sheet");
            }
            this.Init();
            this.worksheet = worksheet;
            this.sheetName = worksheet.Name;
        }
        
        /// <summary>
        /// Gets the column widths.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="area">The area</param>
        /// <param name="startIndex">The start index</param>
        /// <param name="count">The count</param>
        /// <param name="context">The context.</param>
        /// <param name="autoFit">if set to <c>true</c> [auto fit].</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="repeatStartIndex">Start index of the repeat.</param>
        /// <param name="repeatEndIndex">End index of the repeat.</param>
        /// <param name="hasTopPart">if set to <c>true</c> [has top part].</param>
        /// <param name="hasBottomPart">if set to <c>true</c> [has bottom part].</param>
        /// <param name="showGridline">if set to <c>true</c> [show gridline].</param>
        /// <param name="borderCollapse">The border collapse.</param>
        /// <returns></returns>
        static List<double> GetColumnWidths(Worksheet worksheet, SheetArea area, int startIndex, int count, GcReportContext context, bool autoFit, int rowStartIndex, int rowEndIndex, int repeatStartIndex, int repeatEndIndex, bool hasTopPart, bool hasBottomPart, bool showGridline, BorderCollapse borderCollapse)
        {
            return Utilities.GetColumnWidths(worksheet, area, startIndex, count, context, autoFit, rowStartIndex, rowEndIndex, repeatStartIndex, repeatEndIndex, hasTopPart, hasBottomPart, UnitType.CentileInch, showGridline, borderCollapse);
        }

        /// <summary>
        /// Gets the row heights.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="area">The area</param>
        /// <param name="startIndex">The start index</param>
        /// <param name="count">The count</param>
        /// <param name="context">The context</param>
        /// <param name="autoFit">If set to <c>true</c>, [auto fit]</param>
        /// <param name="columnStartIndex">Start index of the column</param>
        /// <param name="columnEndIndex">End index of the column</param>
        /// <param name="repeatStartIndex">Start index of the repeat</param>
        /// <param name="repeatEndIndex">End index of the repeat</param>
        /// <param name="hasLeftPart">if set to <c>true</c> [has left part].</param>
        /// <param name="hasRightPart">if set to <c>true</c> [has right part].</param>
        /// <param name="colWidths">The col widths.</param>
        /// <param name="lColWidths">The l col widths.</param>
        /// <param name="rColWidths">The r col widths.</param>
        /// <param name="showGridline">if set to <c>true</c> [show gridline].</param>
        /// <param name="borderCollapse">The border collapse.</param>
        /// <returns></returns>
        static List<double> GetRowHeights(Worksheet worksheet, SheetArea area, int startIndex, int count, GcReportContext context, bool autoFit, int columnStartIndex, int columnEndIndex, int repeatStartIndex, int repeatEndIndex, bool hasLeftPart, bool hasRightPart, PartLayoutData colWidths, PartLayoutData lColWidths, PartLayoutData rColWidths, bool showGridline, BorderCollapse borderCollapse)
        {
            return Utilities.GetRowHeights(worksheet, area, startIndex, count, context, autoFit, columnStartIndex, columnEndIndex, repeatStartIndex, repeatEndIndex, hasLeftPart, hasRightPart, colWidths, lColWidths, rColWidths, UnitType.CentileInch, showGridline, borderCollapse);
        }

        /// <summary>
        /// Gets the sheet note part.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="range">The range</param>
        /// <param name="nps">The NPS</param>
        /// <param name="lineNumber">The line number</param>
        /// <param name="offsetY">The offset Y</param>
        /// <param name="width">The width</param>
        /// <param name="high">The height</param>
        /// <param name="titleLabel">The title label</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        static bool GetSheetNotePart(GcReportContext context, GcRangeBlock range, NotePrintState nps, int lineNumber, ref int offsetY, int width, int high, GcLabel titleLabel, string value)
        {
            int lastOffsetX = 0;
            int num2 = 0;
            if (nps.LastColumn <= 0)
            {
                if (nps.HasLastState)
                {
                    nps.ResetLastState();
                }
                GcBlock block = new GcBlock((double) lastOffsetX, (double) offsetY, (double) titleLabel.Width, (double) titleLabel.Height, titleLabel);
                num2 = Math.Max(num2, titleLabel.Height);
                if (high > (offsetY + num2))
                {
                    range.Blocks.Add(block);
                }
                else
                {
                    nps.LastLine = lineNumber;
                    nps.LastColumn = 0;
                    nps.LastOffsetX = lastOffsetX;
                    nps.LastOffsetY = 0;
                    return false;
                }
                lastOffsetX += titleLabel.Width;
            }
            if (nps.LastColumn <= 1)
            {
                if (nps.HasLastState)
                {
                    lastOffsetX = nps.LastOffsetX;
                    offsetY = nps.LastOffsetY;
                    nps.ResetLastState();
                }
                GcLabel label = new GcLabel(value) {
                    X = lastOffsetX,
                    Y = offsetY,
                    Width = width - lastOffsetX
                };
                label.Font = nps.NoteFont;
                label.AutoSize(context);
                num2 = Math.Max(num2, label.Height);
                range.Blocks.Add(label.GetBlock(context));
                if (high < (offsetY + num2))
                {
                    nps.LastLine = lineNumber;
                    nps.LastColumn = 1;
                    nps.LastOffsetX = lastOffsetX;
                    nps.LastOffsetY = offsetY - high;
                    return false;
                }
                offsetY += num2;
            }
            return true;
        }

        /// <summary>
        /// Gets the sheet note range.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <param name="width">The width</param>
        /// <param name="high">The height</param>
        /// <param name="sectionState">State of the section</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <returns></returns>
        static List<GcRangeBlock> GetSheetNoteRange(GcReportContext context, int x, int y, int width, int high, SheetSectionState sectionState, bool horizontal)
        {
            if ((sectionState == null) || !SheetNotesHasMorePage(sectionState, horizontal))
            {
                return null;
            }
            List<GcRangeBlock> list = new List<GcRangeBlock>();
            GcRangeBlock range = new GcRangeBlock((double) x, (double) y, (double) width, (double) high);
            list.Add(range);
            int offsetY = 0;
            NotePrintState notePrintState = sectionState.NotePrintState;
            while (notePrintState.Index < sectionState.NoteStates.Count)
            {
                NoteState state2 = sectionState.NoteStates[notePrintState.Index];
                if (((notePrintState.LastLine <= 0) && notePrintState.IsMultiSheet) && (!string.IsNullOrEmpty(state2.Sheet.Name) && !GetSheetNotePart(context, range, notePrintState, 0, ref offsetY, width, high, notePrintState.SheetTitle, state2.Sheet.Name)))
                {
                    return list;
                }
                if ((notePrintState.LastLine <= 1) && !GetSheetNotePart(context, range, notePrintState, 1, ref offsetY, width, high, notePrintState.CellTitle, string.Format("{0} {1}", (object[]) new object[] { state2.Sheet.GetColumnLabel(state2.ColumnIndex), state2.Sheet.GetRowLabel(state2.RowIndex) })))
                {
                    return list;
                }
                notePrintState.Index++;
            }
            return list;
        }

        /// <summary>
        /// Gets the state of the sheet section.
        /// </summary>
        /// <param name="buildInControlState">State of the build in control</param>
        /// <returns></returns>
        static SheetSectionState GetSheetSectionState(object buildInControlState)
        {
            if (buildInControlState == null)
            {
                return null;
            }
            return (buildInControlState as SheetSectionState);
        }

        /// <summary>
        /// Determines whether [has more page] [the specified build in control state].
        /// </summary>
        /// <param name="buildInControlState">State of the built-in control.</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal].</param>
        /// <returns>
        /// <c>true</c> if [has more page] [the specified built-in control state]; otherwise, <c>false</c>.
        /// </returns>
        protected override bool HasMorePage(object buildInControlState, bool horizontal)
        {
            SheetSectionState sheetSectionState = GetSheetSectionState(buildInControlState);
            if (sheetSectionState == null)
            {
                return false;
            }
            if (!SheetHasMorePage(sheetSectionState, horizontal))
            {
                return SheetNotesHasMorePage(sheetSectionState, horizontal);
            }
            return true;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.worksheet = null;
            this.sheetName = string.Empty;
            this.flagName = false;
        }

        /// <summary>
        /// Determines whether [is print note mode] [the specified section state].
        /// </summary>
        /// <param name="sectionState">State of the section</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <returns>
        /// <c>true</c> if [is print note mode] [the specified section state]; otherwise, <c>false</c>
        /// </returns>
        static bool IsPrintNoteMode(SheetSectionState sectionState, bool horizontal)
        {
            return (!SheetHasMorePage(sectionState, horizontal) && SheetNotesHasMorePage(sectionState, horizontal));
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void ReadXmlBase(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
            {
                switch (reader.Name)
                {
                    case "SheetName":
                        this.sheetName = Serializer.ReadAttribute("value", reader);
                        return;

                    case "FlagName":
                        this.flagName = Serializer.ReadAttributeBoolean("value", false, reader);
                        return;
                }
                base.ReadXmlBase(reader);
            }
        }

        /// <summary>
        /// The sheet has more pages.
        /// </summary>
        /// <param name="sectionState">State of the section</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <returns></returns>
        static bool SheetHasMorePage(SheetSectionState sectionState, bool horizontal)
        {
            bool flag = false;
            if (horizontal)
            {
                foreach (SheetPrintState state in sectionState.CurrentPrintStates)
                {
                    if ((state.CurrentColumnIndex <= state.ColumnEndIndex) && (state.LastRowCount != 0))
                    {
                        return true;
                    }
                }
                return flag;
            }
            if (sectionState.PrintStateStack.Count > 0)
            {
                return true;
            }
            if (sectionState.CurrentPrintStates.Count > 0)
            {
                SheetPrintState state2 = sectionState.CurrentPrintStates[sectionState.CurrentPrintStates.Count - 1];
                flag = (state2.CurrentRowIndex + state2.LastRowCount) <= state2.RowEndIndex;
            }
            return flag;
        }

        /// <summary>
        /// The sheet notes have more pages.
        /// </summary>
        /// <param name="sectionState">State of the section</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <returns></returns>
        static bool SheetNotesHasMorePage(SheetSectionState sectionState, bool horizontal)
        {
            return ((!horizontal && (sectionState.NoteStates.Count > 0)) && (sectionState.NotePrintState.Index < sectionState.NoteStates.Count));
        }

        /// <summary>
        /// Shows the part.
        /// </summary>
        /// <param name="area">The area</param>
        /// <param name="worksheet">The sheet</param>
        /// <param name="settings">The settings</param>
        /// <returns></returns>
        static bool ShowPart(SheetArea area, Worksheet worksheet, PrintInfo settings)
        {
            VisibilityType showRowHeader;
            bool rowHeaderVisible;
            int rowHeaderColumnCount;
            switch (area)
            {
                case SheetArea.Cells:
                    return true;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    showRowHeader = settings.ShowRowHeader;
                    rowHeaderVisible = worksheet.RowHeaderVisible;
                    rowHeaderColumnCount = worksheet.RowHeaderColumnCount;
                    break;

                case SheetArea.ColumnHeader:
                    showRowHeader = settings.ShowColumnHeader;
                    rowHeaderVisible = worksheet.ColumnHeaderVisible;
                    rowHeaderColumnCount = worksheet.ColumnHeaderRowCount;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("area");
            }
            switch (showRowHeader)
            {
                case VisibilityType.Inherit:
                    if (!rowHeaderVisible)
                    {
                        return false;
                    }
                    return (rowHeaderColumnCount > 0);

                case VisibilityType.Hide:
                    return false;

                case VisibilityType.Show:
                    return (rowHeaderColumnCount > 0);

                case VisibilityType.ShowOnce:
                    return (rowHeaderColumnCount > 0);
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Writes the XML base.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteXmlBase(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            base.WriteXmlBase(writer);
            if (!this.flagName)
            {
                this.sheetName = (this.worksheet == null) ? string.Empty : this.worksheet.Name;
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets a value indicating whether [flag name].
        /// </summary>
        /// <value><c>true</c> if [flag name]; otherwise, <c>false</c></value>
        internal bool FlagName
        {
            get { return  this.flagName; }
            set { this.flagName = value; }
        }

        /// <summary>
        /// Gets or sets the worksheet.
        /// </summary>
        /// <value>The <see cref="T:Dt.Cells.Data.Worksheet" /> object.</value>
        public Worksheet Sheet
        {
            get { return  this.worksheet; }
            set
            {
                this.worksheet = value;
                this.sheetName = (this.worksheet != null) ? this.worksheet.Name : string.Empty;
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the name of the sheet.
        /// </summary>
        /// <value>The name of the sheet</value>
        internal string SheetName
        {
            get { return  this.sheetName; }
            set { this.sheetName = value; }
        }

        /// <summary>
        /// Internal only.
        /// NotePrintState
        /// </summary>
        internal class NotePrintState
        {
            GcLabel cellTitle;
            GcLabel commentTitle;
            int index;
            bool isMultiSheet;
            int lastColumn = -1;
            int lastLine = -1;
            int lastOffsetX;
            int lastOffsetY;
            const string NoteCellTitle = "Cell: ";
            const string NoteCommentTitle = "Comment: ";
            Font noteFont;
            Font noteHeadFont;
            const string NoteSheetTitle = "Sheet: ";
            GcLabel sheetTitle;

            /// <summary>
            /// Creates all title controls.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="font">The font.</param>
            public void CreatAllTitleControls(GcReportContext context, Font font)
            {
                this.sheetTitle = new GcLabel("Sheet: ");
                this.sheetTitle.Font = font;
                this.sheetTitle.Alignment.WordWrap = false;
                this.sheetTitle.AutoSize(context);
                this.cellTitle = new GcLabel("Cell: ");
                this.cellTitle.Font = font;
                this.cellTitle.Alignment.WordWrap = false;
                this.cellTitle.AutoSize(context);
                this.commentTitle = new GcLabel("Comment: ");
                this.commentTitle.Font = font;
                this.commentTitle.Alignment.WordWrap = false;
                this.commentTitle.AutoSize(context);
            }

            /// <summary>
            /// Resets the last state.
            /// </summary>
            public void ResetLastState()
            {
                this.LastLine = -1;
                this.LastColumn = -1;
                this.LastOffsetX = 0;
                this.LastOffsetY = 0;
            }

            /// <summary>
            /// Gets the cell title.
            /// </summary>
            /// <value>The cell title.</value>
            public GcLabel CellTitle
            {
                get { return  this.cellTitle; }
            }

            /// <summary>
            /// Gets the comment title.
            /// </summary>
            /// <value>The comment title.</value>
            public GcLabel CommentTitle
            {
                get { return  this.commentTitle; }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a last state.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a last state; otherwise, <c>false</c>.
            /// </value>
            public bool HasLastState
            {
                get { return  (this.lastLine != -1); }
            }

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            /// <value>The index.</value>
            public int Index
            {
                get { return  this.index; }
                set { this.index = value; }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this instance has multiple sheets.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has multiple sheets; otherwise, <c>false</c>.
            /// </value>
            public bool IsMultiSheet
            {
                get { return  this.isMultiSheet; }
                set { this.isMultiSheet = value; }
            }

            /// <summary>
            /// Gets or sets the last column.
            /// </summary>
            /// <value>The last column.</value>
            public int LastColumn
            {
                get { return  this.lastColumn; }
                set { this.lastColumn = value; }
            }

            /// <summary>
            /// Gets or sets the last line.
            /// </summary>
            /// <value>The last line.</value>
            public int LastLine
            {
                get { return  this.lastLine; }
                set { this.lastLine = value; }
            }

            /// <summary>
            /// Gets or sets the last X offset.
            /// </summary>
            /// <value>The last X offset.</value>
            public int LastOffsetX
            {
                get { return  this.lastOffsetX; }
                set { this.lastOffsetX = value; }
            }

            /// <summary>
            /// Gets or sets the last Y offset.
            /// </summary>
            /// <value>The last Y offset.</value>
            public int LastOffsetY
            {
                get { return  this.lastOffsetY; }
                set { this.lastOffsetY = value; }
            }

            /// <summary>
            /// Gets or sets the note font.
            /// </summary>
            /// <value>The note font.</value>
            public Font NoteFont
            {
                get { return  this.noteFont; }
                set { this.noteFont = value; }
            }

            /// <summary>
            /// Gets or sets the note head font.
            /// </summary>
            /// <value>The note head font.</value>
            public Font NoteHeadFont
            {
                get { return  this.noteHeadFont; }
                set { this.noteHeadFont = value; }
            }

            /// <summary>
            /// Gets the sheet title.
            /// </summary>
            /// <value>The sheet title.</value>
            public GcLabel SheetTitle
            {
                get { return  this.sheetTitle; }
            }
        }

        /// <summary>
        /// Internal only.
        /// NoteState
        /// </summary>
        internal class NoteState
        {
            readonly int columnIndex;
            readonly int rowIndex;
            readonly Worksheet worksheet;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSheetSection.NoteState" /> class.
            /// </summary>
            /// <param name="worksheet">The sheet.</param>
            /// <param name="rowIndex">The row index.</param>
            /// <param name="columnIndex">The column index.</param>
            public NoteState(Worksheet worksheet, int rowIndex, int columnIndex)
            {
                this.worksheet = worksheet;
                this.rowIndex = rowIndex;
                this.columnIndex = columnIndex;
            }

            /// <summary>
            /// Gets the column index.
            /// </summary>
            /// <value>The index of the column.</value>
            public int ColumnIndex
            {
                get { return  this.columnIndex; }
            }

            /// <summary>
            /// Gets the index of the row.
            /// </summary>
            /// <value>The index of the row.</value>
            public int RowIndex
            {
                get { return  this.rowIndex; }
            }

            /// <summary>
            /// Gets the sheet.
            /// </summary>
            /// <value>The sheet.</value>
            public Worksheet Sheet
            {
                get { return  this.worksheet; }
            }
        }

        /// <summary>
        /// Internal only.
        /// SheetPrintState
        /// </summary>
        internal class SheetPrintState
        {
            int columnEndIndex;
            int columnStartIndex;
            int currentColumnIndex;
            int currentRowIndex;
            int lastRowCount;
            double offsetX;
            int rowEndIndex;
            List<int> rowIndexs;
            int rowStartIndex;
            bool seekChild = true;
            readonly Dt.Cells.Data.GcSheetSection.SheetState sheetState;
            readonly Worksheet worksheet;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSheetSection.SheetPrintState" /> class.
            /// </summary>
            /// <param name="worksheet">The sheet.</param>
            /// <param name="sheetState">State of the sheet.</param>
            public SheetPrintState(Worksheet worksheet, Dt.Cells.Data.GcSheetSection.SheetState sheetState)
            {
                this.worksheet = worksheet;
                this.sheetState = sheetState;
                this.rowStartIndex = sheetState.RowStartIndex;
                this.rowEndIndex = sheetState.RowEndIndex;
                this.columnStartIndex = sheetState.ColumnStartIndex;
                this.columnEndIndex = sheetState.ColumnEndIndex;
                this.currentRowIndex = this.rowStartIndex;
                this.currentColumnIndex = this.columnStartIndex;
            }

            /// <summary>
            /// Gets the index of the displayed row.
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
            /// Splits the row.
            /// </summary>
            /// <param name="row">The row.</param>
            /// <returns></returns>
            public GcSheetSection.SheetPrintState SplitRow(int row)
            {
                if (this.rowIndexs != null)
                {
                    row = this.rowIndexs.IndexOf(row);
                }
                if ((row >= this.RowEndIndex) || (row < 0))
                {
                    return null;
                }
                GcSheetSection.SheetPrintState state = new GcSheetSection.SheetPrintState(this.worksheet, this.sheetState);
                if (this.rowIndexs == null)
                {
                    state.rowStartIndex = row + 1;
                    state.currentRowIndex = state.rowStartIndex;
                    state.rowEndIndex = this.rowEndIndex;
                    this.rowEndIndex = row;
                }
                else
                {
                    state.rowStartIndex = 0;
                    state.currentRowIndex = 0;
                    state.rowIndexs = new List<int>();
                    state.rowIndexs.AddRange((IEnumerable<int>) this.rowIndexs.GetRange(row + 1, (this.rowIndexs.Count - row) - 1));
                    state.rowEndIndex = state.rowIndexs.Count - 1;
                    this.rowIndexs.RemoveRange(row + 1, (this.rowIndexs.Count - row) - 1);
                    this.rowEndIndex = this.rowIndexs.Count - 1;
                }
                state.offsetX = this.offsetX;
                state.seekChild = this.seekChild;
                return state;
            }

            /// <summary>
            /// Gets or sets the end index of the column.
            /// </summary>
            /// <value>The end index of the column.</value>
            public int ColumnEndIndex
            {
                get { return  this.columnEndIndex; }
                set { this.columnEndIndex = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the column.
            /// </summary>
            /// <value>The start index of the column.</value>
            public int ColumnStartIndex
            {
                get { return  this.columnStartIndex; }
                set { this.columnStartIndex = value; }
            }

            /// <summary>
            /// Gets or sets the index of the current column.
            /// </summary>
            /// <value>The index of the current column.</value>
            public int CurrentColumnIndex
            {
                get { return  this.currentColumnIndex; }
                set { this.currentColumnIndex = value; }
            }

            /// <summary>
            /// Gets or sets the index of the current row.
            /// </summary>
            /// <value>The index of the current row.</value>
            public int CurrentRowIndex
            {
                get { return  this.currentRowIndex; }
                set { this.currentRowIndex = value; }
            }

            /// <summary>
            /// Gets or sets the last row count.
            /// </summary>
            /// <value>The last row count.</value>
            public int LastRowCount
            {
                get { return  this.lastRowCount; }
                set { this.lastRowCount = value; }
            }

            /// <summary>
            /// Gets or sets the X offset.
            /// </summary>
            /// <value>The X offset.</value>
            public double OffsetX
            {
                get { return  this.offsetX; }
                set { this.offsetX = value; }
            }

            /// <summary>
            /// Gets or sets the end index of the row.
            /// </summary>
            /// <value>The end index of the row.</value>
            public int RowEndIndex
            {
                get { return  this.rowEndIndex; }
                set { this.rowEndIndex = value; }
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
                set { this.rowStartIndex = value; }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether to search for child values.
            /// </summary>
            /// <value><c>true</c> if [seek child]; otherwise, <c>false</c>.</value>
            public bool SeekChild
            {
                get { return  this.seekChild; }
                set { this.seekChild = value; }
            }

            /// <summary>
            /// Gets the sheet.
            /// </summary>
            /// <value>The sheet.</value>
            public Worksheet Sheet
            {
                get { return  this.worksheet; }
            }

            /// <summary>
            /// Gets the state of the sheet.
            /// </summary>
            /// <value>The state of the sheet.</value>
            public Dt.Cells.Data.GcSheetSection.SheetState SheetState
            {
                get { return  this.sheetState; }
            }
        }

        /// <summary>
        /// Internal only.
        /// SheetSectionState
        /// </summary>
        internal class SheetSectionState
        {
            readonly List<DataRelation> circleRelation = new List<DataRelation>();
            readonly List<GcSheetSection.SheetPrintState> currentPrintStates = new List<GcSheetSection.SheetPrintState>();
            Worksheet groupSheet;
            readonly Dt.Cells.Data.GcSheetSection.NotePrintState notePrintState = new Dt.Cells.Data.GcSheetSection.NotePrintState();
            readonly List<GcSheetSection.NoteState> noteStates = new List<GcSheetSection.NoteState>();
            readonly Stack<GcSheetSection.SheetPrintState> printStateStack = new Stack<GcSheetSection.SheetPrintState>();
            readonly Dictionary<Worksheet, GcSheetSection.SheetState> sheetStates = new Dictionary<Worksheet, GcSheetSection.SheetState>();

            /// <summary>
            /// Gets the circle relation.
            /// </summary>
            /// <value>The circle relation.</value>
            public List<DataRelation> CircleRelation
            {
                get { return  this.circleRelation; }
            }

            /// <summary>
            /// Gets the current print states.
            /// </summary>
            /// <value>The current print states.</value>
            public List<GcSheetSection.SheetPrintState> CurrentPrintStates
            {
                get { return  this.currentPrintStates; }
            }

            /// <summary>
            /// Gets or sets the group sheet.
            /// </summary>
            /// <value>The group sheet.</value>
            public Worksheet GroupSheet
            {
                get { return  this.groupSheet; }
                set { this.groupSheet = value; }
            }

            /// <summary>
            /// Gets the state of the print note.
            /// </summary>
            /// <value>The state of the print note.</value>
            public Dt.Cells.Data.GcSheetSection.NotePrintState NotePrintState
            {
                get { return  this.notePrintState; }
            }

            /// <summary>
            /// Gets the note states.
            /// </summary>
            /// <value>The note states.</value>
            public List<GcSheetSection.NoteState> NoteStates
            {
                get { return  this.noteStates; }
            }

            /// <summary>
            /// Gets the print state stack.
            /// </summary>
            /// <value>The print state stack.</value>
            public Stack<GcSheetSection.SheetPrintState> PrintStateStack
            {
                get { return  this.printStateStack; }
            }

            /// <summary>
            /// Gets the sheet states.
            /// </summary>
            /// <value>The sheet states.</value>
            public Dictionary<Worksheet, GcSheetSection.SheetState> SheetStates
            {
                get { return  this.sheetStates; }
            }
        }

        /// <summary>
        /// Internal only.
        /// SheetState
        /// </summary>
        internal class SheetState
        {
            SpanLayoutData bottomLeftCornerSpans;
            SpanLayoutData bottomRightCornerSpans;
            SpanLayoutData cellSpans;
            int columnEndIndex;
            PartLayoutData columnFooterHeights;
            SpanLayoutData columnFooterSpans;
            PartLayoutData columnHeaderHeights;
            SpanLayoutData columnHeaderSpans;
            int columnStartIndex;
            PartLayoutData columnWidths;
            PartLayoutData frozenColumnWidths;
            PartLayoutData frozenRowHeights;
            PartLayoutData frozenTrailingColumnWidths;
            PartLayoutData frozenTrailingRowHeights;
            bool hasHierarchy;
            bool isBookmarked;
            bool isHierarchy;
            int repeatColumnEndIndex = -1;
            int repeatColumnStartIndex = -1;
            PartLayoutData repeatColumnWidths;
            int repeatRowEndIndex = -1;
            PartLayoutData repeatRowHeights;
            int repeatRowStartIndex = -1;
            int rowEndIndex;
            SpanLayoutData rowFooterSpans;
            PartLayoutData rowFooterWidths;
            SpanLayoutData rowHeaderSpans;
            PartLayoutData rowHeaderWidths;
            PartLayoutData rowHeights;
            int rowStartIndex;
            readonly PrintInfo settings;
            readonly Dictionary<Rect, List<Shape>> shapes = new Dictionary<Rect, List<Shape>>();
            readonly Dictionary<Rect, List<StickyNote>> stickyNotes = new Dictionary<Rect, List<StickyNote>>();
            SpanLayoutData topLeftCornerSpans;
            SpanLayoutData topRightCornerSpans;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSheetSection.SheetState" /> class.
            /// </summary>
            /// <param name="settings">The settings.</param>
            public SheetState(PrintInfo settings)
            {
                this.settings = settings;
            }

            public bool NeedRepeatColumn(int columnIndex)
            {
                return (this.HasRepeatColumn && (columnIndex > this.RepeatColumnEndIndex));
            }

            public bool NeedRepeatRow(int rowIndex)
            {
                return (this.HasRepeatRow && (rowIndex > this.RepeatRowEndIndex));
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
                set { this.columnEndIndex = value; }
            }

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
            /// Gets or sets the column footer spans.
            /// </summary>
            /// <value>The column footer spans.</value>
            public SpanLayoutData ColumnFooterSpans
            {
                get { return  this.columnFooterSpans; }
                set { this.columnFooterSpans = value; }
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
            /// Gets or sets the column header spans.
            /// </summary>
            /// <value>The column header spans.</value>
            public SpanLayoutData ColumnHeaderSpans
            {
                get { return  this.columnHeaderSpans; }
                set { this.columnHeaderSpans = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the column.
            /// </summary>
            /// <value>The start index of the column.</value>
            public int ColumnStartIndex
            {
                get { return  this.columnStartIndex; }
                set { this.columnStartIndex = value; }
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
            /// Gets or sets the frozen column widths.
            /// </summary>
            /// <value>The frozen column widths.</value>
            public PartLayoutData FrozenColumnWidths
            {
                get { return  this.frozenColumnWidths; }
                set { this.frozenColumnWidths = value; }
            }

            /// <summary>
            /// Gets or sets the frozen row heights.
            /// </summary>
            /// <value>The frozen row heights.</value>
            public PartLayoutData FrozenRowHeights
            {
                get { return  this.frozenRowHeights; }
                set { this.frozenRowHeights = value; }
            }

            /// <summary>
            /// Gets or sets the frozen trailing column widths.
            /// </summary>
            /// <value>The frozen trailing column widths.</value>
            public PartLayoutData FrozenTrailingColumnWidths
            {
                get { return  this.frozenTrailingColumnWidths; }
                set { this.frozenTrailingColumnWidths = value; }
            }

            /// <summary>
            /// Gets or sets the frozen trailing row heights.
            /// </summary>
            /// <value>The frozen trailing row heights.</value>
            public PartLayoutData FrozenTrailingRowHeights
            {
                get { return  this.frozenTrailingRowHeights; }
                set { this.frozenTrailingRowHeights = value; }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a bottom left corner.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a bottom left corner; otherwise, <c>false</c>.
            /// </value>
            public bool HasBottomLeftCorner
            {
                get { return  (this.HasColumnFooter && this.HasRowHeader); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a bottom right corner.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a bottom right corner; otherwise, <c>false</c>.
            /// </value>
            public bool HasBottomRightCorner
            {
                get { return  (this.HasColumnFooter && this.HasRowFooter); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a column footer.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a column footer; otherwise, <c>false</c>.
            /// </value>
            public bool HasColumnFooter
            {
                get { return  (this.ColumnFooterHeights != null); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a column header.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a column header; otherwise, <c>false</c>.
            /// </value>
            public bool HasColumnHeader
            {
                get { return  (this.ColumnHeaderHeights != null); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has frozen columns.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has frozen columns; otherwise, <c>false</c>.
            /// </value>
            public bool HasFrozenColumn
            {
                get { return  (!this.IsHierarchy && (this.FrozenColumnWidths != null)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has frozen rows.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has frozen rows; otherwise, <c>false</c>.
            /// </value>
            public bool HasFrozenRow
            {
                get { return  (!this.IsHierarchy && (this.FrozenRowHeights != null)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has frozen trailing columns.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has frozen trailing columns; otherwise, <c>false</c>.
            /// </value>
            public bool HasFrozenTrailingColumn
            {
                get { return  (!this.IsHierarchy && (this.FrozenTrailingColumnWidths != null)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has frozen trailing rows.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has frozen trailing rows; otherwise, <c>false</c>.
            /// </value>
            public bool HasFrozenTrailingRow
            {
                get { return  (!this.IsHierarchy && (this.FrozenTrailingRowHeights != null)); }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this instance has a hierarchy.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a hierarchy; otherwise, <c>false</c>.
            /// </value>
            public bool HasHierarchy
            {
                get { return  this.hasHierarchy; }
                set { this.hasHierarchy = value; }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has repeated columns.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has repeated columns; otherwise, <c>false</c>.
            /// </value>
            public bool HasRepeatColumn
            {
                get { return  (((!this.IsHierarchy && (this.RepeatColumnStartIndex >= 0)) && (this.RepeatColumnEndIndex >= 0)) && (this.RepeatColumnEndIndex >= this.RepeatColumnStartIndex)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has repeated rows.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has repeated rows; otherwise, <c>false</c>.
            /// </value>
            public bool HasRepeatRow
            {
                get { return  (((!this.IsHierarchy && (this.RepeatRowStartIndex >= 0)) && (this.RepeatRowEndIndex >= 0)) && (this.RepeatRowEndIndex >= this.RepeatRowStartIndex)); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a row footer.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a row footer; otherwise, <c>false</c>.
            /// </value>
            public bool HasRowFooter
            {
                get { return  (this.RowFooterWidths != null); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a row header.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a row header; otherwise, <c>false</c>.
            /// </value>
            public bool HasRowHeader
            {
                get { return  (this.RowHeaderWidths != null); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a top left corner.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a top left corner; otherwise, <c>false</c>.
            /// </value>
            public bool HasTopLeftCorner
            {
                get { return  (this.HasColumnHeader && this.HasRowHeader); }
            }

            /// <summary>
            /// Gets a value that indicates whether this instance has a top right corner.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has a top right corner; otherwise, <c>false</c>.
            /// </value>
            public bool HasTopRightCorner
            {
                get { return  (this.HasColumnHeader && this.HasRowFooter); }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this instance is bookmarked.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is bookmarked; otherwise, <c>false</c>.
            /// </value>
            public bool IsBookmarked
            {
                get { return  this.isBookmarked; }
                set { this.isBookmarked = value; }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this instance is a hierarchy.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is a hierarchy; otherwise, <c>false</c>.
            /// </value>
            public bool IsHierarchy
            {
                get { return  this.isHierarchy; }
                set { this.isHierarchy = value; }
            }

            /// <summary>
            /// Gets the repeated column count.
            /// </summary>
            /// <value>The repeated column count.</value>
            public int RepeatColumnCount
            {
                get
                {
                    if (!this.HasRepeatColumn)
                    {
                        return 0;
                    }
                    return ((this.RepeatColumnEndIndex - this.RepeatColumnStartIndex) + 1);
                }
            }

            /// <summary>
            /// Gets or sets the end index of the repeated column.
            /// </summary>
            /// <value>The end index of the repeated column.</value>
            public int RepeatColumnEndIndex
            {
                get { return  this.repeatColumnEndIndex; }
                set { this.repeatColumnEndIndex = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the repeated column.
            /// </summary>
            /// <value>The start index of the repeated column.</value>
            public int RepeatColumnStartIndex
            {
                get { return  this.repeatColumnStartIndex; }
                set { this.repeatColumnStartIndex = value; }
            }

            /// <summary>
            /// Gets or sets the repeated column widths.
            /// </summary>
            /// <value>The repeated column widths.</value>
            public PartLayoutData RepeatColumnWidths
            {
                get { return  this.repeatColumnWidths; }
                set { this.repeatColumnWidths = value; }
            }

            /// <summary>
            /// Gets the repeated row count.
            /// </summary>
            /// <value>The repeated row count.</value>
            public int RepeatRowCount
            {
                get
                {
                    if (!this.HasRepeatRow)
                    {
                        return 0;
                    }
                    return ((this.RepeatRowEndIndex - this.RepeatRowStartIndex) + 1);
                }
            }

            /// <summary>
            /// Gets or sets the end index of the repeated row.
            /// </summary>
            /// <value>The end index of the repeated row.</value>
            public int RepeatRowEndIndex
            {
                get { return  this.repeatRowEndIndex; }
                set { this.repeatRowEndIndex = value; }
            }

            /// <summary>
            /// Gets or sets the repeated row heights.
            /// </summary>
            /// <value>The repeated row heights.</value>
            public PartLayoutData RepeatRowHeights
            {
                get { return  this.repeatRowHeights; }
                set { this.repeatRowHeights = value; }
            }

            /// <summary>
            /// Gets or sets the start index of the repeated row.
            /// </summary>
            /// <value>The start index of the repeated row.</value>
            public int RepeatRowStartIndex
            {
                get { return  this.repeatRowStartIndex; }
                set { this.repeatRowStartIndex = value; }
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
                set { this.rowEndIndex = value; }
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
            /// Gets or sets the row footer widths.
            /// </summary>
            /// <value>The row footer widths.</value>
            public PartLayoutData RowFooterWidths
            {
                get { return  this.rowFooterWidths; }
                set { this.rowFooterWidths = value; }
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

            /// <summary>
            /// Gets or sets the start index of the row.
            /// </summary>
            /// <value>The start index of the row</value>
            public int RowStartIndex
            {
                get { return  this.rowStartIndex; }
                set { this.rowStartIndex = value; }
            }

            /// <summary>
            /// Gets the shapes.
            /// </summary>
            /// <value>The shapes.</value>
            public Dictionary<Rect, List<Shape>> Shapes
            {
                get { return  this.shapes; }
            }

            /// <summary>
            /// Gets a value that indicates whether to show the border.
            /// </summary>
            /// <value><c>true</c> to show the border; otherwise, <c>false</c>.</value>
            public bool ShowBorder
            {
                get { return  this.settings.ShowBorder; }
            }

            /// <summary>
            /// Gets a value that indicates whether to show the grid line.
            /// </summary>
            /// <value><c>true</c> to show the grid line; otherwise, <c>false</c>.</value>
            public bool ShowGridLine
            {
                get { return  this.settings.ShowGridLine; }
            }

            /// <summary>
            /// Gets the sticky notes.
            /// </summary>
            /// <value>The sticky notes.</value>
            public Dictionary<Rect, List<StickyNote>> StickyNotes
            {
                get { return  this.stickyNotes; }
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
}

