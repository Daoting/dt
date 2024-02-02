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
    /// Returns the one-tailed probability-value of a z-test.
    /// </summary>
    /// <remarks>
    /// For a given hypothesized population mean, ¦Ì0, ZTEST returns the probability 
    /// that the sample mean would be greater than the average of observations in
    /// the data set (array) ¡ª that is, the observed sample mean.
    /// </remarks>
    public class CalcZTestFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an array arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsArray(int i)
        {
            return (i == 0);
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </returns>
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
            return (i == 0);
        }

        /// <summary>
        /// Returns the one-tailed probability-value of a z-test.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: array, ¦Ì0, [sigma].
        /// </para>
        /// <para>
        /// Array is the array or range of data against which to test ¦Ì0.
        /// </para>
        /// <para>
        /// ¦Ì0  is the value to test.
        /// </para>
        /// <para>
        /// [Sigma] is the population (known) standard deviation. If omitted,
        /// the sample standard deviation is used.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[1], out num, true))
            {
                return CalcErrors.Value;
            }
            double number = 0.0;
            if (CalcHelper.ArgumentExists(args, 2) && !CalcConvert.TryToDouble(args[2], out number, true))
            {
                return CalcErrors.Value;
            }
            double num3 = 0.0;
            double num4 = 0.0;
            int num7 = 0;
            if (ArrayHelper.IsArrayOrRange(args[0]))
            {
                for (int i = 0; i < ArrayHelper.GetLength(args[0], 0); i++)
                {
                    object obj2 = ArrayHelper.GetValue(args[0], i, 0);
                    if (CalcConvert.IsNumber(obj2))
                    {
                        double num9 = CalcConvert.ToDouble(obj2);
                        num3 += num9;
                        num4 += num9 * num9;
                        num7++;
                    }
                    else if (obj2 is CalcError)
                    {
                        return obj2;
                    }
                }
            }
            else
            {
                double num10;
                if (!CalcConvert.TryToDouble(args[0], out num10, true))
                {
                    return CalcErrors.Value;
                }
                num3 += num10;
                num4 += num10 * num10;
                num7++;
            }
            switch (num7)
            {
                case 0:
                    return CalcErrors.NotAvailable;

                case 1:
                    return CalcErrors.DivideByZero;
            }
            double num5 = num3 / ((double) num7);
            double num6 = CalcHelper.ArgumentExists(args, 2) ? number : Math.Sqrt(((num7 * num4) - (num3 * num3)) / ((double) (num7 * (num7 - 1))));
            if (num6 == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            CalcNormSDistFunction function = new CalcNormSDistFunction();
            object[] objArray = new object[] { (double) ((num5 - num) / (num6 / Math.Sqrt((double) num7))) };
            object obj3 = function.Evaluate(objArray);
            if (obj3 is CalcError)
            {
                return obj3;
            }
            return CalcConvert.ToResult(1.0 - ((double) obj3));
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
                return "ZTEST";
            }
        }
    }
}

