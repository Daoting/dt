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
    /// Returns the error value #N/A.
    /// </summary>
    public class CalcNAFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the error value #N/A.
        /// </summary>
        /// <param name="args">The args contains 0 item.</param>
        /// <returns>
        /// A <see cref="T:Dt.CalcEngine.CalcError" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            return CalcErrors.NotAvailable;
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
                return 0;
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
                return 0;
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
                return "NA";
            }
        }
    }
}

