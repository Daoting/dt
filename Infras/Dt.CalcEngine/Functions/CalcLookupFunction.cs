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
    /// Returns a value either from a one-row or one-column range or from an array.
    /// </summary>
    public class CalcLookupFunction : CalcBuiltinFunction
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
            return (i != 0);
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
            return (i != 0);
        }

        /// <summary>
        /// Returns a value either from a one-row or one-column range or from an array.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: lookup_value, lookup_vector,
        /// [result_vector].
        /// </para>
        /// <para>
        /// Lookup_value is A value that LOOKUP searches for in the first vector.
        /// Lookup_value can be a number, text, a logical value, or a name or reference
        /// that refers to a value.
        /// </para>
        /// <para>
        /// Lookup_vector A range that contains only one row or one column. The values
        /// in lookup_vector can be text, numbers, or logical values.
        /// </para>
        /// <para>
        /// Result_vector A range that contains only one row or column. It must be the
        /// same size as lookup_vector.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            object obj2;
            base.CheckArgumentsLength(args);
            int x = -1;
            int columnCount = ArrayHelper.GetColumnCount(args[1], 0);
            int rowCount = ArrayHelper.GetRowCount(args[1], 0);
            if (args.Length > 2)
            {
                obj2 = args[2];
            }
            else
            {
                if (columnCount > rowCount)
                {
                    object obj3 = new CalcHLookupFunction().Evaluate(new object[] { args[0], args[1], (int) rowCount });
                    if (obj3 is CalcError)
                    {
                        return CalcErrors.NotAvailable;
                    }
                    return obj3;
                }
                object obj4 = new CalcVLookupFunction().Evaluate(new object[] { args[0], args[1], (int) columnCount });
                if (obj4 is CalcError)
                {
                    return CalcErrors.NotAvailable;
                }
                return obj4;
            }
            if (obj2 != null)
            {
                int num4 = ArrayHelper.GetColumnCount(obj2, 0);
                int num5 = ArrayHelper.GetRowCount(obj2, 0);
                if ((num4 > 1) && (num5 > 1))
                {
                    return CalcErrors.NotAvailable;
                }
            }
            else
            {
                obj2 = args[1];
            }
            x = LookupHelper.find_index_bisection(args[0], args[1], 1, columnCount <= rowCount);
            if (x < 0)
            {
                return CalcErrors.NotAvailable;
            }
            columnCount = ArrayHelper.GetColumnCount(obj2, 0);
            rowCount = ArrayHelper.GetRowCount(obj2, 0);
            if (columnCount > rowCount)
            {
                return LookupHelper.value_area_fetch_x_y(obj2, x, rowCount - 1);
            }
            return LookupHelper.value_area_fetch_x_y(obj2, columnCount - 1, x);
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
                return "LOOKUP";
            }
        }
    }
}

