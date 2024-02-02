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
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// The function described in this Help topic converts a number to
    /// text format and applies a currency symbol. 
    /// The name of the function (and the symbol that it applies) depends
    /// upon your language settings.
    /// This function converts a number to text using currency format, 
    /// with the decimals rounded to the specified place. 
    /// The format used is $#,##0.00_);($#,##0.00).
    /// </summary>
    public class CalcDollarFunction : CalcBuiltinFunction
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
            return (i == 1);
        }

        /// <summary>
        /// The function described in this Help topic converts a number to
        /// text format and applies a currency symbol.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: number, [decimals].
        /// </para>
        /// <para>
        /// Number   is a number, a reference to a cell containing a number,
        /// or a formula that evaluates to a number.
        /// </para>
        /// <para>
        /// [Decimals] is the number of digits to the right of the decimal point.
        /// If decimals is negative, number is rounded to the left of
        /// the decimal point. If you omit decimals, it is assumed to be 2.
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
            if (num2 > 0x63)
            {
                return CalcErrors.Value;
            }
            CalcRoundFunction function = new CalcRoundFunction();
            num = (double) ((double) function.Evaluate(new object[] { (double) num, (int) num2 }));
            NumberFormatInfo provider = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
            provider.CurrencyDecimalDigits = (num2 < 0) ? 0 : num2;
            return ((double) num).ToString("C", provider);
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
                return "DOLLAR";
            }
        }
    }
}

