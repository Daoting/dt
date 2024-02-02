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
    /// Represents a cell range reference on current sheet expression.
    /// </summary>
    public class CalcBangRangeExpression : CalcRangeExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcRangeExpression" /> class.
        /// </summary>
        public CalcBangRangeExpression()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcRangeExpression" /> class.
        /// </summary>
        /// <param name="startRow">Starting row coordinate of the range.</param>
        /// <param name="startColumn">Starting column coordinate of the range.</param>
        /// <param name="endRow">Ending row coordinate of the range.</param>
        /// <param name="endColumn">Ending column coordinate of the range.</param>
        public CalcBangRangeExpression(int startRow, int startColumn, int endRow, int endColumn) : base(startRow, startColumn, endRow, endColumn)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcBangRangeExpression" /> class.
        /// </summary>
        /// <param name="startBandIndex">Starting row or column coordinate of the range.</param>
        /// <param name="endBandIndex">Ending row or column coordinate of the range.</param>
        /// <param name="startBandRelative">Whether the start row or column coordinate is relative or absolute.</param>
        /// <param name="endBandRelative">Whether the end row or column coordinate is relative or absolute.</param>
        /// <param name="isRowBand"><see langword="true" /> if current range is a full row range, otherwise, 
        /// current range is a full column range.</param>
        public CalcBangRangeExpression(int startBandIndex, int endBandIndex, bool startBandRelative, bool endBandRelative, bool isRowBand) : base(startBandIndex, endBandIndex, startBandRelative, endBandRelative, isRowBand)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcRangeExpression" /> class.
        /// </summary>
        /// <param name="startRow">Starting row coordinate of the range.</param>
        /// <param name="startColumn">Starting column coordinate of the range.</param>
        /// <param name="endRow">Ending row coordinate of the range.</param>
        /// <param name="endColumn">Ending column coordinate of the range.</param>
        /// <param name="startRowRelative">Whether the start row coordinate is relative or absolute.</param>
        /// <param name="startColumnRelative">Whether the start column coordinate is relative or absolute.</param>
        /// <param name="endRowRelative">Whether the end row coordinate is relative or absolute.</param>
        /// <param name="endColumnRelative">Whether end column coordinate is relative or absolute.</param>
        public CalcBangRangeExpression(int startRow, int startColumn, int endRow, int endColumn, bool startRowRelative, bool startColumnRelative, bool endRowRelative, bool endColumnRelative) : base(startRow, startColumn, endRow, endColumn, startRowRelative, startColumnRelative, endRowRelative, endColumnRelative)
        {
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
        public override CalcRangeExpression Offset(int row, int column, int endRow, int endColumn, bool offsetAbsolute = false, bool offsetRelative = true)
        {
            bool flag = (base.StartRowRelative && offsetRelative) || (!base.StartRowRelative && offsetAbsolute);
            bool flag2 = (base.StartColumnRelative && offsetRelative) || (!base.StartColumnRelative && offsetAbsolute);
            bool flag3 = (base.EndRowRelative && offsetRelative) || (!base.EndRowRelative && offsetAbsolute);
            bool flag4 = (base.EndColumnRelative && offsetRelative) || (!base.EndColumnRelative && offsetAbsolute);
            switch (this.RangeType)
            {
                case CalcRangeType.Cell:
                    return new CalcBangRangeExpression(base.StartRow + (flag ? row : 0), base.StartColumn + (flag2 ? column : 0), base.EndRow + (flag3 ? endRow : 0), base.EndColumn + (flag4 ? endColumn : 0), base.StartRowRelative, base.StartColumnRelative, base.EndRowRelative, base.EndColumnRelative);

                case CalcRangeType.Row:
                    return new CalcBangRangeExpression(base.StartRow + (flag ? row : 0), base.EndRow + (flag3 ? endRow : 0), base.StartRowRelative, base.EndRowRelative, true);

                case CalcRangeType.Column:
                    return new CalcBangRangeExpression(base.StartColumn + (flag2 ? column : 0), base.EndColumn + (flag4 ? endColumn : 0), base.StartColumnRelative, base.EndColumnRelative, false);

                case CalcRangeType.Sheet:
                    return new CalcBangRangeExpression();
            }
            return new CalcBangRangeExpression(base.StartRow + (flag ? row : 0), base.StartColumn + (flag2 ? column : 0), base.EndRow + (flag3 ? endRow : 0), base.EndColumn + (flag4 ? endColumn : 0), base.StartRowRelative, base.StartColumnRelative, base.EndRowRelative, base.EndColumnRelative);
        }
    }
}

