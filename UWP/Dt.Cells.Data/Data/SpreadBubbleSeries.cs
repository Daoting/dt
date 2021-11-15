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
    /// The BubbleSeries can contain values that specify the symbol size for each data point. Use BubbleSeries to create bubble charts.
    /// </summary>
    public class SpreadBubbleSeries : SpreadXYDataSeries
    {
        bool _bubble3D;
        DataOrientation? _sizeDataOrientation = 0;
        IFormatter _sizeFormatter;
        string _sizeValueFormula;
        SheetCellRange[] _sizeValueRange;
        CalcExpression _sizeValueReference;
        DoubleSeriesCollection _sizeValues;

        internal override DataLabel CreateDataLabelOnNeeded(int pointIndex)
        {
            return new BubbleDataLabel(this, pointIndex);
        }

        internal override DataMarker CreateDataMarkerOnNeeded(int pointIndex)
        {
            return new BubbleDataMarker(this, pointIndex);
        }

        internal override DataPoint CreateDataPointOnNeeded(int pointIndex)
        {
            return new BubbleDataPoint(this, pointIndex);
        }

        internal override List<List<SheetCellRange>> GetDataRanges()
        {
            List<List<SheetCellRange>> dataRanges = base.GetDataRanges();
            if ((this.SizeRange != null) && (this.SizeRange.Length > 0))
            {
                dataRanges.Add(new List<SheetCellRange>(this.SizeRange));
            }
            return dataRanges;
        }

        internal override void Init()
        {
            base.Init();
            this._sizeValues = new DoubleSeriesCollection();
            this._sizeDataOrientation = 0;
            this._sizeFormatter = null;
        }

        internal override void OnAddColumnRange(int column, int columnCount)
        {
            base.OnAddColumnRange(column, columnCount);
            this.SizeFormula = FormulaUtility.AddColumnRange(base.Sheet, this.SizeFormula, column, columnCount);
        }

        internal override void OnAddRowRange(int row, int rowCount)
        {
            base.OnAddRowRange(row, rowCount);
            this.SizeFormula = FormulaUtility.AddRowRange(base.Sheet, this.SizeFormula, row, rowCount);
        }

        internal override void OnDisposed()
        {
            base.OnDisposed();
            this.SizeValues.Dispose();
        }

        internal override void OnRemoveColumnRange(int column, int columnCount)
        {
            base.OnRemoveColumnRange(column, columnCount);
            this.SizeFormula = FormulaUtility.RemoveColumnRange(base.Sheet, this.SizeFormula, column, columnCount);
        }

        internal override void OnRemoveRowRange(int row, int rowCount)
        {
            base.OnRemoveRowRange(row, rowCount);
            this.SizeFormula = FormulaUtility.RemoveRowRange(base.Sheet, this.SizeFormula, row, rowCount);
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "SizeValueFormula")
                {
                    if (str != "SizeValues")
                    {
                        if (str == "Bubble3D")
                        {
                            this._bubble3D = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                            return;
                        }
                        if (str == "Formatter")
                        {
                            this._sizeFormatter = Serializer.DeserializeObj(typeof(IFormatter), reader) as IFormatter;
                        }
                        return;
                    }
                }
                else
                {
                    this._sizeValueFormula = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                    return;
                }
                Serializer.DeserializeList(this._sizeValues, reader);
            }
        }

        internal override void RefreshValues()
        {
            base.RefreshValues();
            if ((this.SizeValues != null) && (this.SizeValues.DataSeries != null))
            {
                this.SizeValues.RefreshData();
            }
        }

        void SizeValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.NotifyChartChanged("Sizes");
        }

        internal override void UpdateReferences()
        {
            base.UpdateReferences();
            this.UpdateSizeReference(this.SizeFormula);
        }

        void UpdateSizeReference(string sizeFormula)
        {
            this.SizeReference = FormulaUtility.Formula2Expression(base.Sheet, sizeFormula);
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (!string.IsNullOrEmpty(this._sizeValueFormula))
            {
                Serializer.SerializeObj(this._sizeValueFormula, "SizeValueFormula", writer);
            }
            else if ((this._sizeValues != null) && (this._sizeValues.Count > 0))
            {
                Serializer.SerializeList(this._sizeValues, "SizeValues", writer);
            }
            if (this.Bubble3D)
            {
                Serializer.SerializeObj((bool) this.Bubble3D, "Bubble3D", writer);
            }
            if (this._sizeFormatter != null)
            {
                Serializer.SerializeObj(this._sizeFormatter, "Formatter", writer);
            }
        }

        internal bool Bubble3D
        {
            get { return  this._bubble3D; }
            set
            {
                if (this._bubble3D != value)
                {
                    this._bubble3D = value;
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hidden; otherwise, <c>false</c>.
        /// </value>
        public override bool IsHidden
        {
            get { return  ((base.IsHidden && (this._sizeValues != null)) && this._sizeValues.AreAllDataSeriesInVisible); }
        }

        internal IFormatter SizeFormatter
        {
            get { return  this._sizeFormatter; }
            set
            {
                if (this._sizeFormatter != value)
                {
                    this._sizeFormatter = value;
                    base.NotifyChartChanged("SizeFormatter");
                }
            }
        }

        /// <summary>
        /// Gets or sets the size formula.
        /// </summary>
        /// <value>
        /// The size formula.
        /// </value>
        public string SizeFormula
        {
            get { return  this._sizeValueFormula; }
            set
            {
                if (value != this.SizeFormula)
                {
                    this.UpdateSizeReference(value);
                    this._sizeValueFormula = value;
                }
            }
        }

        internal SheetCellRange[] SizeRange
        {
            get { return  this._sizeValueRange; }
            set
            {
                DataOrientation? nullable = base.ValidateSeriesRange(value, false);
                if (!nullable.HasValue)
                {
                    throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
                }
                this._sizeValueRange = value;
                this._sizeDataOrientation = new DataOrientation?(nullable.Value);
            }
        }

        CalcExpression SizeReference
        {
            get { return  this._sizeValueReference; }
            set
            {
                if (value != this.SizeReference)
                {
                    this._sizeValueReference = value;
                    if (this._sizeValueReference != null)
                    {
                        this.SizeRange = SheetCellRangeUtility.ExtractAllExternalReference(base.Sheet, value);
                    }
                    else
                    {
                        this.SizeRange = null;
                    }
                    if (this.SizeValues.DataSeries != null)
                    {
                        this.SizeValues.RefreshData();
                    }
                    else
                    {
                        this.SizeValues.DataSeries = new SizeDataSeries(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the size values.
        /// </summary>
        /// <value>
        /// The size values.
        /// </value>
        public DoubleSeriesCollection SizeValues
        {
            get { return  this._sizeValues; }
            set
            {
                if (value != this.SizeValues)
                {
                    if (this._sizeValues != null)
                    {
                        this._sizeValues.DataSeries = null;
                    }
                    this._sizeValues = value;
                    this._sizeValueFormula = null;
                    DoubleSeriesCollection collection1 = this._sizeValues;
                    base.NotifyChartChanged("Sizes");
                }
            }
        }

        class SizeDataSeries : IDataSeries
        {
            SpreadBubbleSeries _bubbleSeries;

            public SizeDataSeries(SpreadBubbleSeries bubbleSeries)
            {
                this._bubbleSeries = bubbleSeries;
            }

            public Dt.Cells.Data.DataOrientation? DataOrientation
            {
                get { return  this._bubbleSeries._sizeDataOrientation; }
            }

            public CalcExpression DataReference
            {
                get { return  this._bubbleSeries.SizeReference; }
            }

            public bool DisplayHiddenData
            {
                get { return  this._bubbleSeries.DisplayHidden; }
            }

            public Dt.Cells.Data.EmptyValueStyle EmptyValueStyle
            {
                get { return  this._bubbleSeries.EmptyValueStyle; }
            }

            public ICalcEvaluator Evaluator
            {
                get { return  this._bubbleSeries.Sheet; }
            }
        }
    }
}

