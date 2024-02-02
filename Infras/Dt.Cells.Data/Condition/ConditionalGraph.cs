#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using Dt.CalcEngine.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal abstract class ConditionalGraph : IFormulaOperatorSource
    {
        CalcCalculationManager _calculationManager;
        Dictionary<CalcLocalIdentity, ConditionIdStorage> _conditionals = new Dictionary<CalcLocalIdentity, ConditionIdStorage>();
        Dictionary<CalcLocalIdentity, CalcLocalIdentity> _dirtyList = new Dictionary<CalcLocalIdentity, CalcLocalIdentity>();
        ConditionalCalcGraph _graph;
        int _suspendCount;

        public ConditionalGraph()
        {
            this._graph = new ConditionalCalcGraph(this);
        }

        public void AddConditionals(CalcLocalIdentity actualId, ICollection<IConditionalFormula> conditionals)
        {
            ConditionIdStorage storage;
            if (!this._conditionals.TryGetValue(actualId, out storage))
            {
                storage = new ConditionIdStorage(actualId);
                this._conditionals[actualId] = storage;
            }
            CalcCellIdentity baseCell = this.GetBaseCell(actualId);
            CalcEvaluatorContext evaluatorContext = this.CalculationManager.Source.GetEvaluatorContext(baseCell);
            foreach (IConditionalFormula formula in conditionals)
            {
                CalcExpression expression = this.Formula2Expression(formula.Formula, baseCell.RowIndex, baseCell.ColumnIndex);
                ConditionalIdentity id = storage.AddConditional(formula, expression);
                if (id != null)
                {
                    this._graph.SetConditionalNode(id, expression, evaluatorContext, this.CalculationManager.Source);
                }
            }
        }

        void AddDirty(CalcNode dp)
        {
            ConditionalIdentity id = dp.Id as ConditionalIdentity;
            CalcLocalIdentity identity2 = dp.Id as CalcLocalIdentity;
            if ((identity2 != null) && (dp.Source != this.Source))
            {
                (((IFormulaOperatorSource) this).GetExternalManager(dp.Source) as ConditionalGraph).InvalidateCore(identity2);
            }
            else if (id != null)
            {
                this._dirtyList[id.ActualIdentity] = id.ActualIdentity;
            }
            else if (identity2 != null)
            {
                this._dirtyList[identity2] = identity2;
            }
        }

        public void ClearConditionals()
        {
            this._graph = new ConditionalCalcGraph(this);
            this._dirtyList.Clear();
            this._conditionals.Clear();
        }

        public void ClearNode(CalcLocalIdentity id)
        {
        }

        protected string Expression2Formula(CalcExpression expression, int baseRow, int baseColumn)
        {
            baseRow = (baseRow == -1) ? 0 : baseRow;
            baseColumn = (baseColumn == -1) ? 0 : baseColumn;
            CalcParserContext context = new CalcParserContext(this.UserR1C1, baseRow, baseColumn, CultureInfo.CurrentCulture);
            return this.CalculationManager.Parser.Unparse(expression, context);
        }

        protected CalcExpression Formula2Expression(string formula, int baseRow, int baseColumn)
        {
            baseRow = (baseRow == -1) ? 0 : baseRow;
            baseColumn = (baseColumn == -1) ? 0 : baseColumn;
            CalcParserContext context = new CalcParserContext(this.UserR1C1, baseRow, baseColumn, CultureInfo.CurrentCulture);
            return this.CalculationManager.Parser.Parse(formula, context);
        }

        CalcCellIdentity GetBaseCell(CalcLocalIdentity id)
        {
            int rowIndex;
            int columnIndex;
            if (id is CalcCellIdentity)
            {
                rowIndex = (id as CalcCellIdentity).RowIndex;
                columnIndex = (id as CalcCellIdentity).ColumnIndex;
            }
            else
            {
                CalcRangeIdentity identity = id as CalcRangeIdentity;
                rowIndex = identity.IsFullColumn ? 0 : identity.RowIndex;
                columnIndex = identity.IsFullRow ? 0 : identity.ColumnIndex;
            }
            return new CalcCellIdentity(rowIndex, columnIndex);
        }

        protected abstract IFormulaOperatorSource GetExternalManager(ICalcSource source);

        public List<ConditionalIdentity> GetFormulas(CalcLocalIdentity actualId)
        {
            ConditionIdStorage storage;
            List<ConditionalIdentity> list = new List<ConditionalIdentity>();
            if (this._conditionals.TryGetValue(actualId, out storage))
            {
                foreach (ConditionalIdentity identity in storage.Conditions)
                {
                    list.Add(identity);
                }
            }
            return list;
        }

        public CalcNode GetNode(CalcIdentity id)
        {
            return this._graph.GetNode(id);
        }

        void IFormulaOperatorSource.ClearExpression(CalcLocalIdentity id)
        {
            ConditionalIdentity identity = id as ConditionalIdentity;
            if (identity != null)
            {
                this.GetBaseCell(identity.ActualIdentity);
                this._graph.RemoveNode(identity);
                if (this._conditionals.ContainsKey(identity.ActualIdentity))
                {
                    ConditionIdStorage storage = this._conditionals[identity.ActualIdentity];
                    storage.RemoveConditional(identity.Condition);
                    if (storage.Conditions.Count == 0)
                    {
                        this._conditionals.Remove(identity.ActualIdentity);
                    }
                }
            }
        }

        IEnumerable<CalcLocalIdentity> IFormulaOperatorSource.GetAllLocalIdentities()
        {
            return this._graph.GetAllLocalIdentities();
        }

        CalcExpression IFormulaOperatorSource.GetExpression(CalcLocalIdentity id)
        {
            if (id is ConditionalIdentity)
            {
                return (id as ConditionalIdentity).Expression;
            }
            return null;
        }

        IFormulaOperatorSource IFormulaOperatorSource.GetExternalManager(ICalcSource source)
        {
            return this.GetExternalManager(source);
        }

        void IFormulaOperatorSource.Invalidate(CalcLocalIdentity id, bool autoCalculate)
        {
            if (id is ConditionalIdentity)
            {
                this.InvalidateCore(id);
            }
            else if (id is CalcCellIdentity)
            {
                this.Invalidate(id as CalcCellIdentity);
            }
            else
            {
                this.Invalidate(id as CalcRangeIdentity);
            }
        }

        public void Invalidate(IEnumerable<CalcLocalIdentity> ids, bool autoCalculate)
        {
        }

        void IFormulaOperatorSource.SetExpression(CalcLocalIdentity id, CalcExpression expr)
        {
            ConditionalIdentity identity = id as ConditionalIdentity;
            if (identity != null)
            {
                ConditionIdStorage storage;
                identity.Expression = expr;
                CalcCellIdentity baseCell = this.GetBaseCell(identity.ActualIdentity);
                identity.Condition.Formula = this.Expression2Formula(expr, baseCell.RowIndex, baseCell.ColumnIndex);
                identity.Expression = expr;
                if (!this._conditionals.TryGetValue(identity.ActualIdentity, out storage))
                {
                    storage = new ConditionIdStorage(identity.ActualIdentity);
                    this._conditionals[identity.ActualIdentity] = storage;
                }
                storage.AddConditional(identity);
                this._graph.SetConditionalNode(identity, expr, this.Source.GetEvaluatorContext(id), this.Source);
            }
        }

        public bool Insert(int index, int count, bool row, bool isFullBand)
        {
            this.Suspend();
            InsertVisitor visitor = new InsertVisitor(index, count, row, isFullBand, null);
            this.UpdateActualIds(visitor, index, count, row, isFullBand);
            FormulaOperatorHelper.Insert(this, index, count, row, isFullBand, false, false);
            this.Resume(true);
            return true;
        }

        public void Invalidate(CalcCellIdentity cell)
        {
            this.InvalidateCore(cell);
        }

        public void Invalidate(CalcRangeIdentity range)
        {
            this.InvalidateCore(range);
        }

        void InvalidateCore(CalcLocalIdentity id)
        {
            ConditionalIdentity identity = id as ConditionalIdentity;
            if (identity != null)
            {
                this._dirtyList[identity.ActualIdentity] = identity.ActualIdentity;
            }
            else
            {
                CalcNode node = this._graph.GetNode(id);
                if (node == null)
                {
                    node = new CalcNode(this.Source, id);
                }
                CalcLocalIdentity identity2 = node.Id as CalcLocalIdentity;
                if (!object.ReferenceEquals(node.Dependents, null))
                {
                    foreach (CalcNode node2 in node.Dependents.Keys)
                    {
                        this.AddDirty(node2);
                    }
                }
                foreach (CalcNode node3 in this._graph.EnumerateIntersectedNodesExcludeSelf(this.Source.GetEvaluatorContext(id), identity2))
                {
                    CalcLocalIdentity key = node3.Id as CalcLocalIdentity;
                    if (key != null)
                    {
                        if (this._conditionals.ContainsKey(key))
                        {
                            this._dirtyList[key] = key;
                        }
                        if (!object.ReferenceEquals(node3.Dependents, null))
                        {
                            foreach (CalcNode node4 in node3.Dependents.Keys)
                            {
                                this.AddDirty(node4);
                            }
                        }
                    }
                }
            }
            if ((this._dirtyList.Count != 0) && (this._suspendCount == 0))
            {
                this.Refresh();
            }
        }

        static CalcLocalIdentity OffsetLocalIdentity(CalcLocalIdentity id, int offset, bool row)
        {
            CalcCellIdentity cellIdentity = id as CalcCellIdentity;
            CalcRangeIdentity range = id as CalcRangeIdentity;
            if (cellIdentity != null)
            {
                id = new CalcCellIdentity(cellIdentity, row ? offset : 0, row ? 0 : offset);
                return id;
            }
            if (range != null)
            {
                id = new CalcRangeIdentity(range, row ? offset : 0, row ? 0 : offset);
            }
            return id;
        }

        protected abstract void OnInvalidate(List<CalcRangeIdentity> ranges, List<CalcCellIdentity> cells);
        void Refresh()
        {
            if ((this._dirtyList != null) && (this._dirtyList.Count != 0))
            {
                List<CalcRangeIdentity> ranges = new List<CalcRangeIdentity>();
                List<CalcCellIdentity> cells = new List<CalcCellIdentity>();
                foreach (CalcLocalIdentity identity in this._dirtyList.Keys)
                {
                    if ((identity is CalcRangeIdentity) && this._conditionals.ContainsKey(identity as CalcRangeIdentity))
                    {
                        CalcRangeIdentity item = identity as CalcRangeIdentity;
                        ranges.Add(item);
                    }
                    else if ((identity is CalcCellIdentity) && this._conditionals.ContainsKey(identity as CalcCellIdentity))
                    {
                        CalcCellIdentity identity3 = identity as CalcCellIdentity;
                        cells.Add(identity3);
                    }
                }
                this.OnInvalidate(ranges, cells);
                this._dirtyList.Clear();
            }
        }

        public bool Remove(int index, int count, bool row, bool isFullBand)
        {
            this.Suspend();
            RemoveVisitor visitor = new RemoveVisitor(index, count, row, isFullBand, null);
            this.UpdateActualIds(visitor, index, count, row, isFullBand);
            FormulaOperatorHelper.Remove(this, index, count, row, isFullBand, false, false);
            this.Resume(true);
            return true;
        }

        public void RemoveConditionals(CalcLocalIdentity actualId, ICollection<IConditionalFormula> conditionals)
        {
            ConditionIdStorage storage;
            if (this._conditionals.TryGetValue(actualId, out storage))
            {
                if (conditionals == null)
                {
                    this._conditionals.Remove(actualId);
                }
                else
                {
                    int rowIndex;
                    int columnIndex;
                    if (actualId is CalcCellIdentity)
                    {
                        rowIndex = (actualId as CalcCellIdentity).RowIndex;
                        columnIndex = (actualId as CalcCellIdentity).ColumnIndex;
                    }
                    else
                    {
                        CalcRangeIdentity identity = actualId as CalcRangeIdentity;
                        rowIndex = identity.IsFullColumn ? 0 : identity.RowIndex;
                        columnIndex = identity.IsFullRow ? 0 : identity.ColumnIndex;
                    }
                    this.CalculationManager.Source.GetEvaluatorContext(new CalcCellIdentity(rowIndex, columnIndex));
                    foreach (IConditionalFormula formula in conditionals)
                    {
                        this.Formula2Expression(formula.Formula, rowIndex, columnIndex);
                        ConditionalIdentity id = storage.RemoveConditional(formula);
                        if (id != null)
                        {
                            this._graph.RemoveNode(id);
                        }
                    }
                    if (storage.Conditions.Count == 0)
                    {
                        this._conditionals.Remove(actualId);
                    }
                }
            }
        }

        public bool Reorder(int from, int to, int count, bool row, bool isFullBand)
        {
            this.Suspend();
            CalcStorage calcStorage = new CalcStorage();
            foreach (KeyValuePair<CalcLocalIdentity, ConditionIdStorage> pair in this._conditionals)
            {
                CalcRangeIdentity key = pair.Key as CalcRangeIdentity;
                if ((key != null) && ((!key.IsFullRow && !key.IsFullColumn) || ((key.RowCount > 1) || (key.ColumnCount > 1))))
                {
                    throw new InvalidOperationException();
                }
                foreach (ConditionalIdentity identity3 in pair.Value.IDs)
                {
                    calcStorage[identity3] = identity3.Expression;
                }
            }
            FormulaOperatorHelper.Reorder(this, calcStorage, from, to, count, row, false, false);
            this.Resume(true);
            return true;
        }

        public void Resume(bool refresh)
        {
            foreach (KeyValuePair<CalcLocalIdentity, ConditionIdStorage> pair in this._conditionals)
            {
                foreach (ConditionalIdentity identity in pair.Value.Conditions)
                {
                    identity.OldActualIdentity = null;
                }
            }
            this._suspendCount--;
            this._suspendCount = (this._suspendCount < 0) ? 0 : this._suspendCount;
            if ((this._suspendCount == 0) && refresh)
            {
                this.Refresh();
            }
        }

        internal void SetNode(CalcNode node)
        {
            this._graph.SetNode(node);
        }

        public void Suspend()
        {
            this._suspendCount++;
        }

        public void UpdataFormulas()
        {
            if (this._conditionals.Values != null)
            {
                foreach (ConditionIdStorage storage in this._conditionals.Values)
                {
                    if (storage.Conditions != null)
                    {
                        foreach (ConditionalIdentity identity in storage.Conditions)
                        {
                            try
                            {
                                CalcCellIdentity baseCell = this.GetBaseCell(identity.ActualIdentity);
                                identity.Condition.Formula = this.Expression2Formula(identity.Expression, baseCell.RowIndex, baseCell.ColumnIndex);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        void UpdateActualIds(OperatorExpressionVisistor visitor, int index, int count, bool row, bool isFullBand)
        {
            Dictionary<CalcLocalIdentity, ConditionIdStorage> dictionary = new Dictionary<CalcLocalIdentity, ConditionIdStorage>();
            foreach (KeyValuePair<CalcLocalIdentity, ConditionIdStorage> pair in this._conditionals)
            {
                CalcReferenceExpression expression;
                CalcLocalIdentity key = pair.Key;
                CalcCellIdentity objA = key as CalcCellIdentity;
                CalcRangeIdentity identity3 = key as CalcRangeIdentity;
                if (!object.ReferenceEquals(objA, null))
                {
                    expression = new CalcCellExpression(objA.RowIndex, objA.ColumnIndex);
                }
                else
                {
                    expression = new CalcRangeExpression(identity3.RowIndex, identity3.ColumnIndex, (identity3.RowIndex + identity3.RowCount) - 1, (identity3.ColumnIndex + identity3.ColumnCount) - 1);
                }
                expression = visitor.Visit(expression, 0, 0) as CalcReferenceExpression;
                CalcLocalIdentity id = expression.GetId(0, 0) as CalcLocalIdentity;
                if (id != key)
                {
                    key = id;
                    pair.Value.Offset(key);
                }
                dictionary[key] = pair.Value;
            }
            this._conditionals = dictionary;
        }

        CalcCalculationManager CalculationManager
        {
            get
            {
                if (this._calculationManager == null)
                {
                    this._calculationManager = new CalcService().GetCalculationManager(this.Source, null, true);
                }
                return this._calculationManager;
            }
        }

        IMultiSourceProvider IFormulaOperatorSource.MultiSourceProvider
        {
            get { return  this.MultiSourceProvider; }
        }

        ICalcSource IFormulaOperatorSource.Source
        {
            get { return  this.CalculationManager.Source; }
        }

        public bool HasFormulaCondition
        {
            get { return  (this._conditionals.Count > 0); }
        }

        protected abstract IMultiSourceProvider MultiSourceProvider { get; }

        protected abstract ReferenceStyle ReferenceStyle { get; }

        protected abstract ICalcSource Source { get; }

        bool UserR1C1
        {
            get { return  (this.ReferenceStyle == ReferenceStyle.R1C1); }
        }

        class CalcStorage : ICalcStorage<CalcLocalIdentity, CalcExpression>, IEnumerable<KeyValuePair<CalcLocalIdentity, CalcExpression>>, IEnumerable
        {
            readonly Dictionary<CalcLocalIdentity, CalcExpression> _storage = new Dictionary<CalcLocalIdentity, CalcExpression>();

            internal void Add(CalcLocalIdentity id, CalcExpression exp)
            {
                this._storage.Add(id, exp);
            }

            public IEnumerator<KeyValuePair<CalcLocalIdentity, CalcExpression>> GetEnumerator()
            {
                return this._storage.Select<KeyValuePair<CalcLocalIdentity, CalcExpression>, KeyValuePair<CalcLocalIdentity, CalcExpression>>(delegate (KeyValuePair<CalcLocalIdentity, CalcExpression> item) {
                    return new KeyValuePair<CalcLocalIdentity, CalcExpression>(item.Key, item.Value);
                }).GetEnumerator();
            }

            public void RemoveAt(CalcLocalIdentity id)
            {
                this._storage.Remove(id);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public int Count
            {
                get { return  this._storage.Count; }
            }

            public CalcExpression this[CalcLocalIdentity id]
            {
                get
                {
                    CalcExpression expression;
                    this._storage.TryGetValue(id, out expression);
                    return expression;
                }
                set { this._storage[id] = value; }
            }
        }

        class ConditionalCalcGraph
        {
            ICalcStorage<CalcIdentity, CalcNode> _conditionalNodeStorage = new SimpleNodeStorage();
            ICalcStorage<CalcIdentity, CalcNode> _nodes = new SimpleNodeStorage();
            ICalcStorage<CalcIdentity, CalcNode> _rangeNodeStorage = new SimpleNodeStorage();
            Dictionary<CalcLocalIdentity, CalcLocalIdentity> _sheetRangeIds = new Dictionary<CalcLocalIdentity, CalcLocalIdentity>();

            public ConditionalCalcGraph(IFormulaOperatorSource source)
            {
                this.Manager = source;
            }

            internal IEnumerable<CalcNode> EnumerateIntersectedNodesExcludeSelf(CalcEvaluatorContext context, CalcLocalIdentity id)
            {
                if (!object.ReferenceEquals(context, null) && !object.ReferenceEquals(id, null))
                {
                    if (id is ConditionalGraph.ConditionalIdentity)
                    {
                        id = (id as ConditionalGraph.ConditionalIdentity).ActualIdentity;
                    }
                    if (id is CalcRangeIdentity)
                    {
                        CalcRangeIdentity srcId = id as CalcRangeIdentity;
                        if (srcId.IsFullColumn && srcId.IsFullRow)
                        {
                            foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable1 in this._nodes)
                            {
                                yield return iteratorVariable1.Value;
                            }
                            foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable2 in this._rangeNodeStorage)
                            {
                                if (iteratorVariable2.Key == srcId)
                                {
                                    continue;
                                }
                                yield return iteratorVariable2.Value;
                            }
                        }
                        else if (srcId.IsFullColumn || srcId.IsFullRow)
                        {
                            foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable3 in this._nodes)
                            {
                                if (!context.IsIntersected(srcId, iteratorVariable3.Key))
                                {
                                    continue;
                                }
                                yield return iteratorVariable3.Value;
                            }
                            foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable4 in this._rangeNodeStorage)
                            {
                                if (!context.IsIntersected(srcId, iteratorVariable4.Key) || (srcId == iteratorVariable4.Key))
                                {
                                    continue;
                                }
                                yield return iteratorVariable4.Value;
                            }
                        }
                        else
                        {
                            for (int i = srcId.RowIndex; i < (srcId.RowIndex + srcId.RowCount); i++)
                            {
                                for (int j = srcId.ColumnIndex; j < (srcId.ColumnIndex + srcId.ColumnCount); j++)
                                {
                                    CalcCellIdentity iteratorVariable7 = new CalcCellIdentity(i, j);
                                    CalcNode node = this.GetNode(iteratorVariable7);
                                    if (node != null)
                                    {
                                        yield return node;
                                    }
                                }
                            }
                            foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable9 in this._rangeNodeStorage)
                            {
                                if (!context.IsIntersected(id, iteratorVariable9.Key) || (id == iteratorVariable9.Key))
                                {
                                    continue;
                                }
                                yield return iteratorVariable9.Value;
                            }
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable10 in this._rangeNodeStorage)
                        {
                            if (!context.IsIntersected(id, iteratorVariable10.Key))
                            {
                                continue;
                            }
                            yield return iteratorVariable10.Value;
                        }
                    }
                }
            }


            static CalcIdentity ExtendPrecedents(CalcReferenceExpression expr, CalcIdentity id, int extendRow, int extendColumn)
            {
                if ((extendRow == 0) && (extendColumn == 0))
                {
                    return id;
                }
                int bandIndex = -1;
                int columnIndex = -1;
                int num3 = -1;
                int num4 = -1;
                CalcCellExpression expression = expr as CalcCellExpression;
                CalcRangeExpression expression2 = expr as CalcRangeExpression;
                CalcExternalCellExpression expression3 = expr as CalcExternalCellExpression;
                CalcExternalRangeExpression expression4 = expr as CalcExternalRangeExpression;
                if (expression != null)
                {
                    if (!expression.RowRelative && !expression.ColumnRelative)
                    {
                        return id;
                    }
                    CalcCellIdentity identity = id as CalcCellIdentity;
                    bandIndex = identity.RowIndex;
                    columnIndex = identity.ColumnIndex;
                    num3 = bandIndex + (expression.RowRelative ? extendRow : 0);
                    num4 = columnIndex + (expression.ColumnRelative ? extendColumn : 0);
                }
                else if (expression3 != null)
                {
                    if (!expression3.RowRelative && !expression3.ColumnRelative)
                    {
                        return id;
                    }
                    CalcExternalCellIdentity identity2 = id as CalcExternalCellIdentity;
                    bandIndex = identity2.RowIndex;
                    columnIndex = identity2.ColumnIndex;
                    num3 = bandIndex + (expression3.RowRelative ? extendRow : 0);
                    num4 = columnIndex + (expression3.ColumnRelative ? extendColumn : 0);
                }
                else if (expression2 != null)
                {
                    if (!expression2.EndRowRelative && !expression2.EndColumnRelative)
                    {
                        return id;
                    }
                    CalcRangeIdentity identity3 = id as CalcRangeIdentity;
                    bandIndex = identity3.RowIndex;
                    columnIndex = identity3.ColumnIndex;
                    if (!identity3.IsFullColumn)
                    {
                        num3 = ((identity3.RowIndex + identity3.RowCount) - 1) + (expression2.EndRowRelative ? extendRow : 0);
                    }
                    if (!identity3.IsFullRow)
                    {
                        num4 = ((identity3.ColumnIndex + identity3.ColumnCount) - 1) + (expression2.EndColumnRelative ? extendColumn : 0);
                    }
                }
                else
                {
                    if (expression4 == null)
                    {
                        return id;
                    }
                    if (!expression4.EndRowRelative && !expression4.EndColumnRelative)
                    {
                        return id;
                    }
                    CalcExternalRangeIdentity identity4 = id as CalcExternalRangeIdentity;
                    bandIndex = identity4.RowIndex;
                    columnIndex = identity4.ColumnIndex;
                    if (!identity4.IsFullColumn)
                    {
                        num3 = ((identity4.RowIndex + identity4.RowCount) - 1) + (expression4.EndRowRelative ? extendRow : 0);
                    }
                    if (!identity4.IsFullRow)
                    {
                        num4 = ((identity4.ColumnIndex + identity4.ColumnCount) - 1) + (expression4.EndColumnRelative ? extendColumn : 0);
                    }
                }
                if (extendRow < 0)
                {
                    bandIndex = num3 = -1;
                }
                if (extendColumn < 0)
                {
                    columnIndex = num4 = -1;
                }
                if ((num3 == -1) && (num4 == -1))
                {
                    return new CalcRangeIdentity();
                }
                if (num3 == -1)
                {
                    return new CalcRangeIdentity(columnIndex, (num4 - columnIndex) + 1, false);
                }
                if (num4 == -1)
                {
                    return new CalcRangeIdentity(bandIndex, (num3 - bandIndex) + 1, true);
                }
                return new CalcRangeIdentity(bandIndex, columnIndex, (num3 - bandIndex) + 1, (num4 - columnIndex) + 1);
            }

            internal IEnumerable<CalcLocalIdentity> GetAllLocalIdentities()
            {
                Dictionary<CalcLocalIdentity, CalcLocalIdentity> dictionary = new Dictionary<CalcLocalIdentity, CalcLocalIdentity>();
                foreach (KeyValuePair<CalcIdentity, CalcNode> pair in this._nodes)
                {
                    CalcLocalIdentity key = pair.Key as CalcLocalIdentity;
                    if (!object.ReferenceEquals(key, null))
                    {
                        dictionary[key] = key;
                    }
                }
                foreach (KeyValuePair<CalcIdentity, CalcNode> pair2 in this._rangeNodeStorage)
                {
                    CalcLocalIdentity objA = pair2.Key as CalcLocalIdentity;
                    if (!object.ReferenceEquals(objA, null))
                    {
                        dictionary[objA] = objA;
                    }
                }
                foreach (KeyValuePair<CalcIdentity, CalcNode> pair3 in this._conditionalNodeStorage)
                {
                    CalcLocalIdentity identity3 = pair3.Key as CalcLocalIdentity;
                    if (!object.ReferenceEquals(identity3, null))
                    {
                        dictionary[identity3] = identity3;
                    }
                }
                return dictionary.Keys;
            }

            public CalcNode GetNode(CalcIdentity id)
            {
                CalcNode objA = this._conditionalNodeStorage[id];
                if (!object.ReferenceEquals(objA, null))
                {
                    return objA;
                }
                objA = this._nodes[id];
                if (!object.ReferenceEquals(objA, null))
                {
                    return objA;
                }
                return this._rangeNodeStorage[id];
            }

            bool IsIsolatedNode(CalcNode node)
            {
                if ((node.Precedents != null) && (node.Precedents.Count > 0))
                {
                    return false;
                }
                if (node.Dependents != null)
                {
                    return (node.Dependents.Count <= 0);
                }
                return true;
            }

            List<CalcNode> ParsePrecedents(CalcNode current, CalcExpression expr, CalcEvaluatorContext context)
            {
                CalcIdentity id = current.Id;
                if (id is ConditionalGraph.ConditionalIdentity)
                {
                    id = (id as ConditionalGraph.ConditionalIdentity).ActualIdentity;
                }
                while (expr is CalcParenthesesExpression)
                {
                    expr = (expr as CalcParenthesesExpression).Arg;
                }
                List<CalcIdentity> theRefs = new List<CalcIdentity>();
                if (expr is CalcSheetRangeExpression)
                {
                    CalcSheetRangeExpression sheetRangeExpr = expr as CalcSheetRangeExpression;
                    this._sheetRangeIds[current.Id as CalcLocalIdentity] = current.Id as CalcLocalIdentity;
                    this.ParseSheetRangePrecedents(id, theRefs, sheetRangeExpr);
                }
                else if (expr is CalcReferenceExpression)
                {
                    ParseReferencePrecedents(id as CalcLocalIdentity, expr as CalcReferenceExpression, theRefs);
                }
                else if (expr is CalcExternalNameExpression)
                {
                    CalcExternalNameExpression expression2 = expr as CalcExternalNameExpression;
                    theRefs.Add(new CalcExternalNameIdentity(expression2.Source, expression2.Name));
                }
                else if (expr is CalcNameExpression)
                {
                    theRefs.Add(new CalcNameIdentity((expr as CalcNameExpression).Name));
                }
                else
                {
                    if (expr is CalcUnaryOperatorExpression)
                    {
                        return this.ParsePrecedents(current, (expr as CalcUnaryOperatorExpression).Operand, context);
                    }
                    if (expr is CalcBinaryOperatorExpression)
                    {
                        CalcBinaryOperatorExpression expression3 = expr as CalcBinaryOperatorExpression;
                        List<CalcNode> collection = this.ParsePrecedents(current, expression3.Left, context);
                        List<CalcNode> list3 = this.ParsePrecedents(current, expression3.Right, context);
                        if ((collection != null) && (list3 != null))
                        {
                            List<CalcNode> list4 = new List<CalcNode>(collection.Count + list3.Count);
                            list4.AddRange(collection);
                            list4.AddRange(list3);
                            return list4;
                        }
                        if (collection != null)
                        {
                            return collection;
                        }
                        if (list3 != null)
                        {
                            return list3;
                        }
                    }
                    else if (expr is CalcFunctionExpression)
                    {
                        CalcFunctionExpression expression4 = expr as CalcFunctionExpression;
                        CalcFunction function = expression4.Function;
                        object[] objArray = new object[expression4.ArgCount];
                        if ((function == null) && (context != null))
                        {
                            function = context.GetFunction(expression4.FunctionName);
                        }
                        if (((function != null) && (function.MinArgs <= objArray.Length)) && (objArray.Length <= function.MaxArgs))
                        {
                            List<CalcNode> list5 = new List<CalcNode>();
                            for (int i = 0; i < objArray.Length; i++)
                            {
                                List<CalcNode> list6 = this.ParsePrecedents(current, expression4.GetArg(i), context);
                                if ((list6 != null) && (list6.Count > 0))
                                {
                                    list5.AddRange(list6);
                                }
                            }
                            if (list5.Count > 0)
                            {
                                return list5;
                            }
                        }
                    }
                }
                List<CalcNode> list7 = new List<CalcNode>();
                foreach (CalcIdentity identity2 in theRefs)
                {
                    CalcNode objA = this.GetNode(identity2);
                    if (object.ReferenceEquals(objA, null))
                    {
                        objA = new CalcNode(context.Source, identity2);
                    }
                    if (objA.Dependents == null)
                    {
                        objA.Dependents = new Dictionary<CalcNode, CalcNode>();
                    }
                    objA.Dependents[current] = current;
                    list7.Add(objA);
                }
                if (expr is CalcNameExpression)
                {
                    CalcExpression name = context.GetName((expr as CalcNameExpression).Name);
                    if (name != null)
                    {
                        List<CalcNode> list8 = this.ParsePrecedents(current, name, context);
                        if ((list8 != null) && (list8.Count > 0))
                        {
                            list7.AddRange(list8);
                        }
                    }
                }
                return list7;
            }

            static void ParseReferencePrecedents(CalcLocalIdentity targetId, CalcReferenceExpression expr, List<CalcIdentity> theRefs)
            {
                int rowIndex;
                int columnIndex;
                CalcCellIdentity identity = targetId as CalcCellIdentity;
                CalcRangeIdentity identity2 = targetId as CalcRangeIdentity;
                if (identity != null)
                {
                    rowIndex = identity.RowIndex;
                    columnIndex = identity.ColumnIndex;
                }
                else
                {
                    rowIndex = identity2.RowIndex;
                    columnIndex = identity2.ColumnIndex;
                }
                CalcIdentity id = expr.GetId(rowIndex, columnIndex);
                CalcCellIdentity identity4 = id as CalcCellIdentity;
                CalcRangeIdentity identity5 = id as CalcRangeIdentity;
                CalcExternalCellIdentity identity6 = id as CalcExternalCellIdentity;
                CalcExternalRangeIdentity identity7 = id as CalcExternalRangeIdentity;
                if ((((identity4 == null) || ((identity4.RowIndex >= 0) && (identity4.ColumnIndex >= 0))) && ((identity5 == null) || ((identity5.IsFullRow || (identity5.ColumnIndex >= 0)) && (identity5.IsFullColumn || (identity5.RowIndex >= 0))))) && (((identity6 == null) || ((identity6.RowIndex >= 0) && (identity6.ColumnIndex >= 0))) && ((identity7 == null) || ((identity7.IsFullRow || (identity7.ColumnIndex >= 0)) && (identity7.IsFullColumn || (identity7.RowIndex >= 0))))))
                {
                    int num3;
                    int num4;
                    ShouldExtendPrecedents(targetId, out num3, out num4);
                    theRefs.Add(ExtendPrecedents(expr, id, num3, num4));
                }
            }

            void ParseSheetRangePrecedents(CalcIdentity currentId, List<CalcIdentity> theRefs, CalcSheetRangeExpression sheetRangeExpr)
            {
                IMultiSourceProvider multiSourceProvider = this.Manager.MultiSourceProvider;
                if (multiSourceProvider != null)
                {
                    foreach (ICalcSource source in multiSourceProvider.GetCalcSources(sheetRangeExpr.StartSource, sheetRangeExpr.EndSource))
                    {
                        CalcReferenceExpression expression;
                        if ((sheetRangeExpr.EndRow == -1) || (sheetRangeExpr.EndColumn == -1))
                        {
                            expression = new CalcExternalCellExpression(source, sheetRangeExpr.StartRow, sheetRangeExpr.StartColumn, sheetRangeExpr.StartRowRelative, sheetRangeExpr.StartColumnRelative);
                        }
                        else if (sheetRangeExpr.IsFullRow && sheetRangeExpr.IsFullColumn)
                        {
                            expression = new CalcExternalRangeExpression(source);
                        }
                        else if (!sheetRangeExpr.IsFullColumn && !sheetRangeExpr.IsFullColumn)
                        {
                            expression = new CalcExternalRangeExpression(source, sheetRangeExpr.StartRow, sheetRangeExpr.StartColumn, sheetRangeExpr.EndRow, sheetRangeExpr.EndColumn, sheetRangeExpr.StartRowRelative, sheetRangeExpr.StartColumnRelative, sheetRangeExpr.EndRowRelative, sheetRangeExpr.EndColumnRelative);
                        }
                        else if (sheetRangeExpr.IsFullRow)
                        {
                            expression = new CalcExternalRangeExpression(source, sheetRangeExpr.StartRow, sheetRangeExpr.EndRow, sheetRangeExpr.StartRowRelative, sheetRangeExpr.EndRowRelative, true);
                        }
                        else
                        {
                            expression = new CalcExternalRangeExpression(source, sheetRangeExpr.StartColumn, sheetRangeExpr.EndColumn, sheetRangeExpr.StartColumnRelative, sheetRangeExpr.EndColumnRelative, false);
                        }
                        ParseReferencePrecedents(currentId as CalcLocalIdentity, expression, theRefs);
                    }
                }
            }

            public void RemoveNode(CalcIdentity id)
            {
                CalcNode key = this.GetNode(id);
                if (key != null)
                {
                    if (key.Precedents != null)
                    {
                        foreach (CalcNode node2 in key.Precedents)
                        {
                            if (node2.Dependents != null)
                            {
                                node2.Dependents.Remove(key);
                            }
                            if (this.IsIsolatedNode(node2))
                            {
                                this.RemoveNodeAt(node2.Id);
                            }
                        }
                        key.Precedents.Clear();
                    }
                    if (this.IsIsolatedNode(key))
                    {
                        this.RemoveNodeAt(id);
                    }
                }
            }

            void RemoveNodeAt(CalcIdentity id)
            {
                this._nodes.RemoveAt(id);
                this._rangeNodeStorage.RemoveAt(id);
                this._conditionalNodeStorage.RemoveAt(id);
            }

            public void SetConditionalNode(ConditionalGraph.ConditionalIdentity id, CalcExpression expr, CalcEvaluatorContext evalContext, ICalcSource source)
            {
                if (object.ReferenceEquals(expr, null))
                {
                    throw new ArgumentNullException("expr");
                }
                CalcNode objA = this.GetNode(id);
                if (!object.ReferenceEquals(objA, null))
                {
                    this.RemoveNode(objA.Id);
                }
                objA = new CalcNode(source, id);
                objA.Precedents = this.ParsePrecedents(objA, expr, evalContext);
                this.SetNode(objA);
                if ((objA.Precedents != null) && (objA.Precedents.Count > 0))
                {
                    CalcIdentity identity = new ConditionalGraph.ExternalConditionalIdentity(source, id.ActualIdentity, id.Condition, id.Expression);
                    foreach (CalcNode node2 in objA.Precedents)
                    {
                        this.SetNode(node2);
                        CalcExternalIdentity identity2 = node2.Id as CalcExternalIdentity;
                        if (!object.ReferenceEquals(identity2, null) && (this.Manager != null))
                        {
                            ICalcSource source2 = identity2.Source;
                            ConditionalGraph externalManager = this.Manager.GetExternalManager(source2) as ConditionalGraph;
                            CalcIdentity identity3 = identity2.ConvertToLocal();
                            CalcNode node = externalManager.GetNode(identity3);
                            if (node == null)
                            {
                                externalManager.SetNode(node = new CalcNode(identity2.Source, identity3));
                            }
                            if (node.Dependents == null)
                            {
                                node.Dependents = new Dictionary<CalcNode, CalcNode>();
                            }
                            if (!object.ReferenceEquals(identity, null))
                            {
                                CalcNode node4 = externalManager.GetNode(identity);
                                if (node4 == null)
                                {
                                    externalManager.SetNode(node4 = new CalcNode(source, identity));
                                }
                                if (node4.Precedents == null)
                                {
                                    node4.Precedents = new List<CalcNode>();
                                }
                                node4.Precedents.Add(node);
                                node.Dependents[node4] = node4;
                            }
                        }
                    }
                }
            }

            public void SetNode(CalcNode node)
            {
                if ((node.Id is ConditionalGraph.ConditionalIdentity) || (node.Id is ConditionalGraph.ExternalConditionalIdentity))
                {
                    this._conditionalNodeStorage[node.Id] = node;
                }
                if (((node.Id is CalcRangeIdentity) || (node.Id is CalcExternalRangeIdentity)) || ((node.Id is CalcSheetRangeIdentity) || (node.Id is CalcStructReferenceIndentity)))
                {
                    this._rangeNodeStorage[node.Id] = node;
                }
                else
                {
                    this._nodes[node.Id] = node;
                }
            }

            static void ShouldExtendPrecedents(CalcLocalIdentity targetId, out int extendRow, out int extendColumn)
            {
                extendRow = 0;
                extendColumn = 0;
                CalcRangeIdentity identity = targetId as CalcRangeIdentity;
                if (identity != null)
                {
                    if (identity.IsFullRow || (identity.ColumnCount > 1))
                    {
                        extendColumn = identity.ColumnCount - 1;
                    }
                    if (identity.IsFullColumn || (identity.RowCount > 1))
                    {
                        extendRow = identity.RowCount - 1;
                    }
                }
            }

            public IFormulaOperatorSource Manager { get; set; }
            
            class SimpleNodeStorage : ICalcStorage<CalcIdentity, CalcNode>, IEnumerable<KeyValuePair<CalcIdentity, CalcNode>>, IEnumerable
            {
                Dictionary<CalcIdentity, CalcNode> _storage = new Dictionary<CalcIdentity, CalcNode>();

                public IEnumerator<KeyValuePair<CalcIdentity, CalcNode>> GetEnumerator()
                {
                    return this._storage.GetEnumerator();
                }

                public void RemoveAt(CalcIdentity id)
                {
                    this._storage.Remove(id);
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable0 in this._storage)
                    {
                        yield return new KeyValuePair<CalcIdentity, CalcNode>(iteratorVariable0.Key, iteratorVariable0.Value);
                    }
                }


                public int Count
                {
                    get { return  this._storage.Count; }
                }

                public CalcNode this[CalcIdentity id]
                {
                    get
                    {
                        CalcNode node;
                        this._storage.TryGetValue(id, out node);
                        return node;
                    }
                    set { this._storage[id] = value; }
                }

            }
        }

        internal class ConditionalIdentity : CalcLocalIdentity
        {
            public ConditionalIdentity(CalcLocalIdentity actualIdentity, IConditionalFormula condition, CalcExpression expression)
            {
                this.ActualIdentity = actualIdentity;
                this.Condition = condition;
                this.Expression = expression;
            }

            protected override bool CompareTo(CalcIdentity other)
            {
                ConditionalGraph.ConditionalIdentity identity = other as ConditionalGraph.ConditionalIdentity;
                return (((identity != null) && (this.ActualIdentity == identity.ActualIdentity)) && (this.Condition == identity.Condition));
            }

            protected override int ComputeHash()
            {
                return this.Condition.GetHashCode();
            }

            public CalcLocalIdentity ActualIdentity { get; set; }

            public IConditionalFormula Condition { get; private set; }

            public CalcExpression Expression { get; set; }

            public CalcLocalIdentity OldActualIdentity { get; set; }
        }

        class ConditionIdStorage
        {
            CalcLocalIdentity _actualId;
            Dictionary<IConditionalFormula, ConditionalGraph.ConditionalIdentity> _conditions = new Dictionary<IConditionalFormula, ConditionalGraph.ConditionalIdentity>();

            public ConditionIdStorage(CalcLocalIdentity actualId)
            {
                this._actualId = actualId;
            }

            public ConditionalGraph.ConditionalIdentity AddConditional(ConditionalGraph.ConditionalIdentity id)
            {
                if (this._conditions.ContainsKey(id.Condition))
                {
                    return null;
                }
                this._conditions[id.Condition] = id;
                return id;
            }

            public ConditionalGraph.ConditionalIdentity AddConditional(IConditionalFormula condition, CalcExpression expression)
            {
                ConditionalGraph.ConditionalIdentity identity;
                if (this._conditions.TryGetValue(condition, out identity))
                {
                    return null;
                }
                identity = new ConditionalGraph.ConditionalIdentity(this._actualId, condition, expression);
                this._conditions[condition] = identity;
                return identity;
            }

            public void Offset(CalcLocalIdentity newActualId)
            {
                this._actualId = newActualId;
                foreach (KeyValuePair<IConditionalFormula, ConditionalGraph.ConditionalIdentity> pair in this._conditions)
                {
                    pair.Value.OldActualIdentity = pair.Value.ActualIdentity;
                    pair.Value.ActualIdentity = this._actualId;
                }
            }

            public ConditionalGraph.ConditionalIdentity RemoveConditional(IConditionalFormula condition)
            {
                if (this._conditions.ContainsKey(condition))
                {
                    ConditionalGraph.ConditionalIdentity identity = this._conditions[condition];
                    this._conditions.Remove(condition);
                    return identity;
                }
                return null;
            }

            public ICollection<ConditionalGraph.ConditionalIdentity> Conditions
            {
                get { return  this._conditions.Values; }
            }

            public ICollection<ConditionalGraph.ConditionalIdentity> IDs
            {
                get { return  this._conditions.Values; }
            }
        }

        internal class ExternalConditionalIdentity : CalcExternalIdentity
        {
            public ExternalConditionalIdentity(ICalcSource source, CalcLocalIdentity actualIdentity, IConditionalFormula condition, CalcExpression expression) : base(source)
            {
                this.ActualIdentity = actualIdentity;
                this.Condition = condition;
                this.Expression = expression;
            }

            protected override bool CompareTo(CalcIdentity other)
            {
                ConditionalGraph.ExternalConditionalIdentity identity = other as ConditionalGraph.ExternalConditionalIdentity;
                return (((identity != null) && (this.ActualIdentity == identity.ActualIdentity)) && (this.Condition == identity.Condition));
            }

            protected override int ComputeHash()
            {
                return ((base.ComputeHash() ^ this.Condition.GetHashCode()) ^ base.Source.GetHashCode());
            }

            public override CalcLocalIdentity ConvertToLocal()
            {
                return new ConditionalGraph.ConditionalIdentity(this.ActualIdentity, this.Condition, this.Expression);
            }

            public CalcLocalIdentity ActualIdentity { get; private set; }

            public IConditionalFormula Condition { get; private set; }

            public CalcExpression Expression { get; set; }

            public CalcLocalIdentity OldActualIdentity { get; set; }
        }
    }
}

