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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 图例说明
    /// </summary>
    public partial class ChartLegend : UnoControl
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
        StackPanel _pnl;

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
                        _parent.LegendChanged -= OnLegendChanged;
                    _parent = value;
                    if (_parent != null)
                        _parent.LegendChanged += OnLegendChanged;
                }
            }
        }

        protected override void OnLoadTemplate()
        {
            _pnl = (StackPanel)GetTemplateChild("ItemPanel");
            UpdatePosition();
            OnLegendChanged(null, null);
        }

        void OnLegendChanged(object sender, EventArgs e)
        {
            if (_pnl == null)
                return;

            if (_pnl.Children.Count > 0)
                _pnl.Children.Clear();
            foreach (LegendItem item in _parent.LegendItems)
            {
                Grid grid = new Grid
                {
                    Height = 24,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(24) },
                        new ColumnDefinition { Width = GridLength.Auto },
                    }
                };

                if (item.Symbol != null)
                {
                    item.Symbol.Margin = new Thickness(5);
                    grid.Children.Add(item.Symbol);
                }
                else if (item.Line != null)
                {
                    grid.Children.Add(item.Line);
                }

                TextBlock tb = new TextBlock { Text = item.Label == null ? "无" : item.Label, VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(tb, 1);
                grid.Children.Add(tb);

                _pnl.Children.Add(grid);
            }
        }

        void UpdatePosition()
        {
            switch (Position)
            {
                case LegendPosition.Left:
                    Grid.SetColumn(this, OverlapChart ? 1 : 0);
                    Grid.SetRow(this, 2);
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Center;
                    return;

                case LegendPosition.Right:
                    Grid.SetColumn(this, OverlapChart ? 1 : 2);
                    Grid.SetRow(this, 2);
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Center;
                    return;

                case LegendPosition.Top:
                case LegendPosition.TopCenter:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 2 : 1);
                    HorizontalAlignment = HorizontalAlignment.Center;
                    VerticalAlignment = VerticalAlignment.Top;
                    return;

                case LegendPosition.Bottom:
                case LegendPosition.BottomCenter:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 2 : 3);
                    HorizontalAlignment = HorizontalAlignment.Center;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    return;

                case LegendPosition.TopLeft:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 2 : 1);
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Top;
                    return;

                case LegendPosition.TopRight:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 2 : 1);
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Top;
                    return;

                case LegendPosition.BottomLeft:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 2 : 3);
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    return;

                case LegendPosition.BottomRight:
                    Grid.SetColumn(this, 1);
                    Grid.SetRow(this, OverlapChart ? 2 : 3);
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    return;
            }
        }
    }
}