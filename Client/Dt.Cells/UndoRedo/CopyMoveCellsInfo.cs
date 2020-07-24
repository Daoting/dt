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
        object[,] _arrayFormulas;
        bool _arrayFormulaSaved;
        int _columnCount;
        List<CellData> _formulas = new List<CellData>();
        bool _formulaSaved;
        int _rowCount;
        List<CellRange> _spans;
        bool _spanSaved;
        List<CellData> _sparklines = new List<CellData>();
        bool _sparklineSaved;
        List<CellData> _styles = new List<CellData>();
        bool _styleSaved;
        List<CellData> _tags = new List<CellData>();
        bool _tagSaved;
        List<CellData> _values = new List<CellData>();
        bool _valueSaved;

        public CopyMoveCellsInfo(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;
        }

        public object[,] GetArrayFormula()
        {
            return _arrayFormulas;
        }

        public List<CellData> GetFormulas()
        {
            return _formulas;
        }

        public List<CellData> GetSparklines()
        {
            return _sparklines;
        }

        public List<CellData> GetStyles()
        {
            return _styles;
        }

        public List<CellData> GetTags()
        {
            return _tags;
        }

        public List<CellData> GetValues()
        {
            return _values;
        }

        public bool IsArrayFormulaSaved()
        {
            return _arrayFormulaSaved;
        }

        public bool IsFormulaSaved()
        {
            return _formulaSaved;
        }

        public bool IsSpanSaved()
        {
            return _spanSaved;
        }

        public bool IsSparklineSaved()
        {
            return _sparklineSaved;
        }

        public bool IsStyleSaved()
        {
            return _styleSaved;
        }

        public bool IsTagSaved()
        {
            return _tagSaved;
        }

        public bool IsValueSaved()
        {
            return _valueSaved;
        }

        public void SaveArrayFormula(object[,] arrayFormulas)
        {
            _arrayFormulas = arrayFormulas;
            _arrayFormulaSaved = true;
        }

        public void SaveFormula(int row, int column, string formula)
        {
            if (!string.IsNullOrEmpty(formula))
            {
                _formulas.Add(new CellData(row, column, formula));
            }
            _formulaSaved = true;
        }

        public void SaveSpan(CellRange span)
        {
            if (_spans == null)
            {
                _spans = new List<CellRange>();
            }
            _spans.Add(span);
            _spanSaved = true;
        }

        public void SaveSparkline(int row, int column, SparklineInfo sparkline)
        {
            if (sparkline != null)
            {
                _sparklines.Add(new CellData(row, column, sparkline));
            }
            _sparklineSaved = true;
        }

        public void SaveStyle(int row, int column, object style)
        {
            if (style != null)
            {
                _styles.Add(new CellData(row, column, style));
            }
            _styleSaved = true;
        }

        public void SaveTag(int row, int column, object tag)
        {
            if (tag != null)
            {
                _tags.Add(new CellData(row, column, tag));
            }
            _tagSaved = true;
        }

        public void SaveValue(int row, int column, object value)
        {
            if (value != null)
            {
                _values.Add(new CellData(row, column, value));
            }
            _valueSaved = true;
        }

        public int ColumnCount
        {
            get { return  _columnCount; }
        }

        public int RowCount
        {
            get { return  _rowCount; }
        }

        public List<CellRange> Spans
        {
            get { return  _spans; }
        }
    }
}

