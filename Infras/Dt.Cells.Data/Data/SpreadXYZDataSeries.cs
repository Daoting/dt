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
    /// Represents data series that contains collection of 3D points.
    /// </summary>
    public class SpreadXYZDataSeries : SpreadXYDataSeries
    {
        DataOrientation? _zDataOrientation = 0;
        string _zValueFormula;
        SheetCellRange[] _zValueRange;
        CalcExpression _zValueReference;
        DoubleSeriesCollection _zValues;

        internal override DataLabel CreateDataLabelOnNeeded(int pointIndex)
        {
            return new XYZDataLabel(this, pointIndex);
        }

        internal override List<List<SheetCellRange>> GetDataRanges()
        {
            List<List<SheetCellRange>> dataRanges = base.GetDataRanges();
            if ((this.ZValueRange != null) && (this.ZValueRange.Length > 0))
            {
                dataRanges.Add(new List<SheetCellRange>(this.ZValueRange));
            }
            return dataRanges;
        }

        internal override void Init()
        {
            base.Init();
            this._zValues = new DoubleSeriesCollection();
            this._zDataOrientation = 0;
        }

        internal override void OnAddColumnRange(int column, int columnCount)
        {
            base.OnAddRowRange(column, columnCount);
            this.ZValueFormula = FormulaUtility.AddColumnRange(base.Sheet, this.ZValueFormula, column, columnCount);
        }

        internal override void OnAddRowRange(int row, int rowCount)
        {
            base.OnAddRowRange(row, rowCount);
            this.ZValueFormula = FormulaUtility.AddRowRange(base.Sheet, this.ZValueFormula, row, rowCount);
        }

        internal override void OnDisposed()
        {
            base.OnDisposed();
            this.ZValues.Dispose();
        }

        internal override void OnRemoveColumnRange(int column, int columnCount)
        {
            base.OnRemoveColumnRange(column, columnCount);
            this.ZValueFormula = FormulaUtility.RemoveColumnRange(base.Sheet, this.ZValueFormula, column, columnCount);
        }

        internal override void OnRemoveRowRange(int row, int rowCount)
        {
            base.OnRemoveRowRange(row, rowCount);
            this.ZValueFormula = FormulaUtility.RemoveRowRange(base.Sheet, this.ZValueFormula, row, rowCount);
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "ZValueFormula")
                {
                    if (str != "ZValues")
                    {
                        return;
                    }
                }
                else
                {
                    this._zValueFormula = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                    return;
                }
                Serializer.DeserializeList(this._zValues, reader);
            }
        }

        internal override void RefreshValues()
        {
            base.RefreshValues();
            if ((this.ZValues != null) && (this.ZValues.DataSeries != null))
            {
                this.ZValues.RefreshData();
            }
        }

        internal override void UpdateReferences()
        {
            base.UpdateReferences();
            this.UpdateZValueReference(this.ZValueFormula);
        }

        void UpdateZValueReference(string zValueFormula)
        {
            this.ZValueReference = FormulaUtility.Formula2Expression(base.Sheet, zValueFormula);
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (!string.IsNullOrEmpty(this._zValueFormula))
            {
                Serializer.SerializeObj(this._zValueFormula, "ZValueFormula", writer);
            }
            else if ((this._zValues != null) && (this._zValues.Count > 0))
            {
                Serializer.SerializeList(this._zValues, "ZValues", writer);
            }
        }

        void ZValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ((ISpreadChartElement) this).NotifyElementChanged("ZValues");
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hidden; otherwise, <c>false</c>.
        /// </value>
        public override bool IsHidden
        {
            get { return  ((base.IsHidden && (this._zValues != null)) && this._zValues.AreAllDataSeriesInVisible); }
        }

        /// <summary>
        /// Gets or sets the Z value formula.
        /// </summary>
        /// <value>
        /// The Z value formula.
        /// </value>
        public string ZValueFormula
        {
            get { return  this._zValueFormula; }
            set
            {
                if (value != this.ZValueFormula)
                {
                    this.UpdateZValueReference(value);
                    this._zValueFormula = value;
                }
            }
        }

        internal SheetCellRange[] ZValueRange
        {
            get { return  this._zValueRange; }
            set
            {
                DataOrientation? nullable = base.ValidateSeriesRange(value, false);
                if (!nullable.HasValue)
                {
                    throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
                }
                this._zValueRange = value;
                this._zDataOrientation = new DataOrientation?(nullable.Value);
            }
        }

        CalcExpression ZValueReference
        {
            get { return  this._zValueReference; }
            set
            {
                if (value != this.ZValueReference)
                {
                    this._zValueReference = value;
                    if (this._zValueReference != null)
                    {
                        this.ZValueRange = SheetCellRangeUtility.ExtractAllExternalReference(base.Sheet, value);
                    }
                    else
                    {
                        this.ZValueRange = null;
                    }
                    if (this.ZValues.DataSeries != null)
                    {
                        this.ZValues.RefreshData();
                    }
                    else
                    {
                        this.ZValues.DataSeries = new ZValueDataSeries(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Z values.
        /// </summary>
        /// <value>
        /// The Z values.
        /// </value>
        public DoubleSeriesCollection ZValues
        {
            get { return  this._zValues; }
            set
            {
                if (value != this.ZValues)
                {
                    if (this._zValues != null)
                    {
                        this._zValues.DataSeries = null;
                    }
                    this._zValues = value;
                    this._zValueFormula = null;
                    ((ISpreadChartElement) this).NotifyElementChanged("ZValues");
                }
            }
        }

        class ZValueDataSeries : IDataSeries
        {
            SpreadXYZDataSeries _xyzSeries;

            public ZValueDataSeries(SpreadXYZDataSeries xyzSeries)
            {
                this._xyzSeries = xyzSeries;
            }

            public Dt.Cells.Data.DataOrientation? DataOrientation
            {
                get { return  this._xyzSeries._zDataOrientation; }
            }

            public CalcExpression DataReference
            {
                get { return  this._xyzSeries.ZValueReference; }
            }

            public bool DisplayHiddenData
            {
                get { return  this._xyzSeries.DisplayHidden; }
            }

            public Dt.Cells.Data.EmptyValueStyle EmptyValueStyle
            {
                get { return  this._xyzSeries.EmptyValueStyle; }
            }

            public ICalcEvaluator Evaluator
            {
                get { return  this._xyzSeries.Sheet; }
            }
        }
    }
}

