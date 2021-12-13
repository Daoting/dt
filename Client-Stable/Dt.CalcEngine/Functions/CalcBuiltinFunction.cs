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
    /// Represents an abstract base class for CalcEngine builtin functions.
    /// </summary>
    public abstract class CalcBuiltinFunction : CalcFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Functions.CalcBuiltinFunction" /> class.
        /// </summary>
        internal CalcBuiltinFunction()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// </param>
        internal void CheckArgumentsLength(object[] args)
        {
            if (args == null)
            {
                if (this.MinArgs > 0)
                {
                    throw new ArgumentNullException();
                }
            }
            else if ((args.Length < this.MinArgs) || (args.Length > this.MaxArgs))
            {
                throw new ArgumentException("Exceptions.InvalidFunctionArgs");
            }
        }
    }
}

