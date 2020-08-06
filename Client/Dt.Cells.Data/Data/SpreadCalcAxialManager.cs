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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal class SpreadCalcAxialManager : IRangeSupport, ICrossSheetRangeSupport
    {
        IFormulaOperatorSource _formulaOperatorSource;
        CalcCalculationManager _manager;
        int _suspendCount;

        public SpreadCalcAxialManager(Worksheet sheet, CalcCalculationManager manager, ICalcStorage<CalcLocalIdentity, CalcExpression> storage)
        {
            this.Sheet = sheet;
            this.Manager = manager;
            this.Storage = storage;
        }

        public void AddColumns(int column, int count)
        {
            if ((count > 0) && !this.Suspended)
            {
                if (this.Manager.GetExpression(new CalcRangeIdentity(column, 1, false)) == null)
                {
                    CalcExpressionHelper.ValidateIntersectedArrayFormula(this.Manager, -1, column, -1, 1, true);
                }
                FormulaOperatorHelper.Insert(this._formulaOperatorSource, column, count, false, false, true, true);
            }
        }

        public void AddRows(int row, int count)
        {
            if ((count > 0) && !this.Suspended)
            {
                if (this.Manager.GetExpression(new CalcRangeIdentity(row, 1, true)) == null)
                {
                    CalcExpressionHelper.ValidateIntersectedArrayFormula(this.Manager, row, -1, 1, -1, true);
                }
                FormulaOperatorHelper.Insert(this._formulaOperatorSource, row, count, true, false, true, true);
            }
        }

        public void Clear(int row, int column, int rowCount, int columnCount)
        {
            row = (rowCount <= 0) ? -1 : row;
            column = (columnCount <= 0) ? -1 : column;
            FormulaClipboardHelper.Clear(this.Manager, FormulaOperatorHelper.Indexs2Identity(row, column, (row + rowCount) - 1, (column + columnCount) - 1));
        }

        public void Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.Copy(null, fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        public void Copy(SpreadCalcAxialManager src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.Copy(src, fromRow, fromColumn, toRow, toColumn, rowCount, columnCount, rowCount, columnCount);
        }

        public void Copy(SpreadCalcAxialManager src, int fromRow, int fromColumn, int toRow, int toColumn, int fromRowCount, int fromColumnCount, int toRowCount, int toColumnCount)
        {
            CalcCalculationManager sourceMgr = (src == null) ? this.Manager : src.Manager;
            Dictionary<CalcLocalIdentity, CalcExpression> formulas = this.GetFormulaIds(fromRow, fromColumn, fromRowCount, fromColumnCount, sourceMgr, null, false);
            this.Save(src, fromRow, fromColumn, toRow, toColumn, fromRowCount, fromColumnCount, toRowCount, toColumnCount, formulas, false, false);
        }

        Dictionary<CalcLocalIdentity, CalcExpression> GetFormulaIds(int fromRow, int fromColumn, int rowCount, int columnCount, CalcCalculationManager sourceMgr, ChangingContext context, bool clearSource)
        {
            CalcLocalIdentity id = FormulaOperatorHelper.Indexs2Identity(fromRow, fromColumn, (fromRow + rowCount) - 1, (fromColumn + columnCount) - 1);
            Dictionary<CalcLocalIdentity, CalcExpression> allContainsIds = FormulaOperatorHelper.GetAllContainsIds(sourceMgr, id);
            if (clearSource)
            {
                foreach (CalcLocalIdentity identity2 in allContainsIds.Keys)
                {
                    if (sourceMgr == this.Manager)
                    {
                        context.ChangedFormulas[identity2] = null;
                    }
                    else
                    {
                        context.AddExtChangedFormula(new FormulaOperatorSource(sourceMgr), identity2, identity2, null);
                    }
                }
            }
            return allContainsIds;
        }

        void ICrossSheetRangeSupport.Copy(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.Copy(src.CalcAxial, fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        void ICrossSheetRangeSupport.Move(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.Move(src.CalcAxial, fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        public void InsertSheet(IList sources)
        {
            foreach (object obj2 in sources)
            {
                ICalcSource source = obj2 as ICalcSource;
                if (source != null)
                {
                    CalcCalculationManager mgr = this.Manager.Service.GetCalculationManager(source, null, true);
                    if (mgr != null)
                    {
                        foreach (CalcLocalIdentity identity in mgr.Graph.GetAllSheetRangeIdentities())
                        {
                            CalcExpression expr = mgr.GetExpression(identity);
                            CalcExpressionHelper.SetExpression(mgr, identity, expr, true);
                        }
                    }
                }
            }
        }

        public void Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.Move(null, fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        public void Move(SpreadCalcAxialManager src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            CalcCalculationManager sourceMgr = (src == null) ? this.Manager : src.Manager;
            ChangingContext context = new ChangingContext();
            Dictionary<CalcLocalIdentity, CalcExpression> formulas = this.GetFormulaIds(fromRow, fromColumn, rowCount, columnCount, sourceMgr, context, true);
            int sourceRowCount = (src == null) ? this.RowCount : src.RowCount;
            int sourceColumnCount = (src == null) ? this.ColumnCount : src.ColumnCount;
            FormulaClipboardHelper.Save(sourceMgr, this.Manager, context, sourceRowCount, sourceColumnCount, fromRow, fromColumn, toRow, toColumn, rowCount, columnCount, rowCount, columnCount, formulas, true, true);
        }

        static CalcLocalIdentity Offset(CalcLocalIdentity id, int rowOffset, int colOffset)
        {
            CalcCellIdentity cellIdentity = id as CalcCellIdentity;
            CalcRangeIdentity range = id as CalcRangeIdentity;
            if (cellIdentity != null)
            {
                return new CalcCellIdentity(cellIdentity, rowOffset, colOffset);
            }
            if (range != null)
            {
                return new CalcRangeIdentity(range, rowOffset, colOffset);
            }
            return id;
        }

        public void RemoveColumns(int column, int count)
        {
            if ((count > 0) && !this.Suspended)
            {
                if (this.Manager.GetExpression(new CalcRangeIdentity(column, count, false)) == null)
                {
                    CalcExpressionHelper.ValidateIntersectedArrayFormula(this.Manager, -1, column, -1, count, false);
                }
                FormulaOperatorHelper.Remove(this._formulaOperatorSource, column, count, false, false, true, true);
            }
        }

        public void RemoveRows(int row, int count)
        {
            if ((count > 0) && !this.Suspended)
            {
                if (this.Manager.GetExpression(new CalcRangeIdentity(row, 1, false)) == null)
                {
                    CalcExpressionHelper.ValidateIntersectedArrayFormula(this.Manager, row, -1, count, -1, false);
                }
                FormulaOperatorHelper.Remove(this._formulaOperatorSource, row, count, true, false, true, true);
            }
        }

        public void RemoveSheet(IList sources, ICalcSource replacedSource)
        {
            ICalcSource currentSource = this.Manager.Source;
            Dictionary<CalcCalculationManager, Dictionary<CalcLocalIdentity, CalcExpression>> dictionary = new Dictionary<CalcCalculationManager, Dictionary<CalcLocalIdentity, CalcExpression>>();
            foreach (object obj2 in sources)
            {
                ICalcSource source = obj2 as ICalcSource;
                if (source != null)
                {
                    CalcCalculationManager manager = this.Manager.Service.GetCalculationManager(source, null, true);
                    if (manager != null)
                    {
                        foreach (CalcLocalIdentity identity in manager.Graph.GetAllSheetRangeIdentities())
                        {
                            CalcNode node = manager.Graph.GetNode(identity);
                            if (node.Precedents != null)
                            {
                                using (List<CalcNode>.Enumerator enumerator3 = node.Precedents.GetEnumerator())
                                {
                                    while (enumerator3.MoveNext())
                                    {
                                        if (enumerator3.Current.Source == currentSource)
                                        {
                                            Dictionary<CalcLocalIdentity, CalcExpression> dictionary2;
                                            if (!dictionary.TryGetValue(manager, out dictionary2))
                                            {
                                                dictionary2 = new Dictionary<CalcLocalIdentity, CalcExpression>();
                                                dictionary[manager] = dictionary2;
                                            }
                                            dictionary2[identity] = manager.GetExpression(identity);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            RemoveSheetVisitor visitor = new RemoveSheetVisitor(currentSource, replacedSource);
            foreach (KeyValuePair<CalcCalculationManager, Dictionary<CalcLocalIdentity, CalcExpression>> pair in dictionary)
            {
                CalcCalculationManager mgr = pair.Key;
                foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair2 in pair.Value)
                {
                    int num;
                    int num2;
                    int num3;
                    int num4;
                    mgr.ClearExpression(pair2.Key);
                    FormulaOperatorHelper.Identity2Indexs(pair2.Key, out num, out num2, out num3, out num4);
                    CalcExpression expr = visitor.Visit(pair2.Value, num, num2);
                    CalcExpressionHelper.SetExpression(mgr, pair2.Key, expr, true);
                    mgr.Invalidate(pair2.Key, false);
                }
            }
        }

        public void ReorderColumns(int fromColumn, int toColumn, int columnCount)
        {
            if ((fromColumn != toColumn) && !this.Suspended)
            {
                if (((this.Manager.GetExpression(new CalcRangeIdentity(fromColumn, columnCount, false)) == null) && this.Manager.IsIsIntersectantWithArrayFormula(new CalcRangeIdentity(fromColumn, columnCount, false))) || ((this.Manager.GetExpression(new CalcRangeIdentity(toColumn, columnCount, false)) == null) && this.Manager.IsIsIntersectantWithArrayFormula(new CalcRangeIdentity(toColumn, columnCount, false))))
                {
                    throw new InvalidOperationException(ResourceStrings.FormulaChangePartOfArrayFormulaError);
                }
                FormulaOperatorHelper.Reorder(this._formulaOperatorSource, this.Storage, fromColumn, toColumn, columnCount, false, true, true);
            }
        }

        public void ReorderRows(int fromRow, int toRow, int rowCount)
        {
            if ((fromRow != toRow) && !this.Suspended)
            {
                if (((this.Manager.GetExpression(new CalcRangeIdentity(fromRow, rowCount, false)) == null) && this.Manager.IsIsIntersectantWithArrayFormula(new CalcRangeIdentity(fromRow, rowCount, true))) || ((this.Manager.GetExpression(new CalcRangeIdentity(toRow, rowCount, false)) == null) && this.Manager.IsIsIntersectantWithArrayFormula(new CalcRangeIdentity(toRow, rowCount, true))))
                {
                    throw new InvalidOperationException(ResourceStrings.FormulaChangePartOfArrayFormulaError);
                }
                FormulaOperatorHelper.Reorder(this._formulaOperatorSource, this.Storage, fromRow, toRow, rowCount, true, true, true);
            }
        }

        public void Resume()
        {
            this._suspendCount--;
        }

        public void Save(SpreadCalcAxialManager src, int fromRow, int fromColumn, int toRow, int toColumn, int fromRowCount, int fromColumnCount, int toRowCount, int toColumnCount, Dictionary<CalcLocalIdentity, CalcExpression> formulas, bool offsetSelf, bool updataDependens)
        {
            CalcCalculationManager sourceMgr = (src == null) ? this.Manager : src.Manager;
            int sourceRowCount = (src == null) ? this.RowCount : src.RowCount;
            int sourceColumnCount = (src == null) ? this.ColumnCount : src.ColumnCount;
            FormulaClipboardHelper.Save(sourceMgr, this.Manager, null, sourceRowCount, sourceColumnCount, fromRow, fromColumn, toRow, toColumn, fromRowCount, fromColumnCount, toRowCount, toColumnCount, formulas, offsetSelf, updataDependens);
        }

        public void Sort(IDictionary<int, int> movedRows)
        {
            if (movedRows.Count != 0)
            {
                ChangingContext context = new ChangingContext();
                foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair in this.Storage)
                {
                    CalcCellIdentity identity = pair.Key as CalcCellIdentity;
                    if (identity != null)
                    {
                        int num2;
                        int rowIndex = identity.RowIndex;
                        if (movedRows.TryGetValue(rowIndex, out num2))
                        {
                            if (!context.ChangedFormulas.ContainsKey(identity))
                            {
                                context.ChangedFormulas[identity] = null;
                            }
                            if (num2 >= 0)
                            {
                                CalcCellIdentity identity2 = new CalcCellIdentity(num2, identity.ColumnIndex);
                                CalcExpression expr = pair.Value;
                                int? maxColCount = null;
                                expr = new ReferenceValidateVisitor(new int?(this.RowCount - 1), maxColCount).Visit(expr, num2, identity.ColumnIndex);
                                context.ChangedFormulas[identity2] = new Tuple<CalcLocalIdentity, CalcExpression>(identity, expr);
                            }
                        }
                    }
                }
                FormulaOperatorHelper.UpdataChangings(this._formulaOperatorSource, context);
            }
        }

        public void Suspend()
        {
            this._suspendCount++;
        }

        public void Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            CalcLocalIdentity id = FormulaOperatorHelper.Indexs2Identity(fromRow, fromColumn, (fromRow + rowCount) - 1, (fromColumn + columnCount) - 1);
            Dictionary<CalcLocalIdentity, CalcExpression> allContainsIds = FormulaOperatorHelper.GetAllContainsIds(this.Manager, id);
            CalcLocalIdentity identity2 = FormulaOperatorHelper.Indexs2Identity(toRow, toColumn, (toRow + rowCount) - 1, (toColumn + columnCount) - 1);
            Dictionary<CalcLocalIdentity, CalcExpression> dictionary2 = FormulaOperatorHelper.GetAllContainsIds(this.Manager, identity2);
            foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair in allContainsIds)
            {
                this.Manager.ClearExpression(pair.Key);
            }
            foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair2 in dictionary2)
            {
                this.Manager.ClearExpression(pair2.Key);
            }
            int rowOffset = toRow - fromRow;
            int colOffset = toColumn - fromColumn;
            ReferenceValidateVisitor visitor = new ReferenceValidateVisitor(new int?(this.RowCount), new int?(this.ColumnCount));
            foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair3 in allContainsIds)
            {
                int num3;
                int num4;
                int num5;
                int num6;
                FormulaOperatorHelper.Identity2Indexs(pair3.Key, out num3, out num4, out num5, out num6);
                CalcLocalIdentity identity3 = Offset(pair3.Key, rowOffset, colOffset);
                bool? isArrayFormula = null;
                CalcExpressionHelper.SetExpression(this.Manager, identity3, visitor.Visit(pair3.Value, num3 + rowOffset, num4 + colOffset), isArrayFormula);
                this.Manager.Invalidate(identity3, false);
            }
            foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair4 in dictionary2)
            {
                int num7;
                int num8;
                int num9;
                int num10;
                FormulaOperatorHelper.Identity2Indexs(pair4.Key, out num7, out num8, out num9, out num10);
                CalcLocalIdentity identity4 = Offset(pair4.Key, -rowOffset, -colOffset);
                bool? nullable2 = null;
                CalcExpressionHelper.SetExpression(this.Manager, identity4, visitor.Visit(pair4.Value, num7 - rowOffset, num8 - colOffset), nullable2);
                this.Manager.Invalidate(identity4, false);
            }
        }

        public int ColumnCount
        {
            get { return  this.Sheet.ColumnCount; }
        }

        public CalcCalculationManager Manager
        {
            get { return  this._manager; }
            private set
            {
                this._manager = value;
                this._formulaOperatorSource = new FormulaOperatorSource(this._manager);
            }
        }

        public int RowCount
        {
            get { return  this.Sheet.RowCount; }
        }

        public Worksheet Sheet { get; private set; }

        public ICalcStorage<CalcLocalIdentity, CalcExpression> Storage { get; set; }

        bool Suspended
        {
            get { return  (this._suspendCount > 0); }
        }

        internal class FormulaOperatorSource : IFormulaOperatorSource
        {
            public FormulaOperatorSource(CalcCalculationManager manager)
            {
                this.Manager = manager;
            }

            public void ClearExpression(CalcLocalIdentity id)
            {
                this.Manager.ClearExpression(id);
            }

            public void ClearNode(CalcLocalIdentity id)
            {
                this.Manager.Graph.RemoveNode(id);
            }

            public override bool Equals(object obj)
            {
                SpreadCalcAxialManager.FormulaOperatorSource source = obj as SpreadCalcAxialManager.FormulaOperatorSource;
                if (source == null)
                {
                    return false;
                }
                return (source.Manager == this.Manager);
            }

            public IEnumerable<CalcLocalIdentity> GetAllLocalIdentities()
            {
                return this.Manager.Graph.GetAllLocalIdentities();
            }

            public CalcExpression GetExpression(CalcLocalIdentity id)
            {
                return this.Manager.GetExpression(id);
            }

            public IFormulaOperatorSource GetExternalManager(ICalcSource source)
            {
                return new SpreadCalcAxialManager.FormulaOperatorSource(this.Manager.Service.GetCalculationManager(source, null, true));
            }

            public override int GetHashCode()
            {
                return this.Manager.GetHashCode();
            }

            public CalcNode GetNode(CalcIdentity id)
            {
                return this.Manager.Graph.GetNode(id);
            }

            public void Invalidate(CalcLocalIdentity id, bool autoCalculate)
            {
                this.Manager.Invalidate(id, autoCalculate);
            }

            public void Invalidate(IEnumerable<CalcLocalIdentity> ids, bool autoCalculate)
            {
                this.Manager.Invalidate(ids, autoCalculate);
            }

            public void SetExpression(CalcLocalIdentity id, CalcExpression expr)
            {
                CalcExpressionHelper.SetExpression(this.Manager, id, expr, null);
            }

            public CalcCalculationManager Manager { get; set; }

            public IMultiSourceProvider MultiSourceProvider
            {
                get { return  (this.Manager.Source as IMultiSourceProvider); }
            }

            public ICalcSource Source
            {
                get { return  this.Manager.Source; }
            }
        }
    }
}

