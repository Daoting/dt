#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
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
    /// Represents the calculation manager which used for storing all formulas and their dependents.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class CalcCalculationManager : IDisposable
    {
        private ICalcStorage<CalcLocalIdentity, CalcExpression> _formulas;
        private Dictionary<CalcLocalIdentity, Tuple<CalcExpression, bool>> _tmpFormulas;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcCalculationManager" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="source">The source.</param>
        /// <param name="storage">The storage.</param>
        internal CalcCalculationManager(CalcService service, ICalcSource source, ICalcStorage<CalcLocalIdentity, CalcExpression> storage = null)
        {
            this.Graph = new CalcGraph(this);
            this.Evaluator = new CalcEvaluator();
            this.Parser = new CalcParser();
            this._formulas = storage ?? new SimpleFormulaStorage();
            this._tmpFormulas = new Dictionary<CalcLocalIdentity, Tuple<CalcExpression, bool>>();
            this.Source = source;
            this.Service = service;
        }

        /// <summary>
        /// Clears the expression.
        /// </summary>
        /// <param name="id">The id.</param>
        public void ClearExpression(CalcLocalIdentity id)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            if ((this.Service.IsGraphSuspended && (this._tmpFormulas.Count > 0)) && this._tmpFormulas.ContainsKey(id))
            {
                this._tmpFormulas.Remove(id);
            }
            else
            {
                this._formulas.RemoveAt(id);
                this.Graph.RemoveNode(id);
            }
        }

        /// <summary>
        /// Clears the formula.
        /// </summary>
        /// <param name="id">The id.</param>
        public void ClearFormula(CalcLocalIdentity id)
        {
            this.ClearExpression(id);
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="depNode"></param>
        internal void CreateDirtyItem(CalcNode depNode)
        {
            if (object.ReferenceEquals(depNode.Source, this.Source))
            {
                depNode.DirtyItem = new DirtyItem(this, depNode);
            }
            else
            {
                CalcGraph graph = this.Service.GetCalculationManager(depNode.Source, null, true).Graph;
                depNode.DirtyItem = new DirtyItem(graph.Manager, graph.GetNode(depNode.Id as CalcLocalIdentity));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Service.DisposeCalculationManager(this);
            this._formulas = new SimpleFormulaStorage();
            this._tmpFormulas.Clear();
            this.Graph.ClearNodes();
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="cellId"></param>
        /// <returns></returns>
        private IEnumerable<CalcNode> EnumerateIntersectedNodesExcludeSelf(CalcCellIdentity cellId)
        {
            foreach (CalcNode iteratorVariable0 in this.Graph.GetAllRangeNodes())
            {
                CalcRangeIdentity id = iteratorVariable0.Id as CalcRangeIdentity;
                if ((!object.ReferenceEquals(id, null) && (((cellId.RowIndex >= id.RowIndex) && (cellId.RowIndex < (id.RowIndex + id.RowCount))) || id._isFullColumn)) && (((cellId.ColumnIndex >= id.ColumnIndex) && (cellId.ColumnIndex < (id.ColumnIndex + id.ColumnCount))) || id._isFullRow))
                {
                    yield return iteratorVariable0;
                }
            }
        }

        private void ExpandSharedFormula(CalcExpression expr, CalcRangeIdentity rangeId)
        {
            for (int i = rangeId.RowIndex; i < (rangeId.RowIndex + rangeId.RowCount); i++)
            {
                for (int j = rangeId.ColumnIndex; j < (rangeId.ColumnIndex + rangeId.ColumnCount); j++)
                {
                    CalcCellIdentity id = new CalcCellIdentity(i, j);
                    this.SetExpression(id, expr, false);
                }
            }
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="header"></param>
        /// <param name="tailItem"></param>
        /// <param name="addingItem"></param>
        /// <param name="predencyNode"></param>
        private static void AddDirtyItem(ref DirtyItem header, ref DirtyItem tailItem, DirtyItem addingItem, CalcNode predencyNode)
        {
            if (header == null)
            {
                header = tailItem = addingItem;
            }
            else
            {
                addingItem.PreviousItem = tailItem;
                tailItem.NextItem = addingItem;
                tailItem = addingItem;
            }
            if (!object.ReferenceEquals(predencyNode, null) && (predencyNode.DirtyItem != null))
            {
                DirtyItem dirtyItem = predencyNode.DirtyItem;
                if (dirtyItem.DependencyCellTemp == null)
                {
                    dirtyItem.DependencyCellTemp = new HashSet<CalcNode>();
                }
                if (!dirtyItem.DependencyCellTemp.Contains(tailItem.Node))
                {
                    dirtyItem.DependencyCellTemp.Add(tailItem.Node);
                    tailItem.PredencyItemCount++;
                }
            }
        }


        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="header"></param>
        /// <param name="tailItem"></param>
        private void GetAllDirtyItems(ref DirtyItem header, ref DirtyItem tailItem)
        {
            DirtyItem nextItem;
            DirtyItem removingItem = header;
            while (removingItem != null)
            {
                nextItem = removingItem.NextItem;
                DirtyItem previousItem = removingItem.PreviousItem;
                CalcNode objB = removingItem.Node;
                if (objB.NodeType != NodeType.None)
                {
                    objB.DirtyItem.DirtyFlag = true;
                }
                else
                {
                    objB.DirtyItem = null;
                    RemoveDirtyItem(ref header, ref tailItem, removingItem);
                }
                if ((objB.Dependents == null) || (objB.Dependents.Count == 0))
                {
                    removingItem = nextItem;
                }
                else
                {
                    foreach (KeyValuePair<CalcNode, CalcNode> pair in objB.Dependents)
                    {
                        if (((pair.Key.DirtyItem == null) && !object.ReferenceEquals(pair.Key, objB)) && ((pair.Key.Id is CalcCellIdentity) || (pair.Key.Id is CalcExternalCellIdentity)))
                        {
                            this.CreateDirtyItem(pair.Key);
                            AddDirtyItem(ref header, ref tailItem, pair.Key.DirtyItem, objB);
                            CalcLocalIdentity id = pair.Key.Id as CalcLocalIdentity;
                            CalcGraph graph = pair.Key.DirtyItem.Manager.Graph;
                            if (object.ReferenceEquals(id, null))
                            {
                                id = (pair.Key.Id as CalcExternalIdentity).ConvertToLocal();
                            }
                            foreach (CalcNode node2 in graph.Manager.EnumerateIntersectedNodesExcludeSelf(id as CalcCellIdentity))
                            {
                                node2.DirtyItem = new DirtyItem(this, node2);
                                AddDirtyItem(ref header, ref tailItem, node2.DirtyItem, pair.Key);
                            }
                        }
                    }
                    if (nextItem == null)
                    {
                        if ((previousItem == null) && (removingItem != header))
                        {
                            removingItem = header;
                        }
                        else if ((previousItem != null) && (removingItem != previousItem.NextItem))
                        {
                            removingItem = previousItem.NextItem;
                        }
                        else
                        {
                            removingItem = null;
                        }
                        continue;
                    }
                    removingItem = nextItem;
                }
            }
            for (removingItem = header; removingItem != null; removingItem = nextItem)
            {
                nextItem = removingItem.NextItem;
                CalcNode node = removingItem.Node;
                if (removingItem.Node.Dependents != null)
                {
                    foreach (KeyValuePair<CalcNode, CalcNode> pair2 in removingItem.Node.Dependents)
                    {
                        if ((pair2.Key.DirtyItem != null) && !object.ReferenceEquals(pair2.Key, node))
                        {
                            if (removingItem.DependencyCellTemp == null)
                            {
                                removingItem.DependencyCellTemp = new HashSet<CalcNode>();
                            }
                            if (!removingItem.DependencyCellTemp.Contains(pair2.Key))
                            {
                                removingItem.DependencyCellTemp.Add(pair2.Key);
                                pair2.Key.DirtyItem.PredencyItemCount++;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Gets the array formula.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="range">The range that the array formula was set.</param>
        /// <returns>A <see langword="string" /> indicates the array formula.</returns>
        public string GetArrayFormulaByCell(CalcCellIdentity id, out CalcRangeIdentity range)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            if (!this.IsIsIntersectantWithArrayFormula(id))
            {
                range = null;
                return null;
            }
            range = this.Graph.GetArrayFormulaRange(id, false);
            if (range == null)
            {
                return null;
            }
            return this.GetFormula(range);
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="id"></param>
        /// <param name="containsRanges"></param>
        /// <param name="intersectedRanges"></param>
        public void GetArrayFormulaByRange(CalcRangeIdentity id, out List<CalcRangeIdentity> containsRanges, out List<CalcRangeIdentity> intersectedRanges)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            this.Graph.GetArrayFormulaByRange(id, out containsRanges, out intersectedRanges);
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> indicates the expression.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="id" /> is <see langword="null" />.</exception>
        public CalcExpression GetExpression(CalcLocalIdentity id)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            CalcExpression expressionCore = this.GetExpressionCore(id);
            if (object.ReferenceEquals(expressionCore, null) && (id is CalcCellIdentity))
            {
                CalcRangeIdentity sharedFormulaRange = this.Graph.GetSharedFormulaRange(id as CalcCellIdentity);
                if (!object.ReferenceEquals(sharedFormulaRange, null))
                {
                    expressionCore = this.GetExpressionCore(sharedFormulaRange);
                    if (!object.ReferenceEquals(expressionCore, null) && !(expressionCore is CalcSharedExpression))
                    {
                        expressionCore = new CalcSharedExpression(expressionCore);
                    }
                }
            }
            return expressionCore;
        }

        private CalcExpression GetExpressionCore(CalcLocalIdentity id)
        {
            Tuple<CalcExpression, bool> tuple;
            CalcExpression objB = this._formulas[id];
            if ((object.ReferenceEquals(null, objB) && this.Service.IsGraphSuspended) && this._tmpFormulas.TryGetValue(id, out tuple))
            {
                objB = tuple.Item1;
            }
            return objB;
        }

        /// <summary>
        /// Gets the formula.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see langword="string" /> indicates the formula.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="id" /> is <see langword="null" />.</exception>
        public string GetFormula(CalcLocalIdentity id)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            CalcExpression objA = this.GetExpression(id);
            if (!object.ReferenceEquals(objA, null))
            {
                return this.Parser.Unparse(objA, this.Source.GetParserContext(id));
            }
            return null;
        }

        /// <summary>
        /// Invalidates the specified id.
        /// </summary>
        /// <param name="id">The data address.</param>
        /// <param name="autoCalculate">if set to <see langword="true" /> automatically calculate immediately.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="id" /> is <see langword="null" />.</exception>
        public void Invalidate(CalcLocalIdentity id, bool autoCalculate = true)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            if (!this.Graph.IsEmpty)
            {
                CalcNode objA = this.Graph.GetNode(id);
                if (!object.ReferenceEquals(objA, null))
                {
                    objA.MarkAsDirty(this.Service, autoCalculate, true, true, true);
                }
                else if (objA == null)
                {
                    CalcNode.CreateTempNode(this.Source, id).MarkAsDirty(this.Service, autoCalculate, true, true, true);
                }
                else
                {
                    CalcCellIdentity identity = id as CalcCellIdentity;
                    if (!object.ReferenceEquals(identity, null))
                    {
                        foreach (CalcNode node2 in this.Graph.GetAllDependentRangeNodes(identity.RowIndex, identity.ColumnIndex, 1, 1))
                        {
                            node2.MarkAsDirty(this.Service, autoCalculate, false, true, true);
                        }
                    }
                    else
                    {
                        CalcRangeIdentity identity2 = id as CalcRangeIdentity;
                        if (!object.ReferenceEquals(identity2, null))
                        {
                            if (identity2.IsFullColumn && identity2.IsFullRow)
                            {
                                this.InvalidateAllIdentity();
                                return;
                            }
                            if (!identity2.IsFullColumn && !identity2.IsFullRow)
                            {
                                foreach (CalcNode node3 in this.Graph.GetAllDependentRangeNodes(identity2.RowIndex, identity2.ColumnIndex, identity2.RowCount, identity2.ColumnCount))
                                {
                                    node3.MarkAsDirty(this.Service, autoCalculate, true, true, true);
                                }
                                for (int i = 0; i < identity2.RowCount; i++)
                                {
                                    for (int j = 0; j < identity2.ColumnCount; j++)
                                    {
                                        CalcNode node = this.Graph.GetNode(new CalcCellIdentity(identity2.RowIndex + i, identity2.ColumnIndex + j));
                                        if (!object.ReferenceEquals(node, null))
                                        {
                                            node.MarkAsDirty(this.Service, autoCalculate, false, true, true);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                int columnIndex;
                                int columnCount;
                                if (identity2.IsFullColumn)
                                {
                                    columnIndex = identity2.ColumnIndex;
                                    columnCount = identity2.ColumnCount;
                                }
                                else
                                {
                                    columnIndex = identity2.RowIndex;
                                    columnCount = identity2.RowCount;
                                }
                                foreach (CalcNode node5 in this.Graph.GetBandDependentNodes(columnIndex, columnCount, identity2.IsFullRow))
                                {
                                    node5.MarkAsDirty(this.Service, autoCalculate, true, true, true);
                                }
                                CalcReference reference = this.Evaluator.Evaluate(new CalcRangeExpression(columnIndex, columnCount, false, false, true), this.Source.GetEvaluatorContext(identity2)) as CalcReference;
                                if (!object.ReferenceEquals(reference, null) && (reference.RangeCount == 1))
                                {
                                    for (int k = 0; k < reference.GetRowCount(0); k++)
                                    {
                                        for (int m = 0; m < reference.GetColumnCount(0); m++)
                                        {
                                            CalcNode node6 = this.Graph.GetNode(new CalcCellIdentity(reference.GetRow(0) + k, reference.GetColumn(0) + m));
                                            if (!object.ReferenceEquals(node6, null))
                                            {
                                                node6.MarkAsDirty(this.Service, autoCalculate, false, true, true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (CalcLocalIdentity identity3 in this.Graph.GetAllVolantedIdeneities())
                {
                    this.Graph.EnsureNode(identity3, null).MarkAsDirty(this.Service, autoCalculate, true, true, true);
                }
                if (autoCalculate)
                {
                    this.Service.Recalculate(0xc350, false);
                }
            }
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="autoCalculate"></param>
        public void Invalidate(IEnumerable<CalcLocalIdentity> ids, bool autoCalculate = true)
        {
            if (object.ReferenceEquals(ids, null))
            {
                throw new ArgumentNullException("ids");
            }
            if (!this.Graph.IsEmpty)
            {
                CalcEvaluatorContext evaluatorContext = this.Source.GetEvaluatorContext(new CalcCellIdentity(0, 0));
                DirtyItem header = null;
                DirtyItem tailItem = null;
                foreach (CalcLocalIdentity identity in ids)
                {
                    this.InvalidateIntersecteds(identity, evaluatorContext, null, ref header, ref tailItem);
                }
                foreach (CalcNode node in this.Graph.GetAllVolantedNodes())
                {
                    this.InvalidateIntersecteds(node.Id as CalcLocalIdentity, evaluatorContext, node, ref header, ref tailItem);
                }
                if (header != null)
                {
                    this.InvalidateDependencies(ref header, ref tailItem);
                }
                if (autoCalculate)
                {
                    this.Service.Recalculate(0xc350, false);
                }
            }
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="id"></param>
        /// <param name="context"></param>
        /// <param name="node"></param>
        private void Invalidate(CalcLocalIdentity id, CalcEvaluatorContext context, CalcNode node = null)
        {
            DirtyItem header = null;
            DirtyItem tailItem = null;
            this.InvalidateIntersecteds(id, context, node, ref header, ref tailItem);
            foreach (CalcNode node2 in this.Graph.GetAllVolantedNodes())
            {
                this.InvalidateIntersecteds(node2.Id as CalcLocalIdentity, context, node2, ref header, ref tailItem);
            }
            if (header != null)
            {
                this.InvalidateDependencies(ref header, ref tailItem);
            }
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="id"></param>
        /// <param name="context"></param>
        /// <param name="node"></param>
        /// <param name="header"></param>
        /// <param name="tailItem"></param>
        private void InvalidateIntersecteds(CalcLocalIdentity id, CalcEvaluatorContext context, CalcNode node, ref DirtyItem header, ref DirtyItem tailItem)
        {
            ICalcSource source = this.Source;
            if (node == null)
            {
                node = this.Graph.GetNode(id);
            }
            if (!object.ReferenceEquals(node, null))
            {
                if (node.DirtyItem != null)
                {
                    return;
                }
                if (node.DirtyItem == null)
                {
                    node.DirtyItem = new DirtyItem(this, node);
                }
                AddDirtyItem(ref header, ref tailItem, node.DirtyItem, null);
            }
            if (id is CalcCellIdentity)
            {
                foreach (CalcNode node2 in this.EnumerateIntersectedNodesExcludeSelf(id as CalcCellIdentity))
                {
                    if (((node2.Dependents != null) && (node2.Dependents.Count > 0)) && (node2.DirtyItem == null))
                    {
                        node2.DirtyItem = new DirtyItem(this, node2);
                        AddDirtyItem(ref header, ref tailItem, node2.DirtyItem, node);
                    }
                }
            }
            else
            {
                foreach (CalcNode node3 in this.Graph.EnumerateIntersectedNodesExcludeSelf(context, id))
                {
                    if (node3.DirtyItem == null)
                    {
                        node3.DirtyItem = new DirtyItem(this, node3);
                        AddDirtyItem(ref header, ref tailItem, node3.DirtyItem, node);
                    }
                }
            }
        }

        internal void InvalidateAllIdentity()
        {
            foreach (CalcLocalIdentity identity in this.Graph.GetAllLocalIdentities())
            {
                this.Graph.EnsureNode(identity, null).MarkAsDirty(this.Service, true, false, false, true);
            }
            foreach (CalcLocalIdentity identity2 in this.Graph.GetAllVolantedIdeneities())
            {
                this.Graph.EnsureNode(identity2, null).MarkAsDirty(this.Service, true, false, false, true);
            }
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="header"></param>
        /// <param name="tailItem"></param>
        private void InvalidateDependencies(ref DirtyItem header, ref DirtyItem tailItem)
        {
            this.GetAllDirtyItems(ref header, ref tailItem);
            while (header != null)
            {
                DirtyItem removingItem = header;
                bool flag = false;
                while (removingItem != null)
                {
                    DirtyItem nextItem = removingItem.NextItem;
                    if (removingItem.PredencyItemCount == 0)
                    {
                        RemoveDirtyItem(ref header, ref tailItem, removingItem);
                        this.Service.AddDirtyItem(removingItem);
                        flag = true;
                    }
                    removingItem = nextItem;
                }
                if ((header != null) && !flag)
                {
                    Queue<DirtyItem> queue = new Queue<DirtyItem>();
                    queue.Enqueue(header);
                    header.CircleReferenceCount++;
                    DirtyItem item3 = header;
                    while ((queue.Count > 0) || (item3 != null))
                    {
                        removingItem = queue.Dequeue();
                        if (removingItem.DependencyCellTemp != null)
                        {
                            foreach (CalcNode node in removingItem.DependencyCellTemp)
                            {
                                node.DirtyItem.CircleReferenceCount++;
                                if (node.DirtyItem.CircleReferenceCount > node.DirtyItem.PredencyItemCount)
                                {
                                    RemoveDirtyItem(ref header, ref tailItem, node.DirtyItem);
                                    this.Service.AddDirtyItem(node.DirtyItem);
                                    flag = true;
                                    removingItem = node.DirtyItem;
                                    break;
                                }
                                queue.Enqueue(node.DirtyItem);
                            }
                        }
                        if (flag)
                        {
                            item3 = header;
                            while (item3 != null)
                            {
                                item3.CircleReferenceCount = 0;
                                item3 = item3.NextItem;
                            }
                            continue;
                        }
                        if ((queue.Count == 0) && (item3 != null))
                        {
                            item3 = item3.NextItem;
                            while ((item3 != null) && (item3.CircleReferenceCount != 0))
                            {
                                item3 = item3.NextItem;
                            }
                            if (item3 == null)
                            {
                                continue;
                            }
                            queue.Enqueue(item3);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether the specified <see cref="T:Dt.CalcEngine.CalcIdentity" /> is intersected with an range that has array formula.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:Dt.CalcEngine.CalcIdentity" /> is intersected
        /// with an range that has array formula;  otherwise, <see langword="false" />.
        /// </returns>
        public bool IsIsIntersectantWithArrayFormula(CalcLocalIdentity id)
        {
            return this.Graph.IsIsIntersectantWithArrayFormula(id);
        }

        /// <summary>
        /// Recalculates this instance.
        /// </summary>
        /// <param name="maxCalcCount">The max iterator.</param>
        /// <param name="forceRecalculateAll">Whether force recalculate all formula in current manager.</param>
        /// <returns></returns>
        public bool Recalculate(int maxCalcCount = 0xc350, bool forceRecalculateAll = false)
        {
            if (forceRecalculateAll)
            {
                this.InvalidateAllIdentity();
            }
            return this.Service.Recalculate(maxCalcCount, false);
        }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <param name="header"></param>
        /// <param name="tailItem"></param>
        /// <param name="removingItem"></param>
        private static void RemoveDirtyItem(ref DirtyItem header, ref DirtyItem tailItem, DirtyItem removingItem)
        {
            if (header == removingItem)
            {
                header = removingItem.NextItem;
            }
            if (tailItem == removingItem)
            {
                tailItem = removingItem.PreviousItem;
            }
            if (removingItem.PreviousItem != null)
            {
                removingItem.PreviousItem.NextItem = removingItem.NextItem;
            }
            if (removingItem.NextItem != null)
            {
                removingItem.NextItem.PreviousItem = removingItem.PreviousItem;
            }
            removingItem.NextItem = removingItem.PreviousItem = null;
            if (removingItem.DependencyCellTemp != null)
            {
                foreach (CalcNode node in removingItem.DependencyCellTemp)
                {
                    if (node.DirtyItem != null)
                    {
                        node.DirtyItem.PredencyItemCount--;
                    }
                }
            }
        }


        internal void ResumeCalcGraph()
        {
            Exception exception = null;
            CalcExpression expr = null;
            foreach (KeyValuePair<CalcLocalIdentity, Tuple<CalcExpression, bool>> pair in this._tmpFormulas)
            {
                CalcLocalIdentity key = pair.Key;
                expr = pair.Value.Item1;
                try
                {
                    this.Graph.SetNode(key, expr, this.Source.GetEvaluatorContext(key), this.Source, pair.Value.Item2);
                    this._formulas[key] = expr;
                }
                catch (Exception exception2)
                {
                    if (exception == null)
                    {
                        exception = exception2;
                    }
                }
            }
            this._tmpFormulas.Clear();
            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Sets the expression.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="expr">The exception.</param>
        /// <param name="isArrayFormula">Indicates the expression is array formula or not.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="id" /> is <see langword="null" />.</exception>
        public void SetExpression(CalcCellIdentity id, CalcExpression expr, bool isArrayFormula = false)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            if (this.Service.IsGraphSuspended)
            {
                this._tmpFormulas[id] = new Tuple<CalcExpression, bool>(expr, isArrayFormula);
            }
            else
            {
                this.Graph.SetNode(id, expr, this.Source.GetEvaluatorContext(id), this.Source, isArrayFormula);
                this._formulas[id] = expr;
            }
        }

        /// <summary>
        /// Sets the expression.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="expr">The exception.</param>
        /// <param name="isArrayFormula">Indicates the expression is array formula or not.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="id" /> is <see langword="null" />.</exception>
        public void SetExpression(CalcRangeIdentity id, CalcExpression expr, bool isArrayFormula = true)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            if (!isArrayFormula && !(expr is CalcSharedExpression))
            {
                expr = new CalcSharedExpression(expr);
            }
            if ((!isArrayFormula && !id.IsFullRow) && (!id.IsFullColumn && !CalcGraph.IsLargeArray(id)))
            {
                this.ExpandSharedFormula(expr, id);
            }
            else if (this.Service.IsGraphSuspended)
            {
                this._tmpFormulas[id] = new Tuple<CalcExpression, bool>(expr, isArrayFormula);
            }
            else
            {
                this.Graph.SetNode(id, expr, this.Source.GetEvaluatorContext(id), this.Source, isArrayFormula);
                this._formulas[id] = expr;
            }
        }

        /// <summary>
        /// Sets the formula.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="formula">The formula.</param>
        /// <param name="isArrayFormula">Indicates the formula is array formula or not.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="id" /> is <see langword="null" />.</exception>
        public void SetFormula(CalcCellIdentity id, string formula, bool isArrayFormula = false)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            CalcExpression expr = this.Parser.Parse(formula, this.Source.GetParserContext(id));
            this.SetExpression(id, expr, isArrayFormula);
        }

        /// <summary>
        /// Sets the formula.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="formula">The formula.</param>
        /// <param name="isArrayFormula">Indicates the formula is array formula or not.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="id" /> is <see langword="null" />.</exception>
        public void SetFormula(CalcRangeIdentity id, string formula, bool isArrayFormula = true)
        {
            if (object.ReferenceEquals(id, null))
            {
                throw new ArgumentNullException("id");
            }
            CalcExpression expr = this.Parser.Parse(formula, this.Source.GetParserContext(id));
            this.SetExpression(id, expr, isArrayFormula);
        }

        internal void SuspendCalcGraph()
        {
        }

        internal void UpdateStorage()
        {
            if (this._formulas.Count > 0)
            {
                foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair in this._formulas)
                {
                    this.Graph.SetNode(pair.Key, pair.Value, this.Source.GetEvaluatorContext(pair.Key), this.Source, !(pair.Value is CalcSharedExpression));
                }
            }
        }

        /// <summary>
        /// Gets the evaluator.
        /// </summary>
        /// <value>The evaluator.</value>
        public CalcEvaluator Evaluator { get; private set; }

        /// <summary>
        /// Get the count of all formulas stored in current <see cref="T:Dt.CalcEngine.CalcCalculationManager" />
        /// </summary>
        public int FormulaCount
        {
            get
            {
                return (((this._formulas != null) ? this._formulas.Count : 0) + ((this._tmpFormulas != null) ? this._tmpFormulas.Count : 0));
            }
        }

        /// <summary>
        /// Gets the graph which manages the dependents for all calculation units.
        /// </summary>
        /// <value>A <see cref="T:Dt.CalcEngine.CalcGraph" /> indicates the graph.</value>
        public CalcGraph Graph { get; private set; }

        /// <summary>
        /// Gets the parser.
        /// </summary>
        /// <value>The parser.</value>
        public CalcParser Parser { get; private set; }

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>The service.</value>
        public CalcService Service { get; private set; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public ICalcSource Source { get; private set; }

        private class SimpleFormulaStorage : ICalcStorage<CalcLocalIdentity, CalcExpression>, IEnumerable<KeyValuePair<CalcLocalIdentity, CalcExpression>>, IEnumerable
        {
            private readonly Dictionary<CalcLocalIdentity, CalcExpression> _storage = new Dictionary<CalcLocalIdentity, CalcExpression>();

            public IEnumerator<KeyValuePair<CalcLocalIdentity, CalcExpression>> GetEnumerator()
            {
                foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> iteratorVariable0 in this._storage)
                {
                    yield return new KeyValuePair<CalcLocalIdentity, CalcExpression>(iteratorVariable0.Key, iteratorVariable0.Value);
                }
            }

            public void RemoveAt(CalcLocalIdentity id)
            {
                this._storage.Remove(id);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> iteratorVariable0 in this._storage)
                {
                    yield return new KeyValuePair<CalcLocalIdentity, CalcExpression>(iteratorVariable0.Key, iteratorVariable0.Value);
                }
            }

            public int Count
            {
                get
                {
                    return this._storage.Count;
                }
            }

            public CalcExpression this[CalcLocalIdentity id]
            {
                get
                {
                    CalcExpression expression;
                    this._storage.TryGetValue(id, out expression);
                    return expression;
                }
                set
                {
                    this._storage[id] = value;
                }
            }
        }
    }
}

