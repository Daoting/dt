#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal class FormulaOperatorHelper
    {
        static bool ContainsOrEqual(CalcCalculationManager mgr, CalcLocalIdentity id1, CalcLocalIdentity id2)
        {
            CalcLocalIdentityExtension.CompareResult result;
            CalcLocalIdentityExtension.CompareResult result2;
            if (id2.TryCompareTo(id1, out result, out result2))
            {
                CalcNode objA = mgr.Graph.GetNode(id2);
                CalcExpression expression = mgr.GetExpression(id2);
                if (expression == null)
                {
                    return false;
                }
                if ((!object.ReferenceEquals(objA, null) && (objA.OwnerNode != null)) && (expression is CalcSharedExpression))
                {
                    return false;
                }
                if (((id2 is CalcRangeIdentity) && (((result == CalcLocalIdentityExtension.CompareResult.Great_Intersected) || (result == CalcLocalIdentityExtension.CompareResult.Less_Intersected)) || (result == CalcLocalIdentityExtension.CompareResult.Contains))) && (((result2 == CalcLocalIdentityExtension.CompareResult.Great_Intersected) || (result2 == CalcLocalIdentityExtension.CompareResult.Less_Intersected)) || (result2 == CalcLocalIdentityExtension.CompareResult.Contains)))
                {
                    if (!(mgr.GetExpression(id2) is CalcSharedExpression))
                    {
                        throw new InvalidOperationException(ResourceStrings.FormulaChangePartOfArrayFormulaError);
                    }
                    return false;
                }
                if (((result == CalcLocalIdentityExtension.CompareResult.Contained) || (result == CalcLocalIdentityExtension.CompareResult.Equal)) && ((result2 == CalcLocalIdentityExtension.CompareResult.Contained) || (result2 == CalcLocalIdentityExtension.CompareResult.Equal)))
                {
                    return true;
                }
            }
            return false;
        }

        static CalcIdentity GetActualId(CalcIdentity id)
        {
            if (id is ConditionalGraph.ConditionalIdentity)
            {
                ConditionalGraph.ConditionalIdentity identity = id as ConditionalGraph.ConditionalIdentity;
                if (identity.OldActualIdentity == null)
                {
                    return identity.ActualIdentity;
                }
                return identity.OldActualIdentity;
            }
            if (!(id is ConditionalGraph.ExternalConditionalIdentity))
            {
                return id;
            }
            ConditionalGraph.ExternalConditionalIdentity identity2 = id as ConditionalGraph.ExternalConditionalIdentity;
            if (identity2.OldActualIdentity == null)
            {
                return identity2.ActualIdentity;
            }
            return identity2.OldActualIdentity;
        }

        public static List<CalcRangeIdentity> GetAllContainsArrayFormulaIds(CalcCalculationManager manager, CalcLocalIdentity id)
        {
            int num;
            int num2;
            int num3;
            int num4;
            List<CalcRangeIdentity> list = new List<CalcRangeIdentity>();
            Identity2Indexs(id, out num, out num2, out num3, out num4);
            foreach (CalcRangeIdentity identity in manager.Graph.GetAllArrayFormulaIdentities())
            {
                if (ContainsOrEqual(manager, id, identity))
                {
                    list.Add(identity);
                }
            }
            return list;
        }

        internal static Dictionary<CalcLocalIdentity, CalcExpression> GetAllContainsIds(CalcCalculationManager mgr, CalcLocalIdentity id)
        {
            Dictionary<CalcCellIdentity, CalcExpression> dictionary2;
            Dictionary<CalcRangeIdentity, CalcExpression> dictionary3;
            Dictionary<CalcLocalIdentity, CalcExpression> dictionary = new Dictionary<CalcLocalIdentity, CalcExpression>();
            GetAllContainsIds(mgr, id, out dictionary2, out dictionary3);
            foreach (KeyValuePair<CalcCellIdentity, CalcExpression> pair in dictionary2)
            {
                CalcCellIdentity introduced7 = pair.Key;
                dictionary[introduced7] = pair.Value;
            }
            foreach (KeyValuePair<CalcRangeIdentity, CalcExpression> pair2 in dictionary3)
            {
                CalcRangeIdentity introduced8 = pair2.Key;
                dictionary[introduced8] = pair2.Value;
            }
            return dictionary;
        }

        internal static void GetAllContainsIds(CalcCalculationManager mgr, CalcLocalIdentity id, out Dictionary<CalcCellIdentity, CalcExpression> cellFromulas, out Dictionary<CalcRangeIdentity, CalcExpression> arrayFormulas)
        {
            cellFromulas = new Dictionary<CalcCellIdentity, CalcExpression>();
            arrayFormulas = new Dictionary<CalcRangeIdentity, CalcExpression>();
            CalcLocalIdentity identity = id;
            CalcExpression expression = mgr.GetExpression(identity);
            if ((expression == null) && (id is CalcCellIdentity))
            {
                int rowIndex = (id as CalcCellIdentity).RowIndex;
                int columnIndex = (id as CalcCellIdentity).ColumnIndex;
                CalcRangeIdentity identity2 = new CalcRangeIdentity(rowIndex, columnIndex, 1, 1);
                expression = mgr.GetExpression(identity2);
                identity = identity2;
            }
            bool flag = false;
            if (expression == null)
            {
                flag = mgr.IsIsIntersectantWithArrayFormula(identity);
            }
            if (expression != null)
            {
                if (identity is CalcCellIdentity)
                {
                    cellFromulas.Add(identity as CalcCellIdentity, expression);
                }
                else if (identity is CalcRangeIdentity)
                {
                    arrayFormulas.Add(identity as CalcRangeIdentity, expression);
                }
            }
            else
            {
                if ((id is CalcCellIdentity) && flag)
                {
                    throw new InvalidOperationException(ResourceStrings.FormulaChangePartOfArrayFormulaError);
                }
                if (id is CalcRangeIdentity)
                {
                    CalcRangeIdentity identity3 = id as CalcRangeIdentity;
                    if ((identity3.RowCount != 1) || (identity3.ColumnCount != 1))
                    {
                        foreach (CalcLocalIdentity identity4 in mgr.Graph.GetAllLocalIdentities())
                        {
                            if (ContainsOrEqual(mgr, id, identity4))
                            {
                                expression = mgr.GetExpression(identity4);
                                if (expression != null)
                                {
                                    if (identity4 is CalcCellIdentity)
                                    {
                                        cellFromulas[identity4 as CalcCellIdentity] = expression;
                                    }
                                    else if (identity4 is CalcRangeIdentity)
                                    {
                                        arrayFormulas.Add(identity4 as CalcRangeIdentity, expression);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static List<CalcLocalIdentity> GetChangedDependents(IFormulaOperatorSource mgr, ChangingContext context, CalcNode node, HashSet<CalcNode> arrayFormulaNodes, Dictionary<CalcLocalIdentity, CalcExpression> formulas1 = null, Dictionary<CalcLocalIdentity, CalcExpression> formulas2 = null)
        {
            List<CalcLocalIdentity> list = new List<CalcLocalIdentity>();
            if (node.Dependents != null)
            {
                CalcNode myOwnerNode;
                    foreach (CalcNode ownerNode in node.Dependents.Keys)
                    {
                        myOwnerNode = ownerNode;
                        if ( ownerNode.OwnerNode != null)
                        {
                            myOwnerNode = ownerNode.OwnerNode;
                            if (arrayFormulaNodes.Contains(myOwnerNode) || (mgr.GetExpression(myOwnerNode.Id as CalcLocalIdentity) is CalcSharedExpression))
                            {
                                continue;
                            }
                            arrayFormulaNodes.Add(myOwnerNode);
                        }
                        CalcLocalIdentity id = myOwnerNode.Id as CalcLocalIdentity;
                        if ((id != null) && (myOwnerNode.Source == mgr.Source))
                        {
                            if (!context.ChangingIdentities.ContainsKey(id))
                            {
                                CalcExpression expression = mgr.GetExpression(id);
                                if (expression != null)
                                {
                                    if (((formulas1 != null) && formulas1.ContainsKey(id)) || ((formulas2 != null) && formulas2.ContainsKey(id)))
                                    {
                                        list.Add(id);
                                    }
                                    else
                                    {
                                        context.ChangedFormulas[id] = new Tuple<CalcLocalIdentity, CalcExpression>(id, expression);
                                    }
                                }
                            }
                        }
                        else if ((id != null) && ((myOwnerNode.Source != mgr.Source) || !context.ChangingIdentities.ContainsKey(id)))
                        {
                            IFormulaOperatorSource externalManager = mgr.GetExternalManager(myOwnerNode.Source);
                            if (externalManager != null)
                            {
                                CalcExpression expr = externalManager.GetExpression(id);
                                if (expr != null)
                                {
                                    if (((formulas1 != null) && formulas1.ContainsKey(id)) || ((formulas2 != null) && formulas2.ContainsKey(id)))
                                    {
                                        list.Add(id);
                                    }
                                    else
                                    {
                                        context.AddExtChangedFormula(externalManager, id, id, expr);
                                    }
                                }
                            }
                        }
                    }
            }
            return list;
        }

        static void GetChangingIds(IFormulaOperatorSource mgr, bool row, ChangingContext context, OperatorExpressionVisistor visitor, bool updateDependents)
        {
            foreach (CalcLocalIdentity identity in mgr.GetAllLocalIdentities())
            {
                CalcIdentity id = identity;
                if (identity is ConditionalGraph.ConditionalIdentity)
                {
                    ConditionalGraph.ConditionalIdentity identity3 = identity as ConditionalGraph.ConditionalIdentity;
                    id = (identity3.OldActualIdentity != null) ? identity3.OldActualIdentity : identity3.ActualIdentity;
                }
                CalcNode node = mgr.GetNode(id);
                if ((node == null) || (node.OwnerNode == null))
                {
                    CalcCellIdentity identity4 = id as CalcCellIdentity;
                    CalcRangeIdentity identity5 = id as CalcRangeIdentity;
                    CalcExpression expression = mgr.GetExpression(identity);
                    CalcLocalIdentity identity6 = null;
                    int oldStart = -2147483648;
                    int oldEnd = -2147483648;
                    int newEnd = -2147483648;
                    if (identity4 != null)
                    {
                        oldStart = row ? identity4.RowIndex : identity4.ColumnIndex;
                    }
                    else if (identity5 != null)
                    {
                        if (identity5.IsFullRow && identity5.IsFullColumn)
                        {
                            identity6 = identity;
                            oldStart = 0;
                        }
                        else if ((identity5.IsFullRow && !row) || (identity5.IsFullColumn && row))
                        {
                            if (node != null)
                            {
                                context.InvalidateFormula(mgr, identity5);
                            }
                            if (((node == null) || (node.Dependents == null)) || ((node.Dependents.Count == 0) || !updateDependents))
                            {
                                continue;
                            }
                            InvalidateNode(mgr, context, node);
                        }
                        else
                        {
                            oldStart = row ? identity5.RowIndex : identity5.ColumnIndex;
                            oldEnd = row ? ((identity5.RowIndex + identity5.RowCount) - 1) : ((identity5.ColumnIndex + identity5.ColumnCount) - 1);
                        }
                    }
                    if (oldStart != -2147483648)
                    {
                        if (identity6 == null)
                        {
                            int num2;
                            if (identity5 != null)
                            {
                                visitor.GetRangeOffset(oldStart, oldEnd, out num2, out newEnd);
                                if ((num2 == oldStart) && (newEnd == oldEnd))
                                {
                                    continue;
                                }
                                if ((num2 == -2147483648) || (newEnd == -2147483648))
                                {
                                    expression = null;
                                }
                                else if (identity5.IsFullRow && row)
                                {
                                    identity6 = new CalcRangeIdentity(num2, (newEnd - num2) + 1, true);
                                }
                                else if (identity5.IsFullColumn && !row)
                                {
                                    identity6 = new CalcRangeIdentity(num2, (newEnd - num2) + 1, false);
                                }
                                else if (row)
                                {
                                    identity6 = new CalcRangeIdentity(num2, identity5.ColumnIndex, (newEnd - num2) + 1, identity5.ColumnCount);
                                }
                                else
                                {
                                    identity6 = new CalcRangeIdentity(identity5.RowIndex, num2, identity5.RowCount, (newEnd - num2) + 1);
                                }
                            }
                            else if (identity4 != null)
                            {
                                visitor.GetCellOffset(oldStart, out num2);
                                if (num2 == oldStart)
                                {
                                    continue;
                                }
                                if (num2 == -2147483648)
                                {
                                    expression = null;
                                }
                                else if (identity4 != null)
                                {
                                    identity6 = row ? new CalcCellIdentity(num2, identity4.ColumnIndex) : new CalcCellIdentity(identity4.RowIndex, num2);
                                }
                            }
                        }
                        context.ChangingIdentities[identity] = identity;
                        if (expression != null)
                        {
                            identity6 = (identity is ConditionalGraph.ConditionalIdentity) ? identity : identity6;
                            context.ChangedFormulas[identity6] = new Tuple<CalcLocalIdentity, CalcExpression>(identity, expression);
                        }
                    }
                }
            }
        }

        static void GetInvalidFormulas(IFormulaOperatorSource mgr, ChangingContext context)
        {
            HashSet<CalcNode> arrayFormulaNodes = new HashSet<CalcNode>();
            foreach (CalcLocalIdentity identity in context.ChangingIdentities.Keys)
            {
                CalcNode node = mgr.GetNode(identity);
                if ((node != null) && (node.Dependents != null))
                {
                    GetChangedDependents(mgr, context, node, arrayFormulaNodes, null, null);
                }
            }
        }

        public static void Identity2Indexs(CalcIdentity id, out int startRow, out int startColumn, out int lastRow, out int lastColumn)
        {
            if (id is CalcCellIdentity)
            {
                CalcCellIdentity identity = id as CalcCellIdentity;
                startRow = identity.RowIndex;
                startColumn = identity.ColumnIndex;
                lastRow = identity.RowIndex;
                lastColumn = identity.ColumnIndex;
            }
            else if (id is CalcExternalCellIdentity)
            {
                CalcExternalCellIdentity identity2 = id as CalcExternalCellIdentity;
                startRow = identity2.RowIndex;
                startColumn = identity2.ColumnIndex;
                lastRow = identity2.RowIndex;
                lastColumn = identity2.ColumnIndex;
            }
            else if (id is CalcRangeIdentity)
            {
                CalcRangeIdentity identity3 = id as CalcRangeIdentity;
                if (identity3.IsFullRow)
                {
                    startColumn = -1;
                    lastColumn = -1;
                }
                else
                {
                    startColumn = identity3.ColumnIndex;
                    lastColumn = (identity3.ColumnIndex + identity3.ColumnCount) - 1;
                }
                if (identity3.IsFullColumn)
                {
                    startRow = -1;
                    lastRow = -1;
                }
                else
                {
                    startRow = identity3.RowIndex;
                    lastRow = (identity3.RowIndex + identity3.RowCount) - 1;
                }
            }
            else if (id is CalcExternalRangeIdentity)
            {
                CalcExternalRangeIdentity identity4 = id as CalcExternalRangeIdentity;
                if (identity4.IsFullRow)
                {
                    startColumn = -1;
                    lastColumn = -1;
                }
                else
                {
                    startColumn = identity4.ColumnIndex;
                    lastColumn = (identity4.ColumnIndex + identity4.ColumnCount) - 1;
                }
                if (identity4.IsFullColumn)
                {
                    startRow = -1;
                    lastRow = -1;
                }
                else
                {
                    startRow = identity4.RowIndex;
                    lastRow = (identity4.RowIndex + identity4.RowCount) - 1;
                }
            }
            else if (id is CalcSheetRangeIdentity)
            {
                CalcSheetRangeIdentity identity5 = id as CalcSheetRangeIdentity;
                if (identity5.IsFullRow)
                {
                    startColumn = -1;
                    lastColumn = -1;
                }
                else
                {
                    startColumn = identity5.ColumnIndex;
                    lastColumn = (identity5.ColumnIndex + identity5.ColumnCount) - 1;
                }
                if (identity5.IsFullColumn)
                {
                    startRow = -1;
                    lastRow = -1;
                }
                else
                {
                    startRow = identity5.RowIndex;
                    lastRow = (identity5.RowIndex + identity5.RowCount) - 1;
                }
            }
            else
            {
                startRow = -1;
                startColumn = -1;
                lastRow = -1;
                lastColumn = -1;
            }
        }

        public static CalcLocalIdentity Indexs2Identity(int startRow, int startColumn, int lastRow, int lastColumn)
        {
            if ((startRow == -1) && (startColumn == -1))
            {
                return new CalcRangeIdentity();
            }
            if (startRow == -1)
            {
                return new CalcRangeIdentity(startColumn, (lastColumn - startColumn) + 1, false);
            }
            if (startColumn == -1)
            {
                return new CalcRangeIdentity(startRow, (lastRow - startRow) + 1, true);
            }
            if ((startRow == lastRow) && (startColumn == lastColumn))
            {
                return new CalcCellIdentity(startRow, startColumn);
            }
            return new CalcRangeIdentity(startRow, startColumn, (lastRow - startRow) + 1, (lastColumn - startColumn) + 1);
        }

        public static void Insert(IFormulaOperatorSource mgr, int index, int count, bool row, bool isFullBand = false, bool updateDependents = true, bool updateNames = true)
        {
            ChangingContext context = new ChangingContext();
            InsertVisitor visitor = new InsertVisitor(index, count, row, isFullBand, null);
            GetChangingIds(mgr, row, context, visitor, updateDependents);
            GetInvalidFormulas(mgr, context);
            if (updateNames)
            {
                UpdataCustomnNames(mgr, visitor);
            }
            visitor.CurrentCalcSource = mgr.Source;
            UpdataInvalidFormula(mgr, context, visitor);
        }

        static void InvalidateNode(IFormulaOperatorSource mgr, ChangingContext context, CalcNode node)
        {
            using (Dictionary<CalcNode, CalcNode>.Enumerator enumerator = node.Dependents.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    CalcLocalIdentity id = enumerator.Current.Key.Id as CalcLocalIdentity;
                    if ((id != null) && (node.Source == mgr.Source))
                    {
                        if (!context.ChangingIdentities.ContainsKey(id) && (mgr.GetExpression(id) != null))
                        {
                            context.InvalidateFormula(mgr, id);
                        }
                    }
                    else if (id != null)
                    {
                        IFormulaOperatorSource externalManager = mgr.GetExternalManager(node.Source);
                        if ((externalManager != null) && (externalManager.GetExpression(id) != null))
                        {
                            context.InvalidateFormula(externalManager, id);
                        }
                    }
                }
            }
        }

        public static void Remove(IFormulaOperatorSource mgr, int index, int count, bool row, bool isFullBand = false, bool updateDependents = true, bool updateNames = true)
        {
            ChangingContext context = new ChangingContext();
            RemoveVisitor visitor = new RemoveVisitor(index, count, row, isFullBand, null);
            GetChangingIds(mgr, row, context, visitor, updateDependents);
            GetInvalidFormulas(mgr, context);
            if (updateNames)
            {
                UpdataCustomnNames(mgr, visitor);
            }
            visitor.CurrentCalcSource = mgr.Source;
            UpdataInvalidFormula(mgr, context, visitor);
        }

        public static void Reorder(IFormulaOperatorSource mgr, ICalcStorage<CalcLocalIdentity, CalcExpression> calcStorage, int from, int to, int count, bool row, bool updateDependents = true, bool updateNames = true)
        {
            ChangingContext context = new ChangingContext();
            ReorderVisitor visitor = new ReorderVisitor(from, to, count, row, false, null);
            GetChangingIds(mgr, row, context, visitor, updateDependents);
            if (updateDependents)
            {
                GetInvalidFormulas(mgr, context);
            }
            if (updateNames)
            {
                UpdataCustomnNames(mgr, visitor);
            }
            visitor.CurrentCalcSource = mgr.Source;
            UpdataInvalidFormula(mgr, context, visitor);
        }

        public static void UpdataChangings(IFormulaOperatorSource manager, ChangingContext context)
        {
            Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>> changedFormulas = context.ChangedFormulas;
            Dictionary<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>>> extChangedFormulas = context.ExtChangedFormulas;
            foreach (CalcLocalIdentity identity in changedFormulas.Keys)
            {
                manager.ClearExpression(identity);
            }
            foreach (KeyValuePair<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>>> pair in extChangedFormulas)
            {
                foreach (CalcLocalIdentity identity2 in pair.Value.Keys)
                {
                    pair.Key.ClearExpression(identity2);
                }
            }
            foreach (KeyValuePair<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>> pair2 in changedFormulas)
            {
                if (pair2.Value != null)
                {
                    CalcLocalIdentity id = pair2.Key;
                    manager.SetExpression(id, pair2.Value.Item2);
                    context.InvalidateFormula(manager, id);
                }
            }
            foreach (KeyValuePair<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>>> pair3 in extChangedFormulas)
            {
                foreach (KeyValuePair<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>> pair4 in pair3.Value)
                {
                    if (pair4.Value != null)
                    {
                        CalcLocalIdentity identity4 = pair4.Key;
                        pair3.Key.SetExpression(identity4, pair4.Value.Item2);
                        context.InvalidateFormula(pair3.Key, identity4);
                    }
                }
            }
            foreach (KeyValuePair<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, CalcLocalIdentity>> pair5 in context.InvalidateIdentities)
            {
                foreach (KeyValuePair<CalcLocalIdentity, CalcLocalIdentity> pair6 in pair5.Value)
                {
                    if (pair6.Value != null)
                    {
                        CalcLocalIdentity identity5 = pair6.Key;
                        pair5.Key.Invalidate(identity5, false);
                    }
                }
            }
        }

        static void UpdataCustomnNames(IFormulaOperatorSource mgr, Dt.Cells.Data.ExpressionVisitor visitor)
        {
            ICustomNameSupport source = mgr.Source as ICustomNameSupport;
            OperatorExpressionVisistor visistor = visitor as OperatorExpressionVisistor;
            if ((source != null) && (visistor != null))
            {
                Dictionary<string, CalcExpression> dictionary;
                Dictionary<ICustomNameSupport, Dictionary<string, CalcExpression>> dictionary2;
                CalcExpression expression;
                source.EnumerateAllNames(out dictionary, out dictionary2);
                visistor.OffsetAbsoluteOnly = true;
                visistor.CurrentCalcSource = mgr.Source;
                foreach (KeyValuePair<string, CalcExpression> pair in dictionary)
                {
                    expression = visitor.Visit(pair.Value, 0, 0);
                    if (expression != pair.Value)
                    {
                        source.SetDefinedName(pair.Key, expression, true);
                    }
                }
                foreach (KeyValuePair<ICustomNameSupport, Dictionary<string, CalcExpression>> pair2 in dictionary2)
                {
                    source = pair2.Key;
                    foreach (KeyValuePair<string, CalcExpression> pair3 in pair2.Value)
                    {
                        expression = visitor.Visit(pair3.Value, 0, 0);
                        if (expression != pair3.Value)
                        {
                            source.SetDefinedName(pair3.Key, expression, false);
                        }
                    }
                }
                if (visistor != null)
                {
                    visistor.OffsetAbsoluteOnly = false;
                    visistor.CurrentCalcSource = null;
                }
            }
        }

        public static void UpdataInvalidFormula(IFormulaOperatorSource mgr, ChangingContext context, Dt.Cells.Data.ExpressionVisitor visitor)
        {
            foreach (KeyValuePair<CalcLocalIdentity, CalcLocalIdentity> pair in context.ChangingIdentities)
            {
                if (mgr.GetExpression(pair.Key) != null)
                {
                    mgr.ClearExpression(pair.Key);
                }
                else
                {
                    mgr.ClearNode(pair.Key);
                }
            }
            foreach (KeyValuePair<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>> pair2 in context.ChangedFormulas)
            {
                if (pair2.Value == null)
                {
                    mgr.ClearExpression(pair2.Key);
                }
                else
                {
                    int num;
                    int num2;
                    int num3;
                    int num4;
                    Identity2Indexs(GetActualId(pair2.Value.Item1), out num, out num2, out num3, out num4);
                    CalcExpression expr = visitor.Visit(pair2.Value.Item2, num, num2);
                    mgr.SetExpression(pair2.Key, expr);
                }
                context.InvalidateFormula(mgr, pair2.Key);
            }
            OperatorExpressionVisistorBase base2 = visitor as OperatorExpressionVisistorBase;
            if (base2 != null)
            {
                base2.CurrentCalcSource = mgr.Source;
                base2.OffsetExternalOnly = true;
            }
            foreach (KeyValuePair<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>>> pair3 in context.ExtChangedFormulas)
            {
                foreach (KeyValuePair<CalcLocalIdentity, Tuple<CalcLocalIdentity, CalcExpression>> pair4 in pair3.Value)
                {
                    CalcLocalIdentity id = pair4.Key;
                    if (pair4.Value.Item2 == null)
                    {
                        pair3.Key.ClearExpression(id);
                    }
                    else
                    {
                        int num5;
                        int num6;
                        int num7;
                        int num8;
                        Identity2Indexs(GetActualId(pair4.Value.Item1), out num5, out num6, out num7, out num8);
                        CalcExpression expression2 = visitor.Visit(pair4.Value.Item2, num5, num6);
                        pair3.Key.SetExpression(id, expression2);
                    }
                    context.InvalidateFormula(pair3.Key, id);
                }
            }
            foreach (KeyValuePair<IFormulaOperatorSource, Dictionary<CalcLocalIdentity, CalcLocalIdentity>> pair5 in context.InvalidateIdentities)
            {
                List<CalcLocalIdentity> list = new List<CalcLocalIdentity>();
                foreach (KeyValuePair<CalcLocalIdentity, CalcLocalIdentity> pair6 in pair5.Value)
                {
                    if (pair6.Value != null)
                    {
                        list.Add(pair6.Key);
                    }
                }
                pair5.Key.Invalidate((IEnumerable<CalcLocalIdentity>) list, false);
            }
        }
    }
}

