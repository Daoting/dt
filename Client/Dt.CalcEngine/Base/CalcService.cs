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
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CalcService
    {
        private int _calcEngineSuspended;
        private readonly Dictionary<ICalcSource, CalcCalculationManager> _calcManagers = new Dictionary<ICalcSource, CalcCalculationManager>();
        private DirtyItem _headerDirtyItems;
        private double _minValue = 4.94065645841247E-322;
        private List<DirtyItem> _movedItems = new List<DirtyItem>();
        private readonly object _syncRoot = new object();
        private DirtyItem _tailDirtyItems;
        internal const int MAX_CALC_COUNT = 0xc350;

        internal void AddDirtyItem(DirtyItem item)
        {
            if ((item != null) && !this.IsDirtyItem(item))
            {
                if (this._tailDirtyItems != null)
                {
                    this._tailDirtyItems.NextItem = item;
                    item.Position = this._tailDirtyItems.Position + 1.0;
                }
                else
                {
                    item.Position = (this._headerDirtyItems != null) ? (this._headerDirtyItems.Position - 1.0) : 0.0;
                    this._headerDirtyItems = item;
                }
                item.PreviousItem = this._tailDirtyItems;
                item.NextItem = null;
                this._tailDirtyItems = item;
            }
        }

        private void AdjustPositions(DirtyItem item)
        {
            double position = 0.0;
            while (!object.ReferenceEquals(item.PreviousItem, null) && ((item.Position - item.PreviousItem.Position) < 0.0001))
            {
                item = item.PreviousItem;
            }
            if (object.ReferenceEquals(item, null))
            {
                item = this._headerDirtyItems;
                position = 0.0;
            }
            else
            {
                position = item.Position;
            }
            while (!object.ReferenceEquals(item, null))
            {
                position++;
                item.Position = position;
                item = item.NextItem;
            }
        }

        private void CheckPosition(DirtyItem item, DirtyItem at, DirtyItem prev)
        {
            if (((item.Position - prev.Position) < this._minValue) || ((at.Position - prev.Position) < this._minValue))
            {
                this.AdjustPositions(item);
            }
        }

        private void CheckPredencies(DirtyItem item, DirtyItem at)
        {
            if (item.HasPredencyItem)
            {
                foreach (KeyValuePair<CalcIdentity, DirtyItem> pair in item.PredencyItems)
                {
                    if (at.Position <= pair.Value.Position)
                    {
                        this.MoveTo(pair.Value, item);
                    }
                }
            }
        }

        private void ClearDirtyItem()
        {
            DirtyItem item2;
            for (DirtyItem item = this._headerDirtyItems; item != null; item = item2)
            {
                item2 = (item == null) ? null : item.NextItem;
                item.PreviousItem = null;
                item.NextItem = null;
            }
            this._headerDirtyItems = this._tailDirtyItems = null;
        }

        private void ClearMovedItemCatch()
        {
            foreach (DirtyItem item in this._movedItems)
            {
                item.Moved = false;
            }
            this._movedItems.Clear();
        }

        internal void DisposeCalculationManager(CalcCalculationManager mgr)
        {
            ICalcSource key = mgr.Source;
            if (key != null)
            {
                DirtyItem nextItem;
                if (this._calcManagers.ContainsKey(key))
                {
                    this._calcManagers.Remove(key);
                }
                for (DirtyItem item = this._headerDirtyItems; item != null; item = nextItem)
                {
                    nextItem = item.NextItem;
                    if (item.Source == key)
                    {
                        this.RemoveDirtyItem(item);
                    }
                }
            }
        }

        internal bool EvaluateFormula(CalcCalculationManager mgr, DirtyItem dirtyItem)
        {
            CalcLocalIdentity id = dirtyItem.Id as CalcLocalIdentity;
            if (object.ReferenceEquals(id, null))
            {
                return false;
            }
            CalcExpression objA = mgr.GetExpression(id);
            if (object.ReferenceEquals(objA, null))
            {
                return false;
            }
            ICalcSource source = mgr.Source;
            Dictionary<CalcLocalIdentity, CalcExpression> dictionary = new Dictionary<CalcLocalIdentity, CalcExpression>();
            if ((id is CalcRangeIdentity) && mgr.Graph.IsSharedFormula(id as CalcRangeIdentity))
            {
                List<CalcLocalIdentity> dirtySubIds = new List<CalcLocalIdentity>();
                SharedFormulaDirtyItem item = dirtyItem as SharedFormulaDirtyItem;
                if ((item != null) && !item.IsFullRangeDirty)
                {
                    dirtySubIds = (dirtyItem as SharedFormulaDirtyItem).DirtySubIds;
                }
                else
                {
                    dirtySubIds.Add(id);
                }
                foreach (CalcLocalIdentity identity3 in dirtySubIds)
                {
                    int num;
                    int num2;
                    int num3;
                    int num4;
                    bool flag;
                    CalcRangeIdentity identity4 = identity3 as CalcRangeIdentity;
                    CalcReferenceHelper.Id2Range(source, identity3, out num, out num2, out num3, out num4, out flag);
                    for (int i = num; i < (num + num3); i++)
                    {
                        for (int j = num2; j < (num2 + num4); j++)
                        {
                            if ((identity4 != null) && (identity4.IsFullRow || identity4.IsFullColumn))
                            {
                                new FullBandMappingVisitor(identity4.IsFullRow, identity4.IsFullRow ? i : j).Visit(objA, 0, 0);
                            }
                            CalcCellIdentity identity5 = new CalcCellIdentity(i, j);
                            CalcExpression expression = mgr.GetExpression(identity5);
                            if (((expression != null) && (expression == objA)) && !mgr.Graph.IsIsIntersectantWithArrayFormula(identity5))
                            {
                                dictionary[identity5] = objA;
                            }
                        }
                    }
                }
            }
            else
            {
                dictionary.Add(id, objA);
            }
            foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair in dictionary)
            {
                id = pair.Key;
                object obj2 = mgr.Evaluator.Evaluate(pair.Value, source.GetEvaluatorContext(id));
                while (!(id is CalcRangeIdentity) && ((obj2 is CalcReference) || (obj2 is CalcArray)))
                {
                    if (obj2 is CalcReference)
                    {
                        obj2 = ExtractValueFromReference(id, obj2);
                    }
                    if (obj2 is CalcArray)
                    {
                        obj2 = (obj2 as CalcArray).GetValue(0);
                    }
                }
                source.SetValue(id, obj2);
            }
            return true;
        }

        private static object ExtractValueFromReference(CalcLocalIdentity localId, object value)
        {
            for (CalcReference reference = value as CalcReference; (reference != null) && !(localId is CalcRangeIdentity); reference = value as CalcReference)
            {
                CalcCellIdentity objA = localId as CalcCellIdentity;
                int rowCount = reference.GetRowCount(0);
                int columnCount = reference.GetColumnCount(0);
                if (((reference.RangeCount <= 0) || (rowCount <= 0)) || (columnCount <= 0))
                {
                    value = CalcErrors.Reference;
                }
                else
                {
                    try
                    {
                        if (!object.ReferenceEquals(objA, null))
                        {
                            if ((reference.RangeCount != 1) || ((rowCount > 1) && (columnCount > 1)))
                            {
                                value = CalcErrors.Value;
                            }
                            else
                            {
                                int rowOffset = objA.RowIndex - reference.GetRow(0);
                                int columnOffset = objA.ColumnIndex - reference.GetColumn(0);
                                if ((rowCount == 1) && (columnCount == 1))
                                {
                                    value = reference.GetValue(0, 0, 0);
                                }
                                else if (((rowCount == 1) && (columnCount > 1)) && ((columnOffset >= 0) && (columnOffset < columnCount)))
                                {
                                    value = reference.GetValue(0, 0, columnOffset);
                                }
                                else if (((rowCount > 1) && (columnCount == 1)) && ((rowOffset >= 0) && (rowOffset < rowCount)))
                                {
                                    value = reference.GetValue(0, rowOffset, 0);
                                }
                                else
                                {
                                    value = CalcErrors.Value;
                                }
                            }
                        }
                        else
                        {
                            value = reference.GetValue(0, 0, 0);
                        }
                    }
                    catch (InvalidCastException)
                    {
                        value = CalcErrors.Value;
                    }
                }
            }
            return value;
        }

        private void GetBaseIndex(CalcLocalIdentity lid, out int row, out int column)
        {
            CalcCellIdentity objA = lid as CalcCellIdentity;
            if (!object.ReferenceEquals(objA, null))
            {
                row = objA.RowIndex;
                column = objA.ColumnIndex;
            }
            else
            {
                CalcRangeIdentity identity2 = lid as CalcRangeIdentity;
                if (!object.ReferenceEquals(identity2, null))
                {
                    row = identity2.RowIndex;
                    row = (row < 0) ? 0 : row;
                    column = identity2.ColumnIndex;
                    column = (column < 0) ? 0 : column;
                }
                else
                {
                    row = -1;
                    column = -1;
                }
            }
        }

        /// <summary>
        /// Gets the calculation manager.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="formulaStorage">The formula storage.</param>
        /// <param name="autoCreate">if set to <see langword="true" /> [auto create].</param>
        /// <returns></returns>
        public CalcCalculationManager GetCalculationManager(ICalcSource source, ICalcStorage<CalcLocalIdentity, CalcExpression> formulaStorage = null, bool autoCreate = true)
        {
            CalcCalculationManager manager;
            if (object.ReferenceEquals(source, null))
            {
                throw new ArgumentNullException("source");
            }
            if ((!this._calcManagers.TryGetValue(source, out manager) || object.ReferenceEquals(manager, null)) && autoCreate)
            {
                this._calcManagers[source] = manager = new CalcCalculationManager(this, source, formulaStorage);
                manager.UpdateStorage();
            }
            return manager;
        }

        internal void InsertToDirtyBefore(DirtyItem item, DirtyItem at)
        {
            if ((item != null) && !this.IsDirtyItem(item))
            {
                if (at == null)
                {
                    this.AddDirtyItem(item);
                }
                else
                {
                    DirtyItem previousItem = at.PreviousItem;
                    item.NextItem = at;
                    item.PreviousItem = previousItem;
                    if (previousItem == null)
                    {
                        this._headerDirtyItems = item;
                        item.Position = at.Position - 1.0;
                    }
                    else
                    {
                        previousItem.NextItem = item;
                        item.Position = previousItem.Position + ((at.Position - previousItem.Position) / 2.0);
                        this.CheckPosition(item, at, previousItem);
                    }
                    at.PreviousItem = item;
                    this.CheckPredencies(item, at);
                    this.ClearMovedItemCatch();
                }
            }
        }

        internal bool IsDirtyItem(DirtyItem item)
        {
            if (item.PreviousItem == null)
            {
                return (item == this._headerDirtyItems);
            }
            return true;
        }

        private void MoveTo(DirtyItem item, DirtyItem beforeAt)
        {
            if (!item.Moved)
            {
                item.Moved = true;
                this._movedItems.Add(item);
                this.RemoveDirtyItem(item);
                DirtyItem previousItem = beforeAt.PreviousItem;
                item.NextItem = beforeAt;
                item.PreviousItem = previousItem;
                beforeAt.PreviousItem = item;
                if (previousItem == null)
                {
                    this._headerDirtyItems = item;
                    item.Position = beforeAt.Position - 1.0;
                }
                else
                {
                    previousItem.NextItem = item;
                    item.Position = previousItem.Position + ((beforeAt.Position - previousItem.Position) / 2.0);
                    this.CheckPosition(item, beforeAt, previousItem);
                }
                this.CheckPredencies(item, beforeAt);
            }
        }

        /// <summary>
        /// Recalculates this instance.
        /// </summary>
        /// <param name="maxCalcCount">The max iterator.</param>
        /// <param name="forceRecalculateAll">Whether force recalculate all formula in current manager.</param>
        /// <returns><see langword="true" /> if all dirty nodes have been recalculated, otherwise, <see langword="false" /></returns>
        public bool Recalculate(int maxCalcCount = 0xc350, bool forceRecalculateAll = false)
        {
            if (forceRecalculateAll)
            {
                this.ClearDirtyItem();
                foreach (CalcCalculationManager manager in this._calcManagers.Values)
                {
                    if (manager.FormulaCount > 0)
                    {
                        manager.InvalidateAllIdentity();
                        maxCalcCount += manager.FormulaCount;
                    }
                }
            }
            int num = 0;
            while ((this._headerDirtyItems != null) && (num < maxCalcCount))
            {
                DirtyItem dirtyItem = this._headerDirtyItems;
                CalcCalculationManager mgr = this.GetCalculationManager(dirtyItem.Source, null, true);
                this.EvaluateFormula(mgr, dirtyItem);
                CalcNode objA = dirtyItem.Node;
                if (object.ReferenceEquals(objA, null))
                {
                    objA = mgr.Graph.GetNode(dirtyItem.Id);
                }
                if (!object.ReferenceEquals(objA, null))
                {
                    objA.ClearDirty();
                    if (dirtyItem == this._headerDirtyItems)
                    {
                        this.RemoveDirtyItem(dirtyItem);
                    }
                }
                else
                {
                    this.RemoveDirtyItem(dirtyItem);
                }
                num++;
            }
            return (num < maxCalcCount);
        }

        internal void RemoveDirtyItem(DirtyItem item)
        {
            if (this.IsDirtyItem(item))
            {
                DirtyItem previousItem = item.PreviousItem;
                DirtyItem nextItem = item.NextItem;
                if (previousItem != null)
                {
                    previousItem.NextItem = nextItem;
                }
                else
                {
                    this._headerDirtyItems = nextItem;
                }
                if (nextItem != null)
                {
                    nextItem.PreviousItem = previousItem;
                }
                else
                {
                    this._tailDirtyItems = previousItem;
                }
                item.PreviousItem = null;
                item.NextItem = null;
            }
        }

        /// <summary>
        /// Resumes all calculation graphs.
        /// </summary>
        public void ResumeCalcGraphs()
        {
            this._calcEngineSuspended = (this._calcEngineSuspended == 0) ? 0 : (this._calcEngineSuspended - 1);
            if (this._calcEngineSuspended == 0)
            {
                foreach (CalcCalculationManager manager in this._calcManagers.Values)
                {
                    manager.ResumeCalcGraph();
                }
            }
        }

        /// <summary>
        /// Suspends all calculation graphs.
        /// </summary>
        public void SuspendCalcGraphs()
        {
            this._calcEngineSuspended++;
        }

        internal bool IsGraphSuspended
        {
            get
            {
                return (this._calcEngineSuspended > 0);
            }
        }
    }
}

