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
    /// Multiplies corresponding components in the given arrays,
    /// and returns the sum of those products.
    /// </summary>
    public class CalcSumProductFunction : CalcBuiltinFunction
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
        /// Multiplies corresponding components in the given arrays,
        /// and returns the sum of those products.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 255 items: array1, [array2], [array3], ..
        /// </para>
        /// <para>
        /// Array1, array2, array3, ... are 2 to 255 arrays whose components
        /// you want to multiply and then add.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = 0.0;
            int rowCount = ArrayHelper.GetRowCount(args[0], 0);
            int columnCount = ArrayHelper.GetColumnCount(args[0], 0);
            for (int i = 1; i < args.Length; i++)
            {
                if (rowCount != ArrayHelper.GetRowCount(args[i], 0))
                {
                    return CalcErrors.Value;
                }
                if (columnCount != ArrayHelper.GetColumnCount(args[i], 0))
                {
                    return CalcErrors.Value;
                }
            }
            for (int j = 0; j < rowCount; j++)
            {
                for (int k = 0; k < columnCount; k++)
                {
                    double num7 = 1.0;
                    for (int m = 0; m < args.Length; m++)
                    {
                        object obj2 = ArrayHelper.GetValue(args[m], j, k, 0);
                        if (obj2 is CalcError)
                        {
                            return obj2;
                        }
                        if (CalcConvert.IsNumber(obj2))
                        {
                            num7 *= CalcConvert.ToDouble(obj2);
                        }
                        else
                        {
                            num7 = 0.0;
                        }
                    }
                    num += num7;
                }
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
                return 0xff;
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
                return 1;
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
                return "SUMPRODUCT";
            }
        }
    }
}

