#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    internal class WorksheetConditionalGraph : ConditionalGraph
    {
        Dictionary<CellValueRule, List<DummyCellValue>> cellValues = new Dictionary<CellValueRule, List<DummyCellValue>>();
        Dictionary<IconSetRule, List<DummyIconSetValue>> iconValues = new Dictionary<IconSetRule, List<DummyIconSetValue>>();
        Dictionary<ScaleRule, List<DummyScaleValue>> scaleRules = new Dictionary<ScaleRule, List<DummyScaleValue>>();
        Worksheet worksheet;

        public WorksheetConditionalGraph(Worksheet source)
        {
            this.worksheet = source;
        }

        void AddCellValueRuleCondtion(CellValueRule cellValueRule)
        {
            if (cellValueRule != null)
            {
                List<DummyCellValue> dummyCellvalues = DummyCellValue.GetDummyCellvalues(cellValueRule);
                if (dummyCellvalues.Count > 0)
                {
                    this.cellValues.Add(cellValueRule, dummyCellvalues);
                    this.AddConditionsByRange(cellValueRule.Ranges, dummyCellvalues.ToArray());
                }
            }
        }

        void AddConditionsByRange(CellRange[] ranges, params IConditionalFormula[] conditions)
        {
            if (((ranges != null) && (ranges.Length != 0)) && ((conditions != null) && (conditions.Length != 0)))
            {
                for (int i = 0; i < ranges.Length; i++)
                {
                    CalcRangeIdentity actualId = new CalcRangeIdentity(ranges[i].Row, ranges[i].Column, ranges[i].RowCount, ranges[i].ColumnCount);
                    base.AddConditionals(actualId, conditions);
                }
            }
        }

        void AddIconSetRuleCondition(IconSetRule iconSetRule)
        {
            if (iconSetRule != null)
            {
                List<DummyIconSetValue> list = new List<DummyIconSetValue>();
                for (int i = 0; i < iconSetRule.IconCriteria.Length; i++)
                {
                    IconCriterion criterion = iconSetRule.IconCriteria[i];
                    if (((criterion != null) && (criterion.IconValueType == IconValueType.Formula)) && !string.IsNullOrEmpty((string) (criterion.Value as string)))
                    {
                        list.Add(new DummyIconSetValue(iconSetRule, i));
                    }
                    else if (((criterion != null) && !string.IsNullOrEmpty((string) (criterion.Value as string))) && (criterion.Value as string).StartsWith("="))
                    {
                        list.Add(new DummyIconSetValue(iconSetRule, i));
                    }
                }
                if (list.Count > 0)
                {
                    this.iconValues.Add(iconSetRule, list);
                    this.AddConditionsByRange(iconSetRule.Ranges, list.ToArray());
                }
            }
        }

        public void AddRule(FormattingRuleBase rule)
        {
            if (rule is IConditionalFormula)
            {
                this.AddConditionsByRange(rule.Ranges, new IConditionalFormula[] { rule as IConditionalFormula });
            }
            else if (rule is ScaleRule)
            {
                this.AddScaleRuleCondition(rule as ScaleRule);
            }
            else if (rule is CellValueRule)
            {
                this.AddCellValueRuleCondtion(rule as CellValueRule);
            }
            else if (rule is IconSetRule)
            {
                this.AddIconSetRuleCondition(rule as IconSetRule);
            }
            else
            {
                IConditionalFormula[] formulaConditions = rule.FormulaConditions;
                if ((formulaConditions != null) && (formulaConditions.Length > 0))
                {
                    this.AddConditionsByRange(rule.Ranges, formulaConditions);
                }
            }
            rule.ConditionChanged += new EventHandler<ConditionChangedEventArgs>(this.rule_ConditionChanged);
        }

        void AddScaleRuleCondition(ScaleRule scaleRule)
        {
            if (scaleRule != null)
            {
                List<DummyScaleValue> list = new List<DummyScaleValue>();
                for (int i = 0; i < 3; i++)
                {
                    ScaleValue value2 = scaleRule.Scales[i];
                    if (((value2 != null) && (value2.Type == ScaleValueType.Formula)) && !string.IsNullOrEmpty((string) (value2.Value as string)))
                    {
                        list.Add(new DummyScaleValue(scaleRule, i));
                    }
                    else if (((value2 != null) && !string.IsNullOrEmpty((string) (value2.Value as string))) && (value2.Value as string).StartsWith("="))
                    {
                        list.Add(new DummyScaleValue(scaleRule, i));
                    }
                }
                if (list.Count > 0)
                {
                    this.scaleRules.Add(scaleRule, list);
                    this.AddConditionsByRange(scaleRule.Ranges, list.ToArray());
                }
            }
        }

        protected override IFormulaOperatorSource GetExternalManager(ICalcSource source)
        {
            return this;
        }

        protected override void OnInvalidate(List<CalcRangeIdentity> ranges, List<CalcCellIdentity> cells)
        {
        }

        void RemoveCellVauleRuleCondition(CellValueRule cellVauleRule)
        {
            if (this.cellValues.ContainsKey(cellVauleRule))
            {
                this.RemoveConditionsByRange(cellVauleRule.Ranges, this.cellValues[cellVauleRule].ToArray());
            }
        }

        void RemoveCondition(CellRange[] ranges, ConditionBase condition)
        {
            if ((condition != null) && (ranges != null))
            {
                RelationCondition condition2 = condition as RelationCondition;
                if (condition2 != null)
                {
                    this.RemoveCondition(ranges, condition2.Item1);
                    this.RemoveCondition(ranges, condition2.Item2);
                }
                else
                {
                    this.RemoveConditionsByRange(ranges, new IConditionalFormula[] { condition });
                }
            }
        }

        void RemoveConditionsByRange(CellRange[] ranges, params IConditionalFormula[] conditions)
        {
            if (((ranges != null) && (ranges.Length != 0)) && ((conditions != null) && (conditions.Length != 0)))
            {
                for (int i = 0; i < ranges.Length; i++)
                {
                    CalcRangeIdentity actualId = new CalcRangeIdentity(ranges[i].Row, ranges[i].Column, ranges[i].RowCount, ranges[i].ColumnCount);
                    base.RemoveConditionals(actualId, conditions);
                }
            }
        }

        void RemoveIconSetRuleCondition(IconSetRule iconSetRule)
        {
            if (this.iconValues.ContainsKey(iconSetRule))
            {
                this.RemoveConditionsByRange(iconSetRule.Ranges, this.iconValues[iconSetRule].ToArray());
            }
        }

        public void RemoveRule(FormattingRuleBase rule)
        {
            rule.ConditionChanged -= new EventHandler<ConditionChangedEventArgs>(this.rule_ConditionChanged);
            if (rule is IConditionalFormula)
            {
                this.RemoveConditionsByRange(rule.Ranges, new IConditionalFormula[] { rule as IConditionalFormula });
            }
            else if (rule is ScaleRule)
            {
                this.RemoveScaleRuleCondition(rule as ScaleRule);
            }
            else if (rule is CellValueRule)
            {
                this.RemoveCellVauleRuleCondition(rule as CellValueRule);
            }
            else if (rule is IconSetRule)
            {
                this.RemoveIconSetRuleCondition(rule as IconSetRule);
            }
            else
            {
                this.RemoveCondition(rule.Ranges, rule.Condition);
            }
        }

        void RemoveScaleRuleCondition(ScaleRule scaleRule)
        {
            if (this.scaleRules.ContainsKey(scaleRule))
            {
                this.RemoveConditionsByRange(scaleRule.Ranges, this.scaleRules[scaleRule].ToArray());
            }
        }

        void rule_ConditionChanged(object sender, ConditionChangedEventArgs e)
        {
            FormattingRuleBase base2 = sender as FormattingRuleBase;
            if (base2 != null)
            {
                if ((e.OldConditionalFormulas != null) && (e.OldConditionalFormulas.Length > 0))
                {
                    this.RemoveConditionsByRange(base2.Ranges, e.OldConditionalFormulas);
                }
                else if ((e.NewConditionalFormulas != null) && (e.NewConditionalFormulas.Length > 0))
                {
                    this.AddConditionsByRange(base2.Ranges, e.NewConditionalFormulas);
                }
            }
        }

        protected override IMultiSourceProvider MultiSourceProvider
        {
            get { return  null; }
        }

        protected override Dt.Cells.Data.ReferenceStyle ReferenceStyle
        {
            get { return  this.worksheet.ReferenceStyle; }
        }

        protected override ICalcSource Source
        {
            get { return  this.worksheet; }
        }
    }
}

