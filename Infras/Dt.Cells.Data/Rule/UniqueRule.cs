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
    /// Represents a unique condition rule.
    /// </summary>
    public sealed class UniqueRule : FormattingRuleBase
    {
        /// <summary>
        /// Constructs a unique rule.
        /// </summary>
        public UniqueRule() : this(null)
        {
        }

        /// <summary>
        /// Creates a new text rule with the specified comparison operator, text, and style.
        /// </summary>
        /// <param name="style">Cell style.</param>
        internal UniqueRule(StyleInfo style) : base(style)
        {
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return (base.Clone() as UniqueRule);
        }

        /// <summary>
        /// Creates a new unique rule with specified parameters.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>The new unique rule.</returns>
        public static UniqueRule Create(StyleInfo style)
        {
            return new UniqueRule(style);
        }

        /// <summary>
        /// Creates conditions for the rule.
        /// </summary>
        /// <returns>
        /// The  condition.
        /// </returns>
        protected override ConditionBase CreateCondition()
        {
            return new UniqueCondition(false, base.Ranges);
        }

        /// <summary>
        /// Initial condition for the rule.
        /// </summary>
        protected override void InitCondition()
        {
            base.InitCondition();
            UniqueCondition condition = base.condition as UniqueCondition;
            if (condition != null)
            {
                condition.Ranges = base.Ranges;
            }
        }
    }
}

