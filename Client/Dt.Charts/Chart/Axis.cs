#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class Axis : AxisCanvas, INotifyPropertyChanged
    {
        /// <summary>
        /// 调整为12，原NaN
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(Axis), new PropertyMetadata(12d, new PropertyChangedCallback(Axis.OnDPChanged)));

        public static readonly DependencyProperty AnnoAngleProperty = DependencyProperty.Register("AnnoAngle", typeof(double), typeof(Axis), new PropertyMetadata((double)0.0, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty AnnoFormatProperty = DependencyProperty.Register("AnnoFormat", typeof(string), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnAnnoFormatChanged)));
        public static readonly DependencyProperty AnnoPositionProperty = DependencyProperty.Register("AnnoPosition", typeof(Dt.Charts.AnnoPosition), typeof(Axis), new PropertyMetadata(Dt.Charts.AnnoPosition.Auto, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty AnnoTemplateProperty = DependencyProperty.Register("AnnoTemplate", typeof(object), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty AnnoVisibilityProperty = DependencyProperty.Register("AnnoVisibilityProperty", typeof(Dt.Charts.AnnoVisibility), typeof(Axis), new PropertyMetadata(Dt.Charts.AnnoVisibility.HideOverlapped, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty AutoMaxProperty = DependencyProperty.Register("AutoMax", typeof(bool), typeof(Axis), new PropertyMetadata((bool)true, new PropertyChangedCallback(Axis.OnAutoMaxChanged)));
        public static readonly DependencyProperty AutoMinProperty = DependencyProperty.Register("AutoMin", typeof(bool), typeof(Axis), new PropertyMetadata((bool)true, new PropertyChangedCallback(Axis.OnAutoMinChanged)));
        public static readonly DependencyProperty AxisLineProperty = DependencyProperty.Register("AxisLine", typeof(Line), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty IsTimeProperty = DependencyProperty.Register("IsTime", typeof(bool), typeof(Axis), new PropertyMetadata((bool)false, new PropertyChangedCallback(Axis.OnIsTimeChanged)));
        public static readonly DependencyProperty ItemsLabelBindingProperty = DependencyProperty.Register("ItemsLabelBinding", typeof(Binding), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty ItemsValueBindingProperty = DependencyProperty.Register("ItemsValueBinding", typeof(Binding), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty LogBaseProperty = DependencyProperty.Register("LogBase", typeof(double), typeof(Axis), new PropertyMetadata((double)double.NaN, new PropertyChangedCallback(Axis.OnLogBaseChanged)));
        public static readonly DependencyProperty MajorGridFillProperty = DependencyProperty.Register("MajorGridFill", typeof(Brush), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MajorGridStrokeDashesProperty = DependencyProperty.Register("MajorGridStrokeDashes", typeof(DoubleCollection), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MajorGridStrokeProperty = DependencyProperty.Register("MajorGridStroke", typeof(Brush), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MajorGridStrokeThicknessProperty = DependencyProperty.Register("MajorGridStrokeThickness", typeof(double), typeof(Axis), new PropertyMetadata((double)0.5, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MajorTickHeightProperty = DependencyProperty.Register("MajorTickHeight", typeof(double), typeof(Axis), new PropertyMetadata((double)3.0, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MajorTickOverlapProperty = DependencyProperty.Register("MajorTickOverlap", typeof(double), typeof(Axis), new PropertyMetadata((double)0.0, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MajorTickStrokeProperty = DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MajorTickThicknessProperty = DependencyProperty.Register("MajorTickThickness", typeof(double), typeof(Axis), new PropertyMetadata((double)1.0, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MajorUnitProperty = DependencyProperty.Register("MajorUnit", typeof(double), typeof(Axis), new PropertyMetadata((double)double.NaN, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(double), typeof(Axis), new PropertyMetadata((double)double.NaN, new PropertyChangedCallback(Axis.OnMaxChanged)));
        public static readonly DependencyProperty MinorGridStrokeDashesProperty = DependencyProperty.Register("MinorGridStrokeDashes", typeof(DoubleCollection), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MinorGridStrokeProperty = DependencyProperty.Register("MinorGridStroke", typeof(Brush), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MinorGridStrokeThicknessProperty = DependencyProperty.Register("MinorGridStrokeThickness", typeof(double), typeof(Axis), new PropertyMetadata((double)0.25, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MinorTickHeightProperty = DependencyProperty.Register("MinorTickHeight", typeof(double), typeof(Axis), new PropertyMetadata((double)2.0, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MinorTickOverlapProperty = DependencyProperty.Register("MinorTickOverlap", typeof(double), typeof(Axis), new PropertyMetadata((double)0.0, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MinorTickStrokeProperty = DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MinorTickThicknessProperty = DependencyProperty.Register("MinorTickThickness", typeof(double), typeof(Axis), new PropertyMetadata((double)1.0, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MinorUnitProperty = DependencyProperty.Register("MinorUnit", typeof(double), typeof(Axis), new PropertyMetadata((double)double.NaN, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(double), typeof(Axis), new PropertyMetadata((double)double.NaN, new PropertyChangedCallback(Axis.OnMinChanged)));
        public static readonly DependencyProperty OriginProperty = DependencyProperty.Register("Origin", typeof(double), typeof(Axis), new PropertyMetadata((double)double.NaN, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty PlotAreaIndexProperty = DependencyProperty.Register("PlotAreaIndex", typeof(int), typeof(Axis), new PropertyMetadata((int)0));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(AxisPosition), typeof(Axis), new PropertyMetadata(AxisPosition.Near, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty RadarLabelVisibilityProperty = DependencyProperty.Register("RadarLabelVisibility", typeof(Dt.Charts.RadarLabelVisibility), typeof(Axis), new PropertyMetadata(Dt.Charts.RadarLabelVisibility.First, new PropertyChangedCallback(Axis.OnRadarLabelVisibilityChanged)));
        public static readonly DependencyProperty RadarPointIndicesProperty = DependencyProperty.Register("RadarPointIndices", typeof(IList<int>), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnRadarPointIndicesChanged)));
        public static readonly DependencyProperty ReversedProperty = DependencyProperty.Register("Reversed", typeof(bool), typeof(Axis), new PropertyMetadata((bool)false, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(Axis), new PropertyMetadata((double)1.0, new PropertyChangedCallback(Axis.OnScaleChanged)));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(Axis), new PropertyMetadata(null, new PropertyChangedCallback(Axis.OnDPChanged)));
        public static readonly DependencyProperty UseExactLimitsProperty = DependencyProperty.Register("UseExactLimits", typeof(bool), typeof(Axis), new PropertyMetadata((bool)false));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(Axis), new PropertyMetadata((double)0.0, new PropertyChangedCallback(Axis.OnValueChanged)));
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(bool), typeof(Axis), new PropertyMetadata((bool)true, new PropertyChangedCallback(Axis.OnDPChanged)));

        double _actualMax;
        double _actualMin;
        IAxisScrollBar _asb;
        Dt.Charts.AxisType _axType;
        Func<double, double> _dependentConverter;
        bool _fixedType;
        bool _isDependent;
        bool? _isTime;
        double _minScale;
        const double _MinScale = 1E-05;
        DispatcherTimer _timer;
        double _tmpScale;
        double _tmpValue;
        internal double Change;
        internal IAxis iax;
        internal bool IsValidFmt;
        internal bool IsValidTimeFmt;
        bool notify;

        public Axis() : this(Dt.Charts.AxisType.Y)
        {
        }

        internal Axis(Dt.Charts.AxisType atype) : base(null)
        {
            _axType = Dt.Charts.AxisType.Y;
            _minScale = 1E-05;
            Change = 0.5;
            _isTime = null;
            IsValidFmt = true;
            IsValidTimeFmt = true;
            notify = true;
            _tmpValue = double.NaN;
            _tmpScale = double.NaN;
            base._axis = this;
            iax = this;
            _axType = atype;
        }

        void _timer_Tick(object sender, object e)
        {
            _timer.Stop();
            if ((_tmpValue != Value) || (_tmpScale != Scale))
            {
                notify = false;
                Value = _tmpValue;
                Scale = _tmpScale;
                notify = true;
                OnPropertyChanged("");
            }
        }

        public event AnnoCreatedEventHandler AnnoCreated;

        internal event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChanged += value;
            }
            remove
            {
                PropertyChanged -= value;
            }
        }

        /// <summary>
        /// 获取设置坐标轴标题
        /// </summary>
        public object Title
        {
            get { return base.GetValue(TitleProperty); }
            set { base.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置坐标轴单位
        /// </summary>
        public double MajorUnit
        {
            get { return (double)((double)base.GetValue(MajorUnitProperty)); }
            set { base.SetValue(MajorUnitProperty, (double)value); }
        }

        public double ActualMax
        {
            get { return _actualMax; }
            internal set
            {
                if (!double.IsNaN(value))
                {
                    _actualMax = value;
                }
            }
        }

        public double ActualMin
        {
            get { return _actualMin; }
            internal set
            {
                if (!double.IsNaN(value))
                {
                    _actualMin = value;
                }
            }
        }

        public double AnnoAngle
        {
            get { return (double)((double)base.GetValue(AnnoAngleProperty)); }
            set { base.SetValue(AnnoAngleProperty, (double)value); }
        }

        public string AnnoFormat
        {
            get { return (string)((string)base.GetValue(AnnoFormatProperty)); }
            set { base.SetValue(AnnoFormatProperty, value); }
        }

        internal string AnnoFormatInternal { get; set; }

        public Dt.Charts.AnnoPosition AnnoPosition
        {
            get { return (Dt.Charts.AnnoPosition)base.GetValue(AnnoPositionProperty); }
            set { base.SetValue(AnnoPositionProperty, value); }
        }

        public object AnnoTemplate
        {
            get { return base.GetValue(AnnoTemplateProperty); }
            set { base.SetValue(AnnoTemplateProperty, value); }
        }

        public Dt.Charts.AnnoVisibility AnnoVisibility
        {
            get { return (Dt.Charts.AnnoVisibility)base.GetValue(AnnoVisibilityProperty); }
            set { base.SetValue(AnnoVisibilityProperty, value); }
        }

        public bool AutoMax
        {
            get { return (bool)((bool)base.GetValue(AutoMaxProperty)); }
            set { base.SetValue(AutoMaxProperty, (bool)value); }
        }

        public bool AutoMin
        {
            get { return (bool)((bool)base.GetValue(AutoMinProperty)); }
            set { base.SetValue(AutoMinProperty, (bool)value); }
        }

        public Line AxisLine
        {
            get { return (Line)base.GetValue(AxisLineProperty); }
            set { base.SetValue(AxisLineProperty, value); }
        }

        public Dt.Charts.AxisType AxisType
        {
            get { return _axType; }
            set
            {
                if ((_axType != value) && !_fixedType)
                {
                    _axType = value;
                    OnPropertyChanged("AxisType");
                }
            }
        }

        public Func<double, double> DependentAxisConverter
        {
            get { return _dependentConverter; }
            set { _dependentConverter = value; }
        }

        public Windows.UI.Xaml.Media.FontFamily FontFamily
        {
            get { return (Windows.UI.Xaml.Media.FontFamily)base.GetValue(FontFamilyProperty); }
            set { base.SetValue(FontFamilyProperty, value); }
        }

        public double FontSize
        {
            get { return (double)((double)base.GetValue(FontSizeProperty)); }
            set { base.SetValue(FontSizeProperty, (double)value); }
        }

#if ANDROID
    new
#endif
        public Brush Foreground
        {
            get { return (Brush)base.GetValue(ForegroundProperty); }
            set { base.SetValue(ForegroundProperty, value); }
        }

        internal Brush ForegroundInternal
        {
            get
            {
                Brush foreground = Foreground;
                if (((foreground == null) && (base.Chart != null)) && (base.Chart.Foreground != null))
                {
                    return base.Chart.Foreground;
                }
                return foreground;
            }
        }

        public bool IsDependent
        {
            get
            {
                if (_fixedType)
                {
                    return false;
                }
                return _isDependent;
            }
            set { _isDependent = value; }
        }

        public bool IsTime
        {
            get { return (bool)((bool)base.GetValue(IsTimeProperty)); }
            set { base.SetValue(IsTimeProperty, (bool)value); }
        }

        public Binding ItemsLabelBinding
        {
            get { return (Binding)base.GetValue(ItemsLabelBindingProperty); }
            set { base.SetValue(ItemsLabelBindingProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)base.GetValue(ItemsSourceProperty); }
            set { base.SetValue(ItemsSourceProperty, value); }
        }

        public Binding ItemsValueBinding
        {
            get { return (Binding)base.GetValue(ItemsValueBindingProperty); }
            set { base.SetValue(ItemsValueBindingProperty, value); }
        }

        public double LogBase
        {
            get { return (double)((double)base.GetValue(LogBaseProperty)); }
            set { base.SetValue(LogBaseProperty, (double)value); }
        }

        public Brush MajorGridFill
        {
            get { return (Brush)base.GetValue(MajorGridFillProperty); }
            set { base.SetValue(MajorGridFillProperty, value); }
        }

        public Brush MajorGridStroke
        {
            get { return (Brush)base.GetValue(MajorGridStrokeProperty); }
            set { base.SetValue(MajorGridStrokeProperty, value); }
        }

        public DoubleCollection MajorGridStrokeDashes
        {
            get { return (DoubleCollection)base.GetValue(MajorGridStrokeDashesProperty); }
            set { base.SetValue(MajorGridStrokeDashesProperty, value); }
        }

        public double MajorGridStrokeThickness
        {
            get { return (double)((double)base.GetValue(MajorGridStrokeThicknessProperty)); }
            set { base.SetValue(MajorGridStrokeThicknessProperty, (double)value); }
        }

        public double MajorTickHeight
        {
            get { return (double)((double)base.GetValue(MajorTickHeightProperty)); }
            set { base.SetValue(MajorTickHeightProperty, (double)value); }
        }

        public double MajorTickOverlap
        {
            get { return (double)((double)base.GetValue(MajorTickOverlapProperty)); }
            set { base.SetValue(MajorTickOverlapProperty, (double)value); }
        }

        public Brush MajorTickStroke
        {
            get { return (Brush)base.GetValue(MajorTickStrokeProperty); }
            set { base.SetValue(MajorTickStrokeProperty, value); }
        }

        public double MajorTickThickness
        {
            get { return (double)((double)base.GetValue(MajorTickThicknessProperty)); }
            set { base.SetValue(MajorTickThicknessProperty, (double)value); }
        }

        public double Max
        {
            get { return (double)((double)base.GetValue(MaxProperty)); }
            set { base.SetValue(MaxProperty, (double)value); }
        }

        public double Min
        {
            get { return (double)((double)base.GetValue(MinProperty)); }
            set { base.SetValue(MinProperty, (double)value); }
        }

        public Brush MinorGridStroke
        {
            get { return (Brush)base.GetValue(MinorGridStrokeProperty); }
            set { base.SetValue(MinorGridStrokeProperty, value); }
        }

        public DoubleCollection MinorGridStrokeDashes
        {
            get { return (DoubleCollection)base.GetValue(MinorGridStrokeDashesProperty); }
            set { base.SetValue(MinorGridStrokeDashesProperty, value); }
        }

        public double MinorGridStrokeThickness
        {
            get { return (double)((double)base.GetValue(MinorGridStrokeThicknessProperty)); }
            set { base.SetValue(MinorGridStrokeThicknessProperty, (double)value); }
        }

        public double MinorTickHeight
        {
            get { return (double)((double)base.GetValue(MinorTickHeightProperty)); }
            set { base.SetValue(MinorTickHeightProperty, (double)value); }
        }

        public double MinorTickOverlap
        {
            get { return (double)((double)base.GetValue(MinorTickOverlapProperty)); }
            set { base.SetValue(MinorTickOverlapProperty, (double)value); }
        }

        public Brush MinorTickStroke
        {
            get { return (Brush)base.GetValue(MinorTickStrokeProperty); }
            set { base.SetValue(MinorTickStrokeProperty, value); }
        }

        public double MinorTickThickness
        {
            get { return (double)((double)base.GetValue(MinorTickThicknessProperty)); }
            set { base.SetValue(MinorTickThicknessProperty, (double)value); }
        }

        public double MinorUnit
        {
            get { return (double)((double)base.GetValue(MinorUnitProperty)); }
            set { base.SetValue(MinorUnitProperty, (double)value); }
        }

        [DefaultValue((double)1E-05)]
        public double MinScale
        {
            get { return _minScale; }
            set
            {
                if (value > 1.0)
                {
                    value = 1.0;
                }
                else if (value <= 0.0)
                {
                    value = 1E-05;
                }
                if (_minScale != value)
                {
                    _minScale = value;
                    if (Scale < _minScale)
                    {
                        Scale = _minScale;
                    }
                    OnPropertyChanged("MinScale");
                }
            }
        }

        public double Origin
        {
            get { return (double)((double)base.GetValue(OriginProperty)); }
            set { base.SetValue(OriginProperty, (double)value); }
        }

        public int PlotAreaIndex
        {
            get { return (int)((int)base.GetValue(PlotAreaIndexProperty)); }
            set { base.SetValue(PlotAreaIndexProperty, (int)value); }
        }

        public AxisPosition Position
        {
            get { return (AxisPosition)base.GetValue(PositionProperty); }
            set { base.SetValue(PositionProperty, value); }
        }

        public Dt.Charts.RadarLabelVisibility RadarLabelVisibility
        {
            get { return (Dt.Charts.RadarLabelVisibility)base.GetValue(RadarLabelVisibilityProperty); }
            set { base.SetValue(RadarLabelVisibilityProperty, value); }
        }

        public IList<int> RadarPointIndices
        {
            get { return (IList<int>)base.GetValue(RadarPointIndicesProperty); }
            set { base.SetValue(RadarPointIndicesProperty, value); }
        }

        public bool Reversed
        {
            get { return (bool)((bool)base.GetValue(ReversedProperty)); }
            set { base.SetValue(ReversedProperty, (bool)value); }
        }

        new public double Scale
        {
            get { return (double)((double)base.GetValue(ScaleProperty)); }
            set { base.SetValue(ScaleProperty, (double)value); }
        }

        public IAxisScrollBar ScrollBar
        {
            get { return _asb; }
            set
            {
                if (value != _asb)
                {
                    if (_asb != null)
                    {
                        _asb.AxisRangeChanged -= new AxisRangeChangedEventHandler(ScrollBar_AxisRangeChanged);
                    }
                    _asb = value;
                    if (_asb != null)
                    {
                        _asb.Axis = this;
                        _asb.AxisRangeChanged += new AxisRangeChangedEventHandler(ScrollBar_AxisRangeChanged);
                    }
                }
            }
        }

        DispatcherTimer Timer
        {
            get
            {
                if (_timer == null)
                {
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(100.0);
                    _timer = timer;
                    DispatcherTimer timer2 = _timer;
                    timer2.Tick += _timer_Tick;
                }
                return _timer;
            }
        }

        public bool UseExactLimits
        {
            get { return (bool)((bool)base.GetValue(UseExactLimitsProperty)); }
            set { base.SetValue(UseExactLimitsProperty, (bool)value); }
        }

        public double Value
        {
            get { return (double)((double)base.GetValue(ValueProperty)); }
            set { base.SetValue(ValueProperty, (double)value); }
        }

        public bool Visible
        {
            get { return (bool)((bool)base.GetValue(VisibleProperty)); }
            set { base.SetValue(VisibleProperty, (bool)value); }
        }

        internal void FireAnnoCreated(AnnoCreatedEventArgs args)
        {
            if (AnnoCreated != null)
            {
                AnnoCreated(this, args);
            }
        }

        internal string Format(object val)
        {
            return iax.Format(val);
        }

        public Rect GetAxisRect()
        {
            return ((AxisCanvas)iax).AxisRect;
        }

        internal bool IsTimeInternal(Chart chart)
        {
            if (_isTime.HasValue)
            {
                return _isTime.Value;
            }
            if (base.ReadLocalValue(IsTimeProperty) != DependencyProperty.UnsetValue)
            {
                return IsTime;
            }
            if (((AxisType != Dt.Charts.AxisType.X) || (chart == null)) || (((chart.ChartType != ChartType.Candle) && (chart.ChartType != ChartType.HighLowOpenClose)) && (chart.ChartType != ChartType.Gantt)))
            {
                return false;
            }
            return true;
        }

        static void OnAnnoFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Axis axis = (Axis)obj;
            axis.IsValidFmt = TestFormat(axis.AnnoFormat, false);
            axis.IsValidTimeFmt = TestFormat(axis.AnnoFormat, true);
            axis.OnPropertyChanged(args.Property.ToString());
        }

        static void OnAutoMaxChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Axis axis = (Axis)obj;
            if ((bool)args.NewValue)
            {
                if (axis.notify)
                {
                    axis.notify = false;
                    axis.Max = double.NaN;
                    axis.notify = true;
                }
                else
                {
                    axis.Max = double.NaN;
                }
            }
            axis.OnPropertyChanged("AutoMax");
        }

        static void OnAutoMinChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Axis axis = (Axis)obj;
            if ((bool)args.NewValue)
            {
                if (axis.notify)
                {
                    axis.notify = false;
                    axis.Min = double.NaN;
                    axis.notify = true;
                }
                else
                {
                    axis.Min = double.NaN;
                }
            }
            axis.OnPropertyChanged("AutoMin");
        }

        static void OnDPChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((Axis)obj).OnPropertyChanged(args.Property.ToString());
        }

        static void OnIsTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)obj;
            axis._isTime = new bool?((bool)((bool)e.NewValue));
            axis.IsValidFmt = TestFormat(axis.AnnoFormat, false);
            axis.IsValidTimeFmt = TestFormat(axis.AnnoFormat, true);
            axis.OnPropertyChanged("IsTime");
        }

        static void OnLogBaseChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Axis axis = (Axis)obj;
            if (((axis.LogBase <= 0.0) || (axis.LogBase > 3.4028234663852886E+38)) || (axis.LogBase == 1.0))
            {
                axis.LogBase = double.NaN;
            }
            else
            {
                axis.OnPropertyChanged("LogBase");
            }
        }

        static void OnMaxChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)obj;
            if (axis.notify)
            {
                axis.notify = false;
                axis.AutoMax = double.IsNaN(axis.Max);
                axis.notify = true;
            }
            else
            {
                axis.AutoMax = double.IsNaN(axis.Max);
            }
            axis.OnPropertyChanged("Max");
        }

        static void OnMinChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)obj;
            if (axis.notify)
            {
                axis.notify = false;
                axis.AutoMin = double.IsNaN(axis.Min);
                axis.notify = true;
            }
            else
            {
                axis.AutoMin = double.IsNaN(axis.Min);
            }
            axis.OnPropertyChanged("Min");
        }

        void OnPropertyChanged(string name)
        {
            if (notify && (PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        static void OnRadarLabelVisibilityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((Axis)obj).OnPropertyChanged("RadarLabelVisibility");
        }

        static void OnRadarPointIndicesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((Axis)obj).OnPropertyChanged("RadarPointIndices");
        }

        static void OnScaleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Axis axis = (Axis)obj;
            if (axis.Scale < axis.MinScale)
            {
                axis.Scale = axis.MinScale;
            }
            else if (axis.Scale > 1.0)
            {
                axis.Scale = 1.0;
            }
            else
            {
                axis.OnPropertyChanged("Scale");
            }
        }

        static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Axis axis = (Axis)obj;
            if (axis.Value > 1.0)
            {
                axis.Value = 1.0;
            }
            else if (axis.Value < 0.0)
            {
                axis.Value = 0.0;
            }
            else if (double.IsNaN(axis.Value))
            {
                axis.Value = 0.0;
            }
            axis.OnPropertyChanged("Value");
        }

        internal PointD PointFromData(PointD pt)
        {
            if ((base.Chart == null) || (base.Chart.View == null))
            {
                return new PointD(double.NaN, double.NaN);
            }
            PointD td = base.Chart.View.PointFromData(pt);
            if (iax != null)
            {
                if (AxisType == Dt.Charts.AxisType.X)
                {
                    td.X = iax.FromData(pt.X);
                    return td;
                }
                if (AxisType == Dt.Charts.AxisType.Y)
                {
                    td.Y = iax.FromData(pt.Y);
                }
            }
            return td;
        }

        public Point PointFromData(Point pt)
        {
            if ((base.Chart == null) || (base.Chart.View == null))
            {
                return new Point(double.NaN, double.NaN);
            }
            Point point = base.Chart.View.PointFromData(pt);
            if (iax != null)
            {
                if (AxisType == Dt.Charts.AxisType.X)
                {
                    point.X = iax.FromData(pt.X);
                    return point;
                }
                if (AxisType == Dt.Charts.AxisType.Y)
                {
                    point.Y = iax.FromData(pt.Y);
                }
            }
            return point;
        }

        internal PointD PointToData(PointD pt)
        {
            if ((base.Chart == null) || (base.Chart.View == null))
            {
                return new PointD(double.NaN, double.NaN);
            }
            PointD td = base.Chart.View.PointToData(pt);
            if (iax != null)
            {
                if (AxisType == Dt.Charts.AxisType.X)
                {
                    td.X = iax.ToData(pt.X);
                    return td;
                }
                if (AxisType == Dt.Charts.AxisType.Y)
                {
                    td.Y = iax.ToData(pt.Y);
                }
            }
            return td;
        }

        public Point PointToData(Point pt)
        {
            if ((base.Chart == null) || (base.Chart.View == null))
            {
                return new Point(double.NaN, double.NaN);
            }
            Point point = base.Chart.View.PointToData(pt);
            if (iax != null)
            {
                if (AxisType == Dt.Charts.AxisType.X)
                {
                    point.X = iax.ToData(pt.X);
                    return point;
                }
                if (AxisType == Dt.Charts.AxisType.Y)
                {
                    point.Y = iax.ToData(pt.Y);
                }
            }
            return point;
        }

        internal void Reset()
        {
            AnnoAngle = 0.0;
            AnnoFormat = "";
            AnnoPosition = Dt.Charts.AnnoPosition.Auto;
            AnnoTemplate = null;
            ItemsSource = null;
            MajorUnit = double.NaN;
            MajorTickThickness = 1.0;
            MajorTickHeight = 2.0;
            MajorTickOverlap = 0.0;
            if (_fixedType)
            {
                SetFixedType(AxisType);
            }
            else
            {
                MajorGridStroke = null;
                MajorGridStrokeDashes = null;
            }
            AutoMin = true;
            AutoMax = true;
            Min = double.NaN;
            Max = double.NaN;
            MinorGridStroke = null;
            MinorGridStrokeDashes = null;
            MinorGridStrokeThickness = 0.0;
            MinorTickThickness = 1.0;
            MinorTickHeight = 1.0;
            MinorTickOverlap = 0.0;
            MinorUnit = double.NaN;
            MinScale = 1E-05;
            Position = AxisPosition.Near;
            Reversed = false;
            Scale = 1.0;
            Value = 0.0;
            Origin = double.NaN;
            LogBase = double.NaN;
            Title = null;
            base.ClearValue(IsTimeProperty);
            _isTime = null;
            MajorGridStrokeThickness = 0.5;
            MinorGridStrokeThickness = 0.25;
        }

        void ScrollBar_AxisRangeChanged(object sender, AxisRangeChangedEventArgs e)
        {
            _tmpValue = e.Value;
            _tmpScale = e.Scale;
            if (((_tmpValue != Value) || (_tmpScale != Scale)) && !Timer.IsEnabled)
            {
                if (base.Chart != null)
                {
                    Timer.Interval = TimeSpan.FromMilliseconds(base.Chart.ActionUpdateDelay);
                }
                Timer.Start();
            }
        }

        internal void SetAxisLine(Line line)
        {
            if (notify)
            {
                notify = false;
                AxisLine = line;
                notify = true;
            }
            else
            {
                AxisLine = line;
            }
        }

        internal void SetFixedType(Dt.Charts.AxisType type)
        {
            _axType = type;
            _fixedType = true;
            if (MajorGridStrokeThickness == 0.0)
            {
                MajorGridStrokeThickness = 0.5;
            }
            if (MajorGridStroke == null)
            {
                MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
            }
        }

        static bool TestFormat(string fmt, bool time)
        {
            try
            {
                if (!string.IsNullOrEmpty(fmt))
                {
                    if (time)
                    {
                        DateTime.Now.ToString(fmt, (IFormatProvider)CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        double num = 0.0;
                        ((double)num).ToString(fmt, (IFormatProvider)CultureInfo.CurrentCulture);
                    }
                }
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

