#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    internal class DummyCellValue : IConditionalFormula
    {
        CellValueRule cellValueRule;
        int index;

        public DummyCellValue(CellValueRule cellValueRule, int index)
        {
            this.cellValueRule = cellValueRule;
            this.index = index;
        }

        public static List<DummyCellValue> GetDummyCellvalues(CellValueRule cellValueRule)
        {
            List<DummyCellValue> list = new List<DummyCellValue>();
            if (cellValueRule != null)
            {
                int[] numArray = new int[] { 0, 1, 10, 11, 20, 0x15 };
                foreach (int num in numArray)
                {
                    if (GetExpectedFormula(cellValueRule, num) != null)
                    {
                        list.Add(new DummyCellValue(cellValueRule, num));
                    }
                }
            }
            return list;
        }

        static string GetExpectedFormula(CellValueRule cellValueRule, int index)
        {
            if ((cellValueRule != null) && (cellValueRule.Condition != null))
            {
                if (cellValueRule.Condition is RelationCondition)
                {
                    RelationCondition condition = cellValueRule.Condition as RelationCondition;
                    if (((index != 10) && (index != 11)) || ((condition.Item1 == null) || !(condition.Item1 is RelationCondition)))
                    {
                        if (((index != 20) && (index != 0x15)) || ((condition.Item2 == null) || !(condition.Item2 is RelationCondition)))
                        {
                            if (((index == 0) && (condition.Item1 != null)) && (condition.Item1.ExpectedFormula != null))
                            {
                                return ("=" + condition.Item1.ExpectedFormula);
                            }
                            if (((index == 1) && (condition.Item2 != null)) && (condition.Item2.ExpectedFormula != null))
                            {
                                return ("=" + condition.Item2.ExpectedFormula);
                            }
                        }
                        else
                        {
                            condition = condition.Item2 as RelationCondition;
                            if (((index == 20) && (condition.Item1 != null)) && (condition.Item1.ExpectedFormula != null))
                            {
                                return ("=" + condition.Item1.ExpectedFormula);
                            }
                            if (((index == 0x15) && (condition.Item2 != null)) && (condition.Item2.ExpectedFormula != null))
                            {
                                return ("=" + condition.Item2.ExpectedFormula);
                            }
                        }
                    }
                    else
                    {
                        condition = condition.Item1 as RelationCondition;
                        if (((index == 10) && (condition.Item1 != null)) && (condition.Item1.ExpectedFormula != null))
                        {
                            return ("=" + condition.Item1.ExpectedFormula);
                        }
                        if (((index == 11) && (condition.Item2 != null)) && (condition.Item2.ExpectedFormula != null))
                        {
                            return ("=" + condition.Item2.ExpectedFormula);
                        }
                    }
                }
                else if (((index == 0) && (cellValueRule.Condition != null)) && (cellValueRule.Condition.ExpectedFormula != null))
                {
                    return ("=" + cellValueRule.Condition.ExpectedFormula);
                }
            }
            return null;
        }

        public string Formula
        {
            get { return  GetExpectedFormula(this.cellValueRule, this.index); }
            set
            {
                if ((this.cellValueRule != null) && !string.IsNullOrWhiteSpace(value))
                {
                    string str = value;
                    if (!str.StartsWith("="))
                    {
                        str = "=" + str;
                    }
                    if (((this.index == 0) || (this.index == 10)) || (this.index == 20))
                    {
                        this.cellValueRule.Value1 = str;
                    }
                    else if (((this.index == 1) || (this.index == 11)) || (this.index == 0x15))
                    {
                        this.cellValueRule.Value2 = str;
                    }
                }
            }
        }
    }
}

