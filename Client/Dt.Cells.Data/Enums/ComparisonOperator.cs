#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Specifies the conditional format operators.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// Determines whether a cell value is between the two parameter values.
        /// </summary>
        Between = 1,
        /// <summary>
        /// Determines whether a cell value is equal to the parameter value.
        /// </summary>
        EqualTo = 3,
        /// <summary>
        /// Determines whether a cell value is greater than the parameter value.
        /// </summary>
        GreaterThan = 5,
        /// <summary>
        /// Determines whether a cell value is greater than or equal to the parameter value.
        /// </summary>
        GreaterThanOrEqualTo = 7,
        /// <summary>
        /// Determines whether a cell value is less than the parameter value.
        /// </summary>
        LessThan = 6,
        /// <summary>
        /// Determines whether a cell value is less than or equal to the parameter value.
        /// </summary>
        LessThanOrEqualTo = 8,
        /// <summary>
        /// Determines whether a cell value is not between the two parameter values.
        /// </summary>
        NotBetween = 2,
        /// <summary>
        /// Determines whether a cell value is not equal to the parameter value.
        /// </summary>
        NotEqualTo = 4
    }
}

