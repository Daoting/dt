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

namespace Dt.CalcEngine
{
    /// <summary>
    /// Providers a external data address.
    /// </summary>
    public abstract class CalcExternalIdentity : CalcIdentity
    {
        internal ICalcSource _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcExternalIdentity" /> class.
        /// </summary>
        protected CalcExternalIdentity(ICalcSource source)
        {
            this.Source = source;
        }

        /// <summary>
        /// Converts to non-external identity.
        /// </summary>
        /// <returns></returns>
        public virtual CalcLocalIdentity ConvertToLocal()
        {
            return null;
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public ICalcSource Source
        {
            get
            {
                return this._source;
            }
            private set
            {
                this._source = value;
            }
        }
    }
}

