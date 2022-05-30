#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal class ConditionChangedEventArgs : EventArgs
    {
        public ConditionChangedEventArgs(IConditionalFormula[] oldConditionalFormulas, IConditionalFormula[] newConditionalFormulas)
        {
            this.OldConditionalFormulas = oldConditionalFormulas;
            this.NewConditionalFormulas = newConditionalFormulas;
        }

        public IConditionalFormula[] NewConditionalFormulas { get; private set; }

        public IConditionalFormula[] OldConditionalFormulas { get; private set; }
    }
}

