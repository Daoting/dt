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
    /// Returns the <see cref="T:System.Double" /> slope of the linear regression.
    /// </summary>
    public class CalcSlopeFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an array arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
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
        /// Returns the <see cref="T:System.Double" /> slope of the linear regression.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: known_y's, known_x's.
        /// </para>
        /// <para>
        /// Known_y's is an array or cell range of numeric dependent data points.
        /// </para>
        /// <para>
        /// Known_x's is the set of independent data points.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object o = args[0];
            object obj3 = args[1];
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            int num5 = 0;
            int length = ArrayHelper.GetLength(args[0], 0);
            if (length != ArrayHelper.GetLength(args[1], 0))
            {
                return CalcErrors.NotAvailable;
            }
            for (int i = 0; i < length; i++)
            {
                object obj4 = ArrayHelper.GetValue(o, i, 0);
                object obj5 = ArrayHelper.GetValue(obj3, i, 0);
                if (CalcConvert.IsNumber(obj4) && CalcConvert.IsNumber(obj5))
                {
                    double num8 = CalcConvert.ToDouble(obj4);
                    double num9 = CalcConvert.ToDouble(obj5);
                    num += num8;
                    num2 += num9;
                    num3 += num9 * num9;
                    num4 += num9 * num8;
                    num5++;
                }
            }
            if (((num5 * num3) - (num2 * num2)) == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult(((num5 * num4) - (num2 * num)) / ((num5 * num3) - (num2 * num2)));
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
                return "SLOPE";
            }
        }
    }
}

