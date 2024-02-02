#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Provides a data address for the special function.
    /// </summary>
    public class CalcFunctionIdentity : CalcIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcFunctionIdentity" /> class.
        /// </summary>
        /// <param name="functionName">The name that represents a function call.</param>
        public CalcFunctionIdentity(string functionName)
        {
            this.FunctionName = functionName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> 
        /// is equal to the current <see cref="T:Dt.CalcEngine.CalcCellIdentity" />.
        /// </summary>
        /// <param name="other">
        /// The <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> to compare with the
        /// current <see cref="T:Dt.CalcEngine.CalcCellIdentity" />. 
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> 
        /// is equal to the current <see cref="T:Dt.CalcEngine.CalcCellIdentity" />; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        protected override bool CompareTo(CalcIdentity other)
        {
            CalcFunctionIdentity objA = other as CalcFunctionIdentity;
            return (!object.ReferenceEquals(objA, null) && (string.Compare(this.FunctionName, objA.FunctionName, StringComparison.CurrentCultureIgnoreCase) == 0));
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        protected override int ComputeHash()
        {
            if (this.FunctionName == null)
            {
                return 0;
            }
            return this.FunctionName.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}", new object[] { this.FunctionName });
        }

        /// <summary>
        /// Gets the name that represents the calling function.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.String" /> value that represents the function name.
        /// </value>
        public string FunctionName { get; private set; }
    }
}

