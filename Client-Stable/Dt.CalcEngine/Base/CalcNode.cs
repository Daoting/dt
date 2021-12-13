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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represent a node which describe an dependence unit for <see cref="P:Dt.CalcEngine.CalcNode.Id" />.
    /// </summary>
    public sealed class CalcNode : IEquatable<CalcNode>
    {
        private bool _isProcessingDirty;
        private bool _isTempNode;
        private bool _isWattingForProcess;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcNode" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="id">The id.</param>
        public CalcNode(ICalcSource source, CalcIdentity id)
        {
            this.Source = source;
            this.Id = id;
        }

        internal void ClearDirty()
        {
            if (this.DirtyItem != null)
            {
                this.DirtyItem.Service.RemoveDirtyItem(this.DirtyItem);
                this.DirtyItem = null;
            }
        }

        private bool CompareTo(CalcNode other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            if (!object.ReferenceEquals(this.Source, other.Source))
            {
                return false;
            }
            return (this.Id == other.Id);
        }

        internal static CalcNode CreateTempNode(ICalcSource source, CalcIdentity id)
        {
            return new CalcNode(source, id) { _isTempNode = true };
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.CompareTo(obj as CalcNode);
        }

        private static void ExpandSharedFormulaDependency(CalcService service, CalcGraph graph, List<CalcNode> searchingDepends, List<SharedFormulaDirtyItem> sharedDirtyItmes, CalcNode rangeIntersected, CalcLocalIdentity currentId)
        {
            CalcRangeIdentity identity = currentId as CalcRangeIdentity;
            CalcRangeIdentity id = rangeIntersected.Id as CalcRangeIdentity;
            if ((!object.ReferenceEquals(id, null) && (rangeIntersected.Dependents != null)) && ((rangeIntersected.Dependents.Count > 0) && ShouldExpand(graph, id)))
            {
                foreach (KeyValuePair<CalcNode, CalcNode> pair in rangeIntersected.Dependents)
                {
                    List<CalcLocalIdentity> list;
                    CalcRangeIdentity rangeId = pair.Key.Id as CalcRangeIdentity;
                    if ((rangeId == null) || !graph.IsSharedFormula(rangeId))
                    {
                        goto Label_019E;
                    }
                    if ((currentId == rangeId) || ((identity != null) && (identity.IsFullRow || identity.IsFullColumn)))
                    {
                        searchingDepends.Add(pair.Key);
                        continue;
                    }
                    SharedFormulaDirtyItem dirtyItem = null;
                    if (pair.Key.DirtyItem != null)
                    {
                        if (pair.Key.DirtyItem is SharedFormulaDirtyItem)
                        {
                            dirtyItem = pair.Key.DirtyItem as SharedFormulaDirtyItem;
                            if (!dirtyItem.IsFullRangeDirty)
                            {
                                goto Label_0136;
                            }
                        }
                        continue;
                    }
                    dirtyItem = new SharedFormulaDirtyItem(service, graph.Manager.Source, rangeId, graph.GetNode(rangeId));
                    graph.GetNode(rangeId).DirtyItem = dirtyItem;
                Label_0136:
                    list = GetDepIdsInSharedFormulaRange(graph.Manager, rangeId, currentId);
                    if ((list.Count == 1) && (list[0] == rangeId))
                    {
                        dirtyItem.IsFullRangeDirty = true;
                        searchingDepends.Add(graph.GetNode(rangeId));
                    }
                    else
                    {
                        dirtyItem.DirtySubIds2.AddRange(list);
                        if (dirtyItem.DirtySubIds2.Count > 0)
                        {
                            sharedDirtyItmes.Add(dirtyItem);
                        }
                    }
                    continue;
                Label_019E:
                    searchingDepends.Add(pair.Key);
                }
            }
            else
            {
                searchingDepends.Add(rangeIntersected);
            }
        }

        

        private static List<CalcLocalIdentity> GetDepIdsInSharedFormulaRange(CalcCalculationManager mgr, CalcRangeIdentity sharedFormulaRange, CalcLocalIdentity dityId)
        {
            int num;
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            int num7;
            int num8;
            List<CalcLocalIdentity> list = new List<CalcLocalIdentity>();
            LocalId2Index(sharedFormulaRange, out num, out num2, out num3, out num4);
            ReferencePredenceVisitor visitor = new ReferencePredenceVisitor(mgr.Source, num, num2);
            visitor.Visit(mgr.GetExpression(sharedFormulaRange), num, num2);
            Dictionary<CalcLocalIdentity, bool> predenceIds = visitor.PredenceIds;
            LocalId2Index(dityId, out num5, out num6, out num7, out num8);
            foreach (KeyValuePair<CalcLocalIdentity, bool> pair in predenceIds)
            {
                int num9;
                int num10;
                int num11;
                int num12;
                CalcRangeIdentity key = pair.Key as CalcRangeIdentity;
                if ((key != null) && (key.IsFullRow || key.IsFullColumn))
                {
                    list.Clear();
                    list.Add(sharedFormulaRange);
                    return list;
                }
                LocalId2Index(pair.Key, out num9, out num10, out num11, out num12);
                int rowIndex = (num5 > num11) ? ((num + num5) - num11) : num;
                int columnIndex = (num6 > num12) ? ((num2 + num6) - num12) : num2;
                int num15 = (num + num7) - num9;
                num15 = (num15 > num3) ? num3 : num15;
                int num16 = (num2 + num8) - num10;
                num16 = (num16 > num4) ? num4 : num16;
                if (((rowIndex <= num3) && (num15 >= num)) && ((columnIndex <= num4) && (num16 >= num2)))
                {
                    if (!pair.Value)
                    {
                        list.Clear();
                        list.Add(sharedFormulaRange);
                        return list;
                    }
                    CalcRangeIdentity item = new CalcRangeIdentity(rowIndex, columnIndex, (num15 - rowIndex) + 1, (num16 - columnIndex) + 1);
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ((!object.ReferenceEquals(this.Source, null) ? this.Source.GetHashCode() : 0) ^ (!object.ReferenceEquals(this.Id, null) ? this.Id.GetHashCode() : 0));
        }

        private static bool IsContains(CalcLocalIdentity id1, CalcLocalIdentity id2)
        {
            CalcRangeIdentity rangeId = id1 as CalcRangeIdentity;
            if (rangeId == null)
            {
                return false;
            }
            CalcRangeIdentity identity2 = id2 as CalcRangeIdentity;
            CalcCellIdentity identity3 = id2 as CalcCellIdentity;
            if (identity3 != null)
            {
                return IsContains(rangeId, identity3.RowIndex, identity3.ColumnIndex, 1, 1);
            }
            return ((identity2 != null) && IsContains(rangeId, identity2.RowIndex, identity2.ColumnIndex, identity2.RowCount, identity2.ColumnCount));
        }

        private static bool IsContains(CalcRangeIdentity rangeId, int row, int col, int rowCount, int colCount)
        {
            if (!rangeId.IsFullColumn && ((row < rangeId.RowIndex) || ((row + rowCount) > (rangeId.RowIndex + rangeId.RowCount))))
            {
                return false;
            }
            return (rangeId.IsFullRow || ((col >= rangeId.ColumnIndex) && ((col + colCount) <= (rangeId.ColumnIndex + rangeId.ColumnCount))));
        }

        private static void LocalId2Index(CalcLocalIdentity item, out int startRow, out int startCol, out int endRow, out int endCol)
        {
            CalcCellIdentity identity = item as CalcCellIdentity;
            CalcRangeIdentity identity2 = item as CalcRangeIdentity;
            if (identity != null)
            {
                startRow = endRow = identity.RowIndex;
                startCol = endCol = identity.ColumnIndex;
            }
            else
            {
                startRow = identity2.RowIndex;
                startCol = identity2.ColumnIndex;
                endRow = (identity2.RowIndex + identity2.RowCount) - 1;
                endCol = (identity2.ColumnIndex + identity2.ColumnCount) - 1;
            }
            startRow = (startRow < 0) ? 0 : startRow;
            startCol = (startCol < 0) ? 0 : startCol;
            endRow = (endRow < 0) ? 0xfffff : endRow;
            endCol = (endCol < 0) ? 0x3fff : endCol;
        }

        internal void MarkAsDirty(CalcService service, bool recalculateAll = false, bool recursiveToIntersectant = true, bool recursiveToDependency = true, bool recursiveToSharedFormula = true)
        {
            if (!this.IsDirty && !this._isProcessingDirty)
            {
                int num;
                int num2;
                int num3;
                int num4;
                bool flag;
                this._isProcessingDirty = true;
                if ((this.DirtyItem == null) && !this._isTempNode)
                {
                    CalcLocalIdentity objA = this.Id as CalcLocalIdentity;
                    if (object.ReferenceEquals(objA, null))
                    {
                        return;
                    }
                    this.DirtyItem = new DirtyItem(service, this.Source, objA, this);
                }
                CalcGraph graph = service.GetCalculationManager(this.Source, null, true).Graph;
                CalcReferenceHelper.Id2Range(this.Source, this.Id, out num, out num2, out num3, out num4, out flag);
                CalcRangeIdentity id = this.Id as CalcRangeIdentity;
                List<CalcNode> searchingDepends = new List<CalcNode>();
                List<CalcNode> list2 = new List<CalcNode>();
                if (recursiveToIntersectant && !object.ReferenceEquals(id, null))
                {
                    List<CalcNode> list3 = new List<CalcNode>();
                    for (int i = 0; i < num3; i++)
                    {
                        for (int j = 0; j < num4; j++)
                        {
                            CalcNode node = graph.GetNode(new CalcCellIdentity(num + i, num2 + j));
                            if (!object.ReferenceEquals(node, null) && node.IsDirty)
                            {
                                list2.Add(node);
                            }
                            if ((!object.ReferenceEquals(node, null) && !node._isWattingForProcess) && (!node._isProcessingDirty && !node.IsDirty))
                            {
                                node._isWattingForProcess = true;
                                list3.Add(node);
                            }
                        }
                    }
                    foreach (CalcNode node2 in list3)
                    {
                        node2.MarkAsDirty(service, recalculateAll, false, true, true);
                        node2._isWattingForProcess = false;
                    }
                }
                DirtyItem dependForehand = null;
                List<CalcNode> pendingDependDirtyNode = new List<CalcNode>();
                if ((this.Dependents != null) && (this.Dependents.Count > 0))
                {
                    searchingDepends.AddRange(this.Dependents.Values);
                }
                List<SharedFormulaDirtyItem> sharedDirtyItmes = new List<SharedFormulaDirtyItem>();
                if (flag || !recalculateAll)
                {
                    foreach (CalcNode node3 in graph.GetAllDependentRangeNodes(num, num2, num3, num4))
                    {
                        if ((node3 != this) && ((recalculateAll || !graph.IsSharedFormula(node3.Id as CalcRangeIdentity)) || ((node3.Dependents != null) && (node3.Dependents.Count != 0))))
                        {
                            if (!recalculateAll && recursiveToDependency)
                            {
                                ExpandSharedFormulaDependency(service, graph, list2, sharedDirtyItmes, node3, this.Id as CalcLocalIdentity);
                            }
                            else if (recalculateAll)
                            {
                                list2.Add(node3);
                            }
                        }
                    }
                    if (recursiveToSharedFormula && this._isTempNode)
                    {
                        foreach (CalcNode node4 in graph.GetAllDependentSharedNodes(num, num2, num3, num4))
                        {
                            list2.Add(node4);
                        }
                    }
                }
                if (recursiveToIntersectant)
                {
                    searchingDepends.AddRange(list2);
                    list2.Clear();
                }
                dependForehand = SerchDependForehand(dependForehand, searchingDepends, list2, pendingDependDirtyNode, sharedDirtyItmes);
                if (recursiveToSharedFormula && !this._isTempNode)
                {
                    if (dependForehand != null)
                    {
                        service.InsertToDirtyBefore(this.DirtyItem, dependForehand);
                    }
                    else
                    {
                        service.AddDirtyItem(this.DirtyItem);
                    }
                }
                if (recursiveToDependency)
                {
                    foreach (CalcNode node5 in pendingDependDirtyNode)
                    {
                        node5._isWattingForProcess = true;
                    }
                    foreach (CalcNode node6 in pendingDependDirtyNode)
                    {
                        node6.MarkAsDirty(service, recalculateAll, true, true, true);
                        node6._isWattingForProcess = false;
                    }
                    foreach (SharedFormulaDirtyItem item2 in sharedDirtyItmes)
                    {
                        if (!item2.Node.IsDirty)
                        {
                            service.AddDirtyItem(item2);
                        }
                        List<CalcNode> list6 = new List<CalcNode>();
                        foreach (CalcLocalIdentity identity3 in item2.DirtySubIds2)
                        {
                            if (!item2.DirtySubIds.Contains(identity3))
                            {
                                item2.DirtySubIds.Add(identity3);
                                CalcNode item = graph.GetNode(identity3);
                                if (item == null)
                                {
                                    item = CreateTempNode(this.Source, identity3);
                                }
                                item._isWattingForProcess = true;
                                list6.Add(item);
                            }
                        }
                        foreach (CalcNode node8 in list6)
                        {
                            node8.MarkAsDirty(service, recalculateAll, true, true, false);
                            node8._isWattingForProcess = false;
                        }
                    }
                }
                else if (recalculateAll)
                {
                    foreach (CalcNode node9 in searchingDepends)
                    {
                        this.SetPredencyItem(service, node9);
                    }
                    foreach (CalcNode node10 in list2)
                    {
                        if (IsContains(node10.Id as CalcLocalIdentity, this.Id as CalcLocalIdentity))
                        {
                            this.SetPredencyItem(service, node10);
                        }
                    }
                }
                this._isProcessingDirty = false;
            }
        }

        /// <summary>
        /// Normalizes the specified node to convert all external reference to internal reference.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="node">The node.</param>
        /// <returns>
        /// If <paramref name="node" /> is external identity, return a new node which identity is not external, otherwise, return <paramref name="node" />.
        /// </returns>
        internal static CalcNode Normalize(CalcService service, CalcNode node)
        {
            if (!object.ReferenceEquals(node, null) && (node.Id is CalcExternalIdentity))
            {
                CalcExternalIdentity id = node.Id as CalcExternalIdentity;
                CalcGraph graph = service.GetCalculationManager(id.Source, null, true).Graph;
                CalcLocalIdentity identity2 = id.ConvertToLocal();
                node = graph.GetNode(identity2) ?? new CalcNode(id.Source, identity2);
            }
            return node;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(CalcNode left, CalcNode right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }
            return left.CompareTo(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(CalcNode left, CalcNode right)
        {
            return !(left == right);
        }

        private static DirtyItem SerchDependForehand(DirtyItem dependForehand, List<CalcNode> searchingDepends, List<CalcNode> intersectants, List<CalcNode> pendingDependDirtyNode, List<SharedFormulaDirtyItem> sharedDirtyItmes)
        {
            if (searchingDepends.Count > 0)
            {
                foreach (CalcNode node in searchingDepends)
                {
                    if (node.IsDirty)
                    {
                        if (dependForehand == null)
                        {
                            dependForehand = node.DirtyItem;
                        }
                        else if (node.DirtyItem.Position < dependForehand.Position)
                        {
                            dependForehand = node.DirtyItem;
                        }
                    }
                    else if (!node._isProcessingDirty && !node._isWattingForProcess)
                    {
                        pendingDependDirtyNode.Add(node);
                    }
                }
            }
            if ((intersectants != null) && (intersectants.Count > 0))
            {
                foreach (CalcNode node2 in intersectants)
                {
                    if (node2.IsDirty)
                    {
                        if (dependForehand == null)
                        {
                            dependForehand = node2.DirtyItem;
                        }
                        else if (node2.DirtyItem.Position < dependForehand.Position)
                        {
                            dependForehand = node2.DirtyItem;
                        }
                    }
                }
            }
            if (sharedDirtyItmes.Count > 0)
            {
                foreach (SharedFormulaDirtyItem item in sharedDirtyItmes)
                {
                    if (item.Node.IsDirty)
                    {
                        if (dependForehand == null)
                        {
                            dependForehand = item;
                        }
                        else if (item.Position < dependForehand.Position)
                        {
                            dependForehand = item;
                        }
                    }
                }
            }
            return dependForehand;
        }

        private void SetPredencyItem(CalcService service, CalcNode node)
        {
            if (node.DirtyItem == null)
            {
                node.DirtyItem = new DirtyItem(service, node.Source, node.Id as CalcLocalIdentity, node);
            }
            node.DirtyItem.PredencyItems[this.Id] = this.DirtyItem;
        }

        private static bool ShouldExpand(CalcGraph graph, CalcRangeIdentity rangeIntersectedDepId)
        {
            if (graph.Manager.GetExpression(rangeIntersectedDepId) == null)
            {
                return true;
            }
            CalcNode node = graph.GetNode(rangeIntersectedDepId);
            return (((node != null) && (node.DirtyItem is SharedFormulaDirtyItem)) && !(node.DirtyItem as SharedFormulaDirtyItem).IsFullRangeDirty);
        }

        bool IEquatable<CalcNode>.Equals(CalcNode other)
        {
            return this.CompareTo(other);
        }

        /// <summary>
        /// Gets or sets the dependents nodes which indicates what nodes are affected by value of current node.
        /// </summary>
        /// <example>
        /// There are following formulas:
        /// <table>
        /// <th><td>Identity</td><td>Formula</td></th>
        /// <tr><td>A1</td><td>=B2+B3</td></tr>
        /// <tr><td>A2</td><td>=SUM(B:B)</td></tr>
        /// <tr><td>B2</td><td>=C3+C4</td></tr>
        /// <tr><td>B3</td><td>=0</td></tr>
        /// <tr><td>C3</td><td>=1</td></tr>
        /// <tr><td>C4</td><td>=2</td></tr>
        /// </table>
        /// Then the Dependents for each identity would be:
        /// <table>
        /// <th><td>Identity</td><td>Dependents</td></th>
        /// <tr><td>A1</td><td>n/a</td></tr>
        /// <tr><td>A2</td><td>n/a</td></tr>
        /// <tr><td>B2</td><td>A1, A2</td></tr>
        /// <tr><td>B3</td><td>A1, A2</td></tr> 
        /// <tr><td>C3</td><td>B2</td></tr>
        /// <tr><td>C4</td><td>B2</td></tr>
        /// </table>
        /// </example>
        public Dictionary<CalcNode, CalcNode> Dependents { get; set; }

        /// <summary>
        /// hdt 唐忠宝修改。
        /// </summary>
        internal DirtyItem DirtyItem;

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        public CalcIdentity Id { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is dirty; otherwise, <see langword="false" />.
        /// </value>
        public bool IsDirty
        {
            get { return  ((this.DirtyItem != null) && this.DirtyItem.Service.IsDirtyItem(this.DirtyItem)); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.CalcEngine.CalcNode" /> is volatile.
        /// </summary>
        /// <value>
        /// <c>true</c> if volatile; otherwise, <c>false</c>.
        /// </value>
        internal bool IsVolatile { get; set; }

        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        internal NodeType NodeType { get; set; }
       
        /// <summary>
        /// hdt 唐忠宝增加
        /// </summary>
        /// <returns></returns>

        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public CalcNode OwnerNode { get; internal set; }

        /// <summary>
        /// Gets or sets the precedents nodes which indicates what nodes affect the value of the current node.
        /// </summary>
        /// <example>
        /// There are following formulas:
        /// <table>
        /// <th><td>Identity</td><td>Formula</td></th>
        /// <tr><td>A1</td><td>=B2+B3</td></tr>
        /// <tr><td>A2</td><td>=SUM(B:B)</td></tr>
        /// <tr><td>B2</td><td>=C3+C4</td></tr>
        /// <tr><td>B3</td><td>=0</td></tr> 
        /// <tr><td>C3</td><td>=1</td></tr>
        /// <tr><td>C4</td><td>=2</td></tr>
        /// </table>
        /// Then the Precedents for each identity should be:
        /// <table>
        /// <th><td>Identity</td><td>Precedents</td></th>
        /// <tr><td>A1</td><td>B2, B3</td></tr>
        /// <tr><td>A2</td><td>B:B</td></tr>
        /// <tr><td>B2</td><td>C3, C4</td></tr>
        /// <tr><td>B3</td><td>n/a</td></tr> 
        /// <tr><td>C3</td><td>n/a</td></tr>
        /// <tr><td>C4</td><td>n/a</td></tr>
        /// </table>
        /// </example>
        public List<CalcNode> Precedents { get; set; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public ICalcSource Source { get; internal set; }

        private class ReferencePredenceVisitor : ExpressionVisitor
        {
            private int _BaseColumn;
            private int _baseRow;
            private ICalcSource _source;

            public ReferencePredenceVisitor(ICalcSource source, int baseRow, int baseColumn)
            {
                this._source = source;
                this._baseRow = baseRow;
                this._BaseColumn = baseColumn;
                this.PredenceIds = new Dictionary<CalcLocalIdentity, bool>();
            }

            private void AddPredenceIds(CalcLocalIdentity id, bool isFullRelative)
            {
                bool flag;
                if (this.PredenceIds.TryGetValue(id, out flag))
                {
                    if (!isFullRelative && flag)
                    {
                        this.PredenceIds[id] = isFullRelative;
                    }
                }
                else
                {
                    this.PredenceIds.Add(id, isFullRelative);
                }
            }

            protected override CalcExpression VisitCellExpression(CalcCellExpression expr, int baseRow, int baseColumn)
            {
                this.AddPredenceIds(expr.GetId(this._baseRow, this._BaseColumn) as CalcLocalIdentity, expr.RowRelative && expr.ColumnRelative);
                return base.VisitCellExpression(expr, baseRow, baseColumn);
            }

            protected override CalcExpression VisitExternalCellExpression(CalcExternalCellExpression expr, int baseRow, int baseColumn)
            {
                if (expr.Source == this._source)
                {
                    this.AddPredenceIds((expr.GetId(this._baseRow, this._BaseColumn) as CalcExternalIdentity).ConvertToLocal(), expr.RowRelative && expr.ColumnRelative);
                }
                return base.VisitExternalCellExpression(expr, baseRow, baseColumn);
            }

            protected override CalcExpression VisitExternalRangeExpression(CalcExternalRangeExpression expr, int baseRow, int baseColumn)
            {
                if (expr.Source == this._source)
                {
                    this.AddPredenceIds((expr.GetId(this._baseRow, this._BaseColumn) as CalcExternalIdentity).ConvertToLocal(), ((expr.StartRowRelative && expr.StartColumnRelative) && expr.EndRowRelative) && expr.EndColumnRelative);
                }
                return base.VisitExternalRangeExpression(expr, baseRow, baseColumn);
            }

            protected override CalcExpression VisitRangeExpression(CalcRangeExpression expr, int baseRow, int baseColumn)
            {
                this.AddPredenceIds(expr.GetId(this._baseRow, this._BaseColumn) as CalcLocalIdentity, ((expr.StartRowRelative && expr.StartColumnRelative) && expr.EndRowRelative) && expr.EndColumnRelative);
                return base.VisitRangeExpression(expr, baseRow, baseColumn);
            }

            public Dictionary<CalcLocalIdentity, bool> PredenceIds { get; private set; }
        }
    }
}

