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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    [ContentProperty(Name = "Children")]
    public partial class ChartData : DependencyObject
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemNamesProperty = Utils.RegisterProperty(
            "ItemNames",
            typeof(IEnumerable),
            typeof(ChartData),
            new PropertyChangedCallback(ChartData.OnItemNamesChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = Utils.RegisterProperty(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(ChartData),
            new PropertyChangedCallback(ChartData.OnItemsSourceChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LoadAnimationProperty = DependencyProperty.Register(
            "LoadAnimation", 
            typeof(PlotElementAnimation),
            typeof(ChartData), 
            new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RendererProperty = Utils.RegisterProperty(
            "Renderer",
            typeof(IRenderer), 
            typeof(ChartData), 
            new PropertyChangedCallback(ChartData.OnRendererChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectionActionProperty = DependencyProperty.Register(
            "SelectionAction", 
            typeof(Dt.Charts.SelectionAction), 
            typeof(ChartData), 
            new PropertyMetadata(Dt.Charts.SelectionAction.None, new PropertyChangedCallback(ChartData.OnSelectionActionChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SeriesItemsSourceProperty = DependencyProperty.Register(
            "SeriesItemsSource",
            typeof(IEnumerable), 
            typeof(ChartData),
            new PropertyMetadata(null, new PropertyChangedCallback(ChartData.OnSeriesItemsSourceChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SeriesItemTemplateProperty = DependencyProperty.Register(
            "SeriesItemTemplate",
            typeof(DataTemplate), 
            typeof(ChartData), 
            new PropertyMetadata(null, new PropertyChangedCallback(ChartData.OnSeriesItemTemplateChanged)));

        DataSeriesCollection _children;
        Binding _nameBinding;
        object[] _namesInternal;
        int currentIndex = -1;
        Chart _handlers;
        internal bool notify = true;
        
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler DataChanged;

        /// <summary>
        /// 
        /// </summary>
        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取设置图表数据的数据源
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// 获取设置数据项名称绑定，ItemNames为null且指定ItemsSource时有效
        /// </summary>
        public Binding ItemNameBinding
        {
            get { return _nameBinding; }
            set
            {
                if (_nameBinding != value)
                {
                    _namesInternal = null;
                    _nameBinding = value;
                    FireDataChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 获取设置数据项名称
        /// </summary>
        public IEnumerable ItemNames
        {
            get { return (IEnumerable)GetValue(ItemNamesProperty); }
            set { SetValue(ItemNamesProperty, value); }
        }

        public Dt.Charts.SelectionAction SelectionAction
        {
            get { return (Dt.Charts.SelectionAction)GetValue(SelectionActionProperty); }
            set { SetValue(SelectionActionProperty, value); }
        }

        public IEnumerable SeriesItemsSource
        {
            get { return (IEnumerable)GetValue(SeriesItemsSourceProperty); }
            set { SetValue(SeriesItemsSourceProperty, value); }
        }

        public DataTemplate SeriesItemTemplate
        {
            get { return (DataTemplate)GetValue(SeriesItemTemplateProperty); }
            set { SetValue(SeriesItemTemplateProperty, value); }
        }

        public PlotElementAnimation LoadAnimation
        {
            get { return (PlotElementAnimation)GetValue(LoadAnimationProperty); }
            set { SetValue(LoadAnimationProperty, value); }
        }

        public DataSeriesCollection Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new DataSeriesCollection(this);
                    _children.Changed += new EventHandler(_children_Changed);
                }
                return _children;
            }
        }

        internal Dt.Charts.Aggregate Aggregate { get; set; }
        
        internal object[] ItemNamesInternal
        {
            get
            {
                if (ItemNames == null)
                {
                    return _namesInternal;
                }
                if (ItemNames is string)
                {
                    return ((string)ItemNames).Split(null);
                }
                return Utils.CreateArray(ItemNames);
            }
            set { _namesInternal = value; }
        }

        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public IRenderer Renderer
        {
            get { return (IRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        void _children_Changed(object sender, EventArgs e)
        {
            FireDataChanged(this, (EventArgs)new PropertyChangedEventArgs("Children"));
        }

        internal void AddSeries(IDataSeriesInfo ser)
        {
            DataSeries series = (DataSeries)ser;
            series.ParentData = this;
            series.PropertyChanged += new PropertyChangedEventHandler(ChartData_PropertyChanged);
        }

        void AttachHandlers()
        {
            if (((_handlers == null) && (SelectionAction != Dt.Charts.SelectionAction.None)) && (Renderer != null))
            {
                Chart visual = Renderer.Visual as Chart;
                if (visual != null)
                {
                    visual.Tapped += chart_Tapped;
                    _handlers = visual;
                }
            }
        }

        void chart_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SelectionAction == Dt.Charts.SelectionAction.Tap)
            {
                HandleMouseEvent(sender, e);
            }
        }

        void ChartData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FireDataChanged(this, EventArgs.Empty);
        }

        void ChartData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FireDataChanged(this, EventArgs.Empty);
        }

        void CollectionViewCurrentChanged(object sender, object e)
        {
            UpdateSelection(true);
        }

        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public bool CreateDesignTimeData()
        {
            bool flag = false;
            if (Children.Count == 0)
            {
                DataSeries series = new DataSeries
                {
                    Label = "Series 1",
                    ValuesSource = new double[] { 20.0, 22.0, 19.0, 24.0, 25.0 }
                };
                Children.Add(series);
                series = new DataSeries
                {
                    Label = "Series 2",
                    ValuesSource = new double[] { 8.0, 12.0, 10.0, 12.0, 15.0 }
                };
                Children.Add(series);
                flag = true;
            }
            return flag;
        }

        void DetachHandlers()
        {
            if (_handlers != null)
            {
                _handlers.Tapped -= chart_Tapped;
                _handlers = null;
            }
        }

        void FireDataChanged(object sender, EventArgs e)
        {
            if (notify)
            {
                if (DataChanged != null)
                {
                    DataChanged(sender, e);
                }
                if (PropertyChanged != null)
                {
                    PropertyChangedEventArgs args = e as PropertyChangedEventArgs;
                    if (args != null)
                    {
                        PropertyChanged(this, args);
                    }
                    else
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(""));
                    }
                }
            }
        }

        Chart GetChart()
        {
            Chart visual = null;
            if (Renderer != null)
            {
                visual = Renderer.Visual as Chart;
            }
            return visual;
        }

        internal double GetPointSum(int pi)
        {
            double num = 0.0;
            using (IEnumerator<DataSeries> enumerator = Children.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    double[,] values = enumerator.Current.GetValues();
                    if (values != null)
                    {
                        int length = values.GetLength(1);
                        if (pi < length)
                        {
                            num += values[0, pi];
                        }
                    }
                }
            }
            return num;
        }

        void HandleMouseEvent(object sender, TappedRoutedEventArgs e)
        {
            ICollectionView itemsSource = ItemsSource as ICollectionView;
            PlotElement originalSource = e.OriginalSource as PlotElement;
            if (((originalSource != null) && (originalSource.DataPoint != null)) && (originalSource.DataPoint.PointIndex >= 0))
            {
                DataSeries series = originalSource.DataPoint.Series;
                if (series.HasDataSource)
                {
                    ICollectionView view2 = series.ItemsSource as ICollectionView;
                    if (view2 != null)
                    {
                        view2.MoveCurrentToPosition(originalSource.DataPoint.PointIndex);
                    }
                }
                else if (itemsSource != null)
                {
                    itemsSource.MoveCurrentToPosition(originalSource.DataPoint.PointIndex);
                }
            }
        }

        void ItemsSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FireDataChanged(this, EventArgs.Empty);
        }

        static void OnItemNamesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ChartData sender = (ChartData)obj;
            sender.FireDataChanged(sender, EventArgs.Empty);
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            INotifyCollectionChanged oldValue = e.OldValue as INotifyCollectionChanged;
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= ChartData_CollectionChanged;
            }
            else if (e.OldValue is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= ItemsSource_PropertyChanged;
            }
            oldValue = e.NewValue as INotifyCollectionChanged;
            if (oldValue != null)
            {
                oldValue.CollectionChanged += ChartData_CollectionChanged;
            }
            else if (e.NewValue is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)e.NewValue).PropertyChanged += ItemsSource_PropertyChanged;
            }
            ICollectionView newValue = e.OldValue as ICollectionView;
            if (newValue != null)
            {
                newValue.CurrentChanged -= CollectionViewCurrentChanged;
            }
            newValue = e.NewValue as ICollectionView;
            if (newValue != null)
            {
                newValue.CurrentChanged += CollectionViewCurrentChanged;
                UpdateSelection(true);
            }
            FireDataChanged(this, EventArgs.Empty);
        }

        static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((ChartData)obj).OnItemsSourceChanged(e);
        }

        protected virtual void OnRendererChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((IRenderer)e.OldValue).Changed -= new EventHandler(FireDataChanged);
            }
            if (e.NewValue != null)
            {
                ((IRenderer)e.NewValue).Dirty = true;
                ((IRenderer)e.NewValue).Changed += new EventHandler(FireDataChanged);
            }
            IRenderer oldValue = e.OldValue as IRenderer;
            if (oldValue != null)
            {
                oldValue.Rendered -= new EventHandler(rend_Rendered);
                DetachHandlers();
            }
            oldValue = e.NewValue as IRenderer;
            if (oldValue != null)
            {
                oldValue.Rendered += new EventHandler(rend_Rendered);
            }
            FireDataChanged(this, EventArgs.Empty);
        }

        static void OnRendererChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((ChartData)obj).OnRendererChanged(e);
        }

        static void OnSelectionActionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartData data = (ChartData)obj;
            if (data.SelectionAction == Dt.Charts.SelectionAction.None)
            {
                data.DetachHandlers();
            }
            else
            {
                data.AttachHandlers();
            }
        }

        static void OnSeriesItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartData data = (ChartData)obj;
            INotifyCollectionChanged oldValue = args.OldValue as INotifyCollectionChanged;
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= data.SeriesItemsCollectionChanged;
            }
            oldValue = args.NewValue as INotifyCollectionChanged;
            if (oldValue != null)
            {
                oldValue.CollectionChanged += data.SeriesItemsCollectionChanged;
            }
            data.SyncSeries();
        }

        static void OnSeriesItemTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ChartData)obj).SyncSeries();
        }

        internal void RemoveSeries(IDataSeriesInfo ser)
        {
            DataSeries series = (DataSeries)ser;
            series.ParentData = null;
            series.PropertyChanged -= new PropertyChangedEventHandler(ChartData_PropertyChanged);
        }

        void rend_Rendered(object sender, EventArgs e)
        {
            UpdateSelection(false);
            AttachHandlers();
        }

        internal void Reset()
        {
            Children.Clear();
            ItemNameBinding = null;
            ItemNames = null;
            ItemsSource = null;
            ItemNamesInternal = null;
        }

        void SeriesItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SyncSeries();
        }

        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
        public bool ShouldSerializeChildren()
        {
            return true;
        }

        void SyncSeries()
        {
            Chart chart = GetChart();
            if (chart != null)
            {
                chart.BeginUpdate();
            }
            Children.Clear();
            if ((SeriesItemsSource != null) && (SeriesItemTemplate != null))
            {
                foreach (object obj2 in SeriesItemsSource)
                {
                    DataSeries series = SeriesItemTemplate.LoadContent() as DataSeries;
                    if (series != null)
                    {
                        series.DataContext = obj2;
                        Children.Add(series);
                    }
                }
            }
            if (chart != null)
            {
                chart.EndUpdate();
            }
        }
        
        internal void UpdateSelection(bool usePrevious = true)
        {
            ICollectionView itemsSource = ItemsSource as ICollectionView;
            if (itemsSource != null)
            {
                int previousIndex = usePrevious ? currentIndex : -1;
                currentIndex = itemsSource.CurrentPosition;
                int num2 = Children.Count;
                for (int i = 0; i < num2; i++)
                {
                    DataSeries series = Children[i];
                    if (!series.HasDataSource)
                    {
                        series.UpdateSelectedElement(previousIndex, currentIndex);
                    }
                }
            }
            else
            {
                int num4 = Children.Count;
                for (int j = 0; j < num4; j++)
                {
                    DataSeries series2 = Children[j];
                    series2.nloaded = 0;
                    if (series2.HasDataSource)
                    {
                        series2.UpdateSelection(usePrevious);
                    }
                }
            }
        }
    }
}

