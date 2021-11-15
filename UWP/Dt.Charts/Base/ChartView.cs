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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    [ContentProperty(Name = "Axes")]
    public partial class ChartView : FrameworkElement
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InvertedProperty = DependencyProperty.Register(
            "Inverted",
            typeof(bool),
            typeof(ChartView),
            new PropertyMetadata((bool)false, new PropertyChangedCallback(ChartView.OnInvertedChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PlotBackgroundProperty = Utils.RegisterProperty(
            "PlotBackground",
            typeof(Brush),
            typeof(ChartView),
            new PropertyChangedCallback(ChartView.OnPlotBackgroundChanged));

        readonly ChartViewport2D _viewport;
        readonly Chart _chart;
        PlotAreaCollection _areas;
        Axis _ax;
        readonly AxisCollection _axes = new AxisCollection();
        Axis _ay;
        Dictionary<UIElement, Point> _childPoints = new Dictionary<UIElement, Point>();
        ObservableCollection<UIElement> _children;
        ObservableCollection<IChartLayer> _layers;
        Shape _plotShape;
        IRenderer _renderer;
        SolidColorBrush _transparentBrush = new SolidColorBrush(Colors.Transparent);


        public ChartView(Chart p_chart)
        {
            _chart = p_chart;
            _axes.View = this;
            AxisX = new Axis();
            AxisY = new Axis();
            AxisX.SetFixedType(AxisType.X);
            AxisY.SetFixedType(AxisType.Y);
            _axes.Add(AxisX);
            _axes.Add(AxisY);

            _viewport = new ChartViewport2D(this);

            _plotShape = new Rectangle();
            _plotShape.Fill = new SolidColorBrush(Colors.Transparent);

            _areas = new PlotAreaCollection();
            _areas.CollectionChanged += _areas_CollectionChanged;

            _layers = new ObservableCollection<IChartLayer>();
            _layers.CollectionChanged += _layers_CollectionChanged;
        }

        /// <summary>
        /// 获取设置是否转换XY轴
        /// </summary>
        public bool Inverted
        {
            get { return (bool)GetValue(InvertedProperty); }
            set { SetValue(InvertedProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Axis AxisX
        {
            get { return _ax; }
            set
            {
                if ((_ax != value) && (value != null))
                {
                    ReplaceAxis(_ax, value);
                    _ax = value;
                    _ax.SetFixedType(AxisType.X);
                    _ax.PropertyChanged += new PropertyChangedEventHandler(_axis_Changed);
                    _viewport?.SetAxisX(_ax);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Axis AxisY
        {
            get { return _ay; }
            set
            {
                if ((_ay != value) && (value != null))
                {
                    ReplaceAxis(_ay, value);
                    _ay = value;
                    _ay.SetFixedType(AxisType.Y);
                    _ay.PropertyChanged += new PropertyChangedEventHandler(_axis_Changed);
                    _viewport?.SetAxisY(_ay);
                }
            }
        }

        public AxisCollection Axes
        {
            get { return _axes; }
        }

        internal Chart Chart
        {
            get { return _chart; }
        }

        public ObservableCollection<UIElement> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new ObservableCollection<UIElement>();
                    _children.CollectionChanged += _children_CollectionChanged;
                }
                return _children;
            }
        }

        internal bool HasPlotAreas
        {
            get
            {
                int num = Axes.Count;
                for (int i = 0; i < num; i++)
                {
                    if (Axes[i].PlotAreaIndex > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public ObservableCollection<IChartLayer> Layers
        {
            get { return _layers; }
        }

        public ObservableCollection<PlotAreaColumnDefinition> PlotAreaColumnDefinitions
        {
            get { return _areas.ColumnDefinitions; }
        }

        public ObservableCollection<PlotAreaRowDefinition> PlotAreaRowDefinitions
        {
            get { return _areas.RowDefinitions; }
        }

        public PlotAreaCollection PlotAreas
        {
            get { return _areas; }
        }

        public Brush PlotBackground
        {
            get { return (Brush)base.GetValue(PlotBackgroundProperty); }
            set { base.SetValue(PlotBackgroundProperty, value); }
        }

        public Rect PlotRect
        {
            get { return _viewport.PlotRect; }
        }

        public Shape PlotShape
        {
            get { return _plotShape; }
        }

        internal IRenderer Renderer
        {
            get { return _renderer; }
            set
            {
                if (_renderer != value)
                {
                    _renderer = value;
                    _viewport.Reset();
                }
            }
        }

        /// <summary>
        /// 图表实际的可视内容
        /// </summary>
        internal ChartViewport2D Viewport
        {
            get { return _viewport; }
        }

        void _areas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _chart.InvalidateChart();
        }

        void _axis_Changed(object sender, EventArgs e)
        {
            _viewport.InvalidateMeasure();
        }

        void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (object obj2 in e.OldItems)
                {
                    UIElement element = obj2 as UIElement;
                    if (_viewport.Children.Contains(element))
                    {
                        _viewport.Children.Remove(element);
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (object obj3 in e.NewItems)
                {
                    UIElement element2 = obj3 as UIElement;
                    if (!_viewport.Children.Contains(element2))
                    {
                        _viewport.Children.Add(element2);
                    }
                }
            }
        }

        void _layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (IChartLayer layer in e.NewItems)
                    {
                        layer.Chart = _chart;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (IChartLayer layer2 in e.OldItems)
                    {
                        layer2.Chart = null;
                    }
                    break;

                default:
                    if (e.NewItems != null)
                    {
                        foreach (IChartLayer layer3 in e.NewItems)
                        {
                            layer3.Chart = _chart;
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (IChartLayer layer4 in e.OldItems)
                        {
                            layer4.Chart = null;
                        }
                    }
                    break;
            }
        }

        internal void AddAxis(Axis ax)
        {
            ax.PropertyChanged += new PropertyChangedEventHandler(_axis_Changed);
        }

        public int DataIndexFromPoint(Point pt, int seriesIndex, MeasureOption option, out double distance)
        {
            distance = double.MaxValue;
            if (seriesIndex >= _chart.Data.Children.Count)
            {
                return -1;
            }
            DataSeries ds = _chart.Data.Children[seriesIndex];
            double[,] values = ds.GetValues();
            if (values == null)
            {
                return -1;
            }
            PointD td = PointToData(ds, pt);
            int num = 0;
            double y = td.Y;
            if (option == MeasureOption.X)
            {
                num = 1;
                y = td.X;
            }
            else if (option == MeasureOption.XY)
            {
                num = 2;
            }
            int num3 = -1;
            int length = values.GetLength(0);
            Axis axisX = GetAxisX(ds);
            Axis axisY = GetAxisY(ds);
            double actualMin = axisX.ActualMin;
            double actualMax = axisX.ActualMax;
            double num7 = axisY.ActualMin;
            double num8 = axisY.ActualMax;
            if (values != null)
            {
                int num9 = values.GetLength(1);
                for (int i = 0; i < num9; i++)
                {
                    double d = (length > 1) ? values[1, i] : ((double)i);
                    double num12 = values[0, i];
                    if (((d >= actualMin) && (d <= actualMax)) && ((num12 >= num7) && (num12 <= num8)))
                    {
                        if (num == 2)
                        {
                            if (!double.IsNaN(d) && !double.IsNaN(num12))
                            {
                                PointD td2 = PointFromData(ds, new PointD(d, num12));
                                double num13 = td2.X - pt.X;
                                double num14 = td2.Y - pt.Y;
                                double num15 = Math.Sqrt((num13 * num13) + (num14 * num14));
                                if (num15 < distance)
                                {
                                    distance = num15;
                                    num3 = i;
                                }
                            }
                        }
                        else
                        {
                            double num16 = (num == 1) ? d : num12;
                            if (!double.IsNaN(num16))
                            {
                                double num17 = Math.Abs((double)(num16 - y));
                                if (num17 < distance)
                                {
                                    distance = num17;
                                    num3 = i;
                                }
                            }
                        }
                    }
                }
            }
            if (distance != double.MaxValue)
            {
                if (option == MeasureOption.X)
                {
                    distance = _viewport.ConvertX(distance) - _viewport.ConvertX(0.0);
                    return num3;
                }
                if (option == MeasureOption.Y)
                {
                    distance = _viewport.ConvertY(distance) - _viewport.ConvertY(0.0);
                }
            }
            return num3;
        }

        public Point DataIndexToPoint(int seriesIndex, int pointIndex)
        {
            PointD pt = new PointD(double.NaN, double.NaN);
            DataSeries ds = _chart.Data.Children[seriesIndex];
            double[,] values = ds.GetValues();
            if (values != null)
            {
                int length = values.GetLength(0);
                int num2 = values.GetLength(1);
                if ((pointIndex < 0) || (pointIndex >= num2))
                {
                    throw new ArgumentOutOfRangeException("pointIndex");
                }
                pt.Y = values[0, pointIndex];
                pt.X = (length > 1) ? values[1, pointIndex] : ((double)pointIndex);
                pt = PointFromData(ds, pt);
            }
            return new Point(pt.X, pt.Y);
        }

        internal Axis GetAxisX(DataSeries ds)
        {
            if (!string.IsNullOrEmpty(ds.AxisX))
            {
                Axis axis = Axes[ds.AxisX];
                if (axis != null)
                {
                    return axis;
                }
            }
            return AxisX;
        }

        internal Axis GetAxisY(DataSeries ds)
        {
            if (!string.IsNullOrEmpty(ds.AxisY))
            {
                Axis axis = Axes[ds.AxisY];
                if (axis != null)
                {
                    return axis;
                }
            }
            return AxisY;
        }

        public Point GetDataPoint(UIElement visual)
        {
            return _childPoints[visual];
        }

        static void OnInvertedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartView view = (ChartView)obj;
            view._chart.ApplyChartType(view._chart.ChartType);
        }

        static void OnPlotBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ChartView view = obj as ChartView;
            if (view.PlotShape != null)
            {
                view.PlotShape.Fill = e.NewValue as Brush;
                if (view.PlotShape.Fill == null)
                {
                    view.PlotShape.Fill = view._transparentBrush;
                }
            }
        }

        internal PointD PointFromData(PointD pt)
        {
            return new PointD(_viewport.ConvertX(pt.X), _viewport.ConvertY(pt.Y));
        }

        public Point PointFromData(Point pt)
        {
            return new Point(_viewport.ConvertX(pt.X), _viewport.ConvertY(pt.Y));
        }

        PointD PointFromData(DataSeries ds, PointD pt)
        {
            return PointFromData(ds.AxisX, ds.AxisY, pt);
        }

        internal PointD PointFromData(string axisx, string axisy, PointD pt)
        {
            PointD td = pt;
            if (!string.IsNullOrEmpty(axisx))
            {
                Axis axis = Axes[axisx];
                if (axis != null)
                {
                    pt = axis.PointFromData(pt);
                }
                if (!string.IsNullOrEmpty(axisy))
                {
                    double x = pt.X;
                    Axis axis2 = Axes[axisy];
                    if (axis2 != null)
                    {
                        pt = axis2.PointFromData(td);
                    }
                    pt.X = x;
                }
                return pt;
            }
            if (!string.IsNullOrEmpty(axisy))
            {
                Axis axis3 = Axes[axisy];
                if (axis3 != null)
                {
                    pt = axis3.PointFromData(pt);
                    return pt;
                }
                pt = PointFromData(pt);
                return pt;
            }
            pt = PointFromData(pt);
            return pt;
        }

        internal Point PointFromData(string axisx, string axisy, Point pt)
        {
            PointD td = PointFromData(axisx, axisy, new PointD(pt.X, pt.Y));
            return new Point(td.X, td.Y);
        }

        internal PointD PointToData(PointD pt)
        {
            return new PointD(_viewport.ConvertBackX(pt.X), _viewport.ConvertBackY(pt.Y));
        }

        public Point PointToData(Point pt)
        {
            return new Point(_viewport.ConvertBackX(pt.X), _viewport.ConvertBackY(pt.Y));
        }

        PointD PointToData(DataSeries ds, PointD pt)
        {
            return PointToData(ds.AxisX, ds.AxisY, pt);
        }

        PointD PointToData(DataSeries ds, Point pt)
        {
            return PointToData(ds.AxisX, ds.AxisY, new PointD(pt.X, pt.Y));
        }

        internal PointD PointToData(string axisx, string axisy, PointD pt)
        {
            PointD td = pt;
            if (!string.IsNullOrEmpty(axisx))
            {
                Axis axis = Axes[axisx];
                if (axis != null)
                {
                    pt = axis.PointToData(pt);
                }
                if (!string.IsNullOrEmpty(axisy))
                {
                    double x = pt.X;
                    Axis axis2 = Axes[axisy];
                    if (axis2 != null)
                    {
                        pt = axis2.PointToData(td);
                    }
                    pt.X = x;
                }
                return pt;
            }
            if (!string.IsNullOrEmpty(axisy))
            {
                Axis axis3 = Axes[axisy];
                if (axis3 != null)
                {
                    pt = axis3.PointToData(pt);
                    return pt;
                }
                pt = PointToData(pt);
                return pt;
            }
            pt = PointToData(pt);
            return pt;
        }

        internal Point PointToData(string axisx, string axisy, Point pt)
        {
            PointD td = PointToData(axisx, axisy, new PointD(pt.X, pt.Y));
            return new Point(td.X, td.Y);
        }

        internal void RemoveAxis(Axis ax)
        {
            ax.PropertyChanged -= new PropertyChangedEventHandler(_axis_Changed);
        }

        void ReplaceAxis(Axis oldAxis, Axis newAxis)
        {
            if ((oldAxis != null) && (newAxis != null))
            {
                int index = Axes.IndexOf(oldAxis);
                if (index >= 0)
                {
                    Axes[index] = newAxis;
                }
            }
        }

        internal void ResetInternal()
        {
            Children.Clear();
            if (AxisX != null)
            {
                AxisX.Reset();
            }
            if (AxisY != null)
            {
                AxisY.Reset();
            }
            Axes.Clear();
        }

        public void SetDataPoint(UIElement visual, Point point)
        {
            _childPoints[visual] = point;
        }

        internal void UpdateChildPositions()
        {
            foreach (UIElement element in Children)
            {
                if (_childPoints.ContainsKey(element))
                {
                    Point point = PointFromData(_childPoints[element]);
                    element.SetValue(Canvas.LeftProperty, (double)point.X);
                    element.SetValue(Canvas.TopProperty, (double)point.Y);
                }
                else
                {
                    element.SetValue(Canvas.LeftProperty, (double)0.0);
                    element.SetValue(Canvas.TopProperty, (double)0.0);
                }
            }
        }

        internal PlotArea UpdateMainPlotArea()
        {
            PlotArea area = null;
            int num = PlotAreas.Count;
            for (int i = 0; i < num; i++)
            {
                if ((PlotAreas[i].Column == AxisX.PlotAreaIndex) && (PlotAreas[i].Row == AxisY.PlotAreaIndex))
                {
                    area = PlotAreas[i];
                    break;
                }
            }
            return area;
        }
    }
}

