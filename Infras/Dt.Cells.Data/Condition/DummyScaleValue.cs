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
    internal class DummyScaleValue : IConditionalFormula
    {
        ScaleRule scaleRule;
        int valueIndex;

        public DummyScaleValue(ScaleRule rule, int index)
        {
            this.scaleRule = rule;
            this.valueIndex = index;
        }

        public string Formula
        {
            get
            {
                if (this.scaleRule.Scales[this.valueIndex].Type == ScaleValueType.Formula)
                {
                    return (string) (this.scaleRule.Scales[this.valueIndex].Value as string);
                }
                string str = (string) (this.scaleRule.Scales[this.valueIndex].Value as string);
                if (((str != null) && (str.Length > 1)) && str.StartsWith("="))
                {
                    return str;
                }
                return null;
            }
            set
            {
                string str = value;
                if (!string.IsNullOrWhiteSpace(str) && !str.StartsWith("="))
                {
                    str = "=" + str;
                }
                this.scaleRule.Scales[this.valueIndex] = new ScaleValue(this.scaleRule.Scales[this.valueIndex].Type, str);
            }
        }
    }
}

