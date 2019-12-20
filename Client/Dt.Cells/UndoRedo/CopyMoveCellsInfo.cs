#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.UndoRedo
{
    internal class CopyMoveCellsInfo
    {
        private object[,] _arrayFormulas;
        private bool _arrayFormulaSaved;
        private int _columnCount;
        private List<CellData> _formulas = new List<CellData>();
        private bool _formulaSaved;
        private int _rowCount;
        private List<CellRange> _spans;
        private bool _spanSaved;
        private List<CellData> _sparklines = new List<CellData>();
        private bool _sparklineSaved;
        private List<CellData> _styles = new List<CellData>();
        private bool _styleSaved;
        private List<CellData> _tags = new List<CellData>();
        private bool _tagSaved;
        private List<CellData> _values = new List<CellData>();
        private bool _valueSaved;

        public CopyMoveCellsInfo(int rowCount, int columnCount)
        {
            this._rowCount = rowCount;
            this._columnCount = columnCount;
        }

        public object[,] GetArrayFormula()
        {
            return this._arrayFormulas;
        }

        public List<CellData> GetFormulas()
        {
            return this._formulas;
        }

        public List<CellData> GetSparklines()
        {
            return this._sparklines;
        }

        public List<CellData> GetStyles()
        {
            return this._styles;
        }

        public List<CellData> GetTags()
        {
            return this._tags;
        }

        public List<CellData> GetValues()
        {
            return this._values;
        }

        public bool IsArrayFormulaSaved()
        {
            return this._arrayFormulaSaved;
        }

        public bool IsFormulaSaved()
        {
            return this._formulaSaved;
        }

        public bool IsSpanSaved()
        {
            return this._spanSaved;
        }

        public bool IsSparklineSaved()
        {
            return this._sparklineSaved;
        }

        public bool IsStyleSaved()
        {
            return this._styleSaved;
        }

        public bool IsTagSaved()
        {
            return this._tagSaved;
        }

        public bool IsValueSaved()
        {
            return this._valueSaved;
        }

        public void SaveArrayFormula(object[,] arrayFormulas)
        {
            this._arrayFormulas = arrayFormulas;
            this._arrayFormulaSaved = true;
        }

        public void SaveFormula(int row, int column, string formula)
        {
            if (!string.IsNullOrEmpty(formula))
            {
                this._formulas.Add(new CellData(row, column, formula));
            }
            this._formulaSaved = true;
        }

        public void SaveSpan(CellRange span)
        {
            if (this._spans == null)
            {
                this._spans = new List<CellRange>();
            }
            this._spans.Add(span);
            this._spanSaved = true;
        }

        public void SaveSparkline(int row, int column, SparklineInfo sparkline)
        {
            if (sparkline != null)
            {
                this._sparklines.Add(new CellData(row, column, sparkline));
            }
            this._sparklineSaved = true;
        }

        public void SaveStyle(int row, int column, object style)
        {
            if (style != null)
            {
                this._styles.Add(new CellData(row, column, style));
            }
            this._styleSaved = true;
        }

        public void SaveTag(int row, int column, object tag)
        {
            if (tag != null)
            {
                this._tags.Add(new CellData(row, column, tag));
            }
            this._tagSaved = true;
        }

        public void SaveValue(int row, int column, object value)
        {
            if (value != null)
            {
                this._values.Add(new CellData(row, column, value));
            }
            this._valueSaved = true;
        }

        public int ColumnCount
        {
            get { return  this._columnCount; }
        }

        public int RowCount
        {
            get { return  this._rowCount; }
        }

        public List<CellRange> Spans
        {
            get { return  this._spans; }
        }
    }
}

