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
    internal class RulesChangedEventArgs : EventArgs
    {
        public RulesChangedEventArgs(FormattingRuleBase rule, RulesChangedAction action)
        {
            this.Rule = rule;
            this.Action = action;
        }

        public RulesChangedAction Action { get; private set; }

        public FormattingRuleBase Rule { get; private set; }
    }
}

