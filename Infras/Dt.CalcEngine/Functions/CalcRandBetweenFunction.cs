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
    /// Returns a random integer number between the numbers you specify.
    /// </summary>
    /// <remarks>
    /// A new random integer number is returned every time the worksheet is calculated.
    /// </remarks>
    public class CalcRandBetweenFunction : CalcBuiltinFunction
    {
        private readonly Random _rand = new Random((int) DateTime.Now.Ticks);

        /// <summary>
        /// Returns a random integer number between the numbers you specify.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: bottom, top.
        /// </para>
        /// <para>
        /// Bottom is the smallest integer RANDBETWEEN will return.
        /// </para>
        /// <para>
        /// Top is the largest integer RANDBETWEEN will return.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num;
            int num2;
            base.CheckArgumentsLength(args);
            if (args[1] is CalcError)
            {
                return args[1];
            }
            if (args[0] is CalcError)
            {
                return args[0];
            }
            if (!CalcConvert.TryToInt(args[0], out num) || !CalcConvert.TryToInt(args[1], out num2))
            {
                return CalcErrors.Value;
            }
            if (num2 < num)
            {
                return CalcErrors.Number;
            }
            return (double) CalcConvert.ToDouble((int) this._rand.Next(num, num2 + 1));
        }

        /// <summary>
        /// Determines whether the function is volatile while evaluate.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the function is volatile; 
        /// <see langword="false" /> otherwise.
        /// </returns>
        public override bool IsVolatile()
        {
            return true;
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
                return "RANDBETWEEN";
            }
        }
    }
}

