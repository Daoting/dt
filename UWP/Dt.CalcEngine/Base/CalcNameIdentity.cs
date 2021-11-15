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
    /// Provides a data address by the special name.
    /// </summary>
    /// <remarks>
    /// The name can identify the cell, cell range, function, formula, constant or table.
    /// </remarks>
    public class CalcNameIdentity : CalcLocalIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcNameIdentity" /> class.
        /// </summary>
        /// <param name="name">The name that represents a data address.</param>
        public CalcNameIdentity(string name)
        {
            this.Name = name;
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
            CalcNameIdentity objA = other as CalcNameIdentity;
            return (!object.ReferenceEquals(objA, null) && (this.Name == objA.Name));
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        protected override int ComputeHash()
        {
            if (this.Name == null)
            {
                return 0;
            }
            return this.Name.GetHashCode();
        }

        /// <summary>
        /// Converts to external identity.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <returns>The external identity</returns>
        public override CalcExternalIdentity ConvertToExternal(ICalcSource source)
        {
            return new CalcExternalNameIdentity(source, this.Name);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}", new object[] { this.Name });
        }

        /// <summary>
        /// Gets the name that represents a data address.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.String" /> value that represents a data address.
        /// </value>
        public string Name { get; private set; }
    }
}

