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
    /// Searches for a value in the first column of a table array and
    /// returns a value in the same row from another column in the table array.
    /// </summary>
    /// <remarks>
    /// The V in VLOOKUP stands for vertical. Use VLOOKUP instead of HLOOKUP
    /// when your comparison values are located in a column to the left of the
    /// data that you want to find.
    /// </remarks>
    public class CalcVLookupFunction : CalcBuiltinFunction
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
            return (i == 1);
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
            return (i == 3);
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
            return (i == 1);
        }

        /// <summary>
        /// Searches for a value in the first column of a table array and
        /// returns a value in the same row from another column in the table array.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 4 items: lookup_value, table_array,
        /// col_index_num, [range_lookup].
        /// </para>
        /// <para>
        /// Lookup_value is the value to be found in the first row of the table.
        /// Lookup_value can be a value, a reference, or a text string.
        /// </para>
        /// <para>
        /// Table_array is a table of information in which data is looked up.
        /// Use a reference to a range or a range name.
        /// </para>
        /// <para>
        /// Col_index_num  The column number in table_array from which the
        /// matching value must be returned.
        /// </para>
        /// <para>
        /// [Range_lookup] is a logical value that specifies whether you want
        /// HLOOKUP to find an exact match or an approximate match.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object obj1 = args[0];
            object o = args[1];
            int num = CalcConvert.ToInt(args[2]);
            bool flag = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToBool(args[3]) : true;
            int y = -1;
            if (num <= 0)
            {
                return CalcErrors.Value;
            }
            if (num > ArrayHelper.GetColumnCount(o, 0))
            {
                return CalcErrors.Reference;
            }
            y = flag ? LookupHelper.find_index_bisection(args[0], args[1], 1, true) : LookupHelper.find_index_linear(args[0], args[1], 0, true);
            if (y >= 0)
            {
                return LookupHelper.value_area_fetch_x_y(args[1], num - 1, y);
            }
            return CalcErrors.NotAvailable;
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
                return "VLOOKUP";
            }
        }
    }
}

