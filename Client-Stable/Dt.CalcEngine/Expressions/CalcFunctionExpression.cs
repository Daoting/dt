#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Functions;
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents an expression with a function applied to 
    /// a list of parameters as the expression.
    /// </summary>
    /// <remarks>
    /// The expression uses dynamic function binding (that is, 
    /// function implementation is determined at evaluation time). 
    /// Dynamic binding is commonly used for user defined functions.
    /// </remarks>
    public class CalcFunctionExpression : CalcExpression
    {
        private CalcExpression[] _arguments;
        private CalcFunction _function;
        private string _functionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcFunctionExpression" /> class.
        /// </summary>
        /// <param name="calcFunction">The function.</param>
        /// <param name="args">List of parameters.</param>
        /// <exception cref="T:System.ArgumentNullException">calcFunction or args is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">calcFunction does not accept the number of parameters found in args.</exception>
        public CalcFunctionExpression(CalcFunction calcFunction, CalcExpression[] args)
        {
            base.ThrowIfArgumentNull<CalcFunction>(calcFunction, "calcFunction");
            if ((calcFunction.MinArgs > 0) && ((args == null) || (calcFunction.MinArgs > args.Length)))
            {
                throw new ArgumentException("args");
            }
            this._functionName = null;
            this._function = calcFunction;
            this._arguments = args;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcFunctionExpression" /> class.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="args">List of parameters.</param>
        /// <exception cref="T:System.ArgumentNullException">functionName or args is <see langword="null" />.</exception>
        public CalcFunctionExpression(string functionName, CalcExpression[] args)
        {
            base.ThrowIfArgumentNull<CalcExpression[]>(args, "args");
            this._functionName = functionName;
            this._function = null;
            this._arguments = args;
        }

        /// <summary>
        /// Returns the specified parameter being passed to the function.
        /// </summary>
        /// <param name="i">Index of the parameter (or argument).</param>
        /// <returns>
        /// The parameter at the specified index.
        /// </returns>
        public CalcExpression GetArg(int i)
        {
            if (this._arguments == null)
            {
                return null;
            }
            return this._arguments[i];
        }

        /// <summary>
        /// Get a new expression with specific offset.
        /// </summary>
        /// <param name="row">the row offset</param>
        /// <param name="column">the column offset</param>
        /// <param name="offsetAbsolute"><c>true</c> if offset the absolute indexes.</param>
        /// <param name="offsetRelative"><c>true</c> if offset the relative indexes.</param>
        /// <returns>
        /// Return a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> which offset from current expression.
        /// </returns>
        public override CalcExpression Offset(int row, int column, bool offsetAbsolute = false, bool offsetRelative = true)
        {
            CalcExpression[] args = new CalcExpression[this.ArgCount];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = this.GetArg(i).Offset(row, column, offsetAbsolute, offsetRelative);
            }
            if (this.Function != null)
            {
                return new CalcFunctionExpression(this.Function, args);
            }
            return new CalcFunctionExpression(this.FunctionName, args);
        }

        /// <summary>
        /// Gets the number of parameters being passed to the function.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that represents 
        /// the number of parameters being passed to the function.
        /// </value>
        public int ArgCount
        {
            get
            {
                if (this._arguments == null)
                {
                    return 0;
                }
                return this._arguments.Length;
            }
        }

        /// <summary>
        /// Gets the implementation (dynamic or static function binding) of the function. 
        /// </summary>
        /// <remarks>
        /// If dynamic binding was used then the returned value will be null.  
        /// If static binding was used then the returned value will be non-null.
        /// </remarks>
        /// <value>The implementation of the function.</value>
        public CalcFunction Function
        {
            get
            {
                return this._function;
            }
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.String" /> value that represents the name of the function.
        /// </value>
        public string FunctionName
        {
            get
            {
                if (this._function != null)
                {
                    return this._function.Name;
                }
                return this._functionName;
            }
        }
    }
}

