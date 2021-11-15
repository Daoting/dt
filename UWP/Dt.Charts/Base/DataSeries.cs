#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Charts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Base
{
    public partial class DataSeries : Canvas, IDataSeriesInfo
    {
        public static readonly DependencyProperty AggregateProperty = DependencyProperty.Register(
            "Aggregate",
            typeof(Aggregate),
            typeof(DataSeries), 
            new PropertyMetadata(Aggregate.None, new PropertyChangedCallback(DataSeries.OnAggregateChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ChartTypeProperty = DependencyProperty.Register(
            "ChartType", 
            typeof(ChartType?),
            typeof(DataSeries), 
            new PropertyMetadata(null, new PropertyChangedCallback(DataSeries.OnSeriesChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ConnectionFillProperty = Utils.RegisterProperty(
            "ConnectionFill", 
            typeof(Brush),
            typeof(DataSeries), 
            new PropertyChangedCallback(DataSeries.ConnectionFillChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ConnectionProperty = Utils.RegisterProperty(
            "Connection",
            typeof(object),
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnSeriesChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ConnectionStrokeDashesProperty = Utils.RegisterProperty(
            "ConnectionStrokeDashes",
            typeof(DoubleCollection), 
            typeof(DataSeries), 
            new PropertyChangedCallback(DataSeries.OnConnectionStrokeDashesChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ConnectionStrokeProperty = Utils.RegisterProperty(
            "ConnectionStroke", 
            typeof(Brush), 
            typeof(DataSeries), 
            new PropertyChangedCallback(DataSeries.ConnectionStrokeChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ConnectionStrokeThicknessProperty = Utils.RegisterProperty(
            "ConnectionStrokeThickness",
            typeof(double),
            typeof(DataSeries), 
            new PropertyChangedCallback(DataSeries.ConnectionStrokeThicknessChanged), (double)0.0);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ConnectionStyleProperty = Utils.RegisterProperty(
            "ConnectionStyle",
            typeof(Style),
            typeof(DataSeries), 
            new PropertyChangedCallback(DataSeries.OnSeriesChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisplayProperty = Utils.RegisterProperty(
            "Display",
            typeof(SeriesDisplay),
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnSeriesChanged), SeriesDisplay.SkipNaN);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = Utils.RegisterProperty(
            "ItemsSource", 
            typeof(IEnumerable), 
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnItemsSourceChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelProperty = Utils.RegisterProperty(
            "Label", 
            typeof(string), 
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnLabelChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PointLabelTemplateProperty = Utils.RegisterProperty(
            "PointLabelTemplate", 
            typeof(DataTemplate),
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnSeriesChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PointTooltipTemplateProperty = Utils.RegisterProperty(
            "PointTooltipTemplate",
            typeof(DataTemplate), 
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnPointTooltipChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedItemLabelTemplateProperty = DependencyProperty.Register(
            "SelectedItemLabelTemplate",
            typeof(DataTemplate), 
            typeof(DataSeries), 
            new PropertyMetadata(null, new PropertyChangedCallback(DataSeries.OnSelectedItemLabelTemplateChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedItemStyleProperty = DependencyProperty.Register(
            "SelectedItemStyle", 
            typeof(Style), 
            typeof(DataSeries), 
            new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SymbolFillProperty = Utils.RegisterProperty(
            "SymbolFill",
            typeof(Brush),
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.SymbolFillChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SymbolMarkerProperty = Utils.RegisterProperty(
            "SymbolMarker",
            typeof(Marker),
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnSeriesChanged), Marker.None);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SymbolProperty = Utils.RegisterProperty(
            "Symbol", 
            typeof(object),
            typeof(DataSeries), 
            new PropertyChangedCallback(DataSeries.OnSeriesChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SymbolSizeProperty = Utils.RegisterProperty(
            "SymbolSize", 
            typeof(Size),
            typeof(DataSeries), 
            new PropertyChangedCallback(DataSeries.OnSeriesChanged), Size.Empty);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SymbolStrokeDashesProperty = Utils.RegisterProperty(
            "SymbolStrokeDashes",
            typeof(DoubleCollection), 
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnSymbolStrokeDashesChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SymbolStrokeProperty = Utils.RegisterProperty(
            "SymbolStroke",
            typeof(Brush), 
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.SymbolStrokeChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SymbolStrokeThicknessProperty = Utils.RegisterProperty(
            "SymbolStrokeThickness", 
            typeof(double),
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.SymbolStrokeThicknessChanged), (double)0.0);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SymbolStyleProperty = Utils.RegisterProperty(
            "SymbolStyle", 
            typeof(Style),
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnSeriesChanged));

        /// <summary>
        /// 
        /// </summary>
        static DependencyProperty ValuesProperty = DependencyProperty.Register(
            "Values", 
            typeof(DoubleCollection),
            typeof(DataSeries), 
            new PropertyMetadata(null, new PropertyChangedCallback(DataSeries.ValuesChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValuesSourceProperty = Utils.RegisterProperty(
            "ValuesSource",
            typeof(IEnumerable), 
            typeof(DataSeries),
            new PropertyChangedCallback(DataSeries.OnChangeValues));

        bool _autoGenerated;
        string _ax = "";
        string _ay = "";
        IPlotElement _cpe;
        internal string _fmtElementName = "s{0}p{1}";
        ChartData _parent;
        ToolTip _ptt;
        ShapeStyle _shapeCon;
        ShapeStyle _shapeSym;
        IPlotElement _spe;
        double _sum = double.NaN;
        Binding _valueBinding;
        int currentIndex = -1;
        Dictionary<DependencyProperty, object> currentStyleDict;
        protected double[,] datavalues;
        internal bool FireNotifications = true;
        protected bool[] isTimeValues;
        protected List<object> listY = new List<object>();
        internal int nloaded;
        protected double[,] previousDataValues;
        UIElement selectedItemLabel;
        protected string _xfmt;
        protected string yfmt;

        public DataSeries()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler PlotElementLoaded;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler PlotElementUnloaded;

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取设置自定义标签模板
        /// </summary>
        public DataTemplate PointLabelTemplate
        {
            get { return (DataTemplate)base.GetValue(PointLabelTemplateProperty); }
            set { base.SetValue(PointLabelTemplateProperty, value); }
        }

        /// <summary>
        /// 获取设置Tooltip模板
        /// </summary>
        public DataTemplate PointTooltipTemplate
        {
            get { return (DataTemplate)base.GetValue(PointTooltipTemplateProperty); }
            set { base.SetValue(PointTooltipTemplateProperty, value); }
        }

        public Aggregate Aggregate
        {
            get { return (Aggregate)base.GetValue(AggregateProperty); }
            set { base.SetValue(AggregateProperty, value); }
        }

        public bool AutoGenerated
        {
            get { return _autoGenerated; }
            internal set { _autoGenerated = value; }
        }

        [DefaultValue("")]
        public string AxisX
        {
            get { return _ax; }
            set
            {
                if (_ax != value)
                {
                    _ax = value;
                    FirePropertyChanged("AxisX");
                }
            }
        }

        [DefaultValue("")]
        public string AxisY
        {
            get { return _ay; }
            set
            {
                if (_ay != value)
                {
                    _ay = value;
                    FirePropertyChanged("AxisY");
                }
            }
        }

        internal WriteableBitmap Bitmap { get; set; }

        public ChartType? ChartType
        {
            get { return (ChartType?)base.GetValue(ChartTypeProperty); }
            set { base.SetValue(ChartTypeProperty, value); }
        }

        public object Connection
        {
            get { return base.GetValue(ConnectionProperty); }
            set { base.SetValue(ConnectionProperty, value); }
        }

        public Brush ConnectionFill
        {
            get { return (Brush)base.GetValue(ConnectionFillProperty); }
            set { base.SetValue(ConnectionFillProperty, value); }
        }

        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        internal ShapeStyle ConnectionShape
        {
            get { return _shapeCon; }
            set
            {
                if (_shapeCon != value)
                {
                    _shapeCon = value;
                    if (_shapeCon != null)
                    {
                        _shapeCon.StrokeThickness = ConnectionStrokeThickness;
                        _shapeCon.StrokeDashArray = ConnectionStrokeDashes;
                        if (ConnectionStroke != null)
                        {
                            _shapeCon.Stroke = ConnectionStroke;
                        }
                        if (ConnectionFill != null)
                        {
                            _shapeCon.Fill = ConnectionFill;
                        }
                    }
                }
            }
        }

        public Brush ConnectionStroke
        {
            get { return (Brush)base.GetValue(ConnectionStrokeProperty); }
            set { base.SetValue(ConnectionStrokeProperty, value); }
        }

        public DoubleCollection ConnectionStrokeDashes
        {
            get { return (DoubleCollection)base.GetValue(ConnectionStrokeDashesProperty); }
            set { base.SetValue(ConnectionStrokeDashesProperty, value); }
        }

        public double ConnectionStrokeThickness
        {
            get { return (double)((double)base.GetValue(ConnectionStrokeThicknessProperty)); }
            set { base.SetValue(ConnectionStrokeThicknessProperty, (double)value); }
        }

        public Style ConnectionStyle
        {
            get { return (Style)base.GetValue(ConnectionStyleProperty); }
            set { base.SetValue(ConnectionStyleProperty, value); }
        }

#if ANDROID
    new
#endif
        internal RenderContext Context { get; set; }

        internal Dictionary<DependencyProperty, object> CurrentStyleDict
        {
            get { return currentStyleDict; }
            set { currentStyleDict = value; }
        }

        internal bool Dirty
        {
            get { return (datavalues == null); }
            set
            {
                if (value)
                {
                    datavalues = null;
                    _sum = double.NaN;
                }
            }
        }

        internal ToolTip PointToolTip
        {
            get
            {
                if ((_ptt == null) && (PointTooltipTemplate != null))
                {
                    object obj2 = PointTooltipTemplate.LoadContent();
                    if (obj2 is ToolTip)
                    {
                        _ptt = (ToolTip)obj2;
                    }
                    else
                    {
                        ToolTip tip = new ToolTip();
                        tip.Content = obj2;
                        _ptt = tip;
                    }
                }
                return _ptt;
            }
        }

#if ANDROID
    new
#endif
        public SeriesDisplay Display
        {
            get { return (SeriesDisplay)base.GetValue(DisplayProperty); }
            set { base.SetValue(DisplayProperty, value); }
        }

        internal bool HasDataSource
        {
            get
            {
                if ((ItemsSource == null) && (ValuesSource == null))
                {
                    return (Values != null);
                }
                return true;
            }
        }

        internal virtual bool IsDefaultConnection
        {
            get { return (base.ReadLocalValue(ConnectionProperty) == DependencyProperty.UnsetValue); }
        }

        internal virtual bool IsDefaultSymbol
        {
            get { return (base.ReadLocalValue(SymbolProperty) == DependencyProperty.UnsetValue); }
        }

        internal bool IsStacked
        {
            get
            {
                if (ChartType.HasValue)
                {
                    ChartSubtype subtype = ChartTypes.GetSubtype(ChartType.ToString());
                    if ((subtype != null) && subtype.RendererOptions.Contains("Stacked=None"))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)base.GetValue(ItemsSourceProperty); }
            set { base.SetValue(ItemsSourceProperty, value); }
        }

        public string Label
        {
            get { return (string)((string)base.GetValue(LabelProperty)); }
            set { base.SetValue(LabelProperty, value); }
        }

        internal int Length
        {
            get
            {
                if (datavalues != null)
                {
                    return datavalues.GetLength(1);
                }
                return 0;
            }
        }

        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public virtual Binding[] MemberPaths
        {
            get
            {
                if (ValueBinding == null)
                {
                    return null;
                }
                return new Binding[] { ValueBinding };
            }
        }

        internal ChartData ParentData
        {
            get { return _parent; }
            set { _parent = value; }
        }
        
        internal UIElement SelectedItemLabel
        {
            get
            {
                if ((selectedItemLabel == null) && (SelectedItemLabelTemplate != null))
                {
                    selectedItemLabel = SelectedItemLabelTemplate.LoadContent() as UIElement;
                }
                return selectedItemLabel;
            }
        }

        public DataTemplate SelectedItemLabelTemplate
        {
            get { return (DataTemplate)base.GetValue(SelectedItemLabelTemplateProperty); }
            set { base.SetValue(SelectedItemLabelTemplateProperty, value); }
        }

        public Style SelectedItemStyle
        {
            get { return (Style)base.GetValue(SelectedItemStyleProperty); }
            set { base.SetValue(SelectedItemStyleProperty, value); }
        }

        public object Symbol
        {
            get { return base.GetValue(SymbolProperty); }
            set { base.SetValue(SymbolProperty, value); }
        }

        [DefaultValue((string)null)]
        public Brush SymbolFill
        {
            get { return (Brush)base.GetValue(SymbolFillProperty); }
            set { base.SetValue(SymbolFillProperty, value); }
        }

        public Marker SymbolMarker
        {
            get { return (Marker)base.GetValue(SymbolMarkerProperty); }
            set { base.SetValue(SymbolMarkerProperty, value); }
        }

        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        internal ShapeStyle SymbolShape
        {
            get { return _shapeSym; }
            set
            {
                if (_shapeSym != value)
                {
                    _shapeSym = value;
                    if (_shapeSym != null)
                    {
                        _shapeSym.StrokeThickness = SymbolStrokeThickness;
                        _shapeSym.StrokeDashArray = SymbolStrokeDashes;
                        if (base.ReadLocalValue(SymbolStrokeProperty) != DependencyProperty.UnsetValue)
                        {
                            _shapeSym.Stroke = SymbolStroke;
                        }
                        if (SymbolFill != null)
                        {
                            _shapeSym.Fill = SymbolFill;
                        }
                    }
                }
            }
        }

        public Size SymbolSize
        {
            get { return (Size)base.GetValue(SymbolSizeProperty); }
            set { base.SetValue(SymbolSizeProperty, value); }
        }

        public Brush SymbolStroke
        {
            get { return (Brush)base.GetValue(SymbolStrokeProperty); }
            set { base.SetValue(SymbolStrokeProperty, value); }
        }

        public DoubleCollection SymbolStrokeDashes
        {
            get { return (DoubleCollection)base.GetValue(SymbolStrokeDashesProperty); }
            set { base.SetValue(SymbolStrokeDashesProperty, value); }
        }

        public double SymbolStrokeThickness
        {
            get { return (double)((double)base.GetValue(SymbolStrokeThicknessProperty)); }
            set { base.SetValue(SymbolStrokeThicknessProperty, (double)value); }
        }

        public Style SymbolStyle
        {
            get { return (Style)base.GetValue(SymbolStyleProperty); }
            set { base.SetValue(SymbolStyleProperty, value); }
        }

        [DefaultValue((string)null)]
        public Binding ValueBinding
        {
            get { return _valueBinding; }
            set
            {
                _valueBinding = value;
                Dirty = true;
                FirePropertyChanged("ValueBinding");
            }
        }

        public DoubleCollection Values
        {
            get { return (DoubleCollection)base.GetValue(ValuesProperty); }
            set { base.SetValue(ValuesProperty, value); }
        }

        public IEnumerable ValuesSource
        {
            get { return (IEnumerable)base.GetValue(ValuesSourceProperty); }
            set { base.SetValue(ValuesSourceProperty, value); }
        }

        protected double[,] AggregateValues(double[,] values)
        {
            double[,] numArray = values;
            ChartData parentData = ParentData;
            Aggregate aggregate = parentData.Aggregate;
            if ((Aggregate != Aggregate.None) || (ItemsSource != null))
            {
                aggregate = Aggregate;
            }
            if (((values != null) && (parentData != null)) && (aggregate != Aggregate.None))
            {
                object[] itemNamesInternal = parentData.ItemNamesInternal;
                Dictionary<object, List<double>> dictionary = new Dictionary<object, List<double>>();
                int num = (itemNamesInternal != null) ? itemNamesInternal.Length : 0;
                int length = values.GetLength(1);
                int num3 = values.GetLength(0);
                for (int i = 0; i < num; i++)
                {
                    List<double> list;
                    object obj2 = itemNamesInternal[i];
                    if (obj2 != null)
                    {
                        obj2 = obj2.ToString();
                    }
                    else
                    {
                        obj2 = "";
                    }
                    if (!dictionary.TryGetValue(obj2, out list))
                    {
                        list = new List<double>();
                        dictionary[obj2] = list;
                    }
                }
                for (int j = 0; j < length; j++)
                {
                    List<double> list2;
                    object obj3 = null;
                    if (j < num)
                    {
                        obj3 = itemNamesInternal[j];
                    }
                    if (num3 > 1)
                    {
                        obj3 = (double)values[1, j];
                    }
                    if (obj3 != null)
                    {
                        obj3 = obj3.ToString();
                    }
                    else
                    {
                        obj3 = "";
                    }
                    if (!dictionary.TryGetValue(obj3, out list2))
                    {
                        list2 = new List<double>();
                        dictionary[obj3] = list2;
                    }
                    list2.Add(values[0, j]);
                }
                num = dictionary.Count;
                if (num3 > 1)
                {
                    numArray = new double[2, num];
                }
                else
                {
                    numArray = new double[1, num];
                }
                int num6 = 0;
                foreach (object obj4 in dictionary.Keys)
                {
                    List<double> list3 = dictionary[obj4];
                    double sum = 0.0;
                    Statistics statistics = new Statistics((IList<double>)list3);
                    switch (aggregate)
                    {
                        case Aggregate.Sum:
                            sum = statistics.Sum;
                            break;

                        case Aggregate.Count:
                            sum = statistics.Count;
                            break;

                        case Aggregate.Average:
                            sum = statistics.Avg;
                            break;

                        case Aggregate.Minimum:
                            sum = statistics.Min;
                            break;

                        case Aggregate.Maximum:
                            sum = statistics.Max;
                            break;

                        case Aggregate.Variance:
                            sum = statistics.Variance;
                            break;

                        case Aggregate.VariancePop:
                            sum = statistics.VariancePop;
                            break;

                        case Aggregate.StandardDeviation:
                            sum = statistics.StandardDeviation;
                            break;

                        case Aggregate.StandardDeviationPop:
                            sum = statistics.StandardDeviationPop;
                            break;
                    }
                    if (num3 > 1)
                    {
                        numArray[1, num6] = num6;
                    }
                    numArray[0, num6++] = sum;
                }
            }
            return numArray;
        }

        Dictionary<DependencyProperty, object> ApplyStyle(PlotElement pe, Style style)
        {
            Dictionary<DependencyProperty, object> dictionary = new Dictionary<DependencyProperty, object>();
            foreach (Setter setter in style.Setters)
            {
                dictionary.Add(setter.Property, pe.GetValue(setter.Property));
                pe.ClearValue(setter.Property);
            }
            if (pe.GetValue(FrameworkElement.StyleProperty) != null)
            {
                dictionary.Add(FrameworkElement.StyleProperty, pe.GetValue(FrameworkElement.StyleProperty));
            }
            pe.Style = style;
            return dictionary;
        }

        void ApplyStyle(PlotElement pe, Dictionary<DependencyProperty, object> dict)
        {
            if ((pe != null) && (dict != null))
            {
                foreach (KeyValuePair<DependencyProperty, object> pair in dict)
                {
                    DependencyProperty dp = pair.Key;
                    pe.SetValue(dp, pair.Value);
                }
            }
        }

        string[] IDataSeriesInfo.GetDataNames()
        {
            return GetDataNamesInternal();
        }

        ValueCoordinate[] IDataSeriesInfo.GetValueCoordinates()
        {
            return GetValueCoordinates(true);
        }

        double[,] IDataSeriesInfo.GetValues()
        {
            return GetValues();
        }

        void IDataSeriesInfo.SetResolvedValues(int index, object[] vals)
        {
            SetResolvedValues(index, vals);
        }

        internal ValueCoordinate Check(ValueCoordinate vc)
        {
            switch (vc)
            {
                case ValueCoordinate.X:
                    if (!string.IsNullOrEmpty(AxisX))
                    {
                        vc = ValueCoordinate.None;
                    }
                    return vc;

                case ValueCoordinate.Y:
                    if (!string.IsNullOrEmpty(AxisY))
                    {
                        vc = ValueCoordinate.None;
                    }
                    return vc;
            }
            return vc;
        }

        internal void ClearDataCache()
        {
            previousDataValues = null;
        }

        void CollectionViewCurrentChanged(object sender, object e)
        {
            UpdateSelection(true);
        }

        static void ConnectionFillChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            ShapeStyle connectionShape = series.ConnectionShape;
            series.ConnectionShape = null;
            series.FirePropertyChanged("ConnectionFill");
        }

        static void ConnectionStrokeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            ShapeStyle connectionShape = series.ConnectionShape;
            series.ConnectionShape = null;
            series.FirePropertyChanged("ConnectionStroke");
        }

        static void ConnectionStrokeThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            ShapeStyle connectionShape = series.ConnectionShape;
            if (connectionShape != null)
            {
                connectionShape.StrokeThickness = series.ConnectionStrokeThickness;
            }
            series.FirePropertyChanged("ConnectionStrokeThickness");
        }

        internal static double ConvertObject(object obj, double index)
        {
            double naN = double.NaN;
            if (obj == null)
            {
                return double.NaN;
            }
            if (obj is DateTime)
            {
                return ((DateTime)obj).ToOADate();
            }
            string s = (string)(obj as string);
            if (s != null)
            {
                if (double.TryParse(s, out naN))
                {
                    return naN;
                }
                DateTime minValue = DateTime.MinValue;
                if (DateTime.TryParse(s, out minValue))
                {
                    return minValue.ToOADate();
                }
                return index;
            }
            try
            {
                return Convert.ToDouble(obj, (IFormatProvider)CultureInfo.CurrentCulture);
            }
            catch (InvalidCastException)
            {
                return double.NaN;
            }
        }

        internal static double ConvertObjectDateTime(object obj, double index)
        {
            if (obj is DateTime)
            {
                return ((DateTime)obj).ToOADate();
            }
            if (obj != null)
            {
                if (obj is double)
                {
                    return (double)((double)obj);
                }
                string str = (string)(obj as string);
                if (str != null)
                {
                    DateTime minValue = DateTime.MinValue;
                    if (DateTime.TryParse(str, out minValue))
                    {
                        return minValue.ToOADate();
                    }
                    return index;
                }
            }
            return double.NaN;
        }

        internal static double ConvertObjectNumber(object obj, double index)
        {
            double naN = double.NaN;
            if (obj is double)
            {
                return (double)((double)obj);
            }
            if (obj == null)
            {
                return double.NaN;
            }
            string s = (string)(obj as string);
            if (s == null)
            {
                if (obj is DateTime)
                {
                    return ((DateTime)obj).ToOADate();
                }
                try
                {
                    return Convert.ToDouble(obj, (IFormatProvider)CultureInfo.CurrentCulture);
                }
                catch (InvalidCastException)
                {
                    return double.NaN;
                }
            }
            if (!double.TryParse(s, out naN))
            {
                naN = index;
            }
            return naN;
        }

        internal virtual DataPoint CreateDataPoint(int i, int j)
        {
            return new DataPoint(this, i, j, GetDataNamesInternal());
        }

        internal double[,] CreateValues(IList[] lists)
        {
            double[,] numArray = null;
            if (lists != null)
            {
                int length = lists.Length;
                if (length <= 0)
                {
                    return numArray;
                }
                int maxCount = GetMaxCount(lists);
                if (maxCount <= 0)
                {
                    return numArray;
                }
                numArray = new double[length, maxCount];
                for (int i = 0; i < length; i++)
                {
                    if (lists[i] is double[])
                    {
                        double[] numArray2 = (double[])lists[i];
                        int num4 = numArray2.Length;
                        for (int j = 0; j < num4; j++)
                        {
                            numArray[i, j] = numArray2[j];
                        }
                    }
                    else if (lists[i] is float[])
                    {
                        float[] numArray3 = (float[])lists[i];
                        int num6 = numArray3.Length;
                        for (int k = 0; k < num6; k++)
                        {
                            numArray[i, k] = numArray3[k];
                        }
                    }
                    else
                    {
                        IList list = lists[i];
                        if (list != null)
                        {
                            int count = list.Count;
                            for (int m = 0; m < maxCount; m++)
                            {
                                if (m < count)
                                {
                                    numArray[i, m] = ConvertObject(list[m], (double)m);
                                }
                                else
                                {
                                    numArray[i, m] = double.NaN;
                                }
                            }
                        }
                    }
                }
            }
            return numArray;
        }

        internal void FireLoaded(object sender, EventArgs e)
        {
            if ((ParentData != null) && (ParentData.LoadAnimation != null))
            {
                PlotElement element = sender as PlotElement;
                if (element != null)
                {
                    bool flag = true;
                    if (element is Lines)
                    {
                        flag = !IsEqualArrays(datavalues, previousDataValues);
                    }
                    else if (((datavalues != null) && (previousDataValues != null)) && (element.DataPoint != null))
                    {
                        int pointIndex = element.DataPoint.PointIndex;
                        int num2 = Math.Min(datavalues.GetLength(1), previousDataValues.GetLength(1));
                        int num3 = Math.Min(datavalues.GetLength(0), previousDataValues.GetLength(0));
                        if ((pointIndex >= 0) && (pointIndex < num2))
                        {
                            flag = false;
                            for (int i = 0; i < num3; i++)
                            {
                                if (datavalues[i, pointIndex] != previousDataValues[i, pointIndex])
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                    nloaded++;
                    if (nloaded >= base.Children.Count)
                    {
                        previousDataValues = datavalues;
                    }
                    if (flag)
                    {
                        ParentData.LoadAnimation.Start(sender as PlotElement);
                    }
                }
            }
            if (PlotElementLoaded != null)
            {
                PlotElementLoaded(sender, e);
            }
        }

        internal void FirePlotElementLoaded(object sender, EventArgs e)
        {
            if (PlotElementLoaded != null)
            {
                PlotElementLoaded(sender, e);
            }
        }

        internal void FirePlotElementUnloaded(object sender, EventArgs e)
        {
            if (PlotElementUnloaded != null)
            {
                PlotElementUnloaded(sender, e);
            }
        }

        internal void FirePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        internal IPlotElement GetConnectionElement(BaseRenderer rend, bool clear)
        {
            if (clear)
            {
                _cpe = null;
            }
            if (_cpe == null)
            {
                _cpe = Connection as IPlotElement;
                if ((_cpe == null) && ChartType.HasValue)
                {
                    ChartSubtype subtype = ChartTypes.GetSubtype(ChartType.ToString());
                    _cpe = subtype.Connection as IPlotElement;
                }
                if ((_cpe == null) && !ChartType.HasValue)
                {
                    _cpe = rend.Connection as IPlotElement;
                }
            }
            return _cpe;
        }

        internal virtual string[] GetDataNamesInternal()
        {
            return new string[] { "Values" };
        }

        public double GetDataValue(string name, int pointIndex)
        {
            int num = -1;
            string[] dataNamesInternal = GetDataNamesInternal();
            if (dataNamesInternal != null)
            {
                for (int i = 0; i < dataNamesInternal.Length; i++)
                {
                    if (string.Compare(dataNamesInternal[i], name, (StringComparison)StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        num = i;
                        break;
                    }
                }
            }
            if (num == -1)
            {
                throw new ArgumentOutOfRangeException("name");
            }
            double[,] values = GetValues();
            if (values == null)
            {
                throw new ArgumentException(C1Localizer.GetString("Data value does not exist."));
            }
            int length = values.GetLength(0);
            int num4 = values.GetLength(1);
            if (num >= length)
            {
                throw new ArgumentOutOfRangeException("name");
            }
            if ((pointIndex < 0) || (pointIndex >= num4))
            {
                throw new ArgumentOutOfRangeException("pointIndex");
            }
            return values[num, pointIndex];
        }

        internal virtual string GetElementName(int seriesIndex, int pointIndex)
        {
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, _fmtElementName, new object[] { (int)seriesIndex, (int)pointIndex });
        }

        internal string GetFormattedValue(int i, int j)
        {
            string str = null;
            object obj2 = GetValue(i, j);
            if (obj2 == null)
            {
                return str;
            }
            IFormattable formattable = obj2 as IFormattable;
            string xfmt = null;
            if (formattable != null)
            {
                ValueCoordinate coordinate = GetValueCoordinates(false)[i];
                if (coordinate == ValueCoordinate.X)
                {
                    xfmt = _xfmt;
                }
                else if (coordinate == ValueCoordinate.X)
                {
                    xfmt = yfmt;
                }
                if (!string.IsNullOrEmpty(xfmt))
                {
                    return formattable.ToString(xfmt, (IFormatProvider)null);
                }
                return obj2.ToString();
            }
            return obj2.ToString();
        }

        internal bool GetIsClustered(bool defval)
        {
            object symbol = Symbol;
            if (IsDefaultSymbol && ChartType.HasValue)
            {
                ChartSubtype subtype = ChartTypes.GetSubtype(ChartType.ToString());
                if (subtype != null)
                {
                    symbol = subtype.Symbol;
                    if (symbol == null)
                    {
                        return false;
                    }
                }
            }
            if (!IsDefaultSymbol && (symbol == null))
            {
                return false;
            }
            return Utils.GetIsClustered(defval, symbol);
        }

        internal static int GetMaxCount(params IList[] list)
        {
            int count = 0;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Count > count)
                {
                    count = list[i].Count;
                }
            }
            return count;
        }

        internal void GetMinMax(ref Point min, ref Point max)
        {
            double[,] values = GetValues();
            if (values != null)
            {
                int length = values.GetLength(0);
                int num2 = values.GetLength(1);
                if ((length > 0) && (num2 > 0))
                {
                    ValueCoordinate[] valueCoordinates = GetValueCoordinates(false);
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < num2; j++)
                        {
                            double num5 = values[i, j];
                            switch (valueCoordinates[i])
                            {
                                case ValueCoordinate.X:
                                    if ((num5 > max.X) || double.IsNaN(max.X))
                                    {
                                        max.X = num5;
                                    }
                                    if ((num5 < min.X) || double.IsNaN(min.X))
                                    {
                                        min.X = num5;
                                    }
                                    break;

                                case ValueCoordinate.Y:
                                    if ((num5 > max.Y) || double.IsNaN(max.Y))
                                    {
                                        max.Y = num5;
                                    }
                                    if ((num5 < min.Y) || double.IsNaN(min.Y))
                                    {
                                        min.Y = num5;
                                    }
                                    break;
                            }
                        }
                    }
                }
                if (length == 1)
                {
                    if (double.IsNaN(min.X))
                    {
                        min.X = 0.0;
                    }
                    if (double.IsNaN(max.X))
                    {
                        max.X = num2 - 1;
                    }
                }
            }
        }

        internal double GetPointSum(int pi)
        {
            ChartData parentData = ParentData;
            if (parentData != null)
            {
                return parentData.GetPointSum(pi);
            }
            return double.NaN;
        }

        internal static double[] GetPointValues(double[,] vals, int ip)
        {
            double[] numArray = null;
            if (vals != null)
            {
                int length = vals.GetLength(1);
                if ((ip < 0) || (ip >= length))
                {
                    return numArray;
                }
                int num2 = vals.GetLength(0);
                numArray = new double[num2];
                for (int i = 0; i < num2; i++)
                {
                    numArray[i] = vals[i, ip];
                }
            }
            return numArray;
        }

        internal double GetSum()
        {
            if (double.IsNaN(_sum))
            {
                double[,] values = GetValues();
                if (values != null)
                {
                    int length = values.GetLength(1);
                    double num2 = 0.0;
                    for (int i = 0; i < length; i++)
                    {
                        if (!double.IsNaN(values[0, i]))
                        {
                            num2 += values[0, i];
                        }
                    }
                    _sum = num2;
                }
            }
            return _sum;
        }

        internal IPlotElement GetSymbolElement(BaseRenderer rend, bool clear)
        {
            if (clear || (SymbolMarker != Marker.None))
            {
                _spe = null;
            }
            if (_spe == null)
            {
                PlotElement symbol = null;
                symbol = Symbol as PlotElement;
                if ((symbol == null) && ChartType.HasValue)
                {
                    symbol = ChartTypes.GetSubtype(ChartType.ToString()).Symbol as PlotElement;
                }
                if ((symbol == null) || (SymbolMarker != Marker.None))
                {
                    symbol = Symbol as PlotElement;
                    if (symbol == null)
                    {
                        symbol = PlotElement.SymbolFromMarker(SymbolMarker);
                    }
                }
                if ((symbol == null) && !ChartType.HasValue)
                {
                    symbol = rend.Symbol as PlotElement;
                }
                if (((symbol != null) && !SymbolSize.IsEmpty) && ((SymbolSize.Width > 0.0) && (SymbolSize.Height > 0.0)))
                {
                    symbol.Size = SymbolSize;
                }
                _spe = symbol;
            }
            return _spe;
        }

        internal virtual Size GetSymbolSize(int pointIndex, DataInfo dataInfo, Chart chart)
        {
            return SymbolSize;
        }

        internal object GetValue(int i, int j)
        {
            object obj2 = null;
            if (datavalues == null)
            {
                return obj2;
            }
            double d = datavalues[i, j];
            if (isTimeValues[i])
            {
                return d.FromOADate();
            }
            return (double)d;
        }

        internal virtual ValueCoordinate[] GetValueCoordinates(bool check)
        {
            if (check)
            {
                return new ValueCoordinate[] { Check(ValueCoordinate.Y) };
            }
            return new ValueCoordinate[] { ValueCoordinate.Y };
        }

        internal virtual double[,] GetValues()
        {
            if (datavalues == null)
            {
                if (ValueBinding == null)
                {
                    InitList(listY, ValuesSource, (IList<double>)Values);
                }
                datavalues = CreateValues(new IList[] { listY });
                if (isTimeValues == null)
                {
                    isTimeValues = new bool[1];
                }
                isTimeValues[0] = IsTimeData(listY);
                datavalues = AggregateValues(datavalues);
                datavalues = ProcessValues(datavalues);
            }
            return datavalues;
        }

        void inc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (FireNotifications)
            {
                FirePropertyChanged("ValuesSource");
            }
        }

        internal static void InitList(List<object> list, IEnumerable vals, IList<double> coll)
        {
            list.Clear();
            if (vals != null)
            {
                IList list2 = vals as IList;
                if (list2 != null)
                {
                    int count = list2.Count;
                    if (list.Capacity < count)
                    {
                        list.Capacity = count;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        list.Add(list2[i]);
                    }
                }
                else
                {
                    IEnumerator enumerator = vals.GetEnumerator();
                    DataUtils.TryReset(enumerator);
                    while (enumerator.MoveNext())
                    {
                        list.Add(enumerator.Current);
                    }
                }
            }
            else if (coll != null)
            {
                int num3 = coll.Count;
                for (int j = 0; j < num3; j++)
                {
                    list.Add((double)coll[j]);
                }
            }
        }

        void inp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FireNotifications)
            {
                FirePropertyChanged("ValuesSource");
            }
        }

#if ANDROID
    new
#endif
        protected void Invalidate()
        {
            Dirty = true;
            FirePropertyChanged("Dirty");
        }

        static bool IsEqualArrays(double[,] a1, double[,] a2)
        {
            if (a1 != a2)
            {
                if ((a1 == null) || (a2 == null))
                {
                    return false;
                }
                int length = a1.GetLength(0);
                int num2 = a1.GetLength(1);
                if (length != a2.GetLength(0))
                {
                    return false;
                }
                if (num2 != a2.GetLength(1))
                {
                    return false;
                }
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < num2; j++)
                    {
                        if (a1[i, j] != a2[i, j])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        protected bool IsTimeData(List<object> list)
        {
            return ((list.Count > 0) && (list[0] is DateTime));
        }

        static void OnAggregateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            series.Dirty = true;
            series.FirePropertyChanged("Aggregate");
        }

        protected static void OnChangeValues(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            INotifyCollectionChanged oldValue = args.OldValue as INotifyCollectionChanged;
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= series.inc_CollectionChanged;
            }
            else
            {
                INotifyPropertyChanged changed2 = args.OldValue as INotifyPropertyChanged;
                if (changed2 != null)
                {
                    changed2.PropertyChanged -= series.inp_PropertyChanged;
                }
            }
            oldValue = args.NewValue as INotifyCollectionChanged;
            if (oldValue != null)
            {
                oldValue.CollectionChanged += series.inc_CollectionChanged;
            }
            else
            {
                INotifyPropertyChanged newValue = args.NewValue as INotifyPropertyChanged;
                if (newValue != null)
                {
                    newValue.PropertyChanged += series.inp_PropertyChanged;
                }
            }
            series.Dirty = true;
            series.FirePropertyChanged(args.Property.ToString());
        }

        static void OnConnectionStrokeDashesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            ShapeStyle connectionShape = series.ConnectionShape;
            if (connectionShape != null)
            {
                connectionShape.StrokeDashArray = series.ConnectionStrokeDashes;
            }
            series.FirePropertyChanged("ConnectionStrokeDashes");
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            ICollectionView oldValue = args.OldValue as ICollectionView;
            if (oldValue != null)
            {
                oldValue.CurrentChanged -= CollectionViewCurrentChanged;
            }
            oldValue = args.NewValue as ICollectionView;
            if (oldValue != null)
            {
                oldValue.CurrentChanged += CollectionViewCurrentChanged; 
                UpdateSelection(true);
            }
        }

        static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            OnChangeValues(obj, args);
            series.OnItemsSourceChanged(args);
        }

        static void OnLabelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataSeries)obj).FirePropertyChanged("Label");
        }

        static void OnPointTooltipChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            series._ptt = null;
            series.FirePropertyChanged("PointTooltipTemplate");
        }

        static void OnSelectedItemLabelTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            if (series.selectedItemLabel != null)
            {
                FrameworkElement selectedItemLabel = series.selectedItemLabel as FrameworkElement;
                if (selectedItemLabel != null)
                {
                    selectedItemLabel.DataContext = null;
                }
                series.selectedItemLabel = null;
            }
        }

        protected static void OnSeriesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            if ((args.Property == ChartTypeProperty) || (args.Property == SymbolMarkerProperty))
            {
                series._spe = null;
                series._cpe = null;
            }
            series.Dirty = true;
            series.FirePropertyChanged(args.Property.ToString());
        }

        static void OnSymbolStrokeDashesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            ShapeStyle symbolShape = series.SymbolShape;
            if (symbolShape != null)
            {
                symbolShape.StrokeDashArray = series.SymbolStrokeDashes;
            }
            series.FirePropertyChanged("SymbolStrokeDashes");
        }

        internal void PerformBinding(Action<INotifyPropertyChanged> action)
        {
            Binding[] memberPaths = MemberPaths;
            IEnumerable itemsSource = ItemsSource;
            if (memberPaths != null)
            {
                IEnumerator enumerator = itemsSource.GetEnumerator();
                enumerator.Reset();
                int length = memberPaths.Length;
                List<object>[] listArray = new List<object>[length];
                for (int i = 0; i < length; i++)
                {
                    listArray[i] = new List<object>();
                }
                DataBindingProxy proxy = new DataBindingProxy();
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    proxy.DataContext = current;
                    for (int k = 0; k < length; k++)
                    {
                        if (memberPaths[k] != null)
                        {
                            listArray[k].Add(proxy.GetValue(memberPaths[k]));
                        }
                    }
                    proxy.DataContext = null;
                    if ((current is INotifyPropertyChanged) && (action != null))
                    {
                        action((INotifyPropertyChanged)current);
                    }
                }
                for (int j = 0; j < length; j++)
                {
                    if (memberPaths[j] != null)
                    {
                        SetResolvedValues(j, listArray[j].ToArray());
                    }
                }
            }
        }

        protected virtual double[,] ProcessValues(double[,] values)
        {
            return values;
        }

        void SelectedPlotElementLoaded(object sender, RoutedEventArgs e)
        {
            PlotElement pe = (PlotElement)sender;
            pe.Loaded -= new RoutedEventHandler(SelectedPlotElementLoaded);
            UpdateSelectionLabel(pe, SelectedItemLabel);
        }

        internal void SetDefaultFormat(string xformat, string yformat)
        {
            _xfmt = xformat;
            yfmt = yformat;
        }

        internal virtual void SetResolvedValues(int index, object[] vals)
        {
            if (index == 0)
            {
                listY.Clear();
                listY.AddRange(vals);
            }
        }

        static void SymbolFillChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            ShapeStyle symbolShape = series.SymbolShape;
            series.SymbolShape = null;
            series.FirePropertyChanged("SymbolFill");
        }

        static void SymbolStrokeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            ShapeStyle symbolShape = series.SymbolShape;
            series.SymbolShape = null;
            series.FirePropertyChanged("SymbolStroke");
        }

        static void SymbolStrokeThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataSeries series = (DataSeries)obj;
            ShapeStyle symbolShape = series.SymbolShape;
            if (symbolShape != null)
            {
                symbolShape.StrokeThickness = series.SymbolStrokeThickness;
            }
            series.FirePropertyChanged("SymbolStrokeThickness");
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Label))
            {
                return base.ToString();
            }
            return Label;
        }

        protected static void UpdateLabelPosition(PlotElement pe, FrameworkElement lbl)
        {
            PlotElement.UpdateLabelPosition(pe, lbl);
        }

        internal void UpdateSelectedElement(int previousIndex, int currentIndex)
        {
            if (SelectedItemStyle != null)
            {
                Dictionary<DependencyProperty, object> currentStyleDict = CurrentStyleDict;
                foreach (PlotElement element in base.Children)
                {
                    if (((element.DataPoint != null) && (element.DataPoint.PointIndex == currentIndex)) && (currentIndex != -1))
                    {
                        CurrentStyleDict = ApplyStyle(element, SelectedItemStyle);
                        element.Style = SelectedItemStyle;
                        Canvas.SetZIndex(element, 1);
                        UIElement selectedItemLabel = SelectedItemLabel;
                        if (selectedItemLabel != null)
                        {
                            FrameworkElement lbl = selectedItemLabel as FrameworkElement;
                            if (lbl != null)
                            {
                                lbl.DataContext = element.DataPoint;
                            }
                            Panel parent = base.Parent as Panel;
                            if (parent != null)
                            {
                                if (!parent.Children.Contains(selectedItemLabel))
                                {
                                    parent.Children.Add(selectedItemLabel);
                                }
                                if (lbl != null)
                                {
                                    UpdateLabelPosition(element, lbl);
                                }
                            }
                            else
                            {
                                element.Loaded += new RoutedEventHandler(SelectedPlotElementLoaded);
                            }
                        }
                    }
                    if ((element.DataPoint != null) && (element.DataPoint.PointIndex == previousIndex))
                    {
                        ApplyStyle(element, currentStyleDict);
                        Canvas.SetZIndex(element, 0);
                    }
                }
            }
        }

        internal void UpdateSelection(bool usePrevious = true)
        {
            ICollectionView itemsSource = ItemsSource as ICollectionView;
            if (itemsSource != null)
            {
                int previousIndex = usePrevious ? currentIndex : -1;
                currentIndex = itemsSource.CurrentPosition;
                UpdateSelectedElement(previousIndex, currentIndex);
            }
        }

        void UpdateSelectionLabel(PlotElement pe, UIElement lbl)
        {
            if (lbl != null)
            {
                Panel parent = base.Parent as Panel;
                if (parent != null)
                {
                    if (!parent.Children.Contains(lbl))
                    {
                        parent.Children.Add(lbl);
                    }
                    FrameworkElement element = lbl as FrameworkElement;
                    if (element != null)
                    {
                        UpdateLabelPosition(pe, element);
                    }
                }
            }
        }

        static void UpdateShapeStyle(DataSeries ds, ShapeStyle ss, Brush brush, Brush autoBrush, DependencyProperty brushProperty)
        {
        }

        static void ValuesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataSeries)obj).FirePropertyChanged("Values");
        }

    }
}

