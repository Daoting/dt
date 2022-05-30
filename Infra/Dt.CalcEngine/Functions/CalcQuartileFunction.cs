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
    /// Returns the quartile of a data set. Quartiles often are used in 
    /// sales and survey data to divide populations into groups.
    /// </summary>
    public class CalcQuartileFunction : CalcBuiltinFunction
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
        /// Returns the quartile of a data set. Quartiles often are used in
        /// sales and survey data to divide populations into groups.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array, quart.
        /// </para>
        /// <para>
        /// Array is the array or cell range of numeric values for which you
        /// want the quartile value.
        /// </para>
        /// <para>
        /// Quart indicates which value to return.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object obj2 = args[0];
            int num = CalcConvert.ToInt(args[1]);
            double num2 = 0.0;
            switch (num)
            {
                case 0:
                    num2 = 0.0;
                    break;

                case 1:
                    num2 = 0.25;
                    break;

                case 2:
                    num2 = 0.5;
                    break;

                case 3:
                    num2 = 0.75;
                    break;

                case 4:
                    num2 = 1.0;
                    break;

                default:
                    return CalcErrors.Number;
            }
            return new CalcPercentileFunction().Evaluate(new object[] { obj2, (double) num2 });
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
                return "QUARTILE";
            }
        }
    }
}

