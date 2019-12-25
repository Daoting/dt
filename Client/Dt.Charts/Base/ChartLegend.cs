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
        #region 静态内容
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position",
            typeof(LegendPosition),
            typeof(ChartLegend),
            new PropertyMetadata(LegendPosition.Bottom, OnPositionChanged));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(ChartLegend),
            new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty OverlapChartProperty = DependencyProperty.Register(
            "OverlapChart",
            typeof(bool),
            typeof(ChartLegend),
            new PropertyMetadata(false, OnOverlapChartChanged));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(object),
            typeof(ChartLegend),
            new PropertyMetadata(null));

        static void OnOverlapChartChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ChartLegend)obj).UpdatePosition();
        }

        static void OnPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ChartLegend)obj).UpdatePosition();
        }
        #endregion

        Chart _parent;

        public ChartLegend()
        {
            DefaultStyleKey = typeof(ChartLegend);
        }

        /// <summary>
        /// 
        /// </summary>
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool OverlapChart
        {
            get { return (bool)GetValue(OverlapChartProperty); }
            set { SetValue(OverlapChartProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public LegendPosition Position
        {
            get { return (LegendPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
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
                        _parent.LegendChanged -= _parent_LegendChanged;
                    _parent = value;
                    if (_parent != null)
                        _parent.LegendChanged += _parent_LegendChanged;
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            // 为绑定Orientation
            var pre = (ItemsPresenter)GetTemplateChild("Presenter");
            if (pre != null)
                pre.DataContext = this;

            UpdatePosition();
        }

        void _parent_LegendChanged(object sender, EventArgs e)
        {
            Items.Clear();
            foreach (LegendItem item in _parent.LegendItems)
            {
                Items.Add(item);
            }
        }

        void UpdatePosition()
        {
            switch (Position)
            {
                case LegendPosition.Left:
                    Grid.SetColumn(this, OverlapChart ? 1 : 0);
                    Grid.SetRow(this, 1);
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Center;
                    return;

                case LegendPosition.Right:
                    Grid.SetColumn(this, OverlapChart ? 1 : 2);
                    Grid.SetRow(this, 1);
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Center;
                    return;

                case LegendPosition.Top:
                case LegendPosition.TopCenter:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 0);
                    HorizontalAlignment = HorizontalAlignment.Center;
                    VerticalAlignment = VerticalAlignment.Top;
                    return;

                case LegendPosition.Bottom:
                case LegendPosition.BottomCenter:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 2);
                    HorizontalAlignment = HorizontalAlignment.Center;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    return;

                case LegendPosition.TopLeft:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 0);
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Top;
                    return;

                case LegendPosition.TopRight:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 0);
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Top;
                    return;

                case LegendPosition.BottomLeft:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 2);
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    return;

                case LegendPosition.BottomRight:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 1 : 2);
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    return;
            }
        }
    }
}