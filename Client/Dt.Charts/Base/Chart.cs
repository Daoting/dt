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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    [ContentProperty(Name = "Children")]
    public partial class Chart : ContentControl, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AggregateProperty = DependencyProperty.Register(
            "Aggregate",
            (Type)typeof(Aggregate), 
            (Type)typeof(Chart),
            new PropertyMetadata(Aggregate.None, new PropertyChangedCallback(Chart.OnAggregateChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ChartTypeProperty = Utils.RegisterProperty(
            "ChartType",
            typeof(ChartType), 
            typeof(Chart),
            new PropertyChangedCallback(Chart.OnChartTypeChanged), ChartType.Column);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ClipToBoundsProperty = DependencyProperty.Register(
            "ClipToBounds", 
            (Type)typeof(bool), 
            (Type)typeof(Chart),
            new PropertyMetadata((bool)false, new PropertyChangedCallback(Chart.OnClipToBoundsChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            (Type)typeof(Windows.UI.Xaml.CornerRadius), 
            (Type)typeof(Chart),
            new PropertyMetadata(new Windows.UI.Xaml.CornerRadius(), new PropertyChangedCallback(Chart.OnCornerRadiusChanged)));
       
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CustomPaletteProperty = DependencyProperty.Register(
            "CustomPalette", 
            (Type)typeof(IEnumerable), 
            (Type)typeof(Chart),
            new PropertyMetadata(null, new PropertyChangedCallback(Chart.OnCustomPaletteChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty CustomThemeProperty = DependencyProperty.Register(
            "CustomTheme",
            (Type)typeof(ResourceDictionary), 
            (Type)typeof(Chart), 
            new PropertyMetadata(null, new PropertyChangedCallback(Chart.OnCustomThemeChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DataProperty = Utils.RegisterProperty(
            "Data",
            typeof(ChartData),
            typeof(Chart),
            new PropertyChangedCallback(Chart.OnDataChanged));

        /// <summary>
        /// 
        /// </summary>
        private static readonly DependencyProperty ForegroundInternalProperty = DependencyProperty.Register(
            "ForegroundInternal", 
            (Type)typeof(Brush),
            (Type)typeof(Chart), 
            new PropertyMetadata(null, new PropertyChangedCallback(Chart.OnForegroundInternalChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LegendItemsProperty = DependencyProperty.Register(
            "LegendItems", 
            (Type)typeof(LegendItemCollection),
            (Type)typeof(Chart), 
            new PropertyMetadata(null, new PropertyChangedCallback(Chart.OnLegendItemsChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty ThemeProperty = DependencyProperty.Register(
            "Theme", 
            typeof(ChartTheme), 
            typeof(Chart),
            new PropertyMetadata(ChartTheme.None, new PropertyChangedCallback(Chart.OnThemeChanged)));

        /// <summary>
        /// 标题
        /// </summary>
        public readonly static DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(Chart),
            new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ViewProperty = Utils.RegisterProperty(
            "View", 
            typeof(ChartView), 
            typeof(Chart), 
            new PropertyChangedCallback(Chart.OnViewChanged));

        private ActionCollection _actions;
        private bool _autoSeries;
        private ChartBindings _bindings;
        private ObservableCollection<UIElement> _children;
        private UIElement _content;
        private ResourceDictionary _customTheme = new ResourceDictionary();
        private bool _dirtyChildren = true;
        private GradientMethod _gradientMethod;
        private bool _inBuild;
        private List<INotifyPropertyChanged> _inps = new List<INotifyPropertyChanged>();
        private ChartLegend _legend;
        private LegendItems _litems;
        private LegendItemCollection _litemsRO;
        private bool _loaded = true;
        private Renderers _renderers;
        private StyleGenerator _stgen;
        private bool _templated;
        private static ThemeConverter _themeConverter = new ThemeConverter();
        private int _updateCount;
        private FrameworkElement _viewElement;
        private bool dataChanged = true;
        internal bool forceRebuild = true;
        private Point pinchCenter = new Point();
        internal bool rebuilding = false;
        private Size sz = new Size();
        
        public Chart()
        {
            base.DefaultStyleKey = typeof(Chart);

            BubbleOptions.SetMinSize(this, Size.Empty);
            BubbleOptions.SetMaxSize(this, Size.Empty);
            Data = new ChartData();
            View = new ChartView();
            
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Foreground");
            SetBinding(ForegroundInternalProperty, binding);

            _litems = new LegendItems();
            _litemsRO = new LegendItemCollection(_litems);
            LegendItems = _litemsRO;

            LayoutUpdated += C1Chart_LayoutUpdated;
            PointerPressed += C1Chart_PointerPressed;
            PointerReleased += C1Chart_PointerReleased;
            PointerMoved += C1Chart_PointerMoved;
            ManipulationStarted += C1Chart_ManipulationStarted;
            ManipulationDelta += C1Chart_ManipulationDelta;
            ManipulationCompleted += C1Chart_ManipulationCompleted;
            Loaded += C1Chart_Loaded;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler ActionEnter;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler ActionLeave;

        /// <summary>
        /// 
        /// </summary>
        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public event EventHandler LegendChanged;

        /// <summary>
        /// 
        /// </summary>
        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取设置图表类型
        /// </summary>
        public ChartType ChartType
        {
            get { return (ChartType)base.GetValue(ChartTypeProperty); }
            set { base.SetValue(ChartTypeProperty, value); }
        }

        /// <summary>
        /// 获取设置图表数据
        /// </summary>
        public ChartData Data
        {
            get { return (ChartData)base.GetValue(DataProperty); }
            set { base.SetValue(DataProperty, value); }
        }

        /// <summary>
        /// 获取设置图表主题样式
        /// </summary>
        public ChartTheme Theme
        {
            get { return (ChartTheme)base.GetValue(ThemeProperty); }
            set { base.SetValue(ThemeProperty, value); }
        }

        /// <summary>
        /// 获取设置图表标题内容
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// 获取设置图表调色板
        /// </summary>
        [DefaultValue(0)]
        public Palette Palette
        {
            get { return StyleGenerator.Palette; }
            set
            {
                StyleGenerator.Palette = value;
                forceRebuild = true;
                InvalidateChart();
            }
        }

        internal ActionCollection Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new ActionCollection(this);
                    _actions.CollectionChanged += _actions_CollectionChanged;
                }
                return _actions;
            }
        }

        [DefaultValue(40)]
        public double ActionUpdateDelay
        {
            get { return Actions.UpdateDelay; }
            set { Actions.UpdateDelay = value; }
        }

        public Aggregate Aggregate
        {
            get { return (Aggregate)base.GetValue(AggregateProperty); }
            set { base.SetValue(AggregateProperty, value); }
        }

        public bool AutoGenerateSeries
        {
            get { return _autoSeries; }
            set
            {
                if (_autoSeries != value)
                {
                    _autoSeries = value;
                    dataChanged = true;
                    InvalidateChart();
                }
            }
        }

        public ChartBindings Bindings
        {
            get { return _bindings; }
            set
            {
                if (_bindings != value)
                {
                    _bindings = value;
                    dataChanged = true;
                    InvalidateChart();
                }
            }
        }

        public ObservableCollection<UIElement> Children
        {
            get
            {
                if (_children == null)
                {
                    ChartObservableCollection observables = new ChartObservableCollection();
                    observables.OnClear += new EventHandler(_children_Clear);
                    _children = observables;
                    _children.CollectionChanged += _children_CollectionChanged;
                }
                return _children;
            }
        }

        public bool ClipToBounds
        {
            get { return (bool)((bool)base.GetValue(ClipToBoundsProperty)); }
            set { base.SetValue(ClipToBoundsProperty, (bool)value); }
        }

        public Windows.UI.Xaml.CornerRadius CornerRadius
        {
            get { return (Windows.UI.Xaml.CornerRadius)base.GetValue(CornerRadiusProperty); }
            set { base.SetValue(CornerRadiusProperty, value); }
        }

        public IEnumerable CustomPalette
        {
            get { return (IEnumerable)base.GetValue(CustomPaletteProperty); }
            set { base.SetValue(CustomPaletteProperty, value); }
        }

        public ResourceDictionary CustomTheme
        {
            get { return (ResourceDictionary)base.GetValue(CustomThemeProperty); }
            set { base.SetValue(CustomThemeProperty, value); }
        }

        public GestureDoubleTapAction GestureDoubleTap { get; set; }

        public GesturePinchAction GesturePinch { get; set; }

        public GestureSlideAction GestureSlide { get; set; }

        public GradientMethod GradientMethod
        {
            get { return _gradientMethod; }
            set
            {
                if (_gradientMethod != value)
                {
                    _gradientMethod = value;
                    InvalidateChart();
                }
            }
        }

        public LegendItemCollection LegendItems
        {
            get { return (LegendItemCollection)base.GetValue(LegendItemsProperty); }
            private set { base.SetValue(LegendItemsProperty, value); }
        }

        internal LegendItems LegendItemsInternal
        {
            get { return _litems; }
        }

        internal Renderers Renderers
        {
            get
            {
                if (_renderers == null)
                {
                    _renderers = new Renderers();
                }
                return _renderers;
            }
        }

        private StyleGenerator StyleGenerator
        {
            get
            {
                if (_stgen == null)
                {
                    _stgen = new StyleGenerator();
                    _stgen.CustomBrushConverter = new Dt.Charts.Converter<Brush, Brush>(BrushConverter);
                }
                return _stgen;
            }
        }

        internal int UpdateCount
        {
            get { return _updateCount; }
            set
            {
                if (value <= 0)
                {
                    _updateCount = 0;
                    InvalidateChart();
                }
                else
                {
                    _updateCount = value;
                }
            }
        }

        public ChartView View
        {
            get { return (ChartView)base.GetValue(ViewProperty); }
            set { base.SetValue(ViewProperty, value); }
        }

        internal FrameworkElement ViewElement
        {
            get { return _viewElement; }
            set
            {
                try
                {
                    if (_viewElement != value)
                    {
                        _viewElement = value;
                        base.Content = _viewElement;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void _actions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        private void _children_Clear(object sender, EventArgs e)
        {
            RemoveChildren((IList)Children);
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != ((NotifyCollectionChangedAction)((int)NotifyCollectionChangedAction.Reset)))
            {
                RemoveChildren(e.OldItems);
            }
            UpdateChildren();
        }

        private void _data_DataChanged(object sender, EventArgs e)
        {
            if (!rebuilding)
            {
                if ((Data != null) && (Data.Children.Count == 0))
                {
                    StyleGenerator.Reset();
                }
                forceRebuild = true;
                dataChanged = true;
                InvalidateChart();
            }
        }

        private void AddINP(INotifyPropertyChanged inp)
        {
            inp.PropertyChanged += inp_PropertyChanged;
            _inps.Add(inp);
        }

        internal void AddLogicalChild(object child)
        {
        }

        internal void ApplyChartType(ChartType chartType)
        {
            ApplyChartType(chartType.ToString());
        }

        private void ApplyChartType(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                ChartSubtype subtype = ChartTypes.GetSubtype(type);
                if (subtype != null)
                {
                    subtype.Apply(this);
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_content != null)
            {
                if (ClipToBounds)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
                    _content.Clip = geometry;
                }
                else
                {
                    _content.Clip = null;
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        public void BeginUpdate()
        {
            UpdateCount++;
        }

        private Brush BrushConverter(Brush brush)
        {
            if (GradientMethod == GradientMethod.None)
            {
                return brush;
            }
            return brush;
        }

        private void C1Chart_LayoutUpdated(object sender, object e)
        {
            LayoutUpdatedInternal();
        }

        private void C1Chart_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
            if (!_templated)
            {
                base.ApplyTemplate();
            }
            UpdateChildren();
            RebuildChart();
            if (ViewElement != null)
            {
                ViewElement.Width = base.Width;
                ViewElement.Height = base.Height;
            }
        }

        private void C1Chart_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if ((GesturePinch == GesturePinchAction.Scale) && (Data.Renderer is Renderer2D))
            {
                if (Actions.State == ActionType.Pinch)
                {
                    Actions.State = ActionType.None;
                    e.Handled = true;
                    Actions.FireLeave();
                }
            }
            else if (((GestureSlide == GestureSlideAction.Translate) && (Data.Renderer is Renderer2D)) && (Actions.State == ActionType.Translate))
            {
                Actions.State = ActionType.None;
                e.Handled = true;
                Actions.FireLeave();
            }
        }

        private void C1Chart_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Delta.Scale != 1.0)
            {
                if ((GesturePinch == GesturePinchAction.Scale) && (Data.Renderer is Renderer2D))
                {
                    Actions.FireEnter();
                    Actions.State = ActionType.Pinch;
                    float scale = e.Delta.Scale;
                    Actions.PerformScale(pinchCenter, (double)scale, (double)scale);
                    e.Handled = true;
                }
            }
            else if (((e.Delta.Translation.X != 0.0) || (e.Delta.Translation.Y != 0.0)) && ((GestureSlide == GestureSlideAction.Translate) && (Data.Renderer is Renderer2D)))
            {
                Actions.FireEnter();
                Actions.State = ActionType.Translate;
                Actions.PerformTranslate(new Point(), e.Delta.Translation);
            }
        }

        private void C1Chart_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            pinchCenter = e.Position;
        }

        private void C1Chart_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (GestureSlide == GestureSlideAction.Zoom)
            {
                Actions.OnMouseMove(e);
            }
        }

        private void C1Chart_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (GestureSlide == GestureSlideAction.Zoom)
            {
                Actions.OnMouseDown(e);
            }
        }

        private void C1Chart_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (GestureSlide == GestureSlideAction.Zoom)
            {
                Actions.OnMouseUp(e);
            }
        }

        private void ClearAllINP()
        {
            int num = _inps.Count;
            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    _inps[i].PropertyChanged -= inp_PropertyChanged;
                }
                _inps.Clear();
            }
        }

        internal void ClearLegendBindings()
        {
            if (_legend != null)
            {
                ClearValue(_legend, Control.BackgroundProperty);
                ClearValue(_legend, Control.ForegroundProperty);
                ClearValue(_legend, Control.BorderBrushProperty);
                ClearValue(_legend, Control.BorderThicknessProperty);
                ClearValue(_legend, ChartLegend.CornerRadiusProperty);
            }
        }

        private void ClearThemeBindings()
        {
            FrameworkElement templateChild = base.GetTemplateChild("view") as FrameworkElement;
            if (templateChild != null)
            {
                ClearValue(templateChild, Border.BackgroundProperty);
                ClearValue(templateChild, Border.BorderBrushProperty);
                ClearValue(templateChild, Border.BorderThicknessProperty);
                ClearValue(templateChild, Border.CornerRadiusProperty);
                ClearValue(templateChild, Border.PaddingProperty);
            }
            ClearValue(View, ChartView.PlotBackgroundProperty);
            ClearValue(this, Control.BackgroundProperty);
            ClearValue(this, Control.ForegroundProperty);
            ClearValue(this, Control.BorderThicknessProperty);
            ClearValue(this, Control.BorderBrushProperty);
            ClearValue(this, FrameworkElement.MarginProperty);
            ClearValue(this, Control.PaddingProperty);
            ClearValue(this, CornerRadiusProperty);
            ClearValue(this, CustomPaletteProperty);
            ClearLegendBindings();
        }

        private static void ClearValue(FrameworkElement fe, DependencyProperty dp)
        {
            if (fe != null)
            {
                object obj2 = fe.ReadLocalValue(dp);
                if (dp == CustomPaletteProperty)
                {
                    fe.ClearValue(dp);
                }
                if (obj2 is BindingBase)
                {
                    fe.ClearValue(dp);
                }
            }
        }

        private void ContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled)
            {
                InvalidateChart();
            }
        }

        private void EffectChanged(object sender, EventArgs e)
        {
            InvalidateChart();
        }

        public void EndUpdate()
        {
            UpdateCount--;
        }

        public object FindPlotElement(string name)
        {
            return ViewElement.FindName(name);
        }

        internal void FireActionEnter(object sender, EventArgs args)
        {
            if (ActionEnter != null)
            {
                ActionEnter(sender, args);
            }
        }

        internal void FireActionLeave(object sender, EventArgs args)
        {
            if (ActionLeave != null)
            {
                ActionLeave(sender, args);
            }
        }

        internal void InitLegendBindings(ChartLegend legend)
        {
            _legend = legend;
            if (legend != null)
            {
                InitThemeBinding(legend, Control.BackgroundProperty, "C1Chart_LegendBackground_Brush");
                InitThemeBinding(legend, Control.ForegroundProperty, "C1Chart_LegendForeground_Brush");
                InitThemeBinding(legend, Control.BorderBrushProperty, "C1Chart_LegendBorder_Brush");
                InitThemeBinding(legend, Control.BorderThicknessProperty, "C1Chart_LegendBorder_Thickness");
                InitThemeBinding(legend, ChartLegend.CornerRadiusProperty, "C1Chart_Legend_CornerRadius");
            }
        }

        private void InitThemeBinding(FrameworkElement fe, DependencyProperty property, string key)
        {
            if (((CustomTheme != null) && (fe != null)) && ((CustomTheme == null) || CustomTheme.ContainsKey(key)))
            {
                if (property == CustomPaletteProperty)
                {
                    fe.SetValue(property, CustomTheme[key]);
                }
                if (fe.ReadLocalValue(property) == DependencyProperty.UnsetValue)
                {
                    Binding binding2 = new Binding();
                    binding2.Source = this;
                    binding2.Path = new PropertyPath("CustomTheme");
                    Binding binding = binding2;
                    binding.Mode = BindingMode.OneWay;
                    binding.Converter = _themeConverter;
                    binding.ConverterParameter = new object[] { key, fe };
                    fe.SetBinding(property, binding);
                }
            }
        }

        private void InitThemeBindings()
        {
            FrameworkElement templateChild = base.GetTemplateChild("view") as FrameworkElement;
            if (templateChild != null)
            {
                InitThemeBinding(templateChild, Border.BackgroundProperty, "C1Chart_ChartAreaBackground_Brush");
                InitThemeBinding(templateChild, Border.BorderBrushProperty, "C1Chart_ChartAreaBorder_Brush");
                InitThemeBinding(templateChild, Border.BorderThicknessProperty, "C1Chart_ChartAreaBorder_Thickness");
                InitThemeBinding(templateChild, Border.CornerRadiusProperty, "C1Chart_ChartArea_CornerRadius");
                InitThemeBinding(templateChild, Border.PaddingProperty, "C1Chart_ChartArea_Padding");
            }
            InitThemeBinding(View, ChartView.PlotBackgroundProperty, "C1Chart_PlotAreaBackground_Brush");
            InitThemeBinding(this, Control.BackgroundProperty, "C1Chart_Background_Brush");
            InitThemeBinding(this, Control.ForegroundProperty, "C1Chart_Foreground_Brush");
            InitThemeBinding(this, Control.BorderThicknessProperty, "C1Chart_Border_Thickness");
            InitThemeBinding(this, Control.BorderBrushProperty, "C1Chart_Border_Brush");
            InitThemeBinding(this, FrameworkElement.MarginProperty, "C1Chart_Margin");
            InitThemeBinding(this, Control.PaddingProperty, "C1Chart_Padding");
            InitThemeBinding(this, CornerRadiusProperty, "C1Chart_CornerRadius");
            InitThemeBinding(this, CustomPaletteProperty, "C1Chart_CustomPalette");
            InitLegendBindings(_legend);
        }

        private void inp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _data_DataChanged(this, EventArgs.Empty);
        }

        internal void InvalidateChart()
        {
            if ((UpdateCount <= 0) && (((UpdateCount <= 0) && _loaded) && _templated))
            {
                forceRebuild = true;
                RebuildChart();
            }
        }

        internal void LayoutUpdatedInternal()
        {
            if (((base.ActualWidth != 0.0) && (base.ActualHeight != 0.0)) && _loaded)
            {
                if (!_templated)
                {
                    base.ApplyTemplate();
                }
                if (_dirtyChildren)
                {
                    UpdateChildren();
                }
                Size size = new Size(base.ActualWidth, base.ActualHeight);
                if (size != sz)
                {
                    sz = size;
                    forceRebuild = true;
                    RebuildChart();
                }
                else if ((ViewElement != null) && (ViewElement.Parent is FrameworkElement))
                {
                    FrameworkElement templateChild = base.GetTemplateChild("viewContent") as FrameworkElement;
                    if ((templateChild != null) && ((templateChild.ActualHeight != ViewElement.Height) || (templateChild.ActualWidth != ViewElement.Width)))
                    {
                        Data.Renderer.Dirty = true;
                        forceRebuild = true;
                        RebuildChart();
                    }
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (View != null)
            {
                ViewElement = View.ViewElement;
            }
            return base.MeasureOverride(availableSize);
        }

        private static void OnAggregateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = (Chart)obj;
            if (chart.Data != null)
            {
                using (IEnumerator<DataSeries> enumerator = chart.Data.Children.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Dirty = true;
                    }
                }
            }
            chart.dataChanged = true;
            chart.forceRebuild = true;
            chart.InvalidateChart();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (CustomTheme != null)
            {
                InitThemeBindings();
            }
            _templated = true;
            _dirtyChildren = true;
            FrameworkElement element = _content as FrameworkElement;
            if (element != null)
            {
                element.SizeChanged -= ContentSizeChanged;
            }
            _content = base.GetTemplateChild("content") as UIElement;
            element = _content as FrameworkElement;
            if (element != null)
            {
                element.SizeChanged += ContentSizeChanged;
            }
        }

        internal static void OnAttachedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = obj as Chart;
            if (chart != null)
            {
                if (args.NewValue != args.OldValue)
                {
                    chart.forceRebuild = true;
                    chart.InvalidateChart();
                }
            }
            else
            {
                DataSeries series = obj as DataSeries;
                if (series != null)
                {
                    series.FirePropertyChanged(args.Property.ToString());
                }
            }
        }

        private static void OnChartTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = (Chart)obj;
            ChartType newValue = (ChartType)args.NewValue;
            if (chart._loaded && chart._templated)
            {
                chart.ApplyChartType(newValue);
            }
        }

        private static void OnClipToBoundsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((Chart)obj).InvalidateArrange();
        }

        private static void OnCornerRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        private static void OnCustomPaletteChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = (Chart)obj;
            chart.StyleGenerator.Reset();
            object newValue = args.NewValue;
            IList<Windows.UI.Color> list = newValue as IList<Windows.UI.Color>;
            if (list != null)
            {
                Brush[] brushArray = new Brush[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    brushArray[i] = new SolidColorBrush(list[i]);
                }
                newValue = brushArray;
            }
            Brush[] brushArray2 = newValue as Brush[];
            if ((brushArray2 != null) && (brushArray2.Length > 0))
            {
                chart.StyleGenerator.CustomBrushes = brushArray2;
            }
            else if (brushArray2 != null)
            {
                IList<Brush> list2 = (IList<Brush>)brushArray2;
                int num2 = list2.Count;
                if (num2 > 0)
                {
                    Brush[] brushArray3 = new Brush[num2];
                    for (int j = 0; j < num2; j++)
                    {
                        brushArray3[j] = list2[j];
                    }
                    chart.StyleGenerator.CustomBrushes = brushArray3;
                }
            }
            else
            {
                ResourceDictionary dictionary = newValue as ResourceDictionary;
                if (dictionary != null)
                {
                    List<Brush> list3 = new List<Brush>();
                    foreach (object obj3 in Enumerable.OrderBy<object, object>((IEnumerable<object>)dictionary.Keys, delegate(object key)
                    {
                        return key;
                    }))
                    {
                        if (dictionary[obj3] is Brush)
                        {
                            list3.Add((Brush)dictionary[obj3]);
                        }
                    }
                    chart.StyleGenerator.CustomBrushes = (list3.Count > 0) ? list3.ToArray() : null;
                }
                else
                {
                    IEnumerable enumerable2 = newValue as IEnumerable;
                    if (enumerable2 != null)
                    {
                        IEnumerator enumerator = enumerable2.GetEnumerator();
                        List<Brush> list4 = new List<Brush>();
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            if (current is Brush)
                            {
                                list4.Add((Brush)current);
                            }
                            else if (current is Windows.UI.Color)
                            {
                                list4.Add(new SolidColorBrush((Windows.UI.Color)current));
                            }
                        }
                        chart.StyleGenerator.CustomBrushes = (list4.Count > 0) ? list4.ToArray() : null;
                    }
                    else
                    {
                        chart.StyleGenerator.CustomBrushes = null;
                    }
                }
            }
            chart.InvalidateChart();
        }

        private static void OnCustomThemeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = (Chart)obj;
            chart.BeginUpdate();
            if (chart.PropertyChanged != null)
            {
                chart.PropertyChanged(chart, new PropertyChangedEventArgs("CustomTheme"));
            }
            if ((chart.Data != null) && (chart.Data.Renderer != null))
            {
                chart.Data.Renderer.Dirty = true;
            }
            chart.EndUpdate();
            ResourceDictionary newValue = args.NewValue as ResourceDictionary;
            if (!Themes.IsStandard(newValue))
            {
                chart.Theme = ChartTheme.Custom;
            }
        }

        private static void OnDataChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = (Chart)obj;
            chart.ClearAllINP();
            ChartData oldValue = (ChartData)args.OldValue;
            if (oldValue != null)
            {
                if (((oldValue.Renderer == chart.Renderers.Pie) || (oldValue.Renderer == chart.Renderers.Radar)) || (oldValue.Renderer == chart.Renderers.Renderer2D))
                {
                    oldValue.Renderer = null;
                }
                oldValue.DataChanged -= new EventHandler(chart._data_DataChanged);
                chart.RemoveLogicalChild(oldValue);
            }
            oldValue = (ChartData)args.NewValue;
            if (oldValue != null)
            {
                oldValue.DataChanged += new EventHandler(chart._data_DataChanged);
            }
            chart.StyleGenerator.Reset();
            if (oldValue != null)
            {
                oldValue.Renderer = null;
                chart.AddLogicalChild(oldValue);
            }
            chart.InvalidateChart();
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            base.OnDoubleTapped(e);
            if ((GestureDoubleTap == GestureDoubleTapAction.Scale) && (Data.Renderer is Renderer2D))
            {
                Actions.FireEnter();
                Actions.PerformScale(e.GetPosition(this));
                Actions.FireLeave();
            }
        }

        private static void OnForegroundInternalChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((Chart)obj).InvalidateChart();
        }

        private static void OnLegendItemsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = (Chart)obj;
            if (args.NewValue != chart._litemsRO)
            {
                chart.LegendItems = chart._litemsRO;
            }
        }

        private static void OnThemeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = (Chart)obj;
            ChartTheme oldValue = (ChartTheme)args.OldValue;
            ChartTheme newValue = (ChartTheme)args.NewValue;
            if (newValue != ChartTheme.Custom)
            {
                chart.CustomTheme = Themes.GetThemes((ChartTheme)args.NewValue);
            }
            chart.ClearThemeBindings();
            if (newValue != ChartTheme.None)
            {
                chart.InitThemeBindings();
            }
            chart.InvalidateChart();
            if (DesignMode.DesignModeEnabled)
            {
                chart.UpdateLayout();
                chart.LayoutUpdatedInternal();
            }
        }

        private static void OnViewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Chart chart = (Chart)obj;
            ChartView oldValue = (ChartView)args.OldValue;
            if (oldValue != null)
            {
                chart.RemoveLogicalChild(oldValue);
                oldValue.Chart = null;
            }
            oldValue = (ChartView)args.NewValue;
            if (oldValue != null)
            {
                oldValue.Chart = chart;
            }
            chart.InvalidateChart();
        }

        private void RebuildChart()
        {
            if (!_inBuild)
            {
                try
                {
                    BaseRenderer renderer;
                    _inBuild = true;
                    if (Data != null)
                    {
                        IRenderer renderer2 = Data.Renderer;
                        if (renderer2 == null)
                        {
                            ApplyChartType(ChartType);
                            renderer2 = Data.Renderer;
                            if (renderer2 == null)
                            {
                                Data.Renderer = renderer2 = new Renderer2D();
                            }
                        }
                        renderer = renderer2 as BaseRenderer;
                        if (renderer != null)
                        {
                            renderer.StyleGen = StyleGenerator;
                        }
                        renderer2.Visual = this;
                        if (renderer2.Dirty)
                        {
                            forceRebuild = true;
                        }
                    }
                    if (forceRebuild)
                    {
                        forceRebuild = false;
                        if (Data != null)
                        {
                            IRenderer renderer3 = Data.Renderer;
                            if (renderer3 != null)
                            {
                                renderer = renderer3 as BaseRenderer;
                                if (renderer != null)
                                {
                                    renderer.ChartType = ChartType;
                                    renderer.StyleGen.Reset();
                                }
                                renderer3.Dirty = false;
                                View.Renderer = renderer3;
                                if (dataChanged)
                                {
                                    dataChanged = false;
                                    renderer3.Clear();
                                    RebuildRenderer(renderer3);
                                }
                                if (renderer != null)
                                {
                                    object[] itemNamesInternal = Data.ItemNamesInternal;
                                    if ((Data.Aggregate != Aggregate.None) && (itemNamesInternal != null))
                                    {
                                        renderer.ItemNames = Enumerable.ToArray<object>(Enumerable.Distinct<object>(itemNamesInternal));
                                    }
                                    else
                                    {
                                        renderer.ItemNames = itemNamesInternal;
                                    }
                                    renderer.UpdateLegend((IList<LegendItem>)LegendItemsInternal);
                                }
                            }
                        }
                        if (View != null)
                        {
                            ViewElement = View.ViewElement;
                            FrameworkElement templateChild = base.GetTemplateChild("viewContent") as FrameworkElement;
                            if (templateChild != null)
                            {
                                double actualWidth = templateChild.ActualWidth;
                                double actualHeight = templateChild.ActualHeight;
                                ViewElement.Width = actualWidth;
                                ViewElement.Height = actualHeight;
                                View.Rebuild(actualWidth, actualHeight);
                            }
                            if (LegendChanged != null)
                            {
                                LegendChanged(this, EventArgs.Empty);
                            }
                            ViewElement.InvalidateArrange();
                        }
                    }
                }
                finally
                {
                    _inBuild = false;
                }
            }
        }

        /// <summary>
        /// 绘制图表
        /// </summary>
        /// <param name="renderer"></param>
        private void RebuildRenderer(IRenderer renderer)
        {
            IEnumerable itemsSource = Data.ItemsSource;
            ClearAllINP();
            if (itemsSource == null)
            {
                itemsSource = base.DataContext as IEnumerable;
            }

            if (itemsSource != null)
            {
                DataBindingHelper.AutoCreateSeries(this, itemsSource);
                List<object> list = null;
                Binding itemNameBinding = Data.ItemNameBinding;
                if ((itemNameBinding == null) && (Bindings != null))
                {
                    itemNameBinding = Bindings.ItemNameBinding;
                }
                if ((itemNameBinding != null) && (Data.ItemNames == null))
                {
                    list = new List<object>();
                }

                int num = Data.Children.Count;
                Dictionary<IDataSeriesInfo, Binding[]> dictionary = new Dictionary<IDataSeriesInfo, Binding[]>();
                int num2 = 0;
                for (int j = 0; j < num; j++)
                {
                    IDataSeriesInfo info = Data.Children[j];
                    Binding[] memberPaths = info.MemberPaths;
                    if ((memberPaths != null) && (Data.Children[j].ItemsSource == null))
                    {
                        dictionary.Add(info, memberPaths);
                        num2 += memberPaths.Length;
                    }
                }

                IEnumerator enumerator = itemsSource.GetEnumerator();
                DataUtils.TryReset(enumerator);
                List<object>[] listArray = new List<object>[num2];
                int index = 0;
                while (index < listArray.Length)
                {
                    listArray[index] = new List<object>();
                    index++;
                }

                DataBindingProxy proxy = new DataBindingProxy();
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    index = 0;
                    proxy.DataContext = current;
                    if (list != null)
                    {
                        list.Add(proxy.GetValue(itemNameBinding));
                    }

                    using (Dictionary<IDataSeriesInfo, Binding[]>.Enumerator enumerator2 = dictionary.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            Binding[] bindingArray2 = enumerator2.Current.Value;
                            for (int k = 0; k < bindingArray2.Length; k++)
                            {
                                if (bindingArray2[k] != null)
                                {
                                    listArray[index++].Add(proxy.GetValue(bindingArray2[k]));
                                }
                            }
                        }
                    }

                    proxy.DataContext = null;
                    if (current is INotifyPropertyChanged)
                    {
                        AddINP((INotifyPropertyChanged)current);
                    }
                }

                index = 0;
                foreach (KeyValuePair<IDataSeriesInfo, Binding[]> pair2 in dictionary)
                {
                    Binding[] bindingArray3 = pair2.Value;
                    for (int m = 0; m < bindingArray3.Length; m++)
                    {
                        if (bindingArray3[m] != null)
                        {
                            pair2.Key.SetResolvedValues(m, listArray[index++].ToArray());
                        }
                    }
                }

                if ((list != null) && (list.Count > 0))
                {
                    Data.ItemNamesInternal = list.ToArray();
                }
            }

            Data.Aggregate = Aggregate;
            for (int i = 0; i < Data.Children.Count; i++)
            {
                DataSeries seriesInfo = Data.Children[i];
                if (seriesInfo.ItemsSource != null)
                {
                    seriesInfo.PerformBinding(new Action<INotifyPropertyChanged>(AddINP));
                }
                renderer.AddSeries(seriesInfo);
            }
        }

        private void RemoveChildren(IList items)
        {
            if (items != null)
            {
                Panel templateChild = base.GetTemplateChild("grid") as Panel;
                if (templateChild != null)
                {
                    foreach (UIElement element in items)
                    {
                        if (templateChild.Children.Contains(element))
                        {
                            templateChild.Children.Remove(element);
                        }
                    }
                }
            }
        }

        internal void RemoveLogicalChild(object child)
        {
        }

        public void Reset(bool clearData)
        {
            BeginUpdate();
            _autoSeries = false;
            Actions.Clear();
            ActionUpdateDelay = 40.0;
            if ((Data != null) && clearData)
            {
                Data.Reset();
                Bindings = null;
                StyleGenerator.Reset();
            }
            if (View != null)
            {
                View.ResetInternal();
            }
            BarColumnOptions.Reset(this);
            PieOptions.Reset(this);
            LineAreaOptions.Reset(this);
            PolarRadarOptions.Reset(this);
            EndUpdate();
        }

        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public void ResetData()
        {
            Data = null;
        }

        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public bool ShouldSerializeData()
        {
            return (Data != null);
        }

        void IDisposable.Dispose()
        {
            ClearAllINP();
        }

        private void UpdateChildren()
        {
            Panel templateChild = base.GetTemplateChild("grid") as Panel;
            if (templateChild != null)
            {
                foreach (UIElement element in Children)
                {
                    ChartLegend legend = element as ChartLegend;
                    FrameworkElement element2 = element as FrameworkElement;
                    if (element2 != null)
                    {
                        if (legend != null)
                        {
                            legend.Chart = this;
                        }
                        else
                        {
                            Grid.SetColumn(element2, 1);
                            Grid.SetRow(element2, 1);
                        }
                        try
                        {
                            if (!templateChild.Children.Contains(element))
                            {
                                if (element2.Parent == null)
                                {
                                    templateChild.Children.Add(element);
                                }
                                else
                                {
                                    Panel parent = element2.Parent as Panel;
                                    if (parent != null)
                                    {
                                        parent.Children.Remove(element2);
                                        templateChild.Children.Add(element2);
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                _dirtyChildren = false;
            }
        }

    }
}

