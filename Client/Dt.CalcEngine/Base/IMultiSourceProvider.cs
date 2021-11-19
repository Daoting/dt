#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Defines a mechanism for retrieving a <see cref="T:Dt.CalcEngine.ICalcSource" /> enumerator.
    /// </summary>
    public interface IMultiSourceProvider
    {
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:Dt.CalcEngine.ICalcSource" /> collection.
        /// </summary>
        /// <param name="startSource">The start <see cref="T:Dt.CalcEngine.ICalcSource" /> that need to enumerate.</param>
        /// <param name="endSource">The end <see cref="T:Dt.CalcEngine.ICalcSource" /> that need to enumerate.</param>
        /// <returns></returns>
        IEnumerable<ICalcSource> GetCalcSources(ICalcSource startSource, ICalcSource endSource);
    }
}

