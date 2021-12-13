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
    /// Returns the <see cref="T:System.Double" /> net present value of a series of cash flows (in and out).
    /// </summary>
    public class CalcNpvFunction : CalcBuiltinFunction
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
            return (i > 0);
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process references.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process references; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsReference(int i)
        {
            return (i > 0);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> net present value of a series of cash flows (in and out).
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 255 items: rate, [value1], [value2], ..
        /// </para>
        /// <para>
        /// Rate is the rate of discount over the length of one period.
        /// </para>
        /// <para>
        /// Value1, value2, ...   are 1 to 254 arguments representing the payments and income.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[0]);
            double num2 = 0.0;
            int num3 = 1;
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] is CalcError)
                {
                    return args[i];
                }
                if (ArrayHelper.IsArrayOrRange(args[i]))
                {
                    for (int j = 0; j < ArrayHelper.GetLength(args[i], 0); j++)
                    {
                        object obj2 = ArrayHelper.GetValue(args[i], j, 0);
                        if (obj2 is CalcError)
                        {
                            return obj2;
                        }
                        if (CalcConvert.IsNumber(obj2))
                        {
                            double num6 = CalcConvert.ToDouble(ArrayHelper.GetValue(args[i], j, 0));
                            num2 += num6 / Math.Pow(1.0 + num, (double) num3);
                            num3++;
                        }
                        else if (obj2 is CalcError)
                        {
                            return obj2;
                        }
                    }
                }
                else
                {
                    double num7 = CalcConvert.ToDouble(args[i]);
                    num2 += num7 / Math.Pow(1.0 + num, (double) num3);
                    num3++;
                }
            }
            return (double) num2;
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
                return "NPV";
            }
        }
    }
}

