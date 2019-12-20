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
using System.Collections.Generic;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the k-th largest value in a data set.
    /// </summary>
    /// <remarks>
    /// You can use this function to select a value based on its relative standing.
    /// </remarks>
    public class CalcLargeFunction : CalcBuiltinFunction
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
        /// Returns the k-th largest value in a data set.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array, k.
        /// </para>
        /// <para>
        /// Array is the array or range of data for which you want to
        /// determine the k-th largest value.
        /// </para>
        /// <para>
        /// K is the position (from the largest) in the array or cell
        /// range of data to return.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            if (args[0] is CalcError)
            {
                return args[0];
            }
            if (args[1] is CalcError)
            {
                return args[1];
            }
            int num = CalcConvert.ToInt(args[1]);
            List<double> list = new List<double>();
            if (ArrayHelper.IsArrayOrRange(args[0]))
            {
                for (int i = 0; i < ArrayHelper.GetLength(args[0], 0); i++)
                {
                    object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                    if (CalcConvert.IsNumber(obj2))
                    {
                        double item = CalcConvert.ToDouble(obj2);
                        list.Add(item);
                    }
                    else if (obj2 is CalcError)
                    {
                        return obj2;
                    }
                }
            }
            else
            {
                double num4;
                if (!CalcConvert.TryToDouble(args[0], out num4, true))
                {
                    return CalcErrors.Value;
                }
                list.Add(num4);
            }
            list.Sort();
            if ((num > 0) && (list.Count >= num))
            {
                return (double) list[list.Count - num];
            }
            return CalcErrors.Number;
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
                return 2;
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
                return "LARGE";
            }
        }
    }
}

