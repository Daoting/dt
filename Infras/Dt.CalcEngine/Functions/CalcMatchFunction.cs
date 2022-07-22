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
    /// Returns the relative position of an item in an array that matches 
    /// a specified value in a specified order.
    /// </summary>
    public class CalcMatchFunction : CalcBuiltinFunction
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
            return (i == 2);
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

        private static bool AssertOrder(object v, bool ascending)
        {
            int length = ArrayHelper.GetLength(v, 0);
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    object a = ArrayHelper.GetValue(v, i - 1, 0);
                    object b = ArrayHelper.GetValue(v, i, 0);
                    if ((LookupHelper.value_compare(a, b, false) == 2) && !ascending)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the relative position of an item in an array that matches
        /// a specified value in a specified order.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: lookup_value, lookup_array, [match_type].
        /// </para>
        /// <para>
        /// Lookup_value is the value you use to find the value you want in a table.
        /// </para>
        /// <para>
        /// Lookup_array is a contiguous range of cells containing possible lookup
        /// values. Lookup_array must be an array or an array reference.
        /// </para>
        /// <para>
        /// [Match_type] is the number -1, 0, or 1. Match_type specifies how to
        /// matches lookup_value with values in lookup_array.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int columnCount = ArrayHelper.GetColumnCount(args[1], 0);
            int rowCount = ArrayHelper.GetRowCount(args[1], 0);
            int num3 = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToInt(args[2]) : 1;
            if ((columnCount > 1) && (rowCount > 1))
            {
                return CalcErrors.NotAvailable;
            }
            if ((num3 == 1) && !AssertOrder(args[1], true))
            {
                return CalcErrors.NotAvailable;
            }
            if ((num3 == -1) && !AssertOrder(args[1], false))
            {
                return CalcErrors.NotAvailable;
            }
            int num4 = -1;
            switch (num3)
            {
                case 1:
                    num4 = LookupHelper.find_index_bisection(args[0], args[1], 1, rowCount > 1);
                    break;

                case 0:
                    num4 = LookupHelper.find_index_linear(args[0], args[1], 0, rowCount > 1);
                    break;

                case -1:
                    num4 = LookupHelper.find_index_bisection(args[0], args[1], -1, rowCount > 1);
                    break;
            }
            if (num4 == -1)
            {
                return CalcErrors.NotAvailable;
            }
            return (int) (num4 + 1);
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
                return 3;
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
                return "MATCH";
            }
        }
    }
}

