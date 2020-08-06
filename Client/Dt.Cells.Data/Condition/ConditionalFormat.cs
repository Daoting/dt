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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a format condition collection.
    /// </summary>
    public class ConditionalFormat : IRangeSupport
    {
        WorksheetConditionalGraph conditionalGrpah;
        /// <summary>
        /// The rules.
        /// </summary>
        Collection<FormattingRuleBase> rules;
        /// <summary>
        /// The sheet.
        /// </summary>
        Worksheet worksheet;

        internal event EventHandler<RulesChangedEventArgs> RulesChanged;

        /// <summary>
        /// Creates a new format condition.
        /// </summary>
        /// <param name="worksheet">Sheet</param>
        internal ConditionalFormat(Worksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("sheet");
            }
            this.worksheet = worksheet;
        }

        /// <summary>
        /// Adds an average rule to the rule collection.
        /// </summary>
        /// <param name="type">
        /// The  <see cref="T:Dt.Cells.Data.AverageConditionType" /> average condition type.
        /// </param>
        /// <param name="style">The style that is applied to the cell when the condition is met.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new average rule.</returns>
        public AverageRule AddAverageRule(AverageConditionType type, StyleInfo style, params CellRange[] ranges)
        {
            AverageRule rule = new AverageRule(type, style) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as AverageRule);
        }

        /// <summary>
        /// Adds the cell value rule to the rule collection.
        /// </summary>
        /// <param name="comparisonOperator">
        /// The <see cref="T:Dt.Cells.Data.ComparisonOperator" /> comparison operator.</param>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="style">The style that is set to the cell when the condition is met.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new cell value rule.</returns>
        public CellValueRule AddCellValueRule(ComparisonOperator comparisonOperator, object value1, object value2, StyleInfo style, params CellRange[] ranges)
        {
            CellValueRule rule = new CellValueRule(comparisonOperator, value1, value2, style) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as CellValueRule);
        }

        /// <summary>
        /// Adds a data bar rule to the rule collection.
        /// </summary>
        /// <param name="minType">The minimum scale type.</param>
        /// <param name="minValue">The minimum scale value.</param>
        /// <param name="maxType">The maximum scale type.</param>
        /// <param name="maxValue">The maximum scale value.</param>
        /// <param name="setColor">The color data bar to show on the view.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new data bar rule.</returns>
        public DataBarRule AddDataBarRule(ScaleValueType minType, object minValue, ScaleValueType maxType, object maxValue, Windows.UI.Color setColor, params CellRange[] ranges)
        {
            DataBarRule rule = new DataBarRule(minType, minValue, maxType, maxValue, setColor) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as DataBarRule);
        }

        /// <summary>
        /// Adds the data occurring rule to the rule collection.
        /// </summary>
        /// <param name="type">
        /// The <see cref="T:Dt.Cells.Data.DateOccurringType" /> data occurring type.
        /// </param>
        /// <param name="style">The style that is set to the cell when the condition is met.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new data occurring rule.</returns>
        public DateOccurringRule AddDateOccurringRule(DateOccurringType type, StyleInfo style, params CellRange[] ranges)
        {
            DateOccurringRule rule = new DateOccurringRule(type, style) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as DateOccurringRule);
        }

        /// <summary>
        /// Adds a duplicate rule to the rule collection.
        /// </summary>
        /// <param name="style">The style that is applied to the cell.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new unique rule.</returns>
        public DuplicateRule AddDuplicateRule(StyleInfo style, params CellRange[] ranges)
        {
            DuplicateRule rule = new DuplicateRule(style) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as DuplicateRule);
        }

        /// <summary>
        /// Adds the formula rule to the rule collection.
        /// </summary>
        /// <param name="formula">The condition formula.</param>
        /// <param name="style">The style that is applied to the cell when the condition is met.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new formula rule.</returns>
        public FormulaRule AddFormulaRule(string formula, StyleInfo style, params CellRange[] ranges)
        {
            FormulaRule rule = new FormulaRule(formula, style) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as FormulaRule);
        }

        /// <summary>
        /// Adds an icon set rule to the rule collection.
        /// </summary>
        /// <param name="iconSetTye">The type of <see cref="T:Dt.Cells.Data.IconSetType" /> icon collection.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new icon set rule.</returns>
        public IconSetRule AddIconSetRule(IconSetType iconSetTye, params CellRange[] ranges)
        {
            IconSetRule rule = new IconSetRule(iconSetTye) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as IconSetRule);
        }

        /// <summary>
        /// Adds the rule.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        /// <returns>Returns the new conditional rule.</returns>
        /// <remarks>
        /// This method will modify the priority of the new added rule. The new added rule always has  the highest priority
        /// that is to say, its priority will be 1 and all other existing rules' proierity will be plus 1.
        /// </remarks>
        public FormattingRuleBase AddRule(FormattingRuleBase rule)
        {
            return this.AddRule(rule, true);
        }

        internal FormattingRuleBase AddRule(FormattingRuleBase rule, bool increasePriority)
        {
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            if (increasePriority)
            {
                foreach (FormattingRuleBase local1 in this.Rules)
                {
                    local1.Priority++;
                }
                rule.Priority = 1;
            }
            this.Rules.Add(rule);
            this.ConditionalGraph.AddRule(rule);
            this.RaiseRuleChanged(rule, RulesChangedAction.Add);
            return rule;
        }

        /// <summary>
        /// Adds the text rule to the rule collection.
        /// </summary>
        /// <param name="comparisonOperator">The comparison operator.</param>
        /// <param name="text">The text for comparison.</param>
        /// <param name="style">The style that is set to the cell when the condition is met.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new text rule.</returns>
        public SpecificTextRule AddSpecificTextRule(TextComparisonOperator comparisonOperator, string text, StyleInfo style, params CellRange[] ranges)
        {
            SpecificTextRule rule = new SpecificTextRule(comparisonOperator, text, style) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as SpecificTextRule);
        }

        /// <summary>
        /// Adds the three scale rule to the rule collection.
        /// </summary>
        /// <param name="minType">The minimum scale type.</param>
        /// <param name="minValue">The minimum scale value.</param>
        /// <param name="minColor">The minimum color scale.</param>
        /// <param name="midType">The midpoint scale type.</param>
        /// <param name="midValue">The midpoint scale value.</param>
        /// <param name="midColor">The midpoint scale color.</param>
        /// <param name="maxType">The maximum scale type.</param>
        /// <param name="maxValue">The maximum scale value.</param>
        /// <param name="maxColor">The maximum scale color.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new three color scale rule.</returns>
        public ThreeColorScaleRule AddThreeScaleRule(ScaleValueType minType, object minValue, Windows.UI.Color minColor, ScaleValueType midType, object midValue, Windows.UI.Color midColor, ScaleValueType maxType, object maxValue, Windows.UI.Color maxColor, params CellRange[] ranges)
        {
            ThreeColorScaleRule rule = new ThreeColorScaleRule(minType, minValue, minColor, midType, midValue, midColor, maxType, maxValue, maxColor) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as ThreeColorScaleRule);
        }

        /// <summary>
        /// Adds the top 10 rule or bottom 10 rule to the collection based on the Top10CondtionType.
        /// </summary>
        /// <param name="type">
        /// The <see cref="T:Dt.Cells.Data.Top10ConditionType" /> top 10 condition.
        /// </param>
        /// <param name="rank">The number of top or bottom items to apply the style to.</param>
        /// <param name="style">The style that is applied to the cell when the condition is met.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new top 10 rule.</returns>
        public Top10Rule AddTop10Rule(Top10ConditionType type, int rank, StyleInfo style, params CellRange[] ranges)
        {
            Top10Rule rule = new Top10Rule(type, rank, style) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as Top10Rule);
        }

        /// <summary>
        /// Adds the two color scale rule to the rule collection.
        /// </summary>
        /// <param name="minType">The minimum scale type.</param>
        /// <param name="minValue">The minimum scale value.</param>
        /// <param name="minColor">The minimum scale color.</param>
        /// <param name="maxType">The maximum scale type.</param>
        /// <param name="maxValue">The maximum scale value.</param>
        /// <param name="maxColor">The maximum scale color.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new two color scale rule.</returns>
        public TwoColorScaleRule AddTwoScaleRule(ScaleValueType minType, object minValue, Windows.UI.Color minColor, ScaleValueType maxType, object maxValue, Windows.UI.Color maxColor, params CellRange[] ranges)
        {
            TwoColorScaleRule rule = new TwoColorScaleRule(minType, minValue, minColor, maxType, maxValue, maxColor) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as TwoColorScaleRule);
        }

        /// <summary>
        /// Adds a unique rule to the rule collection.
        /// </summary>
        /// <param name="style">The style that is applied to the cell.</param>
        /// <param name="ranges">The cell ranges where the rule is applied.</param>
        /// <returns>Returns the new unique rule.</returns>
        public UniqueRule AddUniqueRule(StyleInfo style, params CellRange[] ranges)
        {
            UniqueRule rule = new UniqueRule(style) {
                Ranges = ranges
            };
            return (this.AddRule(rule) as UniqueRule);
        }

        internal void ClearCache()
        {
            if (this.rules != null)
            {
                foreach (var item in this.rules)
                {
                    ICachableRule rule = item as ICachableRule;
                    if (rule != null)
                    {
                        rule.ClearCache();
                    }
                }
            }
        }

        /// <summary>
        /// Removes the contents from the cells in the specified range of cells.
        /// </summary>
        /// <param name="row">The row index of the first cell in the range.</param>
        /// <param name="column">The column index of the first cell in the range.</param>
        /// <param name="rowCount">The number of rows in the range.</param>
        /// <param name="columnCount">The number of columns in the range.</param>
        void ClearInternal(int row, int column, int rowCount, int columnCount)
        {
            this.ClearInternal(row, column, rowCount, columnCount, true);
        }

        /// <summary>
        /// Removes the contents from the cells in the specified range of cells.
        /// </summary>
        /// <param name="row">The row index of the first row in the selected range.</param>
        /// <param name="column">The column index of the first column in the selected range.</param>
        /// <param name="rowCount">The number of rows in the selected range.</param>
        /// <param name="columnCount">The number of columns in the selected range.</param>
        /// <param name="removeEmptyRule">If set to <c>true</c> removes the rule that does not contain a cell range.</param>
        void ClearInternal(int row, int column, int rowCount, int columnCount, bool removeEmptyRule)
        {
            List<FormattingRuleBase> list = new List<FormattingRuleBase>();
            if ((this.rules != null) && (this.rules.Count > 0))
            {
                for (int i = 0; i < this.Rules.Count; i++)
                {
                    FormattingRuleBase rule = this.Rules[i];
                    if ((rule != null) && rule.IntersectsInternal(row, column, rowCount, columnCount))
                    {
                        List<CellRange> list2 = new List<CellRange>();
                        if (rule.Ranges != null)
                        {
                            foreach (CellRange range in rule.Ranges)
                            {
                                CellRange[] rangeArray = CellRange.Remove(range, row, column, rowCount, columnCount, this.worksheet.RowCount, this.worksheet.ColumnCount);
                                if ((rangeArray != null) && (rangeArray.Length > 0))
                                {
                                    list2.AddRange(rangeArray);
                                }
                            }
                            if (list2.Count > 0)
                            {
                                rule.Ranges = list2.ToArray();
                                this.RaiseRuleChanged(rule, RulesChangedAction.Resize);
                            }
                            else
                            {
                                list.Add(rule);
                            }
                        }
                    }
                }
            }
            if (removeEmptyRule)
            {
                foreach (FormattingRuleBase base3 in list)
                {
                    this.RemoveRule(base3);
                    this.ConditionalGraph.RemoveRule(base3);
                }
            }
        }

        /// <summary>
        /// Clears the null ref rules.
        /// </summary>
        void ClearNullRefRules()
        {
            if (this.rules != null)
            {
                for (int i = this.rules.Count - 1; i > -1; i--)
                {
                    FormattingRuleBase rule = this.rules[i];
                    if (rule.HasNoReference)
                    {
                        this.RemoveRule(rule);
                    }
                }
            }
        }

        /// <summary>
        /// Removes all rules.
        /// </summary>
        public void ClearRule()
        {
            if (this.Rules.Count > 0)
            {
                foreach (FormattingRuleBase base2 in this.Rules)
                {
                    this.RaiseRuleChanged(base2, RulesChangedAction.Remove);
                }
            }
            this.Rules.Clear();
            this.conditionalGrpah = null;
        }

        /// <summary>
        /// Determines whether the specified cell contains a specified rule.
        /// </summary>
        /// <param name="rule">The rule for which to check.</param>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>
        /// <c>true</c> if the specified cell contains the rule; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsRule(FormattingRuleBase rule, int row, int column)
        {
            return (((rule != null) && this.Rules.Contains(rule)) && rule.Contains(row, column));
        }

        /// <summary>
        /// Copies a range of cells and pastes it into a range of cells at the specified location.
        /// </summary>
        /// <param name="fromRow">Row index from which to begin copying</param>
        /// <param name="fromColumn">Column index from which to begin copying</param>
        /// <param name="toRow">Row index at which to paste the cell range</param>
        /// <param name="toColumn">Column index at which to paste the cell range</param>
        /// <param name="rowCount">Number of rows to copy</param>
        /// <param name="columnCount">Number of columns to copy</param>
        void CopyInternal(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if ((this.rules != null) && (this.rules.Count > 0))
            {
                CellRange range = new CellRange(fromRow, fromColumn, rowCount, columnCount);
                List<FormattingRuleBase> list = new List<FormattingRuleBase>();
                for (int i = 0; i < this.Rules.Count; i++)
                {
                    FormattingRuleBase base2 = this.Rules[i];
                    if ((base2 != null) && base2.IntersectsInternal(fromRow, fromColumn, rowCount, columnCount))
                    {
                        List<CellRange> list2 = new List<CellRange>();
                        foreach (CellRange range2 in base2.Ranges)
                        {
                            int x = toColumn - fromColumn;
                            int y = toRow - fromRow;
                            CellRange cellRange = CellRange.GetIntersect(range, range2, this.worksheet.RowCount, this.worksheet.ColumnCount);
                            if (cellRange != null)
                            {
                                CellRange range4 = CellRange.Offset(cellRange, x, y);
                                list2.Add(range4);
                            }
                        }
                        if (list2.Count > 0)
                        {
                            FormattingRuleBase base3 = base2.Clone() as FormattingRuleBase;
                            base3.Ranges = list2.ToArray();
                            list.Add(base3);
                        }
                    }
                }
                foreach (FormattingRuleBase base4 in list)
                {
                    this.AddRule(base4);
                }
            }
        }

        /// <summary>
        /// Gets the conditional rules from the cell at the specified row and column.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>The rules for the specified cell.</returns>
        public FormattingRuleBase[] GetRules(int row, int column)
        {
            int modelRowFromViewRow = this.worksheet.GetModelRowFromViewRow(row);
            int modelColumnFromViewColumn = this.worksheet.GetModelColumnFromViewColumn(column);
            List<FormattingRuleBase> list = null;
            if (this.rules != null)
            {
                foreach (FormattingRuleBase base2 in this.rules)
                {
                    if ((base2 != null) && base2.IntersectsInternal(modelRowFromViewRow, modelColumnFromViewColumn, 1, 1))
                    {
                        if (list == null)
                        {
                            list = new List<FormattingRuleBase>();
                        }
                        list.Add(base2);
                    }
                }
            }
            if (list != null)
            {
                return list.ToArray();
            }
            return new FormattingRuleBase[0];
        }

        /// <summary>
        /// Adds columns of cells after the specified column.
        /// </summary>
        /// <param name="column">The column index of the column after which to add columns.</param>
        /// <param name="columnCount">The number of columns to add.</param>
        void IRangeSupport.AddColumns(int column, int columnCount)
        {
            if (this.rules != null)
            {
                foreach (FormattingRuleBase base2 in this.rules)
                {
                    if (base2 != null)
                    {
                        base2.AddColumns(this.worksheet.GetModelColumnFromViewColumn(column), columnCount);
                    }
                }
                this.ConditionalGraph.Insert(column, columnCount, false, false);
            }
        }

        /// <summary>
        /// Adds rows of cells after the specified row.
        /// </summary>
        /// <param name="row">The row index of the row after which to add rows.</param>
        /// <param name="rowCount">The number of rows to add.</param>
        void IRangeSupport.AddRows(int row, int rowCount)
        {
            if ((this.rules != null) && (this.worksheet != null))
            {
                foreach (FormattingRuleBase base2 in this.rules)
                {
                    if (base2 != null)
                    {
                        base2.AddRows(this.worksheet.GetModelRowFromViewRow(row), rowCount);
                    }
                }
                this.ConditionalGraph.Insert(row, rowCount, true, false);
            }
        }

        /// <summary>
        /// Removes all the contents from the cells in the specified range of cells.
        /// </summary>
        /// <param name="row">The row index of the first cell in the range.</param>
        /// <param name="column">The column index of the first cell in the range.</param>
        /// <param name="rowCount">The number of rows in the range.</param>
        /// <param name="columnCount">The number of columns in the range.</param>
        void IRangeSupport.Clear(int row, int column, int rowCount, int columnCount)
        {
            CellRange range = new CellRange(row, column, rowCount, columnCount);
            this.ClearInternal(range.Row, range.Column, range.RowCount, range.ColumnCount);
        }

        /// <summary>
        /// Copies a range of cells and pastes it into a range of cells at the specified location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin copying.</param>
        /// <param name="fromColumn">The column index from which to begin copying.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to copy.</param>
        /// <param name="columnCount">The number of columns to copy.</param>
        void IRangeSupport.Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (this.worksheet.IsColumnSorted || this.worksheet.IsRowSorted)
            {
                if (((fromRow == -1) && (toRow == -1)) && ((fromColumn != -1) && (toColumn != -1)))
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        int modelColumnFromViewColumn = -1;
                        int num3 = -1;
                        if (fromColumn < toColumn)
                        {
                            modelColumnFromViewColumn = this.worksheet.GetModelColumnFromViewColumn(((fromColumn + columnCount) - i) - 1);
                            num3 = this.worksheet.GetModelColumnFromViewColumn(((toColumn + columnCount) - i) - 1);
                        }
                        else
                        {
                            modelColumnFromViewColumn = this.worksheet.GetModelColumnFromViewColumn(fromColumn + i);
                            num3 = this.worksheet.GetModelColumnFromViewColumn(toColumn + i);
                        }
                        this.CopyInternal(-1, modelColumnFromViewColumn, -1, num3, 0, 1);
                    }
                }
                else if (((fromRow != -1) && (toRow != -1)) && ((fromColumn == -1) && (toColumn == -1)))
                {
                    for (int j = 0; j < rowCount; j++)
                    {
                        int modelRowFromViewRow = -1;
                        int num6 = -1;
                        if (fromColumn < toColumn)
                        {
                            modelRowFromViewRow = this.worksheet.GetModelRowFromViewRow(((fromRow + rowCount) - j) - 1);
                            num6 = this.worksheet.GetModelRowFromViewRow(((toRow + rowCount) - j) - 1);
                        }
                        else
                        {
                            modelRowFromViewRow = this.worksheet.GetModelRowFromViewRow(fromRow + j);
                            num6 = this.worksheet.GetModelRowFromViewRow(toRow + j);
                        }
                        this.CopyInternal(modelRowFromViewRow, -1, num6, -1, 1, 0);
                    }
                }
                else if (((fromRow != -1) && (toRow != -1)) && ((fromColumn != -1) && (toColumn != -1)))
                {
                    for (int k = 0; k < rowCount; k++)
                    {
                        int num8 = -1;
                        int num9 = -1;
                        if (fromColumn < toColumn)
                        {
                            num8 = this.worksheet.GetModelRowFromViewRow(((fromRow + rowCount) - k) - 1);
                            num9 = this.worksheet.GetModelRowFromViewRow(((toRow + rowCount) - k) - 1);
                        }
                        else
                        {
                            num8 = this.worksheet.GetModelRowFromViewRow(fromRow + k);
                            num9 = this.worksheet.GetModelRowFromViewRow(toRow + k);
                        }
                        for (int m = 0; m < columnCount; m++)
                        {
                            int num11 = -1;
                            int num12 = -1;
                            if (fromColumn < toColumn)
                            {
                                num11 = this.worksheet.GetModelColumnFromViewColumn(((fromColumn + columnCount) - m) - 1);
                                num12 = this.worksheet.GetModelColumnFromViewColumn(((toColumn + columnCount) - m) - 1);
                            }
                            else
                            {
                                num11 = this.worksheet.GetModelColumnFromViewColumn(fromColumn + m);
                                num12 = this.worksheet.GetModelColumnFromViewColumn(toColumn + m);
                            }
                            this.CopyInternal(num8, num11, num9, num12, 1, 1);
                        }
                    }
                }
            }
            else
            {
                this.CopyInternal(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
            }
        }

        /// <summary>
        /// Moves a range of cells and pastes it into a range of cells at the specified location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the move.</param>
        /// <param name="fromColumn">The column index from which to begin the move.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to move.</param>
        /// <param name="columnCount">The number of columns to move.</param>
        void IRangeSupport.Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.Move(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount, false);
        }

        /// <summary>
        /// Removes columns from the specified starting position.
        /// </summary>
        /// <param name="column">The column index at which to start removing columns.</param>
        /// <param name="columnCount">The number of columns to remove.</param>
        void IRangeSupport.RemoveColumns(int column, int columnCount)
        {
            if (this.rules != null)
            {
                foreach (FormattingRuleBase base2 in this.rules)
                {
                    if (base2 != null)
                    {
                        if (this.worksheet.IsColumnSorted)
                        {
                            for (int i = 0; i < columnCount; i++)
                            {
                                base2.RemoveColumns(this.worksheet.GetModelColumnFromViewColumn(column), columnCount);
                            }
                        }
                        else
                        {
                            base2.RemoveColumns(column, columnCount);
                        }
                    }
                }
                this.ClearNullRefRules();
                this.ConditionalGraph.Remove(column, columnCount, false, false);
            }
        }

        /// <summary>
        /// Removes rows from the specified starting position.
        /// </summary>
        /// <param name="row">The row index at which to start removing rows.</param>
        /// <param name="rowCount">The number of rows to remove.</param>
        void IRangeSupport.RemoveRows(int row, int rowCount)
        {
            if ((this.rules != null) && (this.worksheet != null))
            {
                foreach (FormattingRuleBase base2 in this.rules)
                {
                    if (base2 != null)
                    {
                        if (this.worksheet.IsRowSorted)
                        {
                            for (int i = 0; i < rowCount; i++)
                            {
                                base2.RemoveRows(this.worksheet.GetModelRowFromViewRow(row), 1);
                            }
                        }
                        else
                        {
                            base2.RemoveRows(row, rowCount);
                        }
                    }
                }
                this.ClearNullRefRules();
                this.ConditionalGraph.Remove(row, rowCount, true, false);
            }
        }

        /// <summary>
        /// Swaps a range of cells from one location to another.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the swap.</param>
        /// <param name="fromColumn">The column index from which to begin the swap.</param>
        /// <param name="toRow">The row index of the destination range.</param>
        /// <param name="toColumn">The column index of the destination range.</param>
        /// <param name="rowCount">The number of rows to swap.</param>
        /// <param name="columnCount">The number of columns to swap.</param>
        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (this.worksheet.IsColumnSorted || this.worksheet.IsRowSorted)
            {
                if (((fromRow == -1) && (toRow == -1)) && ((fromColumn != -1) && (toColumn != -1)))
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        int modelColumnFromViewColumn = -1;
                        int num3 = -1;
                        if (fromColumn < toColumn)
                        {
                            modelColumnFromViewColumn = this.worksheet.GetModelColumnFromViewColumn(((fromColumn + columnCount) - i) - 1);
                            num3 = this.worksheet.GetModelColumnFromViewColumn(((toColumn + columnCount) - i) - 1);
                        }
                        else
                        {
                            modelColumnFromViewColumn = this.worksheet.GetModelColumnFromViewColumn(fromColumn + i);
                            num3 = this.worksheet.GetModelColumnFromViewColumn(toColumn + i);
                        }
                        this.SwapInternal(-1, modelColumnFromViewColumn, -1, num3, 0, 1);
                    }
                }
                else if (((fromRow != -1) && (toRow != -1)) && ((fromColumn == -1) && (toColumn == -1)))
                {
                    for (int j = 0; j < rowCount; j++)
                    {
                        int modelRowFromViewRow = -1;
                        int num6 = -1;
                        if (fromColumn < toColumn)
                        {
                            modelRowFromViewRow = this.worksheet.GetModelRowFromViewRow(((fromRow + rowCount) - j) - 1);
                            num6 = this.worksheet.GetModelRowFromViewRow(((toRow + rowCount) - j) - 1);
                        }
                        else
                        {
                            modelRowFromViewRow = this.worksheet.GetModelRowFromViewRow(fromRow + j);
                            num6 = this.worksheet.GetModelRowFromViewRow(toRow + j);
                        }
                        this.SwapInternal(modelRowFromViewRow, -1, num6, -1, 1, 0);
                    }
                }
                else if (((fromRow != -1) && (toRow != -1)) && ((fromColumn != -1) && (toColumn != -1)))
                {
                    for (int k = 0; k < rowCount; k++)
                    {
                        int num8 = -1;
                        int num9 = -1;
                        if (fromColumn < toColumn)
                        {
                            num8 = this.worksheet.GetModelRowFromViewRow(((fromRow + rowCount) - k) - 1);
                            num9 = this.worksheet.GetModelRowFromViewRow(((toRow + rowCount) - k) - 1);
                        }
                        else
                        {
                            num8 = this.worksheet.GetModelRowFromViewRow(fromRow + k);
                            num9 = this.worksheet.GetModelRowFromViewRow(toRow + k);
                        }
                        for (int m = 0; m < columnCount; m++)
                        {
                            int num11 = -1;
                            int num12 = -1;
                            if (fromColumn < toColumn)
                            {
                                num11 = this.worksheet.GetModelColumnFromViewColumn(((fromColumn + columnCount) - m) - 1);
                                num12 = this.worksheet.GetModelColumnFromViewColumn(((toColumn + columnCount) - m) - 1);
                            }
                            else
                            {
                                num11 = this.worksheet.GetModelColumnFromViewColumn(fromColumn + m);
                                num12 = this.worksheet.GetModelColumnFromViewColumn(toColumn + m);
                            }
                            this.SwapInternal(num8, num11, num9, num12, 1, 1);
                        }
                    }
                }
            }
            else
            {
                this.SwapInternal(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
            }
        }

        /// <summary>
        /// Moves a range of cells and pastes it into a range of cells at the specified location.
        /// </summary>
        /// <param name="fromRow">Row index from which to begin the move.</param>
        /// <param name="fromColumn">Column index from which to begin the move.</param>
        /// <param name="toRow">Row index at which to paste the cell range.</param>
        /// <param name="toColumn">Column index at which to paste the cell range.</param>
        /// <param name="rowCount">Number of rows to move.</param>
        /// <param name="columnCount">Number of columns to move.</param>
        /// <param name="removeEmptyRule"></param>
        internal void Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount, bool removeEmptyRule)
        {
            if (this.worksheet.IsColumnSorted || this.worksheet.IsRowSorted)
            {
                if (((fromRow == -1) && (toRow == -1)) && ((fromColumn != -1) && (toColumn != -1)))
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        int modelColumnFromViewColumn = -1;
                        int num3 = -1;
                        if (fromColumn < toColumn)
                        {
                            modelColumnFromViewColumn = this.worksheet.GetModelColumnFromViewColumn(((fromColumn + columnCount) - i) - 1);
                            num3 = this.worksheet.GetModelColumnFromViewColumn(((toColumn + columnCount) - i) - 1);
                        }
                        else
                        {
                            modelColumnFromViewColumn = this.worksheet.GetModelColumnFromViewColumn(fromColumn + i);
                            num3 = this.worksheet.GetModelColumnFromViewColumn(toColumn + i);
                        }
                        this.MoveInternal(-1, modelColumnFromViewColumn, -1, num3, 0, 1, removeEmptyRule);
                    }
                }
                else if (((fromRow != -1) && (toRow != -1)) && ((fromColumn == -1) && (toColumn == -1)))
                {
                    for (int j = 0; j < rowCount; j++)
                    {
                        int modelRowFromViewRow = -1;
                        int num6 = -1;
                        if (fromColumn < toColumn)
                        {
                            modelRowFromViewRow = this.worksheet.GetModelRowFromViewRow(((fromRow + rowCount) - j) - 1);
                            num6 = this.worksheet.GetModelRowFromViewRow(((toRow + rowCount) - j) - 1);
                        }
                        else
                        {
                            modelRowFromViewRow = this.worksheet.GetModelRowFromViewRow(fromRow + j);
                            num6 = this.worksheet.GetModelRowFromViewRow(toRow + j);
                        }
                        this.MoveInternal(modelRowFromViewRow, -1, num6, -1, 1, 0, removeEmptyRule);
                    }
                }
                else if (((fromRow != -1) && (toRow != -1)) && ((fromColumn != -1) && (toColumn != -1)))
                {
                    for (int k = 0; k < rowCount; k++)
                    {
                        int num8 = -1;
                        int num9 = -1;
                        if (fromColumn < toColumn)
                        {
                            num8 = this.worksheet.GetModelRowFromViewRow(((fromRow + rowCount) - k) - 1);
                            num9 = this.worksheet.GetModelRowFromViewRow(((toRow + rowCount) - k) - 1);
                        }
                        else
                        {
                            num8 = this.worksheet.GetModelRowFromViewRow(fromRow + k);
                            num9 = this.worksheet.GetModelRowFromViewRow(toRow + k);
                        }
                        for (int m = 0; m < columnCount; m++)
                        {
                            int num11 = -1;
                            int num12 = -1;
                            if (fromColumn < toColumn)
                            {
                                num11 = this.worksheet.GetModelColumnFromViewColumn(((fromColumn + columnCount) - m) - 1);
                                num12 = this.worksheet.GetModelColumnFromViewColumn(((toColumn + columnCount) - m) - 1);
                            }
                            else
                            {
                                num11 = this.worksheet.GetModelColumnFromViewColumn(fromColumn + m);
                                num12 = this.worksheet.GetModelColumnFromViewColumn(toColumn + m);
                            }
                            this.MoveInternal(num8, num11, num9, num12, 1, 1, removeEmptyRule);
                        }
                    }
                }
            }
            else
            {
                this.MoveInternal(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount, removeEmptyRule);
            }
        }

        /// <summary>
        /// Moves a range of cells and pastes it into a range of cells at the specified location.
        /// </summary>
        /// <param name="fromRow">Row index from which to begin the move</param>
        /// <param name="fromColumn">Column index from which to begin the move</param>
        /// <param name="toRow">Row index at which to paste the cell range</param>
        /// <param name="toColumn">Column index at which to paste the cell range</param>
        /// <param name="rowCount">Number of rows to move</param>
        /// <param name="columnCount">Number of columns to move</param>
        /// <param name="removeEmptyRule"></param>
        void MoveInternal(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount, bool removeEmptyRule)
        {
            List<FormattingRuleBase> list = new List<FormattingRuleBase>();
            new List<FormattingRuleBase>();
            List<KeyValuePair<FormattingRuleBase, List<CellRange>>> list2 = new List<KeyValuePair<FormattingRuleBase, List<CellRange>>>();
            if ((this.rules != null) && (this.rules.Count > 0))
            {
                CellRange range = new CellRange(fromRow, fromColumn, rowCount, columnCount);
                for (int i = 0; i < this.Rules.Count; i++)
                {
                    FormattingRuleBase base2 = this.Rules[i];
                    if ((base2 != null) && base2.IntersectsInternal(fromRow, fromColumn, rowCount, columnCount))
                    {
                        List<CellRange> list3 = new List<CellRange>();
                        List<CellRange> list4 = new List<CellRange>();
                        foreach (CellRange range2 in base2.Ranges)
                        {
                            int x = toColumn - fromColumn;
                            int y = toRow - fromRow;
                            CellRange cellRange = CellRange.GetIntersect(range, range2, this.worksheet.RowCount, this.worksheet.ColumnCount);
                            if (cellRange != null)
                            {
                                CellRange range4 = CellRange.Offset(cellRange, x, y);
                                list4.Add(range4);
                                CellRange[] cellRanges = CellRange.Remove(range2, toRow, toColumn, rowCount, columnCount, this.worksheet.RowCount, this.worksheet.ColumnCount);
                                CellRange[] rangeArray2 = (cellRanges != null) ? CellRange.Remove(cellRanges, fromRow, fromColumn, rowCount, columnCount, this.worksheet.RowCount, this.worksheet.ColumnCount) : null;
                                if ((rangeArray2 != null) && (rangeArray2.Length > 0))
                                {
                                    list3.AddRange(rangeArray2);
                                }
                            }
                            else
                            {
                                list3.Add(range2);
                            }
                        }
                        if (list4.Count > 0)
                        {
                            foreach (CellRange range5 in list4)
                            {
                                list3.Add(range5);
                            }
                        }
                        if (list3.Count > 0)
                        {
                            list2.Add(new KeyValuePair<FormattingRuleBase, List<CellRange>>(base2, list3));
                        }
                        else
                        {
                            list.Add(base2);
                        }
                    }
                    else
                    {
                        this.ClearInternal(toRow, toColumn, rowCount, columnCount, removeEmptyRule);
                    }
                }
            }
            foreach (KeyValuePair<FormattingRuleBase, List<CellRange>> pair in list2)
            {
                FormattingRuleBase introduced22 = pair.Key;
                introduced22.Ranges = (pair.Value != null) ? pair.Value.ToArray() : null;
                this.RaiseRuleChanged(pair.Key, RulesChangedAction.Resize);
            }
            foreach (FormattingRuleBase base3 in list)
            {
                this.RemoveRule(base3);
            }
        }

        /// <summary>
        /// Raises the rule changed.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="changedAction">The rule changed action</param>
        void RaiseRuleChanged(FormattingRuleBase rule, RulesChangedAction changedAction)
        {
            if (this.RulesChanged != null)
            {
                this.RulesChanged(this, new RulesChangedEventArgs(rule, changedAction));
            }
        }

        /// <summary>
        /// Removes the rules for a specified cell range.
        /// </summary>
        /// <param name="range">The cell range.</param>
        public void RemoveRule(CellRange range)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            ((IRangeSupport) this).Clear(range.Row, range.Column, range.RowCount, range.ColumnCount);
        }

        /// <summary>
        /// Removes a rule object from the ConditionalFormats object.
        /// </summary>
        /// <param name="rule">Rule object to remove from the ConditionalFormats object.</param>
        public void RemoveRule(FormattingRuleBase rule)
        {
            if (rule != null)
            {
                this.Rules.Remove(rule);
                this.ConditionalGraph.RemoveRule(rule);
                this.RaiseRuleChanged(rule, RulesChangedAction.Remove);
            }
        }

        /// <summary>
        /// Removes the rules for a specified cell.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        public void RemoveRule(int row, int column)
        {
            ((IRangeSupport) this).Clear(row, column, 1, 1);
        }

        /// <summary>
        /// Removes the rules for a specified cell range.
        /// </summary>
        /// <param name="row">The row index of the first cell in the range.</param>
        /// <param name="column">The column index of the first cell in the range.</param>
        /// <param name="rowCount">The number of rows in the range.</param>
        /// <param name="columnCount">The number of columns in the range.</param>
        public void RemoveRule(int row, int column, int rowCount, int columnCount)
        {
            ((IRangeSupport) this).Clear(row, column, rowCount, columnCount);
        }

        /// <summary>
        /// Swaps a range of cells from one specified location to another.
        /// </summary>
        /// <param name="fromRow">Row index from which to begin swap</param>
        /// <param name="fromColumn">Column index from which to begin swap</param>
        /// <param name="toRow">Row index of the destination range</param>
        /// <param name="toColumn">Column index of the destination range</param>
        /// <param name="rowCount">Number of rows to swap</param>
        /// <param name="columnCount">Number of columns to swap</param>
        void SwapInternal(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            new List<FormattingRuleBase>();
            new List<FormattingRuleBase>();
            List<FormattingRuleBase> list = new List<FormattingRuleBase>();
            List<KeyValuePair<FormattingRuleBase, List<CellRange>>> list2 = new List<KeyValuePair<FormattingRuleBase, List<CellRange>>>();
            if ((this.rules != null) && (this.rules.Count > 0))
            {
                CellRange range = new CellRange(fromRow, fromColumn, rowCount, columnCount);
                for (int i = 0; i < this.Rules.Count; i++)
                {
                    FormattingRuleBase base2 = this.Rules[i];
                    if ((base2 != null) && base2.IntersectsInternal(fromRow, fromColumn, rowCount, columnCount))
                    {
                        List<CellRange> list3 = new List<CellRange>();
                        List<CellRange> list4 = new List<CellRange>();
                        foreach (CellRange range2 in base2.Ranges)
                        {
                            int x = toColumn - fromColumn;
                            int y = toRow - fromRow;
                            CellRange cellRange = CellRange.GetIntersect(range, range2, this.worksheet.RowCount, this.worksheet.ColumnCount);
                            if (cellRange != null)
                            {
                                CellRange range4 = CellRange.Offset(cellRange, x, y);
                                list4.Add(range4);
                                CellRange[] rangeArray = CellRange.Remove(range2, fromRow, fromColumn, rowCount, columnCount, this.worksheet.RowCount, this.worksheet.ColumnCount);
                                if ((rangeArray != null) && (rangeArray.Length > 0))
                                {
                                    list3.AddRange(rangeArray);
                                }
                            }
                            else
                            {
                                list3.Add(range2);
                            }
                        }
                        foreach (CellRange range5 in list4)
                        {
                            list3.Add(range5);
                        }
                        if (list3.Count > 0)
                        {
                            list2.Add(new KeyValuePair<FormattingRuleBase, List<CellRange>>(base2, list3));
                        }
                        else
                        {
                            list.Add(base2);
                        }
                    }
                }
                CellRange range6 = new CellRange(toRow, toColumn, rowCount, columnCount);
                for (int j = 0; j < this.Rules.Count; j++)
                {
                    FormattingRuleBase base3 = this.Rules[j];
                    if ((base3 != null) && base3.IntersectsInternal(toRow, toColumn, rowCount, columnCount))
                    {
                        List<CellRange> list5 = new List<CellRange>();
                        List<CellRange> list6 = new List<CellRange>();
                        foreach (CellRange range7 in base3.Ranges)
                        {
                            int num5 = fromColumn - toColumn;
                            int num6 = fromRow - toRow;
                            CellRange range8 = CellRange.GetIntersect(range6, range7, this.worksheet.RowCount, this.worksheet.ColumnCount);
                            if (range8 != null)
                            {
                                CellRange range9 = CellRange.Offset(range8, num5, num6);
                                list6.Add(range9);
                                CellRange[] rangeArray2 = CellRange.Remove(range7, toRow, toColumn, rowCount, columnCount, this.worksheet.RowCount, this.worksheet.ColumnCount);
                                if ((rangeArray2 != null) && (rangeArray2.Length > 0))
                                {
                                    list5.AddRange(rangeArray2);
                                }
                            }
                            else
                            {
                                list5.Add(range7);
                            }
                        }
                        foreach (CellRange range10 in list6)
                        {
                            list5.Add(range10);
                        }
                        if (list5.Count > 0)
                        {
                            list2.Add(new KeyValuePair<FormattingRuleBase, List<CellRange>>(base3, list5));
                        }
                        else
                        {
                            list.Add(base3);
                        }
                    }
                }
            }
            foreach (FormattingRuleBase base4 in list)
            {
                this.RemoveRule(base4);
            }
            foreach (KeyValuePair<FormattingRuleBase, List<CellRange>> pair in list2)
            {
                pair.Key.Ranges = new CellRange[0];
                this.RaiseRuleChanged(pair.Key, RulesChangedAction.Resize);
            }
            foreach (KeyValuePair<FormattingRuleBase, List<CellRange>> pair2 in list2)
            {
                if (pair2.Value != null)
                {
                    List<CellRange> list7 = new List<CellRange>();
                    list7.AddRange(pair2.Key.Ranges);
                    list7.AddRange(pair2.Value);
                    pair2.Key.Ranges = list7.ToArray();
                }
                this.RaiseRuleChanged(pair2.Key, RulesChangedAction.Resize);
            }
        }

        /// <summary>
        /// Copies the rules to a new array.
        /// </summary>
        /// <returns></returns>
        public FormattingRuleBase[] ToArray()
        {
            FormattingRuleBase[] baseArray = new FormattingRuleBase[this.RuleCount];
            this.Rules.CopyTo(baseArray, 0);
            return baseArray;
        }

        internal WorksheetConditionalGraph ConditionalGraph
        {
            get
            {
                if (this.conditionalGrpah == null)
                {
                    this.conditionalGrpah = new WorksheetConditionalGraph(this.worksheet);
                }
                return this.conditionalGrpah;
            }
        }

        /// <summary>
        /// Gets the rule at the specified index.
        /// </summary>
        /// <param name="index">Index of the rule to return.</param>
        /// <value>The specified rule at the specified index.</value>
        public FormattingRuleBase this[int index]
        {
            get { return  this.Rules[index]; }
        }

        /// <summary>
        /// Gets the rule collection at the specified cell location.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <value>The rule collection at the specified location.</value>
        public FormattingRuleBase[] this[int row, int column]
        {
            get { return  this.GetRules(row, column); }
        }

        /// <summary>
        /// Gets the number of rule objects in the collection.
        /// </summary>
        /// <value>The number of rule objects in the collection. The default value is 0.</value>
        [DefaultValue(0)]
        public int RuleCount
        {
            get { return  this.Rules.Count; }
        }

        /// <summary>
        /// Gets the rules.
        /// </summary>
        /// <value>The rules.</value>
        Collection<FormattingRuleBase> Rules
        {
            get
            {
                if (this.rules == null)
                {
                    this.rules = new Collection<FormattingRuleBase>();
                }
                return this.rules;
            }
        }
    }
}

