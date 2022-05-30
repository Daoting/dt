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
    /// 
    /// </summary>
    public class CalcExternalNameIdentity : CalcExternalIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcNameIdentity" /> class.
        /// </summary>
        /// <param name="source">The owner which contains the data.</param>
        /// <param name="name">The name that represents a data address.</param>
        public CalcExternalNameIdentity(ICalcSource source, string name) : base(source)
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
            CalcExternalNameIdentity objA = other as CalcExternalNameIdentity;
            if (object.ReferenceEquals(objA, null))
            {
                return false;
            }
            return (object.ReferenceEquals(base.Source, objA.Source) && (this.Name == objA.Name));
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        protected override int ComputeHash()
        {
            int num = (base.Source != null) ? base.Source.GetHashCode() : 0;
            int num2 = (this.Name != null) ? this.Name.GetHashCode() : 0;
            return (num ^ num2);
        }

        /// <summary>
        /// Converts to non-external identity.
        /// </summary>
        /// <returns></returns>
        public override CalcLocalIdentity ConvertToLocal()
        {
            return new CalcNameIdentity(this.Name);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("'{0}'!{1}", new object[] { (base.Source != null) ? base.Source.GetParserContext(null).GetExternalSourceToken(base.Source) : CalcErrors.Reference.ToString(), this.Name });
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

