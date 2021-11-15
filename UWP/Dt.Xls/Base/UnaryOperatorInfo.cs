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
    internal class UnaryOperatorInfo : OperatorInfo
    {
        /// <summary>
        /// Represents an instance of the unary negate operator.
        /// </summary>
        public static readonly UnaryOperatorInfo NegateOperator = new UnaryOperatorInfo("-");
        /// <summary>
        /// Represents an instance of the unary percent operator.
        /// </summary>
        public static readonly UnaryOperatorInfo PercentOperator = new UnaryOperatorInfo("%");
        /// <summary>
        /// Represents an instance of the unary plus operator.
        /// </summary>
        public static readonly UnaryOperatorInfo PlusOperator = new UnaryOperatorInfo("+");

        public UnaryOperatorInfo(string name) : base(name)
        {
        }
    }
}

