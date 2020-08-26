#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public abstract partial class SpreadChartBaseView : Panel
    {
        Control _c1ChartControl;
        internal ChartTitleView _chartTitleView;
        internal Rectangle _formatRect;
        Grid _rootLayoutGrid;
        internal SpreadChartBase _spreadChartContent;

        public SpreadChartBaseView(SpreadChartBase spreadChartContent, Control c1Chart)
        {
            _spreadChartContent = spreadChartContent;
            _formatRect = new Rectangle();
            _formatRect.Fill = new SolidColorBrush(Colors.Transparent);
            Children.Add(_formatRect);

            _rootLayoutGrid = new Grid();
            RowDefinition definition = new RowDefinition();
            definition.Height = new Windows.UI.Xaml.GridLength(0.0, Windows.UI.Xaml.GridUnitType.Auto);
            _rootLayoutGrid.RowDefinitions.Add(definition);
            _rootLayoutGrid.RowDefinitions.Add(new RowDefinition());
            Children.Add(_rootLayoutGrid);

            _chartTitleView = new ChartTitleView(_spreadChartContent.ChartTitle, this);
            _chartTitleView.Margin = new Thickness(0.0, 3.0, 0.0, 3.0);
            _chartTitleView.HorizontalAlignment = HorizontalAlignment.Center;
            _rootLayoutGrid.Children.Add(_chartTitleView);
            Grid.SetRow(_chartTitleView, 0);
            _c1ChartControl = c1Chart;
            _rootLayoutGrid.Children.Add(c1Chart);
            Grid.SetRow(c1Chart, 1);
            IsHitTestVisible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _formatRect.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            _rootLayoutGrid.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }

        internal Size GetChartTitleSize(ChartTitle title)
        {
            Size chartTitleSize = Size.Empty;
            if ((title != null) && !string.IsNullOrEmpty(title.Text))
            {
                double fontSize = title.ActualFontSize * ZoomFactor;
                if (fontSize < 0.0)
                {
                    fontSize = new TextBlock().FontSize;
                }
                FontFamily fontFamily = title.ActualFontFamily;
                if (fontFamily == null)
                {
                    fontFamily = Dt.Cells.Data.Utility.DefaultFontFamily;
                }
                chartTitleSize = MeasureHelper.MeasureTextBlock(title.Text, fontFamily, fontSize, title.ActualFontStretch, title.ActualFontStyle, title.FontWeight, new Size(double.PositiveInfinity, double.PositiveInfinity), false, null, UseLayoutRounding, ZoomFactor);
            }
            if (!chartTitleSize.IsEmpty)
            {
                chartTitleSize = MeasureHelper.ConvertTextSizeToExcelCellSize(chartTitleSize, ZoomFactor);
            }
            else
            {
                chartTitleSize = new Size(0.0, 0.0);
            }
            return chartTitleSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            _formatRect.Measure(availableSize);
            _rootLayoutGrid.Measure(availableSize);
            UpdateLayoutsOnMeasure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        internal virtual void RefreshAxises()
        {
        }

        internal virtual void RefreshAxisesTitle()
        {
        }

        internal virtual void RefreshAxisX()
        {
        }

        internal virtual void RefreshAxisY()
        {
        }

        internal virtual void RefreshAxisZ()
        {
        }

        internal virtual void RefreshChartArea()
        {
        }

        internal virtual void RefreshChartContent()
        {
        }

        internal virtual void RefreshChartLegend()
        {
        }

        internal virtual void RefreshChartTitle()
        {
        }

        internal virtual void RefreshDataSeries()
        {
        }

        internal virtual void RefreshPlotArea()
        {
        }
        
        internal void UpdateC1ChartControl(Control newC1Chart)
        {
            if (_rootLayoutGrid.Children.Contains(_c1ChartControl))
            {
                _rootLayoutGrid.Children.Remove(_c1ChartControl);
                _c1ChartControl = newC1Chart;
                _rootLayoutGrid.Children.Add(_c1ChartControl);
                Grid.SetRow(_c1ChartControl, 1);
            }
        }

        internal virtual void UpdateLayoutsOnMeasure(Size size)
        {
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = new Rect(0.0, 0.0, size.Width, size.Height);
            Clip = geometry;
        }

        internal Control C1ChartControl
        {
            get { return  _c1ChartControl; }
        }

        internal CellsPanel ParentViewport { get; set; }

        internal virtual double ZoomFactor
        {
            get
            {
                if ((ParentViewport != null) && (ParentViewport.Excel != null))
                {
                    return (double) ParentViewport.Excel.ZoomFactor;
                }
                return 1.0;
            }
        }
    }
}

