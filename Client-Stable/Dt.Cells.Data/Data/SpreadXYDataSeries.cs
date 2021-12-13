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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents data series with X- and Y-values.
    /// </summary>
    /// <remarks>
    /// The XYDataSeries class has two sets of data values (for x-coordinates and for y-coordinates).
    /// </remarks>
    public class SpreadXYDataSeries : SpreadDataSeries
    {
        DataOrientation? _xDataOrientation = 0;
        IFormatter _xFormatter;
        string _xValueFormula;
        SheetCellRange[] _xValueRange;
        CalcExpression _xValueReference;
        DoubleSeriesCollection _xValues;

        internal override DataLabel CreateDataLabelOnNeeded(int pointIndex)
        {
            return new XYDataLabel(this, pointIndex);
        }

        internal override DataMarker CreateDataMarkerOnNeeded(int pointIndex)
        {
            return new XYDataMarker(this, pointIndex);
        }

        internal override DataPoint CreateDataPointOnNeeded(int pointIndex)
        {
            return new XYDataPoint(this, pointIndex);
        }

        internal override List<List<SheetCellRange>> GetDataRanges()
        {
            List<List<SheetCellRange>> dataRanges = base.GetDataRanges();
            if ((this.XValuesRange != null) && (this.XValuesRange.Length > 0))
            {
                dataRanges.Insert(0, new List<SheetCellRange>(this.XValuesRange));
            }
            return dataRanges;
        }

        internal override void Init()
        {
            base.Init();
            this._xValues = new DoubleSeriesCollection();
            this._xValues.CollectionChanged += new NotifyCollectionChangedEventHandler(this.XValues_CollectionChanged);
            this._xDataOrientation = 0;
            this._xFormatter = null;
        }

        internal override void OnAddColumnRange(int column, int columnCount)
        {
            base.OnAddColumnRange(column, columnCount);
            this.XValueFormula = FormulaUtility.AddColumnRange(base.Sheet, this.XValueFormula, column, columnCount);
        }

        internal override void OnAddRowRange(int row, int rowCount)
        {
            base.OnAddRowRange(row, rowCount);
            this.XValueFormula = FormulaUtility.AddRowRange(base.Sheet, this.XValueFormula, row, rowCount);
        }

        internal override void OnDisposed()
        {
            base.OnDisposed();
            this.XValues.Dispose();
        }

        internal override void OnRemoveColumnRange(int column, int columnCount)
        {
            base.OnRemoveColumnRange(column, columnCount);
            this.XValueFormula = FormulaUtility.RemoveColumnRange(base.Sheet, this.XValueFormula, column, columnCount);
        }

        internal override void OnRemoveRowRange(int row, int rowCount)
        {
            base.OnRemoveRowRange(row, rowCount);
            this.XValueFormula = FormulaUtility.RemoveRowRange(base.Sheet, this.XValueFormula, row, rowCount);
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "XValueFormula")
                {
                    if (str != "XValues")
                    {
                        if (str == "Formatter")
                        {
                            this._xFormatter = Serializer.DeserializeObj(typeof(IFormatter), reader) as IFormatter;
                        }
                        return;
                    }
                }
                else
                {
                    this._xValueFormula = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                    return;
                }
                Serializer.DeserializeList(this._xValues, reader);
            }
        }

        internal override void RefreshValues()
        {
            base.RefreshValues();
            if ((this.XValues != null) && (this.XValues.DataSeries != null))
            {
                this.XValues.RefreshData();
            }
        }

        internal override void UpdateReferences()
        {
            base.UpdateReferences();
            this.UpdateXValuesReference(this.XValueFormula);
        }

        void UpdateXValuesReference(string xValueFormula)
        {
            this.XValuesReference = FormulaUtility.Formula2Expression(base.Sheet, xValueFormula);
        }

        DataOrientation? ValidateXValuesRange(SheetCellRange[] ranges)
        {
            if ((ranges == null) || (ranges.Length == 0))
            {
                return 0;
            }
            SheetCellRange range = ranges[0];
            Worksheet sheet = range.Sheet;
            DataOrientation? nullable = new DataOrientation?((range.ColumnCount > range.RowCount) ? DataOrientation.Horizontal : DataOrientation.Vertical);
            for (int i = 1; i < ranges.Length; i++)
            {
                SheetCellRange range2 = ranges[i];
                if (range2.Sheet != sheet)
                {
                    throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
                }
                if (nullable.HasValue)
                {
                    DataOrientation orientation2 = (range2.ColumnCount > range2.RowCount) ? DataOrientation.Horizontal : DataOrientation.Vertical;
                    DataOrientation? nullable2 = nullable;
                    if ((orientation2 != ((DataOrientation) nullable2.GetValueOrDefault())) || !nullable2.HasValue)
                    {
                        nullable = null;
                    }
                }
            }
            return nullable;
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (!string.IsNullOrEmpty(this._xValueFormula))
            {
                Serializer.SerializeObj(this._xValueFormula, "XValueFormula", writer);
            }
            else if ((this._xValues != null) && (this._xValues.Count > 0))
            {
                Serializer.SerializeList(this._xValues, "XValues", writer);
            }
            if (this._xFormatter != null)
            {
                Serializer.SerializeObj(this._xFormatter, "Formatter", writer);
            }
        }

        void XValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ((ISpreadChartElement) this).NotifyElementChanged("XValues");
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hidden; otherwise, <c>false</c>.
        /// </value>
        public override bool IsHidden
        {
            get { return  ((base.IsHidden && (this._xValues != null)) && this._xValues.AreAllDataSeriesInVisible); }
        }

        /// <summary>
        /// Gets or sets the X formatter.
        /// </summary>
        /// <value>
        /// The X formatter.
        /// </value>
        internal IFormatter XValueFormatter
        {
            get { return  this._xFormatter; }
            set
            {
                if (value != this._xFormatter)
                {
                    this._xFormatter = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the X values formula.
        /// </summary>
        /// <value>
        /// The X values formula.
        /// </value>
        public string XValueFormula
        {
            get { return  this._xValueFormula; }
            set
            {
                if (value != this.XValueFormula)
                {
                    this.UpdateXValuesReference(value);
                    this._xValueFormula = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the X values.
        /// </summary>
        /// <value>
        /// The X values.
        /// </value>
        public DoubleSeriesCollection XValues
        {
            get { return  this._xValues; }
            set
            {
                if (value != this.XValues)
                {
                    if (this._xValues != null)
                    {
                        this._xValues.DataSeries = null;
                        this._xValues.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.XValues_CollectionChanged);
                    }
                    this._xValues = value;
                    this._xValueFormula = null;
                    if (this._xValues != null)
                    {
                        this._xValues.CollectionChanged += new NotifyCollectionChangedEventHandler(this.XValues_CollectionChanged);
                    }
                    base.NotifyChartChanged("XValues");
                }
            }
        }

        internal SheetCellRange[] XValuesRange
        {
            get { return  this._xValueRange; }
            set
            {
                DataOrientation? nullable = this.ValidateXValuesRange(value);
                this._xValueRange = value;
                this._xDataOrientation = new DataOrientation?(nullable.Value);
            }
        }

        CalcExpression XValuesReference
        {
            get { return  this._xValueReference; }
            set
            {
                if (value != this.XValuesReference)
                {
                    this._xValueReference = value;
                    if (this._xValueReference != null)
                    {
                        this.XValuesRange = SheetCellRangeUtility.ExtractAllExternalReference(base.Sheet, value);
                    }
                    else
                    {
                        this.XValuesRange = null;
                    }
                    if (this.XValues.DataSeries != null)
                    {
                        this.XValues.RefreshData();
                    }
                    else
                    {
                        this.XValues.DataSeries = new XValueDataSeries(this);
                    }
                    base.NotifyChartChanged("XValues");
                }
            }
        }

        class XValueDataSeries : IDataSeries
        {
            SpreadXYDataSeries _xyDataSeries;

            public XValueDataSeries(SpreadXYDataSeries xyDataSeries)
            {
                this._xyDataSeries = xyDataSeries;
            }

            public Dt.Cells.Data.DataOrientation? DataOrientation
            {
                get { return  this._xyDataSeries._xDataOrientation; }
            }

            public CalcExpression DataReference
            {
                get { return  this._xyDataSeries.XValuesReference; }
            }

            public bool DisplayHiddenData
            {
                get { return  this._xyDataSeries.DisplayHidden; }
            }

            public Dt.Cells.Data.EmptyValueStyle EmptyValueStyle
            {
                get { return  this._xyDataSeries.EmptyValueStyle; }
            }

            public ICalcEvaluator Evaluator
            {
                get { return  this._xyDataSeries.Sheet; }
            }
        }
    }
}

