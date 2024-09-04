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
using System.Globalization;
using System.Text;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Rounds a number to the specified number of decimals, 
    /// formats the number in decimal format using a period and commas, 
    /// and returns the result as <see cref="T:System.String" />.
    /// </summary>
    public class CalcFixedFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            if (i != 1)
            {
                return (i == 2);
            }
            return true;
        }

        /// <summary>
        /// Rounds a number to the specified number of decimals,
        /// formats the number in decimal format using a period and commas,
        /// and returns the result as <see cref="T:System.String" />.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 3 items: number, [decimals], [no_commas].
        /// </para>
        /// <para>
        /// Number is the number you want to round and convert to text.
        /// </para>
        /// <para>
        /// [Decimals] is the number of digits to the right of the decimal point.
        /// </para>
        /// <para>
        /// [No_commas] is a logical value that, if <see langword="true" />,
        /// prevents FIXED from including commas in the returned text.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            int num2 = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToInt(args[1]) : 2;
            bool flag = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToBool(args[2]) : false;
            CalcRoundFunction function = new CalcRoundFunction();
            object[] objArray = new object[2];
            int num3 = 0;
            if (num2 < 0)
            {
                num3 = (int) Math.Pow(10.0, (double) Math.Abs(num2));
                num /= (double) num3;
                objArray[0] = (double) num;
                objArray[1] = 0;
            }
            else
            {
                objArray[0] = (double) num;
                objArray[1] = (int) num2;
            }
            num = (double) ((double) function.Evaluate(objArray));
            if (num2 < 0)
            {
                num *= num3;
            }
            if (flag)
            {
                StringBuilder builder = new StringBuilder("F");
                if (num2 <= 0)
                {
                    builder.Append(0);
                }
                else
                {
                    builder.Append(num2);
                }
                return ((double) num).ToString(builder.ToString(), CultureInfo.CurrentCulture);
            }
            StringBuilder builder2 = new StringBuilder("N");
            if (num2 <= 0)
            {
                builder2.Append(0);
            }
            else
            {
                builder2.Append(num2);
            }
            return ((double) num).ToString(builder2.ToString(), CultureInfo.CurrentCulture);
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
                return "FIXED";
            }
        }
    }
}

