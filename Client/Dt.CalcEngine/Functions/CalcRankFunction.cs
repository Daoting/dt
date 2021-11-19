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
    /// Returns the rank of a number in a list of numbers.
    /// </summary>
    /// <remarks>
    /// The rank of a number is its size relative to other values in a list.
    /// </remarks>
    public class CalcRankFunction : CalcBuiltinFunction
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

        /// <summary>
        /// Returns the rank of a number in a list of numbers.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: number, ref, [order].
        /// </para>
        /// <para>
        /// Number is the number whose rank you want to find.
        /// </para>
        /// <para>
        /// Ref is an array of, or a reference to, a list of numbers.
        /// Nonnumeric values in ref are ignored.
        /// </para>
        /// <para>
        /// [Order] is a number specifying how to rank number.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            object o = args[1];
            double number = 0.0;
            if (CalcHelper.ArgumentExists(args, 2) && !CalcConvert.TryToDouble(args[2], out number, true))
            {
                return CalcErrors.Value;
            }
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            for (int i = 0; i < ArrayHelper.GetLength(o, 0); i++)
            {
                object obj3 = ArrayHelper.GetValue(args[1], i, 0);
                if (CalcConvert.IsNumber(obj3))
                {
                    double num7 = CalcConvert.ToDouble(obj3);
                    if (num7 < num)
                    {
                        num3++;
                    }
                    else if (num < num7)
                    {
                        num5++;
                    }
                    else
                    {
                        num4++;
                    }
                }
            }
            if (num4 == 0)
            {
                return CalcErrors.NotAvailable;
            }
            return ((number == 0.0) ? ((int) (num5 + 1)) : ((int) (num3 + 1)));
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
                return "RANK";
            }
        }
    }
}

