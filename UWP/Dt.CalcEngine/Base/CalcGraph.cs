#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using Dt.CalcEngine.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represents a calculation graph which used to manage dependents and precedents of each nodes.
    /// </summary>
    public class CalcGraph
    {
        private int[] _arrayFormulaColumnFlags;
        private Dictionary<CalcRangeIdentity, CalcRangeIdentity> _arrayFormulaIds = new Dictionary<CalcRangeIdentity, CalcRangeIdentity>();
        private int[] _arrayFormulaRowFlags;
        private Dictionary<CalcRangeIdentity, CalcNode> _bigRangeDependency = new Dictionary<CalcRangeIdentity, CalcNode>();
        private Dictionary<int, CalcNode> _columnDependency = new Dictionary<int, CalcNode>();
        private Dictionary<CalcRangeIdentity, CalcNode> _columnSharedIds = new Dictionary<CalcRangeIdentity, CalcNode>();
        private CalcEvaluatorContext _evaluatorContext;
        private Dictionary<int, Dictionary<int, List<CalcNode>>> _expandedRangeDependency = new Dictionary<int, Dictionary<int, List<CalcNode>>>();
        private CalcNode _fullSheetRangeDependency;
        private int _fullSheetRangeDependencyRefCount;
        private Dictionary<CalcRangeIdentity, CalcNode> _largeSharedIds = new Dictionary<CalcRangeIdentity, CalcNode>();
        private ICalcStorage<CalcIdentity, CalcNode> _nodes = new SimpleNodeStorage<CalcIdentity>();
        private ICalcStorage<CalcIdentity, CalcNode> _rangeNodeStorage = new SimpleNodeStorage<CalcIdentity>();
        private Dictionary<int, CalcNode> _rowDependency = new Dictionary<int, CalcNode>();
        private Dictionary<CalcRangeIdentity, CalcNode> _rowSharedIds = new Dictionary<CalcRangeIdentity, CalcNode>();
        private Dictionary<CalcLocalIdentity, CalcLocalIdentity> _sheetRangeIds = new Dictionary<CalcLocalIdentity, CalcLocalIdentity>();
        private readonly object _syncRoot = new object();
        private HashSet<CalcLocalIdentity> _volatileIds = new HashSet<CalcLocalIdentity>();
        internal const int Large_Array_Length = 0x80;
        private HashSet<CalcNode> _volatileNodes = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcGraph" /> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public CalcGraph(CalcCalculationManager manager)
        {
            this.Manager = manager;
            if (this._nodes.Count > 0)
            {
                foreach (KeyValuePair<CalcIdentity, CalcNode> pair in this._nodes)
                {
                    if (pair.Value.IsVolatile && (pair.Key is CalcLocalIdentity))
                    {
                        this._volatileIds.Add(pair.Key as CalcLocalIdentity);
                    }
                }
            }
            if (this._rangeNodeStorage.Count > 0)
            {
                foreach (KeyValuePair<CalcIdentity, CalcNode> pair2 in this._rangeNodeStorage)
                {
                    if (pair2.Value.IsVolatile && (pair2.Key is CalcLocalIdentity))
                    {
                        this._volatileIds.Add(pair2.Key as CalcLocalIdentity);
                    }
                    this.ExpandRangeIdentity(pair2.Key as CalcRangeIdentity, pair2.Value);
                }
            }
        }

        private void AddArrayFormula(CalcRangeIdentity rangeId)
        {
            this.EnableArrayFormula();
            if (this._arrayFormulaIds.ContainsKey(rangeId))
            {
                this.RemoveArrayFormula(rangeId);
            }
            this._arrayFormulaIds.Add(rangeId, rangeId);
            if (!rangeId.IsFullColumn)
            {
                for (int i = rangeId.RowIndex; i < (rangeId.RowIndex + rangeId.RowCount); i++)
                {
                    this._arrayFormulaRowFlags[i]++;
                }
            }
            else
            {
                this._arrayFormulaRowFlags[0x100000]++;
            }
            if (!rangeId.IsFullRow)
            {
                for (int j = rangeId.ColumnIndex; j < (rangeId.ColumnIndex + rangeId.ColumnCount); j++)
                {
                    this._arrayFormulaColumnFlags[j]++;
                }
            }
            else
            {
                this._arrayFormulaColumnFlags[0x4000]++;
            }
        }

        private void AddLeftRightPartExpression(CalcRangeIdentity currentRange, CalcRangeIdentity oldRange, Dictionary<CalcRangeIdentity, CalcExpression> addedIds, int row, int rowCount)
        {
            if (currentRange.ColumnIndex > oldRange.ColumnIndex)
            {
                CalcRangeIdentity identity;
                int columnIndex = oldRange.ColumnIndex;
                int bandCount = (currentRange.ColumnIndex - oldRange.ColumnIndex) + 1;
                if (oldRange.IsFullRow)
                {
                    identity = new CalcRangeIdentity(columnIndex, bandCount, false);
                }
                else
                {
                    identity = new CalcRangeIdentity(row, columnIndex, rowCount, bandCount);
                }
                this.AddPartExpression(oldRange, identity, addedIds);
            }
            if ((currentRange.ColumnIndex + currentRange.ColumnCount) < (oldRange.ColumnIndex + oldRange.ColumnCount))
            {
                CalcRangeIdentity identity2;
                int bandIndex = (currentRange.ColumnIndex + currentRange.ColumnCount) - 1;
                int num4 = (oldRange.ColumnIndex + oldRange.ColumnCount) - bandIndex;
                if (oldRange.IsFullRow)
                {
                    identity2 = new CalcRangeIdentity(bandIndex, num4, false);
                }
                else
                {
                    identity2 = new CalcRangeIdentity(row, bandIndex, rowCount, num4);
                }
                this.AddPartExpression(oldRange, identity2, addedIds);
            }
        }

        private void AddPartExpression(CalcRangeIdentity oldRange, CalcRangeIdentity partRange, Dictionary<CalcRangeIdentity, CalcExpression> addedIds)
        {
            CalcExpression expression2 = this.Manager.GetExpression(oldRange).Offset(0, 0, false, true);
            addedIds.Add(partRange, expression2);
        }

        private void AddSharedFormula(CalcRangeIdentity rangeId, CalcNode node)
        {
            if (rangeId.IsFullRow)
            {
                this._rowSharedIds.Add(rangeId, node);
            }
            else if (rangeId.IsFullColumn)
            {
                this._columnSharedIds.Add(rangeId, node);
            }
            else
            {
                this._largeSharedIds.Add(rangeId, node);
            }
        }

        private void AddTopOrBottomPartExpression(CalcRangeIdentity oldRange, Dictionary<CalcRangeIdentity, CalcExpression> addedIds, int row, int rowCount)
        {
            CalcRangeIdentity identity;
            if (oldRange.IsFullRow)
            {
                identity = new CalcRangeIdentity(row, rowCount, true);
            }
            else
            {
                identity = new CalcRangeIdentity(row, oldRange.ColumnIndex, rowCount, oldRange.ColumnCount);
            }
            this.AddPartExpression(oldRange, identity, addedIds);
        }

        internal void ClearNodes()
        {
            this.RemoveExternalPrecedentsDependents();
            this._nodes = new SimpleNodeStorage<CalcIdentity>();
            this._rangeNodeStorage = new SimpleNodeStorage<CalcIdentity>();
            this._sheetRangeIds.Clear();
            this._arrayFormulaIds.Clear();
        }

        private CalcNode CreateNode(CalcLocalIdentity id, CalcEvaluatorContext evalContext, ICalcSource source, bool isArrayFormula)
        {
            CalcRangeIdentity key = id as CalcRangeIdentity;
            if (key != null)
            {
                List<CalcRangeIdentity> list;
                RangeRelative relative = this.GetArrayFormulaRange(id, out list, true);
                if (!this._arrayFormulaIds.ContainsKey(key))
                {
                    switch (relative)
                    {
                        case RangeRelative.Intersectant:
                            ExceptionHelper.ThrowChangePartOfArrayFormulaException();
                            break;

                        case RangeRelative.Contains:
                            foreach (CalcRangeIdentity identity2 in list)
                            {
                                this.Manager.ClearFormula(identity2);
                            }
                            break;
                    }
                }
                if (!object.ReferenceEquals(evalContext, null))
                {
                    foreach (CalcNode node in this.GetAllContainsCellWithExpressions(evalContext, key))
                    {
                        this.RemoveNode(node.Id as CalcCellIdentity);
                    }
                }
                if (isArrayFormula)
                {
                    this.AddArrayFormula(key);
                }
            }
            else if (this.IsIsIntersectantWithArrayFormula(id))
            {
                ExceptionHelper.ThrowChangePartOfArrayFormulaException();
            }
            else
            {
                key = new CalcRangeIdentity((id as CalcCellIdentity).RowIndex, (id as CalcCellIdentity).ColumnIndex, 1, 1);
            }
            CalcNode nodeInternal = this.GetNodeInternal(id);
            if (!object.ReferenceEquals(nodeInternal, null))
            {
                if (nodeInternal.Precedents != null)
                {
                    foreach (CalcNode node3 in nodeInternal.Precedents)
                    {
                        this.RemovePredentsNode(nodeInternal, node3);
                    }
                    nodeInternal.Precedents.Clear();
                }
            }
            else
            {
                if (!isArrayFormula)
                {
                    if (!object.ReferenceEquals(evalContext, null))
                    {
                        foreach (CalcNode node4 in this.GetAllContainsCellWithExpressions(evalContext, key))
                        {
                            this.RemoveNode(node4.Id as CalcCellIdentity);
                        }
                    }
                    List<CalcRangeIdentity> removedIds = new List<CalcRangeIdentity>();
                    Dictionary<CalcRangeIdentity, CalcExpression> addedIds = new Dictionary<CalcRangeIdentity, CalcExpression>();
                    foreach (KeyValuePair<CalcRangeIdentity, CalcNode> pair in this._largeSharedIds)
                    {
                        this.ProcessIntersectedSharedFormula(key, pair.Key, removedIds, addedIds);
                    }
                    foreach (CalcRangeIdentity identity3 in removedIds)
                    {
                        this.RemoveNode(identity3);
                    }
                    foreach (KeyValuePair<CalcRangeIdentity, CalcExpression> pair2 in addedIds)
                    {
                        this.SetNode(pair2.Key, pair2.Value, evalContext, source, isArrayFormula);
                    }
                }
                nodeInternal = this.EnsureNode(id, null);
            }
            if ((!isArrayFormula && (id is CalcRangeIdentity)) && !this.IsSharedFormula(id as CalcRangeIdentity))
            {
                this.AddSharedFormula(key, nodeInternal);
            }
            return nodeInternal;
        }

        private void EnableArrayFormula()
        {
            if (!this.HasArrayFormula)
            {
                this._arrayFormulaRowFlags = new int[0x100001];
                this._arrayFormulaColumnFlags = new int[0x4001];
            }
        }

        internal CalcNode EnsureNode(CalcIdentity id, ICalcSource source = null)
        {
            if ((source == null) && (this.Manager != null))
            {
                source = this.Manager.Source;
            }
            CalcNode nodeInternal = this.GetNodeInternal(id);
            if (object.ReferenceEquals(nodeInternal, null))
            {
                this.SetNodeInternal(id, nodeInternal = new CalcNode(source, id));
            }
            return nodeInternal;
        }

        public IEnumerable<CalcNode> EnumerateIntersectedNodesExcludeSelf(CalcEvaluatorContext context, CalcLocalIdentity id)
        {
            if (!object.ReferenceEquals(context, null) && !object.ReferenceEquals(id, null))
            {
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
                        foreach (KeyValuePair<CalcRangeIdentity, CalcNode> iteratorVariable3 in this._rowSharedIds)
                        {
                            yield return iteratorVariable3.Value;
                        }
                        foreach (KeyValuePair<CalcRangeIdentity, CalcNode> iteratorVariable4 in this._columnSharedIds)
                        {
                            yield return iteratorVariable4.Value;
                        }
                    }
                    else if (srcId.IsFullColumn || srcId.IsFullRow)
                    {
                        foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable5 in this._nodes)
                        {
                            if (!context.IsIntersected(srcId, iteratorVariable5.Key))
                            {
                                continue;
                            }
                            yield return iteratorVariable5.Value;
                        }
                        foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable6 in this._rangeNodeStorage)
                        {
                            if (!context.IsIntersected(srcId, iteratorVariable6.Key) || (srcId == iteratorVariable6.Key))
                            {
                                continue;
                            }
                            yield return iteratorVariable6.Value;
                        }
                        if (srcId.IsFullColumn)
                        {
                            foreach (KeyValuePair<CalcRangeIdentity, CalcNode> iteratorVariable7 in this._rowSharedIds)
                            {
                                yield return iteratorVariable7.Value;
                            }
                        }
                        if (srcId.IsFullRow)
                        {
                            foreach (KeyValuePair<CalcRangeIdentity, CalcNode> iteratorVariable8 in this._columnSharedIds)
                            {
                                yield return iteratorVariable8.Value;
                            }
                        }
                    }
                    else
                    {
                        for (int i = srcId.RowIndex; i < (srcId.RowIndex + srcId.RowCount); i++)
                        {
                            for (int j = srcId.ColumnIndex; j < (srcId.ColumnIndex + srcId.ColumnCount); j++)
                            {
                                CalcCellIdentity iteratorVariable11 = new CalcCellIdentity(i, j);
                                CalcNode nodeInternal = this.GetNodeInternal(iteratorVariable11);
                                if (nodeInternal != null)
                                {
                                    yield return nodeInternal;
                                }
                            }
                        }
                        foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable13 in this._rangeNodeStorage)
                        {
                            if (!context.IsIntersected(id, iteratorVariable13.Key) || (id == iteratorVariable13.Key))
                            {
                                continue;
                            }
                            yield return iteratorVariable13.Value;
                        }
                        foreach (KeyValuePair<CalcRangeIdentity, CalcNode> iteratorVariable14 in this._rowSharedIds)
                        {
                            if (!context.IsIntersected(id, iteratorVariable14.Key))
                            {
                                continue;
                            }
                            yield return iteratorVariable14.Value;
                        }
                        foreach (KeyValuePair<CalcRangeIdentity, CalcNode> iteratorVariable15 in this._columnSharedIds)
                        {
                            if (!context.IsIntersected(id, iteratorVariable15.Key))
                            {
                                continue;
                            }
                            yield return iteratorVariable15.Value;
                        }
                    }
                }
                else
                {
                    IEnumerator<KeyValuePair<CalcIdentity, CalcNode>> enumerator = this._rangeNodeStorage.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<CalcIdentity, CalcNode> iteratorVariable16 = enumerator.Current;
                        if (context.IsIntersected(id, iteratorVariable16.Key))
                        {
                            yield return iteratorVariable16.Value;
                        }
                    }
                }
            }
        }


        private void ExpandRangeIdentity(CalcRangeIdentity rangeId, CalcNode node)
        {
            if (!object.ReferenceEquals(rangeId, null))
            {
                if (rangeId.IsFullColumn && rangeId.IsFullRow)
                {
                    if (object.ReferenceEquals(this._fullSheetRangeDependency, null))
                    {
                        this._fullSheetRangeDependency = node;
                    }
                    this._fullSheetRangeDependencyRefCount++;
                }
                else if (rangeId.IsFullColumn)
                {
                    for (int i = 0; i < rangeId.ColumnCount; i++)
                    {
                        this._columnDependency[rangeId.ColumnIndex + i] = node;
                    }
                }
                else if (rangeId.IsFullRow)
                {
                    for (int j = 0; j < rangeId.RowCount; j++)
                    {
                        this._rowDependency[rangeId.RowIndex + j] = node;
                    }
                }
                else if (!IsLargeArray(rangeId))
                {
                    for (int k = 0; k < rangeId.RowCount; k++)
                    {
                        Dictionary<int, List<CalcNode>> dictionary;
                        if (!this._expandedRangeDependency.TryGetValue(k + rangeId.RowIndex, out dictionary) || (dictionary == null))
                        {
                            this._expandedRangeDependency[k + rangeId.RowIndex] = dictionary = new Dictionary<int, List<CalcNode>>();
                        }
                        for (int m = 0; m < rangeId.ColumnCount; m++)
                        {
                            List<CalcNode> list = null;
                            if (!dictionary.TryGetValue(m + rangeId.ColumnIndex, out list) || (list == null))
                            {
                                list = new List<CalcNode>();
                                dictionary[m + rangeId.ColumnIndex] = list;
                            }
                            list.Add(node);
                        }
                    }
                }
                else
                {
                    this._bigRangeDependency[rangeId] = node;
                }
            }
        }

        private static CalcIdentity ExpandReferencedIdForSharedFormula(CalcRangeIdentity rangeId, CalcReferenceExpression expr)
        {
            bool flag;
            bool flag2;
            bool flag3;
            bool flag4;
            NeedExpandForExpr(expr, out flag, out flag3, out flag2, out flag4);
            if ((!flag && !flag3) && (!flag2 && !flag4))
            {
                return expr.GetId(0, 0);
            }
            int row = ((rangeId.RowIndex < 0) || !flag) ? 0 : rangeId.RowIndex;
            int column = ((rangeId.ColumnIndex < 0) || !flag3) ? 0 : rangeId.ColumnIndex;
            CalcIdentity id = expr.GetId(row, column);
            if ((rangeId.RowCount == 1) && (rangeId.ColumnCount == 1))
            {
                return id;
            }
            row = ((flag2 ? rangeId.RowIndex : 0) + rangeId.RowCount) - 1;
            column = ((flag4 ? rangeId.ColumnIndex : 0) + rangeId.ColumnCount) - 1;
            row = (row < 0) ? 0 : row;
            column = (column < 0) ? 0 : column;
            CalcIdentity identity2 = expr.GetId(row, column);
            if (id.GetType() != identity2.GetType())
            {
                return id;
            }
            if (id is CalcCellIdentity)
            {
                CalcCellIdentity identity3 = id as CalcCellIdentity;
                CalcCellIdentity identity4 = identity2 as CalcCellIdentity;
                if (rangeId.IsFullRow)
                {
                    return new CalcRangeIdentity(identity3.RowIndex, (identity4.RowIndex - identity3.RowIndex) + 1, true);
                }
                if (rangeId.IsFullColumn)
                {
                    return new CalcRangeIdentity(identity3.ColumnIndex, (identity4.ColumnIndex - identity3.ColumnIndex) + 1, false);
                }
                return new CalcRangeIdentity(identity3.RowIndex, identity3.ColumnIndex, (identity4.RowIndex - identity3.RowIndex) + 1, (identity4.ColumnIndex - identity3.ColumnIndex) + 1);
            }
            if (id is CalcRangeIdentity)
            {
                CalcRangeIdentity identity5 = id as CalcRangeIdentity;
                CalcRangeIdentity identity6 = identity2 as CalcRangeIdentity;
                if (rangeId.IsFullRow)
                {
                    return new CalcRangeIdentity(identity5.RowIndex, (identity6.RowIndex + identity6.RowCount) - identity5.RowIndex, true);
                }
                if (rangeId.IsFullColumn)
                {
                    return new CalcRangeIdentity(identity5.ColumnIndex, (identity6.ColumnIndex + identity6.ColumnCount) - identity5.ColumnIndex, false);
                }
                return new CalcRangeIdentity(identity5.RowIndex, identity5.ColumnIndex, (identity6.RowIndex + identity6.RowCount) - identity5.RowIndex, (identity6.ColumnIndex + identity6.ColumnCount) - identity5.ColumnIndex);
            }
            if (id is CalcExternalCellIdentity)
            {
                CalcExternalCellIdentity identity7 = id as CalcExternalCellIdentity;
                CalcExternalCellIdentity identity8 = identity2 as CalcExternalCellIdentity;
                if (rangeId.IsFullRow)
                {
                    return new CalcExternalRangeIdentity(identity7.Source, identity7.RowIndex, (identity8.RowIndex - identity7.RowIndex) + 1, true);
                }
                if (rangeId.IsFullColumn)
                {
                    return new CalcExternalRangeIdentity(identity7.Source, identity7.ColumnIndex, (identity8.ColumnIndex - identity7.ColumnIndex) + 1, false);
                }
                return new CalcExternalRangeIdentity(identity7.Source, identity7.RowIndex, identity7.ColumnIndex, (identity8.RowIndex - identity7.RowIndex) + 1, (identity8.ColumnIndex - identity7.ColumnIndex) + 1);
            }
            if (!(id is CalcExternalRangeIdentity))
            {
                return id;
            }
            CalcExternalRangeIdentity identity9 = id as CalcExternalRangeIdentity;
            CalcExternalRangeIdentity identity10 = identity2 as CalcExternalRangeIdentity;
            if (rangeId.IsFullRow)
            {
                return new CalcExternalRangeIdentity(identity9.Source, identity9.RowIndex, (identity10.RowIndex + identity10.RowCount) - identity9.RowIndex, true);
            }
            if (rangeId.IsFullColumn)
            {
                return new CalcExternalRangeIdentity(identity9.Source, identity9.ColumnIndex, (identity10.ColumnIndex + identity10.ColumnCount) - identity9.ColumnIndex, false);
            }
            return new CalcExternalRangeIdentity(identity9.Source, identity9.RowIndex, identity9.ColumnIndex, (identity10.RowIndex + identity10.RowCount) - identity9.RowIndex, (identity10.ColumnIndex + identity10.ColumnCount) - identity9.ColumnIndex);
        }

        /// <summary>
        /// Gets all array formula identities.
        /// </summary>
        /// <returns></returns>
        public ICollection<CalcRangeIdentity> GetAllArrayFormulaIdentities()
        {
            return this._arrayFormulaIds.Keys;
        }

        private List<CalcNode> GetAllContainsCellWithExpressions(CalcEvaluatorContext context, CalcRangeIdentity range)
        {
            List<CalcNode> list = new List<CalcNode>();
            if (range.IsFullColumn && range.IsFullRow)
            {
                foreach (KeyValuePair<CalcIdentity, CalcNode> pair in this._nodes)
                {
                    list.Add(pair.Value);
                }
                return list;
            }
            if (range.IsFullColumn || range.IsFullRow)
            {
                foreach (KeyValuePair<CalcIdentity, CalcNode> pair2 in this._nodes)
                {
                    if (context.IsIntersected(range, pair2.Key))
                    {
                        list.Add(pair2.Value);
                    }
                }
                return list;
            }
            for (int i = range.RowIndex; i < (range.RowIndex + range.RowCount); i++)
            {
                for (int j = range.ColumnIndex; j < (range.ColumnIndex + range.ColumnCount); j++)
                {
                    CalcCellIdentity id = new CalcCellIdentity(i, j);
                    CalcNode nodeInternal = this.GetNodeInternal(id);
                    if ((nodeInternal != null) && (this.Manager.GetExpression(id) != null))
                    {
                        list.Add(nodeInternal);
                    }
                }
            }
            return list;
        }

        internal ICollection<CalcNode> GetAllDependentRangeNodes(int row, int column, int rowCount = 1, int columnCount = 1)
        {
            List<CalcNode> list = new List<CalcNode>();
            if (this._fullSheetRangeDependency != null)
            {
                list.Add(this._fullSheetRangeDependency);
            }
            for (int i = 0; i < rowCount; i++)
            {
                CalcNode node;
                if (((this._rowDependency.Count > 0) && this._rowDependency.TryGetValue(i + row, out node)) && !object.ReferenceEquals(node, null))
                {
                    list.Insert(0, node);
                }
                Dictionary<int, List<CalcNode>> dictionary = null;
                bool flag = ((this._expandedRangeDependency.Count > 0) && this._expandedRangeDependency.TryGetValue(i + row, out dictionary)) && (dictionary != null);
                for (int j = 0; j < columnCount; j++)
                {
                    if (((i == 0) && (this._columnDependency.Count > 0)) && (this._columnDependency.TryGetValue(j + column, out node) && !object.ReferenceEquals(node, null)))
                    {
                        list.Insert(0, node);
                    }
                    if (flag)
                    {
                        List<CalcNode> list2 = new List<CalcNode>();
                        if (((dictionary.Count > 0) && dictionary.TryGetValue(j + column, out list2)) && !object.ReferenceEquals(list2, null))
                        {
                            list.AddRange(list2);
                        }
                    }
                }
            }
            if (this._bigRangeDependency.Count > 0)
            {
                CalcRangeIdentity srcId = new CalcRangeIdentity(row, column, rowCount, columnCount);
                foreach (KeyValuePair<CalcRangeIdentity, CalcNode> pair in this._bigRangeDependency)
                {
                    if (this.EvaluatorContext.IsIntersected(srcId, pair.Key))
                    {
                        list.Add(pair.Value);
                    }
                }
            }
            return list;
        }

        internal ICollection<CalcNode> GetAllDependentSharedNodes(CalcLocalIdentity id)
        {
            int rowIndex;
            int columnIndex;
            int rowCount;
            int columnCount;
            CalcCellIdentity objA = id as CalcCellIdentity;
            CalcRangeIdentity identity2 = id as CalcRangeIdentity;
            if (!object.ReferenceEquals(objA, null))
            {
                rowIndex = objA.RowIndex;
                columnIndex = objA.ColumnIndex;
                rowCount = columnCount = 1;
            }
            else
            {
                if (object.ReferenceEquals(identity2, null))
                {
                    return new List<CalcNode>();
                }
                if (identity2.IsFullColumn && identity2.IsFullRow)
                {
                    rowIndex = columnIndex = rowCount = columnCount = -1;
                }
                else if (identity2.IsFullColumn)
                {
                    rowIndex = identity2.RowIndex;
                    rowCount = identity2.RowCount;
                    columnIndex = columnCount = -1;
                }
                else if (identity2.IsFullRow)
                {
                    columnIndex = identity2.ColumnIndex;
                    columnCount = identity2.ColumnCount;
                    rowIndex = rowCount = -1;
                }
                else
                {
                    rowIndex = identity2.RowIndex;
                    rowCount = identity2.RowCount;
                    columnIndex = identity2.ColumnIndex;
                    columnCount = identity2.ColumnCount;
                }
            }
            return this.GetAllDependentSharedNodes(rowIndex, columnIndex, rowCount, columnCount);
        }

        internal ICollection<CalcNode> GetAllDependentSharedNodes(int row, int column, int rowCount = 1, int columnCount = 1)
        {
            List<CalcNode> list = new List<CalcNode>();
            if (row >= 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    CalcNode node;
                    if (this._rowSharedIds.TryGetValue(new CalcRangeIdentity(row + i, 1, true), out node) && !node.IsDirty)
                    {
                        list.Add(node);
                    }
                }
            }
            if (column >= 0)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    CalcNode node2;
                    if (this._columnSharedIds.TryGetValue(new CalcRangeIdentity(column + j, 1, false), out node2) && !node2.IsDirty)
                    {
                        list.Add(node2);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gets all local identities.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CalcLocalIdentity> GetAllLocalIdentities()
        {
            foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable0 in this._rangeNodeStorage)
            {
                CalcLocalIdentity objA = iteratorVariable0.Key as CalcLocalIdentity;
                if (!object.ReferenceEquals(objA, null))
                {
                    yield return objA;
                }
            }
            foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable2 in this._nodes)
            {
                CalcLocalIdentity iteratorVariable3 = iteratorVariable2.Key as CalcLocalIdentity;
                if (!object.ReferenceEquals(iteratorVariable3, null))
                {
                    yield return iteratorVariable3;
                }
            }
        }


        /// <summary>
        /// Gets all sheetRange identities.
        /// </summary>
        /// <returns></returns>
        public ICollection<CalcLocalIdentity> GetAllSheetRangeIdentities()
        {
            return this._sheetRangeIds.Keys;
        }

        internal ICollection<CalcLocalIdentity> GetAllVolantedIdeneities()
        {
            return this._volatileIds;
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <returns></returns>
        internal ICollection<CalcNode> GetAllVolantedNodes()
        {
            return (ICollection<CalcNode>)this._volatileNodes;
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="id"></param>
        /// <param name="containsRanges"></param>
        /// <param name="intersectedRanges"></param>
        internal void GetArrayFormulaByRange(CalcRangeIdentity id, out List<CalcRangeIdentity> containsRanges, out List<CalcRangeIdentity> intersectedRanges)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            containsRanges = new List<CalcRangeIdentity>();
            intersectedRanges = new List<CalcRangeIdentity>();
            CalcNode nodeInternal = this.GetNodeInternal(id);
            if ((!object.ReferenceEquals(nodeInternal, null) && (nodeInternal.NodeType != NodeType.None)) && (this.Manager.GetExpression(id) != null))
            {
                containsRanges.Add(id);
            }
            else
            {
                ICalcSource source = this.Manager.Source;
                int rowIndex = id.RowIndex;
                int columnIndex = id.ColumnIndex;
                int num3 = id.IsFullColumn ? source.GetRowCount() : id.RowCount;
                int num4 = id.IsFullRow ? source.GetColumnCount() : id.ColumnCount;
                for (int i = 0; i < num3; i++)
                {
                    for (int j = 0; j < num4; j++)
                    {
                        CalcNode objA = this.GetNodeInternal(new CalcCellIdentity(rowIndex + i, columnIndex + j));
                        if (!object.ReferenceEquals(objA, null) && (objA.NodeType == NodeType.ArrayFormulaPart))
                        {
                            CalcRangeIdentity identity = objA.OwnerNode.Id as CalcRangeIdentity;
                            if (!containsRanges.Contains(identity) && !intersectedRanges.Contains(identity))
                            {
                                if (((identity.RowIndex >= rowIndex) && ((identity.RowIndex + identity.RowCount) <= (rowIndex + num3))) && ((identity.ColumnIndex >= columnIndex) && ((identity.ColumnIndex + identity.ColumnCount) <= (columnIndex + num4))))
                                {
                                    containsRanges.Add(identity);
                                }
                                else
                                {
                                    intersectedRanges.Add(identity);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<CalcNode> GetAllRangeNodes()
        {
            foreach (KeyValuePair<CalcIdentity, CalcNode> iteratorVariable0 in this._rangeNodeStorage)
            {
                yield return iteratorVariable0.Value;
            }
        }

        internal CalcRangeIdentity GetArrayFormulaRange(CalcLocalIdentity id, bool direct = false)
        {
            List<CalcRangeIdentity> list;
            if (this.GetArrayFormulaRange(id, out list, direct) == RangeRelative.Intersectant)
            {
                return list[0];
            }
            return null;
        }

        private RangeRelative GetArrayFormulaRange(CalcLocalIdentity id, out List<CalcRangeIdentity> containsRanges, bool direct = false)
        {
            containsRanges = new List<CalcRangeIdentity>();
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            if (direct || this.IsIsIntersectantWithArrayFormula(id))
            {
                int rowIndex;
                int columnIndex;
                int num3;
                int num4;
                CalcCellIdentity identity = id as CalcCellIdentity;
                CalcRangeIdentity identity2 = id as CalcRangeIdentity;
                if ((identity == null) && (identity2 == null))
                {
                    return RangeRelative.None;
                }
                if (identity != null)
                {
                    rowIndex = num3 = identity.RowIndex;
                    columnIndex = num4 = identity.ColumnIndex;
                }
                else
                {
                    if (identity2.IsFullColumn)
                    {
                        rowIndex = -2147483648;
                        num3 = 0x7fffffff;
                    }
                    else
                    {
                        rowIndex = identity2.RowIndex;
                        num3 = (identity2.RowIndex + identity2.RowCount) - 1;
                    }
                    if (identity2.IsFullRow)
                    {
                        columnIndex = -2147483648;
                        num4 = 0x7fffffff;
                    }
                    else
                    {
                        columnIndex = identity2.ColumnIndex;
                        num4 = (identity2.ColumnIndex + identity2.ColumnCount) - 1;
                    }
                }
                foreach (KeyValuePair<CalcRangeIdentity, CalcRangeIdentity> pair in this._arrayFormulaIds)
                {
                    CalcRangeIdentity key = pair.Key;
                    if (((identity2 != null) && (identity2.IsFullColumn || ((rowIndex <= key.RowIndex) && (num3 >= ((key.RowIndex + key.RowCount) - 1))))) && (identity2.IsFullRow || ((columnIndex <= key.ColumnIndex) && (num4 >= ((key.ColumnIndex + key.ColumnCount) - 1)))))
                    {
                        containsRanges.Add(key);
                    }
                    else if ((key.IsFullColumn || ((rowIndex < (key.RowIndex + key.RowCount)) && (num3 >= key.RowIndex))) && (key.IsFullRow || ((columnIndex < (key.ColumnIndex + key.ColumnCount)) && (num4 >= key.ColumnIndex))))
                    {
                        containsRanges.Add(key);
                        return RangeRelative.Intersectant;
                    }
                }
                if (containsRanges.Count != 0)
                {
                    return RangeRelative.Contains;
                }
            }
            return RangeRelative.None;
        }

        internal ICollection<CalcNode> GetBandDependentNodes(int index, int count, bool row)
        {
            Dictionary<int, CalcNode> dictionary = row ? this._rowDependency : this._columnDependency;
            List<CalcNode> list = new List<CalcNode>();
            if (dictionary.Count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    CalcNode node;
                    if ((dictionary.TryGetValue(i + index, out node) && !object.ReferenceEquals(node, null)) && !node.IsDirty)
                    {
                        list.Add(node);
                    }
                }
            }
            return list;
        }

        internal CalcNode GetNode(CalcCellIdentity cellId)
        {
            lock (this._syncRoot)
            {
                return this.GetNodeInternal(cellId);
            }
        }

        /// <summary>
        /// Gets the node stands the relationship for a position by <paramref name="id" />.
        /// </summary>
        /// <param name="id">The id present the position.</param>
        /// <returns>
        /// A <see cref="T:Dt.CalcEngine.CalcNode" /> indicate the node.
        /// if there is not setting any nodes by invoke <see cref="M:Dt.CalcEngine.CalcGraph.SetNode(Dt.CalcEngine.CalcLocalIdentity,Dt.CalcEngine.Expressions.CalcExpression,Dt.CalcEngine.CalcEvaluatorContext,Dt.CalcEngine.ICalcSource,System.Boolean)" /> for <paramref name="id" />, return <see langword="null" />.
        /// </returns>
        public CalcNode GetNode(CalcIdentity id)
        {
            lock (this._syncRoot)
            {
                return this.GetNodeInternal(id);
            }
        }

        private CalcNode GetNodeInternal(CalcCellIdentity id)
        {
            return this._nodes[id];
        }

        private CalcNode GetNodeInternal(CalcIdentity id)
        {
            CalcRangeIdentity key = id as CalcRangeIdentity;
            if ((key != null) && this._rowSharedIds.ContainsKey(key))
            {
                return this._rowSharedIds[key];
            }
            CalcRangeIdentity identity2 = id as CalcRangeIdentity;
            if ((identity2 != null) && this._columnSharedIds.ContainsKey(identity2))
            {
                return this._columnSharedIds[identity2];
            }
            CalcNode objA = this._nodes[id];
            if (!object.ReferenceEquals(objA, null))
            {
                return objA;
            }
            return this._rangeNodeStorage[id];
        }

        internal CalcRangeIdentity GetSharedFormulaRange(CalcCellIdentity cellId)
        {
            foreach (CalcRangeIdentity identity in this._rowSharedIds.Keys)
            {
                if ((cellId.RowIndex >= identity.RowIndex) && (cellId.RowIndex < (identity.RowIndex + identity.RowCount)))
                {
                    return identity;
                }
            }
            foreach (CalcRangeIdentity identity2 in this._columnSharedIds.Keys)
            {
                if ((cellId.ColumnIndex >= identity2.ColumnIndex) && (cellId.ColumnIndex < (identity2.ColumnIndex + identity2.ColumnCount)))
                {
                    return identity2;
                }
            }
            foreach (CalcRangeIdentity identity3 in this._largeSharedIds.Keys)
            {
                if (((cellId.ColumnIndex >= identity3.ColumnIndex) && (cellId.ColumnIndex < (identity3.ColumnIndex + identity3.ColumnCount))) && ((cellId.RowIndex >= identity3.RowIndex) && (cellId.RowIndex < (identity3.RowIndex + identity3.RowCount))))
                {
                    return identity3;
                }
            }
            return null;
        }

        internal bool IsIsIntersectantWithArrayFormula(CalcLocalIdentity id)
        {
            if (!this.HasArrayFormula)
            {
                return false;
            }
            bool flag = false;
            bool flag2 = false;
            CalcCellIdentity identity = id as CalcCellIdentity;
            CalcRangeIdentity identity2 = id as CalcRangeIdentity;
            if (identity != null)
            {
                if (identity.RowIndex >= 0)
                {
                    flag |= this._arrayFormulaRowFlags[identity.RowIndex] > 0;
                }
                if (identity.ColumnIndex >= 0)
                {
                    flag2 |= this._arrayFormulaColumnFlags[identity.ColumnIndex] > 0;
                }
            }
            else if (identity2 != null)
            {
                if (identity2.IsFullColumn)
                {
                    flag = true;
                }
                else
                {
                    for (int i = identity2.RowIndex; i < (identity2.RowIndex + identity2.RowCount); i++)
                    {
                        flag |= this._arrayFormulaRowFlags[i] > 0;
                    }
                }
                if (identity2.IsFullRow)
                {
                    flag2 = true;
                }
                else
                {
                    for (int j = identity2.ColumnIndex; j < (identity2.ColumnIndex + identity2.ColumnCount); j++)
                    {
                        flag2 |= this._arrayFormulaColumnFlags[j] > 0;
                    }
                }
            }
            if (((!flag || !flag2) && (!flag || (this._arrayFormulaColumnFlags[0x4000] <= 0))) && (!flag2 || (this._arrayFormulaRowFlags[0x100000] <= 0)))
            {
                return false;
            }
            return (this.GetArrayFormulaRange(id, true) != null);
        }

        private bool IsIsolatedNode(CalcNode node)
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

        internal static bool IsLargeArray(CalcRangeIdentity rangeId)
        {
            if ((rangeId.RowCount < 0x80) && (rangeId.ColumnCount < 0x80))
            {
                return ((rangeId.RowCount * rangeId.ColumnCount) >= 0x80);
            }
            return true;
        }

        /// <summary>
        /// Gets all shared formula identities.
        /// </summary>
        /// <returns></returns>
        internal bool IsSharedFormula(CalcRangeIdentity rangeId)
        {
            if (rangeId.IsFullRow)
            {
                return this._rowSharedIds.ContainsKey(rangeId);
            }
            if (rangeId.IsFullColumn)
            {
                return this._columnSharedIds.ContainsKey(rangeId);
            }
            return this._largeSharedIds.ContainsKey(rangeId);
        }

        private static void NeedExpandForExpr(CalcReferenceExpression expr, out bool startRow, out bool startCol, out bool endRow, out bool endCol)
        {
            bool flag;
            bool flag2;
            endCol = flag = false;
            endRow = flag2 = flag;
            startRow = startCol = flag2;
            CalcCellExpression expression = expr as CalcCellExpression;
            if (expression != null)
            {
                startRow = endRow = expression.RowRelative;
                startCol = endCol = expression.ColumnRelative;
            }
            CalcRangeExpression expression2 = expr as CalcRangeExpression;
            if (expression2 != null)
            {
                startRow = expression2.StartRowRelative;
                startCol = expression2.StartColumnRelative;
                endRow = expression2.EndRowRelative;
                endCol = expression2.EndColumnRelative;
            }
            CalcExternalCellExpression expression3 = expr as CalcExternalCellExpression;
            if (expression3 != null)
            {
                startRow = endRow = expression3.RowRelative;
                startCol = endCol = expression3.ColumnRelative;
            }
            CalcExternalRangeExpression expression4 = expr as CalcExternalRangeExpression;
            if (expression4 != null)
            {
                startRow = expression4.StartRowRelative;
                startCol = expression4.StartColumnRelative;
                endRow = expression4.EndRowRelative;
                endCol = expression4.EndColumnRelative;
            }
            CalcSheetRangeExpression expression5 = expr as CalcSheetRangeExpression;
            if (expression5 != null)
            {
                startRow = expression5.StartRowRelative;
                startCol = expression5.StartColumnRelative;
                endRow = expression5.EndRowRelative;
                endCol = expression5.EndColumnRelative;
            }
        }

        private List<CalcNode> ParsePrecedents(CalcNode current, CalcExpression expr, CalcEvaluatorContext context, bool isArrayFormula)
        {
            current.IsVolatile = false;
            if (expr is CalcSharedExpression)
            {
                expr = (expr as CalcSharedExpression).Expression;
            }
            while (expr is CalcParenthesesExpression)
            {
                expr = (expr as CalcParenthesesExpression).Arg;
            }
            List<CalcNode> list = new List<CalcNode>();
            List<CalcIdentity> theRefs = new List<CalcIdentity>();
            if (expr is CalcSheetRangeExpression)
            {
                CalcSheetRangeExpression sheetRangeExpr = expr as CalcSheetRangeExpression;
                this._sheetRangeIds[current.Id as CalcLocalIdentity] = current.Id as CalcLocalIdentity;
                this.ParseSheetRangePrecedents(current.Id, theRefs, sheetRangeExpr, isArrayFormula);
            }
            else if (expr is CalcReferenceExpression)
            {
                ParseReferencePrecedents(current.Id, expr as CalcReferenceExpression, theRefs, isArrayFormula);
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
                    return this.ParsePrecedents(current, (expr as CalcUnaryOperatorExpression).Operand, context, isArrayFormula);
                }
                if (expr is CalcBinaryOperatorExpression)
                {
                    CalcBinaryOperatorExpression expression3 = expr as CalcBinaryOperatorExpression;
                    List<CalcNode> collection = this.ParsePrecedents(current, expression3.Left, context, isArrayFormula);
                    List<CalcNode> list4 = this.ParsePrecedents(current, expression3.Right, context, isArrayFormula);
                    if ((collection != null) && (list4 != null))
                    {
                        List<CalcNode> list5 = new List<CalcNode>(collection.Count + list4.Count);
                        list5.AddRange(collection);
                        list5.AddRange(list4);
                        return list5;
                    }
                    if (collection != null)
                    {
                        return collection;
                    }
                    if (list4 != null)
                    {
                        return list4;
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
                        theRefs.Add(new CalcFunctionIdentity(expression4.FunctionName));
                    }
                    if ((function != null) && function.IsVolatile())
                    {
                        current.IsVolatile = true;
                        this._volatileIds.Add(current.Id as CalcLocalIdentity);
                    }
                    if ((function == null) || ((function.MinArgs <= objArray.Length) && (objArray.Length <= function.MaxArgs)))
                    {
                        List<CalcNode> list6 = new List<CalcNode>();
                        for (int i = 0; i < objArray.Length; i++)
                        {
                            List<CalcNode> list7 = this.ParsePrecedents(current, expression4.GetArg(i), context, isArrayFormula);
                            if ((list7 != null) && (list7.Count > 0))
                            {
                                list6.AddRange(list7);
                            }
                        }
                        if (list6.Count > 0)
                        {
                            list.AddRange(list6);
                        }
                    }
                }
            }
            foreach (CalcIdentity identity in theRefs)
            {
                CalcIdentity id = identity;
                CalcExternalIdentity objA = id as CalcExternalIdentity;
                CalcGraph graph = this;
                ICalcSource source = (this.Manager != null) ? this.Manager.Source : null;
                if (!object.ReferenceEquals(objA, null) && (this.Manager != null))
                {
                    id = objA.ConvertToLocal();
                    if (objA.Source != this.Manager.Source)
                    {
                        source = objA.Source;
                        graph = this.Manager.Service.GetCalculationManager(source, null, true).Graph;
                    }
                }
                CalcNode item = graph.EnsureNode(id, source);
                if (item.Dependents == null)
                {
                    item.Dependents = new Dictionary<CalcNode, CalcNode>();
                }
                item.Dependents[current] = current;
                list.Add(item);
            }
            if (expr is CalcNameExpression)
            {
                CalcExpression name = context.GetName((expr as CalcNameExpression).Name);
                if (name != null)
                {
                    List<CalcNode> list8 = this.ParsePrecedents(current, name, context, isArrayFormula);
                    if ((list8 != null) && (list8.Count > 0))
                    {
                        list.AddRange(list8);
                    }
                }
                return list;
            }
            if (expr is CalcExternalNameExpression)
            {
                CalcExpression expression6 = context.GetName((expr as CalcExternalNameExpression).Name);
                CalcGraph graph2 = this.Manager.Service.GetCalculationManager((expr as CalcExternalNameExpression).Source, null, true).Graph;
                if (expression6 != null)
                {
                    List<CalcNode> list9 = graph2.ParsePrecedents(current, expression6, context, isArrayFormula);
                    if ((list9 != null) && (list9.Count > 0))
                    {
                        list.AddRange(list9);
                    }
                }
            }
            return list;
        }

        private static void ParseReferencePrecedents(CalcIdentity currentId, CalcReferenceExpression expr, List<CalcIdentity> theRefs, bool isArrayFormula)
        {
            CalcIdentity id;
            int rowIndex;
            int columnIndex;
            CalcCellIdentity identity2 = currentId as CalcCellIdentity;
            CalcRangeIdentity rangeId = currentId as CalcRangeIdentity;
            if (identity2 != null)
            {
                rowIndex = identity2.RowIndex;
                columnIndex = identity2.ColumnIndex;
            }
            else
            {
                rowIndex = rangeId.RowIndex;
                columnIndex = rangeId.ColumnIndex;
            }
            if ((rangeId != null) && !isArrayFormula)
            {
                id = ExpandReferencedIdForSharedFormula(rangeId, expr);
            }
            else
            {
                id = expr.GetId(rowIndex, columnIndex);
            }
            CalcCellIdentity identity4 = id as CalcCellIdentity;
            CalcRangeIdentity identity5 = id as CalcRangeIdentity;
            CalcExternalCellIdentity identity6 = id as CalcExternalCellIdentity;
            CalcExternalRangeIdentity identity7 = id as CalcExternalRangeIdentity;
            if ((((identity4 == null) || ((identity4.RowIndex >= 0) && (identity4.ColumnIndex >= 0))) && ((identity5 == null) || ((identity5.IsFullRow || (identity5.ColumnIndex >= 0)) && (identity5.IsFullColumn || (identity5.RowIndex >= 0))))) && (((identity6 == null) || ((identity6.RowIndex >= 0) && (identity6.ColumnIndex >= 0))) && ((identity7 == null) || ((identity7.IsFullRow || (identity7.ColumnIndex >= 0)) && (identity7.IsFullColumn || (identity7.RowIndex >= 0))))))
            {
                theRefs.Add(id);
            }
        }

        private void ParseSheetRangePrecedents(CalcIdentity currentId, List<CalcIdentity> theRefs, CalcSheetRangeExpression sheetRangeExpr, bool isArrayFormula)
        {
            IMultiSourceProvider provider = this.Manager.Source as IMultiSourceProvider;
            if (provider != null)
            {
                foreach (ICalcSource source in provider.GetCalcSources(sheetRangeExpr.StartSource, sheetRangeExpr.EndSource))
                {
                    CalcReferenceExpression expression;
                    if ((sheetRangeExpr.EndRow == -2147483648) || (sheetRangeExpr.EndColumn == -2147483648))
                    {
                        expression = new CalcExternalCellExpression(source, sheetRangeExpr.StartRow, sheetRangeExpr.StartColumn, sheetRangeExpr.StartRowRelative, sheetRangeExpr.StartColumnRelative);
                    }
                    else
                    {
                        switch (sheetRangeExpr.RangeType)
                        {
                            case CalcRangeType.Cell:
                                expression = new CalcExternalRangeExpression(source, sheetRangeExpr.StartRow, sheetRangeExpr.StartColumn, sheetRangeExpr.EndRow, sheetRangeExpr.EndColumn, sheetRangeExpr.StartRowRelative, sheetRangeExpr.StartColumnRelative, sheetRangeExpr.EndRowRelative, sheetRangeExpr.EndColumnRelative);
                                goto Label_015D;

                            case CalcRangeType.Row:
                                expression = new CalcExternalRangeExpression(source, sheetRangeExpr.StartRow, sheetRangeExpr.EndRow, sheetRangeExpr.StartRowRelative, sheetRangeExpr.EndRowRelative, true);
                                goto Label_015D;

                            case CalcRangeType.Column:
                                expression = new CalcExternalRangeExpression(source, sheetRangeExpr.StartColumn, sheetRangeExpr.EndColumn, sheetRangeExpr.StartColumnRelative, sheetRangeExpr.EndColumnRelative, false);
                                goto Label_015D;

                            case CalcRangeType.Sheet:
                                expression = new CalcExternalRangeExpression(source);
                                goto Label_015D;
                        }
                        expression = new CalcExternalRangeExpression(source, sheetRangeExpr.StartRow, sheetRangeExpr.StartColumn, sheetRangeExpr.EndRow, sheetRangeExpr.EndColumn, sheetRangeExpr.StartRowRelative, sheetRangeExpr.StartColumnRelative, sheetRangeExpr.EndRowRelative, sheetRangeExpr.EndColumnRelative);
                    }
                Label_015D:
                    ParseReferencePrecedents(currentId, expression, theRefs, isArrayFormula);
                }
            }
        }

        private void ProcessIntersectedSharedFormula(CalcRangeIdentity currentRange, CalcRangeIdentity oldRange, List<CalcRangeIdentity> removedIds, Dictionary<CalcRangeIdentity, CalcExpression> addedIds)
        {
            if (currentRange == oldRange)
            {
                removedIds.Add(currentRange);
            }
            else if (((!oldRange.IsFullRow || !oldRange.IsFullColumn) && (((((currentRange.RowIndex + currentRange.RowCount) > oldRange.RowIndex) && ((currentRange.ColumnIndex + currentRange.ColumnCount) > oldRange.ColumnIndex)) && ((currentRange.RowIndex < (oldRange.RowIndex + oldRange.RowCount)) && (currentRange.ColumnIndex < (oldRange.ColumnIndex + oldRange.ColumnCount)))) && (!oldRange.IsFullColumn || currentRange.IsFullColumn))) && (!oldRange.IsFullRow || currentRange.IsFullRow))
            {
                if ((currentRange.IsFullColumn || ((currentRange.RowIndex <= oldRange.RowIndex) && ((currentRange.RowIndex + currentRange.RowCount) >= (oldRange.RowIndex + oldRange.RowCount)))) && (currentRange.IsFullRow || ((currentRange.ColumnIndex <= oldRange.ColumnIndex) && ((currentRange.ColumnIndex + currentRange.ColumnCount) >= (oldRange.ColumnIndex + oldRange.ColumnCount)))))
                {
                    removedIds.Add(oldRange);
                }
                else if (currentRange.IsFullColumn || ((currentRange.RowIndex < oldRange.RowIndex) && ((currentRange.RowIndex + currentRange.RowCount) > (oldRange.RowIndex + oldRange.RowCount))))
                {
                    removedIds.Add(oldRange);
                    int rowIndex = oldRange.RowIndex;
                    int rowCount = oldRange.RowCount;
                    this.AddLeftRightPartExpression(currentRange, oldRange, addedIds, rowIndex, rowCount);
                }
                else
                {
                    removedIds.Add(oldRange);
                    if (currentRange.RowIndex > oldRange.RowIndex)
                    {
                        int row = currentRange.RowIndex;
                        int num4 = (currentRange.RowIndex - oldRange.RowIndex) + 1;
                        this.AddTopOrBottomPartExpression(oldRange, addedIds, row, num4);
                    }
                    else if (!currentRange.IsFullRow && !oldRange.IsFullRow)
                    {
                        this.AddLeftRightPartExpression(currentRange, oldRange, addedIds, oldRange.RowIndex, (currentRange.RowIndex + currentRange.RowCount) - oldRange.RowIndex);
                    }
                    if ((currentRange.RowIndex + currentRange.RowCount) < (oldRange.RowIndex + oldRange.RowCount))
                    {
                        int num5 = (currentRange.RowIndex + currentRange.RowCount) - 1;
                        int num6 = (oldRange.RowIndex + oldRange.ColumnCount) - num5;
                        this.AddTopOrBottomPartExpression(oldRange, addedIds, num5, num6);
                    }
                    else if (!currentRange.IsFullRow && !oldRange.IsFullRow)
                    {
                        this.AddLeftRightPartExpression(currentRange, oldRange, addedIds, currentRange.RowIndex, (oldRange.RowIndex + oldRange.RowCount) - currentRange.RowIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the node, so it will update all of the dependencies.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="invalidateFormula">if set to <c>true</c> [invalidate formula].</param>
        public void RefreshNode(CalcIdentity id, bool invalidateFormula = false)
        {
            CalcNode node = this.GetNode(id);
            if (((node != null) && (node.Dependents != null)) && (node.Dependents.Count > 0))
            {
                List<CalcLocalIdentity> list = new List<CalcLocalIdentity>();
                foreach (CalcNode node2 in node.Dependents.Keys)
                {
                    CalcLocalIdentity objA = node2.Id as CalcLocalIdentity;
                    if (!object.ReferenceEquals(objA, null))
                    {
                        list.Add(objA);
                    }
                }
                foreach (CalcLocalIdentity identity2 in list)
                {
                    ICalcSource source = this.Manager.Source;
                    CalcExpression expression = this.Manager.GetExpression(identity2);
                    if (!object.ReferenceEquals(expression, null))
                    {
                        this.SetNode(identity2, expression, source.GetEvaluatorContext(identity2), source, !(expression is CalcSharedExpression));
                    }
                }
            }
        }

        private void RemoveArrayFormula(CalcRangeIdentity rangeId)
        {
            this._arrayFormulaIds.Remove(rangeId);
            if (!rangeId.IsFullColumn)
            {
                for (int i = rangeId.RowIndex; i < (rangeId.RowIndex + rangeId.RowCount); i++)
                {
                    this._arrayFormulaRowFlags[i]--;
                }
            }
            else
            {
                this._arrayFormulaRowFlags[0x100000]--;
            }
            if (!rangeId.IsFullRow)
            {
                for (int j = rangeId.ColumnIndex; j < (rangeId.ColumnIndex + rangeId.ColumnCount); j++)
                {
                    this._arrayFormulaColumnFlags[j]--;
                }
            }
            else
            {
                this._arrayFormulaColumnFlags[0x4000]--;
            }
        }

        private void RemoveExternalPrecedentsDependents()
        {
            List<CalcNode> list = new List<CalcNode>();
            new List<CalcIdentity>();
            ICalcSource source = this.Manager.Source;
            foreach (KeyValuePair<CalcIdentity, CalcNode> pair in this._nodes)
            {
                list.Add(pair.Value);
            }
            foreach (KeyValuePair<CalcIdentity, CalcNode> pair2 in this._rangeNodeStorage)
            {
                list.Add(pair2.Value);
            }
            foreach (CalcNode node in list)
            {
                if (node.Precedents != null)
                {
                    foreach (CalcNode node2 in node.Precedents)
                    {
                        if (node2.Source != source)
                        {
                            node2.Dependents.Remove(node);
                        }
                    }
                }
                if (node.Dependents != null)
                {
                    foreach (CalcNode node3 in node.Dependents.Keys)
                    {
                        if (node3.Source != source)
                        {
                            node3.Precedents.Remove(node);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the node at specified position by <paramref name="id" />.
        /// </summary>
        /// <param name="id">A <see cref="T:Dt.CalcEngine.CalcIdentity" /> indicates the position.</param>
        public void RemoveNode(CalcIdentity id)
        {
            lock (this._syncRoot)
            {
                CalcNode nodeInternal = this.GetNodeInternal(id);
                if (nodeInternal != null)
                {
                    CalcRangeIdentity key = id as CalcRangeIdentity;
                    if (key != null)
                    {
                        if (this._arrayFormulaIds.ContainsKey(key))
                        {
                            this.RemoveArrayFormula(key);
                        }
                        else
                        {
                            this.RemoveSharedFormula(key);
                        }
                    }
                    else if (this.IsIsIntersectantWithArrayFormula(id as CalcLocalIdentity))
                    {
                        ExceptionHelper.ThrowChangePartOfArrayFormulaException();
                    }
                    if (nodeInternal.Precedents != null)
                    {
                        foreach (CalcNode node2 in nodeInternal.Precedents)
                        {
                            this.RemovePredentsNode(nodeInternal, node2);
                        }
                        nodeInternal.Precedents.Clear();
                    }
                    if (this.IsIsolatedNode(nodeInternal))
                    {
                        this.RemoveNodeAt(id);
                    }
                }
            }
        }

        private void RemoveNodeAt(CalcIdentity id)
        {
            this._nodes.RemoveAt(id);
            this._rangeNodeStorage.RemoveAt(id);
            if (id is CalcLocalIdentity)
            {
                this._volatileIds.Remove(id as CalcLocalIdentity);
            }
            if (((id is CalcRangeIdentity) || (id is CalcExternalRangeIdentity)) || ((id is CalcSheetRangeIdentity) || (id is CalcStructReferenceIndentity)))
            {
                CalcRangeIdentity objA = id as CalcRangeIdentity;
                if (!object.ReferenceEquals(objA, null))
                {
                    if (!objA.IsFullColumn || !objA.IsFullRow)
                    {
                        if (objA.IsFullColumn)
                        {
                            for (int i = 0; i < objA.ColumnCount; i++)
                            {
                                this._columnDependency.Remove(objA.ColumnIndex + i);
                            }
                        }
                        else if (objA.IsFullRow)
                        {
                            for (int j = 0; j < objA.RowCount; j++)
                            {
                                this._rowDependency.Remove(objA.RowIndex + j);
                            }
                        }
                        else if (!IsLargeArray(objA))
                        {
                            for (int k = 0; k < objA.RowCount; k++)
                            {
                                Dictionary<int, List<CalcNode>> dictionary;
                                if (this._expandedRangeDependency.TryGetValue(k + objA.RowIndex, out dictionary) || (dictionary != null))
                                {
                                    for (int m = 0; m < objA.ColumnCount; m++)
                                    {
                                        List<CalcNode> list = null;
                                        if (dictionary.TryGetValue(m + objA.ColumnIndex, out list) && (list != null))
                                        {
                                            for (int n = 0; n < list.Count; n++)
                                            {
                                                if (list[n].Id == id)
                                                {
                                                    list.RemoveAt(n);
                                                    n--;
                                                }
                                            }
                                        }
                                        if ((list != null) && (list.Count == 0))
                                        {
                                            dictionary.Remove(m + objA.ColumnIndex);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            this._bigRangeDependency.Remove(objA);
                        }
                    }
                    else
                    {
                        if (!object.ReferenceEquals(this._fullSheetRangeDependency, null) && (this._fullSheetRangeDependencyRefCount > 0))
                        {
                            this._fullSheetRangeDependencyRefCount--;
                        }
                        if (this._fullSheetRangeDependencyRefCount == 0)
                        {
                            this._fullSheetRangeDependency = null;
                        }
                    }
                }
            }
            else if (id is CalcRangeIdentity)
            {
                this._rowDependency.Remove((id as CalcRangeIdentity).RowIndex);
            }
            else if (id is CalcRangeIdentity)
            {
                this._columnDependency.Remove((id as CalcRangeIdentity).ColumnIndex);
            }
        }

        private void RemovePredentsNode(CalcNode node, CalcNode predentsNode)
        {
            if (predentsNode.Dependents != null)
            {
                predentsNode.Dependents.Remove(node);
            }
            CalcCalculationManager manager = this.Manager.Service.GetCalculationManager(predentsNode.Source, null, true);
            if ((manager != null) && manager.Graph.IsIsolatedNode(predentsNode))
            {
                manager.Graph.RemoveNodeAt(predentsNode.Id);
            }
        }

        private void RemoveSharedFormula(CalcRangeIdentity rangeId)
        {
            if (rangeId.IsFullRow)
            {
                this._rowSharedIds.Remove(rangeId);
            }
            else if (rangeId.IsFullColumn)
            {
                this._columnSharedIds.Remove(rangeId);
            }
            else
            {
                this._largeSharedIds.Remove(rangeId);
            }
        }

        /// <summary>
        /// Sets the node and its expression to <see cref="T:Dt.CalcEngine.CalcGraph" /> under specified <see cref="T:Dt.CalcEngine.CalcEvaluatorContext" />.
        /// </summary>
        /// <param name="id">A <see cref="T:Dt.CalcEngine.CalcLocalIdentity" /> indicates the position.</param>
        /// <param name="expr">A <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> indicates the expression to position at <paramref name="id" />.</param>
        /// <param name="evalContext">The evaluator context.</param>
        /// <param name="source">The source.</param>
        /// <param name="isArrayFormula">Indicates that the expression is a array formula or not.</param>
        public void SetNode(CalcLocalIdentity id, CalcExpression expr, CalcEvaluatorContext evalContext, ICalcSource source, bool isArrayFormula)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            if (object.ReferenceEquals(expr, null))
            {
                throw new ArgumentNullException("expr");
            }
            lock (this._syncRoot)
            {
                CalcNode node = this.CreateNode(id, evalContext, source, isArrayFormula);
                this.SetNodeInternal(id, node);
                node.Precedents = this.ParsePrecedents(node, expr, evalContext, isArrayFormula);
            }
        }

        private void SetNodeInternal(CalcIdentity id, CalcNode node)
        {
            if (((id is CalcRangeIdentity) || (id is CalcExternalRangeIdentity)) || ((id is CalcSheetRangeIdentity) || (id is CalcStructReferenceIndentity)))
            {
                this._rangeNodeStorage[id] = node;
                this.ExpandRangeIdentity(id as CalcRangeIdentity, node);
            }
            else if (((id is CalcCellIdentity) || (id is CalcExternalCellIdentity)) || (((id is CalcNameIdentity) || (id is CalcExternalNameIdentity)) || (id is CalcFunctionIdentity)))
            {
                this._nodes[id] = node;
            }
            else
            {
                CalcRangeIdentity identity = id as CalcRangeIdentity;
                if (identity != null)
                {
                    this._rowSharedIds[identity] = node;
                }
                CalcRangeIdentity identity2 = id as CalcRangeIdentity;
                if (identity2 != null)
                {
                    this._columnSharedIds[identity2] = node;
                }
            }
        }

        private CalcEvaluatorContext EvaluatorContext
        {
            get
            {
                if (this._evaluatorContext == null)
                {
                    this._evaluatorContext = new CalcEvaluatorContext(this.Manager.Source, false, 0, 0, 1, 1);
                }
                return this._evaluatorContext;
            }
        }

        private bool HasArrayFormula
        {
            get
            {
                return (((this._arrayFormulaIds.Count > 0) && (this._arrayFormulaColumnFlags != null)) && (this._arrayFormulaRowFlags != null));
            }
        }

        /// <summary>
        /// Gets whether current <see cref="T:Dt.CalcEngine.CalcGraph" /> is empty or not.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if ((((this._nodes == null) || (this._nodes.Count <= 0)) && ((this._rangeNodeStorage == null) || (this._rangeNodeStorage.Count <= 0))) && ((this._rowSharedIds == null) || (this._rowSharedIds.Count <= 0)))
                {
                    if (this._columnSharedIds != null)
                    {
                        return (this._columnSharedIds.Count <= 0);
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        /// <value>The manager.</value>
        public CalcCalculationManager Manager { get; private set; }
        
        private enum RangeRelative
        {
            None,
            Contains,
            Intersectant
        }

        private class SimpleNodeStorage<T> : ICalcStorage<T, CalcNode>, IEnumerable<KeyValuePair<T, CalcNode>>, IEnumerable where T: IEqualityComparer<T>
        {
            private Dictionary<T, CalcNode> _storage;

            public SimpleNodeStorage()
            {
                this._storage = new Dictionary<T, CalcNode>();
            }

            public IEnumerator<KeyValuePair<T, CalcNode>> GetEnumerator()
            {
                return this._storage.GetEnumerator();
            }

            public void RemoveAt(T id)
            {
                this._storage.Remove(id);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach (KeyValuePair<T, CalcNode> iteratorVariable0 in this._storage)
                {
                    yield return new KeyValuePair<T, CalcNode>(iteratorVariable0.Key, iteratorVariable0.Value);
                }
            }


            public int Count
            {
                get
                {
                    return this._storage.Count;
                }
            }

            public CalcNode this[T id]
            {
                get
                {
                    CalcNode node;
                    this._storage.TryGetValue(id, out node);
                    return node;
                }
                set
                {
                    this._storage[id] = value;
                }
            }

            public ICollection<T> Keys
            {
                get
                {
                    return this._storage.Keys;
                }
            }
        }
    }
}

