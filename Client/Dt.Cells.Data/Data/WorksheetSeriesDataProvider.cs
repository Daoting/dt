#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
using System.ComponentModel;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    internal abstract class WorksheetSeriesDataProvider : ISeriesDataProvider, IDisposable
    {
        object[,] _cachedData;
        string[,] _cachedText;
        CellRange _cachRange;
        Dt.Cells.Data.Worksheet _worksheet;

        public event EventHandler DataChanged;

        protected WorksheetSeriesDataProvider()
        {
        }

        void AttachEvents()
        {
            if (this.Worksheet != null)
            {
                this.Worksheet.CellChanged += new EventHandler<CellChangedEventArgs>(this.Worksheet_CellChanged);
                this.Worksheet.RowChanged += new EventHandler<SheetChangedEventArgs>(this.Worksheet_RowChanged);
                this.Worksheet.ColumnChanged += new EventHandler<SheetChangedEventArgs>(this.Worksheet_ColumnChanged);
                this.Worksheet.PropertyChanged += new PropertyChangedEventHandler(this.Worksheet_PropertyChanged);
            }
        }

        internal void ClearCache()
        {
            this._cachRange = null;
            this._cachedData = null;
        }

        void DetachEvents()
        {
            if (this.Worksheet != null)
            {
                this.Worksheet.CellChanged -= new EventHandler<CellChangedEventArgs>(this.Worksheet_CellChanged);
                this.Worksheet.RowChanged -= new EventHandler<SheetChangedEventArgs>(this.Worksheet_RowChanged);
                this.Worksheet.ColumnChanged -= new EventHandler<SheetChangedEventArgs>(this.Worksheet_ColumnChanged);
                this.Worksheet.PropertyChanged -= new PropertyChangedEventHandler(this.Worksheet_PropertyChanged);
            }
        }

        public void Dispose()
        {
            this.DetachEvents();
        }

        public IFormatter GetFormatter(int valueIndex)
        {
            return this.GetFormatter(0, valueIndex);
        }

        public IFormatter GetFormatter(int seriesIndex, int valueIndex)
        {
            if (this._cachRange == null)
            {
                this.UpdateDataRange();
            }
            if (this.IsValueVisible(valueIndex) && (this._cachRange != null))
            {
                switch (this.DataOrientation)
                {
                    case Dt.Cells.Data.DataOrientation.Vertical:
                        return this._worksheet.GetActualFormatter(this._cachRange.Row + valueIndex, this._cachRange.Column + seriesIndex, SheetArea.Cells);

                    case Dt.Cells.Data.DataOrientation.Horizontal:
                        return this._worksheet.GetActualFormatter(this._cachRange.Row + seriesIndex, this._cachRange.Column + valueIndex, SheetArea.Cells);
                }
            }
            return null;
        }

        public string GetText(int valueIndex)
        {
            return this.GetText(0, valueIndex);
        }

        public string GetText(int seriesIndex, int valueIndex)
        {
            if (this.Worksheet != null)
            {
                if (this._cachedText == null)
                {
                    this.UpdateCacheText();
                }
                if (this._cachedText != null)
                {
                    switch (this.DataOrientation)
                    {
                        case Dt.Cells.Data.DataOrientation.Vertical:
                            return this._cachedText[seriesIndex, valueIndex];

                        case Dt.Cells.Data.DataOrientation.Horizontal:
                            return this._cachedText[valueIndex, seriesIndex];
                    }
                }
            }
            return null;
        }

        public object GetValue(int valueIndex)
        {
            return this.GetValue(0, valueIndex);
        }

        public object GetValue(int seriesIndex, int valueIndex)
        {
            if (this.Worksheet != null)
            {
                if (this._cachedData == null)
                {
                    this.UpdateCacheData();
                }
                if (this._cachedData != null)
                {
                    switch (this.DataOrientation)
                    {
                        case Dt.Cells.Data.DataOrientation.Vertical:
                            return this._cachedData[seriesIndex, valueIndex];

                        case Dt.Cells.Data.DataOrientation.Horizontal:
                            return this._cachedData[valueIndex, seriesIndex];
                    }
                }
            }
            return null;
        }

        public bool IsSeriesVisible(int seriesIndex)
        {
            if (this.Worksheet != null)
            {
                if (this._cachRange == null)
                {
                    this.UpdateDataRange();
                }
                switch (this.DataOrientation)
                {
                    case Dt.Cells.Data.DataOrientation.Vertical:
                        return this.Worksheet.GetActualColumnVisible(this._cachRange.Column + seriesIndex, SheetArea.Cells);

                    case Dt.Cells.Data.DataOrientation.Horizontal:
                        return this.Worksheet.GetActualRowVisible(this._cachRange.Row + seriesIndex, SheetArea.Cells);
                }
            }
            return true;
        }

        public bool IsValueVisible(int valueIndex)
        {
            return this.IsValueVisible(0, valueIndex);
        }

        public bool IsValueVisible(int seriesIndex, int valueIndex)
        {
            if (this.Worksheet != null)
            {
                if (this._cachRange == null)
                {
                    this.UpdateDataRange();
                }
                switch (this.DataOrientation)
                {
                    case Dt.Cells.Data.DataOrientation.Vertical:
                        if (!this.Worksheet.GetActualColumnVisible(this._cachRange.Column + seriesIndex, SheetArea.Cells))
                        {
                            return false;
                        }
                        return this.Worksheet.GetActualRowVisible(this._cachRange.Row + valueIndex, SheetArea.Cells);

                    case Dt.Cells.Data.DataOrientation.Horizontal:
                        if (!this.Worksheet.GetActualRowVisible(this._cachRange.Row + seriesIndex, SheetArea.Cells))
                        {
                            return false;
                        }
                        return this.Worksheet.GetActualColumnVisible(this._cachRange.Column + valueIndex, SheetArea.Cells);
                }
            }
            return true;
        }

        void OnDataChanged()
        {
            if (this.DataChanged != null)
            {
                this.DataChanged(this, EventArgs.Empty);
            }
        }

        void UpdateCacheData()
        {
            if ((this.Worksheet == null) || (this.DataReference == null))
            {
                this._cachedData = null;
            }
            else
            {
                ICalcEvaluator worksheet = this.Worksheet;
                if (worksheet == null)
                {
                    this._cachedData = null;
                }
                else
                {
                    this._cachedData = worksheet.EvaluateExpression(this.DataReference, 0, 0, 0, 0, true) as object[,];
                }
            }
        }

        void UpdateCacheText()
        {
            if (this._cachedData == null)
            {
                this.UpdateCacheData();
            }
            if (this._cachedData != null)
            {
                int length = this._cachedData.GetLength(1);
                int num2 = this._cachedData.GetLength(0);
                this._cachedText = new string[num2, length];
                for (int i = 0; i < num2; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        object obj2 = this._cachedData[i, j];
                        if (obj2 != null)
                        {
                            IFormatter formatter = this.Worksheet.GetActualFormatter(j + this._cachRange.Row, i + this._cachRange.Column, SheetArea.Cells);
                            this._cachedText[i, j] = this.Worksheet.Value2Text(obj2, formatter);
                        }
                        else
                        {
                            this._cachedText[i, j] = string.Empty;
                        }
                    }
                }
            }
        }

        void UpdateDataRange()
        {
            this._cachRange = new CellRange(0, 0, 0, 0);
            CalcExternalRangeExpression dataReference = this.DataReference as CalcExternalRangeExpression;
            if (dataReference != null)
            {
                this._cachRange = new CellRange(dataReference.StartRow, dataReference.StartColumn, (dataReference.EndRow - dataReference.StartRow) + 1, (dataReference.EndColumn - dataReference.StartColumn) + 1);
            }
            CalcRangeExpression expression2 = this.DataReference as CalcRangeExpression;
            if (expression2 != null)
            {
                this._cachRange = new CellRange(expression2.StartRow, expression2.StartColumn, (expression2.EndRow - expression2.StartRow) + 1, (expression2.EndColumn - expression2.StartColumn) + 1);
            }
            CalcExternalCellExpression expression3 = this.DataReference as CalcExternalCellExpression;
            if (expression3 != null)
            {
                this._cachRange = new CellRange(expression3.Row, expression3.Column, 1, 1);
            }
            CalcCellExpression expression4 = this.DataReference as CalcCellExpression;
            if (expression4 != null)
            {
                this._cachRange = new CellRange(expression4.Row, expression4.Column, 1, 1);
            }
        }

        void Worksheet_CellChanged(object sender, CellChangedEventArgs e)
        {
            if (((this._cachRange != null) && (e.PropertyName == "Value")) && (this._cachRange != null))
            {
                CellRange range = new CellRange(e.Row, e.Column, e.RowCount, e.ColumnCount);
                if (range.Intersects(this._cachRange.Row, this._cachRange.Column, this._cachRange.RowCount, this._cachRange.ColumnCount))
                {
                    this.ClearCache();
                    this.OnDataChanged();
                }
            }
        }

        void Worksheet_ColumnChanged(object sender, SheetChangedEventArgs e)
        {
            if (this._cachRange != null)
            {
                CellRange range = new CellRange(-1, this._cachRange.Column, -1, this._cachRange.ColumnCount);
                if (range.Intersects(e.Row, e.Column, e.RowCount, e.ColumnCount) && (e.PropertyName == "IsVisible"))
                {
                    this.ClearCache();
                    this.OnDataChanged();
                }
            }
        }

        void Worksheet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this._cachRange != null)
            {
                if (e.PropertyName == "RowRangeGroup")
                {
                    this.ClearCache();
                    this.OnDataChanged();
                }
                else if (e.PropertyName == "ColumnRangeGroup")
                {
                    this.ClearCache();
                    this.OnDataChanged();
                }
                else if (e.PropertyName == "RowFilter")
                {
                    this.ClearCache();
                    this.OnDataChanged();
                }
                else if (e.PropertyName == "TableFilter")
                {
                    this.ClearCache();
                    this.OnDataChanged();
                }
            }
        }

        void Worksheet_RowChanged(object sender, SheetChangedEventArgs e)
        {
            if (this._cachRange != null)
            {
                CellRange range = new CellRange(this._cachRange.Row, -1, this._cachRange.RowCount, -1);
                if (range.Intersects(e.Row, e.Column, e.RowCount, e.ColumnCount) && (e.PropertyName == "IsVisible"))
                {
                    this.ClearCache();
                    this.OnDataChanged();
                }
            }
        }

        public abstract Dt.Cells.Data.DataOrientation DataOrientation { get; }

        public abstract CalcExpression DataReference { get; }

        public int SeriesCount
        {
            get
            {
                if (this._cachRange == null)
                {
                    this.UpdateDataRange();
                }
                if (this._cachRange == null)
                {
                    return 0;
                }
                if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
                {
                    return this._cachRange.ColumnCount;
                }
                return this._cachRange.RowCount;
            }
        }

        public int ValuesCount
        {
            get
            {
                if (this._cachRange == null)
                {
                    this.UpdateDataRange();
                }
                if (this._cachRange == null)
                {
                    return 0;
                }
                if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
                {
                    return this._cachRange.RowCount;
                }
                return this._cachRange.ColumnCount;
            }
        }

        internal Dt.Cells.Data.Worksheet Worksheet
        {
            get { return  this._worksheet; }
            set
            {
                if (value != this.Worksheet)
                {
                    this.DetachEvents();
                    this._worksheet = value;
                    this.AttachEvents();
                }
            }
        }
    }
}

