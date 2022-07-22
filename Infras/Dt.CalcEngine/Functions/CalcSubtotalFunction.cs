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
    /// Returns a subtotal in a list or database.
    /// </summary>
    /// <remarks>
    /// It is generally easier to create a list with subtotals by using 
    /// the Subtotal command in the Outline group on the Data tab. Once 
    /// the subtotal list is created, you can modify it by editing the 
    /// SUBTOTAL function.
    /// </remarks>
    public class CalcSubtotalFunction : CalcBuiltinFunction
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
            return (i != 0);
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
            return (i != 0);
        }

        /// <summary>
        /// Returns a subtotal in a list or database.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 255 items: function_num, [ref1], [ref2], ..
        /// </para>
        /// <para>
        /// Function_num is the number 1 to 11 (includes hidden values) or 101
        /// to 111 (ignores hidden values) that specifies which function to use
        /// in calculating subtotals within a list.
        /// </para>
        /// <para>
        /// Ref1, ref2 are 1 to 254 ranges or references for which you want the subtotal.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int num = CalcConvert.ToInt(args[0]);
            object[] destinationArray = new object[args.Length - 1];
            Array.Copy(args, 1, destinationArray, 0, args.Length - 1);
            switch (num)
            {
                case 1:
                case 0x65:
                    return CalcAverageFunction.Evaluate(destinationArray, false);

                case 2:
                case 0x66:
                    return CalcCountFunction.Evaluate(destinationArray, false);

                case 3:
                case 0x67:
                    return CalcCountAFunction.Evaluate(destinationArray, false);

                case 4:
                case 0x68:
                    return CalcMaxFunction.Evaluate(destinationArray, false);

                case 5:
                case 0x69:
                    return CalcMinFunction.Evaluate(destinationArray, false);

                case 6:
                case 0x6a:
                    return CalcProductFunction.Evaluate(destinationArray, false);

                case 7:
                case 0x6b:
                    return CalcStDevFunction.Evaluate(destinationArray, false);

                case 8:
                case 0x6c:
                    return CalcStDevPFunction.Evaluate(destinationArray, false);

                case 9:
                case 0x6d:
                    return CalcSumFunction.Evaluate(destinationArray, false);

                case 10:
                case 110:
                    return CalcVarFunction.Evaluate(destinationArray, false);

                case 11:
                case 0x6f:
                    return CalcVarPFunction.Evaluate(destinationArray, false);
            }
            return CalcErrors.Value;
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
                return "SUBTOTAL";
            }
        }
    }
}

