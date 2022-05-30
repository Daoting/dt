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
    /// Represents an abstract base class for defining functions.
    /// </summary>
    public abstract class CalcFunction
    {
        protected CalcFunction()
        {
        }

        /// <summary>
        /// Determines whether the function accepts array values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts array values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public virtual bool AcceptsArray(int i)
        {
            return false;
        }

        /// <summary>
        /// Indicates whether the function can process CalcError values.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function can process CalcError values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public virtual bool AcceptsError(int i)
        {
            return false;
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public virtual bool AcceptsMissingArgument(int i)
        {
            return false;
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
        public virtual bool AcceptsReference(int i)
        {
            return false;
        }

        /// <summary>
        /// Returns the result of the function applied to the arguments.
        /// </summary>
        /// <param name="args">Arguments for the function evaluation</param>
        /// <returns>
        /// Result of the function applied to the arguments
        /// </returns>
        public abstract object Evaluate(object[] args);
        /// <summary>
        /// Returns the result of the function applied to the arguments.
        /// </summary>
        /// <param name="args">Arguments for the function evaluation</param>
        /// <param name="context">Context in which the evaluation occurs</param>
        /// <returns>
        /// Result of the function applied to the arguments
        /// </returns>
        public virtual object Evaluate(object[] args, object context)
        {
            return this.Evaluate(args);
        }

        /// <summary>
        /// Finds the branch argument.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <returns></returns>
        public virtual int FindBranchArgument(object test)
        {
            return -1;
        }

        /// <summary>
        /// Finds the test argument when this function is branched.
        /// </summary>
        /// <returns>An index indicates the argument which would be treated as test condition</returns>
        public virtual int FindTestArgument()
        {
            return -1;
        }

        /// <summary>
        /// Determines whether the function is volatile while evaluate.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the function is volatile; 
        /// <see langword="false" /> otherwise.
        /// </returns>
        public virtual bool IsVolatile()
        {
            return false;
        }

        /// <summary>
        /// Returns the string representation of the function.
        /// </summary>
        /// <returns>String representation of the function</returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Gets a value indicating whether this function is branched by arguments as condition.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is branch; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsBranch
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the evaluation of the function is dependent
        /// on the context in which the evaluation occurs.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> the evaluation of the function is dependent on the context;
        /// <see langword="false" /> otherwise.
        /// </returns>
        public virtual bool IsContextSensitive
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The maximum number of arguments for the function.
        /// </value>
        public abstract int MaxArgs { get; }

        /// <summary>
        /// Gets the minimum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The minimum number of arguments for the function.
        /// </value>
        public abstract int MinArgs { get; }

        /// <summary>
        /// Gets The name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public abstract string Name { get; }
    }
}

