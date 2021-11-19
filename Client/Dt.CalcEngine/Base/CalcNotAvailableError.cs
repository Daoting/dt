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
    /// Represent a not available error which is detailed as <b>#N/A</b>.
    /// </summary>
    public class CalcNotAvailableError : CalcError
    {
        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "#N/A";
        }

        internal override int ErrorCode
        {
            get
            {
                return 0x2a;
            }
        }
    }
}

