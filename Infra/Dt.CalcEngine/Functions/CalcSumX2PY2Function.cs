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
    /// Returns the sum of the sum of squares of corresponding values in two arrays.
    /// </summary>
    /// <remarks>
    /// The sum of the sum of squares is a common term in many statistical calculations.
    /// </remarks>
    public class CalcSumX2PY2Function : CalcBuiltinFunction
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
            return true;
        }

        /// <summary>
        /// Returns the sum of the sum of squares of corresponding values in two arrays.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array_x, array_y.
        /// </para>
        /// <para>
        /// Array_x is the first array or range of values.
        /// </para>
        /// <para>
        /// Array_y is the second array or range of values.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num2;
            double num3;
            base.CheckArgumentsLength(args);
            double num = 0.0;
            if (ArrayHelper.GetLength(args[0], 0) != ArrayHelper.GetLength(args[1], 0))
            {
                return CalcErrors.NotAvailable;
            }
            if (args[0] is CalcError)
            {
                return args[0];
            }
            if (args[1] is CalcError)
            {
                return args[1];
            }
            if (ArrayHelper.IsArrayOrRange(args[0]))
            {
                for (int i = 0; i < ArrayHelper.GetLength(args[0], 0); i++)
                {
                    object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                    object obj3 = ArrayHelper.GetValue(args[1], i, 0);
                    if (CalcConvert.IsNumber(obj2) && CalcConvert.IsNumber(obj3))
                    {
                        num2 = CalcConvert.ToDouble(obj2);
                        num3 = CalcConvert.ToDouble(obj3);
                        num += (num2 * num2) + (num3 * num3);
                    }
                    else
                    {
                        if (obj2 is CalcError)
                        {
                            return obj2;
                        }
                        if (obj3 is CalcError)
                        {
                            return obj3;
                        }
                    }
                }
            }
            else
            {
                if (!CalcConvert.TryToDouble(args[0], out num2, true) || !CalcConvert.TryToDouble(args[1], out num3, true))
                {
                    return CalcErrors.Value;
                }
                num += (num2 * num2) + (num3 * num3);
            }
            return CalcConvert.ToResult(num);
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
                return "SUMX2PY2";
            }
        }
    }
}

