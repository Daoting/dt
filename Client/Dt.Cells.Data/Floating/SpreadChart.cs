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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a chart instance in Spread.
    /// </summary>
    public class SpreadChart : SpreadChartBase, IDisposable, ICloneable
    {
        Wall _backWall;
        Dt.Cells.Data.BubbleChartSettings _bubbleChartSettings;
        SpreadChartType _chartType;
        string _dataFormula;
        Dt.Cells.Data.DataLabelSettings _dataLabelSettings;
        Dt.Cells.Data.DataOrientation? _dataOrientation;
        Dt.Cells.Data.DataSeriesSettings _dataSeriesSettings;
        Dt.Cells.Data.DataTableSettings _dataTableSettings;
        int _defaultStyleIndex;
        EmptyValueStyle _displayEmptyCellsAs;
        bool _displayHidden;
        IList<SpreadDataSeries> _displayingDataSeries;
        Wall _floorWall;
        string _formula;
        string _itemsFormula;
        Dt.Cells.Data.Legend _legend;
        string _nameFormula;
        Dt.Cells.Data.PieChartSettings _pieChartSettings;
        ObservableCollection<PlotAreaColumnDefinition> _plotAreaColumnDefinitions;
        ObservableCollection<PlotAreaRowDefinition> _plotAreaRowDefinitions;
        PlotAreaCollection _plotAreas;
        IExcelChart _secondaryChart;
        DataSeriesCollection _series;
        bool _showAutoTitle;
        bool _showDataLabelOverMax;
        Wall _sideWall;
        bool _varyColorsByPoint;
        View3DSettings _view3D;
        AxisCollection _xAxises;
        AxisCollection _yAxises;
        AxisCollection _zAxises;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChart" /> class.
        /// </summary>
        public SpreadChart() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChart" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public SpreadChart(string name) : this(name, SpreadChartType.ColumnClustered, null, null, null, 0.0, 0.0, 300.0, 300.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChart" /> class.
        /// </summary>
        /// <param name="name">The name of the chart.</param>
        /// <param name="type">The type of the chart.</param>
        /// <param name="x">The x location of the chart.</param>
        /// <param name="y">The y location of the chart.</param>
        /// <param name="width">The width of the chart.</param>
        /// <param name="height">The height of the chart.</param>
        public SpreadChart(string name, SpreadChartType type, double x, double y, double width, double height) : base(name, x, y, width, height)
        {
            this._chartType = SpreadChartType.ColumnClustered;
            this._dataOrientation = null;
            this._showAutoTitle = true;
            this._defaultStyleIndex = -1;
            this.Init(name, type, null, null, null, null, x, y, width, height, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChart" /> class.
        /// </summary>
        /// <param name="name">The name of the chart.</param>
        /// <param name="type">The type of the chart.</param>
        /// <param name="formula">The formula for the chart.</param>
        /// <param name="x">The x location of the chart.</param>
        /// <param name="y">The y location of the chart.</param>
        /// <param name="width">The width of the chart.</param>
        /// <param name="height">The height of the chart.</param>
        /// <remarks>
        /// The formula parameter is for the chart data. The type parameter indicates which type of chart will be added.
        /// </remarks>
        public SpreadChart(string name, SpreadChartType type, string formula, double x, double y, double width, double height) : base(name, x, y, width, height)
        {
            this._chartType = SpreadChartType.ColumnClustered;
            this._dataOrientation = null;
            this._showAutoTitle = true;
            this._defaultStyleIndex = -1;
            this.Init(name, type, formula, null, null, null, x, y, width, height, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChart" /> class.
        /// </summary>
        /// <param name="name">The name of the chart.</param>
        /// <param name="type">The type of the chart.</param>
        /// <param name="nameFormula">The name formula for the chart.</param>
        /// <param name="itemsFormula">The items formula for the chart.</param>
        /// <param name="dataFormula">The data formula for the chart.</param>
        /// <param name="x">The x location of the chart.</param>
        /// <param name="y">The y location of the chart.</param>
        /// <param name="width">The width of the chart.</param>
        /// <param name="height">The height of the chart.</param>
        /// <remarks>
        /// The itemsFormula is for the axis title. The nameFormula is for the names of the series (SeriesName).  
        /// The dataFormula is for the actual data in each series. The type parameter indicates which type of chart will be added.
        /// </remarks>
        public SpreadChart(string name, SpreadChartType type, string itemsFormula, string nameFormula, string dataFormula, double x, double y, double width, double height) : base(name, x, y, width, height)
        {
            this._chartType = SpreadChartType.ColumnClustered;
            this._dataOrientation = null;
            this._showAutoTitle = true;
            this._defaultStyleIndex = -1;
            this.Init(name, type, null, itemsFormula, nameFormula, dataFormula, x, y, width, height, true);
        }

        internal override void AfterReadXml()
        {
            base.AfterReadXml();
            this.UpdateAxisesAutoType();
            this.UpdateDisplayingDataSeries();
            this.UpdateAxisesTypeBySeriesValues();
            this._series.ResumeEvent();
            this._xAxises.ResumeEvent();
            this._yAxises.ResumeEvent();
        }

        bool AreValuesDateTime(Worksheet worksheet, CellRange cellRange)
        {
            if (cellRange == null)
            {
                return false;
            }
            for (int i = cellRange.Row; i < (cellRange.Row + cellRange.RowCount); i++)
            {
                for (int j = cellRange.Column; j < (cellRange.Column + cellRange.ColumnCount); j++)
                {
                    if (!(worksheet.GetValue(i, j) is DateTime))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        bool AreXValueFormulaSame(out string xValueFormua)
        {
            xValueFormua = null;
            if (this.DataSeries.Count != 0)
            {
                SpreadXYDataSeries series = this.DataSeries[0] as SpreadXYDataSeries;
                if (series != null)
                {
                    xValueFormua = series.XValueFormula;
                }
                else
                {
                    return false;
                }
                for (int i = 1; i < this.DataSeries.Count; i++)
                {
                    series = this.DataSeries[i] as SpreadXYDataSeries;
                    if (series == null)
                    {
                        return false;
                    }
                    if (series.XValueFormula != xValueFormua)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        Dictionary<int, List<SheetCellRange>> ArrangeRangesByOrientation(IEnumerable<SheetCellRange> ranges)
        {
            Dictionary<int, List<SheetCellRange>> dictionary = new Dictionary<int, List<SheetCellRange>>();
            if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
            {
                foreach (SheetCellRange range in ranges)
                {
                    if (!dictionary.ContainsKey(range.Column))
                    {
                        dictionary.Add(range.Column, new List<SheetCellRange>());
                    }
                    if (!dictionary[range.Column].Contains(range))
                    {
                        dictionary[range.Column].Add(range);
                    }
                }
                return dictionary;
            }
            foreach (SheetCellRange range2 in ranges)
            {
                if (!dictionary.ContainsKey(range2.Row))
                {
                    dictionary.Add(range2.Row, new List<SheetCellRange>());
                }
                if (!dictionary[range2.Row].Contains(range2))
                {
                    dictionary[range2.Row].Add(range2);
                }
            }
            return dictionary;
        }

        internal override void BeforeReadXml()
        {
            base.BeforeReadXml();
            this.Init(string.Empty, SpreadChartType.ColumnClustered, string.Empty, string.Empty, string.Empty, string.Empty, 0.0, 0.0, 200.0, 200.0, false);
            this._series.SuspendEvent();
            this._xAxises.SuspendEvent();
            this._yAxises.SuspendEvent();
        }

        void BubbleChartSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyChartAreaChanged(ChartArea.Chart, "BubbleChartSettings");
        }

        SpreadDataSeries CreateDataSeries()
        {
            SpreadDataSeries series = null;
            switch (this.ChartType)
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
                    series = new SpreadDataSeries();
                    break;

                case SpreadChartType.Scatter:
                case SpreadChartType.ScatterLines:
                case SpreadChartType.ScatterLinesWithMarkers:
                case SpreadChartType.ScatterLinesSmoothed:
                case SpreadChartType.ScatterLinesSmoothedWithMarkers:
                    series = new SpreadXYDataSeries();
                    break;

                case SpreadChartType.Bubble:
                    series = new SpreadBubbleSeries();
                    break;

                case SpreadChartType.StockHighLowOpenClose:
                    series = new SpreadOpenHighLowCloseSeries();
                    break;

                default:
                    series = new SpreadDataSeries();
                    break;
            }
            series.DisplayHidden = this.DisplayHidden;
            return series;
        }

        void DataLabelSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyChartAreaChanged(ChartArea.DataLabel, "DataLabel");
        }

        void DataSeriesSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyChartAreaChanged(ChartArea.Chart, "DataSeriesSettings");
        }

        void DetectFormulas(string formula, out string dataFormula, out string nameFormula, out string categoryFormula)
        {
            dataFormula = null;
            nameFormula = null;
            categoryFormula = null;
            formula = FormulaUtility.ProcessFormula(formula);
            SheetCellRange[] ranges = SheetCellRangeUtility.ExtractAllExternalReference(base.Sheet, formula);
            if ((!string.IsNullOrEmpty(formula) && (base.Sheet != null)) && ((ranges == null) || (ranges.Length == 0)))
            {
                throw new ArgumentException("The formula is invalid!");
            }
            this.DetectFormulas(ranges, out dataFormula, out nameFormula, out categoryFormula);
        }

        /// <summary>
        /// 如果ranges都在同一个strip上对齐，则每块range作为DataRange，否则只有第二块后所有range都要生成namerange和datarange.
        /// </summary>
        /// <param name="ranges">Ranges from Formula</param>
        /// <param name="dataFormula"></param>
        /// <param name="nameFormula"></param>
        /// <param name="categoryFormula"></param>
        void DetectFormulas(SheetCellRange[] ranges, out string dataFormula, out string nameFormula, out string categoryFormula)
        {
            CellRange range2;
            CellRange range3;
            CellRange range4;
            SheetCellRange range7;
            dataFormula = string.Empty;
            nameFormula = string.Empty;
            categoryFormula = string.Empty;
            if ((ranges == null) || (ranges.Length == 0))
            {
                return;
            }
            SheetCellRange range = ranges[0];
            bool flag = SpreadChartUtility.DetectRanges(range.Sheet, range.Row, range.Column, range.RowCount, range.ColumnCount, out range2, out range3, out range4);
            if (!this._dataOrientation.HasValue)
            {
                bool flag2 = this.AreValuesDateTime(range.Sheet, range3);
                bool flag3 = this.AreValuesDateTime(range.Sheet, range2);
                if ((!flag2 && !flag3) || flag2)
                {
                    this._dataOrientation = new Dt.Cells.Data.DataOrientation?(flag ? Dt.Cells.Data.DataOrientation.Vertical : Dt.Cells.Data.DataOrientation.Horizontal);
                }
                else
                {
                    CellRange range5 = range3;
                    range3 = range2;
                    range2 = range5;
                    this._dataOrientation = new Dt.Cells.Data.DataOrientation?(flag ? Dt.Cells.Data.DataOrientation.Horizontal : Dt.Cells.Data.DataOrientation.Vertical);
                }
            }
            else
            {
                Dt.Cells.Data.DataOrientation? nullable = this._dataOrientation;
                if (!((((Dt.Cells.Data.DataOrientation) nullable.GetValueOrDefault()) == Dt.Cells.Data.DataOrientation.Vertical) && nullable.HasValue) || flag)
                {
                    Dt.Cells.Data.DataOrientation? nullable2 = this._dataOrientation;
                    if (!((((Dt.Cells.Data.DataOrientation) nullable2.GetValueOrDefault()) == Dt.Cells.Data.DataOrientation.Horizontal) && nullable2.HasValue) || !flag)
                    {
                        goto Label_0102;
                    }
                }
                CellRange range6 = range3;
                range3 = range2;
                range2 = range6;
            }
        Label_0102:
            range7 = null;
            if (range3 != null)
            {
                range7 = new SheetCellRange(range.Sheet, range3.Row, range3.Column, range3.RowCount, range3.ColumnCount);
            }
            if (range4 != null)
            {
                SheetCellRange range8 = new SheetCellRange(range.Sheet, range4.Row, range4.Column, range4.RowCount, range4.ColumnCount);
                SheetCellRange range9 = null;
                if (range2 != null)
                {
                    range9 = new SheetCellRange(range.Sheet, range2.Row, range2.Column, range2.RowCount, range2.ColumnCount);
                }
                if (ranges.Length > 1)
                {
                    List<SheetCellRange> list = new List<SheetCellRange>();
                    List<SheetCellRange> list2 = new List<SheetCellRange>();
                    List<SheetCellRange> nameRanges = new List<SheetCellRange>();
                    if (this.InSameColumnRowStrip(ranges))
                    {
                        if (range7 != null)
                        {
                            list.Add(range7);
                        }
                        if (range9 != null)
                        {
                            nameFormula = SheetCellRangeUtility.SheetRange2Formula(base.Sheet, range9);
                        }
                        list2.Add(range8);
                        for (int i = 1; i < ranges.Length; i++)
                        {
                            if (range7 != null)
                            {
                                if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
                                {
                                    SheetCellRange range10 = new SheetCellRange(ranges[i].Sheet, ranges[i].Row, ranges[i].Column, ranges[i].RowCount, 1);
                                    SheetCellRange range11 = new SheetCellRange(ranges[i].Sheet, ranges[i].Row, ranges[i].Column + 1, ranges[i].RowCount, ranges[i].ColumnCount - 1);
                                    list.Add(range10);
                                    list2.Add(range11);
                                }
                                else
                                {
                                    SheetCellRange range12 = new SheetCellRange(ranges[i].Sheet, ranges[i].Row, ranges[i].Column, 1, ranges[i].ColumnCount);
                                    SheetCellRange range13 = new SheetCellRange(ranges[i].Sheet, ranges[i].Row + 1, ranges[i].Column, ranges[i].RowCount - 1, ranges[i].ColumnCount);
                                    list.Add(range12);
                                    list2.Add(range13);
                                }
                            }
                            else
                            {
                                list2.Add(ranges[i]);
                            }
                        }
                        categoryFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list);
                        dataFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list2);
                    }
                    else
                    {
                        if (range7 != null)
                        {
                            categoryFormula = SheetCellRangeUtility.SheetRange2Formula(base.Sheet, range7);
                        }
                        list2.Add(range8);
                        this.GenerateOtherNameDataRanges(ranges, range9, nameRanges, list2);
                        if (range2 != null)
                        {
                            nameFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, nameRanges);
                        }
                        dataFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list2);
                    }
                }
                else
                {
                    if (range7 != null)
                    {
                        categoryFormula = SheetCellRangeUtility.SheetRange2Formula(base.Sheet, range7);
                    }
                    if (range9 != null)
                    {
                        nameFormula = SheetCellRangeUtility.SheetRange2Formula(base.Sheet, range9);
                    }
                    dataFormula = SheetCellRangeUtility.SheetRange2Formula(base.Sheet, range8);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            using (IEnumerator<Axis> enumerator = this._xAxises.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Dispose();
                }
            }
            using (IEnumerator<Axis> enumerator2 = this._yAxises.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.Dispose();
                }
            }
            using (IEnumerator<Axis> enumerator3 = this._zAxises.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.Dispose();
                }
            }
            using (IEnumerator<SpreadDataSeries> enumerator4 = this._series.GetEnumerator())
            {
                while (enumerator4.MoveNext())
                {
                    enumerator4.Current.Dispose();
                }
            }
        }

        void GenerateOtherNameDataRanges(SheetCellRange[] ranges, SheetCellRange firstNameRange, List<SheetCellRange> nameRanges, List<SheetCellRange> dataRanges)
        {
            if (firstNameRange != null)
            {
                SheetCellRange range = new SheetCellRange(firstNameRange.Sheet, firstNameRange.Row, firstNameRange.Column, firstNameRange.RowCount, firstNameRange.ColumnCount);
                nameRanges.Add(range);
            }
            for (int i = 1; i < ranges.Length; i++)
            {
                SheetCellRange range2 = ranges[i];
                if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
                {
                    if (range2.RowCount != 1)
                    {
                        if (firstNameRange != null)
                        {
                            SheetCellRange range3 = new SheetCellRange(range2.Sheet, range2.Row, range2.Column, firstNameRange.RowCount, range2.ColumnCount);
                            SheetCellRange range4 = new SheetCellRange(range2.Sheet, range2.Row + firstNameRange.RowCount, range2.Column, range2.RowCount - firstNameRange.RowCount, range2.ColumnCount);
                            nameRanges.Add(range3);
                            dataRanges.Add(range4);
                        }
                        else
                        {
                            dataRanges.Add(range2);
                        }
                    }
                }
                else if (range2.ColumnCount != 1)
                {
                    if (firstNameRange != null)
                    {
                        SheetCellRange range5 = new SheetCellRange(range2.Sheet, range2.Row, range2.Column, range2.RowCount, firstNameRange.ColumnCount);
                        SheetCellRange range6 = new SheetCellRange(range2.Sheet, range2.Row, range2.Column + firstNameRange.ColumnCount, range2.RowCount, range2.ColumnCount - firstNameRange.ColumnCount);
                        nameRanges.Add(range5);
                        dataRanges.Add(range6);
                    }
                    else
                    {
                        dataRanges.Add(range2);
                    }
                }
            }
        }

        List<List<SheetCellRange>> GetAllDataRanges(bool includXValueRange)
        {
            List<List<SheetCellRange>> list = new List<List<SheetCellRange>>();
            if (this.DataSeries.Count != 0)
            {
                using (IEnumerator<SpreadDataSeries> enumerator = this.DataSeries.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        List<List<SheetCellRange>> dataRanges = enumerator.Current.GetDataRanges();
                        if (!includXValueRange)
                        {
                            dataRanges.RemoveAt(0);
                        }
                        list.AddRange((IEnumerable<List<SheetCellRange>>) dataRanges);
                    }
                }
            }
            return list;
        }

        void GetAxisMinMaxValue(Axis axis, XYZValues xyzValues, out double minValue, out double maxValue)
        {
            minValue = axis.Min;
            maxValue = axis.Max;
            if (axis.AutoMin || axis.AutoMax)
            {
                if (xyzValues == XYZValues.XValues)
                {
                    this.GetXValuesMinMaxValue(axis, out minValue, out maxValue);
                }
                else if (xyzValues == XYZValues.YValues)
                {
                    this.GetYValuesMinMaxValue(axis, out minValue, out maxValue);
                }
            }
        }

        bool GetCanSwitchRowColumn()
        {
            List<SheetCellRange> chartAllRanges = this.GetChartAllRanges();
            if ((chartAllRanges == null) || (chartAllRanges.Count == 0))
            {
                return false;
            }
            SheetCellRange[] ranges = chartAllRanges.ToArray();
            List<SheetCellRange> list2 = null;
            if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
            {
                list2 = SheetCellRangeUtility.SplitToColumnRanges(ranges, 1);
            }
            else
            {
                list2 = SheetCellRangeUtility.SplitToRowRanges(ranges, 1);
            }
            List<List<SheetCellRange>> list3 = new List<List<SheetCellRange>>((IEnumerable<List<SheetCellRange>>) SpreadChartUtility.ArrangeRangesByOrientation((IEnumerable<SheetCellRange>) list2, this.DataOrientation).Values);
            return SpreadChartUtility.AreSegmentsAligned(list3, this.DataOrientation);
        }

        List<SheetCellRange> GetChartAllRanges()
        {
            SheetCellRange[] itemsRange = this.GetItemsRange();
            List<SheetCellRange> list = new List<SheetCellRange>();
            foreach (SpreadDataSeries series in this.DataSeries)
            {
                List<List<SheetCellRange>> dataRanges = series.GetDataRanges();
                if ((dataRanges != null) && (dataRanges.Count > 0))
                {
                    using (List<List<SheetCellRange>>.Enumerator enumerator2 = dataRanges.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            foreach (SheetCellRange range in enumerator2.Current)
                            {
                                if (!list.Contains(range))
                                {
                                    list.Add(range);
                                }
                            }
                        }
                        continue;
                    }
                }
                if (series.Values.Count > 0)
                {
                    return null;
                }
            }
            List<SheetCellRange> list4 = this.TryCombineSeriesNameRanges();
            if (list4 == null)
            {
                return null;
            }
            SheetCellRange range2 = null;
            SheetCellRange range3 = (list4.Count > 0) ? list4[0] : null;
            SheetCellRange range4 = ((itemsRange != null) && (itemsRange.Length > 0)) ? itemsRange[0] : null;
            SheetCellRange range5 = (list.Count > 0) ? list[0] : null;
            SheetCellRange range6 = null;
            if (range4 != null)
            {
                range6 = range4;
            }
            else
            {
                range6 = range5;
            }
            if ((range3 != null) && (range6 != null))
            {
                if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
                {
                    if (((range6.Column + range6.ColumnCount) == range3.Column) && ((range3.Row + range3.RowCount) == range6.Row))
                    {
                        range2 = new SheetCellRange(range3.Sheet, range3.Row, range6.Column, range3.RowCount, range6.ColumnCount);
                    }
                }
                else if (((range6.Row + range6.RowCount) == range3.Row) && ((range3.Column + range3.ColumnCount) == range6.Column))
                {
                    range2 = new SheetCellRange(range3.Sheet, range6.Row, range3.Column, range6.RowCount, range3.ColumnCount);
                }
            }
            List<SheetCellRange> list5 = new List<SheetCellRange>();
            if (range2 != null)
            {
                list5.Add(range2);
            }
            if ((itemsRange != null) && (itemsRange.Length > 0))
            {
                list5.AddRange(itemsRange);
            }
            if (list4.Count > 0)
            {
                list5.AddRange((IEnumerable<SheetCellRange>) list4);
            }
            if (list.Count > 0)
            {
                list5.AddRange((IEnumerable<SheetCellRange>) list);
            }
            return list5;
        }

        string GetDataFormula()
        {
            List<List<SheetCellRange>> ranges = this.TryCombineDataRanges();
            return SheetCellRangeUtility.BuildFormula(base.Sheet, ranges);
        }

        string GetFormula()
        {
            List<List<SheetCellRange>> ranges = this.TryCombineChartRange();
            return SheetCellRangeUtility.BuildFormula(base.Sheet, ranges);
        }

        string GetItemsFormula()
        {
            List<List<SheetCellRange>> ranges = this.TryCombineItemsRanges();
            if ((ranges != null) && (ranges.Count > 0))
            {
                return SheetCellRangeUtility.BuildFormula(base.Sheet, ranges);
            }
            if (SpreadChartUtility.IsBarChart(this.ChartType))
            {
                if (this.AxisY == null)
                {
                    return "";
                }
                return this.AxisY.ItemsFormula;
            }
            if (this.AxisX == null)
            {
                return "";
            }
            return this.AxisX.ItemsFormula;
        }

        SheetCellRange[] GetItemsRange()
        {
            if (SpreadChartUtility.IsBarChart(this.ChartType))
            {
                return this.AxisY.ItemsRange;
            }
            return this.AxisX.ItemsRange;
        }

        string GetNameFormula()
        {
            List<SheetCellRange> ranges = this.TryCombineSeriesNameRanges();
            return SheetCellRangeUtility.BuildFormula(base.Sheet, ranges);
        }

        int GetNumbersCount(double number)
        {
            return ((double) number).ToString().Length;
        }

        Dt.Cells.Data.DataOrientation GetSeriesRangeDataOrientation(SheetCellRange[] seriesRanges)
        {
            if (seriesRanges.Length != 0)
            {
                if ((seriesRanges.Length == 1) && (seriesRanges[0] != null))
                {
                    if (seriesRanges[0].ColumnCount != 1)
                    {
                        return Dt.Cells.Data.DataOrientation.Horizontal;
                    }
                    return Dt.Cells.Data.DataOrientation.Vertical;
                }
                bool flag = true;
                bool flag2 = true;
                foreach (SheetCellRange range in seriesRanges)
                {
                    if (range != null)
                    {
                        if (flag && (range.RowCount != 1))
                        {
                            flag = false;
                        }
                        if (flag2 && (range.ColumnCount != 1))
                        {
                            flag2 = false;
                        }
                    }
                }
                if (!flag2 || !flag)
                {
                    if (flag2)
                    {
                        return Dt.Cells.Data.DataOrientation.Vertical;
                    }
                    if (flag)
                    {
                        return Dt.Cells.Data.DataOrientation.Horizontal;
                    }
                }
            }
            return Dt.Cells.Data.DataOrientation.Vertical;
        }

        internal override IFloatingObjectSheet GetSheetFromOwner()
        {
            if (base.Owner != null)
            {
                return (base.Owner as SpreadCharts).Sheet;
            }
            return null;
        }

        IList<SpreadDataSeries> GetVisibleDataSeries()
        {
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            foreach (SpreadDataSeries series in this.DataSeries)
            {
                if (!series.IsHidden)
                {
                    list.Add(series);
                }
            }
            return (IList<SpreadDataSeries>) list;
        }

        void GetXValuesMinMaxValue(Axis axis, out double minValue, out double maxValue)
        {
            minValue = double.MaxValue;
            maxValue = double.MinValue;
            foreach (SpreadDataSeries series in this.DisplayingDataSeries)
            {
                if (series is SpreadOpenHighLowCloseSeries)
                {
                    SpreadOpenHighLowCloseSeries series2 = series as SpreadOpenHighLowCloseSeries;
                    if ((series2.XValues != null) && (series2.XValues.Count > 0))
                    {
                        foreach (double num in series2.XValues)
                        {
                            if (Utility.IsNumber(num))
                            {
                                minValue = Math.Min(num, minValue);
                                maxValue = Math.Max(num, maxValue);
                            }
                            else
                            {
                                minValue = 0.0;
                                maxValue = Math.Max((double) series2.OpenSeries.Values.Count, maxValue);
                                maxValue = Math.Max((double) series2.HighSeries.Values.Count, maxValue);
                                maxValue = Math.Max((double) series2.LowSeries.Values.Count, maxValue);
                                maxValue = Math.Max((double) series2.CloseSeries.Values.Count, maxValue);
                            }
                        }
                    }
                    else
                    {
                        minValue = 0.0;
                        maxValue = Math.Max((double) series2.OpenSeries.Values.Count, maxValue);
                        maxValue = Math.Max((double) series2.HighSeries.Values.Count, maxValue);
                        maxValue = Math.Max((double) series2.LowSeries.Values.Count, maxValue);
                        maxValue = Math.Max((double) series2.CloseSeries.Values.Count, maxValue);
                    }
                }
                else if (series is SpreadHighLowSeries)
                {
                    SpreadHighLowSeries series3 = series as SpreadHighLowSeries;
                    if ((series3.XValues != null) && (series3.XValues.Count > 0))
                    {
                        foreach (double num2 in series3.XValues)
                        {
                            if (Utility.IsNumber(num2))
                            {
                                minValue = Math.Min(num2, minValue);
                                maxValue = Math.Max(num2, maxValue);
                            }
                            else
                            {
                                minValue = 0.0;
                                maxValue = Math.Max((double) series3.HighSeries.Values.Count, maxValue);
                                maxValue = Math.Max((double) series3.LowSeries.Values.Count, maxValue);
                            }
                        }
                    }
                    else
                    {
                        minValue = 0.0;
                        maxValue = Math.Max((double) series3.HighSeries.Values.Count, maxValue);
                        maxValue = Math.Max((double) series3.LowSeries.Values.Count, maxValue);
                    }
                }
                else if (series is SpreadBubbleSeries)
                {
                    SpreadBubbleSeries series4 = series as SpreadBubbleSeries;
                    if ((series4.XValues != null) && (series4.XValues.Count > 0))
                    {
                        foreach (double num3 in series4.XValues)
                        {
                            if (Utility.IsNumber(num3))
                            {
                                minValue = Math.Min(num3, minValue);
                                maxValue = Math.Max(num3, maxValue);
                            }
                            else
                            {
                                minValue = 0.0;
                                maxValue = series4.XValues.Count;
                            }
                        }
                    }
                    else
                    {
                        minValue = 0.0;
                        maxValue = Math.Max(series4.Values.Count, series4.SizeValues.Count);
                    }
                }
                else if (series is SpreadXYDataSeries)
                {
                    SpreadXYDataSeries series5 = series as SpreadXYDataSeries;
                    if ((series5.XValues != null) && (series5.XValues.Count > 0))
                    {
                        foreach (double num4 in series5.XValues)
                        {
                            if (Utility.IsNumber(num4))
                            {
                                minValue = Math.Min(num4, minValue);
                                maxValue = Math.Max(num4, maxValue);
                            }
                            else
                            {
                                minValue = 0.0;
                                maxValue = series5.XValues.Count;
                            }
                        }
                    }
                    else
                    {
                        minValue = 0.0;
                        maxValue = series5.Values.Count;
                    }
                }
                else if (series != null)
                {
                    minValue = Math.Min(minValue, 0.0);
                    maxValue = Math.Max(maxValue, (double) series.Values.Count);
                }
            }
            if (axis.AxisType == AxisType.Value)
            {
                minValue = AxisUtility.CalculateValidMinimum(minValue, maxValue, false, axis.LogBase, axis.AutoMin, axis.AutoMax);
                maxValue = AxisUtility.CalculateValidMaximum(minValue, maxValue, false, axis.LogBase);
                minValue = AxisUtility.CalculateMinimum(minValue, maxValue, false, axis.LogBase);
                maxValue = AxisUtility.CalculateMaximum(minValue, maxValue, false, axis.LogBase);
            }
        }

        void GetYValuesMinMaxValue(Axis axis, out double minValue, out double maxValue)
        {
            minValue = double.MaxValue;
            maxValue = double.MinValue;
            if (SpreadChartUtility.IsStackedChart(this.ChartType))
            {
                int num = 0;
                using (IEnumerator<SpreadDataSeries> enumerator = this.DisplayingDataSeries.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        num = Math.Max(enumerator.Current.Values.Count, num);
                    }
                }
                double[] numArray = new double[num];
                double[] numArray2 = new double[num];
                for (int i = 0; i < num; i++)
                {
                    foreach (SpreadDataSeries series2 in this.DisplayingDataSeries)
                    {
                        if ((i < series2.Values.Count) && Utility.IsNumber(series2.Values[i]))
                        {
                            if (series2.Values[i] > 0.0)
                            {
                                numArray[i] += series2.Values[i];
                            }
                            else
                            {
                                numArray2[i] += series2.Values[i];
                            }
                        }
                    }
                }
                double[] numArray3 = numArray;
                for (int j = 0; j < numArray3.Length; j++)
                {
                    double num3 = (double) numArray3[j];
                    maxValue = Math.Max(maxValue, num3);
                }
                double[] numArray4 = numArray2;
                for (int k = 0; k < numArray4.Length; k++)
                {
                    double num4 = (double) numArray4[k];
                    minValue = Math.Min(minValue, num4);
                }
            }
            else
            {
                foreach (SpreadDataSeries series3 in this.DisplayingDataSeries)
                {
                    if (series3 is SpreadHighLowSeries)
                    {
                        SpreadHighLowSeries series4 = series3 as SpreadHighLowSeries;
                        foreach (double num5 in series4.HighSeries.Values)
                        {
                            if (Utility.IsNumber(num5))
                            {
                                minValue = Math.Min(num5, minValue);
                                maxValue = Math.Max(num5, maxValue);
                            }
                        }
                        foreach (double num6 in series4.LowSeries.Values)
                        {
                            if (Utility.IsNumber(num6))
                            {
                                minValue = Math.Min(num6, minValue);
                                maxValue = Math.Max(num6, maxValue);
                            }
                        }
                        if (series4 is SpreadOpenHighLowCloseSeries)
                        {
                            SpreadOpenHighLowCloseSeries series5 = series4 as SpreadOpenHighLowCloseSeries;
                            foreach (double num7 in series5.OpenSeries.Values)
                            {
                                if (Utility.IsNumber(num7))
                                {
                                    minValue = Math.Min(num7, minValue);
                                    maxValue = Math.Max(num7, maxValue);
                                }
                            }
                            foreach (double num8 in series5.CloseSeries.Values)
                            {
                                if (Utility.IsNumber(num8))
                                {
                                    minValue = Math.Min(num8, minValue);
                                    maxValue = Math.Max(num8, maxValue);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (double num9 in series3.Values)
                        {
                            if (Utility.IsNumber(num9))
                            {
                                minValue = Math.Min(num9, minValue);
                                maxValue = Math.Max(num9, maxValue);
                            }
                        }
                    }
                }
            }
            minValue = AxisUtility.CalculateValidMinimum(minValue, maxValue, false, axis.LogBase, axis.AutoMin, axis.AutoMax);
            maxValue = AxisUtility.CalculateValidMaximum(minValue, maxValue, false, axis.LogBase);
            minValue = AxisUtility.CalculateMinimum(minValue, maxValue, false, axis.LogBase);
            maxValue = AxisUtility.CalculateMaximum(minValue, maxValue, false, axis.LogBase);
        }

        bool HasTwoDimensionSeries()
        {
            using (IEnumerator<SpreadDataSeries> enumerator = this.DisplayingDataSeries.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is SpreadXYDataSeries)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        SheetCellRange InflateNameRange(SheetCellRange nameRange, SpreadDataSeries dataSeries)
        {
            SheetCellRange range = nameRange;
            List<List<SheetCellRange>> dataRanges = dataSeries.GetDataRanges();
            if ((dataRanges.Count > 0) && SpreadChartUtility.AreSegmentsAligned(dataRanges, this.DataOrientation))
            {
                if (dataSeries is SpreadBubbleSeries)
                {
                    SpreadBubbleSeries series = dataSeries as SpreadBubbleSeries;
                    if (((series.ValueRange != null) && (series.ValueRange.Length > 0)) && ((series.SizeRange != null) && (series.SizeRange.Length > 0)))
                    {
                        SheetCellRange range2 = series.SizeRange[0];
                        SheetCellRange range3 = series.ValueRange[0];
                        if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
                        {
                            if ((nameRange.Column == range3.Column) && (range2.Column == (range3.Column + 1)))
                            {
                                range = new SheetCellRange(nameRange.Sheet, nameRange.Row, nameRange.Column, nameRange.RowCount, nameRange.ColumnCount + 1);
                            }
                            return range;
                        }
                        if ((nameRange.Row == range3.Row) && (range2.Row == (range3.Row + 1)))
                        {
                            range = new SheetCellRange(nameRange.Sheet, nameRange.Row, nameRange.Column, nameRange.RowCount + 1, nameRange.ColumnCount);
                        }
                    }
                    return range;
                }
                if (!(dataSeries is SpreadXYZDataSeries))
                {
                    return range;
                }
                SpreadXYZDataSeries series2 = dataSeries as SpreadXYZDataSeries;
                if (((series2.ValueRange == null) || (series2.ValueRange.Length <= 0)) || ((series2.ZValueRange == null) || (series2.ZValueRange.Length <= 0)))
                {
                    return range;
                }
                SheetCellRange range4 = series2.ZValueRange[0];
                SheetCellRange range5 = series2.ValueRange[0];
                if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
                {
                    if ((nameRange.Column == range5.Column) && (range4.Column == (range5.Column + 1)))
                    {
                        range = new SheetCellRange(nameRange.Sheet, nameRange.Row, nameRange.Column, nameRange.RowCount, nameRange.ColumnCount + 1);
                    }
                    return range;
                }
                if ((nameRange.Row == range5.Row) && (range4.Row == (range5.Row + 1)))
                {
                    range = new SheetCellRange(nameRange.Sheet, nameRange.Row, nameRange.Column, nameRange.RowCount + 1, nameRange.ColumnCount);
                }
            }
            return range;
        }

        void Init(string name, SpreadChartType type, string formula, string itemsFormula, string nameFormula, string dataFormula, double x, double y, double width, double height, bool addDefaultElements)
        {
            base.SuspendEvents();
            base.Init(name, x, y, width, height);
            this._chartType = type;
            this._formula = formula;
            this._nameFormula = nameFormula;
            this._itemsFormula = itemsFormula;
            this._dataFormula = dataFormula;
            this._dataOrientation = null;
            this._series = new DataSeriesCollection(this, ChartArea.DataSeries);
            this._plotAreas = new PlotAreaCollection(this, ChartArea.PlotArea);
            if (addDefaultElements)
            {
                Dt.Cells.Data.PlotArea element = new Dt.Cells.Data.PlotArea(this);
                this._plotAreas.AddElementInternal(element);
            }
            else
            {
                this._plotAreas.ClearElementsInternal();
            }
            this._xAxises = new AxisCollection(this, ChartArea.AxisX);
            if (addDefaultElements)
            {
                Axis axis = new Axis(this, Dt.Cells.Data.AxisOrientation.X);
                if (((this._chartType == SpreadChartType.Radar) || (this._chartType == SpreadChartType.RadarFilled)) || ((this._chartType == SpreadChartType.RadarWithMarkers) || SpreadChartUtility.IsBarChart(type)))
                {
                    axis.ShowMajorGridlines = true;
                }
                else
                {
                    axis.ShowMajorGridlines = false;
                }
                axis.ShowMinorGridlines = false;
                this._xAxises.AddElementInternal(axis);
            }
            else
            {
                this._xAxises.ClearElementsInternal();
            }
            this._yAxises = new AxisCollection(this, ChartArea.AxisY);
            if (addDefaultElements)
            {
                Axis axis2 = new Axis(this, Dt.Cells.Data.AxisOrientation.Y);
                if (SpreadChartUtility.IsBarChart(type))
                {
                    axis2.ShowMajorGridlines = false;
                    axis2.ShowMinorGridlines = false;
                }
                else
                {
                    axis2.ShowMajorGridlines = true;
                    axis2.ShowMinorGridlines = false;
                }
                this._yAxises.AddElementInternal(axis2);
            }
            else
            {
                this._yAxises.ClearElementsInternal();
            }
            this._zAxises = new AxisCollection(this, ChartArea.AxisZ);
            if (addDefaultElements)
            {
                Axis item = new Axis(this, Dt.Cells.Data.AxisOrientation.Z);
                this._zAxises.Add(item);
            }
            else
            {
                this._zAxises.ClearElementsInternal();
            }
            if (addDefaultElements)
            {
                this.InitAxisesTypes(this._chartType);
            }
            if (addDefaultElements)
            {
                this._legend = new Dt.Cells.Data.Legend(this);
            }
            else
            {
                this._legend = null;
            }
            this._showAutoTitle = true;
            this._showDataLabelOverMax = false;
            this._dataLabelSettings = null;
            this._dataSeriesSettings = null;
            this._dataTableSettings = null;
            this.UpdateSettings();
            this.UpdateAxisesFormatter();
            if ((this._chartType == SpreadChartType.StockHighLowOpenClose) && (this.UpDownDarsSettings == null))
            {
                this.UpDownDarsSettings = new UpDownBars();
                this.UpDownDarsSettings.GapWidth = 150;
            }
            base.ResumeEvents();
        }

        void InitAxisesTypes(SpreadChartType chartType)
        {
            int dataDimension = SpreadChartUtility.GetDataDimension(chartType);
            if (dataDimension == 1)
            {
                this.AxisX.SetAutoAxisTypeInternal(AxisType.Category);
                this.AxisY.SetAutoAxisTypeInternal(AxisType.Value);
            }
            else if (dataDimension >= 2)
            {
                this.AxisX.SetAutoAxisTypeInternal(AxisType.Value);
                this.AxisY.SetAutoAxisTypeInternal(AxisType.Value);
            }
            this.AxisZ.SetAutoAxisTypeInternal(AxisType.Category);
        }

        bool InSameColumnRowStrip(SheetCellRange[] ranges)
        {
            int column = -1;
            int num2 = -1;
            if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
            {
                for (int i = 0; i < ranges.Length; i++)
                {
                    SheetCellRange range = ranges[i];
                    if (((column != -1) && (num2 != -1)) && ((range.Column != column) || (((range.Column + range.ColumnCount) - 1) != num2)))
                    {
                        return false;
                    }
                    column = range.Column;
                    num2 = (range.Column + range.ColumnCount) - 1;
                }
            }
            else
            {
                for (int j = 0; j < ranges.Length; j++)
                {
                    SheetCellRange range2 = ranges[j];
                    if (((column != -1) && (num2 != -1)) && ((range2.Row != column) || (((range2.Row + range2.RowCount) - 1) != num2)))
                    {
                        return false;
                    }
                    column = range2.Row;
                    num2 = (range2.Row + range2.RowCount) - 1;
                }
            }
            return true;
        }

        bool Is100PercentChart()
        {
            if (((this.ChartType != SpreadChartType.AreaStacked100pc) && (this.ChartType != SpreadChartType.BarStacked100pc)) && ((this.ChartType != SpreadChartType.ColumnStacked100pc) && (this.ChartType != SpreadChartType.LineStacked100pc)))
            {
                return (this.ChartType == SpreadChartType.LineStacked100pcWithMarkers);
            }
            return true;
        }

        bool NeedReGenerateAxisValues(string axisChanged)
        {
            if ((((axisChanged != "AxisType") && (axisChanged != "AutoMin")) && ((axisChanged != "AutoMax") && (axisChanged != "AutoMajorUnit"))) && (axisChanged != "AutoMinorUnit"))
            {
                return (axisChanged == "BaseTimeUnit");
            }
            return true;
        }

        internal override void NotifyChartAreaChanged(ChartArea chartArea, string changed)
        {
            if (chartArea == ChartArea.DataSeries)
            {
                this.UpdateDisplayingDataSeries();
                this.UpdateAxisesTypeBySeriesValues();
                this.UpdateAxisesFormatter();
                this.UpdataAxisMaxDataPointCount();
                if (string.IsNullOrEmpty(changed) || changed.ToLower().Contains("values"))
                {
                    this.UpdateAxisValuesOnDataSeriesChanged(changed);
                }
            }
            else if (chartArea == ChartArea.AxisX)
            {
                if (this.NeedReGenerateAxisValues(changed))
                {
                    this.UpdateAxisesFormatter();
                    this.UpdateAxisValuesOnDataSeriesChanged("XValues");
                }
            }
            else if (chartArea == ChartArea.AxisY)
            {
                if (this.NeedReGenerateAxisValues(changed))
                {
                    this.UpdateAxisesFormatter();
                    this.UpdateAxisValuesOnDataSeriesChanged("Values");
                }
            }
            else if (chartArea == ChartArea.AxisZ)
            {
                if (this.NeedReGenerateAxisValues(changed))
                {
                    this.UpdateAxisesFormatter();
                    this.UpdateAxisValuesOnDataSeriesChanged("ZValues");
                }
            }
            else if ((chartArea == ChartArea.Chart) && (((changed == "AxisX") || (changed == "AxisY")) || (changed == "AxisZ")))
            {
                this.UpdateAxisesAutoType();
                this.UpdateAxisesValues();
            }
            this.RaiseChartChanged(chartArea, changed);
        }

        internal override void OnAddColumns(int column, int columnCount)
        {
            if (this._series != null)
            {
                using (IEnumerator<SpreadDataSeries> enumerator = this._series.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ((IRangeSupport)(enumerator.Current)).AddColumns(column, columnCount);
                    }
                }
            }
            if (this._yAxises != null)
            {
                using (IEnumerator<Axis> enumerator2 = this._yAxises.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        ((IRangeSupport)(enumerator2.Current)).AddColumns(column, columnCount);
                    }
                }
            }
            if (this._xAxises != null)
            {
                using (IEnumerator<Axis> enumerator3 = this._xAxises.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        ((IRangeSupport)(enumerator3.Current)).AddColumns(column, columnCount);
                    }
                }
            }
        }

        internal override void OnAddRows(int row, int rowCount)
        {
            if (this._series != null)
            {
                using (IEnumerator<SpreadDataSeries> enumerator = this._series.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ((IRangeSupport)(enumerator.Current)).AddRows(row, rowCount);
                    }
                }
            }
            if (this._yAxises != null)
            {
                using (IEnumerator<Axis> enumerator2 = this._yAxises.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        ((IRangeSupport)(enumerator2.Current)).AddRows(row, rowCount);
                    }
                }
            }
            if (this._xAxises != null)
            {
                using (IEnumerator<Axis> enumerator3 = this._xAxises.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        ((IRangeSupport)(enumerator3.Current)).AddRows(row, rowCount);
                    }
                }
            }
        }

        internal override void OnOwnerChanged()
        {
            base.OnOwnerChanged();
            this.UpdateChartElements();
        }

        internal override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "IsSelected")
            {
                this.RaiseChartSelectionEvent();
            }
            this.RaiseChartChanged(ChartArea.Chart, propertyName);
        }

        internal override void OnRemoveColumns(int column, int columnCount)
        {
            if (this._series != null)
            {
                using (IEnumerator<SpreadDataSeries> enumerator = this._series.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ((IRangeSupport)(enumerator.Current)).RemoveColumns(column, columnCount);
                    }
                }
            }
            if (this._yAxises != null)
            {
                using (IEnumerator<Axis> enumerator2 = this._yAxises.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        ((IRangeSupport)(enumerator2.Current)).RemoveColumns(column, columnCount);
                    }
                }
            }
            if (this._xAxises != null)
            {
                using (IEnumerator<Axis> enumerator3 = this._xAxises.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        ((IRangeSupport)(enumerator3.Current)).RemoveColumns(column, columnCount);
                    }
                }
            }
        }

        internal override void OnRemoveRows(int row, int rowCount)
        {
            if (this._series != null)
            {
                using (IEnumerator<SpreadDataSeries> enumerator = this._series.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ((IRangeSupport)(enumerator.Current)).RemoveRows(row, rowCount);
                    }
                }
            }
            if (this._yAxises != null)
            {
                using (IEnumerator<Axis> enumerator2 = this._yAxises.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        ((IRangeSupport)(enumerator2.Current)).RemoveRows(row, rowCount);
                    }
                }
            }
            if (this._xAxises != null)
            {
                using (IEnumerator<Axis> enumerator3 = this._xAxises.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        ((IRangeSupport)(enumerator3.Current)).RemoveRows(row, rowCount);
                    }
                }
            }
        }

        internal override void OnResumeAfterDeserialization()
        {
            base.OnResumeAfterDeserialization();
            if (base.Sheet != null)
            {
                using (IEnumerator<SpreadDataSeries> enumerator = this.DataSeries.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.ResumeAfterDeserialization();
                    }
                }
                this.UpdataAxisMaxDataPointCount();
                if (this.AxisX != null)
                {
                    this.AxisX.ResumeAfterDeserialization();
                }
                if (this.AxisY != null)
                {
                    this.AxisY.ResumeAfterDeserialization();
                }
                if (this.AxisZ != null)
                {
                    this.AxisZ.ResumeAfterDeserialization();
                }
                if (base.ChartTitle != null)
                {
                    base.ChartTitle.ResumeAfterDeserialization();
                }
            }
        }

        void PieChartSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyChartAreaChanged(ChartArea.Chart, "PieChartSettings");
        }

        void ProcessDataOrientationChange(Dt.Cells.Data.DataOrientation newOrientation)
        {
            string formula = this.Formula;
            if (!string.IsNullOrEmpty(formula))
            {
                this._dataOrientation = new Dt.Cells.Data.DataOrientation?(newOrientation);
                this.UpdateFormula(formula);
            }
            base.RaisePropertyChanged("DataOrientation");
        }

        void RaiseChartChanged(ChartArea chartArea, string changed)
        {
            if (!base.IsEventSuspened() && (base.Worksheet != null))
            {
                base.Worksheet.RaiseChartChanged(this, chartArea, changed);
            }
        }

        void RaiseChartSelectionEvent()
        {
            if (!base.IsEventSuspened() && (base.Worksheet != null))
            {
                base.Worksheet.RaiseChartSelectionChangedEvent(this);
            }
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
                            this._chartType = (SpreadChartType) Serializer.DeserializeObj(typeof(SpreadChartType), reader);
                            this.UpdateUpDownDars();
                        }
                        catch
                        {
                            this._chartType = SpreadChartType.None;
                        }
                        return;

                    case "DataSeries":
                    {
                        Serializer.DeserializeList(this._series, reader);
                        using (IEnumerator<SpreadDataSeries> enumerator = this._series.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                enumerator.Current.SetChartInternal(this);
                            }
                            return;
                        }
                    }
                    case "XAxies":
                    {
                        Serializer.DeserializeList(this._xAxises, reader);
                        using (IEnumerator<Axis> enumerator2 = this._xAxises.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                enumerator2.Current.SetChartInternal(this);
                            }
                            return;
                        }
                    }
                    case "YAxies":
                    {
                        Serializer.DeserializeList(this._yAxises, reader);
                        using (IEnumerator<Axis> enumerator3 = this._yAxises.GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                enumerator3.Current.SetChartInternal(this);
                            }
                            return;
                        }
                    }
                    case "ZAxies":
                    {
                        Serializer.DeserializeList(this._zAxises, reader);
                        using (IEnumerator<Axis> enumerator4 = this._zAxises.GetEnumerator())
                        {
                            while (enumerator4.MoveNext())
                            {
                                enumerator4.Current.SetChartInternal(this);
                            }
                            return;
                        }
                    }
                    case "PlotAreas":
                    {
                        Serializer.DeserializeList(this._plotAreas, reader);
                        using (IEnumerator<Dt.Cells.Data.PlotArea> enumerator5 = this._plotAreas.GetEnumerator())
                        {
                            while (enumerator5.MoveNext())
                            {
                                enumerator5.Current.SetChartInternal(this);
                            }
                            return;
                        }
                    }
                    case "DataOrientation":
                        this._dataOrientation = new Dt.Cells.Data.DataOrientation?((Dt.Cells.Data.DataOrientation) Serializer.DeserializeObj(typeof(Dt.Cells.Data.DataOrientation), reader));
                        return;

                    case "DisplayEmptyCellsAs":
                        this._displayEmptyCellsAs = (EmptyValueStyle) Serializer.DeserializeObj(typeof(EmptyValueStyle), reader);
                        return;

                    case "DisplayHidden":
                        this._displayHidden = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "View3D":
                        if (this._view3D != null)
                        {
                            this._view3D.PropertyChanged -= new PropertyChangedEventHandler(this.View3D_PropertyChanged);
                        }
                        this._view3D = (View3DSettings) Serializer.DeserializeObj(typeof(View3DSettings), reader);
                        this._view3D.PropertyChanged += new PropertyChangedEventHandler(this.View3D_PropertyChanged);
                        return;

                    case "DataLabelSettings":
                        if (this._dataLabelSettings != null)
                        {
                            this._dataLabelSettings.PropertyChanged -= new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
                        }
                        this._dataLabelSettings = (Dt.Cells.Data.DataLabelSettings) Serializer.DeserializeObj(typeof(Dt.Cells.Data.DataLabelSettings), reader);
                        this._dataLabelSettings.PropertyChanged += new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
                        return;

                    case "VaryColorsByPoint":
                        this._varyColorsByPoint = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "Legend":
                        this._legend = (Dt.Cells.Data.Legend) Serializer.DeserializeObj(typeof(Dt.Cells.Data.Legend), reader);
                        this._legend.SetChartInternal(this);
                        return;

                    case "FloorWall":
                        this._floorWall = (Wall) Serializer.DeserializeObj(typeof(Wall), reader);
                        this._floorWall.SetChartInternal(this);
                        return;

                    case "SideWall":
                        this._sideWall = (Wall) Serializer.DeserializeObj(typeof(Wall), reader);
                        this._sideWall.SetChartInternal(this);
                        return;

                    case "BackWall":
                        this._backWall = (Wall) Serializer.DeserializeObj(typeof(Wall), reader);
                        this._backWall.SetChartInternal(this);
                        return;

                    case "ShowDataLabelOverMax":
                        this._showDataLabelOverMax = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "AutoTitle":
                        this._showDataLabelOverMax = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;
                }
            }
        }

        void RelLoadAxisItemsFormula()
        {
            this.AxisX.ReLoadItemsFormula();
            this.AxisY.ReLoadItemsFormula();
        }

        void ResetDataSeriesChartType()
        {
            using (IEnumerator<SpreadDataSeries> enumerator = this.DataSeries.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.ResetChartType();
                }
            }
        }

        internal override void ResetElementsFont()
        {
            base.ResetElementsFont();
            if (this.AxisX != null)
            {
                this.AxisX.ResetFontFamily();
                this.AxisX.ResetFontTheme();
                if (this.AxisX.Title != null)
                {
                    this.AxisX.Title.ResetFontFamily();
                    this.AxisX.Title.ResetFontTheme();
                }
            }
            if (this.AxisY != null)
            {
                this.AxisY.ResetFontFamily();
                this.AxisY.ResetFontTheme();
                if (this.AxisY.Title != null)
                {
                    this.AxisY.Title.ResetFontFamily();
                    this.AxisY.Title.ResetFontTheme();
                }
            }
            if (this.AxisZ != null)
            {
                this.AxisZ.ResetFontFamily();
                this.AxisZ.ResetFontTheme();
                if (this.AxisZ.Title != null)
                {
                    this.AxisZ.Title.ResetFontFamily();
                    this.AxisZ.Title.ResetFontTheme();
                }
            }
        }

        internal override void ResetElementsFontSize()
        {
            base.ResetElementsFontSize();
            if (this.AxisX != null)
            {
                this.AxisX.ResetFontSize();
                if (this.AxisX.Title != null)
                {
                    this.AxisX.Title.ResetFontSize();
                }
            }
            if (this.AxisY != null)
            {
                this.AxisY.ResetFontSize();
                if (this.AxisY.Title != null)
                {
                    this.AxisY.Title.ResetFontSize();
                }
            }
            if (this.AxisZ != null)
            {
                this.AxisZ.ResetFontSize();
                if (this.AxisZ.Title != null)
                {
                    this.AxisZ.Title.ResetFontSize();
                }
            }
        }

        internal override void ResetElementsFontStretch()
        {
            base.ResetElementsFontStretch();
            if (this.AxisX != null)
            {
                this.AxisX.ResetFontStretch();
                if (this.AxisX.Title != null)
                {
                    this.AxisX.Title.ResetFontStretch();
                }
            }
            if (this.AxisY != null)
            {
                this.AxisY.ResetFontStretch();
                if (this.AxisY.Title != null)
                {
                    this.AxisY.Title.ResetFontStretch();
                }
            }
            if (this.AxisZ != null)
            {
                this.AxisZ.ResetFontStretch();
                if (this.AxisZ.Title != null)
                {
                    this.AxisZ.Title.ResetFontStretch();
                }
            }
        }

        internal override void ResetElementsFontStyle()
        {
            base.ResetElementsFontStyle();
            if (this.AxisX != null)
            {
                this.AxisX.ResetFontStyle();
                if (this.AxisX.Title != null)
                {
                    this.AxisX.Title.ResetFontStyle();
                }
            }
            if (this.AxisY != null)
            {
                this.AxisY.ResetFontStyle();
                if (this.AxisY.Title != null)
                {
                    this.AxisY.Title.ResetFontStyle();
                }
            }
            if (this.AxisZ != null)
            {
                this.AxisZ.ResetFontStyle();
                if (this.AxisZ.Title != null)
                {
                    this.AxisZ.Title.ResetFontStyle();
                }
            }
        }

        internal override void ResetElementsFontWeight()
        {
            base.ResetElementsFontWeight();
            if (this.AxisX != null)
            {
                this.AxisX.ResetFontWeight();
                if (this.AxisX.Title != null)
                {
                    this.AxisX.Title.ResetFontWeight();
                }
            }
            if (this.AxisY != null)
            {
                this.AxisY.ResetFontWeight();
                if (this.AxisY.Title != null)
                {
                    this.AxisY.Title.ResetFontWeight();
                }
            }
            if (this.AxisZ != null)
            {
                this.AxisZ.ResetFontWeight();
                if (this.AxisZ.Title != null)
                {
                    this.AxisZ.Title.ResetFontWeight();
                }
            }
        }

        internal override void ResetElementsForeground()
        {
            base.ResetElementsForeground();
            if (this.AxisX != null)
            {
                this.AxisX.ResetForeground();
                this.AxisX.ResetForegroundThemeColor();
                if (this.AxisX.Title != null)
                {
                    this.AxisX.Title.ResetForeground();
                    this.AxisX.Title.ResetForegroundThemeColor();
                }
            }
            if (this.AxisY != null)
            {
                this.AxisY.ResetForeground();
                this.AxisY.ResetForegroundThemeColor();
                if (this.AxisY.Title != null)
                {
                    this.AxisY.Title.ResetForeground();
                    this.AxisY.Title.ResetForegroundThemeColor();
                }
            }
            if (this.AxisZ != null)
            {
                this.AxisZ.ResetForeground();
                this.AxisZ.ResetForegroundThemeColor();
                if (this.AxisZ.Title != null)
                {
                    this.AxisZ.Title.ResetForeground();
                    this.AxisZ.Title.ResetForegroundThemeColor();
                }
            }
            if (this.DataSeries.Count > 0)
            {
                foreach (SpreadDataSeries series in this.DataSeries)
                {
                    series.ResetForeground();
                    series.ResetForegroundThemeColor();
                }
            }
        }

        internal void SetSecondaryChart(IExcelChart chart)
        {
            if (chart != null)
            {
                if (this._secondaryChart == null)
                {
                    this._secondaryChart = chart;
                }
                else
                {
                    this.SecondaryChart.BarChart = chart.BarChart;
                    this.SecondaryChart.Bar3DChart = chart.Bar3DChart;
                    this.SecondaryChart.AreaChart = chart.AreaChart;
                    this.SecondaryChart.Area3DChart = chart.Area3DChart;
                    this.SecondaryChart.BubbleChart = chart.BubbleChart;
                    this.SecondaryChart.LineChart = chart.LineChart;
                    this.SecondaryChart.Line3DChart = chart.Line3DChart;
                    this.SecondaryChart.PieChart = chart.PieChart;
                    this.SecondaryChart.Pie3DChart = chart.Pie3DChart;
                    this.SecondaryChart.OfPieChart = chart.OfPieChart;
                    this.SecondaryChart.DoughnutChart = chart.DoughnutChart;
                    this.SecondaryChart.SurfaceChart = chart.SurfaceChart;
                    this.SecondaryChart.Surface3DChart = chart.Surface3DChart;
                    this.SecondaryChart.StockChart = chart.StockChart;
                    this.SecondaryChart.ScatterChart = chart.ScatterChart;
                    this.SecondaryChart.RadarChart = chart.RadarChart;
                }
            }
        }

        bool SupportCategoryFormula()
        {
            return (SpreadChartUtility.GetDataDimension(this.ChartType) == 1);
        }

        void SwitchItemsFormula(SpreadChartType oldChartType, SpreadChartType newChartType)
        {
            if ((((oldChartType == SpreadChartType.ColumnClustered) || (oldChartType == SpreadChartType.ColumnStacked)) || (oldChartType == SpreadChartType.ColumnStacked100pc)) && (((newChartType == SpreadChartType.BarClustered) || (newChartType == SpreadChartType.BarStacked)) || (newChartType == SpreadChartType.BarStacked100pc)))
            {
                if (!string.IsNullOrEmpty(this.AxisX.ItemsFormula))
                {
                    string itemsFormula = this.AxisY.ItemsFormula;
                    this.AxisY.ItemsFormula = this.AxisX.ItemsFormula;
                    this.AxisX.ItemsFormula = itemsFormula;
                }
            }
            else if (((((oldChartType == SpreadChartType.BarClustered) || (oldChartType == SpreadChartType.BarStacked)) || (oldChartType == SpreadChartType.BarStacked100pc)) && (((newChartType == SpreadChartType.ColumnClustered) || (newChartType == SpreadChartType.ColumnStacked)) || (newChartType == SpreadChartType.ColumnStacked100pc))) && !string.IsNullOrEmpty(this.AxisY.ItemsFormula))
            {
                string str2 = this.AxisY.ItemsFormula;
                this.AxisX.ItemsFormula = this.AxisY.ItemsFormula;
                this.AxisY.ItemsFormula = str2;
            }
        }

        /// <summary>
        /// Switches the row and column of the chart.
        /// </summary>
        /// <returns>If the switching process is successful, returns true; otherwise, returns false.</returns>
        public bool SwitchRowColumn()
        {
            if (!this.CanSwitchRowColumn)
            {
                return false;
            }
            if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
            {
                this.DataOrientation = Dt.Cells.Data.DataOrientation.Horizontal;
            }
            else
            {
                this.DataOrientation = Dt.Cells.Data.DataOrientation.Vertical;
            }
            return true;
        }

        List<List<SheetCellRange>> TryCombineChartRange()
        {
            List<SheetCellRange> chartAllRanges = this.GetChartAllRanges();
            if ((chartAllRanges == null) || (chartAllRanges.Count == 0))
            {
                return new List<List<SheetCellRange>>();
            }
            List<SheetCellRange> list2 = null;
            if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
            {
                list2 = SheetCellRangeUtility.SplitToColumnRanges(chartAllRanges.ToArray(), 1);
            }
            else
            {
                list2 = SheetCellRangeUtility.SplitToRowRanges(chartAllRanges.ToArray(), 1);
            }
            List<List<SheetCellRange>> ranges = new List<List<SheetCellRange>>((IEnumerable<List<SheetCellRange>>) this.ArrangeRangesByOrientation((IEnumerable<SheetCellRange>) list2).Values);
            bool flag = SpreadChartUtility.AreSegmentsAligned(ranges, this.DataOrientation);
            List<List<SheetCellRange>> list4 = new List<List<SheetCellRange>>();
            if (!flag)
            {
                return list4;
            }
            return FormulaUtility.TryCombineSeriesRange(ranges, this.DataOrientation);
        }

        List<List<SheetCellRange>> TryCombineDataRanges()
        {
            string str;
            if (this.DataSeries.Count == 0)
            {
                return new List<List<SheetCellRange>>();
            }
            bool flag = this.AreXValueFormulaSame(out str);
            List<List<SheetCellRange>> allDataRanges = this.GetAllDataRanges(!flag);
            if (flag)
            {
                SpreadXYDataSeries series = this.DataSeries[0] as SpreadXYDataSeries;
                if ((series != null) && (series.XValuesRange != null))
                {
                    allDataRanges.Insert(0, new List<SheetCellRange>(series.XValuesRange));
                }
            }
            return FormulaUtility.TryCombineSeriesRange(allDataRanges, this.DataOrientation);
        }

        List<List<SheetCellRange>> TryCombineItemsRanges()
        {
            SheetCellRange[] itemsRange = this.GetItemsRange();
            if ((itemsRange == null) || (itemsRange.Length == 0))
            {
                return new List<List<SheetCellRange>>();
            }
            List<SheetCellRange> list = null;
            if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
            {
                list = SheetCellRangeUtility.SplitToColumnRanges(itemsRange, 1);
            }
            else
            {
                list = SheetCellRangeUtility.SplitToRowRanges(itemsRange, 1);
            }
            List<List<SheetCellRange>> seriesRanges = new List<List<SheetCellRange>>((IEnumerable<List<SheetCellRange>>) this.ArrangeRangesByOrientation((IEnumerable<SheetCellRange>) list).Values);
            return FormulaUtility.TryCombineSeriesRange(seriesRanges, this.DataOrientation);
        }

        List<SheetCellRange> TryCombineSeriesNameRanges()
        {
            List<SheetCellRange> list = new List<SheetCellRange>();
            if (this.DataSeries.Count != 0)
            {
                List<SpreadDataSeries> list2 = new List<SpreadDataSeries>();
                foreach (SpreadDataSeries series in this.DataSeries)
                {
                    if (series is SpreadOpenHighLowCloseSeries)
                    {
                        SpreadOpenHighLowCloseSeries series2 = series as SpreadOpenHighLowCloseSeries;
                        list2.Add(series2.OpenSeries);
                        list2.Add(series2.HighSeries);
                        list2.Add(series2.LowSeries);
                        list2.Add(series2.CloseSeries);
                    }
                    else
                    {
                        list2.Add(series);
                    }
                }
                for (int i = 0; i < list2.Count; i++)
                {
                    SpreadDataSeries dataSeries = list2[i];
                    SheetCellRange nameRange = list2[i].NameRange;
                    if (nameRange != null)
                    {
                        nameRange = this.InflateNameRange(nameRange, dataSeries);
                        if (list.Count == 0)
                        {
                            if (nameRange != null)
                            {
                                list.Add(nameRange);
                            }
                        }
                        else
                        {
                            SheetCellRange range2 = list[list.Count - 1];
                            if (((nameRange.Row == range2.Row) && (nameRange.RowCount == range2.RowCount)) && (nameRange.Column == (range2.Column + range2.ColumnCount)))
                            {
                                list.Remove(range2);
                                SheetCellRange range3 = new SheetCellRange(range2.Sheet, range2.Row, range2.Column, range2.RowCount, range2.ColumnCount + nameRange.ColumnCount);
                                list.Add(range3);
                            }
                            else if (((nameRange.Column == range2.Column) && (nameRange.ColumnCount == range2.ColumnCount)) && (nameRange.Row == (range2.Row + range2.RowCount)))
                            {
                                list.Remove(range2);
                                SheetCellRange range4 = new SheetCellRange(range2.Sheet, range2.Row, range2.Column, range2.RowCount + nameRange.RowCount, range2.ColumnCount);
                                list.Add(range4);
                            }
                            else
                            {
                                list.Add(nameRange);
                            }
                        }
                    }
                }
            }
            return list;
        }

        void UpdataAxisMaxDataPointCount()
        {
            int count = 0;
            foreach (SpreadDataSeries series in this.DisplayingDataSeries)
            {
                if (series is SpreadOpenHighLowCloseSeries)
                {
                    SpreadOpenHighLowCloseSeries series2 = series as SpreadOpenHighLowCloseSeries;
                    count = Math.Max(series2.OpenSeries.Values.Count, count);
                    count = Math.Max(series2.HighSeries.Values.Count, count);
                    count = Math.Max(series2.LowSeries.Values.Count, count);
                    count = Math.Max(series2.CloseSeries.Values.Count, count);
                }
                else if (series.Values.Count > count)
                {
                    count = series.Values.Count;
                }
            }
            if (SpreadChartUtility.IsBarChart(this.ChartType))
            {
                if (this.AxisX != null)
                {
                    this.AxisX.MaxDataPointCount = 0;
                }
                if (this.AxisY != null)
                {
                    this.AxisY.MaxDataPointCount = count;
                }
            }
            else
            {
                if (this.AxisY != null)
                {
                    this.AxisY.MaxDataPointCount = 0;
                }
                if (this.AxisX != null)
                {
                    this.AxisX.MaxDataPointCount = count;
                }
            }
        }

        void Update100PercentAxisValues(Axis axis)
        {
            if (this.DisplayingDataSeries.Count == 0)
            {
                axis.Items.Clear();
            }
            else
            {
                bool flag = true;
                bool flag2 = true;
                bool flag3 = true;
                using (IEnumerator<SpreadDataSeries> enumerator = this.DisplayingDataSeries.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        foreach (double num in enumerator.Current.Values)
                        {
                            if (!double.IsNaN(num))
                            {
                                flag3 = false;
                                if (num > 0.0)
                                {
                                    flag2 = false;
                                }
                                else if (num < 0.0)
                                {
                                    flag = false;
                                }
                            }
                        }
                    }
                }
                if (flag || flag3)
                {
                    if (axis.AutoMax)
                    {
                        axis.SetMaxInternal(1.0);
                    }
                    if (axis.AutoMin)
                    {
                        axis.SetMinInternal(0.0);
                    }
                }
                else if (flag2)
                {
                    if (axis.AutoMax)
                    {
                        axis.SetMaxInternal(0.0);
                    }
                    if (axis.AutoMin)
                    {
                        axis.SetMinInternal(-1.0);
                    }
                }
                else
                {
                    if (axis.AutoMax)
                    {
                        axis.SetMaxInternal(1.0);
                    }
                    if (axis.AutoMin)
                    {
                        axis.SetMinInternal(-1.0);
                    }
                }
                if (axis.AutoMinorUnit || axis.AutoMajorUnit)
                {
                    double max;
                    double min;
                    if (axis.Min > axis.Max)
                    {
                        max = axis.Max;
                        min = axis.Min;
                    }
                    else
                    {
                        max = axis.Min;
                        min = axis.Max;
                    }
                    axis.Update100PercentMajorMinorUnit(max, min);
                }
                axis.UpdateItems();
            }
        }

        void UpdateAxisCatetryValues(Axis axis)
        {
            double min = 0.0;
            double num2 = 1.0;
            foreach (SpreadDataSeries series in this.DisplayingDataSeries)
            {
                if (series.GetType() == typeof(SpreadDataSeries))
                {
                    num2 = Math.Max(num2, (double) series.Values.Count);
                }
            }
            if (axis.AutoMax)
            {
                axis.SetMaxInternal(num2);
            }
            if (axis.AutoMin)
            {
                axis.SetMinInternal(min);
            }
            if (axis.AutoMajorUnit)
            {
                axis.SetMajorUnitInteral(1.0);
            }
            if (axis.AutoMinorUnit)
            {
                axis.SetMinorUnitInternal(0.5);
            }
            axis.UpdateItems();
        }

        void UpdateAxisDateTimeValues(Axis axis, XYZValues xyzValues)
        {
            double num;
            double num2;
            this.GetAxisMinMaxValue(axis, xyzValues, out num, out num2);
            if (axis.AutoMinorUnit || axis.AutoMajorUnit)
            {
                axis.UpdateMajorMinorUnit(num, num2);
            }
            if ((num > 0.0) && (num != double.MaxValue))
            {
                try
                {
                    DateTime @this = DateTimeExtension.FromOADate(num);
                    switch (axis.MajorTimeUnit)
                    {
                        case Dt.Cells.Data.AxisTimeUnit.Days:
                            @this = @this.AddDays((double) -((int) axis.MajorUnit));
                            break;

                        case Dt.Cells.Data.AxisTimeUnit.Months:
                            @this = @this.AddMonths(-((int) axis.MajorUnit));
                            break;

                        case Dt.Cells.Data.AxisTimeUnit.Years:
                            @this = @this.AddYears(-((int) axis.MajorUnit));
                            break;
                    }
                    num = @this.ToOADate();
                }
                catch (Exception)
                {
                    num = 0.0;
                }
            }
            if ((num2 > 1.0) && (num2 != double.MaxValue))
            {
                try
                {
                    DateTime time2 = DateTimeExtension.FromOADate(num2);
                    switch (axis.MajorTimeUnit)
                    {
                        case Dt.Cells.Data.AxisTimeUnit.Days:
                            time2 = time2.AddDays((double) ((int) axis.MajorUnit));
                            break;

                        case Dt.Cells.Data.AxisTimeUnit.Months:
                            time2 = time2.AddMonths((int) axis.MajorUnit);
                            break;

                        case Dt.Cells.Data.AxisTimeUnit.Years:
                            time2 = time2.AddYears((int) axis.MajorUnit);
                            break;
                    }
                    num2 = time2.ToOADate();
                }
                catch (Exception)
                {
                    num2 = 1.0;
                }
            }
            if (axis.AutoMin && (num != double.MaxValue))
            {
                axis.SetMinInternal(num);
            }
            if (axis.AutoMax && (num2 != double.MinValue))
            {
                axis.SetMaxInternal(num2);
            }
            axis.UpdateItems();
        }

        void UpdateAxisDoubleValues(Axis axis, XYZValues xyzValues)
        {
            double num;
            double num2;
            this.GetAxisMinMaxValue(axis, xyzValues, out num, out num2);
            if (axis.AutoMinorUnit || axis.AutoMajorUnit)
            {
                axis.UpdateMajorMinorUnit(num, num2);
            }
            if (axis.AutoMin)
            {
                num = AxisUtility.CalculateMinimum(num, num2, axis.MajorUnit, false, axis.LogBase);
                if (num != double.MaxValue)
                {
                    axis.SetMinInternal(num);
                }
            }
            if (axis.AutoMax)
            {
                num2 = AxisUtility.CalculateMaximum(num, num2, axis.MajorUnit, false, axis.LogBase, false);
                if (num2 != double.MinValue)
                {
                    if (!double.IsNaN(axis.MajorUnit))
                    {
                        num2 += axis.MajorUnit;
                    }
                    axis.SetMaxInternal(num2);
                }
            }
            axis.UpdateItems();
        }

        void UpdateAxisDoubleValuesMixed(Axis axis, XYZValues xyzValues)
        {
            double num;
            double num2;
            this.GetAxisMinMaxValue(axis, xyzValues, out num, out num2);
            num = Math.Min(0.0, num);
            if (axis.AutoMinorUnit || axis.AutoMajorUnit)
            {
                axis.UpdateMajorMinorUnit(num, num2);
            }
            if (axis.AutoMin)
            {
                num = AxisUtility.CalculateMinimum(num, num2, axis.MajorUnit, false, axis.LogBase);
                if (num != double.MaxValue)
                {
                    axis.SetMinInternal(num);
                }
            }
            if (axis.AutoMax)
            {
                num2 = AxisUtility.CalculateMaximum(num, num2, axis.MajorUnit, false, axis.LogBase, false);
                if (num2 != double.MinValue)
                {
                    if (!double.IsNaN(axis.MajorUnit))
                    {
                        num2 += axis.MajorUnit;
                    }
                    axis.SetMaxInternal(num2);
                }
            }
            if (string.IsNullOrEmpty(axis.ItemsFormula))
            {
                axis.UpdateItems();
            }
        }

        internal void UpdateAxisesAutoType()
        {
            if (SpreadChartUtility.IsBarChart(this.ChartType))
            {
                if (this.AxisX != null)
                {
                    this.AxisX.SetAutoAxisTypeInternal(AxisType.Value);
                }
                if (this.AxisY != null)
                {
                    if (this.AxisY.Items.IsDateTimeSeries)
                    {
                        this.AxisY.SetAutoAxisTypeInternal(AxisType.Date);
                    }
                    else
                    {
                        this.AxisY.SetAutoAxisTypeInternal(AxisType.Category);
                    }
                }
            }
            else if (SpreadChartUtility.GetDataDimension(this.ChartType) == 1)
            {
                if (this.AxisX != null)
                {
                    if (this.AxisX.Items.IsDateTimeSeries)
                    {
                        this.AxisX.SetAutoAxisTypeInternal(AxisType.Date);
                    }
                    else
                    {
                        this.AxisX.SetAutoAxisTypeInternal(AxisType.Category);
                    }
                }
                if (this.AxisY != null)
                {
                    this.AxisY.SetAutoAxisTypeInternal(AxisType.Value);
                }
            }
            else if (this.AxisY != null)
            {
                this.AxisX.SetAutoAxisTypeInternal(AxisType.Value);
                this.AxisY.SetAutoAxisTypeInternal(AxisType.Value);
            }
        }

        void UpdateAxisesFormatter()
        {
            if (SpreadChartUtility.Is100PercentChart(this.ChartType))
            {
                if (!SpreadChartUtility.IsBarChart(this.ChartType))
                {
                    this.UpdateAxisFormatter(this.AxisX, null);
                    this.UpdateAxisFormatter(this.AxisY, "0%");
                    if (this.AxisZ != null)
                    {
                        this.UpdateAxisFormatter(this.AxisZ, null);
                    }
                }
                else
                {
                    this.UpdateAxisFormatter(this.AxisX, "0%");
                    this.UpdateAxisFormatter(this.AxisY, null);
                    if (this.AxisZ != null)
                    {
                        this.UpdateAxisFormatter(this.AxisZ, null);
                    }
                }
            }
            else
            {
                this.UpdateAxisFormatter(this.AxisX, null);
                this.UpdateAxisFormatter(this.AxisY, null);
                if (this.AxisZ != null)
                {
                    this.UpdateAxisFormatter(this.AxisZ, null);
                }
            }
        }

        void UpdateAxisesTypeBySeriesValues()
        {
            if ((this.DataSeries != null) && (this.DataSeries.Count != 0))
            {
                if (SpreadChartUtility.IsBarChart(this.ChartType))
                {
                    this.UpdateAxisTypeBySeriesValues(this.AxisX, XYZValues.YValues);
                }
                else if (this.DataSeries[0] != null)
                {
                    if (SpreadChartUtility.GetDataDimension(this.ChartType) == 1)
                    {
                        this.UpdateAxisTypeBySeriesValues(this.AxisY, XYZValues.YValues);
                    }
                    else
                    {
                        this.UpdateAxisTypeBySeriesValues(this.AxisX, XYZValues.XValues);
                        this.UpdateAxisTypeBySeriesValues(this.AxisY, XYZValues.YValues);
                    }
                }
            }
        }

        void UpdateAxisesValues()
        {
            bool flag = SpreadChartUtility.Is100PercentChart(this.ChartType);
            if (SpreadChartUtility.IsBarChart(this.ChartType))
            {
                if (flag)
                {
                    this.Update100PercentAxisValues(this.AxisX);
                }
                else
                {
                    this.UpdateAxisValuesBySeriesValues(this.AxisX, XYZValues.YValues);
                }
                this.UpdateAxisValuesBySeriesValues(this.AxisY, XYZValues.XValues);
            }
            else
            {
                this.UpdateAxisValuesBySeriesValues(this.AxisX, XYZValues.XValues);
                if (flag)
                {
                    this.Update100PercentAxisValues(this.AxisY);
                }
                else
                {
                    this.UpdateAxisValuesBySeriesValues(this.AxisY, XYZValues.YValues);
                }
            }
        }

        internal void UpdateAxisFormatter(Axis axis, string formatter = null)
        {
            if (axis != null)
            {
                axis.SuspendEvents();
                if ((axis.LabelFormatter == null) || (axis.LabelFormatter is AutoFormatter))
                {
                    if (!string.IsNullOrEmpty(formatter))
                    {
                        axis.LabelFormatter = new AutoFormatter(new GeneralFormatter(formatter));
                    }
                    else if (axis.AxisType == AxisType.Date)
                    {
                        axis.LabelFormatter = new AutoFormatter(new GeneralFormatter("m/d/yyyy"));
                    }
                    else
                    {
                        axis.LabelFormatter = new AutoFormatter(new GeneralFormatter());
                    }
                }
                axis.ResumeEvents();
            }
        }

        void UpdateAxisItemsFormula(string itemsFormula)
        {
            if (SpreadChartUtility.IsBarChart(this.ChartType))
            {
                this.AxisX.SetItemsFormulaInternal(null);
                this.AxisY.SetItemsFormulaInternal(itemsFormula);
            }
            else if (SpreadChartUtility.GetDataDimension(this.ChartType) == 1)
            {
                this.AxisX.SetItemsFormulaInternal(itemsFormula);
                this.AxisY.SetItemsFormulaInternal(null);
            }
            else
            {
                this.AxisX.SetItemsFormulaInternal(null);
                this.AxisY.SetItemsFormulaInternal(null);
            }
            this.UpdateAxisesFormatter();
        }

        void UpdateAxisOnChartTypeChanged()
        {
            base.SuspendEvents();
            this.UpdateAxisesAutoType();
            this.UpdateAxisesTypeBySeriesValues();
            this.UpdataAxisMaxDataPointCount();
            this.UpdateAxisesFormatter();
            this.UpdateAxisesValues();
            base.ResumeEvents();
        }

        void UpdateAxisTypeBySeriesValues(Axis axis, XYZValues xyzValues)
        {
            if (((axis != null) && !axis.UseCustomItems) && (this.DisplayingDataSeries.Count != 0))
            {
                axis.SuspendEvents();
                if (xyzValues == XYZValues.XValues)
                {
                    SpreadXYDataSeries series = this.DisplayingDataSeries[0] as SpreadXYDataSeries;
                    if (series != null)
                    {
                        AxisType type = series.XValues.IsDateTimeSeries ? AxisType.Date : AxisType.Value;
                        axis.SetAutoAxisTypeInternal(type);
                    }
                }
                else if (xyzValues == XYZValues.YValues)
                {
                    axis.SetAutoAxisTypeInternal(AxisType.Value);
                }
                axis.ResumeEvents();
            }
        }

        void UpdateAxisValuesBySeriesValues(Axis axis, XYZValues xyzValues)
        {
            if (axis.AxisType == AxisType.Value)
            {
                if (string.IsNullOrEmpty(axis.ItemsFormula))
                {
                    if (this.DisplayingDataSeries.Count > 0)
                    {
                        this.UpdateAxisDoubleValues(axis, xyzValues);
                    }
                    else if (!axis.UseCustomItems)
                    {
                        axis.Items.Clear();
                    }
                }
            }
            else if (axis.AxisType == AxisType.Date)
            {
                if (string.IsNullOrEmpty(axis.ItemsFormula))
                {
                    this.UpdateAxisDateTimeValues(axis, xyzValues);
                }
                else
                {
                    axis.UpdateMajorMinorUnit(axis.Min, axis.Max);
                    axis.UpdateItems();
                }
            }
            else if ((axis.AxisType == AxisType.Category) && string.IsNullOrEmpty(axis.ItemsFormula))
            {
                if (this.HasTwoDimensionSeries())
                {
                    this.UpdateAxisDoubleValuesMixed(axis, xyzValues);
                }
                else
                {
                    this.UpdateAxisCatetryValues(axis);
                }
            }
        }

        void UpdateAxisValuesOnDataSeriesChanged(string changed)
        {
            if (changed == "XValues")
            {
                this.UpdateAxisValuesOnSeriesXValuesChanged();
            }
            else if (changed == "Values")
            {
                this.UpdateAxisValuesOnSeriesYValuesChanged();
                if (((this.AxisX.Items.Count > 0) && !this.AxisX.Items.IsBoundToDataSeries) && !this.AxisX.UseCustomItems)
                {
                    this.UpdateAxisValuesOnSeriesXValuesChanged();
                }
            }
            else if (changed == "")
            {
                this.UpdateAxisValuesOnSeriesXValuesChanged();
                this.UpdateAxisValuesOnSeriesYValuesChanged();
            }
        }

        void UpdateAxisValuesOnSeriesXValuesChanged()
        {
            if (SpreadChartUtility.IsBarChart(this.ChartType))
            {
                this.UpdateAxisValuesBySeriesValues(this.AxisY, XYZValues.XValues);
            }
            else
            {
                this.UpdateAxisValuesBySeriesValues(this.AxisX, XYZValues.XValues);
            }
        }

        void UpdateAxisValuesOnSeriesYValuesChanged()
        {
            bool flag = SpreadChartUtility.Is100PercentChart(this.ChartType);
            if (SpreadChartUtility.IsBarChart(this.ChartType))
            {
                if (flag)
                {
                    this.Update100PercentAxisValues(this.AxisX);
                }
                else
                {
                    this.UpdateAxisValuesBySeriesValues(this.AxisX, XYZValues.YValues);
                }
            }
            else if (flag)
            {
                this.Update100PercentAxisValues(this.AxisY);
            }
            else
            {
                this.UpdateAxisValuesBySeriesValues(this.AxisY, XYZValues.YValues);
            }
        }

        void UpdateChartElements()
        {
            base.SuspendEvents();
            string str = (base.Sheet != null) ? this.GetFormula() : null;
            string str2 = !string.IsNullOrEmpty(str) ? str : this._formula;
            if (!string.IsNullOrEmpty(str2))
            {
                this.UpdateFormula(str2);
            }
            else
            {
                if (!this._dataOrientation.HasValue)
                {
                    CellRange category = null;
                    CellRange seriesName = null;
                    CellRange data = null;
                    SheetCellRange[] rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(base.Sheet, this._itemsFormula);
                    if ((rangeArray != null) && (rangeArray.Length > 0))
                    {
                        category = new CellRange(rangeArray[0].Row, rangeArray[0].Column, rangeArray[0].RowCount, rangeArray[0].ColumnCount);
                    }
                    SheetCellRange[] rangeArray2 = SheetCellRangeUtility.ExtractAllExternalReference(base.Sheet, this._dataFormula);
                    if ((rangeArray2 != null) && (rangeArray2.Length > 0))
                    {
                        data = new CellRange(rangeArray2[0].Row, rangeArray2[0].Column, rangeArray2[0].RowCount, rangeArray2[0].ColumnCount);
                    }
                    SheetCellRange[] rangeArray3 = SheetCellRangeUtility.ExtractAllExternalReference(base.Sheet, this._nameFormula);
                    if ((rangeArray3 != null) && (rangeArray3.Length > 0))
                    {
                        seriesName = new CellRange(rangeArray3[0].Row, rangeArray3[0].Column, rangeArray3[0].RowCount, rangeArray3[0].ColumnCount);
                    }
                    if (((category != null) || (data != null)) || (seriesName != null))
                    {
                        bool flag = SpreadChartUtility.IsChartDataVertical(seriesName, category, data);
                        this._dataOrientation = new Dt.Cells.Data.DataOrientation?(flag ? Dt.Cells.Data.DataOrientation.Vertical : Dt.Cells.Data.DataOrientation.Horizontal);
                    }
                }
                if (!string.IsNullOrEmpty(this._itemsFormula))
                {
                    this.UpdateAxisItemsFormula(this._itemsFormula);
                }
                else
                {
                    this.RelLoadAxisItemsFormula();
                }
                this.UpdateAxisesFormatter();
                if (!string.IsNullOrEmpty(this._dataFormula))
                {
                    this.UpdateSeriesByDataFormula(this._dataFormula);
                }
                else
                {
                    this.UpdateDataSeriesReferences();
                }
                if (!string.IsNullOrEmpty(this._nameFormula))
                {
                    this.UpdateSeriesNameFormula(this._nameFormula, false);
                }
            }
            base.ResumeEvents();
        }

        internal void UpdateDataReferences()
        {
            if (((this.AxisX != null) && !this.AxisX.UseCustomItems) && !string.IsNullOrEmpty(this.AxisX.ItemsFormula))
            {
                this.AxisX.UpdateItemsReference();
                if ((this.AxisX.Title != null) && !string.IsNullOrEmpty(this.AxisX.Title.TextFormula))
                {
                    this.AxisX.Title.UpdateTextReference();
                }
            }
            if (((this.AxisY != null) && !this.AxisY.UseCustomItems) && !string.IsNullOrEmpty(this.AxisY.ItemsFormula))
            {
                this.AxisY.UpdateItemsReference();
                if ((this.AxisY.Title != null) && !string.IsNullOrEmpty(this.AxisY.Title.TextFormula))
                {
                    this.AxisY.Title.UpdateTextReference();
                }
            }
            if (((this.AxisZ != null) && !this.AxisZ.UseCustomItems) && !string.IsNullOrEmpty(this.AxisZ.ItemsFormula))
            {
                this.AxisZ.UpdateItemsReference();
                if ((this.AxisZ.Title != null) && !string.IsNullOrEmpty(this.AxisZ.Title.TextFormula))
                {
                    this.AxisZ.Title.UpdateTextReference();
                }
            }
            if ((base.ChartTitle != null) && !string.IsNullOrEmpty(base.ChartTitle.TextFormula))
            {
                base.ChartTitle.UpdateTextReference();
            }
            this.UpdateDataSeriesReferences();
        }

        internal void UpdateDataSeriesReferences()
        {
            this.DataSeries.SuspendEvent();
            using (IEnumerator<SpreadDataSeries> enumerator = this.DataSeries.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.UpdateReferences();
                }
            }
            this.DataSeries.ResumeEvent();
            this.NotifyChartAreaChanged(ChartArea.DataSeries, "");
        }

        void UpdateDisplayingDataSeries()
        {
            this._displayingDataSeries = this.GetVisibleDataSeries();
        }

        void UpdateElementsDisplayHidden(bool displayHidden)
        {
            using (IEnumerator<Axis> enumerator = this._xAxises.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.DisplayHidden = displayHidden;
                }
            }
            using (IEnumerator<Axis> enumerator2 = this._yAxises.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.DisplayHidden = displayHidden;
                }
            }
            using (IEnumerator<Axis> enumerator3 = this._zAxises.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.DisplayHidden = displayHidden;
                }
            }
            using (IEnumerator<SpreadDataSeries> enumerator4 = this._series.GetEnumerator())
            {
                while (enumerator4.MoveNext())
                {
                    enumerator4.Current.DisplayHidden = displayHidden;
                }
            }
        }

        void UpdateElementsEmptyValueStyle(EmptyValueStyle emptyValueStyle)
        {
            using (IEnumerator<Axis> enumerator = this._xAxises.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.EmptyValueStyle = emptyValueStyle;
                }
            }
            using (IEnumerator<Axis> enumerator2 = this._yAxises.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.EmptyValueStyle = emptyValueStyle;
                }
            }
            using (IEnumerator<Axis> enumerator3 = this._zAxises.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.EmptyValueStyle = emptyValueStyle;
                }
            }
            using (IEnumerator<SpreadDataSeries> enumerator4 = this._series.GetEnumerator())
            {
                while (enumerator4.MoveNext())
                {
                    enumerator4.Current.EmptyValueStyle = emptyValueStyle;
                }
            }
        }

        void UpdateFormula(string formula)
        {
            this.DetectFormulas(formula, out this._dataFormula, out this._nameFormula, out this._itemsFormula);
            base.SuspendEvents();
            this.UpdateAxisItemsFormula(this._itemsFormula);
            bool fromSecondNameSeries = this.UpdateSeriesByDataFormula(this._dataFormula);
            this.UpdateSeriesNameFormula(this._nameFormula, fromSecondNameSeries);
            base.ResumeEvents();
            this.NotifyChartAreaChanged(ChartArea.All, "Formula");
        }

        bool UpdateSeriesByDataFormula(string dataFormula)
        {
            SheetCellRange[] rangeArray;
            bool flag = false;
            if ((base.Sheet == null) || string.IsNullOrEmpty(dataFormula))
            {
                this.DataSeries.ClearElementsInternal();
                return flag;
            }
            try
            {
                CalcExpression expression = FormulaUtility.Formula2Expression(base.Sheet, dataFormula);
                rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(base.Sheet, expression);
            }
            catch (Exception)
            {
                return flag;
            }
            if ((rangeArray == null) || (rangeArray.Length == 0))
            {
                this.DataSeries.Clear();
                return flag;
            }
            base.SuspendEvents();
            List<SheetCellRange> list = null;
            if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
            {
                list = SheetCellRangeUtility.SplitToColumnRanges(rangeArray, 1);
            }
            else
            {
                list = SheetCellRangeUtility.SplitToRowRanges(rangeArray, 1);
            }
            Dictionary<int, List<SheetCellRange>> dictionary = this.ArrangeRangesByOrientation((IEnumerable<SheetCellRange>) list);
            List<List<SheetCellRange>> ranges = new List<List<SheetCellRange>>((IEnumerable<List<SheetCellRange>>) dictionary.Values);
            bool flag2 = SpreadChartUtility.AreSegmentsAligned(ranges, this.DataOrientation);
            List<SpreadDataSeries> list3 = new List<SpreadDataSeries>();
            List<List<SheetCellRange>> list4 = new List<List<SheetCellRange>>();
            if (flag2)
            {
                list4.AddRange((IEnumerable<List<SheetCellRange>>)dictionary.Values);
            }
            else
            {
                // hdt
                //List<SheetCellRange> list5 = new List<SheetCellRange>();
                //foreach (SheetCellRange range in list)
                //{
                //    list5.Add(range);
                //}
                //if (list5.Count > 0)
                //    list4.Add(list5);//唐忠宝增加 无此句 list5 无意义
            }
            if (list4.Count == 0)
            {
                base.ResumeEvents();
                return flag;
            }
            int dataDimension = SpreadChartUtility.GetDataDimension(this.ChartType);
            int num2 = dataDimension - 1;
            switch (dataDimension)
            {
                case 1:
                    for (int i = 0; i < list4.Count; i++)
                    {
                        List<SheetCellRange> list6 = list4[i];
                        SpreadDataSeries series = null;
                        if (i < this.DataSeries.Count)
                        {
                            series = this.DataSeries[i];
                        }
                        if (series == null)
                        {
                            series = this.CreateDataSeries();
                        }
                        series.ValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list6);
                        list3.Add(series);
                    }
                    break;

                case 2:
                    if (list4.Count > 1)
                    {
                        string str = this._itemsFormula;
                        if (string.IsNullOrEmpty(str))
                        {
                            List<SheetCellRange> list7 = list4[0];
                            str = SheetCellRangeUtility.BuildFormula(base.Sheet, list7);
                            flag = true;
                        }
                        int num4 = string.IsNullOrEmpty(this._itemsFormula) ? 1 : 0;
                        int num5 = 0;
                        for (int j = num4; j < list4.Count; j++)
                        {
                            SpreadXYDataSeries series2 = null;
                            if (num5 < this.DataSeries.Count)
                            {
                                series2 = this.DataSeries[num5] as SpreadXYDataSeries;
                            }
                            if (series2 == null)
                            {
                                series2 = this.CreateDataSeries() as SpreadXYDataSeries;
                            }
                            else
                            {
                                num5++;
                            }
                            if (series2 != null)
                            {
                                series2.XValueFormula = str;
                                series2.ValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list4[j]);
                                list3.Add(series2);
                            }
                        }
                    }
                    else
                    {
                        SpreadXYDataSeries series3 = null;
                        if (this.DataSeries.Count > 0)
                        {
                            series3 = this.DataSeries[0] as SpreadXYDataSeries;
                        }
                        if (series3 == null)
                        {
                            series3 = this.CreateDataSeries() as SpreadXYDataSeries;
                        }
                        if (series3 != null)
                        {
                            series3.XValueFormula = this._itemsFormula;
                            series3.ValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list4[0]);
                            list3.Add(series3);
                        }
                    }
                    break;

                case 3:
                {
                    string str2 = this._itemsFormula;
                    if (string.IsNullOrEmpty(str2) && ((list4.Count % num2) != 0))
                    {
                        List<SheetCellRange> list8 = list4[0];
                        str2 = SheetCellRangeUtility.BuildFormula(base.Sheet, list8);
                        flag = true;
                    }
                    int num8 = flag ? 1 : 0;
                    int num9 = 0;
                    for (int k = num8; k < list4.Count; k += 2)
                    {
                        SpreadDataSeries series4 = null;
                        if (num9 < this.DataSeries.Count)
                        {
                            if (this.DataSeries[num9] is SpreadXYDataSeries)
                            {
                                series4 = this.DataSeries[num9] as SpreadXYDataSeries;
                            }
                            else if (this.DataSeries[num9] is SpreadBubbleSeries)
                            {
                                series4 = this.DataSeries[num9] as SpreadBubbleSeries;
                            }
                        }
                        if (series4 == null)
                        {
                            series4 = this.CreateDataSeries();
                        }
                        else
                        {
                            num9++;
                        }
                        if (series4 is SpreadXYZDataSeries)
                        {
                            SpreadXYZDataSeries series5 = series4 as SpreadXYZDataSeries;
                            series5.XValueFormula = str2;
                            series5.ValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list4[k]);
                            if ((k + 1) < list4.Count)
                            {
                                series5.ZValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list4[k + 1]);
                            }
                        }
                        else if (series4 is SpreadBubbleSeries)
                        {
                            SpreadBubbleSeries series6 = series4 as SpreadBubbleSeries;
                            series6.XValueFormula = str2;
                            series6.ValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list4[k]);
                            if ((k + 1) < list4.Count)
                            {
                                series6.SizeFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list4[k + 1]);
                            }
                        }
                        list3.Add(series4);
                    }
                    break;
                }
                case 5:
                {
                    string str3 = this._itemsFormula;
                    if (string.IsNullOrEmpty(str3) && ((list4.Count % num2) != 0))
                    {
                        List<SheetCellRange> list9 = list4[0];
                        str3 = SheetCellRangeUtility.BuildFormula(base.Sheet, list9);
                        flag = true;
                    }
                    int num12 = flag ? 1 : 0;
                    int num13 = 0;
                    for (int m = num12; m < list4.Count; m += 4)
                    {
                        SpreadOpenHighLowCloseSeries series7 = null;
                        if (num13 < this.DataSeries.Count)
                        {
                            series7 = this.DataSeries[num13] as SpreadOpenHighLowCloseSeries;
                        }
                        if (series7 == null)
                        {
                            series7 = this.CreateDataSeries() as SpreadOpenHighLowCloseSeries;
                        }
                        else
                        {
                            num13++;
                        }
                        if (series7 != null)
                        {
                            series7.XValueFormula = str3;
                            List<SheetCellRange> list10 = list4[m];
                            series7.OpenSeries.ValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list10);
                            if ((m + 1) < list4.Count)
                            {
                                List<SheetCellRange> list11 = list4[m + 1];
                                series7.HighSeries.ValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list11);
                            }
                            else
                            {
                                series7.HighSeries.ValueFormula = null;
                            }
                            if ((m + 2) < list4.Count)
                            {
                                List<SheetCellRange> list12 = list4[m + 2];
                                series7.LowSeries.ValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list12);
                            }
                            else
                            {
                                series7.LowSeries.ValueFormula = null;
                            }
                            if ((m + 3) < list4.Count)
                            {
                                List<SheetCellRange> list13 = list4[m + 3];
                                series7.CloseSeries.ValueFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, list13);
                            }
                            else
                            {
                                series7.CloseSeries.ValueFormula = null;
                            }
                        }
                        list3.Add(series7);
                    }
                    break;
                }
            }
            using (List<SpreadDataSeries>.Enumerator enumerator2 = list3.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.SuspendEvents();
                }
            }
            this.DataSeries.SuspendEvent();
            this.DataSeries.ClearElementsInternal();
            this.DataSeries.AddRange((IList<SpreadDataSeries>) list3);
            this.DataSeries.ResumeEvent();
            using (List<SpreadDataSeries>.Enumerator enumerator3 = list3.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.ResumeEvents();
                }
            }
            base.ResumeEvents();
            this.NotifyChartAreaChanged(ChartArea.DataSeries, "");
            return flag;
        }

        void UpdateSeriesNameFormula(string nameFormula, bool fromSecondNameSeries = false)
        {
            if ((base.Sheet == null) || string.IsNullOrEmpty(nameFormula))
            {
                base.SuspendEvents();
                using (IEnumerator<SpreadDataSeries> enumerator = this.DataSeries.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.NameFormula = null;
                    }
                }
                base.ResumeEvents();
            }
            else
            {
                SheetCellRange[] rangeArray;
                try
                {
                    CalcExpression expression = FormulaUtility.Formula2Expression(base.Sheet, nameFormula);
                    rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(base.Sheet, expression);
                }
                catch (Exception)
                {
                    return;
                }
                if ((rangeArray != null) && (rangeArray.Length != 0))
                {
                    List<SheetCellRange> list;
                    base.SuspendEvents();
                    if (this.DataOrientation == Dt.Cells.Data.DataOrientation.Vertical)
                    {
                        list = SheetCellRangeUtility.SplitToColumnRanges(rangeArray, 1);
                    }
                    else
                    {
                        list = SheetCellRangeUtility.SplitToRowRanges(rangeArray, 1);
                    }
                    if (fromSecondNameSeries && (list.Count > 0))
                    {
                        list.RemoveAt(0);
                    }
                    if (this.ChartType == SpreadChartType.StockHighLowOpenClose)
                    {
                        for (int i = 0; i < this.DataSeries.Count; i++)
                        {
                            SpreadOpenHighLowCloseSeries series3 = (SpreadOpenHighLowCloseSeries) this.DataSeries[i];
                            try
                            {
                                if (series3.OpenSeries != null)
                                {
                                    series3.OpenSeries.SuspendEvents();
                                }
                                if (series3.HighSeries != null)
                                {
                                    series3.HighSeries.SuspendEvents();
                                }
                                if (series3.LowSeries != null)
                                {
                                    series3.LowSeries.SuspendEvents();
                                }
                                if (series3.CloseSeries != null)
                                {
                                    series3.CloseSeries.SuspendEvents();
                                }
                                int num2 = i * 4;
                                if (series3.OpenSeries != null)
                                {
                                    if (num2 < list.Count)
                                    {
                                        series3.OpenSeries.NameFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, new List<SheetCellRange> { list[num2] });
                                    }
                                    else
                                    {
                                        series3.OpenSeries.NameFormula = null;
                                    }
                                }
                                int num3 = (i * 4) + 1;
                                if (series3.HighSeries != null)
                                {
                                    if (num3 < list.Count)
                                    {
                                        series3.HighSeries.NameFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, new List<SheetCellRange> { list[num3] });
                                    }
                                    else
                                    {
                                        series3.HighSeries.NameFormula = null;
                                    }
                                }
                                int num4 = (i * 4) + 2;
                                if (series3.LowSeries != null)
                                {
                                    if (num4 < list.Count)
                                    {
                                        series3.LowSeries.NameFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, new List<SheetCellRange> { list[num4] });
                                    }
                                    else
                                    {
                                        series3.LowSeries.NameFormula = null;
                                    }
                                }
                                int num5 = (i * 4) + 3;
                                if (series3.CloseSeries != null)
                                {
                                    if (num5 < list.Count)
                                    {
                                        series3.CloseSeries.NameFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, new List<SheetCellRange> { list[num5] });
                                    }
                                    else
                                    {
                                        series3.CloseSeries.NameFormula = null;
                                    }
                                }
                            }
                            finally
                            {
                                if (series3.OpenSeries != null)
                                {
                                    series3.OpenSeries.ResumeEvents();
                                }
                                if (series3.HighSeries != null)
                                {
                                    series3.HighSeries.ResumeEvents();
                                }
                                if (series3.LowSeries != null)
                                {
                                    series3.LowSeries.ResumeEvents();
                                }
                                if (series3.CloseSeries != null)
                                {
                                    series3.CloseSeries.ResumeEvents();
                                }
                            }
                        }
                    }
                    else
                    {
                        int num7 = SpreadChartUtility.GetDataDimension(this.ChartType) - 1;
                        if (num7 <= 0)
                        {
                            num7 = 1;
                        }
                        List<SheetCellRange> list6 = new List<SheetCellRange>();
                        for (int j = 0; j < list.Count; j++)
                        {
                            if ((j % num7) == 0)
                            {
                                list6.Add(list[j]);
                            }
                        }
                        for (int k = 0; k < this.DataSeries.Count; k++)
                        {
                            if (k < list6.Count)
                            {
                                SpreadDataSeries series4 = this.DataSeries[k];
                                List<SheetCellRange> ranges = new List<SheetCellRange> {
                                    list6[k]
                                };
                                series4.SuspendEvents();
                                series4.NameFormula = SheetCellRangeUtility.BuildFormula(base.Sheet, ranges);
                                series4.ResumeEvents();
                            }
                        }
                    }
                    base.ResumeEvents();
                }
                else
                {
                    base.SuspendEvents();
                    using (IEnumerator<SpreadDataSeries> enumerator2 = this.DataSeries.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            enumerator2.Current.NameFormula = null;
                        }
                    }
                    base.ResumeEvents();
                }
            }
        }

        void UpdateSettings()
        {
            base.SuspendEvents();
            this.UpdateAxisesAutoType();
            if (this._dataSeriesSettings != null)
            {
                if (SpreadChartUtility.IsStackedChart(this.ChartType))
                {
                    this._dataSeriesSettings.SeriesOverlap = 1.0;
                }
                else
                {
                    this._dataSeriesSettings.SeriesOverlap = 0.0;
                }
            }
            if ((this._pieChartSettings != null) && SpreadChartUtility.IsPieChart(this.ChartType))
            {
                this.VaryColorsByPoint = true;
                if ((this.ChartType == SpreadChartType.Pie) || (this.ChartType == SpreadChartType.PieDoughnut))
                {
                    this._pieChartSettings.Explosion = 0.0;
                }
                else
                {
                    this._pieChartSettings.Explosion = 0.25;
                }
                if ((this.ChartType == SpreadChartType.PieDoughnut) || (this.ChartType == SpreadChartType.PieExplodedDoughnut))
                {
                    this._pieChartSettings.HoleSize = 0.5;
                }
            }
            if (SpreadChartUtility.IsPieChart(this.ChartType))
            {
                this._varyColorsByPoint = true;
            }
            else
            {
                this._varyColorsByPoint = false;
            }
            base.ResumeEvents();
        }

        void UpdateUpDownDars()
        {
            if ((this.ChartType == SpreadChartType.StockHighLowOpenClose) && (this.UpDownDarsSettings == null))
            {
                this.UpDownDarsSettings = new UpDownBars();
                this.UpDownDarsSettings.GapWidth = 150;
            }
        }

        void View3D_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyChartAreaChanged(ChartArea.Chart, "View3D");
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            Serializer.SerializeObj(this._chartType, "ChartType", writer);
            if (this._dataOrientation.HasValue)
            {
                Serializer.SerializeObj(this._dataOrientation.Value, "DataOrientation", writer);
            }
            if (this._displayEmptyCellsAs != EmptyValueStyle.Gaps)
            {
                Serializer.SerializeObj(this._displayEmptyCellsAs, "DisplayEmptyCellsAs", writer);
            }
            if (this._displayHidden)
            {
                Serializer.SerializeObj((bool) this._displayHidden, "DisplayHidden", writer);
            }
            if (this._view3D != null)
            {
                Serializer.SerializeObj(this._view3D, "View3D", writer);
            }
            if (this._dataLabelSettings != null)
            {
                this._dataLabelSettings.PropertyChanged -= new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
                Serializer.SerializeObj(this._dataLabelSettings, "DataLabelSettings", writer);
            }
            if (!this._varyColorsByPoint)
            {
                Serializer.SerializeObj((bool) this._varyColorsByPoint, "VaryColorsByPoint", writer);
            }
            if (this.DataSeries.Count > 0)
            {
                Serializer.SerializeList(this.DataSeries, "DataSeries", writer);
            }
            if (this._xAxises.Count > 0)
            {
                Serializer.SerializeList(this._xAxises, "XAxies", writer);
            }
            if (this._yAxises.Count > 0)
            {
                Serializer.SerializeList(this._yAxises, "YAxies", writer);
            }
            if (this._zAxises.Count > 0)
            {
                Serializer.SerializeList(this._zAxises, "ZAxies", writer);
            }
            if (this._plotAreas.Count > 0)
            {
                Serializer.SerializeList(this._plotAreas, "PlotAreas", writer);
            }
            if (this._legend != null)
            {
                Serializer.SerializeObj(this._legend, "Legend", writer);
            }
            if (this._floorWall != null)
            {
                Serializer.SerializeObj(this._floorWall, "FloorWall", writer);
            }
            if (this._sideWall != null)
            {
                Serializer.SerializeObj(this._sideWall, "SideWall", writer);
            }
            if (this._backWall != null)
            {
                Serializer.SerializeObj(this._backWall, "BackWall", writer);
            }
            if (!this._showAutoTitle)
            {
                Serializer.SerializeObj((bool) this._showAutoTitle, "AutoTitle", writer);
            }
            if (this._showDataLabelOverMax)
            {
                Serializer.SerializeObj((bool) this._showAutoTitle, "ShowDataLabelOverMax", writer);
            }
        }

        internal List<int> AlternateContentChoiceList { get; set; }

        internal List<int> AlternateFallbackStyleList { get; set; }

        /// <summary>
        /// Gets or sets the x-axis.
        /// </summary>
        /// <value>
        /// The x-axis of the chart.
        /// </value>
        public Axis AxisX
        {
            get
            {
                if (this._xAxises.Count > 0)
                {
                    return this._xAxises[0];
                }
                return null;
            }
            set
            {
                if ((value != this._xAxises[0]) && (value != null))
                {
                    value.Orientation = Dt.Cells.Data.AxisOrientation.X;
                    this._xAxises[0] = value;
                    this.NotifyChartAreaChanged(ChartArea.Chart, "AxisX");
                    base.RaisePropertyChanged("AxisX");
                }
            }
        }

        /// <summary>
        /// Gets or sets the y-axis.
        /// </summary>
        /// <value>
        /// The y-axis of the chart.
        /// </value>
        public Axis AxisY
        {
            get
            {
                if (this._yAxises.Count > 0)
                {
                    return this._yAxises[0];
                }
                return null;
            }
            set
            {
                if ((value != this._yAxises[0]) && (value != null))
                {
                    value.Orientation = Dt.Cells.Data.AxisOrientation.Y;
                    this._yAxises[0] = value;
                    this.NotifyChartAreaChanged(ChartArea.Chart, "AxisY");
                    base.RaisePropertyChanged("AxisY");
                }
            }
        }

        /// <summary>
        /// Gets or sets the z-axis.
        /// </summary>
        /// <value>
        /// The z-axis of the chart.
        /// </value>
        public Axis AxisZ
        {
            get
            {
                if (this._zAxises.Count > 0)
                {
                    return this._zAxises[0];
                }
                return null;
            }
            set
            {
                if (this._zAxises.Count == 0)
                {
                    if (value != null)
                    {
                        value.Orientation = Dt.Cells.Data.AxisOrientation.Z;
                        this._zAxises.Add(value);
                        base.RaisePropertyChanged("AxisZ");
                    }
                }
                else if (value != this._zAxises[0])
                {
                    if (value != null)
                    {
                        value.Orientation = Dt.Cells.Data.AxisOrientation.Z;
                        this._zAxises[0] = value;
                    }
                    else
                    {
                        this._zAxises.ClearElementsInternal();
                    }
                    base.RaisePropertyChanged("AxisZ");
                }
            }
        }

        /// <summary>
        /// Gets or sets the back wall.
        /// </summary>
        /// <value>
        /// The back wall.
        /// </value>
        internal Wall BackWall
        {
            get { return  this._backWall; }
            set
            {
                this._backWall = value;
                if ((this._backWall != null) && !object.ReferenceEquals(this._backWall.Chart, this))
                {
                    this._backWall.Chart = this;
                }
                base.RaisePropertyChanged("BackWall");
            }
        }

        /// <summary>
        /// Gets or sets the pie chart settings.
        /// </summary>
        /// <value>
        /// The pie chart settings.
        /// </value>
        internal Dt.Cells.Data.BubbleChartSettings BubbleChartSettings
        {
            get { return  this._bubbleChartSettings; }
            set
            {
                if (value != this._bubbleChartSettings)
                {
                    this._bubbleChartSettings = value;
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the user can switch the rows and columns of the chart. 
        /// </summary>
        /// <value>
        /// <c>true</c> if user can switch rows and columns of the chart; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This sets whether the user can switch the row and column on the chart.
        /// </remarks>
        public bool CanSwitchRowColumn
        {
            get { return  this.GetCanSwitchRowColumn(); }
        }

        /// <summary>
        /// Gets or sets the type of the chart.
        /// </summary>
        /// <value>
        /// The type of the chart.
        /// </value>
        public SpreadChartType ChartType
        {
            get { return  this._chartType; }
            set
            {
                base.SuspendEvents();
                this.ResetDataSeriesChartType();
                base.ResumeEvents();
                this._chartType = value;
                this.UpdateAxisOnChartTypeChanged();
                this.UpdateUpDownDars();
                base.RaisePropertyChanged("ChartType");
            }
        }

        /// <summary>
        /// Gets or sets the data formula. This action will re-build the chart's series. 
        /// </summary>
        /// <value>
        /// The data formula.
        /// </value>
        /// <remarks>
        /// This property is not designed for persisting so you should save and re-use the value and access infrequently.
        /// </remarks>
        public string DataFormula
        {
            get
            {
                if (base.Sheet == null)
                {
                    return this._dataFormula;
                }
                return this.GetDataFormula();
            }
            set
            {
                SpreadChartUtility.ValidateFormula(base.Sheet, value, false);
                if (value != this.DataFormula)
                {
                    this._dataFormula = value;
                    bool fromSecondNameSeries = this.UpdateSeriesByDataFormula(value);
                    if (!string.IsNullOrEmpty(this._nameFormula))
                    {
                        this.UpdateSeriesNameFormula(this._nameFormula, fromSecondNameSeries);
                    }
                    this.NotifyChartAreaChanged(ChartArea.DataSeries, "");
                }
            }
        }

        /// <summary>
        /// Gets or sets the settings of data labels.
        /// </summary>
        /// <value>
        /// The settings of data labels.
        /// </value>
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

        internal ChartLabelStyleInfo DataLabelStyleInfo { get; set; }

        /// <summary>
        /// Gets or sets the data orientation.
        /// </summary>
        /// <value>
        /// The data orientation.
        /// </value>
        internal Dt.Cells.Data.DataOrientation DataOrientation
        {
            get
            {
                if (this._dataOrientation.HasValue)
                {
                    return this._dataOrientation.Value;
                }
                if (((this.DataSeries.Count > 0) && (this.DataSeries[0] != null)) && this.DataSeries[0].ValueOrientation.HasValue)
                {
                    return this.DataSeries[0].ValueOrientation.Value;
                }
                return Dt.Cells.Data.DataOrientation.Vertical;
            }
            set
            {
                if (value != this.DataOrientation)
                {
                    this.ProcessDataOrientationChange(value);
                }
            }
        }

        /// <summary>
        /// Gets the collection of data series.
        /// </summary>
        /// <value>
        /// The collection of data series.
        /// </value>
        public DataSeriesCollection DataSeries
        {
            get { return  this._series; }
        }

        /// <summary>
        /// Gets or sets the data series settings.
        /// </summary>
        /// <value>
        /// The data series settings.
        /// </value>
        internal Dt.Cells.Data.DataSeriesSettings DataSeriesSettings
        {
            get
            {
                if (this._dataSeriesSettings == null)
                {
                    this._dataSeriesSettings = new Dt.Cells.Data.DataSeriesSettings();
                    if (SpreadChartUtility.IsStackedChart(this.ChartType))
                    {
                        this._dataSeriesSettings.SeriesOverlap = 1.0;
                    }
                    this._dataSeriesSettings.PropertyChanged += new PropertyChangedEventHandler(this.DataSeriesSettings_PropertyChanged);
                }
                return this._dataSeriesSettings;
            }
            set
            {
                if (value != this.DataSeriesSettings)
                {
                    if (this._dataSeriesSettings != null)
                    {
                        this._dataSeriesSettings.PropertyChanged -= new PropertyChangedEventHandler(this.DataSeriesSettings_PropertyChanged);
                    }
                    this._dataSeriesSettings = value;
                    if (this._dataSeriesSettings != null)
                    {
                        this._dataSeriesSettings.PropertyChanged += new PropertyChangedEventHandler(this.DataSeriesSettings_PropertyChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the data table settings.
        /// </summary>
        /// <value>
        /// The data table settings.
        /// </value>
        internal Dt.Cells.Data.DataTableSettings DataTableSettings
        {
            get { return  this._dataTableSettings; }
            set
            {
                if (this._dataTableSettings != value)
                {
                    this._dataTableSettings = value;
                }
            }
        }

        internal int DefaultStyleIndex
        {
            get { return  this._defaultStyleIndex; }
            set { this._defaultStyleIndex = value; }
        }

        /// <summary>
        /// Gets or sets what to display the empty cells as.
        /// </summary>
        /// <value>
        /// The display empty cells as value.
        /// </value>
        public EmptyValueStyle DisplayEmptyCellsAs
        {
            get { return  this._displayEmptyCellsAs; }
            set
            {
                if (value != this.DisplayEmptyCellsAs)
                {
                    this._displayEmptyCellsAs = value;
                    this.UpdateElementsEmptyValueStyle(value);
                    base.RaisePropertyChanged("DisplayEmptyCellsAs");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether data in hidden cells is plotted.
        /// </summary>
        /// <value>
        /// <c>true</c> to plot the ; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayHidden
        {
            get { return  this._displayHidden; }
            set
            {
                if (value != this.DisplayHidden)
                {
                    this._displayHidden = value;
                    this.UpdateElementsDisplayHidden(value);
                    this.UpdateDisplayingDataSeries();
                    base.RaisePropertyChanged("DisplayHidden");
                    this.NotifyChartAreaChanged(ChartArea.DataSeries, "");
                }
            }
        }

        /// <summary>
        /// Gets the displaying data series.
        /// </summary>
        /// <value>
        /// The displaying data series.
        /// </value>
        internal IList<SpreadDataSeries> DisplayingDataSeries
        {
            get
            {
                if (this._displayingDataSeries == null)
                {
                    this.UpdateDisplayingDataSeries();
                }
                return this._displayingDataSeries;
            }
        }

        /// <summary>
        /// Excel Area, Line and Stock charts may have Drop line settings.
        /// </summary>
        internal ExcelChartLines DropLine { get; set; }

        /// <summary>
        /// Gets or sets the wall.
        /// </summary>
        /// <value>
        /// The wall.
        /// </value>
        internal Wall FloorWall
        {
            get { return  this._floorWall; }
            set
            {
                this._floorWall = value;
                if ((this._floorWall != null) && !object.ReferenceEquals(this._floorWall.Chart, this))
                {
                    this._floorWall.Chart = this;
                }
                base.RaisePropertyChanged("FloorWall");
            }
        }

        /// <summary>
        /// Gets or sets the chart formula. This action will re-build the chart's series. 
        /// </summary>
        /// <value>
        /// This property is not designed for persisting so you should save and re-use the value and access infrequently.
        /// </value>
        public string Formula
        {
            get
            {
                if (base.Sheet == null)
                {
                    return this._formula;
                }
                return this.GetFormula();
            }
            set
            {
                SpreadChartUtility.ValidateFormula(base.Sheet, value, false);
                this.UpdateFormula(value);
                this._formula = value;
                this.NotifyChartAreaChanged(ChartArea.Chart, "");
            }
        }

        internal int GapDepth { get; set; }

        /// <summary>
        /// Excel stock chart may has High-Low line styles.
        /// </summary>
        internal ExcelChartLines HighLowLine { get; set; }

        internal bool IsDate1904 { get; set; }

        /// <summary>
        /// Gets or sets the items formula. This action will re-build the chart's axis items. 
        /// </summary>
        /// <value>
        /// This property is not designed for persisting so you should save and re-use the value and access infrequently.
        /// </value>
        public string ItemsFormula
        {
            get
            {
                if (base.Sheet == null)
                {
                    return this._itemsFormula;
                }
                string itemsFormula = this.GetItemsFormula();
                if (!string.IsNullOrEmpty(itemsFormula))
                {
                    return itemsFormula;
                }
                return null;
            }
            set
            {
                SpreadChartUtility.ValidateFormula(base.Sheet, value, true);
                if (value != this.ItemsFormula)
                {
                    this._itemsFormula = value;
                    if (SpreadChartUtility.IsBarChart(this.ChartType))
                    {
                        this.AxisY.SetItemsFormulaInternal(value);
                    }
                    else
                    {
                        this.AxisX.SetItemsFormulaInternal(value);
                    }
                    if (SpreadChartUtility.IsBarChart(this.ChartType))
                    {
                        this.NotifyChartAreaChanged(ChartArea.AxisY, "");
                    }
                    else
                    {
                        this.NotifyChartAreaChanged(ChartArea.AxisX, "");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the legend.
        /// </summary>
        /// <value>
        /// The legend of the chart.
        /// </value>
        /// <remarks>
        /// The legend area can contain legend items, background, and border.
        /// </remarks>
        public Dt.Cells.Data.Legend Legend
        {
            get { return  this._legend; }
            set
            {
                if (value != this.Legend)
                {
                    this._legend = value;
                    if ((this._legend != null) && !object.ReferenceEquals(this._legend.Chart, this))
                    {
                        this._legend.Chart = this;
                    }
                    base.RaisePropertyChanged("Legend");
                    this.NotifyChartAreaChanged(ChartArea.Lengend, "");
                }
            }
        }

        /// <summary>
        /// Gets or sets the series name formula. 
        /// </summary>
        /// <value>
        /// This property is not designed for persisting so you should save and re-use the value and access infrequently.
        /// </value>
        public string NameFormula
        {
            get
            {
                if (base.Sheet == null)
                {
                    return this._nameFormula;
                }
                return this.GetNameFormula();
            }
            set
            {
                SpreadChartUtility.ValidateFormula(base.Sheet, value, true);
                if (value != this.NameFormula)
                {
                    this._nameFormula = value;
                    this.UpdateSeriesNameFormula(value, false);
                    this.NotifyChartAreaChanged(ChartArea.DataSeries, "SeriesName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the pie chart settings.
        /// </summary>
        /// <value>
        /// The pie chart settings.
        /// </value>
        internal Dt.Cells.Data.PieChartSettings PieChartSettings
        {
            get
            {
                if (this._pieChartSettings == null)
                {
                    this._pieChartSettings = new Dt.Cells.Data.PieChartSettings();
                    if (SpreadChartUtility.IsPieChart(this.ChartType))
                    {
                        this.VaryColorsByPoint = true;
                        if ((this.ChartType == SpreadChartType.Pie) || (this.ChartType == SpreadChartType.PieDoughnut))
                        {
                            this._pieChartSettings.Explosion = 0.0;
                        }
                        else
                        {
                            this._pieChartSettings.Explosion = 0.25;
                        }
                        if ((this.ChartType == SpreadChartType.PieDoughnut) || (this.ChartType == SpreadChartType.PieExplodedDoughnut))
                        {
                            this._pieChartSettings.HoleSize = 0.5;
                        }
                    }
                    this._pieChartSettings.PropertyChanged += new PropertyChangedEventHandler(this.PieChartSettings_PropertyChanged);
                }
                return this._pieChartSettings;
            }
            set
            {
                if (value != this.PieChartSettings)
                {
                    if (this._pieChartSettings != null)
                    {
                        this._pieChartSettings.PropertyChanged -= new PropertyChangedEventHandler(this.PieChartSettings_PropertyChanged);
                    }
                    this._pieChartSettings = value;
                    if (this._pieChartSettings != null)
                    {
                        this._pieChartSettings.PropertyChanged += new PropertyChangedEventHandler(this.PieChartSettings_PropertyChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the plot area of chart.
        /// </summary>
        /// <value>
        /// The plot area of chart.
        /// </value>
        public Dt.Cells.Data.PlotArea PlotArea
        {
            get
            {
                if (this.PlotAreas.Count == 0)
                {
                    this.PlotAreas.AddElementInternal(new Dt.Cells.Data.PlotArea());
                }
                return this.PlotAreas[0];
            }
            set
            {
                if (value != this.PlotAreas[0])
                {
                    this.PlotAreas[0] = value;
                    base.RaisePropertyChanged("PlotArea");
                }
            }
        }

        internal ObservableCollection<PlotAreaColumnDefinition> PlotAreaColumnDefinitions
        {
            get
            {
                if (this._plotAreaColumnDefinitions == null)
                {
                    this._plotAreaColumnDefinitions = new ObservableCollection<PlotAreaColumnDefinition>();
                }
                return this._plotAreaColumnDefinitions;
            }
        }

        internal Dt.Cells.Data.Layout PlotAreaLayout { get; set; }

        internal ObservableCollection<PlotAreaRowDefinition> PlotAreaRowDefinitions
        {
            get
            {
                if (this._plotAreaRowDefinitions == null)
                {
                    this._plotAreaRowDefinitions = new ObservableCollection<PlotAreaRowDefinition>();
                }
                return this._plotAreaRowDefinitions;
            }
        }

        internal PlotAreaCollection PlotAreas
        {
            get { return  this._plotAreas; }
        }

        internal IExcelChart SecondaryChart
        {
            get { return  this._secondaryChart; }
            set { this._secondaryChart = value; }
        }

        internal ExcelChartLines SeriesLines { get; set; }

        internal bool ShowAutoTitle
        {
            get { return  this._showAutoTitle; }
            set
            {
                if (value != this._showAutoTitle)
                {
                    this._showAutoTitle = value;
                    base.RaisePropertyChanged("ShowAutoTitle");
                }
            }
        }

        internal bool ShowDataLabelOverMax
        {
            get { return  this._showDataLabelOverMax; }
            set
            {
                if (this._showDataLabelOverMax != value)
                {
                    this._showDataLabelOverMax = value;
                    base.RaisePropertyChanged("ShowDataLablesOverMax");
                }
            }
        }

        /// <summary>
        /// Gets or sets the side wall.
        /// </summary>
        /// <value>
        /// The side wall.
        /// </value>
        internal Wall SideWall
        {
            get { return  this._sideWall; }
            set
            {
                this._sideWall = value;
                if ((this._sideWall != null) && !object.ReferenceEquals(this._sideWall.Chart, this))
                {
                    this._sideWall.Chart = this;
                }
                base.RaisePropertyChanged("SideWall");
            }
        }

        internal UpDownBars UpDownDarsSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [vary colors by point].
        /// </summary>
        /// <value>
        /// <c>true</c> if [vary colors by point]; otherwise, <c>false</c>.
        /// </value>
        internal bool VaryColorsByPoint
        {
            get { return  this._varyColorsByPoint; }
            set
            {
                if (value != this.VaryColorsByPoint)
                {
                    this._varyColorsByPoint = value;
                    base.RaisePropertyChanged("VaryColorsByPoint");
                }
            }
        }
    }
}

