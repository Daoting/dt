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
    internal class DummyIconSetValue : IConditionalFormula
    {
        IconSetRule iconSetRule;
        int valueIndex;

        public DummyIconSetValue(IconSetRule rule, int index)
        {
            this.iconSetRule = rule;
            this.valueIndex = index;
        }

        public string Formula
        {
            get
            {
                if ((this.iconSetRule != null) && (this.iconSetRule.IconCriteria != null))
                {
                    if ((this.valueIndex < this.iconSetRule.IconCriteria.Length) && (this.iconSetRule.IconCriteria[this.valueIndex].IconValueType == IconValueType.Formula))
                    {
                        return (string) (this.iconSetRule.IconCriteria[this.valueIndex].Value as string);
                    }
                    string str = (string) (this.iconSetRule.IconCriteria[this.valueIndex].Value as string);
                    if (((str != null) && (str.Length > 1)) && str.StartsWith("="))
                    {
                        return str;
                    }
                }
                return null;
            }
            set
            {
                bool isGreaterThanOrEqualTo = this.iconSetRule.IconCriteria[this.valueIndex].IsGreaterThanOrEqualTo;
                string str = value;
                if (!string.IsNullOrEmpty(str) && !str.StartsWith("="))
                {
                    str = "=" + str;
                }
                this.iconSetRule.IconCriteria[this.valueIndex] = new IconCriterion(isGreaterThanOrEqualTo, this.iconSetRule.IconCriteria[this.valueIndex].IconValueType, str);
            }
        }
    }
}

