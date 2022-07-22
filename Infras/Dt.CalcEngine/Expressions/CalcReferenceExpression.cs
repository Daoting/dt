#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents an reference expression.
    /// This is an abstract class.
    /// </summary>
    public abstract class CalcReferenceExpression : CalcExpression
    {
        /// <summary>
        /// Gets the identity of current expressions based on <paramref name="row" /> and <paramref name="column" />.
        /// </summary>
        /// <param name="row">The base row.</param>
        /// <param name="column">The base column.</param>
        /// <returns></returns>
        public abstract CalcIdentity GetId(int row, int column);
        internal void Sort(ref int index1, ref int index2)
        {
            if (index1 > index2)
            {
                int num = index1;
                index1 = index2;
                index2 = num;
            }
        }
    }
}

