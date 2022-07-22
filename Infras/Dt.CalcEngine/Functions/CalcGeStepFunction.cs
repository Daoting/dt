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
    /// Returns the boolean value depending if a number is greater than a threshold value.
    /// </summary>
    public class CalcGeStepFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns></returns>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 1);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Int32" /> depending if a number is greater than a threshold value.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: number, [step].
        /// </para>
        /// <para>
        /// Number is the value to test against step.
        /// </para>
        /// <para>
        /// Step is the threshold value. If you omit a value for step, GESTEP uses zero.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            double number = 0.0;
            if (CalcHelper.ArgumentExists(args, 1) && !CalcConvert.TryToDouble(args[1], out number, true))
            {
                return CalcErrors.Value;
            }
            return ((num >= number) ? ((int) 1) : ((int) 0));
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
                return "GESTEP";
            }
        }
    }
}

