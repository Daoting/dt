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
    /// Returns a value or the reference to a value from within a table or range.
    /// </summary>
    public class CalcIndexFunction : CalcBuiltinFunction
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
            if ((i != 1) && (i != 2))
            {
                return (i == 3);
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
        /// Returns a value or the reference to a value from within a table or range.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 4 items: array, row_num, [column_num], [Area_num].
        /// </para>
        /// <para>
        /// Array is a range of cells or an array constant.
        /// </para>
        /// <para>
        /// Row_num selects the row in array from which to return a value.
        /// If row_num is omitted, column_num is required.
        /// </para>
        /// <para>
        /// Row_num selects the row in array from which to return a value. If
        /// row_num is omitted, column_num is required.
        /// </para>
        /// <para>
        /// [Column_num] selects the column in array from which to return a value.
        /// If column_num is omitted, row_num is required.
        /// </para>
        /// [Area_num] selects a range in reference from which to return the intersection 
        /// of row_num and column_num.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object o = args[0];
            int rowCount = ArrayHelper.GetRowCount(o, 0);
            int columnCount = ArrayHelper.GetColumnCount(o, 0);
            if (args.Length == 2)
            {
                int num3 = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToInt(args[1]) : 0;
                if ((rowCount != 1) && (columnCount != 1))
                {
                    return CalcErrors.Reference;
                }
                if (num3 < 0)
                {
                    return CalcErrors.Value;
                }
                if ((rowCount * columnCount) < num3)
                {
                    return CalcErrors.Reference;
                }
                if (num3 == 0)
                {
                    return new SliceArray(o, 0, 0, rowCount, columnCount);
                }
                return ArrayHelper.GetValue(o, num3 - 1, 0);
            }
            int num4 = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToInt(args[1]) : 0;
            int num5 = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToInt(args[2]) : 0;
            int num6 = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToInt(args[3]) : 1;
            if (((num4 < 0) || (num5 < 0)) || (num6 < 1))
            {
                return CalcErrors.Value;
            }
            if (((rowCount < num4) || (columnCount < num5)) || (1 < num6))
            {
                return CalcErrors.Reference;
            }
            if ((num4 == 0) && (num5 == 0))
            {
                return new SliceArray(o, 0, 0, rowCount, columnCount);
            }
            if (num4 == 0)
            {
                return new SliceArray(o, 0, num5 - 1, rowCount, 1);
            }
            if (num5 == 0)
            {
                return new SliceArray(o, num4 - 1, 0, 1, columnCount);
            }
            return ArrayHelper.GetValue(o, num4 - 1, num5 - 1, 0);
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
                return 4;
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
                return 2;
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
                return "INDEX";
            }
        }
    }
}

