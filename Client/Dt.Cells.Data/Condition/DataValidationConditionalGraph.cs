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
#endregion

namespace Dt.Cells.Data
{
    internal class DataValidationConditionalGraph : ConditionalGraph, IRangeSupport, ICrossSheetRangeSupport
    {
        Dt.Cells.Data.ReferenceStyle _referenceStyle;
        int _sheetColumnCount;
        int _sheetRowCount;
        ICalcSource _source;

        public DataValidationConditionalGraph(ICalcSource source, Dt.Cells.Data.ReferenceStyle referenceStyle, int sheetRowCount, int sheetColumnCount)
        {
            this._source = source;
            this._referenceStyle = referenceStyle;
            this._sheetRowCount = sheetRowCount;
            this._sheetColumnCount = sheetColumnCount;
        }

        public void AddColumns(int column, int count)
        {
            base.Insert(column, count, false, false);
        }

        public void AddDataValidation(int row, int column, DataValidator validator)
        {
            IConditionalFormula[] formulaConditions = validator.FormulaConditions;
            if ((formulaConditions != null) && (formulaConditions.Length != 0))
            {
                CalcLocalIdentity actualId = FormulaOperatorHelper.Indexs2Identity(row, column, row, column);
                List<IConditionalFormula> list = new List<IConditionalFormula>();
                list.AddRange(formulaConditions);
                base.AddConditionals(actualId, (ICollection<IConditionalFormula>) list);
            }
        }

        public void AddRows(int row, int count)
        {
            base.Insert(row, count, true, false);
        }

        public void Clear(int row, int column, int rowCount, int columnCount)
        {
            if (rowCount < 0)
            {
                rowCount = 1;
            }
            if (columnCount < 0)
            {
                columnCount = 1;
            }
            for (int i = row; i < (row + rowCount); i++)
            {
                for (int j = column; j < (column + columnCount); j++)
                {
                    this.RemoveDataValidation(i, j);
                }
            }
        }

        public void Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new InvalidOperationException();
        }

        public void Copy(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new InvalidOperationException();
        }

        Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>> GetAllFromulas(int row, int col, int rowCount, int colCount)
        {
            Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>> dictionary = new Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>>();
            rowCount = (rowCount <= 0) ? 1 : rowCount;
            colCount = (colCount <= 0) ? 1 : colCount;
            for (int i = 0; i < (row + rowCount); i++)
            {
                for (int j = 0; j < (col + colCount); j++)
                {
                    CalcLocalIdentity actualId = FormulaOperatorHelper.Indexs2Identity(i, j, i, j);
                    List<ConditionalGraph.ConditionalIdentity> formulas = base.GetFormulas(actualId);
                    if ((formulas != null) && (formulas.Count > 0))
                    {
                        dictionary[actualId] = formulas;
                    }
                }
            }
            return dictionary;
        }

        protected override IFormulaOperatorSource GetExternalManager(ICalcSource source)
        {
            return this;
        }

        public void Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>> dictionary = this.GetAllFromulas(fromRow, fromColumn, rowCount, columnCount);
            this.Clear(fromRow, fromColumn, rowCount, columnCount);
            this.Clear(toRow, toColumn, rowCount, columnCount);
            MoveVisitor visitor = new MoveVisitor(new CellRange(fromRow, fromColumn, rowCount, columnCount), toRow - fromRow, toColumn - fromColumn, this._sheetRowCount, this._sheetColumnCount, false, null, false, null, false);
            using (Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    foreach (ConditionalGraph.ConditionalIdentity identity in enumerator.Current)
                    {
                        CalcExpression expr = visitor.Visit(identity.Expression, toRow, toColumn);
                        ((IFormulaOperatorSource) this).SetExpression(identity, expr);
                    }
                }
            }
        }

        public void Move(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            DataValidationConditionalGraph graph = null;
            Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>> dictionary = this.GetAllFromulas(fromRow, fromColumn, rowCount, columnCount);
            this.Clear(fromRow, fromColumn, rowCount, columnCount);
            graph.Clear(toRow, toColumn, rowCount, columnCount);
            MoveVisitor visitor = new MoveVisitor(new CellRange(fromRow, fromColumn, rowCount, columnCount), toRow - fromRow, toColumn - fromColumn, graph._sheetRowCount, graph._sheetColumnCount, false, null, false, null, false);
            using (Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    foreach (ConditionalGraph.ConditionalIdentity identity in enumerator.Current)
                    {
                        CalcExpression expr = visitor.Visit(identity.Expression, toRow, toColumn);
                        ((IFormulaOperatorSource) graph).SetExpression(identity, expr);
                    }
                }
            }
        }

        public void OffsetAndAddFormulasWhileCopy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount, Dictionary<Tuple<int, int>, DataValidator> copiedValidators)
        {
            rowCount = (rowCount <= 0) ? 0 : rowCount;
            columnCount = (columnCount <= 0) ? 0 : columnCount;
            int num = toRow - fromRow;
            foreach (KeyValuePair<Tuple<int, int>, DataValidator> pair in copiedValidators)
            {
                int rowOffset = num + (((pair.Key.Item1 - toRow) / rowCount) * rowCount);
                int columnOffset = num + (((pair.Key.Item2 - toColumn) / columnCount) * columnCount);
                MoveVisitor visitor = new MoveVisitor(new CellRange(fromRow, fromColumn, rowCount, columnCount), rowOffset, columnOffset, this._sheetRowCount, this._sheetColumnCount, false, null, false, null, true);
                IConditionalFormula[] formulaConditions = pair.Value.FormulaConditions;
                if ((formulaConditions != null) && (formulaConditions.Length != 0))
                {
                    foreach (IConditionalFormula formula in formulaConditions)
                    {
                        CalcExpression expr = base.Formula2Expression(formula.Formula, toRow, toColumn);
                        expr = visitor.Visit(expr, toRow, toColumn);
                        formula.Formula = base.Expression2Formula(expr, toRow, toColumn);
                    }
                    int row = pair.Key.Item1;
                    int column = pair.Key.Item2;
                    this.AddDataValidation(row, column, pair.Value);
                }
            }
        }

        public void OffsetFormulasWhileCopy(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount, Dictionary<Tuple<int, int>, DataValidator> copiedValidators)
        {
            DataValidationConditionalGraph graph = null;
            rowCount = (rowCount <= 0) ? 0 : rowCount;
            columnCount = (columnCount <= 0) ? 0 : columnCount;
            int num = toRow - fromRow;
            foreach (KeyValuePair<Tuple<int, int>, DataValidator> pair in copiedValidators)
            {
                int rowOffset = num + (((pair.Key.Item1 - toRow) / rowCount) * rowCount);
                int columnOffset = num + (((pair.Key.Item2 - toColumn) / columnCount) * columnCount);
                MoveVisitor visitor = new MoveVisitor(new CellRange(fromRow, fromColumn, rowCount, columnCount), rowOffset, columnOffset, graph._sheetRowCount, graph._sheetColumnCount, false, null, false, null, true);
                IConditionalFormula[] formulaConditions = pair.Value.FormulaConditions;
                if ((formulaConditions != null) && (formulaConditions.Length != 0))
                {
                    foreach (IConditionalFormula formula in formulaConditions)
                    {
                        CalcExpression expr = graph.Formula2Expression(formula.Formula, toRow, toColumn);
                        expr = visitor.Visit(expr, toRow, toColumn);
                        formula.Formula = graph.Expression2Formula(expr, toRow, toColumn);
                    }
                }
            }
        }

        protected override void OnInvalidate(List<CalcRangeIdentity> ranges, List<CalcCellIdentity> cells)
        {
        }

        public void RemoveColumns(int column, int count)
        {
            base.Remove(column, count, false, false);
        }

        public void RemoveDataValidation(int row, int column)
        {
            CalcLocalIdentity actualId = FormulaOperatorHelper.Indexs2Identity(row, column, row, column);
            base.RemoveConditionals(actualId, null);
        }

        public void RemoveRows(int row, int count)
        {
            base.Remove(row, count, true, false);
        }

        public void Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>> dictionary = this.GetAllFromulas(fromRow, fromColumn, rowCount, columnCount);
            Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>> dictionary2 = this.GetAllFromulas(toRow, toColumn, rowCount, columnCount);
            int rowOffset = toRow - fromRow;
            int columnOffset = toColumn - fromColumn;
            this.Clear(fromRow, fromColumn, rowCount, columnCount);
            this.Clear(toRow, toColumn, rowCount, columnCount);
            MoveVisitor visitor = new MoveVisitor(new CellRange(fromRow, fromColumn, rowCount, columnCount), rowOffset, columnOffset, this._sheetRowCount, this._sheetColumnCount, false, null, false, null, false);
            using (Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    foreach (ConditionalGraph.ConditionalIdentity identity in enumerator.Current)
                    {
                        CalcExpression expr = visitor.Visit(identity.Expression, toRow, toColumn);
                        ((IFormulaOperatorSource) this).SetExpression(identity, expr);
                    }
                }
            }
            visitor = new MoveVisitor(new CellRange(toRow, toColumn, rowCount, columnCount), -rowOffset, -columnOffset, this._sheetRowCount, this._sheetColumnCount, false, null, false, null, true);
            using (Dictionary<CalcLocalIdentity, List<ConditionalGraph.ConditionalIdentity>>.ValueCollection.Enumerator enumerator3 = dictionary2.Values.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    foreach (ConditionalGraph.ConditionalIdentity identity2 in enumerator3.Current)
                    {
                        CalcExpression expression2 = visitor.Visit(identity2.Expression, fromRow, fromColumn);
                        ((IFormulaOperatorSource) this).SetExpression(identity2, expression2);
                    }
                }
            }
        }

        protected override IMultiSourceProvider MultiSourceProvider
        {
            get { return  null; }
        }

        protected override Dt.Cells.Data.ReferenceStyle ReferenceStyle
        {
            get { return  this._referenceStyle; }
        }

        protected override ICalcSource Source
        {
            get { return  this._source; }
        }
    }
}

