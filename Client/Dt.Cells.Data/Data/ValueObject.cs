#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class ValueObject : ICloneable
    {
        int columnOffset;
        bool expectArrayValue;
        object expression;
        string formula;
        bool isFormula;
        int rowOffset;
        object value;

        ValueObject()
        {
        }

        public object Clone()
        {
            return new ValueObject { isFormula = this.isFormula, value = this.value, formula = this.formula, expression = this.expression, rowOffset = this.rowOffset, columnOffset = this.columnOffset, expectArrayValue = this.expectArrayValue };
        }

        public static ValueObject FromExpression(object expression)
        {
            return new ValueObject { expression = expression, isFormula = true };
        }

        public static ValueObject FromFormula(string formula)
        {
            return new ValueObject { formula = formula, isFormula = true };
        }

        public static ValueObject FromValue(object value)
        {
            return new ValueObject { value = value, isFormula = false };
        }

        public object GetValue(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            return this.GetValue(evaluator, baseRow, baseColumn, false);
        }

        public object GetValue(ICalcEvaluator evaluator, int baseRow, int baseColumn, bool isArrayFormula)
        {
            if (!this.isFormula)
            {
                return this.value;
            }
            this.InitializeFormula(evaluator, baseRow, baseColumn, false);
            object obj2 = evaluator.EvaluateExpression(this.expression, this.rowOffset, this.columnOffset, baseRow, baseColumn, isArrayFormula);
            if (this.ExpectArrayValue || !(obj2 is Array))
            {
                return obj2;
            }
            Array array = obj2 as Array;
            if (array.Rank != 2)
            {
                return obj2;
            }
            int length = array.GetLength(0);
            int num2 = array.GetLength(1);
            if ((length == 1) && (num2 == 1))
            {
                return array.GetValue(new int[2]);
            }
            if ((this.columnOffset < length) && (this.rowOffset < num2))
            {
                return array.GetValue(new int[] { this.columnOffset, this.rowOffset });
            }
            return CalcErrors.NotAvailable;
        }

        internal void InitializeFormula(ICalcEvaluator evaluator, int baseRow, int baseColumn, bool forceReparse)
        {
            if (((this.isFormula && !string.IsNullOrEmpty(this.formula)) && (evaluator != null)) && (forceReparse || (this.expression == null)))
            {
                this.expression = evaluator.Formula2Expression(this.formula, baseRow - this.rowOffset, baseColumn - this.columnOffset);
            }
        }

        public int ColumnOffset
        {
            get { return  this.columnOffset; }
            set { this.columnOffset = value; }
        }

        public bool ExpectArrayValue
        {
            get { return  this.expectArrayValue; }
            set { this.expectArrayValue = value; }
        }

        public object Expression
        {
            get { return  this.expression; }
        }

        public string Formula
        {
            get { return  this.formula; }
        }

        public bool IsFormula
        {
            get { return  this.isFormula; }
        }

        public int RowOffset
        {
            get { return  this.rowOffset; }
            set { this.rowOffset = value; }
        }

        public object Value
        {
            get { return  this.value; }
        }
    }
}

