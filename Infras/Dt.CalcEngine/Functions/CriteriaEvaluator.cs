#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Operators;
using System;
#endregion

namespace Dt.CalcEngine.Functions
{
    internal class CriteriaEvaluator
    {
        private CalcBinaryOperator criteriaOperator;
        private object criteriaValue;

        public CriteriaEvaluator(CalcBinaryOperator criteriaOperator, object criteriaValue)
        {
            this.criteriaOperator = criteriaOperator;
            this.criteriaValue = criteriaValue;
        }

        public bool Evaluate(object databaseValue)
        {
            object obj2 = this.criteriaOperator.Evaluate(databaseValue, this.criteriaValue, null);
            return ((obj2 is bool) && ((bool) ((bool) obj2)));
        }
    }
}

