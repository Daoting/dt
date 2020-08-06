#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using Dt.Xls.Chart;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a data series which contains a group of data values.
    /// </summary>
    public class SpreadDataSeries : SpreadChartElement, IRangeSupport, IXmlSerializable, IDisposable
    {
        int _axisXIndex;
        int _axisYIndex;
        SpreadChartType? _chartType = null;
        Dictionary<int, DataLabel> _dataLabels;
        Dt.Cells.Data.DataLabelSettings _dataLabelSettings;
        ChartLabelStyleInfo _dataLabelStyleInfo;
        Dictionary<int, DataMarker> _dataMarkers;
        Dictionary<int, DataPoint> _dataPoints;
        bool _displayHidden;
        Dt.Cells.Data.EmptyValueStyle _emptyValueStyle;
        bool _invertIfNegative;
        bool _invertIfNegativeSet;
        Windows.Foundation.Size _markerSize;
        ChartSymbolStyleInfo _markerStyleInfo;
        Dt.Cells.Data.MarkerType _markerType;
        string _name;
        string _nameFormula;
        DataOrientation? _nameOrientation = 0;
        SheetCellRange _nameRange;
        CalcExpression _nameReference;
        StringSeriesCollection _names;
        Brush _negativeFill;
        bool _negativeFillSet;
        string _negativeFillThemeColor;
        bool _negativeFillThemeColorSet;
        List<TrendLine> _trendLines;
        bool _useSecondaryAxis;
        string _valueFormula;
        DataOrientation? _valueOrientation = 0;
        SheetCellRange[] _valueRange;
        CalcExpression _valueReference;
        DoubleSeriesCollection _values;
        string defaultPrefix = "Accent";
        string defaultSeparator = " ";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadDataSeries" /> class.
        /// </summary>
        public SpreadDataSeries()
        {
            this.Init();
        }

        internal override void AfterReadXml()
        {
            base.AfterReadXml();
            base.ResumeEvents();
        }

        bool AreDataRangesInColumn(int column)
        {
            DataOrientation? nullable = this._valueOrientation;
            if ((((DataOrientation) nullable.GetValueOrDefault()) == DataOrientation.Horizontal) && nullable.HasValue)
            {
                return false;
            }
            using (List<List<SheetCellRange>>.Enumerator enumerator = this.GetDataRanges().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    using (List<SheetCellRange>.Enumerator enumerator2 = enumerator.Current.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            if (enumerator2.Current.Column != column)
                            {
                                return false;
                            }
                        }
                        continue;
                    }
                }
            }
            return true;
        }

        bool AreDataRangesInRow(int row)
        {
            DataOrientation? nullable = this._valueOrientation;
            if ((((DataOrientation) nullable.GetValueOrDefault()) == DataOrientation.Vertical) && nullable.HasValue)
            {
                return false;
            }
            using (List<List<SheetCellRange>>.Enumerator enumerator = this.GetDataRanges().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    using (List<SheetCellRange>.Enumerator enumerator2 = enumerator.Current.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            if (enumerator2.Current.Row != row)
                            {
                                return false;
                            }
                        }
                        continue;
                    }
                }
            }
            return true;
        }

        void AttachFormatChanged()
        {
        }

        internal override void BeforeReadXml()
        {
            base.BeforeReadXml();
            this.Init();
            base.SuspendEvents();
        }

        bool CanChangeChartType(SpreadChartType newChartType)
        {
            return (base.GetType() == this.GetPreferedDataSeriesType(newChartType));
        }

        internal virtual DataLabel CreateDataLabelOnNeeded(int pointIndex)
        {
            if ((this.Chart == null) || (this.Chart.ChartType != SpreadChartType.StockHighLowOpenClose))
            {
                return new DataLabel(this, pointIndex);
            }
            if ((this.Chart.DataSeries.Count > 0) && (this.Chart.DataSeries[0] is SpreadOpenHighLowCloseSeries))
            {
                return new OpenHighLowCloseDataLabel(this, this.Chart.DataSeries[0] as SpreadOpenHighLowCloseSeries, pointIndex);
            }
            return null;
        }

        internal virtual DataMarker CreateDataMarkerOnNeeded(int pointIndex)
        {
            if ((this.Chart == null) || (this.Chart.ChartType != SpreadChartType.StockHighLowOpenClose))
            {
                return new DataMarker(this, pointIndex);
            }
            if ((this.Chart.DataSeries.Count > 0) && (this.Chart.DataSeries[0] is SpreadOpenHighLowCloseSeries))
            {
                return new OpenHighLowCloseDataMarker(this, this.Chart.DataSeries[0] as SpreadOpenHighLowCloseSeries, pointIndex);
            }
            return null;
        }

        internal virtual DataPoint CreateDataPointOnNeeded(int pointIndex)
        {
            if ((this.Chart == null) || (this.Chart.ChartType != SpreadChartType.StockHighLowOpenClose))
            {
                return new DataPoint(this, pointIndex);
            }
            if ((this.Chart.DataSeries.Count > 0) && (this.Chart.DataSeries[0] is SpreadOpenHighLowCloseSeries))
            {
                return new OpenHighLowCloseDataPoint(this, this.Chart.DataSeries[0] as SpreadOpenHighLowCloseSeries, pointIndex);
            }
            return null;
        }

        internal virtual void DataLabelSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyDataSeriesChanged(e.PropertyName);
        }

        void DataSeries_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyDataSeriesChanged(e.PropertyName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Values.Dispose();
            this.OnDisposed();
        }

        void FormatInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyDataSeriesChanged(e.PropertyName);
        }

        internal Brush GetAutomaticColor(double currentIndex, double totalCount)
        {
            if (base.ThemeContext == null)
            {
                return null;
            }
            if ((currentIndex < 0.0) || (totalCount <= 0.0))
            {
                return null;
            }
            double a = -70.0 + (140.0 * ((currentIndex + 1.0) / (totalCount + 1.0)));
            double num2 = (currentIndex % 6.0) + 1.0;
            string themeColor = this.defaultPrefix + this.defaultSeparator + ((double) num2);
            if (currentIndex > 5.0)
            {
                themeColor = themeColor + this.defaultSeparator + ((double) Math.Ceiling(a));
            }
            return new SolidColorBrush(this.ThemeContext.GetThemeColor(themeColor));
        }

        /// <summary>
        /// Gets the data label.
        /// </summary>
        /// <param name="pointIndex">Index of the point.</param>
        /// <returns></returns>
        public DataLabel GetDataLabel(int pointIndex)
        {
            if (this._dataLabels == null)
            {
                this._dataLabels = new Dictionary<int, DataLabel>();
            }
            DataLabel label = null;
            if (this._dataLabels.TryGetValue(pointIndex, out label) && (label != null))
            {
                return label;
            }
            label = this.CreateDataLabelOnNeeded(pointIndex);
            if (label != null)
            {
                this._dataLabels[pointIndex] = label;
                return label;
            }
            return null;
        }

        /// <summary>
        /// Gets all the data labels.
        /// </summary>
        /// <returns>
        /// Returns all the data labels.
        /// </returns>
        public DataLabel[] GetDataLabels()
        {
            if ((this._dataLabels != null) && (this._dataLabels.Count > 0))
            {
                return Enumerable.ToArray<DataLabel>((IEnumerable<DataLabel>) this._dataLabels.Values);
            }
            return new DataLabel[0];
        }

        /// <summary>
        /// Gets the data marker.
        /// </summary>
        /// <param name="pointIndex">Index of the point.</param>
        /// <returns></returns>
        public DataMarker GetDataMarker(int pointIndex)
        {
            if (this._dataMarkers == null)
            {
                this._dataMarkers = new Dictionary<int, DataMarker>();
            }
            DataMarker marker = null;
            if (this._dataMarkers.TryGetValue(pointIndex, out marker) && (marker != null))
            {
                return marker;
            }
            marker = this.CreateDataMarkerOnNeeded(pointIndex);
            if (marker != null)
            {
                this._dataMarkers[pointIndex] = marker;
                return marker;
            }
            return null;
        }

        /// <summary>
        /// Gets all the data markers.
        /// </summary>
        /// <returns>
        /// Returns all the data markers.
        /// </returns>
        public DataMarker[] GetDataMarkers()
        {
            if ((this._dataMarkers != null) && (this._dataMarkers.Count > 0))
            {
                return Enumerable.ToArray<DataMarker>((IEnumerable<DataMarker>) this._dataMarkers.Values);
            }
            return new DataMarker[0];
        }

        /// <summary>
        /// Gets the data point.
        /// </summary>
        /// <param name="pointIndex">Index of the point.</param>
        /// <returns></returns>
        public DataPoint GetDataPoint(int pointIndex)
        {
            if (this._dataPoints == null)
            {
                this._dataPoints = new Dictionary<int, DataPoint>();
            }
            DataPoint point = null;
            if (this._dataPoints.TryGetValue(pointIndex, out point) && (point != null))
            {
                return point;
            }
            point = this.CreateDataPointOnNeeded(pointIndex);
            if (point != null)
            {
                this._dataPoints[pointIndex] = point;
                return point;
            }
            return null;
        }

        /// <summary>
        /// Gets all the data points.
        /// </summary>
        /// <returns>
        /// Returns all the data points.
        /// </returns>
        public DataPoint[] GetDataPoints()
        {
            if ((this._dataPoints != null) && (this._dataPoints.Count > 0))
            {
                return Enumerable.ToArray<DataPoint>((IEnumerable<DataPoint>) this._dataPoints.Values);
            }
            return new DataPoint[0];
        }

        internal virtual List<List<SheetCellRange>> GetDataRanges()
        {
            List<List<SheetCellRange>> list = new List<List<SheetCellRange>>();
            if ((this.ValueRange != null) && (this.ValueRange.Length > 0))
            {
                list.Add(new List<SheetCellRange>(this.ValueRange));
            }
            return list;
        }

        Type GetPreferedDataSeriesType(SpreadChartType chartType)
        {
            switch (chartType)
            {
                case SpreadChartType.BarClustered:
                case SpreadChartType.BarStacked:
                case SpreadChartType.BarStacked100pc:
                case SpreadChartType.ColumnClustered:
                case SpreadChartType.ColumnStacked:
                case SpreadChartType.ColumnStacked100pc:
                case SpreadChartType.Line:
                case SpreadChartType.LineStacked:
                case SpreadChartType.LineStacked100pc:
                case SpreadChartType.LineWithMarkers:
                case SpreadChartType.LineStackedWithMarkers:
                case SpreadChartType.LineStacked100pcWithMarkers:
                case SpreadChartType.Pie:
                case SpreadChartType.PieExploded:
                case SpreadChartType.PieDoughnut:
                case SpreadChartType.PieExplodedDoughnut:
                case SpreadChartType.Area:
                case SpreadChartType.AreaStacked:
                case SpreadChartType.AreaStacked100pc:
                case SpreadChartType.Radar:
                case SpreadChartType.RadarWithMarkers:
                case SpreadChartType.RadarFilled:
                    return typeof(SpreadDataSeries);

                case SpreadChartType.Scatter:
                case SpreadChartType.ScatterLines:
                case SpreadChartType.ScatterLinesWithMarkers:
                case SpreadChartType.ScatterLinesSmoothed:
                case SpreadChartType.ScatterLinesSmoothedWithMarkers:
                    return typeof(SpreadXYDataSeries);

                case SpreadChartType.Bubble:
                    return typeof(SpreadBubbleSeries);

                case SpreadChartType.StockHighLowOpenClose:
                    return typeof(SpreadOpenHighLowCloseSeries);
            }
            return typeof(SpreadDataSeries);
        }

        void IRangeSupport.AddColumns(int column, int count)
        {
            this.OnAddColumnRange(column, count);
        }

        void IRangeSupport.AddRows(int row, int count)
        {
            this.OnAddRowRange(row, count);
        }

        void IRangeSupport.Clear(int row, int column, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        void IRangeSupport.Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        void IRangeSupport.Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        void IRangeSupport.RemoveColumns(int column, int count)
        {
            this.OnRemoveColumnRange(column, count);
        }

        void IRangeSupport.RemoveRows(int row, int count)
        {
            this.OnRemoveRowRange(row, count);
        }

        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        internal virtual void Init()
        {
            this._valueFormula = null;
            this._values = new DoubleSeriesCollection();
            this._values.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Values_CollectionChanged);
            this._values.IsDateTimeSeriesChanged += new EventHandler(this.Values_IsDateTimeSeriesChanged);
            this._valueOrientation = 0;
            this._names = new StringSeriesCollection();
            this._names.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Names_CollectionChanged);
            this._nameOrientation = 0;
            this._displayHidden = false;
            this._emptyValueStyle = Dt.Cells.Data.EmptyValueStyle.Gaps;
            this._markerStyleInfo = null;
            this._dataLabelStyleInfo = null;
            this._markerType = Dt.Cells.Data.MarkerType.Automatic;
            this._markerSize = new Windows.Foundation.Size(7.0, 7.0);
            this._useSecondaryAxis = false;
            this._invertIfNegative = false;
            this._dataPoints = null;
            this._dataLabels = null;
            this._dataMarkers = null;
            this._axisXIndex = 0;
            this._axisYIndex = 0;
            this.ResetInvertIfNegative();
            this.ResetNegativeFill();
            this.ResetNegativeFillThemeColor();
            this._trendLines = null;
        }

        bool IsPieChart()
        {
            if (this.Chart == null)
            {
                return false;
            }
            if (((this.Chart.ChartType != SpreadChartType.Pie) && (this.Chart.ChartType != SpreadChartType.PieDoughnut)) && (this.Chart.ChartType != SpreadChartType.PieExploded))
            {
                return (this.Chart.ChartType == SpreadChartType.PieExplodedDoughnut);
            }
            return true;
        }

        void Names_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyDataSeriesChanged("Names");
        }

        void NotifyDataSeriesChanged(string changed)
        {
            if (!base.IsEventsSuspend())
            {
                ((ISpreadChartElement) this).NotifyElementChanged(changed);
            }
        }

        internal virtual void OnAddColumnRange(int column, int columnCount)
        {
            this.NameFormula = FormulaUtility.AddColumnRange(this.Sheet, this.NameFormula, column, columnCount);
            this.ValueFormula = FormulaUtility.AddColumnRange(this.Sheet, this.ValueFormula, column, columnCount);
        }

        internal virtual void OnAddRowRange(int row, int rowCount)
        {
            this.NameFormula = FormulaUtility.AddRowRange(this.Sheet, this.NameFormula, row, rowCount);
            this.ValueFormula = FormulaUtility.AddRowRange(this.Sheet, this.ValueFormula, row, rowCount);
        }

        internal override void OnChartChanged()
        {
            this.UpdateReferences();
            this.UpdateDataPointsChart();
        }

        internal virtual void OnDisposed()
        {
        }

        internal virtual void OnRemoveColumnRange(int column, int columnCount)
        {
            this.NameFormula = FormulaUtility.RemoveColumnRange(this.Sheet, this.NameFormula, column, columnCount);
            this.ValueFormula = FormulaUtility.RemoveColumnRange(this.Sheet, this.ValueFormula, column, columnCount);
        }

        internal virtual void OnRemoveRowRange(int row, int rowCount)
        {
            this.NameFormula = FormulaUtility.RemoveRowRange(this.Sheet, this.NameFormula, row, rowCount);
            this.ValueFormula = FormulaUtility.RemoveRowRange(this.Sheet, this.ValueFormula, row, rowCount);
        }

        internal virtual void OnResumeAfterDeserialization()
        {
            this.UpdateReferences();
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            base.ReadXmlInternal(reader);
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
            {
                switch (reader.Name)
                {
                    case "ChartType":
                        try
                        {
                            this._chartType = new SpreadChartType?((SpreadChartType) Serializer.DeserializeObj(typeof(SpreadChartType), reader));
                        }
                        catch (Exception)
                        {
                            this._chartType = 0;
                        }
                        return;

                    case "ValueFormula":
                        this._valueFormula = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                        return;

                    case "Values":
                        Serializer.DeserializeList(this._values, reader);
                        return;

                    case "NameFormula":
                        this._nameFormula = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                        return;

                    case "Name":
                        this._name = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                        return;

                    case "DisplayHidden":
                        this._displayHidden = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "DisplayEmptyCellsAs":
                        this._emptyValueStyle = (Dt.Cells.Data.EmptyValueStyle) Serializer.DeserializeObj(typeof(Dt.Cells.Data.EmptyValueStyle), reader);
                        return;

                    case "MarkerStyleInfo":
                        this._markerStyleInfo = (ChartSymbolStyleInfo) Serializer.DeserializeObj(typeof(ChartSymbolStyleInfo), reader);
                        this._markerStyleInfo.SetSymbolInternal(this);
                        return;

                    case "LabelStyleInfo":
                        this._dataLabelStyleInfo = (ChartLabelStyleInfo) Serializer.DeserializeObj(typeof(ChartLabelStyleInfo), reader);
                        this._dataLabelStyleInfo.SetSymbolInternal(this);
                        return;

                    case "DataLabelSettings":
                        this._dataLabelSettings = (Dt.Cells.Data.DataLabelSettings) Serializer.DeserializeObj(typeof(Dt.Cells.Data.DataLabelSettings), reader);
                        return;

                    case "MarkerType":
                        this._markerType = (Dt.Cells.Data.MarkerType) Serializer.DeserializeObj(typeof(Dt.Cells.Data.MarkerType), reader);
                        return;

                    case "MarkerSize":
                        this._markerSize = (Windows.Foundation.Size) Serializer.DeserializeObj(typeof(Windows.Foundation.Size), reader);
                        return;

                    case "UseSecondaryAxis":
                        this._useSecondaryAxis = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "DataPoints":
                    {
                        List<DataPoint> list = new List<DataPoint>();
                        Serializer.DeserializeList((IList) list, reader);
                        this._dataPoints = new Dictionary<int, DataPoint>();
                        foreach (DataPoint point in list)
                        {
                            point.SetParentSeriesInternal(this);
                            this._dataPoints.Add(point.PointIndex, point);
                        }
                        return;
                    }
                    case "DataLabels":
                    {
                        List<DataLabel> list2 = new List<DataLabel>();
                        Serializer.DeserializeList((IList) list2, reader);
                        this._dataLabels = new Dictionary<int, DataLabel>();
                        foreach (DataLabel label in list2)
                        {
                            label.SetParentSeriesInternal(this);
                            this._dataLabels.Add(label.PointIndex, label);
                        }
                        return;
                    }
                    case "DataMarkers":
                    {
                        List<DataMarker> list3 = new List<DataMarker>();
                        Serializer.DeserializeList((IList) list3, reader);
                        this._dataMarkers = new Dictionary<int, DataMarker>();
                        foreach (DataMarker marker in list3)
                        {
                            marker.SetParentSeriesInternal(this);
                            this._dataMarkers.Add(marker.PointIndex, marker);
                        }
                        return;
                    }
                    case "InvertIfNegative":
                        this._invertIfNegativeSet = true;
                        this._invertIfNegative = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "NegativeFill":
                        this._negativeFillSet = true;
                        this._negativeFill = (Brush) Serializer.DeserializeObj(typeof(Brush), reader);
                        return;

                    case "NegativeFillColor":
                        this._negativeFillThemeColorSet = true;
                        this._negativeFillThemeColor = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
                        return;
                }
            }
        }

        internal virtual void RefreshValues()
        {
            if ((this.Values != null) && (this.Values.DataSeries != null))
            {
                this.Values.RefreshData();
            }
            if ((this.Names != null) && (this.Names.DataSeries != null))
            {
                this.Names.RefreshData();
            }
        }

        /// <summary>
        /// Resets the value of the ChartType property.
        /// </summary>
        public void ResetChartType()
        {
            this._chartType = null;
        }

        /// <summary>
        /// Resets the value of the InvertIfNegative property.
        /// </summary>
        public void ResetInvertIfNegative()
        {
            this._invertIfNegative = false;
            this._invertIfNegativeSet = false;
        }

        /// <summary>
        /// Resets the value of the NegativeFill property.
        /// </summary>
        public void ResetNegativeFill()
        {
            this._negativeFill = null;
            this._negativeFillSet = false;
        }

        /// <summary>
        /// Resets the value of the NegativeFillThemeColor property.
        /// </summary>
        public void ResetNegativeFillThemeColor()
        {
            this._negativeFillThemeColor = null;
            this._negativeFillThemeColorSet = false;
        }

        internal void ResumeAfterDeserialization()
        {
            base.SuspendEvents();
            this.OnResumeAfterDeserialization();
            base.ResumeEvents();
        }

        void UpdateDataPointsChart()
        {
            if (this._dataPoints != null)
            {
                using (Dictionary<int, DataPoint>.ValueCollection.Enumerator enumerator = this._dataPoints.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.ChartBase = this.Chart;
                    }
                }
            }
            if (this._dataMarkers != null)
            {
                using (Dictionary<int, DataMarker>.ValueCollection.Enumerator enumerator2 = this._dataMarkers.Values.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        enumerator2.Current.ChartBase = this.Chart;
                    }
                }
            }
            if (this._dataLabels != null)
            {
                using (Dictionary<int, DataLabel>.ValueCollection.Enumerator enumerator3 = this._dataLabels.Values.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        enumerator3.Current.ChartBase = this.Chart;
                    }
                }
            }
        }

        void UpdateDataPointsFormatter()
        {
        }

        void UpdateFormatter()
        {
            this.UpdateDataPointsFormatter();
        }

        internal virtual void UpdateNameReference(string nameFormula)
        {
            this.NameReference = FormulaUtility.Formula2Expression(this.Sheet, nameFormula);
        }

        internal virtual void UpdateReferences()
        {
            this.UpdateValueReference(this.ValueFormula);
            this.UpdateNameReference(this.NameFormula);
        }

        void UpdateValueReference(string valueFormula)
        {
            this.ValueReference = FormulaUtility.Formula2Expression(this.Sheet, valueFormula);
        }

        internal DataOrientation? ValidateSeriesRange(SheetCellRange[] ranges, bool atSameLine)
        {
            if ((ranges == null) || (ranges.Length == 0))
            {
                return 0;
            }
            SheetCellRange range = ranges[0];
            if ((range.RowCount > 1) && (range.ColumnCount > 1))
            {
                throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
            }
            Worksheet sheet = range.Sheet;
            DataOrientation? nullable = new DataOrientation?((range.ColumnCount > range.RowCount) ? DataOrientation.Horizontal : DataOrientation.Vertical);
            DataOrientation? nullable2 = nullable;
            int num = ((((DataOrientation) nullable2.GetValueOrDefault()) == DataOrientation.Vertical) && nullable2.HasValue) ? range.Column : range.Row;
            for (int i = 1; i < ranges.Length; i++)
            {
                SheetCellRange range2 = ranges[i];
                if (range2.Sheet != sheet)
                {
                    throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
                }
                if ((range2.RowCount > 1) && (range2.ColumnCount > 1))
                {
                    throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
                }
                if (nullable.HasValue)
                {
                    DataOrientation orientation2 = (range2.ColumnCount > range2.RowCount) ? DataOrientation.Horizontal : DataOrientation.Vertical;
                    DataOrientation? nullable3 = nullable;
                    if ((orientation2 != ((DataOrientation) nullable3.GetValueOrDefault())) || !nullable3.HasValue)
                    {
                        nullable = null;
                    }
                }
                if (atSameLine && nullable.HasValue)
                {
                    DataOrientation? nullable4 = nullable;
                    int num3 = ((((DataOrientation) nullable4.GetValueOrDefault()) == DataOrientation.Vertical) && nullable4.HasValue) ? range.Column : range.Row;
                    if (num3 != num)
                    {
                        throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
                    }
                }
            }
            return nullable;
        }

        void Values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyDataSeriesChanged("Values");
        }

        void Values_IsDateTimeSeriesChanged(object sender, EventArgs e)
        {
            this.NotifyDataSeriesChanged("Values");
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (this._chartType.HasValue)
            {
                Serializer.SerializeObj(this._chartType.Value, "ChartType", writer);
            }
            if (!string.IsNullOrEmpty(this._valueFormula))
            {
                Serializer.SerializeObj(this._valueFormula, "ValueFormula", writer);
            }
            else if ((this._values != null) && (this._values.Count > 0))
            {
                Serializer.SerializeList(this._values, "Values", writer);
            }
            if (!string.IsNullOrEmpty(this._nameFormula))
            {
                Serializer.SerializeObj(this._nameFormula, "NameFormula", writer);
            }
            else
            {
                Serializer.SerializeObj(this._name, "Name", writer);
            }
            if (this._displayHidden)
            {
                Serializer.SerializeObj((bool) this._displayHidden, "DisplayHidden", writer);
            }
            if (this._emptyValueStyle != Dt.Cells.Data.EmptyValueStyle.Gaps)
            {
                Serializer.SerializeObj(this._emptyValueStyle, "DisplayEmptyCellsAs", writer);
            }
            if (this._markerStyleInfo != null)
            {
                Serializer.SerializeObj(this._markerStyleInfo, "MarkerStyleInfo", writer);
            }
            if (this._dataLabelStyleInfo != null)
            {
                Serializer.SerializeObj(this._dataLabelStyleInfo, "LabelStyleInfo", writer);
            }
            if (this._dataLabelSettings != null)
            {
                Serializer.SerializeObj(this._dataLabelSettings, "DataLabelSettings", writer);
            }
            if (this._markerType != Dt.Cells.Data.MarkerType.Automatic)
            {
                Serializer.SerializeObj(this._markerType, "MarkerType", writer);
            }
            if (this._markerSize != Windows.Foundation.Size.Empty)
            {
                Serializer.SerializeObj(this._markerSize, "MarkerSize", writer);
            }
            if (this._useSecondaryAxis)
            {
                Serializer.SerializeObj((bool) this._useSecondaryAxis, "UseSecondaryAxis", writer);
            }
            if (this._invertIfNegative)
            {
                Serializer.SerializeObj((bool) this._invertIfNegative, "InvertIfNegative", writer);
            }
            if ((this._dataPoints != null) && (this._dataPoints.Count > 0))
            {
                List<DataPoint> list = new List<DataPoint>((IEnumerable<DataPoint>) this._dataPoints.Values);
                Serializer.SerializeGenericList<DataPoint>((IList<DataPoint>) list, "DataPoints", writer);
            }
            if ((this._dataLabels != null) && (this._dataLabels.Count > 0))
            {
                List<DataLabel> list2 = new List<DataLabel>((IEnumerable<DataLabel>) this._dataLabels.Values);
                Serializer.SerializeGenericList<DataLabel>((IList<DataLabel>) list2, "DataLabels", writer);
            }
            if ((this._dataMarkers != null) && (this._dataMarkers.Count > 0))
            {
                List<DataMarker> list3 = new List<DataMarker>((IEnumerable<DataMarker>) this._dataMarkers.Values);
                Serializer.SerializeGenericList<DataMarker>((IList<DataMarker>) list3, "DataMarkers", writer);
            }
            if (this._invertIfNegativeSet)
            {
                Serializer.SerializeObj((bool) this._invertIfNegative, "InvertIfNegative", writer);
            }
            if (this._negativeFillSet)
            {
                if (this._negativeFill == null)
                {
                    writer.WriteStartElement("NegativeFill");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this._negativeFill, "NegativeFill", writer);
                }
            }
            if (this._negativeFillThemeColorSet)
            {
                if (this._negativeFillThemeColor == null)
                {
                    writer.WriteStartElement("NegativeFillColor");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this._negativeFillThemeColor, "NegativeFillColor", writer);
                }
            }
        }

        /// <summary>
        /// Gets the actual data label settings.
        /// </summary>
        /// <value>
        /// The actual settings of the data label.
        /// </value>
        public Dt.Cells.Data.DataLabelSettings ActualDataLabelSettings
        {
            get
            {
                Dt.Cells.Data.DataLabelSettings settings = new Dt.Cells.Data.DataLabelSettings();
                Dt.Cells.Data.DataLabelSettings settings2 = (this.Chart != null) ? this.Chart.DataLabelSettings : null;
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowCategoryNameSet)
                {
                    settings.ShowCategoryName = this.DataLabelSettings.ShowCategoryName;
                }
                else if ((settings2 != null) && settings2.IsShowCategoryNameSet)
                {
                    settings.ShowCategoryName = settings2.ShowCategoryName;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowValueSet)
                {
                    settings.ShowValue = this.DataLabelSettings.ShowValue;
                }
                else if ((settings2 != null) && settings2.IsShowValueSet)
                {
                    settings.ShowValue = settings2.ShowValue;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowSeriesNameSet)
                {
                    settings.ShowSeriesName = this.DataLabelSettings.ShowSeriesName;
                }
                else if ((settings2 != null) && settings2.IsShowSeriesNameSet)
                {
                    settings.ShowSeriesName = settings2.ShowSeriesName;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowBubbleSizeSet)
                {
                    settings.ShowBubbleSize = this.DataLabelSettings.ShowBubbleSize;
                }
                else if ((settings2 != null) && settings2.IsShowBubbleSizeSet)
                {
                    settings.ShowBubbleSize = settings2.ShowBubbleSize;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowPercentSet)
                {
                    settings.ShowPercent = this.DataLabelSettings.ShowPercent;
                }
                else if ((settings2 != null) && settings2.IsShowPercentSet)
                {
                    settings.ShowPercent = settings2.ShowPercent;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsSeparatorSet)
                {
                    settings.Separator = this.DataLabelSettings.Separator;
                    return settings;
                }
                if ((settings2 != null) && settings2.IsSeparatorSet)
                {
                    settings.Separator = settings2.Separator;
                }
                return settings;
            }
        }

        /// <summary>
        /// Gets the actual fill brush for the negative value.
        /// </summary>
        /// <value>
        /// The actual fill brush for the negative value.
        /// </value>
        public Brush ActualNegativeFill
        {
            get
            {
                if (this._negativeFillSet)
                {
                    return this._negativeFill;
                }
                if (this._negativeFillThemeColorSet && (base.ThemeContext != null))
                {
                    Windows.UI.Color themeColor = base.ThemeContext.GetThemeColor(this._negativeFillThemeColor);
                    return new SolidColorBrush(themeColor);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the fill brush that is created automatically.
        /// </summary>
        /// <value>
        /// The fill brush that is created automatically.
        /// </value>
        public override Brush AutomaticFill
        {
            get
            {
                if (this.Chart != null)
                {
                    double index = this.Chart.DataSeries.IndexOf(this);
                    if (index == -1.0)
                    {
                        foreach (SpreadDataSeries series in this.Chart.DisplayingDataSeries)
                        {
                            if (series is SpreadOpenHighLowCloseSeries)
                            {
                                SpreadOpenHighLowCloseSeries series2 = series as SpreadOpenHighLowCloseSeries;
                                if ((object.ReferenceEquals(series2.OpenSeries, this) || object.ReferenceEquals(series2.HighSeries, this)) || (object.ReferenceEquals(series2.LowSeries, this) || object.ReferenceEquals(series2.CloseSeries, this)))
                                {
                                    index = this.Chart.DisplayingDataSeries.IndexOf(series);
                                }
                            }
                        }
                    }
                    double count = this.Chart.DataSeries.Count;
                    Brush automaticColor = this.GetAutomaticColor(index, count);
                    if (automaticColor != null)
                    {
                        return automaticColor;
                    }
                }
                return base.AutomaticFill;
            }
        }

        /// <summary>
        /// Gets the brush stroke that is created automatically.
        /// </summary>
        /// <value>
        /// The brush stroke that is created automatically.
        /// </value>
        public override Brush AutomaticStroke
        {
            get
            {
                if (this.Chart != null)
                {
                    return this.ActualFill;
                }
                return base.AutomaticStroke;
            }
        }

        internal int AxisXIndex
        {
            get { return  this._axisXIndex; }
            set
            {
                if (value != this.AxisXIndex)
                {
                    this._axisXIndex = value;
                    base.NotifyChartChanged("AxisXIndex");
                }
            }
        }

        internal int AxisYIndex
        {
            get { return  this._axisYIndex; }
            set
            {
                if (value != this.AxisYIndex)
                {
                    this._axisYIndex = value;
                    base.NotifyChartChanged("AxisYIndex");
                }
            }
        }

        internal SpreadChart Chart
        {
            get { return ((ISpreadChartElement)this).Chart as SpreadChart; }
            set { ((ISpreadChartElement)this).Chart = value; }
        }

        internal override Dt.Cells.Data.ChartArea ChartArea
        {
            get { return  Dt.Cells.Data.ChartArea.DataSeries; }
        }

        /// <summary>
        /// Gets or sets the type of the chart.
        /// </summary>
        /// <value>
        /// The type of the chart.
        /// </value>
        public SpreadChartType ChartType
        {
            get
            {
                if (this._chartType.HasValue)
                {
                    return this._chartType.Value;
                }
                if (this.Chart != null)
                {
                    return this.Chart.ChartType;
                }
                return SpreadChartType.None;
            }
            set
            {
                SpreadChartType type = value;
                SpreadChartType? nullable = this._chartType;
                if ((type != ((SpreadChartType) nullable.GetValueOrDefault())) || !nullable.HasValue)
                {
                    if (!this.CanChangeChartType(value))
                    {
                        throw new NotSupportedException(string.Format("Can not change dataseries to {0}", (object[]) new object[] { value }));
                    }
                    this._chartType = new SpreadChartType?(value);
                    this.NotifyDataSeriesChanged("ChartType");
                }
            }
        }

        /// <summary>
        /// Gets or sets the data label settings.
        /// </summary>
        public Dt.Cells.Data.DataLabelSettings DataLabelSettings
        {
            get
            {
                if (this._dataLabelSettings == null)
                {
                    this._dataLabelSettings = new Dt.Cells.Data.DataLabelSettings();
                    this._dataLabelSettings.PropertyChanged += new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
                }
                return this._dataLabelSettings;
            }
            set
            {
                if (value != this.DataLabelSettings)
                {
                    if (this._dataLabelSettings != null)
                    {
                        this._dataLabelSettings.PropertyChanged -= new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
                    }
                    this._dataLabelSettings = value;
                    if (this._dataLabelSettings != null)
                    {
                        this._dataLabelSettings.PropertyChanged += new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the style of the data label.
        /// </summary>
        /// <value>
        /// The style of the data label.
        /// </value>
        public ChartLabelStyleInfo DataLabelStyle
        {
            get
            {
                if (this._dataLabelStyleInfo == null)
                {
                    this._dataLabelStyleInfo = new ChartLabelStyleInfo(this);
                }
                return this._dataLabelStyleInfo;
            }
        }

        /// <summary>
        /// Gets the style of the data marker.
        /// </summary>
        /// <value>
        /// The style of the data marker.
        /// </value>
        public ChartSymbolStyleInfo DataMarkerStyle
        {
            get
            {
                if (this._markerStyleInfo == null)
                {
                    this._markerStyleInfo = new ChartSymbolStyleInfo(this);
                }
                return this._markerStyleInfo;
            }
            internal set { this._markerStyleInfo = value; }
        }

        internal bool DisplayHidden
        {
            get { return  this._displayHidden; }
            set
            {
                if (value != this.DisplayHidden)
                {
                    this._displayHidden = value;
                    this.RefreshValues();
                }
            }
        }

        internal Dt.Cells.Data.EmptyValueStyle EmptyValueStyle
        {
            get { return  this._emptyValueStyle; }
            set
            {
                if (value != this.EmptyValueStyle)
                {
                    this._emptyValueStyle = value;
                    this.RefreshValues();
                }
            }
        }

        internal ErrorBars FirstErrorBars { get; set; }

        /// <summary>
        /// Gets or sets the formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        internal IFormatter Formatter
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.Formatter;
                }
                return null;
            }
            set { base.StyleInfo.Formatter = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to display an alternative color if the value is negative.
        /// </summary>
        /// <value>
        /// <c>true</c> to display an alternative color if the value is negative; otherwise, <c>false</c>.
        /// </value>
        public bool InvertIfNegative
        {
            get { return  this._invertIfNegative; }
            set
            {
                this._invertIfNegativeSet = true;
                if (value != this.InvertIfNegative)
                {
                    this._invertIfNegative = value;
                    this.NotifyDataSeriesChanged("InvertIfNegative");
                }
            }
        }

        internal bool IsAutoName
        {
            get { return  (string.IsNullOrWhiteSpace(this._name) && (this.Names.Count == 0)); }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hidden; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsHidden
        {
            get
            {
                if (this.DisplayHidden)
                {
                    return false;
                }
                return ((this._values != null) && this._values.AreAllDataSeriesInVisible);
            }
        }

        /// <summary>
        /// Gets or sets the marker size in pixels.
        /// </summary>
        /// <value>
        /// The size of the marker.
        /// </value>
        public Windows.Foundation.Size MarkerSize
        {
            get { return  this._markerSize; }
            set
            {
                if (value != this.MarkerSize)
                {
                    this._markerSize = value;
                    this.NotifyDataSeriesChanged("MarkerSize");
                }
            }
        }

        /// <summary>
        /// Gets or sets the data point marker type.
        /// </summary>
        /// <value>
        /// The data point marker type.
        /// </value>
        public Dt.Cells.Data.MarkerType MarkerType
        {
            get { return  this._markerType; }
            set
            {
                if (value != this.MarkerType)
                {
                    this._markerType = value;
                    this.NotifyDataSeriesChanged("MarkerType");
                }
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                if (this.Names.Count != 0)
                {
                    object obj2 = this.Names[0];
                    string str = (obj2 != null) ? obj2.ToString() : string.Empty;
                    for (int i = 1; i < this.Names.Count; i++)
                    {
                        object obj3 = this.Names[i];
                        str = str + " " + ((obj3 != null) ? obj3.ToString() : string.Empty);
                    }
                    return str;
                }
                if (!string.IsNullOrEmpty(this._name))
                {
                    return this._name;
                }
                if (this.Chart != null)
                {
                    int index = this.Chart.DataSeries.IndexOf(this);
                    if (index != -1)
                    {
                        return (ResourceStrings.DefaultDataSeriesName + ((int) (index + 1)));
                    }
                }
                return string.Empty;
            }
            set
            {
                if (value != this._name)
                {
                    this._name = value;
                    if (this._names != null)
                    {
                        this._names.DataSeries = null;
                    }
                    this._nameFormula = null;
                    this.NotifyDataSeriesChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the series name formula
        /// </summary>
        /// <value>
        /// The series name formula.
        /// </value>
        /// <remarks>
        /// This property is not designed for persisting so you should save and re-use the value and access infrequently.
        /// </remarks>
        public string NameFormula
        {
            get { return  this._nameFormula; }
            set
            {
                if (value != this.NameFormula)
                {
                    this.UpdateNameReference(value);
                    this._nameFormula = value;
                }
            }
        }

        internal DataOrientation? NameOrientation
        {
            get { return  this._nameOrientation; }
        }

        internal SheetCellRange NameRange
        {
            get { return  this._nameRange; }
            private set
            {
                if (value != this.NameRange)
                {
                    if (value == null)
                    {
                        this._nameRange = null;
                        this._nameOrientation = 0;
                    }
                    else
                    {
                        if ((value.RowCount > 1) && (value.ColumnCount > 1))
                        {
                            throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
                        }
                        this._nameRange = value;
                        this._nameOrientation = new DataOrientation?((value.ColumnCount == 1) ? DataOrientation.Vertical : DataOrientation.Horizontal);
                    }
                }
            }
        }

        CalcExpression NameReference
        {
            get { return  this._nameReference; }
            set
            {
                if (value != this.NameReference)
                {
                    if (value == null)
                    {
                        this._nameReference = null;
                    }
                    else
                    {
                        this._nameReference = value;
                    }
                    this.NameRange = SpreadChartUtility.ExtractRange(this.Sheet, value);
                    if (this.Names.DataSeries != null)
                    {
                        this.Names.RefreshData();
                    }
                    else
                    {
                        this.Names.DataSeries = new NameDataSeries(this);
                    }
                    this.NotifyDataSeriesChanged("NameReference");
                }
            }
        }

        StringSeriesCollection Names
        {
            get { return  this._names; }
        }

        /// <summary>
        /// Gets or sets the fill brush for the negative value.
        /// </summary>
        /// <value>
        /// The fill brush for the negative value.
        /// </value>
        public Brush NegativeFill
        {
            get { return  this._negativeFill; }
            set
            {
                this._negativeFillThemeColor = null;
                this._negativeFillSet = true;
                if (value != this.NegativeFill)
                {
                    this._negativeFill = value;
                    this.NotifyDataSeriesChanged("NegativeFill");
                }
            }
        }

        /// <summary>
        /// Gets or sets the fill theme color for the negative value.
        /// </summary>
        /// <value>
        /// The fill theme color for the negative value.
        /// </value>
        public string NegativeFillThemeColor
        {
            get { return  this._negativeFillThemeColor; }
            set
            {
                this._negativeFill = null;
                this._negativeFillThemeColorSet = true;
                if (value != this.NegativeFillThemeColor)
                {
                    this._negativeFillThemeColor = value;
                    this.NotifyDataSeriesChanged("NegativeFillThemeColor");
                }
            }
        }

        internal Dt.Xls.Chart.PictureOptions PictureOptions { get; set; }

        internal Dt.Cells.Data.PlotArea PlotArea
        {
            get
            {
                if (this.Chart != null)
                {
                    int num = this.Chart.PlotAreaColumnDefinitions.Count;
                    int num2 = (this.AxisYIndex * num) + this.AxisXIndex;
                    return this.Chart.PlotAreas[num2];
                }
                return null;
            }
        }

        internal ErrorBars SecondErrorBars { get; set; }

        internal IFloatingObjectSheet Sheet
        {
            get
            {
                if (this.Chart == null)
                {
                    return null;
                }
                return this.Chart.Sheet;
            }
        }

        internal List<TrendLine> TrendLines
        {
            get
            {
                if (this._trendLines == null)
                {
                    this._trendLines = new List<TrendLine>();
                }
                return this._trendLines;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use secondary axis].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use secondary axis]; otherwise, <c>false</c>.
        /// </value>
        internal bool UseSecondaryAxis
        {
            get { return  this._useSecondaryAxis; }
            set
            {
                if (value != this.UseSecondaryAxis)
                {
                    this._useSecondaryAxis = value;
                    this.NotifyDataSeriesChanged("UseSecondaryAxis");
                }
            }
        }

        /// <summary>
        /// Gets or sets the formula reference for the series data.
        /// </summary>
        /// <value>
        /// The formula reference.
        /// </value>
        public string ValueFormula
        {
            get { return  this._valueFormula; }
            set
            {
                if (value != this.ValueFormula)
                {
                    this.UpdateValueReference(value);
                    this._valueFormula = value;
                }
            }
        }

        internal DataOrientation? ValueOrientation
        {
            get { return  this._valueOrientation; }
        }

        internal SheetCellRange[] ValueRange
        {
            get { return  this._valueRange; }
            private set
            {
                this._valueOrientation = this.ValidateSeriesRange(value, false);
                this._valueRange = value;
            }
        }

        CalcExpression ValueReference
        {
            get { return  this._valueReference; }
            set
            {
                if (value != this.ValueReference)
                {
                    this._valueReference = value;
                    if (this._valueReference != null)
                    {
                        this.ValueRange = SheetCellRangeUtility.ExtractAllExternalReference(this.Sheet, this._valueReference);
                    }
                    else
                    {
                        this.ValueRange = null;
                    }
                    if (this.Values.DataSeries != null)
                    {
                        this.Values.RefreshData();
                    }
                    else
                    {
                        this.Values.DataSeries = new ValueDataSeries(this);
                    }
                    this.UpdateFormatter();
                    this.NotifyDataSeriesChanged("Values");
                }
            }
        }

        /// <summary>
        /// Gets or sets the data values collection.
        /// </summary>
        /// <value>
        /// The data values collection.
        /// </value>
        public DoubleSeriesCollection Values
        {
            get { return  this._values; }
            set
            {
                if (value != this.Values)
                {
                    if (this._values != null)
                    {
                        this._values.DataSeries = null;
                        this._values.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.Values_CollectionChanged);
                    }
                    this._values = value;
                    this._valueFormula = null;
                    if (this._values != null)
                    {
                        this._values.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Values_CollectionChanged);
                    }
                    this.NotifyDataSeriesChanged("Values");
                }
            }
        }

        internal Worksheet ValuesWorksheet
        {
            get
            {
                if (this.ValueReference != null)
                {
                    CalcExternalExpression valueReference = this.ValueReference as CalcExternalExpression;
                    if (valueReference != null)
                    {
                        return (valueReference.Source as Worksheet);
                    }
                    CalcExternalNameExpression expression2 = this.ValueReference as CalcExternalNameExpression;
                    if (expression2 != null)
                    {
                        return (expression2.Source as Worksheet);
                    }
                }
                return null;
            }
        }

        class NameDataSeries : IDataSeries
        {
            SpreadDataSeries _dataSeries;

            public NameDataSeries(SpreadDataSeries dataSeries)
            {
                this._dataSeries = dataSeries;
            }

            public Dt.Cells.Data.DataOrientation? DataOrientation
            {
                get { return  this._dataSeries.NameOrientation; }
            }

            public CalcExpression DataReference
            {
                get { return  this._dataSeries.NameReference; }
            }

            public bool DisplayHiddenData
            {
                get { return  this._dataSeries.DisplayHidden; }
            }

            public Dt.Cells.Data.EmptyValueStyle EmptyValueStyle
            {
                get { return  this._dataSeries.EmptyValueStyle; }
            }

            public ICalcEvaluator Evaluator
            {
                get { return  this._dataSeries.Sheet; }
            }
        }

        class ValueDataSeries : IDataSeries
        {
            SpreadDataSeries _dataSeries;

            public ValueDataSeries(SpreadDataSeries dataSeries)
            {
                this._dataSeries = dataSeries;
            }

            public Dt.Cells.Data.DataOrientation? DataOrientation
            {
                get { return  this._dataSeries.ValueOrientation; }
            }

            public CalcExpression DataReference
            {
                get { return  this._dataSeries.ValueReference; }
            }

            public bool DisplayHiddenData
            {
                get { return  this._dataSeries.DisplayHidden; }
            }

            public Dt.Cells.Data.EmptyValueStyle EmptyValueStyle
            {
                get { return  this._dataSeries.EmptyValueStyle; }
            }

            public ICalcEvaluator Evaluator
            {
                get { return  this._dataSeries.Sheet; }
            }
        }
    }
}

