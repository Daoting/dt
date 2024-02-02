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
using Dt.CalcEngine.Functions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal class FormulaClipboardHelper
    {
        public static void Clear(CalcCalculationManager mgr, CalcLocalIdentity id)
        {
            Dictionary<CalcCellIdentity, CalcExpression> dictionary;
            Dictionary<CalcRangeIdentity, CalcExpression> dictionary2;
            FormulaOperatorHelper.GetAllContainsIds(mgr, id, out dictionary, out dictionary2);
            foreach (CalcCellIdentity identity in dictionary.Keys)
            {
                mgr.ClearExpression(identity);
                mgr.Invalidate(identity, false);
            }
            foreach (CalcRangeIdentity identity2 in dictionary2.Keys)
            {
                mgr.ClearExpression(identity2);
                mgr.Invalidate(identity2, false);
            }
        }

        static void GetDependentsFromulas(CalcCalculationManager sourceMgr, CalcCalculationManager targetMgr, ChangingContext context, int fromRow, int fromColumn, int toRow, int toColumn, int fromRowCount, Dictionary<CalcLocalIdentity, CalcExpression> formulas, int fromColumnCount, bool offsetSelf, bool updataDependens)
        {
            int rowOffset = toRow - fromRow;
            int columnOffset = toColumn - fromColumn;
            CalcLocalIdentity id = FormulaOperatorHelper.Indexs2Identity(fromRow, fromColumn, (fromRow + fromRowCount) - 1, (fromColumn + fromColumnCount) - 1);
            CellRange fromRange = new CellRange(fromRow, fromColumn, fromRowCount, fromColumnCount);
            MoveVisitor visitor = new MoveVisitor(fromRange, rowOffset, columnOffset, 0x7fffffff, 0x7fffffff, true, targetMgr.Source, sourceMgr != targetMgr, null, !offsetSelf);
            sourceMgr.Source.GetEvaluatorContext(new CalcCellIdentity(toRow, toColumn));
            new Dictionary<CalcIdentity, CalcIdentity>();
            CalcNode node = sourceMgr.Graph.GetNode(id);
            IFormulaOperatorSource mgr = new SpreadCalcAxialManager.FormulaOperatorSource(sourceMgr);
            List<CalcLocalIdentity> list = new List<CalcLocalIdentity>();
            HashSet<CalcNode> arrayFormulaNodes = new HashSet<CalcNode>();
            if ((node != null) && updataDependens)
            {
                list = FormulaOperatorHelper.GetChangedDependents(mgr, context, node, arrayFormulaNodes, formulas, null);
            }
            if (offsetSelf && updataDependens)
            {
                foreach (CalcIdentity identity2 in sourceMgr.Graph.GetAllLocalIdentities())
                {
                    int num3;
                    int num4;
                    int num5;
                    int num6;
                    FormulaOperatorHelper.Identity2Indexs(identity2, out num3, out num4, out num5, out num6);
                    if ((((fromRow == -1) || (num3 >= fromRow)) && ((fromRow == -1) || (num5 < (fromRow + fromRowCount)))) && (((fromColumn == -1) || (num4 >= fromColumn)) && ((fromColumn == -1) || (num6 < (fromColumn + fromColumnCount)))))
                    {
                        CalcNode node2 = sourceMgr.Graph.GetNode(identity2);
                        if ((node2 != null) && (node2.Dependents != null))
                        {
                            list.AddRange((IEnumerable<CalcLocalIdentity>) FormulaOperatorHelper.GetChangedDependents(mgr, context, node2, arrayFormulaNodes, formulas, null));
                        }
                    }
                }
                visitor.CurrentCalcSource = sourceMgr.Source;
                foreach (CalcLocalIdentity identity3 in list)
                {
                    int num7;
                    int num8;
                    int num9;
                    int num10;
                    CalcExpression expr = formulas[identity3];
                    FormulaOperatorHelper.Identity2Indexs(identity3, out num7, out num8, out num9, out num10);
                    expr = visitor.Visit(expr, num7, num8);
                    formulas[identity3] = expr;
                }
            }
            FormulaOperatorHelper.UpdataInvalidFormula(mgr, context, visitor);
        }

        static void GetTargetFormulas(CalcCalculationManager sourceMgr, CalcCalculationManager targetMgr, int sourceRowCount, int sourceColumnCount, int fromRow, int fromColumn, int toRow, int toColumn, int fromRowCount, int fromColumnCount, int toRowCount, int toColumnCount, Dictionary<CalcCellIdentity, CalcExpression> cellFormulas, Dictionary<CalcRangeIdentity, CalcExpression> rangeFormulas, bool offsetSelf, out Dictionary<CalcLocalIdentity, CalcExpression> newExpressions)
        {
            MoveVisitor visitor;
            int rowOffset = toRow - fromRow;
            int columnOffset = toColumn - fromColumn;
            CellRange fromRange = new CellRange(fromRow, fromColumn, fromRowCount, fromColumnCount);
            MoveVisitor visitor2 = new MoveVisitor(fromRange, rowOffset, columnOffset, sourceRowCount, sourceColumnCount, false, sourceMgr.Source, sourceMgr != targetMgr, null, !offsetSelf);
            newExpressions = new Dictionary<CalcLocalIdentity, CalcExpression>();
            if ((cellFormulas != null) && (cellFormulas.Count > 0))
            {
                for (int i = toRow; i < (toRow + toRowCount); i++)
                {
                    int rowIndex = ((i - toRow) % fromRowCount) + fromRow;
                    for (int j = toColumn; j < (toColumn + toColumnCount); j++)
                    {
                        int columnIndex = ((j - toColumn) % fromColumnCount) + fromColumn;
                        CalcCellIdentity identity = new CalcCellIdentity(rowIndex, columnIndex);
                        CalcCellIdentity id = new CalcCellIdentity(i, j);
                        if (cellFormulas.ContainsKey(identity))
                        {
                            CalcExpression expr = cellFormulas[identity];
                            if (!offsetSelf)
                            {
                                visitor = new MoveVisitor(fromRange, rowOffset, columnOffset, sourceRowCount, sourceColumnCount, false, sourceMgr.Source, false, null, true);
                                expr = visitor.FirstVisit(expr, identity.RowIndex, identity.ColumnIndex);
                            }
                            else
                            {
                                expr = visitor2.Visit(expr, identity.RowIndex, identity.ColumnIndex);
                            }
                            newExpressions[id] = expr;
                        }
                        else if (targetMgr.GetExpression(id) != null)
                        {
                            newExpressions[id] = null;
                        }
                    }
                }
            }
            int num7 = (toRow + toRowCount) - 1;
            int num8 = (toColumn + toColumnCount) - 1;
            if ((rangeFormulas != null) && (rangeFormulas.Count > 0))
            {
                foreach (KeyValuePair<CalcRangeIdentity, CalcExpression> pair in rangeFormulas)
                {
                    int baseRow = (pair.Key.RowIndex < 0) ? 0 : pair.Key.RowIndex;
                    int baseColumn = (pair.Key.ColumnIndex < 0) ? 0 : pair.Key.ColumnIndex;
                    if (!pair.Key.IsFullRow || !pair.Key.IsFullColumn)
                    {
                        if (pair.Key.IsFullColumn)
                        {
                            for (int k = toColumn; k <= num8; k += fromColumnCount)
                            {
                                int num12 = (k + pair.Key.ColumnCount) - 1;
                                num12 = (num12 > num8) ? (num8 - k) : num12;
                                CalcRangeIdentity identity3 = new CalcRangeIdentity((k + pair.Key.ColumnIndex) - fromColumn, (num12 - k) + 1, false);
                                CalcExpression expression2 = pair.Value;
                                if (!offsetSelf)
                                {
                                    visitor = new MoveVisitor(fromRange, 0, columnOffset, sourceRowCount, sourceColumnCount, false, sourceMgr.Source, sourceMgr != targetMgr, null, true);
                                    expression2 = visitor.Visit(expression2, baseRow, baseColumn);
                                }
                                else
                                {
                                    expression2 = visitor2.Visit(expression2, baseRow, baseColumn);
                                }
                                newExpressions[identity3] = expression2;
                            }
                        }
                        else if (pair.Key.IsFullRow)
                        {
                            for (int m = toRow; m <= num7; m += fromRowCount)
                            {
                                int num14 = (m + pair.Key.RowCount) - 1;
                                num14 = (num14 > num7) ? (num7 - m) : num14;
                                CalcRangeIdentity identity4 = new CalcRangeIdentity((m + pair.Key.RowIndex) - fromRow, (num14 - m) + 1, true);
                                CalcExpression expression3 = pair.Value;
                                if (!offsetSelf)
                                {
                                    visitor = new MoveVisitor(fromRange, rowOffset, 0, sourceRowCount, sourceColumnCount, false, sourceMgr.Source, sourceMgr != targetMgr, null, true);
                                    expression3 = visitor.Visit(expression3, baseRow, baseColumn);
                                }
                                else
                                {
                                    expression3 = visitor2.Visit(expression3, baseRow, baseColumn);
                                }
                                newExpressions[identity4] = expression3;
                            }
                        }
                        else
                        {
                            for (int n = toRow; n <= num7; n += fromRowCount)
                            {
                                int num16 = (n + pair.Key.RowCount) - 1;
                                num16 = (num16 > num7) ? (num7 - n) : num16;
                                for (int num17 = toColumn; num17 <= num8; num17 += fromColumnCount)
                                {
                                    int num18 = (num17 + pair.Key.ColumnCount) - 1;
                                    num18 = (num18 > num8) ? (num8 - num17) : num18;
                                    CalcRangeIdentity identity5 = new CalcRangeIdentity((n + pair.Key.RowIndex) - fromRow, (num17 + pair.Key.ColumnIndex) - fromColumn, (num16 - n) + 1, (num18 - num17) + 1);
                                    CalcExpression expression4 = pair.Value;
                                    if (!offsetSelf)
                                    {
                                        expression4 = new MoveVisitor(fromRange, rowOffset, columnOffset, sourceRowCount, sourceColumnCount, false, sourceMgr.Source, sourceMgr != targetMgr, null, true).Visit(expression4, baseRow, baseColumn);
                                    }
                                    else
                                    {
                                        expression4 = visitor2.Visit(expression4, baseRow, baseColumn);
                                    }
                                    newExpressions[identity5] = expression4;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Save(CalcCalculationManager sourceMgr, CalcCalculationManager targetMgr, ChangingContext context, int sourceRowCount, int sourceColumnCount, int fromRow, int fromColumn, int toRow, int toColumn, int fromRowCount, int fromColumnCount, int toRowCount, int toColumnCount, Dictionary<CalcLocalIdentity, CalcExpression> formulas, bool offsetSelf, bool updataDependens)
        {
            Dictionary<CalcLocalIdentity, CalcExpression> dictionary3;
            if (context == null)
            {
                context = new ChangingContext();
            }
            GetDependentsFromulas(sourceMgr, targetMgr, context, fromRow, fromColumn, toRow, toColumn, fromRowCount, formulas, fromColumnCount, offsetSelf, updataDependens);
            Dictionary<CalcCellIdentity, CalcExpression> cellFormulas = new Dictionary<CalcCellIdentity, CalcExpression>();
            Dictionary<CalcRangeIdentity, CalcExpression> rangeFormulas = new Dictionary<CalcRangeIdentity, CalcExpression>();
            foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair in formulas)
            {
                if (pair.Key is CalcCellIdentity)
                {
                    cellFormulas[pair.Key as CalcCellIdentity] = pair.Value;
                }
                else if (pair.Key is CalcRangeIdentity)
                {
                    rangeFormulas[pair.Key as CalcRangeIdentity] = pair.Value;
                }
            }
            GetTargetFormulas(sourceMgr, targetMgr, sourceRowCount, sourceColumnCount, fromRow, fromColumn, toRow, toColumn, fromRowCount, fromColumnCount, toRowCount, toColumnCount, cellFormulas, rangeFormulas, offsetSelf, out dictionary3);
            ReferenceValidateVisitor visitor = new ReferenceValidateVisitor(new int?(sourceRowCount - 1), new int?(sourceColumnCount - 1));
            Clear(targetMgr, FormulaOperatorHelper.Indexs2Identity(toRow, toColumn, (toRow + toRowCount) - 1, (toColumn + toColumnCount) - 1));
            foreach (KeyValuePair<CalcLocalIdentity, CalcExpression> pair2 in dictionary3)
            {
                if (pair2.Value == null)
                {
                    targetMgr.ClearExpression(pair2.Key);
                }
                else
                {
                    int num;
                    int num2;
                    int num3;
                    int num4;
                    CalcExpression expr = pair2.Value;
                    FormulaOperatorHelper.Identity2Indexs(pair2.Key, out num, out num2, out num3, out num4);
                    num = (num < 0) ? 0 : num;
                    num2 = (num2 < 0) ? 0 : num2;
                    num3 = (num3 < 0) ? 0 : num3;
                    num4 = (num4 < 0) ? 0 : num4;
                    expr = visitor.Visit(expr, num, num2);
                    if (expr is CalcSharedExpression)
                    {
                        expr = visitor.Visit(expr, num3, num4);
                    }
                    bool? isArrayFormula = null;
                    CalcExpressionHelper.SetExpression(targetMgr, pair2.Key, expr, isArrayFormula);
                }
                targetMgr.Invalidate(pair2.Key, false);
            }
        }

        class NoneSource : ICalcSource, IEqualityComparer<ICalcSource>
        {
            public bool Equals(ICalcSource x, ICalcSource y)
            {
                throw new InvalidOperationException();
            }

            public int GetColumnCount()
            {
                return this.ColumnCount;
            }

            public CalcExpression GetDefinedName(string name, int row, int column)
            {
                throw new InvalidOperationException();
            }

            public CalcEvaluatorContext GetEvaluatorContext(CalcLocalIdentity baseAddress)
            {
                throw new InvalidOperationException();
            }

            public CalcFunction GetFunction(string functionName)
            {
                throw new InvalidOperationException();
            }

            public int GetHashCode(ICalcSource obj)
            {
                throw new InvalidOperationException();
            }

            public CalcParserContext GetParserContext(CalcLocalIdentity baseAddress)
            {
                throw new InvalidOperationException();
            }

            public object GetReference(CalcLocalIdentity id)
            {
                throw new InvalidOperationException();
            }

            public int GetRowCount()
            {
                return this.RowCount;
            }

            public object GetValue(CalcLocalIdentity id)
            {
                throw new InvalidOperationException();
            }

            public void SetValue(CalcLocalIdentity id, object value)
            {
                throw new InvalidOperationException();
            }

            public int ColumnCount
            {
                get
                {
                    throw new InvalidOperationException();
                }
            }

            public int RowCount
            {
                get
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}

