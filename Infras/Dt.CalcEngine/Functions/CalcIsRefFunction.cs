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
    /// Checks whether the value is refers to a reference.
    /// </summary>
    public class CalcIsRefFunction : CalcBuiltinFunction
    {
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
        /// Indicates whether the Evaluate method can process references.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process references; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsReference(int i)
        {
            return true;
        }

        /// <summary>
        /// Checks whether the value is refers to a reference.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: value.
        /// </para>
        /// <para>
        /// Value is the value you want tested. Value can be a blank (empty cell),
        /// error, logical, text, number, or reference value, or a name referring
        /// to any of these, that you want to test.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Boolean" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            return (bool) (args[0] is CalcReference);
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
                return "ISREF";
            }
        }
    }
}

