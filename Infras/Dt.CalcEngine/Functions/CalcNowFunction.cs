#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the <see cref="T:System.DateTime" /> of the current date and time.
    /// </summary>
    public class CalcNowFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.DateTime" /> of the current date and time.
        /// </summary>
        /// <param name="args">The args contains 0 item.</param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            return DateTime.Now;
        }

        /// <summary>
        /// Determines if the function is volatile.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the function is volatile; otherwise, <see langword="false" />.
        /// </value>
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
                return "NOW";
            }
        }
    }
}

