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
    /// Returns an evenly distributed random real number greater than 
    /// or equal to 0 and less than 1.
    /// </summary>
    /// <remarks>
    /// A new random real number is returned every time the worksheet 
    /// is calculated.
    /// </remarks>
    public class CalcRandFunction : CalcBuiltinFunction
    {
        private readonly Random _rand = new Random((int) DateTime.Now.Ticks);

        /// <summary>
        /// Returns an evenly distributed random real number greater than
        /// or equal to 0 and less than 1.
        /// </summary>
        /// <param name="args">The args contains 0 item.</param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            return (double) this._rand.NextDouble();
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
                return "RAND";
            }
        }
    }
}

