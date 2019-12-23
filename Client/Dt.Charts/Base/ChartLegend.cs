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
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 图例说明
    /// </summary>
    public partial class ChartLegend : ItemsControl
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", 
            (Type)typeof(Orientation), 
            (Type)typeof(ChartLegend),
            new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(ChartLegend.OnOrientationChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OverlapChartProperty = DependencyProperty.Register(
            "OverlapChart", 
            (Type)typeof(bool),
            (Type)typeof(ChartLegend), 
            new PropertyMetadata((bool)false, new PropertyChangedCallback(ChartLegend.OnOverlapChartChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", 
            (Type)typeof(object), 
            (Type)typeof(ChartLegend),
            new PropertyMetadata(null, new PropertyChangedCallback(ChartLegend.OnTitleChanged)));

        private Chart _parent;
        private LegendPosition _position;
        private StackPanel sp;
        
        public ChartLegend()
        {
            DefaultStyleKey = typeof(ChartLegend);
            _position = LegendPosition.Right;
            Loaded += (sender, e) => { UpdatePosition(); };
            if (DesignMode.DesignModeEnabled)
            {
                SizeChanged += C1ChartLegend_SizeChanged;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Title
        {
            get { return base.GetValue(TitleProperty); }
            set { base.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)base.GetValue(OrientationProperty); }
            set { base.SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool OverlapChart
        {
            get { return (bool)((bool)base.GetValue(OverlapChartProperty)); }
            set { base.SetValue(OverlapChartProperty, (bool)value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public LegendPosition Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    UpdatePosition();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Chart Chart
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    if (_parent != null)
                    {
                        _parent.LegendChanged -= new EventHandler(_parent_LegendChanged);
                    }
                    _parent = value;
                    if (_parent != null)
                    {
                        _parent.LegendChanged += new EventHandler(_parent_LegendChanged);
                    }
                    else
                    {
                        base.ItemsSource = null;
                    }
                    _parent.InitLegendBindings(this);
                }
            }
        }

        private void _parent_LegendChanged(object sender, EventArgs e)
        {
            while (base.Items.Count > 0)
            {
                base.Items.RemoveAt(0);
            }
            foreach (LegendItem item in _parent.LegendItems)
            {
                base.Items.Add(item);
            }
        }

        private void C1ChartLegend_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Chart != null)
            {
                Chart.InvalidateChart();
            }
        }

        private static void OnOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartLegend legend = (ChartLegend)obj;
            if (legend.sp != null)
            {
                legend.sp.Orientation = legend.Orientation;
            }
        }

        private static void OnOverlapChartChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartLegend legend = (ChartLegend)obj;
            legend.UpdatePosition();
        }

        private static void OnTitleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element != null)
            {
                sp = VisualTreeHelper.GetParent(element) as StackPanel;
                if ((sp != null) && (sp.Orientation != Orientation))
                {
                    sp.Orientation = Orientation;
                }
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        private void UpdatePosition()
        {
            switch (Position)
            {
                case LegendPosition.Left:
                    Grid.SetColumn(this, OverlapChart ? 1 : 0);
                    Grid.SetRow(this, 1);
                    base.HorizontalAlignment = HorizontalAlignment.Left;
                    base.VerticalAlignment = VerticalAlignment.Center;
                    return;

                case LegendPosition.Right:
                    Grid.SetColumn(this, OverlapChart ? 1 : 2);
                    Grid.SetRow(this, 1);
                    base.HorizontalAlignment = HorizontalAlignment.Right;
                    base.VerticalAlignment = VerticalAlignment.Center;
                    return;

                case LegendPosition.Top:
                case LegendPosition.TopCenter:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 0);
                    base.HorizontalAlignment = HorizontalAlignment.Center;
                    base.VerticalAlignment = VerticalAlignment.Top;
                    return;

                case LegendPosition.Bottom:
                case LegendPosition.BottomCenter:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 2);
                    base.HorizontalAlignment = HorizontalAlignment.Center;
                    base.VerticalAlignment = VerticalAlignment.Bottom;
                    return;

                case LegendPosition.TopLeft:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 0);
                    base.HorizontalAlignment = HorizontalAlignment.Left;
                    base.VerticalAlignment = VerticalAlignment.Top;
                    return;

                case LegendPosition.TopRight:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 0);
                    base.HorizontalAlignment = HorizontalAlignment.Right;
                    base.VerticalAlignment = VerticalAlignment.Top;
                    return;

                case LegendPosition.BottomLeft:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 2);
                    base.HorizontalAlignment = HorizontalAlignment.Left;
                    base.VerticalAlignment = VerticalAlignment.Bottom;
                    return;

                case LegendPosition.BottomRight:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 2);
                    base.HorizontalAlignment = HorizontalAlignment.Right;
                    base.VerticalAlignment = VerticalAlignment.Bottom;
                    return;
            }
        }
    }
}