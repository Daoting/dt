#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDataSeries
    {
        /// <summary>
        /// Gets the data orientation.
        /// </summary>
        /// <value>
        /// The data orientation.
        /// </value>
        Dt.Cells.Data.DataOrientation? DataOrientation { get; }

        /// <summary>
        /// Gets the data reference.
        /// </summary>
        /// <value>
        /// The data reference.
        /// </value>
        CalcExpression DataReference { get; }

        /// <summary>
        /// Gets a value indicating whether [display hidden data].
        /// </summary>
        /// <value>
        /// <c>true</c> if [display hidden data]; otherwise, <c>false</c>.
        /// </value>
        bool DisplayHiddenData { get; }

        /// <summary>
        /// Gets the empty value style.
        /// </summary>
        /// <value>
        /// The empty value style.
        /// </value>
        Dt.Cells.Data.EmptyValueStyle EmptyValueStyle { get; }

        /// <summary>
        /// 
        /// </summary>
        ICalcEvaluator Evaluator { get; }
    }
}

