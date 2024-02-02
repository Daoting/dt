#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents a cell range reference expression.
    /// </summary>
    public class CalcSheetRangeExpression : CalcReferenceExpression
    {
        private int _endColumn;
        private bool _endColumnRelative;
        private int _endRow;
        private bool _endRowRelative;
        private ICalcSource _endSource;
        private bool _isFullColumn;
        private bool _isFullRow;
        private int _startColumn;
        private bool _startColumnRelative;
        private int _startRow;
        private bool _startRowRelative;
        private ICalcSource _startSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcRangeExpression" /> class.
        /// </summary>
        /// <param name="startSource">Starting owner of cell range.</param>
        /// <param name="endSource">Ending owner of cell range.</param>
        public CalcSheetRangeExpression(ICalcSource startSource, ICalcSource endSource)
        {
            this._startSource = startSource;
            this._endSource = endSource;
            this._startRow = -2147483648;
            this._endRow = -2147483648;
            this._startColumn = -2147483648;
            this._endColumn = -2147483648;
            this._isFullRow = true;
            this._isFullColumn = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcRangeExpression" /> class.
        /// </summary>
        /// <param name="startSource">Starting owner of cell range.</param>
        /// <param name="endSource">Ending owner of cell range.</param>
        /// <param name="startRow">Starting row coordinate of the range.</param>
        /// <param name="startColumn">Starting column coordinate of the range.</param>
        /// <param name="endRow">Ending row coordinate of the range.</param>
        /// <param name="endColumn">Ending column coordinate of the range.</param>
        public CalcSheetRangeExpression(ICalcSource startSource, ICalcSource endSource, int startRow, int startColumn, int endRow, int endColumn) : this(startSource, endSource, startRow, startColumn, endRow, endColumn, false, false, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcSheetRangeExpression" /> class.
        /// </summary>
        /// <param name="startSource">Starting owner of cell range.</param>
        /// <param name="endSource">Ending owner of cell range.</param>
        /// <param name="startBandIndex">Starting row or column coordinate of the range.</param>
        /// <param name="endBandIndex">Ending row or column coordinate of the range.</param>
        /// <param name="startBandRelative">Whether the start row or column coordinate is relative or absolute.</param>
        /// <param name="endBandRelative">Whether the end row or column coordinate is relative or absolute.</param>
        /// <param name="isRowBand"><see langword="true" /> if current range is a full row range, otherwise, 
        /// current range is a full column range.</param>
        public CalcSheetRangeExpression(ICalcSource startSource, ICalcSource endSource, int startBandIndex, int endBandIndex, bool startBandRelative, bool endBandRelative, bool isRowBand)
        {
            this._startSource = startSource;
            this._endSource = endSource;
            if (startBandIndex > endBandIndex)
            {
                int num = startBandIndex;
                startBandIndex = endBandIndex;
                endBandIndex = num;
                bool flag = startBandRelative;
                startBandRelative = endBandRelative;
                endBandRelative = flag;
            }
            if (isRowBand)
            {
                this._startRow = startBandIndex;
                this._startColumn = -2147483648;
                this._endRow = endBandIndex;
                this._endColumn = -2147483648;
                this._startRowRelative = startBandRelative;
                this._startColumnRelative = false;
                this._endRowRelative = endBandRelative;
                this._endColumnRelative = false;
                this._isFullRow = true;
            }
            else
            {
                this._startRow = -2147483648;
                this._startColumn = startBandIndex;
                this._endRow = -2147483648;
                this._endColumn = endBandIndex;
                this._startRowRelative = false;
                this._startColumnRelative = startBandRelative;
                this._endRowRelative = false;
                this._endColumnRelative = endBandRelative;
                this._isFullColumn = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcRangeExpression" /> class.
        /// </summary>
        /// <param name="startSource">Starting owner of cell range.</param>
        /// <param name="endSource">Ending owner of cell range.</param>
        /// <param name="startRow">Starting row coordinate of the range.</param>
        /// <param name="startColumn">Starting column coordinate of the range.</param>
        /// <param name="endRow">Ending row coordinate of the range.</param>
        /// <param name="endColumn">Ending column coordinate of the range.</param>
        /// <param name="startRowRelative">Whether the start row coordinate is relative or absolute.</param>
        /// <param name="startColumnRelative">Whether the start column coordinate is relative or absolute.</param>
        /// <param name="endRowRelative">Whether the end row coordinate is relative or absolute.</param>
        /// <param name="endColumnRelative">Whether end column coordinate is relative or absolute.</param>
        public CalcSheetRangeExpression(ICalcSource startSource, ICalcSource endSource, int startRow, int startColumn, int endRow, int endColumn, bool startRowRelative, bool startColumnRelative, bool endRowRelative, bool endColumnRelative)
        {
            this._startSource = startSource;
            this._endSource = endSource;
            if (((startRowRelative == endRowRelative) && (startRow > endRow)) && (endRow != -2147483648))
            {
                this._startRow = endRow;
                this._endRow = startRow;
                this._startRowRelative = endRowRelative;
                this._endRowRelative = startRowRelative;
            }
            else
            {
                this._startRow = startRow;
                this._endRow = endRow;
                this._startRowRelative = startRowRelative;
                this._endColumnRelative = endColumnRelative;
            }
            if (((startColumnRelative == endColumnRelative) && (startColumn > endColumn)) && (endColumn != -2147483648))
            {
                this._startColumn = endColumn;
                this._endColumn = startColumn;
                this._startColumnRelative = endColumnRelative;
                this._endColumnRelative = startColumnRelative;
            }
            else
            {
                this._startColumn = startColumn;
                this._endColumn = endColumn;
                this._startColumnRelative = startColumnRelative;
                this._endRowRelative = endRowRelative;
            }
        }

        /// <summary>
        /// Gets the identity of current expressions based on <paramref name="row" /> and <paramref name="column" />.
        /// </summary>
        /// <param name="row">The base row.</param>
        /// <param name="column">The base column.</param>
        /// <returns></returns>
        public override CalcIdentity GetId(int row, int column)
        {
            switch (this.RangeType)
            {
                case CalcRangeType.Row:
                {
                    int num = this.StartRowRelative ? CalcHelper.NormalizeRowIndex(this.StartRow + row, 0xfffff) : this.StartRow;
                    int num2 = this.EndRowRelative ? CalcHelper.NormalizeRowIndex(this.EndRow + row, 0xfffff) : this.EndRow;
                    base.Sort(ref num, ref num2);
                    return new CalcSheetRangeIdentity(this.StartSource, this.EndSource, num, CalcHelper.NormalizeRowIndex((num2 - num) + 1, 0xfffff), true);
                }
                case CalcRangeType.Column:
                {
                    int num3 = this.StartColumnRelative ? CalcHelper.NormalizeRowIndex(this.StartColumn + column, 0xfffff) : this.StartColumn;
                    int num4 = this.EndColumnRelative ? CalcHelper.NormalizeRowIndex(this.EndColumn + column, 0xfffff) : this.EndColumn;
                    base.Sort(ref num3, ref num4);
                    return new CalcSheetRangeIdentity(this.StartSource, this.EndSource, num3, CalcHelper.NormalizeRowIndex((num4 - num3) + 1, 0xfffff), false);
                }
                case CalcRangeType.Sheet:
                    return new CalcSheetRangeIdentity(this.StartSource, this.EndSource);
            }
            bool flag = (this.EndRow == -2147483648) || (this.EndColumn == -2147483648);
            int num5 = this.StartRowRelative ? CalcHelper.NormalizeRowIndex(this.StartRow + row, 0xfffff) : this.StartRow;
            int num6 = this.EndRowRelative ? CalcHelper.NormalizeRowIndex(this.EndRow + row, 0xfffff) : this.EndRow;
            int num7 = this.StartColumnRelative ? CalcHelper.NormalizeRowIndex(this.StartColumn + column, 0xfffff) : this.StartColumn;
            int num8 = this.EndColumnRelative ? CalcHelper.NormalizeRowIndex(this.EndColumn + column, 0xfffff) : this.EndColumn;
            base.Sort(ref num5, ref num6);
            base.Sort(ref num7, ref num8);
            int rowCount = flag ? 1 : CalcHelper.NormalizeRowIndex((num6 - num5) + 1, 0xfffff);
            return new CalcSheetRangeIdentity(this.StartSource, this.EndSource, num5, num7, rowCount, flag ? 1 : CalcHelper.NormalizeRowIndex((num8 - num7) + 1, 0xfffff));
        }

        /// <summary>
        /// Get a new expression with specific offset.
        /// </summary>
        /// <param name="row">the row offset</param>
        /// <param name="column">the column offset</param>
        /// <param name="offsetAbsolute"><c>true</c> if offset the absolute indexes.</param>
        /// <param name="offsetRelative"><c>true</c> if offset the relative indexes.</param>
        /// <returns>
        /// Return a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> which offset from current expression.
        /// </returns>
        public override CalcExpression Offset(int row, int column, bool offsetAbsolute = false, bool offsetRelative = true)
        {
            return this.Offset(row, column, row, column, offsetAbsolute, offsetRelative);
        }

        /// <summary>
        /// Get a new expression with specific offset.
        /// </summary>
        /// <param name="row">the row offset</param>
        /// <param name="column">the column offset</param>
        /// <param name="endRow">the end row offset</param>
        /// <param name="endColumn">the end column offset</param>
        /// <param name="offsetAbsolute"><c>true</c> if offset the absolute indexes.</param>
        /// <param name="offsetRelative"><c>true</c> if offset the relative indexes.</param>
        /// <returns>
        /// Return a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> which offset from current expression.
        /// </returns>
        public CalcSheetRangeExpression Offset(int row, int column, int endRow, int endColumn, bool offsetAbsolute = false, bool offsetRelative = true)
        {
            bool flag = (this.StartRowRelative && offsetRelative) || (!this.StartRowRelative && offsetAbsolute);
            bool flag2 = (this.StartColumnRelative && offsetRelative) || (!this.StartColumnRelative && offsetAbsolute);
            bool flag3 = (this.EndRowRelative && offsetRelative) || (!this.EndRowRelative && offsetAbsolute);
            bool flag4 = (this.EndColumnRelative && offsetRelative) || (!this.EndColumnRelative && offsetAbsolute);
            switch (this.RangeType)
            {
                case CalcRangeType.Cell:
                    return new CalcSheetRangeExpression(this.StartSource, this.EndSource, this.StartRow + (flag ? row : 0), this.StartColumn + (flag2 ? column : 0), this.EndRow + (flag3 ? endRow : 0), this.EndColumn + (flag4 ? endColumn : 0), this.StartRowRelative, this.StartColumnRelative, this.EndRowRelative, this.EndColumnRelative);

                case CalcRangeType.Row:
                    return new CalcSheetRangeExpression(this.StartSource, this.EndSource, this.StartRow + (flag ? row : 0), this.EndRow + (flag3 ? endRow : 0), this.StartRowRelative, this.EndRowRelative, true);

                case CalcRangeType.Column:
                    return new CalcSheetRangeExpression(this.StartSource, this.EndSource, this.StartColumn + (flag2 ? column : 0), this.EndColumn + (flag4 ? endColumn : 0), this.StartColumnRelative, this.EndColumnRelative, false);

                case CalcRangeType.Sheet:
                    return new CalcSheetRangeExpression(this.StartSource, this.EndSource);
            }
            return new CalcSheetRangeExpression(this.StartSource, this.EndSource, this.StartRow + (flag ? row : 0), this.StartColumn + (flag2 ? column : 0), this.EndRow + (flag3 ? endRow : 0), this.EndColumn + (flag4 ? endColumn : 0), this.StartRowRelative, this.StartColumnRelative, this.EndRowRelative, this.EndColumnRelative);
        }

        /// <summary>
        /// Get a new expression with specific Sources.
        /// </summary>
        /// <param name="newStartSource">the new start source.</param>
        /// <param name="newEndSource">the new end source.</param>
        /// <returns>
        /// Return a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> which start source and end source are different from current expression.
        /// </returns>
        public CalcExpression ResetSheetRanges(ICalcSource newStartSource, ICalcSource newEndSource)
        {
            if (newStartSource == newEndSource)
            {
                switch (this.RangeType)
                {
                    case CalcRangeType.Cell:
                        return new CalcExternalRangeExpression(newStartSource, this.StartRow, this.StartColumn, this.EndRow, this.EndColumn, this.StartRowRelative, this.StartColumnRelative, this.EndRowRelative, this.EndColumnRelative);

                    case CalcRangeType.Row:
                        return new CalcExternalRangeExpression(newStartSource, this.StartRow, this.EndRow, this.StartRowRelative, this.EndRowRelative, true);

                    case CalcRangeType.Column:
                        return new CalcExternalRangeExpression(newStartSource, this.StartColumn, this.EndColumn, this.StartColumnRelative, this.EndColumnRelative, false);

                    case CalcRangeType.Sheet:
                        return new CalcExternalRangeExpression(newStartSource);
                }
                return new CalcExternalRangeExpression(newStartSource, this.StartRow, this.StartColumn, this.EndRow, this.EndColumn, this.StartRowRelative, this.StartColumnRelative, this.EndRowRelative, this.EndColumnRelative);
            }
            switch (this.RangeType)
            {
                case CalcRangeType.Cell:
                    return new CalcSheetRangeExpression(newStartSource, newEndSource, this.StartRow, this.StartColumn, this.EndRow, this.EndColumn, this.StartRowRelative, this.StartColumnRelative, this.EndRowRelative, this.EndColumnRelative);

                case CalcRangeType.Row:
                    return new CalcSheetRangeExpression(newStartSource, newEndSource, this.StartRow, this.EndRow, this.StartRowRelative, this.EndRowRelative, true);

                case CalcRangeType.Column:
                    return new CalcSheetRangeExpression(newStartSource, newEndSource, this.StartColumn, this.EndColumn, this.StartColumnRelative, this.EndColumnRelative, false);

                case CalcRangeType.Sheet:
                    return new CalcSheetRangeExpression(newStartSource, newEndSource);
            }
            return new CalcSheetRangeExpression(newStartSource, newEndSource, this.StartRow, this.StartColumn, this.EndRow, this.EndColumn, this.StartRowRelative, this.StartColumnRelative, this.EndRowRelative, this.EndColumnRelative);
        }

        /// <summary>
        /// Gets the ending column index of the range.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that represents the ending column index of the range.
        /// </value>
        public int EndColumn
        {
            get
            {
                return this._endColumn;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the end column coordinate is relative.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the end column coordinate is are relative;
        /// otherwise, <see langword="false" />.
        /// </value>
        public bool EndColumnRelative
        {
            get
            {
                return this._endColumnRelative;
            }
        }

        /// <summary>
        /// Gets the ending row index of the range.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that represents the ending row index of the range.
        /// </value>
        public int EndRow
        {
            get
            {
                return this._endRow;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the end row coordinate is relative.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the end row coordinate is relative; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public bool EndRowRelative
        {
            get
            {
                return this._endRowRelative;
            }
        }

        /// <summary>
        /// Gets the end owner of current range.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.ICalcSource" /> value that represents the owner of current cell.
        /// </value>
        public ICalcSource EndSource
        {
            get
            {
                return this._endSource;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is full column.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is full column; otherwise, <see langword="false" />.
        /// </value>
        public bool IsFullColumn
        {
            get
            {
                return this._isFullColumn;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is full row.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is full row; otherwise, <see langword="false" />.
        /// </value>
        public bool IsFullRow
        {
            get
            {
                return this._isFullRow;
            }
        }

        internal CalcRangeType RangeType
        {
            get
            {
                if (this.IsFullRow && this.IsFullColumn)
                {
                    return CalcRangeType.Sheet;
                }
                if (this.IsFullRow)
                {
                    return CalcRangeType.Row;
                }
                if (this.IsFullColumn)
                {
                    return CalcRangeType.Column;
                }
                return CalcRangeType.Cell;
            }
        }

        /// <summary>
        /// Gets the starting column index of the range.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that represents the starting column index of the range.
        /// </value>
        public int StartColumn
        {
            get
            {
                return this._startColumn;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the start column coordinate is relative.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the start column coordinate is are relative;
        /// otherwise, <see langword="false" />.
        /// </value>
        public bool StartColumnRelative
        {
            get
            {
                return this._startColumnRelative;
            }
        }

        /// <summary>
        /// Gets the starting row index of the range.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that represents the starting row index of the range.
        /// </value>
        public int StartRow
        {
            get
            {
                return this._startRow;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the start row coordinate is relative.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the start row coordinate is relative; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public bool StartRowRelative
        {
            get
            {
                return this._startRowRelative;
            }
        }

        /// <summary>
        /// Gets the start owner of current range.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.ICalcSource" /> value that represents the owner of current cell.
        /// </value>
        public ICalcSource StartSource
        {
            get
            {
                return this._startSource;
            }
        }
    }
}

