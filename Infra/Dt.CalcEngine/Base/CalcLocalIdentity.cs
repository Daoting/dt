#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Providers a local data address.
    /// </summary>
    public abstract class CalcLocalIdentity : CalcIdentity, IEqualityComparer<CalcLocalIdentity>
    {
        protected CalcLocalIdentity()
        {
        }

        /// <summary>
        /// Converts to external identity.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <returns>The external identity</returns>
        public virtual CalcExternalIdentity ConvertToExternal(ICalcSource source)
        {
            return null;
        }

        bool IEqualityComparer<CalcLocalIdentity>.Equals(CalcLocalIdentity x, CalcLocalIdentity y)
        {
            return CalcIdentity.Compare(x, y);
        }

        int IEqualityComparer<CalcLocalIdentity>.GetHashCode(CalcLocalIdentity obj)
        {
            if (!object.ReferenceEquals(obj, null))
            {
                return obj.ComputeHash();
            }
            return 0;
        }
    }
}

