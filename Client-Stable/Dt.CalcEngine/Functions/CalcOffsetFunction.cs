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
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns a reference to a range that is a specified number of 
    /// rows and columns from a cell or range of cells.
    /// </summary>
    /// <remarks>
    /// The reference that is returned can be a single cell or a range 
    /// of cells. You can specify the number of rows and the number of 
    /// columns to be returned.
    /// </remarks>
    public class CalcOffsetFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Determines whether the function accepts array values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts array values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsArray(int i)
        {
            return (i == 0);
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            if (i != 3)
            {
                return (i == 4);
            }
            return true;
        }

        /// <summary>
        /// Determines whether the function accepts CalcReference values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts CalcReference values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsReference(int i)
        {
            return (i == 0);
        }

        /// <summary>
        /// Returns a reference to a range that is a specified number of
        /// rows and columns from a cell or range of cells.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 5 items: reference, rows, cols, [height], [width].
        /// </para>
        /// <para>
        /// Reference is the reference from which you want to base the offset.
        /// </para>
        /// <para>
        /// Rows is the number of rows, up or down, that you want the upper-left
        /// cell to refer to.
        /// </para>
        /// <para>
        /// Cols is the number of columns, to the left or right, that you want
        /// the upper-left cell of the result to refer to.
        /// </para>
        /// <para>
        /// [Height] is the height, in number of rows, that you want the returned
        /// reference to be. Height must be a positive number.
        /// </para>
        /// <para>
        /// [Width] is the width, in number of columns, that you want the returned
        /// reference to be. Width must be a positive number.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:Dt.CalcEngine.CalcReference" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            CalcReference reference = args[0] as CalcReference;
            if ((reference == null) || (reference.RangeCount != 1))
            {
                return CalcErrors.Value;
            }
            int num = CalcConvert.ToInt(args[1]);
            int num2 = CalcConvert.ToInt(args[2]);
            int rowCount = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToInt(args[3]) : reference.GetRowCount(0);
            int columnCount = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToInt(args[4]) : reference.GetColumnCount(0);
            CalcReference source = reference.GetSource();
            int row = reference.GetRow(0) + num;
            int column = reference.GetColumn(0) + num2;
            if ((rowCount > 0) && (columnCount > 0))
            {
                if ((row < source.GetRow(0)) || ((source.GetRow(0) + source.GetRowCount(0)) < (row + rowCount)))
                {
                    return CalcErrors.Reference;
                }
                if ((column >= source.GetColumn(0)) && ((source.GetColumn(0) + source.GetColumnCount(0)) >= (column + columnCount)))
                {
                    return new ConcreteReference(source, row, column, rowCount, columnCount);
                }
            }
            return CalcErrors.Reference;
        }

        /// <summary>
        /// Determines whether the function is volatile while evaluate.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the function is volatile; 
        /// <see langword="false" /> otherwise.
        /// </returns>
        public override bool IsVolatile()
        {
            return true;
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The maximum number of arguments for the function.
        /// </value>
        public override int MaxArgs
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// Gets the minimum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The minimum number of arguments for the function.
        /// </value>
        public override int MinArgs
        {
            get
            {
                return 3;
            }
        }

        /// <summary>
        /// Gets The name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "OFFSET";
            }
        }
    }
}

