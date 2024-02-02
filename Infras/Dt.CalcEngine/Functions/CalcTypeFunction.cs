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
    /// Returns the type of value.
    /// </summary>
    /// <remarks>
    /// Use TYPE when the behavior of another function depends on 
    /// the type of value in a particular cell.
    /// </remarks>
    public class CalcTypeFunction : CalcBuiltinFunction
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
        /// Indicates whether the function can process CalcError values.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function can process CalcError values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsError(int i)
        {
            return true;
        }

        /// <summary>
        /// Returns the type of value.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: value.
        /// </para>
        /// <para>
        /// Value can be any Microsoft Excel value, such as a number, text,
        /// logical value, and so on.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object obj2 = args[0];
            if (CalcConvert.IsNumber(obj2))
            {
                return (int) 1;
            }
            if (obj2 is string)
            {
                return (int) 2;
            }
            if (obj2 is bool)
            {
                return (int) 4;
            }
            if (obj2 is CalcError)
            {
                return (int) 0x10;
            }
            if (obj2 is CalcArray)
            {
                return (int) 0x40;
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
                return "TYPE";
            }
        }
    }
}

