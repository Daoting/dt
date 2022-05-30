#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    internal class BinaryOperatorInfo : OperatorInfo
    {
        /// <summary>
        /// Represents an instance of the binary addition operator.
        /// </summary>
        public static readonly BinaryOperatorInfo AddOperator = new BinaryOperatorInfo("+");
        /// <summary>
        /// Represents an instance of the binary concatenate operator.
        /// </summary>
        public static readonly BinaryOperatorInfo ConcatenateOperator = new BinaryOperatorInfo("&");
        /// <summary>
        /// Represents an instance of the binary division operator.
        /// </summary>
        public static readonly BinaryOperatorInfo DivideOperator = new BinaryOperatorInfo("/");
        /// <summary>
        /// Represents an instance of the binary equals operator.
        /// </summary>
        public static readonly BinaryOperatorInfo EqualOperator = new BinaryOperatorInfo("=");
        /// <summary>
        /// Represents an instance of the binary exponent operator.
        /// </summary>
        public static readonly BinaryOperatorInfo ExponentOperator = new BinaryOperatorInfo("^");
        /// <summary>
        /// Represents an instance of the binary "greater than" operator.
        /// </summary>
        public static readonly BinaryOperatorInfo GreaterThanOperator = new BinaryOperatorInfo(">");
        /// <summary>
        /// Represents an instance of the binary "greater than or equals" operator.
        /// </summary>
        public static readonly BinaryOperatorInfo GreaterThanOrEqualOperator = new BinaryOperatorInfo(">=");
        /// <summary>
        /// Represents an instance of the binary "Intersect" operator.
        /// </summary>
        public static readonly BinaryOperatorInfo IntersectOperator = new BinaryOperatorInfo(" ");
        /// <summary>
        /// Represents an instance of the binary "less than" operator.
        /// </summary>
        public static readonly BinaryOperatorInfo LessThanOperator = new BinaryOperatorInfo("<");
        /// <summary>
        /// Represents an instance of the binary "less than or equals" operator.
        /// </summary>
        public static readonly BinaryOperatorInfo LessThanOrEqualOperator = new BinaryOperatorInfo("<=");
        /// <summary>
        /// Represents an instance of the binary multiplication operator.
        /// </summary>
        public static readonly BinaryOperatorInfo MultiplyOperator = new BinaryOperatorInfo("*");
        /// <summary>
        /// Represents an instance of the binary "not equals" operator.
        /// </summary>
        public static readonly BinaryOperatorInfo NotEqualOperator = new BinaryOperatorInfo("<>");
        /// <summary>
        /// Represents an instance of the binary "Intersect" operator.
        /// </summary>
        public static readonly BinaryOperatorInfo RangeOperator = new BinaryOperatorInfo(":");
        /// <summary>
        /// Represents an instance of the binary subtraction operator.
        /// </summary>
        public static readonly BinaryOperatorInfo SubtractOperator = new BinaryOperatorInfo("-");
        /// <summary>
        /// Represents an instance of the binary "Union" operator.
        /// </summary>
        public static readonly BinaryOperatorInfo UnionOperator = new BinaryOperatorInfo(",");

        public BinaryOperatorInfo(string name) : base(name)
        {
        }
    }
}

