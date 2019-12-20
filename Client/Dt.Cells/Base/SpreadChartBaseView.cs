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
        private Control _c1ChartControl;
        internal ChartTitleView _chartTitleView;
        internal Rectangle _formatRect;
        private Grid _rootLayoutGrid;
        internal SpreadChartBase _spreadChartContent;
        internal ResourceDictionary resourceDictionary;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spreadChartContent"></param>
        /// <param name="c1Chart"></param>
        public SpreadChartBaseView(SpreadChartBase spreadChartContent, Control c1Chart)
        {
            this._spreadChartContent = spreadChartContent;
            this._formatRect = new Rectangle();
            this._formatRect.Fill = new SolidColorBrush(Colors.Transparent);
            base.Children.Add(this._formatRect);
            this._rootLayoutGrid = new Grid();
            RowDefinition definition = new RowDefinition();
            definition.Height = new Windows.UI.Xaml.GridLength(0.0, Windows.UI.Xaml.GridUnitType.Auto);
            this._rootLayoutGrid.RowDefinitions.Add(definition);
            this._rootLayoutGrid.RowDefinitions.Add(new RowDefinition());
            base.Children.Add(this._rootLayoutGrid);
            this._chartTitleView = new ChartTitleView(this._spreadChartContent.ChartTitle, this);
            this._chartTitleView.Margin = new Windows.UI.Xaml.Thickness(0.0, 3.0, 0.0, 3.0);
            this._chartTitleView.HorizontalAlignment = HorizontalAlignment.Center;
            this._rootLayoutGrid.Children.Add(this._chartTitleView);
            Grid.SetRow(this._chartTitleView, 0);
            this._c1ChartControl = c1Chart;
            this._rootLayoutGrid.Children.Add(c1Chart);
            Grid.SetRow(c1Chart, 1);
            base.IsHitTestVisible = false;

            // hdt 原来放在Loaded事件中，无法打印
            Uri uri = new Uri("ms-appx:///Dt.Cells/Themes/DataLableTemplate.xaml");
            this.resourceDictionary = new ResourceDictionary();
            Application.LoadComponent(this.resourceDictionary, uri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            this._formatRect.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            this._rootLayoutGrid.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }

        internal Windows.Foundation.Size GetChartTitleSize(ChartTitle title)
        {
            Windows.Foundation.Size chartTitleSize = Windows.Foundation.Size.Empty;
            if ((title != null) && !string.IsNullOrEmpty(title.Text))
            {
                object textFormattingMode = null;
                Dt.Cells.Data.UIAdaptor.InvokeSync(delegate {
                    double fontSize = title.ActualFontSize * this.ZoomFactor;
                    if (fontSize < 0.0)
                    {
                        fontSize = new TextBlock().FontSize;
                    }
                    FontFamily fontFamily = title.ActualFontFamily;
                    if (fontFamily == null)
                    {
                        fontFamily = Dt.Cells.Data.Utility.DefaultFontFamily;
                    }
                    chartTitleSize = MeasureHelper.MeasureTextBlock(title.Text, fontFamily, fontSize, title.ActualFontStretch, title.ActualFontStyle, title.FontWeight, new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity), false, textFormattingMode, this.UseLayoutRounding, this.ZoomFactor);
                });
            }
            if (!chartTitleSize.IsEmpty)
            {
                chartTitleSize = MeasureHelper.ConvertTextSizeToExcelCellSize(chartTitleSize, this.ZoomFactor);
            }
            else
            {
                chartTitleSize = new Windows.Foundation.Size(0.0, 0.0);
            }
            return chartTitleSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            this._formatRect.Measure(availableSize);
            this._rootLayoutGrid.Measure(availableSize);
            this.UpdateLayoutsOnMeasure(availableSize);
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
            if (this._rootLayoutGrid.Children.Contains(this._c1ChartControl))
            {
                this._rootLayoutGrid.Children.Remove(this._c1ChartControl);
                this._c1ChartControl = newC1Chart;
                this._rootLayoutGrid.Children.Add(this._c1ChartControl);
                Grid.SetRow(this._c1ChartControl, 1);
            }
        }

        internal virtual void UpdateLayoutsOnMeasure(Windows.Foundation.Size size)
        {
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = new Windows.Foundation.Rect(0.0, 0.0, size.Width, size.Height);
            base.Clip = geometry;
        }

        internal Control C1ChartControl
        {
            get { return  this._c1ChartControl; }
        }

        internal GcViewport ParentViewport { get; set; }

        internal virtual double ZoomFactor
        {
            get
            {
                if ((this.ParentViewport != null) && (this.ParentViewport.Sheet != null))
                {
                    return (double) this.ParentViewport.Sheet.ZoomFactor;
                }
                return 1.0;
            }
        }
    }
}

