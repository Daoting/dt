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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Base class for axes.
    /// </summary>
    public class Axis : SpreadChartTextElement, IXmlSerializable, IRangeSupport, IDisposable
    {
        Dt.Cells.Data.AxisType _autoAxisType;
        bool _autoBaseTimeUnit;
        bool _autoMajorUint;
        bool _automaticFill;
        bool _automaticStroke;
        bool _autoMax;
        bool _autoMin;
        bool _autoMinorUnit;
        Dt.Cells.Data.AxisPosition _axisPosition;
        Dt.Cells.Data.AxisType? _axisType;
        Dt.Cells.Data.AxisTimeUnit _baseTimeUnit;
        Dt.Cells.Data.AxisCrosses _corsses;
        double _crossAt;
        AxisCrossBetween _crossBetween;
        bool _displayHidden;
        Dt.Cells.Data.DisplayUnitSettings _displayUnitSettings;
        Dt.Cells.Data.EmptyValueStyle _emptyValueStyle;
        bool _hasMajorGridLine;
        bool _hasMinorGridLine;
        bool _isMajorGridStrokeAutomatic;
        bool _isMinorGridStrokeAutomatic;
        AxisItemsCollection _items;
        string _itemsFormula;
        DataOrientation? _itemsOrientation;
        SheetCellRange[] _itemsRange;
        CalcExpression _itemsReference;
        double _labelAngle;
        IFormatter _labelFormatter;
        AxisLabelPosition _labelPosition;
        int _lableOffset;
        double _logBase;
        ArrowSettings _majorGridLineBeginArrowSettings;
        PenLineCap _majorGridLineCapType;
        ArrowSettings _majorGridLineEndArrowSettings;
        PenLineJoin _majorGridLineJoinType;
        Brush _majorGridStroke;
        StrokeDashType _majorGridStrokeDashType;
        string _majorGridStrokeThemeColor;
        double _majorGridStrokeThickness;
        double _majorTickHeight;
        AxisTickPosition _majorTickPosition;
        Brush _majorTickStroke;
        double _majorTickThickness;
        Dt.Cells.Data.AxisTimeUnit _majorTimeUnit;
        double _majorUnit;
        double _max;
        int _maxDataPointCount;
        double _min;
        ArrowSettings _minorGridLineBeginArrowSettings;
        PenLineCap _minorGridLineCapType;
        ArrowSettings _minorGridLineEndArrowSettings;
        PenLineJoin _minorGridLineJoinType;
        Brush _minorGridStroke;
        StrokeDashType _minorGridStrokeDashType;
        string _minorGridStrokeThemeColor;
        double _minorGridStrokeThickness;
        double _minorTickHeight;
        AxisTickPosition _minorTickPosition;
        Brush _minorTickStroke;
        double _minorTickThickness;
        Dt.Cells.Data.AxisTimeUnit _minorTimeUnit;
        double _minorUnit;
        double _minScale;
        Dt.Cells.Data.AxisOrientation _orientation;
        bool _overlapData;
        bool _reversed;
        double _scale;
        bool _showAxisLabel;
        int _tickLabelInterval;
        int _tickMarkInterval;
        ChartTitle _title;
        bool _useCustomItems;
        bool _useLogBase;
        bool _visible;
        const double DefaultLogBase = 10.0;
        internal bool IsAumaticCategoryAxis;
        internal bool NoMultiLevelLables;
        const int ONE_MONTH = 0x1c;
        const int ONE_WEEK = 7;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Axis" /> class.
        /// </summary>
        public Axis()
        {
            this._itemsOrientation = 0;
            this._automaticFill = true;
            this._automaticStroke = true;
            this._autoMajorUint = true;
            this._autoMinorUnit = true;
            this._autoMax = true;
            this._autoMin = true;
            this._showAxisLabel = true;
            this._logBase = 10.0;
            this._majorGridStrokeThickness = 0.5;
            this._majorGridLineJoinType = PenLineJoin.Round;
            this._isMajorGridStrokeAutomatic = true;
            this._hasMajorGridLine = true;
            this._majorTickHeight = 7.0;
            this._majorTickPosition = AxisTickPosition.OutSide;
            this._majorTickStroke = SpreadChartElement.DefaultAutomaticStroke;
            this._majorTickThickness = 0.5;
            this._majorUnit = 1.0;
            this._minorGridStrokeThickness = 0.5;
            this._minorGridLineJoinType = PenLineJoin.Round;
            this._isMinorGridStrokeAutomatic = true;
            this._minorTickHeight = 4.0;
            this._minorTickStroke = SpreadChartElement.DefaultAutomaticStroke;
            this._minorTickThickness = 0.5;
            this._minorUnit = 0.02;
            this._autoBaseTimeUnit = true;
            this._min = double.NaN;
            this._max = double.NaN;
            this._minScale = double.NaN;
            this._scale = 1.0;
            this._crossAt = double.NaN;
            this._visible = true;
            this._axisType = 0;
            this.IsAumaticCategoryAxis = true;
            this.NoMultiLevelLables = true;
            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Axis" /> class.
        /// </summary>
        /// <param name="axisType">Type of the axis.</param>
        public Axis(Dt.Cells.Data.AxisType axisType)
        {
            this._itemsOrientation = 0;
            this._automaticFill = true;
            this._automaticStroke = true;
            this._autoMajorUint = true;
            this._autoMinorUnit = true;
            this._autoMax = true;
            this._autoMin = true;
            this._showAxisLabel = true;
            this._logBase = 10.0;
            this._majorGridStrokeThickness = 0.5;
            this._majorGridLineJoinType = PenLineJoin.Round;
            this._isMajorGridStrokeAutomatic = true;
            this._hasMajorGridLine = true;
            this._majorTickHeight = 7.0;
            this._majorTickPosition = AxisTickPosition.OutSide;
            this._majorTickStroke = SpreadChartElement.DefaultAutomaticStroke;
            this._majorTickThickness = 0.5;
            this._majorUnit = 1.0;
            this._minorGridStrokeThickness = 0.5;
            this._minorGridLineJoinType = PenLineJoin.Round;
            this._isMinorGridStrokeAutomatic = true;
            this._minorTickHeight = 4.0;
            this._minorTickStroke = SpreadChartElement.DefaultAutomaticStroke;
            this._minorTickThickness = 0.5;
            this._minorUnit = 0.02;
            this._autoBaseTimeUnit = true;
            this._min = double.NaN;
            this._max = double.NaN;
            this._minScale = double.NaN;
            this._scale = 1.0;
            this._crossAt = double.NaN;
            this._visible = true;
            this._axisType = 0;
            this.IsAumaticCategoryAxis = true;
            this.NoMultiLevelLables = true;
            this.Init();
            this._axisType = new Dt.Cells.Data.AxisType?(axisType);
        }

        internal Axis(SpreadChart owner, Dt.Cells.Data.AxisOrientation orientation) : base(owner)
        {
            this._itemsOrientation = 0;
            this._automaticFill = true;
            this._automaticStroke = true;
            this._autoMajorUint = true;
            this._autoMinorUnit = true;
            this._autoMax = true;
            this._autoMin = true;
            this._showAxisLabel = true;
            this._logBase = 10.0;
            this._majorGridStrokeThickness = 0.5;
            this._majorGridLineJoinType = PenLineJoin.Round;
            this._isMajorGridStrokeAutomatic = true;
            this._hasMajorGridLine = true;
            this._majorTickHeight = 7.0;
            this._majorTickPosition = AxisTickPosition.OutSide;
            this._majorTickStroke = SpreadChartElement.DefaultAutomaticStroke;
            this._majorTickThickness = 0.5;
            this._majorUnit = 1.0;
            this._minorGridStrokeThickness = 0.5;
            this._minorGridLineJoinType = PenLineJoin.Round;
            this._isMinorGridStrokeAutomatic = true;
            this._minorTickHeight = 4.0;
            this._minorTickStroke = SpreadChartElement.DefaultAutomaticStroke;
            this._minorTickThickness = 0.5;
            this._minorUnit = 0.02;
            this._autoBaseTimeUnit = true;
            this._min = double.NaN;
            this._max = double.NaN;
            this._minScale = double.NaN;
            this._scale = 1.0;
            this._crossAt = double.NaN;
            this._visible = true;
            this._axisType = 0;
            this.IsAumaticCategoryAxis = true;
            this.NoMultiLevelLables = true;
            this._orientation = orientation;
            this.Init();
        }

        void AddColumnRange(int column, int columnCount)
        {
            if (!string.IsNullOrEmpty(this.ItemsFormula))
            {
                string itemsFormula = FormulaUtility.AddColumnRange(this.Sheet, this.ItemsFormula, column, columnCount);
                this.SetItemsFormulaInternal(itemsFormula);
            }
        }

        void AddRowRange(int row, int rowCount)
        {
            if (!string.IsNullOrEmpty(this.ItemsFormula))
            {
                string itemsFormula = FormulaUtility.AddRowRange(this.Sheet, this.ItemsFormula, row, rowCount);
                this.SetItemsFormulaInternal(itemsFormula);
            }
        }

        internal void AdjustMinMax(out double min, out double max)
        {
            if (this.Min > this.Max)
            {
                min = this.Max;
                max = this.Min;
            }
            else
            {
                min = this.Min;
                max = this.Max;
            }
        }

        internal override void AfterReadXml()
        {
            base.AfterReadXml();
            base.ResumeEvents();
        }

        internal override void BeforeReadXml()
        {
            base.BeforeReadXml();
            this.Init();
            base.SuspendEvents();
        }

        internal void ClearItemsFormula()
        {
            this.ItemsReference = null;
            this._itemsFormula = null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Items.Dispose();
        }

        void FormatInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyAxisChanged(e.PropertyName);
        }

        int GetDecimalPlaces(double value)
        {
            char ch = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator[0];
            return ((double) value).ToString((IFormatProvider) CultureInfo.InvariantCulture).Split(new char[] { ch }).Length;
        }

        internal int GetItemsCount(DateTime min, DateTime max)
        {
            DateTime time = min;
            DateTime time2 = max;
            List<DateTime> list = new List<DateTime>();
            while (time <= time2)
            {
                list.Add(time);
                switch (this.MajorTimeUnit)
                {
                    case Dt.Cells.Data.AxisTimeUnit.Days:
                        time = time.AddDays((double) ((int) this.MajorUnit));
                        break;

                    case Dt.Cells.Data.AxisTimeUnit.Months:
                        time = time.AddMonths((int) this.MajorUnit);
                        break;

                    case Dt.Cells.Data.AxisTimeUnit.Years:
                        time = time.AddYears((int) this.MajorUnit);
                        break;
                }
            }
            return list.Count;
        }

        int GetNumbersCount(double number)
        {
            return ((double) number).ToString().Length;
        }

        void IRangeSupport.AddColumns(int column, int count)
        {
            this.AddColumnRange(column, count);
        }

        void IRangeSupport.AddRows(int row, int count)
        {
            this.AddRowRange(row, count);
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
            this.RemoveColumnRange(column, count);
        }

        void IRangeSupport.RemoveRows(int row, int count)
        {
            this.RemoveRowRange(row, count);
        }

        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        void Init()
        {
            this._itemsFormula = null;
            this._items = new AxisItemsCollection(this);
            this._items.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Items_CollectionChanged);
            this._useCustomItems = false;
            this._labelAngle = 0.0;
            this._labelPosition = AxisLabelPosition.Auto;
            this._autoMax = true;
            this._autoMin = true;
            this._logBase = 10.0;
            this._majorGridStroke = null;
            this._majorGridStrokeThemeColor = null;
            this._majorGridStrokeDashType = StrokeDashType.None;
            this._majorGridStrokeThickness = 0.5;
            this._majorGridLineCapType = PenLineCap.Flat;
            this._majorGridLineJoinType = PenLineJoin.Round;
            this._majorGridLineBeginArrowSettings = null;
            this._majorGridLineEndArrowSettings = null;
            this._majorTickHeight = 7.0;
            this._majorTickPosition = AxisTickPosition.OutSide;
            this._majorTickStroke = SpreadChartElement.DefaultAutomaticStroke;
            this._majorTickThickness = 0.5;
            this._majorUnit = 1.0;
            this._isMajorGridStrokeAutomatic = true;
            this._minorGridStroke = null;
            this._minorGridStrokeThemeColor = null;
            this._minorGridStrokeDashType = StrokeDashType.None;
            this._minorGridStrokeThickness = 0.5;
            this._minorGridLineCapType = PenLineCap.Flat;
            this._minorGridLineJoinType = PenLineJoin.Round;
            this._minorGridLineBeginArrowSettings = null;
            this._minorGridLineEndArrowSettings = null;
            this._minorTickHeight = 4.0;
            this._minorTickPosition = AxisTickPosition.None;
            this._minorTickStroke = SpreadChartElement.DefaultAutomaticStroke;
            this._minorTickThickness = 0.5;
            this._minorUnit = 0.02;
            this._isMinorGridStrokeAutomatic = true;
            this._min = double.NaN;
            this._max = double.NaN;
            this._minScale = double.NaN;
            this._scale = 1.0;
            this._crossAt = double.NaN;
            this._corsses = Dt.Cells.Data.AxisCrosses.AutoZero;
            this._reversed = false;
            this._visible = true;
            this._axisType = null;
            this._autoAxisType = Dt.Cells.Data.AxisType.Category;
            this._majorTimeUnit = Dt.Cells.Data.AxisTimeUnit.Days;
            this._minorTimeUnit = Dt.Cells.Data.AxisTimeUnit.Days;
            this._axisPosition = Dt.Cells.Data.AxisPosition.Near;
            this._crossBetween = AxisCrossBetween.Between;
            this._title = null;
            this._tickLabelInterval = 0;
            this._tickMarkInterval = 0;
            this._labelFormatter = new AutoFormatter(new GeneralFormatter());
            this._displayUnitSettings = null;
            this._hasMinorGridLine = false;
            this._hasMajorGridLine = true;
        }

        void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ((ISpreadChartElement) this).NotifyElementChanged("Items");
        }

        internal void ModifyBaseTimeUnit(DateTime min, DateTime max)
        {
            for (int i = this.GetItemsCount(min, max); (i < this.MaxDataPointCount) && (this._baseTimeUnit != Dt.Cells.Data.AxisTimeUnit.Days); i = this.GetItemsCount(min, max))
            {
                if (this._baseTimeUnit == Dt.Cells.Data.AxisTimeUnit.Years)
                {
                    this._baseTimeUnit = Dt.Cells.Data.AxisTimeUnit.Months;
                }
                else if (this._baseTimeUnit == Dt.Cells.Data.AxisTimeUnit.Months)
                {
                    this._baseTimeUnit = Dt.Cells.Data.AxisTimeUnit.Days;
                    TimeSpan span = (TimeSpan) (max - min);
                    if (span.Days >= 0x1c)
                    {
                        this._majorUnit = 7.0;
                    }
                    else
                    {
                        this._majorUnit = 1.0;
                    }
                }
            }
        }

        internal void NotifyAxisChanged(string changed)
        {
            if (!base.IsEventsSuspend())
            {
                ((ISpreadChartElement) this).NotifyElementChanged(changed);
            }
        }

        internal override void OnChartChanged()
        {
            this.UpdateItemsReference();
        }

        internal override void OnResumeAfterDeserialization()
        {
            base.OnResumeAfterDeserialization();
            this.UpdateItemsReference();
            if (this.Title != null)
            {
                this.Title.ResumeAfterDeserialization();
            }
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            base.ReadXmlInternal(reader);
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
            {
                switch (reader.Name)
                {
                    case "ItemsFormula":
                        this._itemsFormula = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                        return;

                    case "Items":
                        Serializer.DeserializeList(this._items, reader);
                        return;

                    case "UseCustomItems":
                        this._useCustomItems = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "AxisType":
                        this._axisType = new Dt.Cells.Data.AxisType?((Dt.Cells.Data.AxisType) Serializer.DeserializeObj(typeof(Dt.Cells.Data.AxisType), reader));
                        return;

                    case "FloatingObjectStyleInfo":
                        base._styleInfo = (FloatingObjectStyleInfo) Serializer.DeserializeObj(typeof(FloatingObjectStyleInfo), reader);
                        return;

                    case "IsAutomaticFill":
                        this._automaticFill = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "IsAutomaticStroke":
                        this._automaticStroke = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "DisplayHidden":
                        this._displayHidden = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "DisplayEmptyCellsAs":
                        this._emptyValueStyle = (Dt.Cells.Data.EmptyValueStyle) Serializer.DeserializeObj(typeof(Dt.Cells.Data.EmptyValueStyle), reader);
                        return;

                    case "Orientation":
                        this._orientation = (Dt.Cells.Data.AxisOrientation) Serializer.DeserializeObj(typeof(Dt.Cells.Data.AxisOrientation), reader);
                        return;

                    case "LabelAngle":
                        this._labelAngle = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "LabelPosition":
                        this._labelPosition = (AxisLabelPosition) Serializer.DeserializeObj(typeof(AxisLabelPosition), reader);
                        return;

                    case "LabelFormatter":
                    {
                        string format = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                        if (format == null)
                        {
                            break;
                        }
                        string str2 = "[Auto]";
                        bool flag = format.StartsWith(str2);
                        if (flag)
                        {
                            format = format.Remove(0, str2.Length);
                        }
                        this._labelFormatter = new GeneralFormatter(format);
                        if (!flag)
                        {
                            break;
                        }
                        this._labelFormatter = new AutoFormatter(this._labelFormatter as GeneralFormatter);
                        return;
                    }
                    case "AutoMax":
                        this._autoMax = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "AutoMin":
                        this._autoMin = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "LogBase":
                        this._logBase = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "UseLogBase":
                        this._useLogBase = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "IsMajorGridStrokeAutomatic":
                        this._isMajorGridStrokeAutomatic = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "IsMinorGridStrokeAutomatic":
                        this._isMinorGridStrokeAutomatic = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "HasMajorGridlines":
                        this._hasMajorGridLine = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "HasMinorGridlines":
                        this._hasMinorGridLine = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "MajorGridStroke":
                        _majorGridStroke = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                        return;

                    case "MajorGridStrokeThemeColor":
                        this._majorGridStrokeThemeColor = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
                        return;

                    case "MajorGridStrokeDashes":
                        this._majorGridStrokeDashType = (StrokeDashType) Serializer.DeserializeObj(typeof(StrokeDashType), reader);
                        return;

                    case "MajorGridStrokeThickness":
                        this._majorGridStrokeThickness = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "MajorGridLineCapType":
                        this._majorGridLineCapType = (PenLineCap) Serializer.DeserializeObj(typeof(PenLineCap), reader);
                        return;

                    case "MajorGridLineJoinType":
                        this._majorGridLineJoinType = (PenLineJoin) Serializer.DeserializeObj(typeof(PenLineJoin), reader);
                        return;

                    case "MajorGridLineBeginArrowSettings":
                        this._majorGridLineBeginArrowSettings = (ArrowSettings) Serializer.DeserializeObj(typeof(ArrowSettings), reader);
                        return;

                    case "MajorGridLineEndArrowSettings":
                        this._majorGridLineEndArrowSettings = (ArrowSettings) Serializer.DeserializeObj(typeof(ArrowSettings), reader);
                        return;

                    case "MajorTickHeight":
                        this._majorTickHeight = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "MajorTickPosition":
                        this._majorTickPosition = (AxisTickPosition) Serializer.DeserializeObj(typeof(AxisTickPosition), reader);
                        return;

                    case "MajorTickStroke":
                        _majorTickStroke = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                        return;

                    case "MajorTickThickness":
                        this._majorTickThickness = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "MajorUnit":
                        this._majorUnit = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "MinorGridStroke":
                        _minorGridStroke = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                        return;

                    case "MinorGridStrokeThemeColor":
                        this._minorGridStrokeThemeColor = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
                        return;

                    case "MinorGridStrokeDashes":
                        this._minorGridStrokeDashType = (StrokeDashType) Serializer.DeserializeObj(typeof(StrokeDashType), reader);
                        return;

                    case "MinorGridStrokeThickness":
                        this._minorGridStrokeThickness = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "MinorGridLineCapType":
                        this._minorGridLineCapType = (PenLineCap) Serializer.DeserializeObj(typeof(PenLineCap), reader);
                        return;

                    case "MinorGridLineJoinType":
                        this._minorGridLineJoinType = (PenLineJoin) Serializer.DeserializeObj(typeof(PenLineJoin), reader);
                        return;

                    case "MinorGridLineBeginArrowSettings":
                        this._minorGridLineBeginArrowSettings = (ArrowSettings) Serializer.DeserializeObj(typeof(ArrowSettings), reader);
                        return;

                    case "MinorGridLineEndArrowSettings":
                        this._minorGridLineEndArrowSettings = (ArrowSettings) Serializer.DeserializeObj(typeof(ArrowSettings), reader);
                        return;

                    case "MinorTickHeight":
                        this._minorTickHeight = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "MinorTickPosition":
                        this._minorTickPosition = (AxisTickPosition) Serializer.DeserializeObj(typeof(AxisTickPosition), reader);
                        return;

                    case "MinorTickStroke":
                        _minorTickStroke = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                        return;

                    case "MinorTickThickness":
                        this._minorTickThickness = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "MinorUnit":
                        this._minorUnit = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "Min":
                        this._min = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "Max":
                        this._max = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "MinScale":
                        this._minScale = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "Scale":
                        this._scale = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "CrossAt":
                        this._crossAt = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "Crosses":
                        this._corsses = (Dt.Cells.Data.AxisCrosses) Serializer.DeserializeObj(typeof(Dt.Cells.Data.AxisCrosses), reader);
                        return;

                    case "Reversed":
                        this._reversed = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "IsVisible":
                        this._visible = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "BaseTimeUnit":
                        this._baseTimeUnit = (Dt.Cells.Data.AxisTimeUnit) Serializer.DeserializeObj(typeof(Dt.Cells.Data.AxisTimeUnit), reader);
                        return;

                    case "MajorTimeUnit":
                        this._majorTimeUnit = (Dt.Cells.Data.AxisTimeUnit) Serializer.DeserializeObj(typeof(Dt.Cells.Data.AxisTimeUnit), reader);
                        return;

                    case "MinorTimeUnit":
                        this._minorTimeUnit = (Dt.Cells.Data.AxisTimeUnit) Serializer.DeserializeObj(typeof(Dt.Cells.Data.AxisTimeUnit), reader);
                        return;

                    case "AxisPosition":
                        this._axisPosition = (Dt.Cells.Data.AxisPosition) Serializer.DeserializeObj(typeof(Dt.Cells.Data.AxisPosition), reader);
                        return;

                    case "Title":
                        this._title = (ChartTitle) Serializer.DeserializeObj(typeof(ChartTitle), reader);
                        return;

                    case "TickLabelInterval":
                        this._tickLabelInterval = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
                        return;

                    case "TickMarkInterval":
                        this._tickMarkInterval = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
                        return;

                    case "DisplayUnit":
                        if (this._displayUnitSettings == null)
                        {
                            this._displayUnitSettings = new Dt.Cells.Data.DisplayUnitSettings();
                        }
                        this._displayUnitSettings.DisplayUnit = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        return;

                    case "OverlapData":
                        this._overlapData = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "MajorGridlineDrawingColorSettings":
                        this.MajorGridlineDrawingColorSettings = (SpreadDrawingColorSettings) Serializer.DeserializeObj(typeof(SpreadDrawingColorSettings), reader);
                        return;

                    case "MinorGridelineDrawingColorSettings":
                        this.MinorGridlineDrawingColorSettings = (SpreadDrawingColorSettings) Serializer.DeserializeObj(typeof(SpreadDrawingColorSettings), reader);
                        break;

                    default:
                        return;
                }
            }
        }

        internal void ReLoadItemsFormula()
        {
            if (!string.IsNullOrEmpty(this.ItemsFormula))
            {
                this.ItemsReference = FormulaUtility.Formula2Expression(this.Sheet, this.ItemsFormula);
            }
        }

        void RemoveColumnRange(int column, int columnCount)
        {
            if (!string.IsNullOrEmpty(this.ItemsFormula))
            {
                string itemsFormula = FormulaUtility.RemoveColumnRange(this.Sheet, this.ItemsFormula, column, columnCount);
                this.SetItemsFormulaInternal(itemsFormula);
            }
        }

        void RemoveRowRange(int row, int rowCount)
        {
            if (!string.IsNullOrEmpty(this.ItemsFormula))
            {
                string itemsFormula = FormulaUtility.RemoveRowRange(this.Sheet, this.ItemsFormula, row, rowCount);
                this.SetItemsFormulaInternal(itemsFormula);
            }
        }

        /// <summary>
        /// Resets the type of the axis.
        /// </summary>
        public void ResetAxisType()
        {
            this._axisType = null;
        }

        /// <summary>
        /// Resets the display unit settings.
        /// </summary>
        public void ResetDisplayUnitSettings()
        {
            this._displayUnitSettings = null;
        }

        internal void SetAutoAxisTypeInternal(Dt.Cells.Data.AxisType type)
        {
            this._autoAxisType = type;
        }

        internal override void SetChartInternal(SpreadChartBase chart)
        {
            base.SetChartInternal(chart);
            if (this.Title != null)
            {
                this.Title.SetChartInternal(chart);
            }
        }

        internal void SetItemsFormulaInternal(string itemsFormula)
        {
            if (!string.IsNullOrEmpty(itemsFormula))
            {
                this.ItemsReference = FormulaUtility.Formula2Expression(this.Sheet, itemsFormula);
                this._itemsFormula = itemsFormula;
            }
            else
            {
                this.ClearItemsFormula();
            }
        }

        internal void SetMajorUnitInteral(double majorUnit)
        {
            this._majorUnit = majorUnit;
        }

        internal void SetMaxInternal(double max)
        {
            this._max = max;
        }

        internal void SetMinInternal(double min)
        {
            this._min = min;
        }

        internal void SetMinorUnitInternal(double minorUnit)
        {
            this._minorUnit = minorUnit;
        }

        internal void Update100PercentMajorMinorUnit(double min, double max)
        {
            double num = max - min;
            if (this.AutoMajorUnit)
            {
                double majorUnit = num / 10.0;
                this.SetMajorUnitInteral(majorUnit);
            }
            if (this.AutoMinorUnit)
            {
                double minorUnit = this.MajorUnit / 5.0;
                this.SetMinorUnitInternal(minorUnit);
            }
        }

        internal void UpdateDateTimeMajorMinorUnit(DateTime min, DateTime max)
        {
            if (this.AutoBaseTimeUnit)
            {
                if (min.Year != max.Year)
                {
                    this._baseTimeUnit = Dt.Cells.Data.AxisTimeUnit.Years;
                }
                else if (min.Month != max.Month)
                {
                    this._baseTimeUnit = Dt.Cells.Data.AxisTimeUnit.Months;
                }
                else
                {
                    this._baseTimeUnit = Dt.Cells.Data.AxisTimeUnit.Days;
                }
            }
            if (this.AutoMajorUnit)
            {
                if (this.MajorTimeUnit == Dt.Cells.Data.AxisTimeUnit.Days)
                {
                    TimeSpan span = (TimeSpan) (max - min);
                    if (span.Days >= 0x1c)
                    {
                        this._majorUnit = 7.0;
                    }
                    else
                    {
                        this._majorUnit = 1.0;
                    }
                }
                else
                {
                    this._majorUnit = 1.0;
                }
            }
            if (this.AutoMinorUnit)
            {
                this._minorUnit = 1.0;
            }
            if (this.AutoBaseTimeUnit)
            {
                this.ModifyBaseTimeUnit(min, max);
            }
        }

        internal void UpdateDoubleMajorMinorUnit(double min, double max)
        {
            double majorUnit = this.AutoMajorUnit ? 1.0 : this.MajorUnit;
            double minorUnit = this.AutoMinorUnit ? 1.0 : this.MinorUnit;
            majorUnit = AxisUtility.CalculateValidMajorUnit(majorUnit, false, this.LogBase);
            minorUnit = AxisUtility.CalculateValidMinorUnit(minorUnit, false, this.LogBase);
            if (this.AutoMajorUnit)
            {
                majorUnit = AxisUtility.CalculateMajorUnit(min, max, this.AutoMinorUnit, minorUnit, false, this.LogBase);
                if (majorUnit > 0.0)
                {
                    this._majorUnit = majorUnit;
                }
            }
            if (this.AutoMinorUnit)
            {
                minorUnit = AxisUtility.CalculateMinorUnit(min, max, majorUnit, false);
                if (this._minorUnit > 0.0)
                {
                    this._minorUnit = minorUnit;
                }
            }
        }

        internal void UpdateItems()
        {
            if (this.Items != null)
            {
                this.Items.UpdateItemsForAxis();
            }
        }

        internal void UpdateItemsReference()
        {
            this.ItemsReference = FormulaUtility.Formula2Expression(this.Sheet, this.ItemsFormula);
        }

        internal void UpdateMajorMinorUnit(double min, double max)
        {
            if ((min != double.MaxValue) && (max != double.MinValue))
            {
                if ((this.AxisType == Dt.Cells.Data.AxisType.Value) || (this.AxisType == Dt.Cells.Data.AxisType.Category))
                {
                    this.UpdateDoubleMajorMinorUnit(min, max);
                }
                else if (this.AxisType == Dt.Cells.Data.AxisType.Date)
                {
                    DateTime time = DateTimeExtension.FromOADate(min);
                    DateTime time2 = DateTimeExtension.FromOADate(max);
                    this.UpdateDateTimeMajorMinorUnit(time, time2);
                }
            }
        }

        internal void UpdateMinMax()
        {
            this.Items.UpdateAxisMinMax();
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (this._min != double.NaN)
            {
                Serializer.SerializeObj((double) this._min, "Min", writer);
            }
            if (this._max != double.NaN)
            {
                Serializer.SerializeObj((double) this._max, "Max", writer);
            }
            if (!string.IsNullOrEmpty(this._itemsFormula))
            {
                Serializer.SerializeObj(this._itemsFormula, "ItemsFormula", writer);
            }
            else if ((this._items != null) && (this._items.Count > 0))
            {
                Serializer.SerializeList(this._items, "Items", writer);
            }
            if (this._useCustomItems)
            {
                Serializer.SerializeObj((bool) this._useCustomItems, "UseCustomItems", writer);
            }
            if (this._displayHidden)
            {
                Serializer.SerializeObj((bool) this._displayHidden, "DisplayHidden", writer);
            }
            if (this._displayUnitSettings != null)
            {
                Serializer.SerializeObj((double) this._displayUnitSettings.DisplayUnit, "DisplayUnit", writer);
            }
            if (this._emptyValueStyle != Dt.Cells.Data.EmptyValueStyle.Gaps)
            {
                Serializer.SerializeObj(this._emptyValueStyle, "DisplayEmptyCellsAs", writer);
            }
            if (this._axisType.HasValue)
            {
                Serializer.SerializeObj(this._axisType, "AxisType", writer);
            }
            Serializer.SerializeObj(this._orientation, "Orientation", writer);
            if (this._labelAngle != 0.0)
            {
                Serializer.SerializeObj((double) this._labelAngle, "LabelAngle", writer);
            }
            if (this._labelFormatter != null)
            {
                string formatString = this._labelFormatter.FormatString;
                if (this._labelFormatter is AutoFormatter)
                {
                    formatString = formatString.Insert(0, "[Auto]");
                }
                Serializer.SerializeObj(formatString, "LabelFormatter", writer);
            }
            if (this._labelPosition != AxisLabelPosition.Auto)
            {
                Serializer.SerializeObj(this._labelPosition, "LabelPosition", writer);
            }
            if (!this._autoMax)
            {
                Serializer.SerializeObj((bool) this._autoMax, "AutoMax", writer);
            }
            if (!this._autoMin)
            {
                Serializer.SerializeObj((bool) this._autoMin, "AutoMin", writer);
            }
            if (this._logBase != 10.0)
            {
                Serializer.SerializeObj((double) this._logBase, "LogBase", writer);
            }
            if (this._useLogBase)
            {
                Serializer.SerializeObj((bool) this._useLogBase, "UseLogBase", writer);
            }
            if (!this._isMajorGridStrokeAutomatic)
            {
                Serializer.SerializeObj((bool) this._isMajorGridStrokeAutomatic, "IsMajorGridStrokeAutomatic", writer);
            }
            if (!this._isMinorGridStrokeAutomatic)
            {
                Serializer.SerializeObj((bool) this._isMinorGridStrokeAutomatic, "IsMinorGridStrokeAutomatic", writer);
            }
            if (!this._hasMajorGridLine)
            {
                Serializer.SerializeObj((bool) this._hasMajorGridLine, "HasMajorGridlines", writer);
            }
            if (this._hasMinorGridLine)
            {
                Serializer.SerializeObj((bool) this._hasMinorGridLine, "HasMinorGridlines", writer);
            }
            if (!string.IsNullOrEmpty(this._majorGridStrokeThemeColor))
            {
                Serializer.SerializeObj(this._majorGridStrokeThemeColor, "MajorGridStrokeThemeColor", writer);
            }
            if (this._majorGridStroke != null)
            {
                Serializer.SerializeObj(this._majorGridStroke, "MajorGridStroke", writer);
            }
            if (this._majorGridStrokeDashType != StrokeDashType.None)
            {
                Serializer.SerializeObj(this._majorGridStrokeDashType, "MajorGridStrokeDashes", writer);
            }
            if (this._majorGridStrokeThickness != 0.5)
            {
                Serializer.SerializeObj((double) this._majorGridStrokeThickness, "MajorGridStrokeThickness", writer);
            }
            if (this._majorGridLineCapType != PenLineCap.Flat)
            {
                Serializer.SerializeObj(this._majorGridLineCapType, "MajorGridLineCapType", writer);
            }
            if (this._majorGridLineJoinType != PenLineJoin.Round)
            {
                Serializer.SerializeObj(this._majorGridLineJoinType, "MajorGridLineJoinType", writer);
            }
            if (this._majorGridLineBeginArrowSettings != null)
            {
                Serializer.SerializeObj(this._majorGridLineBeginArrowSettings, "MajorGridLineBeginArrowSettings", writer);
            }
            if (this._majorGridLineEndArrowSettings != null)
            {
                Serializer.SerializeObj(this._majorGridLineEndArrowSettings, "MajorGridLineEndArrowSettings", writer);
            }
            if (this._majorTickHeight != 7.0)
            {
                Serializer.SerializeObj((double) this._majorTickHeight, "MajorTickHeight", writer);
            }
            if (this._majorTickPosition != AxisTickPosition.OutSide)
            {
                Serializer.SerializeObj(this._majorTickPosition, "MajorTickPosition", writer);
            }
            if (this._majorTickStroke != null)
            {
                Serializer.SerializeObj(this._majorTickStroke, "MajorTickStroke", writer);
            }
            if (this._majorTickThickness != 0.5)
            {
                Serializer.SerializeObj((double) this._majorTickThickness, "MajorTickThickness", writer);
            }
            if (this._majorUnit != double.NaN)
            {
                Serializer.SerializeObj((double) this._majorUnit, "MajorUnit", writer);
            }
            if (this._minorGridStroke != null)
            {
                Serializer.SerializeObj(this._minorGridStroke, "MinorGridStroke", writer);
            }
            if (!string.IsNullOrEmpty(this._minorGridStrokeThemeColor))
            {
                Serializer.SerializeObj(this._minorGridStrokeThemeColor, "MinorGridStrokeThemeColor", writer);
            }
            if (this._minorGridStrokeDashType != StrokeDashType.None)
            {
                Serializer.SerializeObj(this._minorGridStrokeDashType, "MajorGridStrokeDashes", writer);
            }
            if (this._minorGridStrokeThickness != 0.5)
            {
                Serializer.SerializeObj((double) this._minorGridStrokeThickness, "MinorGridStrokeThickness", writer);
            }
            if (this._minorGridLineCapType != PenLineCap.Flat)
            {
                Serializer.SerializeObj(this._minorGridLineCapType, "MinorGridLineCapType", writer);
            }
            if (this._minorGridLineJoinType != PenLineJoin.Round)
            {
                Serializer.SerializeObj(this._minorGridLineJoinType, "MinorGridLineJoinType", writer);
            }
            if (this._minorGridLineBeginArrowSettings != null)
            {
                Serializer.SerializeObj(this._minorGridLineBeginArrowSettings, "MinorGridLineBeginArrowSettings", writer);
            }
            if (this._minorGridLineEndArrowSettings != null)
            {
                Serializer.SerializeObj(this._minorGridLineEndArrowSettings, "MinorGridLineEndArrowSettings", writer);
            }
            if (this._minorTickHeight != 4.0)
            {
                Serializer.SerializeObj((double) this._minorTickHeight, "MinorTickHeight", writer);
            }
            if (this._minorTickPosition != AxisTickPosition.None)
            {
                Serializer.SerializeObj(this._minorTickPosition, "MinorTickPosition", writer);
            }
            if (this._minorTickStroke != null)
            {
                Serializer.SerializeObj(this._minorTickStroke, "MinorTickStroke", writer);
            }
            if (this._minorTickThickness != 0.0)
            {
                Serializer.SerializeObj((double) this._minorTickThickness, "MinorTickThickness", writer);
            }
            if (this._minorUnit != double.NaN)
            {
                Serializer.SerializeObj((double) this._minorUnit, "MinorUnit", writer);
            }
            if (this._minScale != double.NaN)
            {
                Serializer.SerializeObj((double)this._minScale, "MinScale", writer);
            }
            if (this._scale != 1.0)
            {
                Serializer.SerializeObj((double) this._scale, "Scale", writer);
            }
            if (this._crossAt != double.NaN)
            {
                Serializer.SerializeObj((double) this._crossAt, "CrossAt", writer);
            }
            if (this._corsses != Dt.Cells.Data.AxisCrosses.AutoZero)
            {
                Serializer.SerializeObj(this._corsses, "Crosses", writer);
            }
            if (this._reversed)
            {
                Serializer.SerializeObj((bool) this._reversed, "Reversed", writer);
            }
            if (this._visible)
            {
                Serializer.SerializeObj((bool) this._visible, "IsVisible", writer);
            }
            if (this._baseTimeUnit != Dt.Cells.Data.AxisTimeUnit.Days)
            {
                Serializer.SerializeObj(this._baseTimeUnit, "BaseTimeUnit", writer);
            }
            if (this._majorTimeUnit != Dt.Cells.Data.AxisTimeUnit.Days)
            {
                Serializer.SerializeObj(this._majorTimeUnit, "MajorTimeUnit", writer);
            }
            if (this._minorTimeUnit != Dt.Cells.Data.AxisTimeUnit.Days)
            {
                Serializer.SerializeObj(this._minorTimeUnit, "MinorTimeUnit", writer);
            }
            if (this._axisPosition != Dt.Cells.Data.AxisPosition.Near)
            {
                Serializer.SerializeObj(this._axisPosition, "AxisPosition", writer);
            }
            if (this._title != null)
            {
                Serializer.SerializeObj(this._title, "Title", writer);
            }
            if (this._tickMarkInterval > 0)
            {
                Serializer.SerializeObj((int) this._tickMarkInterval, "TickMarkInterval", writer);
            }
            if (this._tickLabelInterval > 0)
            {
                Serializer.SerializeObj((int) this._tickLabelInterval, "TickLabelInterval", writer);
            }
            if (this._overlapData)
            {
                Serializer.SerializeObj((bool) this._overlapData, "OverlapData", writer);
            }
            if (this.MajorGridlineDrawingColorSettings != null)
            {
                Serializer.SerializeObj(this.MajorGridlineDrawingColorSettings, "MajorGridlineDrawingColorSettings", writer);
            }
            if (this.MinorGridlineDrawingColorSettings != null)
            {
                Serializer.SerializeObj(this.MinorGridlineDrawingColorSettings, "MinorGridelineDrawingColorSettings", writer);
            }
        }

        /// <summary>
        /// Gets the actual brush of the major grid lines.
        /// </summary>
        /// <value>
        /// The actual brush of the major grid lines.
        /// </value>
        public Brush ActualMajorGridlinesStroke
        {
            get
            {
                if (this.IsMajorGridlinesStrokeAutomatic)
                {
                    return this.MajorGridlinesAutomaticStroke;
                }
                if (this._majorGridStroke != null)
                {
                    return this._majorGridStroke;
                }
                if ((base.ThemeContext != null) && !string.IsNullOrEmpty(this._majorGridStrokeThemeColor))
                {
                    Windows.UI.Color color = base.ThemeContext.GetThemeColor(this._majorGridStrokeThemeColor);
                    color = Dt.Cells.Data.ColorHelper.UpdateColor(color, this.MajorGridlineDrawingColorSettings, false);
                    return new SolidColorBrush(color);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual thickness of major grid lines.     
        /// </summary>
        public double ActualMajorGridlinesStrokeThickness
        {
            get { return  this.MajorGridlinesStrokeThickness; }
        }

        /// <summary>
        /// Gets the actual brush of the minor grid lines.
        /// </summary>
        /// <value>
        /// The actual brush of the minor grid lines.
        /// </value>
        public Brush ActualMinorGridlinesStroke
        {
            get
            {
                if (this.IsMinorGridlinesStrokeAutomatic)
                {
                    return this.MinorGridlinesAutomaticStroke;
                }
                if (this._minorGridStroke != null)
                {
                    return this._minorGridStroke;
                }
                if ((base.ThemeContext != null) && !string.IsNullOrEmpty(this._minorGridStrokeThemeColor))
                {
                    Windows.UI.Color color = base.ThemeContext.GetThemeColor(this._minorGridStrokeThemeColor);
                    color = Dt.Cells.Data.ColorHelper.UpdateColor(color, this.MajorGridlineDrawingColorSettings, false);
                    return new SolidColorBrush(color);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual thickness of minor grid lines.   
        /// </summary>
        public double ActualMinorGridlinesStrokeThickness
        {
            get { return  this.MinorGridlinesStrokeThickness; }
        }

        /// <summary>
        /// Gets the dash pattern of the minor grid lines. 
        /// </summary>
        public StrokeDashType ActualMinorGridlineStrokeDashType
        {
            get { return  this.MinorGridlinesStrokeDashType; }
        }

        /// <summary>
        /// Gets the actual dash pattern of major grid lines.
        /// </summary>
        public StrokeDashType AcutalMajorGridlineStrokeDashType
        {
            get { return  this.MajorGridlinesStrokeDashType; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the base time unit is automatically specified.
        /// </summary>
        /// <value>
        /// <c>true</c> if the base time unit is automatically specified; otherwise, <c>false</c>.
        /// </value>
        public bool AutoBaseTimeUnit
        {
            get { return  this._autoBaseTimeUnit; }
            set
            {
                if (value != this.AutoBaseTimeUnit)
                {
                    this._autoBaseTimeUnit = value;
                    if (!this.UseCustomItems && !string.IsNullOrEmpty(this.ItemsFormula))
                    {
                        this.Items.UpdateCollection();
                    }
                    this.NotifyAxisChanged("AutoBaseTimeUnit");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the interval for major tick marks and major grid lines is automatically generated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the interval for major tick marks and major grid lines is automatically generated; otherwise, <c>false</c>.
        /// </value>
        public bool AutoMajorUnit
        {
            get { return  this._autoMajorUint; }
            set
            {
                if (value != this.AutoMajorUnit)
                {
                    this._autoMajorUint = value;
                    if (!this.UseCustomItems && !string.IsNullOrEmpty(this.ItemsFormula))
                    {
                        this.Items.UpdateCollection();
                    }
                    this.NotifyAxisChanged("AutoMajorUnit");
                }
            }
        }

        internal bool AutomaticAxisData { get; set; }

        /// <summary>
        /// Gets the automatic fill brush.
        /// </summary>
        public override Brush AutomaticFill
        {
            get { return new SolidColorBrush(Colors.Transparent); }
        }

        /// <summary>
        /// Gets the automatic stroke brush.
        /// </summary>
        public override Brush AutomaticStroke
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>
        /// Gets or sets whether the maximum value for the axis is set automatically.
        /// </summary>
        public bool AutoMax
        {
            get { return  this._autoMax; }
            set
            {
                if (value != this.AutoMax)
                {
                    this._autoMax = value;
                    if (!this.UseCustomItems && !string.IsNullOrEmpty(this.ItemsFormula))
                    {
                        this.Items.UpdateCollection();
                    }
                    this.NotifyAxisChanged("AutoMax");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the minimum value for the axis is set automatically.    
        /// </summary>
        public bool AutoMin
        {
            get { return  this._autoMin; }
            set
            {
                if (value != this.AutoMin)
                {
                    this._autoMin = value;
                    if (!this.UseCustomItems && !string.IsNullOrEmpty(this.ItemsFormula))
                    {
                        this.Items.UpdateCollection();
                    }
                    this.NotifyAxisChanged("AutoMin");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the interval for minor tick marks and minor grid lines is automatically generated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the interval for minor tick marks and minor grid lines is automatically generated; otherwise, <c>false</c>.
        /// </value>
        public bool AutoMinorUnit
        {
            get { return  this._autoMinorUnit; }
            set
            {
                if (value != this.AutoMinorUnit)
                {
                    this._autoMinorUnit = value;
                    if (!this.UseCustomItems && !string.IsNullOrEmpty(this.ItemsFormula))
                    {
                        this.Items.UpdateCollection();
                    }
                    this.NotifyAxisChanged("AutoMinorUnit");
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal axis crosses.
        /// </summary>
        /// <value>
        /// The horizontal axis crosses.
        /// </value>
        internal Dt.Cells.Data.AxisCrosses AxisCrosses
        {
            get { return  this._corsses; }
            set
            {
                if (value != this._corsses)
                {
                    this._corsses = value;
                    this.NotifyAxisChanged("AxisCrosses");
                }
            }
        }

        /// <summary>
        /// Gets or sets the axis position.
        /// </summary>
        /// <value>
        /// The axis position.
        /// </value>
        public Dt.Cells.Data.AxisPosition AxisPosition
        {
            get { return  this._axisPosition; }
            set
            {
                if (value != this.AxisPosition)
                {
                    this._axisPosition = value;
                    this.NotifyAxisChanged("AxisPosition");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the axis.
        /// </summary>
        /// <value>
        /// The type of the axis.
        /// </value>
        public Dt.Cells.Data.AxisType AxisType
        {
            get
            {
                if (!this._axisType.HasValue)
                {
                    return this._autoAxisType;
                }
                return this._axisType.Value;
            }
            set
            {
                if (value != this.AxisType)
                {
                    this._axisType = new Dt.Cells.Data.AxisType?(value);
                    this.UpdateMinMax();
                    this.UpdateItems();
                    this.NotifyAxisChanged("AxisType");
                }
            }
        }

        /// <summary>
        /// Gets or sets the base time unit.
        /// </summary>
        /// <value>
        /// The base time unit.
        /// </value>
        public Dt.Cells.Data.AxisTimeUnit BaseTimeUnit
        {
            get { return  this._baseTimeUnit; }
            set
            {
                this._autoBaseTimeUnit = false;
                if (value != this.BaseTimeUnit)
                {
                    this._baseTimeUnit = value;
                    if (this.AutoMinorUnit || this.AutoMajorUnit)
                    {
                        if (this.AutoMinorUnit)
                        {
                            this._minorTimeUnit = value;
                        }
                        if (this.AutoMajorUnit)
                        {
                            this._majorTimeUnit = value;
                        }
                        if (!this.UseCustomItems && !string.IsNullOrEmpty(this.ItemsFormula))
                        {
                            this.Items.UpdateCollection();
                        }
                        this.NotifyAxisChanged("BaseTimeUnit");
                    }
                }
            }
        }

        internal SpreadChart Chart
        {
            get { return  (base.ChartBase as SpreadChart); }
            set { base.ChartBase = value; }
        }

        internal override Dt.Cells.Data.ChartArea ChartArea
        {
            get
            {
                switch (this._orientation)
                {
                    case Dt.Cells.Data.AxisOrientation.X:
                        return Dt.Cells.Data.ChartArea.AxisX;

                    case Dt.Cells.Data.AxisOrientation.Y:
                        return Dt.Cells.Data.ChartArea.AxisY;

                    case Dt.Cells.Data.AxisOrientation.Z:
                        return Dt.Cells.Data.ChartArea.AxisZ;
                }
                return Dt.Cells.Data.ChartArea.AxisX;
            }
        }

        /// <summary>
        /// Gets or sets the value at which the axis crosses the perpendicular axis.   
        /// </summary>
        public double CrossAt
        {
            get { return  this._crossAt; }
            set
            {
                if (value != this.CrossAt)
                {
                    this._crossAt = value;
                    this.NotifyAxisChanged("CrossAt");
                }
            }
        }

        /// <summary>
        /// specifies the possible crossing states of an axis.
        /// </summary>
        internal AxisCrossBetween CrossBetween
        {
            get { return  this._crossBetween; }
            set
            {
                if (value != this._crossBetween)
                {
                    this._crossBetween = value;
                    this.NotifyAxisChanged("CrossBetween");
                }
            }
        }

        internal bool Delete { get; set; }

        internal bool DisplayHidden
        {
            get { return  this._displayHidden; }
            set
            {
                if (value != this.DisplayHidden)
                {
                    this._displayHidden = value;
                    if (this.Items.DataSeries != null)
                    {
                        this.Items.RefreshData();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the display unit.
        /// </summary>
        /// <value>
        /// The display unit.
        /// </value>
        public double DisplayUnit
        {
            get
            {
                if (this._displayUnitSettings == null)
                {
                    return double.NaN;
                }
                return this._displayUnitSettings.DisplayUnit;
            }
            set
            {
                if (this._displayUnitSettings == null)
                {
                    this._displayUnitSettings = new Dt.Cells.Data.DisplayUnitSettings();
                }
                this._displayUnitSettings.DisplayUnit = value;
                this.NotifyAxisChanged("DisplayUnit");
            }
        }

        /// <summary>
        /// Gets or sets the display unit settings.
        /// </summary>
        /// <value>
        /// The display unit settings.
        /// </value>
        internal Dt.Cells.Data.DisplayUnitSettings DisplayUnitSettings
        {
            get { return  this._displayUnitSettings; }
            set
            {
                if (value != this._displayUnitSettings)
                {
                    this._displayUnitSettings = value;
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
                    if (this.Items.DataSeries != null)
                    {
                        this.Items.RefreshData();
                    }
                }
            }
        }

        internal int Id { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the major grid lines are displayed automatically.
        /// </summary>
        /// <value>
        /// <c>true</c> if the major grid lines are displayed automatically; otherwise, <c>false</c>.
        /// </value>
        public bool IsMajorGridlinesStrokeAutomatic
        {
            get { return  this._isMajorGridStrokeAutomatic; }
            set
            {
                if (value != this.IsMajorGridlinesStrokeAutomatic)
                {
                    this._isMajorGridStrokeAutomatic = value;
                    this.NotifyAxisChanged("IsMajorGridlinesStrokeAutomatic");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether minor grid lines are displayed automatically.
        /// </summary>
        /// <value>
        /// <c>true</c> if minor grid lines are displayed automatically; otherwise, <c>false</c>.
        /// </value>
        public bool IsMinorGridlinesStrokeAutomatic
        {
            get { return  this._isMinorGridStrokeAutomatic; }
            set
            {
                if (value != this.IsMinorGridlinesStrokeAutomatic)
                {
                    this._isMinorGridStrokeAutomatic = value;
                    this.NotifyAxisChanged("IsMinorGridlinesStrokeAutomatic");
                }
            }
        }

        internal bool IsSerAxis { get; set; }

        /// <summary>
        /// Gets the collection of the items.
        /// </summary>
        /// <value>
        /// the collection of the items.
        /// </value>
        public AxisItemsCollection Items
        {
            get { return  this._items; }
        }

        /// <summary>
        /// Gets or sets the category formula.
        /// </summary>
        /// <value>
        /// The category formula.
        /// </value>
        public string ItemsFormula
        {
            get { return  this._itemsFormula; }
            set
            {
                if (value != this.ItemsFormula)
                {
                    this.SetItemsFormulaInternal(value);
                    this.NotifyAxisChanged("Items");
                }
            }
        }

        DataOrientation? ItemsOrientation
        {
            get { return  this._itemsOrientation; }
        }

        internal SheetCellRange[] ItemsRange
        {
            get { return  this._itemsRange; }
            set
            {
                if (value != this._itemsRange)
                {
                    this._itemsRange = value;
                    if ((this._itemsRange != null) && (this._itemsRange.Length > 0))
                    {
                        int num = 0;
                        int num2 = 0;
                        foreach (SheetCellRange range in this._itemsRange)
                        {
                            num += range.RowCount;
                            num2 += range.ColumnCount;
                        }
                        this._itemsOrientation = new DataOrientation?((num >= num2) ? DataOrientation.Vertical : DataOrientation.Horizontal);
                    }
                    else
                    {
                        this._itemsOrientation = 0;
                    }
                }
            }
        }

        CalcExpression ItemsReference
        {
            get { return  this._itemsReference; }
            set
            {
                if (value != this.ItemsReference)
                {
                    this._itemsReference = value;
                    if (this._itemsReference != null)
                    {
                        this.ItemsRange = SheetCellRangeUtility.ExtractAllExternalReference(this.Sheet, this._itemsReference);
                    }
                    else
                    {
                        this.ItemsRange = null;
                    }
                    if (value != null)
                    {
                        if (this.Items.DataSeries != null)
                        {
                            this.Items.RefreshData();
                        }
                        else
                        {
                            this.Items.DataSeries = new ItemsDataSeries(this);
                        }
                    }
                    else
                    {
                        this.Items.DataSeries = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle of axis annotations.
        /// </summary>
        public double LabelAngle
        {
            get { return  this._labelAngle; }
            set
            {
                if (value != this.LabelAngle)
                {
                    this._labelAngle = value;
                    this.NotifyAxisChanged("LabelAngle");
                }
            }
        }

        /// <summary>
        /// Gets or sets the label formatter.
        /// </summary>
        public IFormatter LabelFormatter
        {
            get { return  this._labelFormatter; }
            set
            {
                if (value != this.LabelFormatter)
                {
                    this._labelFormatter = value;
                    this.NotifyAxisChanged("LabelFormatter");
                }
            }
        }

        /// <summary>
        /// Gets or sets the annotation position.
        /// </summary>
        /// <remarks>
        /// The position is relative to the value on the axis which corresponds to the axis annotation.
        /// </remarks>:
        internal AxisLabelPosition LabelPosition
        {
            get { return  this._labelPosition; }
            set
            {
                if (value != this.LabelPosition)
                {
                    this._labelPosition = value;
                    this.NotifyAxisChanged("LabelPosition");
                }
            }
        }

        internal int LableOffset
        {
            get { return  this._lableOffset; }
            set
            {
                if ((value < 0) || (value > 0x3e8))
                {
                    throw new ArgumentOutOfRangeException("LabelOffset");
                }
                if (value != this._lableOffset)
                {
                    this._lableOffset = value;
                    this.NotifyAxisChanged("LableOffset");
                }
            }
        }

        /// <summary>
        /// Gets or sets the line begin arrow settings.
        /// </summary>
        /// <value>
        /// The line begin arrow settings.
        /// </value>
        internal ArrowSettings LineBeginArrowSettings
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.LineBeginArrowSettings;
                }
                return null;
            }
            set { base.StyleInfo.LineBeginArrowSettings = value; }
        }

        /// <summary>
        /// Gets or sets the type of the line cap.
        /// </summary>
        /// <value>
        /// The type of the line cap.
        /// </value>
        public PenLineCap LineCapType
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.LineCapType;
                }
                return PenLineCap.Flat;
            }
            set { base.StyleInfo.LineCapType = value; }
        }

        /// <summary>
        /// Gets or sets the line end arrow settings.
        /// </summary>
        /// <value>
        /// The line end arrow settings.
        /// </value>
        internal ArrowSettings LineEndArrowSettings
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.LineEndArrowSettings;
                }
                return null;
            }
            set { base.StyleInfo.LineEndArrowSettings = value; }
        }

        /// <summary>
        /// Gets or sets the type of the line join.
        /// </summary>
        /// <value>
        /// The type of the line join.
        /// </value>
        public PenLineJoin LineJoinType
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.LineJoinType;
                }
                return PenLineJoin.Round;
            }
            set { base.StyleInfo.LineJoinType = value; }
        }

        /// <summary>
        /// Gets or sets the logarithmic base for the axis. Default value double.NaN
        /// corresponds to the default linear axis.    
        /// </summary>
        public double LogBase
        {
            get { return  this._logBase; }
            set
            {
                if ((value < 2.0) || (value > 1000.0))
                {
                    throw new ArgumentOutOfRangeException("LogBase");
                }
                if (value != this.LogBase)
                {
                    this._logBase = value;
                    this.NotifyAxisChanged("LogBase");
                }
            }
        }

        /// <summary>
        /// Gets or sets the major grid line begin arrow settings.
        /// </summary>
        /// <value>
        /// The major grid line begin arrow settings.
        /// </value>
        internal ArrowSettings MajorGridLineBeginArrowSettings
        {
            get { return  this._majorGridLineBeginArrowSettings; }
            set
            {
                if (value != this.MajorGridLineBeginArrowSettings)
                {
                    this._majorGridLineBeginArrowSettings = value;
                    this.NotifyAxisChanged("MajorGridLineBeginArrowSettings");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the major grid line cap.
        /// </summary>
        /// <value>
        /// The type of the major grid line cap.
        /// </value>
        internal PenLineCap MajorGridLineCapType
        {
            get { return  this._majorGridLineCapType; }
            set
            {
                if (value != this.MajorGridLineCapType)
                {
                    this._majorGridLineCapType = value;
                    this.NotifyAxisChanged("MajorGridLineCapType");
                }
            }
        }

        internal SpreadDrawingColorSettings MajorGridlineDrawingColorSettings { get; set; }

        /// <summary>
        /// Gets or sets the major grid line end arrow settings.
        /// </summary>
        /// <value>
        /// The major grid line end arrow settings.
        /// </value>
        internal ArrowSettings MajorGridLineEndArrowSettings
        {
            get { return  this._majorGridLineEndArrowSettings; }
            set
            {
                if (value != this.MajorGridLineEndArrowSettings)
                {
                    this._majorGridLineEndArrowSettings = value;
                    this.NotifyAxisChanged("MajorGridLineEndArrowSettings");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the major grid line join.
        /// </summary>
        /// <value>
        /// The type of the major grid line join.
        /// </value>
        internal PenLineJoin MajorGridLineJoinType
        {
            get { return  this._majorGridLineJoinType; }
            set
            {
                if (value != this.MajorGridLineJoinType)
                {
                    this._majorGridLineJoinType = value;
                    this.NotifyAxisChanged("MajorGridLineJoinType");
                }
            }
        }

        /// <summary>
        /// Gets the automatic stoke brush of the major grid lines.
        /// </summary>
        /// <value>
        /// The automatic stoke brush of the major grid lines.
        /// </value>
        public Brush MajorGridlinesAutomaticStroke
        {
            get { return  SpreadChartElement.DefaultAutomaticStroke; }
        }

        /// <summary>
        /// Gets or sets the brush of major grid lines.    
        /// </summary>
        public Brush MajorGridlinesStroke
        {
            get { return  this._majorGridStroke; }
            set
            {
                this._majorGridStrokeThemeColor = null;
                if (value != this.MajorGridlinesStroke)
                {
                    this._isMajorGridStrokeAutomatic = false;
                    this._majorGridStroke = value;
                    this.NotifyAxisChanged("MajorGridlinesStroke");
                }
            }
        }

        /// <summary>
        /// Gets or sets the dash pattern of major grid lines.    
        /// </summary>
        public StrokeDashType MajorGridlinesStrokeDashType
        {
            get { return  this._majorGridStrokeDashType; }
            set
            {
                if (value != this.MajorGridlinesStrokeDashType)
                {
                    this._majorGridStrokeDashType = value;
                    this.NotifyAxisChanged("MajorGridlinesStrokeDashes");
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the major grid stroke theme.
        /// </summary>
        /// <value>
        /// The color of the major grid stroke theme.
        /// </value>
        public string MajorGridlinesStrokeThemeColor
        {
            get { return  this._majorGridStrokeThemeColor; }
            set
            {
                this._majorGridStroke = null;
                if (value != this.MajorGridlinesStrokeThemeColor)
                {
                    this._majorGridStrokeThemeColor = value;
                    this.NotifyAxisChanged("MajorGridlinesStrokeThemeColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the thickness of major grid lines.     
        /// </summary>
        public double MajorGridlinesStrokeThickness
        {
            get { return  this._majorGridStrokeThickness; }
            set
            {
                if (value != this.MajorGridlinesStrokeThickness)
                {
                    this._majorGridStrokeThickness = value;
                    this.NotifyAxisChanged("MajorGridlinesStrokeThickness");
                }
            }
        }

        /// <summary>
        /// Gets or sets the major tick height.    
        /// </summary>
        public double MajorTickHeight
        {
            get { return  this._majorTickHeight; }
            set
            {
                if (value != this.MajorTickHeight)
                {
                    this._majorTickHeight = value;
                    this.NotifyAxisChanged("MajorTickHeight");
                }
            }
        }

        /// <summary>
        /// Gets or sets the major tick position.
        /// </summary>
        /// <value>
        /// The major tick position.
        /// </value>
        public AxisTickPosition MajorTickPosition
        {
            get { return  this._majorTickPosition; }
            set
            {
                if (value != this.MajorTickPosition)
                {
                    this._majorTickPosition = value;
                    this.NotifyAxisChanged("MajorTickPosition");
                }
            }
        }

        /// <summary>
        /// Gets or sets the major tick stroke.   
        /// </summary>
        public Brush MajorTickStroke
        {
            get { return  this._majorTickStroke; }
            set
            {
                if (value != this.MajorTickStroke)
                {
                    this._majorTickStroke = value;
                    this.NotifyAxisChanged("MajorTickStroke");
                }
            }
        }

        /// <summary>
        /// Gets or sets the major tick thickness.   
        /// </summary>
        public double MajorTickThickness
        {
            get { return  this._majorTickThickness; }
            set
            {
                if (value != this.MajorTickThickness)
                {
                    this._majorTickThickness = value;
                    this.NotifyAxisChanged("MajorTickThickness");
                }
            }
        }

        /// <summary>
        /// Gets or sets the major time unit of the axis.
        /// </summary>
        /// <value>
        /// The major time unit.
        /// </value>
        public Dt.Cells.Data.AxisTimeUnit MajorTimeUnit
        {
            get
            {
                if (this._autoMajorUint)
                {
                    return this._baseTimeUnit;
                }
                return this._majorTimeUnit;
            }
            set
            {
                this._autoMajorUint = false;
                if (value != this.MajorTimeUnit)
                {
                    this._majorTimeUnit = value;
                    this.UpdateItems();
                    this.NotifyAxisChanged("MajorTimeUnit");
                }
            }
        }

        /// <summary>
        /// Gets or sets the major unit (distance between labels).   
        /// </summary>
        /// <remarks>
        /// For time axis the distance is measured in days. E.g. 6 hours = 0.25 (days).
        /// </remarks>:
        public double MajorUnit
        {
            get { return  this._majorUnit; }
            set
            {
                if (value <= 0.0)
                {
                    throw new InvalidOperationException();
                }
                this._autoMajorUint = false;
                if (value != this.MajorUnit)
                {
                    this._majorUnit = value;
                    this.UpdateItems();
                    this.NotifyAxisChanged("MajorUnit");
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum value shown on the axis.    
        /// </summary>
        public double Max
        {
            get
            {
                if (double.IsNaN(this._max))
                {
                    return 1.0;
                }
                return this._max;
            }
            set
            {
                this._autoMax = false;
                if (value != this.Max)
                {
                    this._max = value;
                    if (this.AutoMajorUnit || this.AutoMinorUnit)
                    {
                        double num;
                        double num2;
                        this.AdjustMinMax(out num, out num2);
                        this.UpdateMajorMinorUnit(num, num2);
                    }
                    if (this.Items != null)
                    {
                        this.Items.UpdateItemsForAxis();
                    }
                    this.NotifyAxisChanged("Max");
                }
            }
        }

        internal int MaxDataPointCount
        {
            get { return  this._maxDataPointCount; }
            set
            {
                if (value != this.MaxDataPointCount)
                {
                    this._maxDataPointCount = value;
                    if (!string.IsNullOrEmpty(this.ItemsFormula))
                    {
                        this.UpdateMinMax();
                        this.NotifyAxisChanged("Items");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum value shown on the axis.    
        /// </summary>
        public double Min
        {
            get
            {
                if (double.IsNaN(this._min))
                {
                    return 0.0;
                }
                return this._min;
            }
            set
            {
                this._autoMin = false;
                if (value != this.Min)
                {
                    this._min = value;
                    if (this.AutoMinorUnit || this.AutoMajorUnit)
                    {
                        double num;
                        double num2;
                        this.AdjustMinMax(out num, out num2);
                        this.UpdateMajorMinorUnit(num, num2);
                    }
                    this.UpdateItems();
                    this.NotifyAxisChanged("Min");
                }
            }
        }

        /// <summary>
        /// Gets or sets the minor grid line begin arrow settings.
        /// </summary>
        /// <value>
        /// The minor grid line begin arrow settings.
        /// </value>
        internal ArrowSettings MinorGridLineBeginArrowSettings
        {
            get { return  this._minorGridLineBeginArrowSettings; }
            set
            {
                if (value != this.MinorGridLineBeginArrowSettings)
                {
                    this._minorGridLineBeginArrowSettings = value;
                    this.NotifyAxisChanged("MinorGridLineBeginArrowSettings");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the minor grid line cap.
        /// </summary>
        /// <value>
        /// The type of the minor grid line cap.
        /// </value>
        internal PenLineCap MinorGridLineCapType
        {
            get { return  this._minorGridLineCapType; }
            set
            {
                if (value != this.MinorGridLineCapType)
                {
                    this._minorGridLineCapType = value;
                    this.NotifyAxisChanged("MinorGridLineCapType");
                }
            }
        }

        internal SpreadDrawingColorSettings MinorGridlineDrawingColorSettings { get; set; }

        /// <summary>
        /// Gets or sets the minor grid line end arrow settings.
        /// </summary>
        /// <value>
        /// The minor grid line end arrow settings.
        /// </value>
        internal ArrowSettings MinorGridLineEndArrowSettings
        {
            get { return  this._minorGridLineEndArrowSettings; }
            set
            {
                if (value != this.MinorGridLineEndArrowSettings)
                {
                    this._minorGridLineEndArrowSettings = value;
                    this.NotifyAxisChanged("MinorGridLineEndArrowSettings");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the minor grid line join.
        /// </summary>
        /// <value>
        /// The type of the minor grid line join.
        /// </value>
        internal PenLineJoin MinorGridLineJoinType
        {
            get { return  this._minorGridLineJoinType; }
            set
            {
                if (value != this.MinorGridLineJoinType)
                {
                    this._minorGridLineJoinType = value;
                    this.NotifyAxisChanged("MinorGridLineJoinType");
                }
            }
        }

        /// <summary>
        /// Gets the automatic brush of the minor grid lines.
        /// </summary>
        /// <value>
        /// The automatic brush of the minor grid lines.
        /// </value>
        public Brush MinorGridlinesAutomaticStroke
        {
            get { return  SpreadChartElement.DefaultAutomaticStroke; }
        }

        /// <summary>
        /// Gets or sets the brush of minor grid lines.    
        /// </summary>
        public Brush MinorGridlinesStroke
        {
            get { return  this._minorGridStroke; }
            set
            {
                this._minorGridStrokeThemeColor = null;
                if (value != this.MinorGridlinesStroke)
                {
                    this._isMinorGridStrokeAutomatic = false;
                    this._minorGridStroke = value;
                    this.NotifyAxisChanged("MinorGridlinesStroke");
                }
            }
        }

        /// <summary>
        /// Gets or sets the dash pattern of minor grid lines.     
        /// </summary>
        public StrokeDashType MinorGridlinesStrokeDashType
        {
            get { return  this._minorGridStrokeDashType; }
            set
            {
                if (value != this.MinorGridlinesStrokeDashType)
                {
                    this._minorGridStrokeDashType = value;
                    this.NotifyAxisChanged("MinorGridlinesStrokeDashes");
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the minor grid stroke theme.
        /// </summary>
        /// <value>
        /// The color of the minor grid stroke theme.
        /// </value>
        public string MinorGridlinesStrokeThemeColor
        {
            get { return  this._minorGridStrokeThemeColor; }
            set
            {
                this._minorGridStroke = null;
                if (value != this.MinorGridlinesStrokeThemeColor)
                {
                    this._minorGridStrokeThemeColor = value;
                    this.NotifyAxisChanged("MinorGridlinesStrokeThemeColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the thickness of minor grid lines.   
        /// </summary>
        public double MinorGridlinesStrokeThickness
        {
            get { return  this._minorGridStrokeThickness; }
            set
            {
                if (value != this.MinorGridlinesStrokeThickness)
                {
                    this._minorGridStrokeThickness = value;
                    this.NotifyAxisChanged("MinorGridlinesStrokeThickness");
                }
            }
        }

        /// <summary>
        /// Gets or sets the minor tick height.     
        /// </summary>
        public double MinorTickHeight
        {
            get { return  this._minorTickHeight; }
            set
            {
                if (value != this.MinorTickHeight)
                {
                    this._minorTickHeight = value;
                    this.NotifyAxisChanged("MinorTickHeight");
                }
            }
        }

        /// <summary>
        /// Gets or sets the minor tick position.
        /// </summary>
        /// <value>
        /// The minor tick position.
        /// </value>
        public AxisTickPosition MinorTickPosition
        {
            get { return  this._minorTickPosition; }
            set
            {
                if (value != this.MinorTickPosition)
                {
                    this._minorTickPosition = value;
                    this.NotifyAxisChanged("MinorTickPosition");
                }
            }
        }

        /// <summary>
        /// Gets or sets the minor tick stroke.   
        /// </summary>
        public Brush MinorTickStroke
        {
            get { return  this._minorTickStroke; }
            set
            {
                if (value != this.MinorTickStroke)
                {
                    this._minorTickStroke = value;
                    this.NotifyAxisChanged("MinorTickStroke");
                }
            }
        }

        /// <summary>
        /// Gets or sets the minor tick thickness.    
        /// </summary>
        public double MinorTickThickness
        {
            get { return  this._minorTickThickness; }
            set
            {
                if (value != this.MinorTickThickness)
                {
                    this._minorTickThickness = value;
                    this.NotifyAxisChanged("MinorTickThickness");
                }
            }
        }

        /// <summary>
        /// Gets or sets the minor time unit.
        /// </summary>
        /// <value>
        /// The minor time unit.
        /// </value>
        public Dt.Cells.Data.AxisTimeUnit MinorTimeUnit
        {
            get
            {
                if (this._autoMinorUnit)
                {
                    return this._baseTimeUnit;
                }
                return this._minorTimeUnit;
            }
            set
            {
                this._autoMinorUnit = false;
                if (value != this.MinorTimeUnit)
                {
                    this._minorTimeUnit = value;
                    this.NotifyAxisChanged("MinorTimeUnit");
                }
            }
        }

        /// <summary>
        /// Gets or sets the minor unit (distance between minor tick marks).    
        /// </summary>
        /// <remarks>
        /// For time axis the distance is measured in days. E.g. 6 hours = 0.25 (days).
        /// </remarks>:
        public double MinorUnit
        {
            get { return  this._minorUnit; }
            set
            {
                this._autoMinorUnit = false;
                if (value != this.MinorUnit)
                {
                    if (this.AxisType == Dt.Cells.Data.AxisType.Date)
                    {
                        this._minorUnit = (int) value;
                    }
                    else
                    {
                        this._minorUnit = value;
                    }
                    this.NotifyAxisChanged("MinorUnit");
                }
            }
        }

        /// <summary>
        /// Gets or set the minimum scale of the axis.    
        /// </summary>
        [DefaultValue((double) 1E-05)]
        internal double MinScale
        {
            get { return  this._minScale; }
            set
            {
                if (value != this.MinScale)
                {
                    this._minScale = value;
                    this.NotifyAxisChanged("MinScale");
                }
            }
        }

        internal bool NumberFormatSourceLinked { get; set; }

        internal Dt.Cells.Data.AxisOrientation Orientation
        {
            get { return  this._orientation; }
            set
            {
                this._orientation = value;
                if (this._orientation == Dt.Cells.Data.AxisOrientation.Y)
                {
                    this._hasMajorGridLine = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the data overlaps.
        /// </summary>
        /// <value>
        /// <c>true</c> if the data overlaps; otherwise, <c>false</c>.
        /// </value>
        public bool OverlapData
        {
            get { return  this._overlapData; }
            set
            {
                if (value != this.OverlapData)
                {
                    this._overlapData = value;
                    this.NotifyAxisChanged("OverlapData");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the axis is reversed.    
        /// </summary>
        public bool Reversed
        {
            get { return  this._reversed; }
            set
            {
                if (value != this.Reversed)
                {
                    this._reversed = value;
                    this.NotifyAxisChanged("Reversed");
                }
            }
        }

        /// <summary>
        /// Gets or set the scale of the axis. 
        /// </summary>
        /// <remarks>
        /// The scale should be greater than 0 and less than or equal to 1.  The scale specifies
        /// which part of the range between the minimum and maximum is shown. When scale is 1 (default
        /// value) the entire axis range is visible.
        /// </remarks>:
        internal double Scale
        {
            get { return  this._scale; }
            set
            {
                if (value != this.Scale)
                {
                    this._scale = value;
                    this.NotifyAxisChanged("Scale");
                }
            }
        }

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

        /// <summary>
        /// Gets or sets a value that indicates whether to show the axis label.
        /// </summary>
        /// <value>
        /// <c>true</c> if showing the axis label; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAxisLabel
        {
            get { return  this._showAxisLabel; }
            set
            {
                if (value != this.ShowAxisLabel)
                {
                    this._showAxisLabel = value;
                    this.NotifyAxisChanged("ShowAxisLabel");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to display major gridlines.
        /// </summary>
        public bool ShowMajorGridlines
        {
            get { return  this._hasMajorGridLine; }
            set
            {
                if (value != this._hasMajorGridLine)
                {
                    this._hasMajorGridLine = value;
                    this.NotifyAxisChanged("HasMajorGridlines");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show minor grid lines.
        /// </summary>
        public bool ShowMinorGridlines
        {
            get { return  this._hasMinorGridLine; }
            set
            {
                if (value != this._hasMinorGridLine)
                {
                    this._hasMinorGridLine = value;
                    this.NotifyAxisChanged("HasMinorGridlines");
                }
            }
        }

        /// <summary>
        /// Gets or sets the text of the axis.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
        public override string Text
        {
            get { return  base.Text; }
            set { base.Text = value; }
        }

        internal IExcelTextFormat TextFormat { get; set; }

        internal int TickLabelInterval
        {
            get { return  this._tickLabelInterval; }
            set
            {
                if (this._tickLabelInterval != value)
                {
                    this._tickLabelInterval = value;
                    this.NotifyAxisChanged("TickLabelInterval");
                }
            }
        }

        internal int TickMarkInterval
        {
            get { return  this._tickMarkInterval; }
            set
            {
                if (this._tickMarkInterval != value)
                {
                    this._tickMarkInterval = value;
                    this.NotifyAxisChanged("TickMarkInterval");
                }
            }
        }

        /// <summary>
        /// Gets or sets the axis title.     
        /// </summary>
        public ChartTitle Title
        {
            get { return  this._title; }
            set
            {
                if (value != this.Title)
                {
                    if (value != null)
                    {
                        value.TitleType = TitleType.AxisTitle;
                        if (((value._styleInfo != null) && !value._styleInfo.IsFontSizeSet) || (value._styleInfo == null))
                        {
                            value.FontSize = 15.0;
                        }
                    }
                    if ((value != null) && !object.ReferenceEquals(value.Chart, this.Chart))
                    {
                        value.Chart = this.Chart;
                    }
                    this._title = value;
                    this.NotifyAxisChanged("Title");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to use custom items.
        /// </summary>
        /// <value>
        /// <c>true</c> to use custom items; otherwise, <c>false</c>.
        /// </value>
        public bool UseCustomItems
        {
            get { return  this._useCustomItems; }
            set
            {
                if (value != this.UseCustomItems)
                {
                    this._useCustomItems = value;
                    if (this._useCustomItems)
                    {
                        this.Items.Clear();
                    }
                    else
                    {
                        this.Items.UpdateItemsForAxis();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to use a log base.
        /// </summary>
        /// <value>
        /// <c>true</c> if using a log base; otherwise, <c>false</c>.
        /// </value>
        public bool UseLogBase
        {
            get { return  this._useLogBase; }
            set
            {
                if (value != this.UseLogBase)
                {
                    this._useLogBase = value;
                    this.NotifyAxisChanged("UseLogBase");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the axis is visible.    
        /// </summary>
        public bool Visible
        {
            get { return  this._visible; }
            set
            {
                if (value != this.Visible)
                {
                    this._visible = value;
                    this.NotifyAxisChanged("Visible");
                }
            }
        }

        /// <summary>
        /// Represents data series for axis' items.
        /// </summary>
        public class ItemsDataSeries : IDataSeries
        {
            Axis _axis;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Axis.ItemsDataSeries" /> class.
            /// </summary>
            /// <param name="axis">The axis.</param>
            public ItemsDataSeries(Axis axis)
            {
                this._axis = axis;
            }

            /// <summary>
            /// Gets the data orientation.
            /// </summary>
            /// <value>
            /// The data orientation.
            /// </value>
            public Dt.Cells.Data.DataOrientation? DataOrientation
            {
                get { return  this._axis.ItemsOrientation; }
            }

            /// <summary>
            /// Gets the data reference.
            /// </summary>
            /// <value>
            /// The data reference.
            /// </value>
            public CalcExpression DataReference
            {
                get { return  this._axis.ItemsReference; }
            }

            /// <summary>
            /// Gets a value that indicates whether hidden data is displayed.
            /// </summary>
            /// <value>
            /// <c>true</c> if hidden data is displayed; otherwise, <c>false</c>.
            /// </value>
            public bool DisplayHiddenData
            {
                get { return  this._axis.DisplayHidden; }
            }

            /// <summary>
            /// Gets the empty value style.
            /// </summary>
            /// <value>
            /// The empty value style.
            /// </value>
            public Dt.Cells.Data.EmptyValueStyle EmptyValueStyle
            {
                get { return  Dt.Cells.Data.EmptyValueStyle.Zero; }
            }

            /// <summary>
            /// Gets the object that can evaluate an expression or a function.
            /// </summary>
            public ICalcEvaluator Evaluator
            {
                get { return  this._axis.Sheet; }
            }
        }
    }
}

