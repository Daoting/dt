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

namespace Dt.CalcEngine.Operators
{
    /// <summary>
    /// Provide all of the unary operators.
    /// </summary>
    public static class CalcUnaryOperators
    {
        /// <summary>
        /// Represents an instance of the unary negate operator.
        /// </summary>
        public static readonly CalcUnaryOperator Negate = new CalcNegateOperator();
        /// <summary>
        /// Represents an instance of the unary percent operator.
        /// </summary>
        public static readonly CalcUnaryOperator Percent = new CalcPercentOperator();
        /// <summary>
        /// Represents an instance of the unary plus operator.
        /// </summary>
        public static readonly CalcUnaryOperator Plus = new CalcPlusOperator();
    }
}

