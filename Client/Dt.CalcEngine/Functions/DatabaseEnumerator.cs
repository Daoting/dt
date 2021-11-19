#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Operators;
using System;
using System.Collections;
using System.Globalization;
#endregion

namespace Dt.CalcEngine.Functions
{
    internal class DatabaseEnumerator : IEnumerator
    {
        private int column;
        private CalcArray criteria;
        private CalcArray database;
        private static CalcError[] errors = new CalcError[] { CalcErrors.DivideByZero, CalcErrors.NotAvailable, CalcErrors.Name, CalcErrors.Null, CalcErrors.Number, CalcErrors.Reference, CalcErrors.Value };
        private static CalcBinaryOperator[] operators = new CalcBinaryOperator[] { CalcBinaryOperators.Equal, CalcBinaryOperators.NotEqual, CalcBinaryOperators.LessThanOrEqual, CalcBinaryOperators.GreaterThanOrEqual, CalcBinaryOperators.LessThan, CalcBinaryOperators.GreaterThan };
        private int row;

        public DatabaseEnumerator(CalcArray database, CalcArray criteria)
        {
            this.database = database;
            this.criteria = criteria;
            this.row = 0;
            this.column = -1;
            if ((database.RowCount < 2) || (database.ColumnCount < 1))
            {
                throw new InvalidCastException();
            }
            if ((criteria.RowCount < 2) || (criteria.ColumnCount < 1))
            {
                throw new InvalidCastException();
            }
        }

        public DatabaseEnumerator(CalcArray database, object field, CalcArray criteria)
        {
            this.database = database;
            this.criteria = criteria;
            this.row = 0;
            this.column = this.ColumnIndex(database, field);
            if ((database.RowCount < 2) || (database.ColumnCount < 1))
            {
                throw new InvalidCastException();
            }
            if ((criteria.RowCount < 2) || (criteria.ColumnCount < 1))
            {
                throw new InvalidCastException();
            }
            if ((this.column < 0) || (database.ColumnCount <= this.column))
            {
                throw new InvalidCastException();
            }
        }

        private int ColumnIndex(CalcArray database, object field)
        {
            if (!(field is string))
            {
                return (CalcConvert.ToInt(field) - 1);
            }
            for (int i = 0; i < database.ColumnCount; i++)
            {
                string str = CalcConvert.ToString(database.GetValue(0, i));
                if ((str != null) && str.Equals((string) (field as string), StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        /// <remarks>
        /// In Excel, a string criteria with out a leading operator is treated as a
        /// string begins with operation.  In OpenOffice, a string criteria with out
        /// a leading operator is treated as a string equals operation.  We are
        /// supporting the OpenOffice behavior.
        /// 
        /// In Excel and OpenOffice, a string equals criteria supports regular
        /// expressions.  We are not supporting regular expressions.
        /// </remarks>
        private CriteriaEvaluator CreateEvaluator(object criteria)
        {
            if (criteria is string)
            {
                string s = (string) ((string) criteria);
                double result = 0.0;
                for (int i = 0; i < operators.Length; i++)
                {
                    if (s.StartsWith(operators[i].Name))
                    {
                        s = s.Substring(operators[i].Name.Length);
                        if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                        {
                            criteria = (double) result;
                        }
                        else if ("true".Equals(s, StringComparison.OrdinalIgnoreCase))
                        {
                            criteria = true;
                        }
                        else if ("false".Equals(s, StringComparison.OrdinalIgnoreCase))
                        {
                            criteria = false;
                        }
                        else
                        {
                            criteria = s;
                        }
                        return new CriteriaEvaluator(operators[i], criteria);
                    }
                }
                return new CriteriaEvaluator(CalcBinaryOperators.Equal, criteria);
            }
            if (criteria != null)
            {
                return new CriteriaEvaluator(CalcBinaryOperators.Equal, criteria);
            }
            return null;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            bool flag = false;
            while (!flag && (this.row < this.database.RowCount))
            {
                this.row++;
                if (this.row < this.database.RowCount)
                {
                    flag = this.RowMeetsCriteria();
                }
            }
            return flag;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            this.row = 0;
        }

        /// <summary>
        /// Determines if the current row meets the specified criteria.
        /// </summary>
        private bool RowMeetsCriteria()
        {
            bool flag = false;
            for (int i = 1; !flag && (i < this.criteria.RowCount); i++)
            {
                flag = true;
                for (int j = 0; flag && (j < this.criteria.ColumnCount); j++)
                {
                    CriteriaEvaluator evaluator = this.CreateEvaluator(this.criteria.GetValue(i, j));
                    if (evaluator != null)
                    {
                        int column = this.ColumnIndex(this.database, this.criteria.GetValue(0, j));
                        object databaseValue = this.database.GetValue(this.row, column);
                        flag = evaluator.Evaluate(databaseValue);
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public object Current
        {
            get
            {
                if ((this.row <= 0) || (this.database.RowCount <= this.row))
                {
                    throw new InvalidOperationException();
                }
                return this.database.GetValue(this.row, this.column);
            }
        }
    }
}

