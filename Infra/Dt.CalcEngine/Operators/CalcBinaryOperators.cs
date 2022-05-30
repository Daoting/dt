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
    /// Provide all of the binary operators.
    /// </summary>
    public static class CalcBinaryOperators
    {
        /// <summary>
        /// Represents an instance of the binary addition operator.
        /// </summary>
        public static readonly CalcBinaryOperator Add = new CalcAddOperator();
        /// <summary>
        /// Represents an instance of the binary concatenate operator.
        /// </summary>
        public static readonly CalcBinaryOperator Concatenate = new CalcConcatenateOperator();
        /// <summary>
        /// Represents an instance of the binary division operator.
        /// </summary>
        public static readonly CalcBinaryOperator Divide = new CalcDivideOperator();
        /// <summary>
        /// Represents an instance of the binary equals operator.
        /// </summary>
        public static readonly CalcBinaryOperator Equal = new CalcEqualOperator();
        /// <summary>
        /// Represents an instance of the binary exponent operator.
        /// </summary>
        public static readonly CalcBinaryOperator Exponent = new CalcExponentOperator();
        /// <summary>
        /// Represents an instance of the binary "greater than" operator.
        /// </summary>
        public static readonly CalcBinaryOperator GreaterThan = new CalcGreaterThanOperator();
        /// <summary>
        /// Represents an instance of the binary "greater than or equals" operator.
        /// </summary>
        public static readonly CalcBinaryOperator GreaterThanOrEqual = new CalcGreaterThanOrEqualOperator();
        /// <summary>
        /// Represents an instance of the binary intersect operator.
        /// </summary>
        public static readonly CalcBinaryOperator Intersection = new CalcIntersectionOperator();
        /// <summary>
        /// Represents an instance of the binary "less than" operator.
        /// </summary>
        public static readonly CalcBinaryOperator LessThan = new CalcLessThanOperator();
        /// <summary>
        /// Represents an instance of the binary "less than or equals" operator.
        /// </summary>
        public static readonly CalcBinaryOperator LessThanOrEqual = new CalcLessThanOrEqualOperator();
        /// <summary>
        /// Represents an instance of the binary multiplication operator.
        /// </summary>
        public static readonly CalcBinaryOperator Multiply = new CalcMultiplyOperator();
        /// <summary>
        /// Represents an instance of the binary "not equals" operator.
        /// </summary>
        public static readonly CalcBinaryOperator NotEqual = new CalcNotEqualOperator();
        /// <summary>
        /// Represents an instance of the binary range operator.
        /// </summary>
        public static readonly CalcBinaryOperator Range = new CalcRangeOperator();
        /// <summary>
        /// Represents an instance of the binary subtraction operator.
        /// </summary>
        public static readonly CalcBinaryOperator Subtract = new CalcSubtractOperator();
        /// <summary>
        /// Represents an instance of the binary union operator.
        /// </summary>
        public static readonly CalcBinaryOperator Union = new CalcUnionOperator();
    }
}

