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
    /// Returns a value converted to a number.
    /// </summary>
    public class CalcNFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns a value converted to a number.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: value.
        /// </para>
        /// <para>
        /// Value is the value you want converted. N converts values listed in the following table.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object obj2 = args[0];
            if (CalcConvert.IsNumber(obj2))
            {
                return (double) CalcConvert.ToDouble(obj2);
            }
            if (obj2 is bool)
            {
                return (((bool) obj2) ? ((double) 1.0) : ((double) 0.0));
            }
            if (obj2 is CalcError)
            {
                return obj2;
            }
            return (double) 0.0;
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
                return 1;
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
                return "N";
            }
        }
    }
}

