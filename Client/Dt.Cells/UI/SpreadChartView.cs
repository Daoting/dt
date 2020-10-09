#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Dt.Charts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SpreadChartView : SpreadChartBaseView
    {
        // hdt 原来放在Loaded事件中，无法打印
        Dictionary<int, SpreadDataSeries> _cachedDataSeries;
        Dictionary<int, List<PlotElement>> _cachedPlotElement;
        int _cachedTotalDataPointCount;
        int _loadedDataPointCount;

        public SpreadChartView(Dt.Cells.Data.SpreadChart spreadChartContent, Chart c1Chart)
            : base(spreadChartContent, c1Chart)
        {
            _cachedDataSeries = new Dictionary<int, SpreadDataSeries>();
            _cachedPlotElement = new Dictionary<int, List<PlotElement>>();
            // hdt 原来放在Loaded事件中，无法打印
            SyncSpreadChartToC1Chart();
        }

        /// <summary>
        /// 加载事件或刷新时绘制图表
        /// </summary>
        void SyncSpreadChartToC1Chart()
        {
            SyncC1ChartSettings();
            C1Chart.BeginUpdate();
            SyncChartTitle();
            SyncChartArea();
            SyncLegend();
            SyncPlotArea();
            SyncAxises();
            SyncDataSeries();
            C1Chart.EndUpdate();
        }

        void SyncC1ChartSettings()
        {
            UpdateC1ChartControl(new Chart());
            C1Chart.Background = new SolidColorBrush(Colors.Transparent);
            C1Chart.IsTabStop = false;
            double width = 5.0 * ZoomFactor;
            double num2 = 50.0 * ZoomFactor;
            BubbleOptions.SetMinSize(C1Chart, new Size(width, width));
            BubbleOptions.SetMaxSize(C1Chart, new Size(num2, num2));
        }

        void SyncChartTitle()
        {
            Size chartTitleSize = GetChartTitleSize(SpreadChart.ChartTitle);
            _chartTitleView.Width = chartTitleSize.Width;
            _chartTitleView.Height = chartTitleSize.Height;
            _chartTitleView.ChartTitle = SpreadChart.ChartTitle;
        }

        void SyncChartArea()
        {
            _formatRect.Stroke = SpreadChart.ActualStroke;
            _formatRect.StrokeDashArray = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(SpreadChart.StrokeDashType);
            double strokeThickness = SpreadChart.StrokeThickness;
            if (strokeThickness > 0.0)
            {
                _formatRect.StrokeThickness = strokeThickness * ZoomFactor;
            }
            else
            {
                _formatRect.StrokeThickness = 0.0;
            }
            if (SpreadChart.CornerRadius > 0.0)
            {
                _formatRect.RadiusX = SpreadChart.CornerRadius;
                _formatRect.RadiusY = SpreadChart.CornerRadius;
            }
            Brush actualFill = SpreadChart.ActualFill;
            if (actualFill != null)
            {
                _formatRect.Fill = actualFill;
            }
            else
            {
                _formatRect.Fill = new SolidColorBrush(Colors.Transparent);
            }
            C1Chart.ChartType = SpreadChartTypeToC1ChartType(SpreadChart.ChartType);
            FontFamily actualFontFamily = SpreadChart.ActualFontFamily;
            if (actualFontFamily != null)
            {
                C1Chart.FontFamily = actualFontFamily;
            }
            double num2 = SpreadChart.ActualFontSize * ZoomFactor;
            if (num2 > 0.0)
            {
                C1Chart.FontSize = num2;
            }
            C1Chart.FontStretch = SpreadChart.ActualFontStretch;
            C1Chart.FontStyle = SpreadChart.ActualFontStyle;
            C1Chart.FontWeight = SpreadChart.ActualFontWeight;
            Brush actualForeground = SpreadChart.ActualForeground;
            if (actualForeground != null)
            {
                C1Chart.Foreground = actualForeground;
            }
            else
            {
                C1Chart.ClearValue(Control.ForegroundProperty);
            }
            if (((SpreadChart.ChartType == SpreadChartType.Pie) || (SpreadChart.ChartType == SpreadChartType.PieDoughnut)) || ((SpreadChart.ChartType == SpreadChartType.PieExploded) || (SpreadChart.ChartType == SpreadChartType.PieExplodedDoughnut)))
            {
                PieOptions.SetStartingAngle(C1Chart, 0.0);
            }
        }

        void SyncLegend()
        {
            if (SpreadChart.Legend == null)
                return;

            ChartLegend legend = new ChartLegend();
            Brush actualFill = SpreadChart.Legend.ActualFill;
            if (actualFill != null)
            {
                legend.Background = actualFill;
            }
            else
            {
                legend.Background = new SolidColorBrush(Colors.Transparent);
            }
            Brush actualStroke = SpreadChart.Legend.ActualStroke;
            if (actualStroke != null)
            {
                legend.BorderBrush = actualStroke;
            }
            else
            {
                legend.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
            FontFamily actualFontFamily = SpreadChart.Legend.ActualFontFamily;
            if (actualFontFamily == null)
            {
                actualFontFamily = Utility.DefaultFontFamily;
            }
            legend.FontFamily = actualFontFamily;
            double num = SpreadChart.Legend.ActualFontSize * ZoomFactor;
            if (num > 0.0)
            {
                legend.FontSize = num;
            }
            legend.FontStretch = SpreadChart.Legend.ActualFontStretch;
            legend.FontStyle = SpreadChart.Legend.ActualFontStyle;
            legend.FontWeight = SpreadChart.Legend.ActualFontWeight;
            legend.Orientation = SpreadChart.Legend.Orientation;
            Brush actualForeground = SpreadChart.Legend.ActualForeground;
            if (actualForeground != null)
            {
                legend.Foreground = actualForeground;
            }
            else
            {
                legend.ClearValue(Control.ForegroundProperty);
            }
            switch (SpreadChart.Legend.Alignment)
            {
                case LegendAlignment.TopLeft:
                    legend.Position = Dt.Charts.LegendPosition.TopLeft;
                    break;

                case LegendAlignment.TopRight:
                    legend.Position = Dt.Charts.LegendPosition.TopRight;
                    break;

                case LegendAlignment.TopCenter:
                    legend.Position = Dt.Charts.LegendPosition.TopCenter;
                    break;

                case LegendAlignment.MiddleLeft:
                    legend.Position = Dt.Charts.LegendPosition.Left;
                    break;

                case LegendAlignment.MiddleRight:
                    legend.Position = Dt.Charts.LegendPosition.Right;
                    break;

                case LegendAlignment.BottomLeft:
                    legend.Position = Dt.Charts.LegendPosition.BottomLeft;
                    break;

                case LegendAlignment.BottomCenter:
                    legend.Position = Dt.Charts.LegendPosition.BottomCenter;
                    break;

                case LegendAlignment.BottomRight:
                    legend.Position = Dt.Charts.LegendPosition.BottomRight;
                    break;
            }
            string text = SpreadChart.Legend.Text;
            if (!string.IsNullOrEmpty(text))
            {
                legend.Title = text;
            }
            int num2 = -1;
            for (int i = 0; i < C1Chart.Children.Count; i++)
            {
                if (C1Chart.Children[i] is ChartLegend)
                {
                    num2 = i;
                }
            }
            if (num2 >= 0)
            {
                C1Chart.Children.RemoveAt(num2);
                C1Chart.Children.Insert(num2, legend);
            }
            else
            {
                C1Chart.Children.Add(legend);
            }
        }

        void SyncPlotArea()
        {
            ChartView view = C1Chart.View;
            Dt.Cells.Data.PlotArea plotArea = SpreadChart.PlotArea;
            Windows.UI.Xaml.Shapes.Shape plotShape = C1Chart.View.PlotShape;
            if (plotShape != null)
            {
                Brush actualFill = plotArea.ActualFill;
                if (actualFill != null)
                {
                    plotShape.Fill = actualFill;
                }
                else
                {
                    plotShape.Fill = new SolidColorBrush(Colors.Transparent);
                }
                Brush actualStroke = plotArea.ActualStroke;
                if (actualStroke != null)
                {
                    plotShape.Stroke = actualStroke;
                }
                else
                {
                    plotShape.Stroke = new SolidColorBrush(Colors.Transparent);
                }
                if (plotShape != null)
                {
                    plotShape.StrokeDashArray = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(SpreadChart.PlotArea.StrokeDashType);
                }
                double strokeThickness = plotArea.StrokeThickness;
                if (strokeThickness > 0.0)
                {
                    plotShape.StrokeThickness = strokeThickness * ZoomFactor;
                }
                else
                {
                    plotShape.StrokeThickness = 0.0;
                }
            }
        }

        void SyncAxises()
        {
            if (SpreadChart.AxisX != null)
            {
                SyncAxis(SpreadChart.AxisX, C1Chart.View.AxisX, AxisXYZType.AxisX);
            }
            if (SpreadChart.AxisY != null)
            {
                SyncAxis(SpreadChart.AxisY, C1Chart.View.AxisY, AxisXYZType.AxisY);
            }
            SyncAxisTile();
        }

        void SyncDataSeries()
        {
            ChartData data = C1Chart.Data;
            data.Children.Clear();

            DataSeries[] seriesArray = GenerateC1DataSeries();
            if ((seriesArray != null) && (seriesArray.Length != 0))
            {
                IList<SpreadDataSeries> displayingDataSeries = GetDisplayingDataSeries();
                _cachedDataSeries.Clear();
                _cachedTotalDataPointCount = 0;
                for (int i = 0; i < displayingDataSeries.Count; i++)
                {
                    SpreadDataSeries spreadDataSeries = displayingDataSeries[i];
                    DataSeries series2 = seriesArray[i];
                    if (spreadDataSeries is SpreadOpenHighLowCloseSeries)
                    {
                        SpreadOpenHighLowCloseSeries spOHLCSeries = spreadDataSeries as SpreadOpenHighLowCloseSeries;
                        SyncOpenHighLowCloseSeries(seriesArray, spOHLCSeries);
                        i = displayingDataSeries.Count - 1;
                    }
                    else
                    {
                        SyncDataSeriesValues(spreadDataSeries, series2);
                        SyncDataSeriesProperties(spreadDataSeries, series2);
                    }
                    CacheDataSeriesData(spreadDataSeries, series2);
                    series2.PlotElementLoaded += new EventHandler(C1DataSeries_PlotElementLoaded);
                }
                if (SpreadChartUtility.IsPieChart(SpreadChart.ChartType))
                {
                    if (seriesArray.Length > 0)
                    {
                        data.Children.Add(seriesArray[0]);
                    }
                }
                else
                {
                    for (int j = 0; j < seriesArray.Length; j++)
                    {
                        data.Children.Add(seriesArray[j]);
                    }
                    C1Chart.Aggregate = Aggregate.None;
                }
                SyncPieChartPaletteColor();
            }
        }

        void BindingItems(Dt.Charts.Axis c1Axis, Dt.Cells.Data.Axis axis, List<AxisAnnotation> labels)
        {
            c1Axis.ItemsSource = (IEnumerable)labels;
            Binding binding = new Binding();
            PropertyPath path = new PropertyPath("Label");
            binding.Path = path;
            Binding binding2 = new Binding();
            PropertyPath path2 = new PropertyPath("Value");
            binding2.Path = path2;
            c1Axis.ItemsLabelBinding = binding;
            c1Axis.ItemsValueBinding = binding2;
        }

        void C1DataSeries_PlotElementLoaded(object sender, EventArgs e)
        {
            PlotElement plotElement = sender as PlotElement;
            SpreadDataSeries dataSeries = null;
            if (_cachedDataSeries.TryGetValue(plotElement.DataPoint.SeriesIndex, out dataSeries))
            {
                if (dataSeries is SpreadOpenHighLowCloseSeries)
                {
                    return;
                }
                if (SpreadChartUtility.IsSymbolChart(dataSeries.ChartType))
                {
                    SyncDataPoint(plotElement, dataSeries);
                }
                else if (SpreadChartUtility.IsLineOrLineWithMarkerChart(dataSeries.ChartType))
                {
                    SyncLineChartPlotElement(plotElement, dataSeries);
                }
                else if (dataSeries.ChartType == SpreadChartType.StockHighLowOpenClose)
                {
                    SyncDataMarker(plotElement, dataSeries);
                }
            }
            if (NeedTransFormChart())
            {
                if (SpreadChartUtility.IsBarChart(SpreadChart.ChartType))
                {
                    ScaleTransform transform = new ScaleTransform();
                    transform.ScaleY = 0.67;
                    plotElement.RenderTransform = transform;
                    plotElement.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                else
                {
                    ScaleTransform transform2 = new ScaleTransform();
                    transform2.ScaleX = 0.67;
                    plotElement.RenderTransform = transform2;
                    plotElement.RenderTransformOrigin = new Point(0.5, 0.5);
                }
            }
        }

        void CacheDataSeriesData(SpreadDataSeries spDataSeries, DataSeries c1DataSeries)
        {
            if (SpreadChart.ChartType == SpreadChartType.StockHighLowOpenClose)
            {
                CachVOHLCDataSeries(_cachedDataSeries, spDataSeries as SpreadOpenHighLowCloseSeries);
            }
            else if (SpreadChartUtility.IsBarChart(spDataSeries.ChartType) || SpreadChartUtility.IsColumnChart(spDataSeries.ChartType))
            {
                if (((spDataSeries.ChartType == SpreadChartType.ColumnStacked) || (spDataSeries.ChartType == SpreadChartType.ColumnStacked100pc)) || ((spDataSeries.ChartType == SpreadChartType.BarStacked) || (spDataSeries.ChartType == SpreadChartType.BarStacked100pc)))
                {
                    if (c1DataSeries.ValuesSource != null)
                    {
                        foreach (object obj2 in c1DataSeries.ValuesSource)
                        {
                            if (obj2 is double)
                            {
                                double d = (double)((double)obj2);
                                if (!double.IsNaN(d) && (d != 0.0))
                                {
                                    _cachedTotalDataPointCount++;
                                }
                            }
                        }
                    }
                }
                else if (c1DataSeries.ValuesSource != null)
                {
                    foreach (double ble in c1DataSeries.ValuesSource)
                    {
                        _cachedTotalDataPointCount++;
                    }
                }
                int index = GetDisplayingDataSeries().IndexOf(spDataSeries);
                if ((index != -1) && !_cachedDataSeries.ContainsKey(index))
                {
                    _cachedDataSeries.Add(index, spDataSeries);
                }
            }
        }

        void CachVOHLCDataSeries(Dictionary<int, SpreadDataSeries> cachedDataSeries, SpreadOpenHighLowCloseSeries spDataSeries)
        {
            int index = GetDisplayingDataSeries().IndexOf(spDataSeries);
            if (index >= 0)
            {
                if (!cachedDataSeries.ContainsKey(index))
                {
                    cachedDataSeries.Add(index, spDataSeries);
                }
                if (!cachedDataSeries.ContainsKey(index + 1))
                {
                    cachedDataSeries.Add(index + 1, spDataSeries.OpenSeries);
                }
                if (!cachedDataSeries.ContainsKey(index + 2))
                {
                    cachedDataSeries.Add(index + 2, spDataSeries.HighSeries);
                }
                if (!cachedDataSeries.ContainsKey(index + 3))
                {
                    cachedDataSeries.Add(index + 3, spDataSeries.LowSeries);
                }
                if (!cachedDataSeries.ContainsKey(index + 4))
                {
                    cachedDataSeries.Add(index + 4, spDataSeries.CloseSeries);
                }
            }
        }

        double CalculateScale(double lineLength, int dataSeriseCount, int dataPointCount, double gapWidth, double plotElementWidth)
        {
            double num = 1.0;
            if (plotElementWidth > 0.0)
            {
                double num2 = lineLength / ((double)(dataSeriseCount + 1));
                double num3 = num2 / (dataPointCount + gapWidth);
                num = num3 / plotElementWidth;
            }
            return num;
        }

        bool ContainsNaN(IEnumerable<double> values)
        {
            foreach (double num in values)
            {
                if (double.IsNaN(num))
                {
                    return true;
                }
            }
            return false;
        }

        DataTemplate CreateAnnoTemplate(Dt.Cells.Data.Axis axis, Dt.Charts.Axis c1Axis, AxisAnnotation[] annotations, AxisXYZType xyzType)
        {
            double minWidth;
            double minHeight;
            if ((annotations == null) || (annotations.Length == 0))
            {
                minWidth = 0.0;
                minHeight = 0.0;
            }
            else
            {
                minWidth = double.MinValue;
                minHeight = double.MinValue;
            }

            FontFamily actualFontFamily = axis.ActualFontFamily;
            if (actualFontFamily == null)
                actualFontFamily = Utility.DefaultFontFamily;
            double fontSize = axis.ActualFontSize * ZoomFactor;
            if (fontSize < 0.0)
                fontSize = 15;

            if ((annotations != null) && (annotations.Length > 0))
            {
                foreach (AxisAnnotation annotation in annotations)
                {
                    Size size = MeasureHelper.MeasureText(
                        annotation.Label,
                        actualFontFamily,
                        fontSize,
                        axis.ActualFontStretch,
                        axis.ActualFontStyle,
                        axis.ActualFontWeight,
                        new Size(double.PositiveInfinity, double.PositiveInfinity),
                        true,
                        null,
                        true,
                        1.0);
                    minWidth = Math.Round(Math.Max(minWidth, size.Width), 1);
                    minHeight = Math.Round(Math.Max(minHeight, size.Height), 1);
                }
            }

            StringBuilder sb = new StringBuilder("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
            sb.Append("<Border ");
            sb.AppendFormat("Width=\"{0}\" ", minWidth + 5.0);
            sb.AppendFormat("Height=\"{0}\" ", minHeight + 5.0);
            string brush = GetBrushXamlContent(axis.ActualFill);
            if (!string.IsNullOrEmpty(brush))
                sb.AppendFormat("Background=\"{0}\" ", brush);
            sb.Append(">");

            sb.Append("<TextBlock Text=\"{Binding}\" ");
            if (xyzType == AxisXYZType.AxisX)
                sb.Append("TextAlignment=\"Center\" ");
            else if (axis.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
                sb.Append("TextAlignment=\"Right\" ");
            else
                sb.Append("TextAlignment=\"Left\" ");
            sb.Append("VerticalAlignment=\"Center\" ");
            sb.AppendFormat("FontFamily=\"{0}\" ", actualFontFamily.Source);
            sb.AppendFormat("FontSize=\"{0}\" ", fontSize);
            sb.AppendFormat("FontStretch=\"{0}\" ", axis.ActualFontStretch);
            sb.AppendFormat("FontStyle=\"{0}\" ", axis.ActualFontStyle);
            sb.AppendFormat("FontWeight=\"{0}\" ", Utility.GetFontWeightString(axis.ActualFontWeight));
            brush = GetBrushXamlContent(axis.ActualForeground);
            if (!string.IsNullOrEmpty(brush))
                sb.AppendFormat("Foreground=\"{0}\" ", brush);
            sb.Append("/>");

            sb.Append("</Border>");
            sb.Append("</DataTemplate>");

            return XamlReader.Load(sb.ToString()) as DataTemplate;
        }

        DataSeries[] CreateC1DataSeries(Type spreadDataSeriesType)
        {
            if (spreadDataSeriesType == typeof(SpreadDataSeries))
            {
                if ((!SpreadChartUtility.IsPieChart(SpreadChart.ChartType) && !SpreadChartUtility.IsRadarChart(SpreadChart.ChartType)) && IsCategoryValuesDateTime())
                {
                    return new XYDataSeries[] { new XYDataSeries() };
                }
                return new DataSeries[] { new DataSeries() };
            }
            if (spreadDataSeriesType == typeof(SpreadXYDataSeries))
            {
                return new XYDataSeries[] { new XYDataSeries() };
            }
            if (spreadDataSeriesType == typeof(SpreadBubbleSeries))
            {
                return new BubbleSeries[] { new BubbleSeries() };
            }
            if (spreadDataSeriesType == typeof(SpreadHighLowSeries))
            {
                return new List<DataSeries> { new HighLowSeries(), new XYDataSeries(), new XYDataSeries() }.ToArray();
            }
            if (spreadDataSeriesType == typeof(SpreadOpenHighLowCloseSeries))
            {
                return new List<DataSeries> { new HighLowOpenCloseSeries(), new XYDataSeries(), new XYDataSeries(), new XYDataSeries(), new XYDataSeries() }.ToArray();
            }
            return new DataSeries[1];
        }

        TransformGroup CreatTransformGroup(double scale, double offset, bool isTransFormOffset, bool isXScale)
        {
            if (double.IsInfinity(scale))
            {
                scale = 1.0;
            }
            if (double.IsNaN(scale))
            {
                scale = 1.0;
            }
            TransformGroup group = new TransformGroup();
            ScaleTransform transform = new ScaleTransform();
            if (isXScale)
            {
                transform.ScaleX = scale;
            }
            else
            {
                transform.ScaleY = scale;
            }
            group.Children.Add(transform);
            if (isTransFormOffset)
            {
                TranslateTransform transform2 = new TranslateTransform();
                if (isXScale)
                {
                    transform2.X = offset;
                }
                else
                {
                    transform2.Y = offset;
                }
                group.Children.Add(transform2);
            }
            return group;
        }

        DataSeries[] GenerateC1DataSeries()
        {
            if (SpreadChart == null)
            {
                return null;
            }
            List<DataSeries> list = new List<DataSeries>();
            IList<SpreadDataSeries> displayingDataSeries = GetDisplayingDataSeries();
            for (int i = 0; i < displayingDataSeries.Count; i++)
            {
                SpreadDataSeries series = displayingDataSeries[i];
                if (series is SpreadOpenHighLowCloseSeries)
                {
                    return CreateC1DataSeries(series.GetType());
                }
                DataSeries[] seriesArray = CreateC1DataSeries(series.GetType());
                list.AddRange(seriesArray);
            }
            return list.ToArray();
        }

        double[] GenerateValues(SpreadXYDataSeries dataSeries, double min)
        {
            if (dataSeries == null)
            {
                return null;
            }
            DoubleSeriesCollection seriess = new DoubleSeriesCollection();
            for (int i = 0; i < dataSeries.Values.Count; i++)
            {
                seriess.Add(min + i);
            }
            return seriess.ToArray();
        }

        DoubleSeriesCollection GenerateXValues(SpreadOpenHighLowCloseSeries spOHLCSeries)
        {
            DoubleSeriesCollection seriess = new DoubleSeriesCollection();
            int count = 0;
            if (((spOHLCSeries.OpenSeries != null) && (spOHLCSeries.OpenSeries.Values != null)) && (spOHLCSeries.OpenSeries.Values.Count > count))
            {
                count = spOHLCSeries.OpenSeries.Values.Count;
            }
            if (((spOHLCSeries.HighSeries != null) && (spOHLCSeries.HighSeries.Values != null)) && (spOHLCSeries.HighSeries.Values.Count > count))
            {
                count = spOHLCSeries.HighSeries.Values.Count;
            }
            if (((spOHLCSeries.LowSeries != null) && (spOHLCSeries.LowSeries.Values != null)) && (spOHLCSeries.LowSeries.Values.Count > count))
            {
                count = spOHLCSeries.LowSeries.Values.Count;
            }
            if (((spOHLCSeries.CloseSeries != null) && (spOHLCSeries.CloseSeries.Values != null)) && (spOHLCSeries.CloseSeries.Values.Count > count))
            {
                count = spOHLCSeries.CloseSeries.Values.Count;
            }
            for (int i = 0; i < count; i++)
            {
                seriess.Add((double)(i + 1));
            }
            return seriess;
        }

        List<string> GetAxisCategories(Dt.Cells.Data.Axis axis, IList<object> items)
        {
            List<string> list = new List<string>();
            foreach (object obj2 in items)
            {
                if (SpreadChart.AxisX.LabelFormatter != null)
                {
                    string str = axis.LabelFormatter.Format(obj2);
                    list.Add(str);
                }
            }
            return list;
        }

        string GetBindingText(Dt.Cells.Data.Axis axis, object item)
        {
            if (item == null)
            {
                return string.Empty;
            }
            if (axis.LabelFormatter is AutoFormatter)
            {
                if (axis.LabelFormatter.FormatString == "general")
                {
                    object obj2;
                    GeneralFormatter preferredDisplayFormatter = new GeneralFormatter().GetPreferredDisplayFormatter(item.ToString(), out obj2) as GeneralFormatter;
                    return preferredDisplayFormatter.Format(item);
                }
                return axis.LabelFormatter.Format(item);
            }
            return axis.LabelFormatter.Format(item);
        }

        string GetBrushXamlContent(Brush brush)
        {
            if (brush is SolidColorBrush sb)
                return $"#{sb.Color.A.ToString("X2")}{sb.Color.R.ToString("X2")}{sb.Color.G.ToString("X2")}{sb.Color.B.ToString("X2")}";
            return null;
        }

        internal int GetDataDimension(SpreadChartType chartType)
        {
            switch (chartType)
            {
                case SpreadChartType.BarClustered:
                case SpreadChartType.BarStacked:
                case SpreadChartType.BarStacked100pc:
                case SpreadChartType.ColumnClustered:
                case SpreadChartType.ColumnStacked:
                case SpreadChartType.ColumnStacked100pc:
                case SpreadChartType.Line:
                case SpreadChartType.LineStacked:
                case SpreadChartType.LineStacked100pc:
                case SpreadChartType.LineWithMarkers:
                case SpreadChartType.LineStackedWithMarkers:
                case SpreadChartType.LineStacked100pcWithMarkers:
                case SpreadChartType.Pie:
                case SpreadChartType.PieExploded:
                case SpreadChartType.PieDoughnut:
                case SpreadChartType.PieExplodedDoughnut:
                case SpreadChartType.Area:
                case SpreadChartType.AreaStacked:
                case SpreadChartType.AreaStacked100pc:
                case SpreadChartType.Radar:
                case SpreadChartType.RadarWithMarkers:
                case SpreadChartType.RadarFilled:
                    return 1;

                case SpreadChartType.Scatter:
                case SpreadChartType.ScatterLines:
                case SpreadChartType.ScatterLinesWithMarkers:
                case SpreadChartType.ScatterLinesSmoothed:
                case SpreadChartType.ScatterLinesSmoothedWithMarkers:
                    return 2;

                case SpreadChartType.Bubble:
                    return 3;

                case SpreadChartType.StockHighLowOpenClose:
                    return 5;
            }
            return 1;
        }

        double?[] GetDataSeriesXValues(SpreadDataSeries spreadDataSeries)
        {
            double[] itemsOADate = null;
            double maxValue = double.MaxValue;
            double minValue = double.MinValue;
            if (SpreadChartUtility.IsBarChart(SpreadChart.ChartType))
            {
                if (SpreadChart.AxisY != null)
                {
                    maxValue = SpreadChart.AxisY.Min;
                    minValue = SpreadChart.AxisY.Max;
                }
            }
            else if (SpreadChart.AxisX != null)
            {
                maxValue = SpreadChart.AxisX.Min;
                minValue = SpreadChart.AxisX.Max;
            }
            if (spreadDataSeries is SpreadXYDataSeries)
            {
                SpreadXYDataSeries dataSeries = spreadDataSeries as SpreadXYDataSeries;
                if (((dataSeries.XValues != null) && !ContainsNaN(dataSeries.XValues)) && (dataSeries.XValues.Count > 0))
                {
                    itemsOADate = dataSeries.XValues.ToArray();
                }
                else
                {
                    double min = (maxValue == double.MaxValue) ? 0.0 : maxValue;
                    itemsOADate = Enumerable.ToArray<double>(GenerateValues(dataSeries, min));
                }
            }
            else if ((spreadDataSeries != null) && IsCategoryValuesDateTime())
            {
                object[] items = null;
                if (SpreadChartUtility.IsBarChart(SpreadChart.ChartType))
                {
                    items = new AxisOriginalItemsCollection { DataSeries = new Dt.Cells.Data.Axis.ItemsDataSeries(SpreadChart.AxisY) }.ToArray();
                }
                else
                {
                    object[] objArray2 = null;
                    if (SpreadChart.AxisX.UseCustomItems)
                    {
                        objArray2 = SpreadChart.AxisX.Items.ToArray();
                    }
                    else if (!string.IsNullOrEmpty(SpreadChart.AxisX.ItemsFormula))
                    {
                        objArray2 = new AxisOriginalItemsCollection { DataSeries = new Dt.Cells.Data.Axis.ItemsDataSeries(SpreadChart.AxisX) }.ToArray();
                    }
                    items = objArray2;
                }
                if ((items != null) && (items.Length > 0))
                {
                    itemsOADate = GetItemsOADate(items);
                }
            }
            if ((itemsOADate == null) || (itemsOADate.Length <= 0))
            {
                return null;
            }
            List<double?> list = new List<double?>();
            for (int i = 0; i < itemsOADate.Length; i++)
            {
                if ((itemsOADate[i] >= maxValue) && (itemsOADate[i] <= minValue))
                {
                    list.Add(new double?(itemsOADate[i]));
                }
                else
                {
                    double? nullable = null;
                    list.Add(nullable);
                }
            }
            return list.ToArray();
        }

        double[] GetDataSeriesYValues(SpreadDataSeries spreadDataSeries)
        {
            SpreadChartType chartType = (spreadDataSeries != null) ? spreadDataSeries.ChartType : SpreadChart.ChartType;
            if (!SpreadChartUtility.IsColumnChart(chartType) && !SpreadChartUtility.IsBarChart(chartType))
            {
                return spreadDataSeries.Values.ToArray();
            }
            List<double> list = new List<double>();
            for (int i = 0; i < spreadDataSeries.Values.Count; i++)
            {
                list.Add(spreadDataSeries.Values[i]);
            }
            return list.ToArray();
        }

        IList<SpreadDataSeries> GetDisplayingDataSeries()
        {
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            List<SpreadDataSeries> list2 = new List<SpreadDataSeries>();
            foreach (SpreadDataSeries series in SpreadChart.DataSeries)
            {
                if (!series.IsHidden)
                {
                    if (SpreadChartUtility.IsLineOrLineWithMarkerChart(series.ChartType))
                    {
                        list2.Add(series);
                    }
                    else
                    {
                        list.Add(series);
                    }
                }
            }
            if (list2.Count > 0)
            {
                list.AddRange((IEnumerable<SpreadDataSeries>)list2);
            }
            return (IList<SpreadDataSeries>)list;
        }

        string[] GetDisplayItems(Dt.Cells.Data.Axis axis)
        {
            List<string> list = new List<string>();
            if ((axis.Items != null) && (axis.Items.Count > 0))
            {
                for (int i = 0; i < axis.Items.Count; i++)
                {
                    object item = axis.Items[i];
                    string str = axis.ShowAxisLabel ? GetBindingText(axis, item) : string.Empty;
                    list.Add(str);
                }
            }
            return list.ToArray();
        }

        double GetGapWidth()
        {
            return 1.5;
        }

        double[] GetItemsOADate(IEnumerable<object> items)
        {
            List<double> list = new List<double>();
            foreach (object obj2 in items)
            {
                double? nullable = TryDouble(obj2, true);
                if (nullable.HasValue)
                {
                    list.Add(nullable.Value);
                }
            }
            return list.ToArray();
        }

        int GetMaxDataPointCount()
        {
            if (((SpreadChart.ChartType != SpreadChartType.ColumnStacked) && (SpreadChart.ChartType != SpreadChartType.ColumnStacked100pc)) && ((SpreadChart.ChartType != SpreadChartType.BarStacked) && (SpreadChart.ChartType != SpreadChartType.BarStacked100pc)))
            {
                return _cachedDataSeries.Count;
            }
            return 1;
        }

        double GetOffset(int currentIndex, int previousIndex, int startIndex, int endIndex, double[] scaleOffset, double[] offsetArray)
        {
            double num = 0.0;
            double num2 = 0.0;
            if ((currentIndex >= startIndex) && (currentIndex <= endIndex))
            {
                num = scaleOffset[currentIndex];
            }
            if ((previousIndex >= startIndex) && (previousIndex <= endIndex))
            {
                num2 = scaleOffset[previousIndex];
            }
            double num3 = num + num2;
            if ((previousIndex >= startIndex) && (previousIndex <= endIndex))
            {
                offsetArray[currentIndex] = offsetArray[previousIndex] + num3;
            }
            else
            {
                offsetArray[currentIndex] = num3;
            }
            return offsetArray[currentIndex];
        }

        double[] GetPointValuesSum()
        {
            Dictionary<int, double> dictionary = new Dictionary<int, double>();
            IList<SpreadDataSeries> displayingDataSeries = GetDisplayingDataSeries();
            for (int i = 0; i < displayingDataSeries.Count; i++)
            {
                SpreadDataSeries series = displayingDataSeries[i];
                for (int j = 0; j < series.Values.Count; j++)
                {
                    if (!dictionary.ContainsKey(j))
                    {
                        if (Utility.IsNumber(series.Values[j]))
                        {
                            dictionary[j] = Math.Abs(series.Values[j]);
                        }
                        else
                        {
                            dictionary[j] = 0.0;
                        }
                    }
                    else if (Utility.IsNumber(series.Values[j]))
                    {
                        Dictionary<int, double> dictionary2;
                        int num3;
                        (dictionary2 = dictionary)[num3 = j] = dictionary2[num3] + Math.Abs(series.Values[j]);
                    }
                }
            }
            return Enumerable.ToArray<double>((IEnumerable<double>)dictionary.Values);
        }

        bool HasTwoDimensionSeries()
        {
            using (IEnumerator<SpreadDataSeries> enumerator = GetDisplayingDataSeries().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is SpreadXYDataSeries)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool IsCategoryValuesDateTime()
        {
            if (SpreadChartUtility.IsBarChart(SpreadChart.ChartType))
            {
                return (SpreadChart.AxisY.AxisType == Dt.Cells.Data.AxisType.Date);
            }
            return (SpreadChart.AxisX.AxisType == Dt.Cells.Data.AxisType.Date);
        }

        bool IsNumber(object value)
        {
            return ((((((value is double) || (value is float)) || ((value is decimal) || (value is long))) || (((value is int) || (value is short)) || ((value is sbyte) || (value is ulong)))) || (((value is uint) || (value is ushort)) || ((value is byte) || (value is DateTime)))) || (value is TimeSpan));
        }

        bool IsZero(double value)
        {
            return ((!double.IsNaN(value) && !double.IsInfinity(value)) && (Math.Abs((double)(value - 0.0)) < 1E-07));
        }

        double[] LimitDataSeriesValues(double?[] xVlues, double[] values)
        {
            List<double> list = new List<double>();
            if (xVlues != null)
            {
                for (int i = 0; i < xVlues.Length; i++)
                {
                    if (i >= values.Length)
                    {
                        break;
                    }
                    if (xVlues[i].HasValue)
                    {
                        list.Add(values[i]);
                    }
                }
            }
            return list.ToArray();
        }

        bool NeedInflateDateTimeAxis()
        {
            if ((((SpreadChart.ChartType != SpreadChartType.StockHighLowOpenClose) && (SpreadChart.ChartType != SpreadChartType.ColumnClustered)) && ((SpreadChart.ChartType != SpreadChartType.ColumnStacked) && (SpreadChart.ChartType != SpreadChartType.ColumnStacked100pc))) && ((SpreadChart.ChartType != SpreadChartType.BarClustered) && (SpreadChart.ChartType != SpreadChartType.BarStacked)))
            {
                return (SpreadChart.ChartType == SpreadChartType.BarStacked100pc);
            }
            return true;
        }

        bool NeedTransFormChart()
        {
            if (!SpreadChartUtility.IsColumnChart(SpreadChart.ChartType) && !SpreadChartUtility.IsBarChart(SpreadChart.ChartType))
            {
                return false;
            }
            return (((SpreadChart.ChartType != SpreadChartType.ColumnClustered) && (SpreadChart.ChartType != SpreadChartType.BarClustered)) || (SpreadChart.DataSeries.Count == 1));
        }

        internal override void RefreshAxises()
        {
            if (C1Chart != null)
            {
                C1Chart.BeginUpdate();
                SyncAxises();
                C1Chart.EndUpdate();
            }
        }

        internal override void RefreshAxisesTitle()
        {
            if (C1Chart != null)
            {
                C1Chart.BeginUpdate();
                SyncAxisTile();
                C1Chart.EndUpdate();
            }
        }

        internal override void RefreshAxisX()
        {
            if (C1Chart != null)
            {
                C1Chart.BeginUpdate();
                SyncAxis(SpreadChart.AxisX, C1Chart.View.AxisX, AxisXYZType.AxisX);
                C1Chart.EndUpdate();
            }
        }

        internal override void RefreshAxisY()
        {
            if (C1Chart != null)
            {
                C1Chart.BeginUpdate();
                SyncAxis(SpreadChart.AxisY, C1Chart.View.AxisY, AxisXYZType.AxisY);
                C1Chart.EndUpdate();
            }
        }

        internal override void RefreshChartArea()
        {
            if (C1Chart != null)
            {
                C1Chart.BeginUpdate();
                SyncChartArea();
                C1Chart.EndUpdate();
            }
        }

        internal override void RefreshChartContent()
        {
            SyncSpreadChartToC1Chart();
        }

        internal override void RefreshChartLegend()
        {
            if (C1Chart != null)
            {
                C1Chart.BeginUpdate();
                SyncLegend();
                C1Chart.EndUpdate();
            }
        }

        internal override void RefreshChartTitle()
        {
            if (C1Chart != null)
            {
                C1Chart.BeginUpdate();
                SyncChartTitle();
                C1Chart.EndUpdate();
            }
        }

        internal override void RefreshDataSeries()
        {
            if (C1Chart != null)
            {
                C1Chart.BeginUpdate();
                SyncDataSeries();
                C1Chart.EndUpdate();
            }
        }

        internal override void RefreshPlotArea()
        {
            if (C1Chart != null)
            {
                C1Chart.BeginUpdate();
                SyncPlotArea();
                C1Chart.EndUpdate();
            }
        }

        ChartType SpreadChartTypeToC1ChartType(SpreadChartType spChartType)
        {
            switch (spChartType)
            {
                case SpreadChartType.BarClustered:
                    return ChartType.Bar;

                case SpreadChartType.BarStacked:
                    return ChartType.BarStacked;

                case SpreadChartType.BarStacked100pc:
                    return ChartType.BarStacked;

                case SpreadChartType.ColumnClustered:
                    return ChartType.Column;

                case SpreadChartType.ColumnStacked:
                    return ChartType.ColumnStacked;

                case SpreadChartType.ColumnStacked100pc:
                    return ChartType.ColumnStacked;

                case SpreadChartType.Line:
                    return ChartType.Line;

                case SpreadChartType.LineSmoothed:
                    return ChartType.LineSmoothed;

                case SpreadChartType.LineStacked:
                    return ChartType.LineStacked;

                case SpreadChartType.LineStacked100pc:
                    return ChartType.LineStacked;

                case SpreadChartType.LineWithMarkers:
                    return ChartType.LineSymbols;

                case SpreadChartType.LineWithMarkersSmoothed:
                    return ChartType.LineSymbolsSmoothed;

                case SpreadChartType.LineStackedWithMarkers:
                    return ChartType.LineSymbolsStacked;

                case SpreadChartType.LineStacked100pcWithMarkers:
                    return ChartType.LineSymbolsStacked;

                case SpreadChartType.Pie:
                    return ChartType.Pie;

                case SpreadChartType.PieExploded:
                    return ChartType.PieExploded;

                case SpreadChartType.PieDoughnut:
                    return ChartType.PieDoughnut;

                case SpreadChartType.PieExplodedDoughnut:
                    return ChartType.PieExplodedDoughnut;

                case SpreadChartType.Area:
                    return ChartType.Area;

                case SpreadChartType.AreaStacked:
                    return ChartType.AreaStacked;

                case SpreadChartType.AreaStacked100pc:
                    return ChartType.AreaStacked;

                case SpreadChartType.Radar:
                    return ChartType.Radar;

                case SpreadChartType.RadarWithMarkers:
                    return ChartType.RadarSymbols;

                case SpreadChartType.RadarFilled:
                    return ChartType.RadarFilled;

                case SpreadChartType.Scatter:
                    return ChartType.XYPlot;

                case SpreadChartType.ScatterLines:
                    return ChartType.Line;

                case SpreadChartType.ScatterLinesWithMarkers:
                    return ChartType.LineSymbols;

                case SpreadChartType.ScatterLinesSmoothed:
                    return ChartType.LineSmoothed;

                case SpreadChartType.ScatterLinesSmoothedWithMarkers:
                    return ChartType.LineSymbolsSmoothed;

                case SpreadChartType.Bubble:
                    return ChartType.Bubble;

                case SpreadChartType.StockHighLowOpenClose:
                    return ChartType.Candle;
            }
            return ChartType.Column;
        }

        Marker SpreadMarkerToC1Marker(Dt.Cells.Data.MarkerType spMarker)
        {
            switch (spMarker)
            {
                case Dt.Cells.Data.MarkerType.Automatic:
                    return Marker.Diamond;

                case Dt.Cells.Data.MarkerType.None:
                case Dt.Cells.Data.MarkerType.Circle:
                    return Marker.None;

                case Dt.Cells.Data.MarkerType.Box:
                    return Marker.Box;

                case Dt.Cells.Data.MarkerType.Dot:
                    return Marker.Dot;

                case Dt.Cells.Data.MarkerType.Diamond:
                    return Marker.Diamond;

                case Dt.Cells.Data.MarkerType.Triangle:
                    return Marker.Triangle;

                case Dt.Cells.Data.MarkerType.Star4:
                    return Marker.Star4;

                case Dt.Cells.Data.MarkerType.Star8:
                    return Marker.Star8;

                case Dt.Cells.Data.MarkerType.Cross:
                    return Marker.Cross;

                case Dt.Cells.Data.MarkerType.DiagonalCross:
                    return Marker.DiagonalCross;
            }
            return Marker.Diamond;
        }

        static void SwitchDataSeries(DataSeries[] c1DataSeriesList, int i)
        {
            if ((i + 5) < c1DataSeriesList.Length)
            {
                DataSeries series = c1DataSeriesList[i];
                c1DataSeriesList[i] = c1DataSeriesList[i + 5];
                c1DataSeriesList[i + 5] = series;
            }
        }

        void SyncAxis(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis, AxisXYZType axisType)
        {
            SyncAxisStyle(spAxis, c1Axis, axisType);
            SyncAxisLabel(spAxis, c1Axis, axisType);
            SyncAxisPosition(spAxis, c1Axis, axisType);
            SyncAxisLine(spAxis, c1Axis);
            SyncAxisMajorGridLine(spAxis, c1Axis, axisType);
            SyncAxisMajorTick(spAxis, c1Axis);
            SyncAxisMinorGridLine(spAxis, c1Axis);
            SyncAxisMinorTick(spAxis, c1Axis);
        }

        void SyncAxisCategoryPosition(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis, double crossAt)
        {
            c1Axis.Origin = crossAt;
        }

        void SyncAxisDatePosition(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis, double crossAt)
        {
            c1Axis.Origin = crossAt;
        }

        void SyncAxisItems(Dt.Cells.Data.Axis axis, AxisXYZType axisType)
        {
            AxisAnnotation[] annotations;
            Dt.Charts.Axis c1Axis = null;
            if (axisType == AxisXYZType.AxisX)
            {
                c1Axis = C1Chart.View.AxisX;
            }
            else if (axisType == AxisXYZType.AxisY)
            {
                c1Axis = C1Chart.View.AxisY;
            }
            else
            {
                AxisXYZType type1 = axisType;
            }
            if (axis != null)
            {
                annotations = null;
                if (axis.AxisType == Dt.Cells.Data.AxisType.Category)
                {
                    SyncCategoryItems(c1Axis, axis, out annotations);
                }
                else if (axis.AxisType == Dt.Cells.Data.AxisType.Date)
                {
                    SyncDateTimeItems(c1Axis, axis, out annotations);
                }
                else
                {
                    SyncValueItems(c1Axis, axis, axisType, out annotations);
                }
                c1Axis.AnnoTemplate = CreateAnnoTemplate(axis, c1Axis, annotations, axisType);
            }
        }

        void SyncAxisLabel(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis, AxisXYZType axisType)
        {
            c1Axis.AnnoAngle = spAxis.LabelAngle;
            SyncAxisItems(spAxis, axisType);
        }

        void SyncAxisLine(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis)
        {
            if (spAxis.UseLogBase)
            {
                c1Axis.LogBase = spAxis.LogBase;
            }
            else
            {
                c1Axis.LogBase = double.NaN;
            }
            c1Axis.AxisLine = new Windows.UI.Xaml.Shapes.Line();
            Brush actualStroke = spAxis.ActualStroke;
            if (actualStroke != null)
            {
                c1Axis.Foreground = actualStroke;
            }
            else
            {
                c1Axis.Foreground = new SolidColorBrush(Colors.Transparent);
            }
            if (spAxis.StrokeDashType != StrokeDashType.None)
            {
                c1Axis.AxisLine.StrokeDashArray = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(spAxis.StrokeDashType);
            }
            if (spAxis.StrokeThickness > 0.0)
            {
                c1Axis.AxisLine.StrokeThickness = spAxis.StrokeThickness * ZoomFactor;
            }
            c1Axis.AxisLine.StrokeStartLineCap = spAxis.LineCapType;
            c1Axis.AxisLine.StrokeEndLineCap = spAxis.LineCapType;
            c1Axis.AxisLine.StrokeLineJoin = spAxis.LineJoinType;
        }

        void SyncAxisMajorGridLine(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis, AxisXYZType axisType)
        {
            Brush actualMajorGridlinesStroke = spAxis.ActualMajorGridlinesStroke;
            if ((actualMajorGridlinesStroke != null) && spAxis.ShowMajorGridlines)
            {
                c1Axis.MajorGridStroke = actualMajorGridlinesStroke;
            }
            else
            {
                c1Axis.MajorGridStroke = new SolidColorBrush(Colors.Transparent);
            }
            DoubleCollection strokeDashes = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(spAxis.AcutalMajorGridlineStrokeDashType);
            if ((strokeDashes != null) && spAxis.ShowMajorGridlines)
            {
                c1Axis.MajorGridStrokeDashes = strokeDashes;
            }
            else
            {
                c1Axis.MajorGridStrokeDashes = null;
            }
            double actualMajorGridlinesStrokeThickness = spAxis.ActualMajorGridlinesStrokeThickness;
            if ((actualMajorGridlinesStrokeThickness > 0.0) && spAxis.ShowMajorGridlines)
            {
                c1Axis.MajorGridStrokeThickness = actualMajorGridlinesStrokeThickness * ZoomFactor;
            }
            else
            {
                c1Axis.MajorGridStrokeThickness = 0.0;
            }
        }

        void SyncAxisMajorTick(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis)
        {
            if (!double.IsNaN(spAxis.MajorTickHeight))
            {
                c1Axis.MajorTickHeight = spAxis.MajorTickHeight;
            }
            switch (spAxis.MajorTickPosition)
            {
                case AxisTickPosition.None:
                    c1Axis.MajorTickHeight = 0.0;
                    break;

                case AxisTickPosition.Inside:
                    c1Axis.MajorTickOverlap = 1.0;
                    break;

                case AxisTickPosition.OutSide:
                    c1Axis.MajorTickOverlap = 0.0;
                    break;

                case AxisTickPosition.Cross:
                    c1Axis.MajorTickOverlap = 0.5;
                    break;
            }
            if (spAxis.MajorTickStroke != null)
            {
                c1Axis.MajorTickStroke = spAxis.MajorTickStroke;
            }
            else
            {
                c1Axis.MajorTickStroke = new SolidColorBrush(Colors.Transparent);
            }
            if (spAxis.MajorTickThickness > 0.0)
            {
                c1Axis.MajorTickThickness = spAxis.MajorTickThickness * ZoomFactor;
            }
        }

        void SyncAxisMinorGridLine(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis)
        {
            Brush actualMinorGridlinesStroke = spAxis.ActualMinorGridlinesStroke;
            if ((actualMinorGridlinesStroke != null) && spAxis.ShowMinorGridlines)
            {
                c1Axis.MinorGridStroke = actualMinorGridlinesStroke;
            }
            else
            {
                c1Axis.MinorGridStroke = new SolidColorBrush(Colors.Transparent);
            }
            DoubleCollection strokeDashes = StrokeDashHelper.GetStrokeDashes(spAxis.ActualMinorGridlineStrokeDashType);
            if ((strokeDashes != null) && spAxis.ShowMinorGridlines)
            {
                c1Axis.MinorGridStrokeDashes = strokeDashes;
            }
            else
            {
                c1Axis.MinorGridStrokeDashes = null;
            }
            double actualMinorGridlinesStrokeThickness = spAxis.ActualMinorGridlinesStrokeThickness;
            if ((actualMinorGridlinesStrokeThickness > 0.0) && spAxis.ShowMinorGridlines)
            {
                c1Axis.MinorGridStrokeThickness = actualMinorGridlinesStrokeThickness * ZoomFactor;
            }
            else
            {
                c1Axis.MinorGridStrokeThickness = 0.0;
            }
        }

        void SyncAxisMinorTick(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis)
        {
            if (!double.IsNaN(spAxis.MinorTickHeight))
            {
                c1Axis.MinorTickHeight = spAxis.MinorTickHeight;
            }
            switch (spAxis.MinorTickPosition)
            {
                case AxisTickPosition.None:
                    c1Axis.MinorTickHeight = 0.0;
                    break;

                case AxisTickPosition.Inside:
                    c1Axis.MinorTickOverlap = 1.0;
                    break;

                case AxisTickPosition.OutSide:
                    c1Axis.MinorTickOverlap = 0.0;
                    break;

                case AxisTickPosition.Cross:
                    c1Axis.MinorTickOverlap = 0.5;
                    break;
            }
            if (spAxis.MinorTickStroke != null)
            {
                c1Axis.MinorTickStroke = spAxis.MinorTickStroke;
            }
            else
            {
                c1Axis.MinorTickStroke = new SolidColorBrush(Colors.Transparent);
            }
            if (spAxis.MinorTickThickness > 0.0)
            {
                c1Axis.MinorTickThickness = spAxis.MinorTickThickness * ZoomFactor;
            }
            else
            {
                c1Axis.MinorTickThickness = 0.0;
            }
        }

        void SyncAxisPosition(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis, AxisXYZType axisType)
        {
            if (SpreadChartUtility.IsRadarChart(SpreadChart.ChartType))
            {
                c1Axis.Position = Dt.Charts.AxisPosition.OverData;
            }
            else
            {
                if (spAxis.AxisPosition == Dt.Cells.Data.AxisPosition.Far)
                {
                    c1Axis.Position = Dt.Charts.AxisPosition.Far;
                }
                else if (spAxis.AxisPosition == Dt.Cells.Data.AxisPosition.Near)
                {
                    c1Axis.Position = Dt.Charts.AxisPosition.Near;
                }
                if (spAxis.OverlapData)
                {
                    c1Axis.Position |= Dt.Charts.AxisPosition.OverData;
                }
                c1Axis.Origin = spAxis.CrossAt;
            }
        }

        void SyncAxisStyle(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis, AxisXYZType axisType)
        {
            FontFamily actualFontFamily = spAxis.ActualFontFamily;
            if (actualFontFamily != null)
            {
                actualFontFamily = Utility.DefaultFontFamily;
            }
            c1Axis.FontFamily = actualFontFamily;
            double num = spAxis.ActualFontSize * ZoomFactor;
            if (num > 0.0)
            {
                c1Axis.FontSize = num;
            }
            c1Axis.Reversed = spAxis.Reversed;
            c1Axis.Visible = spAxis.Visible;
        }

        void SyncAxisTile()
        {
            if (SpreadChart.AxisX != null)
            {
                SyncAxisTitle(SpreadChart.AxisX, C1Chart.View.AxisX, Dt.Charts.AxisType.X);
            }
            if (SpreadChart.AxisY != null)
            {
                SyncAxisTitle(SpreadChart.AxisY, C1Chart.View.AxisY, Dt.Charts.AxisType.Y);
            }
        }

        void SyncAxisTitle(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis, Dt.Charts.AxisType axisType)
        {
            if (spAxis.Title != null)
            {
                ChartTitleView view = new ChartTitleView(spAxis.Title, this);
                Size chartTitleSize = GetChartTitleSize(spAxis.Title);
                view.HorizontalAlignment = HorizontalAlignment.Center;
                view.VerticalAlignment = VerticalAlignment.Center;
                view.Width = chartTitleSize.Width;
                view.Height = chartTitleSize.Height;
                if (axisType == Dt.Charts.AxisType.Y)
                {
                    view.MaxWidth = SpreadChart.Size.Height;
                    view.MaxHeight = SpreadChart.Size.Width;
                }
                else
                {
                    view.MaxWidth = SpreadChart.Size.Width;
                    view.MaxHeight = SpreadChart.Size.Height;
                }
                c1Axis.Title = view;
            }
            else
            {
                c1Axis.Title = null;
            }
        }

        void SyncAxisValuePosition(Dt.Cells.Data.Axis spAxis, Dt.Charts.Axis c1Axis, double crossAt)
        {
            c1Axis.Origin = crossAt;
        }

        void SyncCategoryItems(Dt.Charts.Axis c1Axis, Dt.Cells.Data.Axis axis, out AxisAnnotation[] annotations)
        {
            if (SpreadChartUtility.IsPieChart(SpreadChart.ChartType))
            {
                string[] displayItems = GetDisplayItems(axis);
                C1Chart.Data.ItemNames = displayItems;
                annotations = new List<AxisAnnotation>().ToArray();
            }
            else if (SpreadChartUtility.IsRadarChart(SpreadChart.ChartType))
            {
                string[] strArray2 = GetDisplayItems(axis);
                int count = 0;
                if (SpreadChart.DataSeries != null)
                {
                    foreach (SpreadDataSeries series in SpreadChart.DataSeries)
                    {
                        if (series.Values.Count > count)
                        {
                            count = series.Values.Count;
                        }
                    }
                }
                if (count > strArray2.Length)
                {
                    int num2 = count - strArray2.Length;
                    List<string> list = Enumerable.ToList<string>(strArray2);
                    for (int i = 0; i < num2; i++)
                    {
                        list.Add(string.Empty);
                    }
                    strArray2 = list.ToArray();
                }
                C1Chart.Data.ItemNames = strArray2;
                annotations = new List<AxisAnnotation>().ToArray();
            }
            else
            {
                List<AxisAnnotation> labels = new List<AxisAnnotation>();
                double min = axis.Min;
                if (!double.IsNaN(min))
                {
                    for (int j = 0; min <= axis.Max; j++)
                    {
                        if (j < axis.Items.Count)
                        {
                            object item = axis.Items[j];
                            string str = axis.ShowAxisLabel ? GetBindingText(axis, item) : string.Empty;
                            AxisAnnotation annotation = new AxisAnnotation
                            {
                                Label = str,
                                Value = (double)min
                            };
                            labels.Add(annotation);
                        }
                        min += axis.MajorUnit;
                    }
                }
                annotations = labels.ToArray();
                BindingItems(c1Axis, axis, labels);
                SyncMinAndMax(c1Axis, axis);
            }
        }

        void SyncDataMarker(PlotElement plotElement, SpreadDataSeries dataSeries)
        {
            if ((plotElement != null) && (plotElement.DataPoint != null))
            {
                DataMarker dataMarker = dataSeries.GetDataMarker(plotElement.DataPoint.PointIndex);
                if (dataMarker != null)
                {
                    if ((dataMarker.ActualMarkerSize != Size.Empty) && (dataMarker.MarkerType != Dt.Cells.Data.MarkerType.None))
                    {
                        plotElement.Size = dataMarker.ActualMarkerSize;
                    }
                    else
                    {
                        plotElement.Size = new Size(0.0, 0.0);
                    }
                    Brush actualFill = dataMarker.ActualFill;
                    if (actualFill != null)
                    {
                        plotElement.Fill = actualFill;
                    }
                    Brush actualStroke = dataMarker.ActualStroke;
                    if (actualStroke != null)
                    {
                        plotElement.Stroke = actualStroke;
                    }
                    double num = dataMarker.ActualStrokeThickness * ZoomFactor;
                    if (num > 0.0)
                    {
                        plotElement.StrokeThickness = num;
                    }
                    else
                    {
                        plotElement.StrokeThickness = 0.0;
                    }
                    DoubleCollection strokeDashes = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(dataMarker.ActualStrokeDashType);
                    if (strokeDashes != null)
                    {
                        plotElement.StrokeDashArray = strokeDashes;
                    }
                }
            }
        }

        void SyncDataPoint(PlotElement plotElement, SpreadDataSeries dataSeries)
        {
            if ((plotElement != null) && (plotElement.DataPoint != null))
            {
                Dt.Cells.Data.DataPoint dataPoint = dataSeries.GetDataPoint(plotElement.DataPoint.PointIndex);
                if (dataPoint != null)
                {
                    Brush actualFill = dataPoint.ActualFill;
                    Brush actualNegativeFill = dataPoint.ActualNegativeFill;
                    double? nullable = dataPoint.Value;
                    if ((((double)nullable.GetValueOrDefault()) > 0.0) && nullable.HasValue)
                    {
                        if (actualFill != null)
                        {
                            plotElement.Fill = actualFill;
                        }
                    }
                    else if (dataPoint.ActualInvertIfNegative)
                    {
                        if (actualNegativeFill != null)
                        {
                            plotElement.Fill = actualNegativeFill;
                        }
                        else
                        {
                            plotElement.Fill = new SolidColorBrush(Colors.Transparent);
                        }
                    }
                    else if (actualFill != null)
                    {
                        plotElement.Fill = actualFill;
                    }
                    double num = dataPoint.ActualStrokeThickness * ZoomFactor;
                    if (num > 0.0)
                    {
                        plotElement.StrokeThickness = num;
                    }
                    else
                    {
                        plotElement.StrokeThickness = 0.0;
                    }
                    Brush actualStroke = dataPoint.ActualStroke;
                    if (actualStroke != null)
                    {
                        plotElement.Stroke = actualStroke;
                    }
                    else
                    {
                        plotElement.Stroke = new SolidColorBrush(Colors.Transparent);
                    }
                    plotElement.StrokeDashArray = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(dataPoint.ActualStrokeDashType);
                }
            }
        }

        void SyncDataSeriesChartType(SpreadDataSeries spDataSeries, DataSeries c1DataSeries)
        {
            if (spDataSeries.ChartType != SpreadChartType.None)
            {
                c1DataSeries.ChartType = new ChartType?(SpreadChartTypeToC1ChartType(spDataSeries.ChartType));
            }
            else
            {
                c1DataSeries.ChartType = null;
            }
        }

        void SyncDataSeriesProperties(SpreadDataSeries spDataSeries, DataSeries c1DataSeries)
        {
            SyncDataSeriesChartType(spDataSeries, c1DataSeries);
            SyncDataSeriesSettings(spDataSeries, c1DataSeries);
        }

        void SyncDataSeriesSettings(SpreadDataSeries spDataSeries, DataSeries c1DataSeries)
        {
            SpreadChartType chartType = (spDataSeries != null) ? spDataSeries.ChartType : SpreadChart.ChartType;
            if (SpreadChartUtility.IsChartWithMarker(chartType))
            {
                c1DataSeries.SymbolMarker = SpreadMarkerToC1Marker(spDataSeries.MarkerType);
            }
            else
            {
                c1DataSeries.SymbolMarker = Marker.None;
            }
            Brush actualFill = spDataSeries.ActualFill;
            if (actualFill != null)
            {
                c1DataSeries.ConnectionFill = actualFill;
            }
            else
            {
                c1DataSeries.ConnectionFill = new SolidColorBrush(Colors.Transparent);
            }
            Brush actualStroke = spDataSeries.ActualStroke;
            if (actualStroke != null)
            {
                c1DataSeries.ConnectionStroke = actualStroke;
            }
            else
            {
                c1DataSeries.ConnectionStroke = new SolidColorBrush(Colors.Transparent);
            }
            double strokeThickness = spDataSeries.StrokeThickness;
            if (strokeThickness > 0.0)
            {
                c1DataSeries.ConnectionStrokeThickness = strokeThickness * ZoomFactor;
            }
            else
            {
                c1DataSeries.ConnectionStrokeThickness = 0.0;
            }
            c1DataSeries.ConnectionStrokeDashes = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(spDataSeries.StrokeDashType);
            if (SpreadChartUtility.IsLineOrLineWithMarkerChart(spDataSeries.ChartType))
            {
                if (spDataSeries.DataMarkerStyle != null)
                {
                    Brush brush3 = spDataSeries.DataMarkerStyle.ActualFill;
                    if (brush3 != null)
                    {
                        c1DataSeries.SymbolFill = brush3;
                    }
                    else
                    {
                        c1DataSeries.SymbolFill = spDataSeries.AutomaticFill;
                    }
                    Brush brush4 = spDataSeries.DataMarkerStyle.ActualStroke;
                    if (brush4 != null)
                    {
                        c1DataSeries.SymbolStroke = brush4;
                    }
                    else
                    {
                        c1DataSeries.SymbolStroke = spDataSeries.AutomaticStroke;
                    }
                    double num2 = spDataSeries.DataMarkerStyle.StrokeThickness;
                    if (num2 > 0.0)
                    {
                        c1DataSeries.SymbolStrokeThickness = num2 * ZoomFactor;
                    }
                    else
                    {
                        c1DataSeries.SymbolStrokeThickness = 0.0;
                    }
                    c1DataSeries.SymbolStrokeDashes = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(spDataSeries.DataMarkerStyle.StrokeDashType);
                }
                Size markerSize = spDataSeries.MarkerSize;
                c1DataSeries.SymbolSize = markerSize;
            }
            else if (SpreadChartUtility.IsSymbolChart(spDataSeries.ChartType) || (spDataSeries.ChartType == SpreadChartType.StockHighLowOpenClose))
            {
                Brush brush5 = spDataSeries.ActualFill;
                if (brush5 != null)
                {
                    c1DataSeries.SymbolFill = brush5;
                }
                else
                {
                    c1DataSeries.SymbolFill = new SolidColorBrush(Colors.Transparent);
                }
                Brush brush6 = spDataSeries.ActualStroke;
                if (brush6 != null)
                {
                    c1DataSeries.SymbolStroke = brush6;
                }
                else
                {
                    c1DataSeries.SymbolStroke = new SolidColorBrush(Colors.Transparent);
                }
                double num3 = spDataSeries.StrokeThickness;
                if (num3 > 0.0)
                {
                    c1DataSeries.SymbolStrokeThickness = num3 * ZoomFactor;
                }
                else
                {
                    c1DataSeries.SymbolStrokeThickness = 0.0;
                }
                c1DataSeries.SymbolStrokeDashes = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(spDataSeries.StrokeDashType);
            }
            if (SpreadChart.DisplayEmptyCellsAs == EmptyValueStyle.Zero)
            {
                c1DataSeries.Display = Dt.Charts.SeriesDisplay.SkipNaN;
            }
            else if (SpreadChart.DisplayEmptyCellsAs == EmptyValueStyle.Gaps)
            {
                c1DataSeries.Display = Dt.Charts.SeriesDisplay.ShowNaNGap;
            }
            if (!SpreadChartUtility.IsPieChart(SpreadChart.ChartType))
            {
                c1DataSeries.Label = spDataSeries.Name;
            }
        }

        void SyncDataSeriesValues(SpreadDataSeries spreadDataSeries, DataSeries c1DataSeries)
        {
            double[] dataSeriesYValues;
            double?[] dataSeriesXValues = GetDataSeriesXValues(spreadDataSeries);
            if ((dataSeriesXValues != null) && (dataSeriesXValues.Length > 0))
            {
                List<double> list = new List<double>();
                for (int i = 0; i < dataSeriesXValues.Length; i++)
                {
                    if (dataSeriesXValues[i].HasValue)
                    {
                        list.Add(dataSeriesXValues[i].Value);
                    }
                }
                XYDataSeries series = c1DataSeries as XYDataSeries;
                if (series != null)
                {
                    series.XValuesSource = (IEnumerable)list;
                }
                dataSeriesYValues = GetDataSeriesYValues(spreadDataSeries);
                dataSeriesYValues = LimitDataSeriesValues(dataSeriesXValues, dataSeriesYValues);
            }
            else
            {
                dataSeriesYValues = GetDataSeriesYValues(spreadDataSeries);
            }
            if (dataSeriesYValues.Length > 0)
            {
                if (!SpreadChartUtility.Is100PercentChart(SpreadChart.ChartType))
                {
                    c1DataSeries.ValuesSource = dataSeriesYValues;
                }
                else
                {
                    double[] pointValuesSum = GetPointValuesSum();
                    List<double> list2 = new List<double>();
                    for (int j = 0; j < dataSeriesYValues.Length; j++)
                    {
                        if (j < pointValuesSum.Length)
                        {
                            list2.Add(dataSeriesYValues[j] / pointValuesSum[j]);
                        }
                    }
                    c1DataSeries.ValuesSource = (IEnumerable)list2;
                }
            }
            if (spreadDataSeries is SpreadBubbleSeries)
            {
                BubbleSeries series2 = c1DataSeries as BubbleSeries;
                SpreadBubbleSeries dataSeries = spreadDataSeries as SpreadBubbleSeries;
                double[] numArray3 = LimitDataSeriesValues(dataSeriesXValues, dataSeries.SizeValues.ToArray());
                if (numArray3.Length == 0)
                {
                    numArray3 = GenerateValues(dataSeries, 0.0);
                }
                if (numArray3.Length > 0)
                {
                    series2.SizeValuesSource = numArray3;
                }
            }
        }

        void SyncDateTimeItems(Dt.Charts.Axis c1Axis, Dt.Cells.Data.Axis axis, out AxisAnnotation[] annotations)
        {
            if (SpreadChartUtility.IsPieChart(SpreadChart.ChartType) || SpreadChartUtility.IsRadarChart(SpreadChart.ChartType))
            {
                string[] displayItems = GetDisplayItems(axis);
                C1Chart.Data.ItemNames = displayItems;
                annotations = null;
            }
            else
            {
                List<AxisAnnotation> labels = new List<AxisAnnotation>();
                List<DateTime> list2 = new List<DateTime>();
                foreach (object obj2 in axis.Items)
                {
                    if (obj2 is DateTime)
                    {
                        list2.Add((DateTime)obj2);
                    }
                }
                list2.Sort();
                for (int i = 0; i < list2.Count; i++)
                {
                    object item = list2[i];
                    string str = axis.ShowAxisLabel ? GetBindingText(axis, item) : string.Empty;
                    double num2 = 0.0;
                    if (item is DateTime)
                    {
                        //hdt
                        num2 = DateTimeExtension.ToOADate((DateTime)item);
                    }
                    AxisAnnotation annotation = new AxisAnnotation
                    {
                        Label = str,
                        Value = (double)num2
                    };
                    labels.Add(annotation);
                }
                annotations = labels.ToArray();
                BindingItems(c1Axis, axis, labels);
                SyncMinAndMax(c1Axis, axis);
            }
        }

        void SyncLineChartPlotElement(PlotElement plotElement, SpreadDataSeries dataSeries)
        {
            if (!(plotElement is Lines) && SpreadChartUtility.IsChartWithMarker(dataSeries.ChartType))
            {
                SyncDataMarker(plotElement, dataSeries);
            }
        }

        void SyncMinAndMax(Dt.Charts.Axis c1Axis, Dt.Cells.Data.Axis axis)
        {
            if (axis.AxisType == Dt.Cells.Data.AxisType.Date)
            {
                double num = 0.0;
                if (axis.MajorTimeUnit == AxisTimeUnit.Years)
                {
                    num = 365.0;
                }
                else if (axis.MajorTimeUnit == AxisTimeUnit.Months)
                {
                    num = 30.0;
                }
                else
                {
                    num = 1.0;
                }
                c1Axis.Min = axis.Min;
                c1Axis.Max = axis.Max;
                if (NeedInflateDateTimeAxis())
                {
                    c1Axis.Min -= num * axis.MajorUnit;
                    c1Axis.Max += num * axis.MajorUnit;
                }
                c1Axis.MajorUnit = num * axis.MajorUnit;
                c1Axis.MinorUnit = num * axis.MinorUnit;
            }
            else if (axis.AxisType == Dt.Cells.Data.AxisType.Category)
            {
                if (axis.AutoMin)
                {
                    c1Axis.Min = double.NaN;
                }
                else
                {
                    c1Axis.Min = axis.Min;
                }
                if (axis.AutoMax)
                {
                    c1Axis.Max = double.NaN;
                }
                else
                {
                    c1Axis.Max = axis.Max;
                }
                c1Axis.MinorUnit = axis.MinorUnit;
                c1Axis.MajorUnit = axis.MajorUnit;
            }
            else if (!axis.UseLogBase || double.IsNaN(axis.LogBase))
            {
                c1Axis.Min = axis.Min;
                c1Axis.Max = axis.Max;
                c1Axis.MinorUnit = axis.MinorUnit;
                c1Axis.MajorUnit = axis.MajorUnit;
            }
        }

        void SyncOHLCDataSeriesSettings(SpreadDataSeries spDataSeries, DataSeries c1DataSeries)
        {
            c1DataSeries.SymbolMarker = SpreadMarkerToC1Marker(spDataSeries.MarkerType);
            if (spDataSeries.ActualFill != null)
            {
                c1DataSeries.ConnectionFill = spDataSeries.ActualFill;
            }
            else
            {
                c1DataSeries.ConnectionFill = new SolidColorBrush(Colors.Transparent);
            }
            Brush actualStroke = spDataSeries.ActualStroke;
            if (actualStroke != null)
            {
                c1DataSeries.ConnectionStroke = actualStroke;
            }
            else
            {
                c1DataSeries.ConnectionStroke = new SolidColorBrush(Colors.Transparent);
            }
            double strokeThickness = spDataSeries.StrokeThickness;
            if (strokeThickness > 0.0)
            {
                c1DataSeries.ConnectionStrokeThickness = strokeThickness * ZoomFactor;
            }
            else
            {
                c1DataSeries.ConnectionStrokeThickness = 0.0;
            }
            c1DataSeries.ConnectionStrokeDashes = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(spDataSeries.StrokeDashType);
            Brush actualFill = spDataSeries.DataMarkerStyle.ActualFill;
            if (actualFill != null)
            {
                c1DataSeries.SymbolFill = actualFill;
            }
            else
            {
                c1DataSeries.SymbolFill = new SolidColorBrush(Colors.Transparent);
            }
            Brush brush4 = spDataSeries.DataMarkerStyle.ActualStroke;
            if (brush4 != null)
            {
                c1DataSeries.SymbolStroke = brush4;
            }
            else
            {
                c1DataSeries.SymbolStroke = new SolidColorBrush(Colors.Transparent);
            }
            double num2 = spDataSeries.DataMarkerStyle.StrokeThickness;
            if (num2 > 0.0)
            {
                c1DataSeries.SymbolStrokeThickness = num2 * ZoomFactor;
            }
            else
            {
                c1DataSeries.SymbolStrokeThickness = 0.0;
            }
            c1DataSeries.SymbolStrokeDashes = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(spDataSeries.DataMarkerStyle.StrokeDashType);
            Size markerSize = spDataSeries.MarkerSize;
            c1DataSeries.SymbolSize = markerSize;
            if (SpreadChart.DisplayEmptyCellsAs == EmptyValueStyle.Zero)
            {
                c1DataSeries.Display = Dt.Charts.SeriesDisplay.SkipNaN;
            }
            else if (SpreadChart.DisplayEmptyCellsAs == EmptyValueStyle.Gaps)
            {
                c1DataSeries.Display = Dt.Charts.SeriesDisplay.ShowNaNGap;
            }
            c1DataSeries.Label = spDataSeries.Name;
        }

        void SyncOpenHighLowCloseSeries(DataSeries[] c1DataSeriesList, SpreadOpenHighLowCloseSeries spOHLCSeries)
        {
            HighLowOpenCloseSeries series = c1DataSeriesList[0] as HighLowOpenCloseSeries;

            if (((spOHLCSeries.XValues != null) && (spOHLCSeries.XValues.Count > 0)) && !ContainsNaN(spOHLCSeries.XValues))
            {
                series.XValuesSource = spOHLCSeries.XValues;
            }
            else
            {
                series.XValuesSource = GenerateXValues(spOHLCSeries);
            }
            SyncDataSeriesChartType(spOHLCSeries, series);
            SyncDataSeriesSettings(spOHLCSeries, series);
            series.Label = string.Empty;
            if (spOHLCSeries.OpenSeries.Values != null)
            {
                series.OpenValuesSource = spOHLCSeries.OpenSeries.Values;
                if (1 < c1DataSeriesList.Length)
                {
                    XYDataSeries series2 = c1DataSeriesList[1] as XYDataSeries;
                    if (series2 != null)
                    {
                        series2.XValuesSource = series.XValuesSource;
                        series2.ValuesSource = spOHLCSeries.OpenSeries.Values;
                        if (spOHLCSeries.OpenSeries.MarkerType == Dt.Cells.Data.MarkerType.None)
                        {
                            series2.ChartType = (ChartType)6;
                        }
                        else
                        {
                            series2.ChartType = (ChartType)10;
                        }
                        SyncOHLCDataSeriesSettings(spOHLCSeries.OpenSeries, series2);
                        if ((string.IsNullOrEmpty(series2.Label) && (spOHLCSeries.OpenSeries.Values != null)) && (spOHLCSeries.OpenSeries.Values.Count > 0))
                        {
                            series2.Label = ResourceStrings.DefaultDataSeriesName + "1";
                        }
                    }
                }
            }
            if (spOHLCSeries.HighSeries != null)
            {
                series.HighValuesSource = spOHLCSeries.HighSeries.Values;
                if (2 < c1DataSeriesList.Length)
                {
                    XYDataSeries series3 = c1DataSeriesList[2] as XYDataSeries;
                    if (series3 != null)
                    {
                        series3.XValuesSource = series.XValuesSource;
                        series3.ValuesSource = spOHLCSeries.HighSeries.Values;
                        if (spOHLCSeries.HighSeries.MarkerType == Dt.Cells.Data.MarkerType.None)
                        {
                            series3.ChartType = (ChartType)6;
                        }
                        else
                        {
                            series3.ChartType = (ChartType)10;
                        }
                        SyncOHLCDataSeriesSettings(spOHLCSeries.HighSeries, series3);
                        if ((string.IsNullOrEmpty(series3.Label) && (spOHLCSeries.HighSeries.Values != null)) && (spOHLCSeries.HighSeries.Values.Count > 0))
                        {
                            series3.Label = ResourceStrings.DefaultDataSeriesName + "2";
                        }
                    }
                }
            }
            if (spOHLCSeries.LowSeries != null)
            {
                series.LowValuesSource = spOHLCSeries.LowSeries.Values;
                if (3 < c1DataSeriesList.Length)
                {
                    XYDataSeries series4 = c1DataSeriesList[3] as XYDataSeries;
                    if (series4 != null)
                    {
                        series4.XValuesSource = series.XValuesSource;
                        series4.ValuesSource = spOHLCSeries.LowSeries.Values;
                        if (spOHLCSeries.LowSeries.MarkerType == Dt.Cells.Data.MarkerType.None)
                        {
                            series4.ChartType = (ChartType)6;
                        }
                        else
                        {
                            series4.ChartType = (ChartType)10;
                        }
                        SyncOHLCDataSeriesSettings(spOHLCSeries.LowSeries, series4);
                        if ((string.IsNullOrEmpty(series4.Label) && (spOHLCSeries.LowSeries.Values != null)) && (spOHLCSeries.LowSeries.Values.Count > 0))
                        {
                            series4.Label = ResourceStrings.DefaultDataSeriesName + "3";
                        }
                    }
                }
            }
            if (spOHLCSeries.CloseSeries != null)
            {
                series.CloseValuesSource = spOHLCSeries.CloseSeries.Values;
                if (4 < c1DataSeriesList.Length)
                {
                    XYDataSeries series5 = c1DataSeriesList[4] as XYDataSeries;
                    if (series5 != null)
                    {
                        series5.XValuesSource = series.XValuesSource;
                        series5.ValuesSource = spOHLCSeries.CloseSeries.Values;
                        if (spOHLCSeries.CloseSeries.MarkerType == Dt.Cells.Data.MarkerType.None)
                        {
                            series5.ChartType = (ChartType)6;
                        }
                        else
                        {
                            series5.ChartType = (ChartType)10;
                        }
                        SyncOHLCDataSeriesSettings(spOHLCSeries.CloseSeries, series5);
                        if ((string.IsNullOrEmpty(series5.Label) && (spOHLCSeries.CloseSeries.Values != null)) && (spOHLCSeries.CloseSeries.Values.Count > 0))
                        {
                            series5.Label = ResourceStrings.DefaultDataSeriesName + "4";
                        }
                    }
                }
            }
        }

        void SyncPieChartPaletteColor()
        {
            if ((SpreadChart != null) && (((SpreadChart.ChartType == SpreadChartType.Pie) || (SpreadChart.ChartType == SpreadChartType.PieDoughnut)) || ((SpreadChart.ChartType == SpreadChartType.PieExploded) || (SpreadChart.ChartType == SpreadChartType.PieExplodedDoughnut))))
            {
                IList<SpreadDataSeries> displayingDataSeries = GetDisplayingDataSeries();
                if (displayingDataSeries.Count > 0)
                {
                    SpreadDataSeries series = displayingDataSeries[0];
                    List<Brush> list2 = new List<Brush>();
                    for (int i = 0; i < series.Values.Count; i++)
                    {
                        Brush actualFill = series.GetDataPoint(i).ActualFill;
                        if (actualFill != null)
                        {
                            list2.Add(actualFill);
                        }
                    }
                    C1Chart.CustomPalette = list2.ToArray();
                }
            }
        }

        void SyncTimeAxis(Dt.Charts.Axis c1Axis, Dt.Cells.Data.Axis spAxis)
        {
            c1Axis.IsTime = true;
            int num = 1;
            switch (spAxis.MajorTimeUnit)
            {
                case AxisTimeUnit.Days:
                    num = 1;
                    break;

                case AxisTimeUnit.Months:
                    num = 30;
                    break;

                case AxisTimeUnit.Years:
                    num = 0x16d;
                    break;
            }
            c1Axis.Min = spAxis.Min;
            c1Axis.Max = spAxis.Max;
            c1Axis.MinorUnit = spAxis.MinorUnit * num;
            c1Axis.MajorUnit = spAxis.MajorUnit * num;
            c1Axis.Origin = spAxis.CrossAt * num;
        }

        void SyncValueItems(Dt.Charts.Axis c1Axis, Dt.Cells.Data.Axis axis, AxisXYZType axisType, out AxisAnnotation[] annotations)
        {
            List<AxisAnnotation> labels = new List<AxisAnnotation>();
            double? nullable = null;
            List<object> list2 = Enumerable.ToList<object>((IEnumerable<object>)axis.Items);
            if (axis.UseLogBase && !double.IsNaN(axis.LogBase))
            {
                list2 = UpdateAxisItems(axis);
            }
            foreach (object obj2 in list2)
            {
                nullable = TryDouble(obj2, true);
                if (nullable.HasValue)
                {
                    double num = nullable.Value;
                    string bindingText = GetBindingText(axis, (double)num);
                    double displayUnit = axis.DisplayUnit;
                    if (displayUnit > 0.0)
                    {
                        num /= displayUnit;
                        if ((axis.LabelFormatter.FormatString.Equals("general", (StringComparison)StringComparison.OrdinalIgnoreCase) && (Math.Abs(num) <= 1E-07)) && (num != 0.0))
                        {
                            bindingText = new GeneralFormatter("0.00E+00").Format((double)num);
                        }
                        else
                        {
                            bindingText = GetBindingText(axis, (double)num);
                        }
                    }
                    if (!axis.ShowAxisLabel)
                    {
                        bindingText = string.Empty;
                    }
                    AxisAnnotation annotation = new AxisAnnotation
                    {
                        Label = bindingText,
                        Value = (double)nullable.Value
                    };
                    labels.Add(annotation);
                }
                else if (obj2 != null)
                {
                    AxisAnnotation annotation2 = new AxisAnnotation
                    {
                        Label = obj2.ToString(),
                        Value = obj2
                    };
                    labels.Add(annotation2);
                }
                else
                {
                    AxisAnnotation annotation3 = new AxisAnnotation
                    {
                        Label = string.Empty,
                        Value = obj2
                    };
                    labels.Add(annotation3);
                }
            }
            annotations = labels.ToArray();
            BindingItems(c1Axis, axis, labels);
            SyncMinAndMax(c1Axis, axis);
        }

        void SyncWholeDataPoint(PlotElement plotElement, SpreadDataSeries dataSeries)
        {
            if ((plotElement != null) && (dataSeries != null))
            {
                Brush actualFill = dataSeries.ActualFill;
                if (actualFill != null)
                {
                    plotElement.Fill = actualFill;
                }
                Brush actualStroke = dataSeries.ActualStroke;
                if (actualStroke != null)
                {
                    plotElement.Stroke = actualStroke;
                }
                if (dataSeries.StrokeThickness > 0.0)
                {
                    plotElement.StrokeThickness = dataSeries.StrokeThickness * ZoomFactor;
                }
                else
                {
                    plotElement.StrokeThickness = 0.0;
                }
                if (dataSeries.StrokeDashType != StrokeDashType.None)
                {
                    plotElement.StrokeDashArray = Dt.Cells.Data.StrokeDashHelper.GetStrokeDashes(dataSeries.StrokeDashType);
                }
            }
        }

        void TransformBarAndColumnChart(PlotElement plotElement, SpreadDataSeries dataSeries)
        {
            if (SpreadChartUtility.IsBarChart(dataSeries.ChartType) || SpreadChartUtility.IsColumnChart(dataSeries.ChartType))
            {
                _loadedDataPointCount++;
                if (_cachedPlotElement.ContainsKey(plotElement.DataPoint.PointIndex))
                {
                    _cachedPlotElement[plotElement.DataPoint.PointIndex].Add(plotElement);
                }
                else
                {
                    List<PlotElement> list = new List<PlotElement> {
                        plotElement
                    };
                    _cachedPlotElement[plotElement.DataPoint.PointIndex] = list;
                }
                if (_loadedDataPointCount == _cachedTotalDataPointCount)
                {
                    TransformDataPointElement(plotElement);
                    _loadedDataPointCount = 0;
                    _cachedPlotElement.Clear();
                }
            }
        }

        void TransformBarChart(double scale, bool reversed)
        {
            using (Dictionary<int, List<PlotElement>>.Enumerator enumerator = _cachedPlotElement.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    List<PlotElement> list = enumerator.Current.Value;
                    if (((list != null) && (list.Count > 0)) && (SpreadChart.ChartType != SpreadChartType.BarClustered))
                    {
                        TransformBarStack(scale, list);
                    }
                }
            }
        }

        void TransformBarStack(double scale, List<PlotElement> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                PlotElement element = list[i];
                element.RenderTransform = CreatTransformGroup(scale, -1.0, false, false);
                element.RenderTransformOrigin = new Point(0.0, 0.5);
            }
        }

        void TransformColumnChart(double scale, bool reversed)
        {
            using (Dictionary<int, List<PlotElement>>.Enumerator enumerator = _cachedPlotElement.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    List<PlotElement> list = enumerator.Current.Value;
                    if ((list != null) && (list.Count > 0))
                    {
                        if (SpreadChart.ChartType == SpreadChartType.ColumnClustered)
                        {
                            TransformColumnClustered(list, scale, reversed);
                        }
                        else
                        {
                            TransformColumnStack(scale, list);
                        }
                    }
                }
            }
        }

        void TransformColumnClustered(List<PlotElement> list, double scale, bool reversed)
        {
            int num = reversed ? -1 : 1;
            int maxDataPointCount = GetMaxDataPointCount();
            PlotElement element = null;
            foreach (PlotElement element2 in list)
            {
                if (element2 != null)
                {
                    element = element2;
                    break;
                }
            }
            if (element != null)
            {
                int num5;
                int num6;
                double num3 = ((num * element.ActualWidth) * (1.0 - scale)) / 2.0;
                int num4 = -1;
                if ((maxDataPointCount % 2) != 0)
                {
                    num4 = maxDataPointCount / 2;
                }
                if (num4 != -1)
                {
                    num5 = num4 - 1;
                }
                else
                {
                    num5 = (maxDataPointCount / 2) - 1;
                }
                if (num4 != -1)
                {
                    num6 = num4 + 1;
                }
                else
                {
                    num6 = maxDataPointCount / 2;
                }
                for (int i = num5; i >= 0; i--)
                {
                    if (i < list.Count)
                    {
                        PlotElement element3 = list[i];
                        double offset = ((num5 - i) + 1) * num3;
                        if (num4 != -1)
                        {
                            offset += num3;
                        }
                        element3.RenderTransform = CreatTransformGroup(scale, offset, true, true);
                        element3.RenderTransformOrigin = new Point(0.5, 0.0);
                    }
                }
                for (int j = num6; j < maxDataPointCount; j++)
                {
                    if (j < list.Count)
                    {
                        PlotElement element4 = list[j];
                        double num10 = ((j - num6) + 1) * num3;
                        if (num4 != -1)
                        {
                            num10 += num3;
                        }
                        element4.RenderTransform = CreatTransformGroup(scale, -num10, true, true);
                        element4.RenderTransformOrigin = new Point(0.5, 0.0);
                    }
                }
                if ((((maxDataPointCount % 2) != 0) && (num4 < list.Count)) && (num4 >= 0))
                {
                    PlotElement element5 = list[num4];
                    element5.RenderTransform = CreatTransformGroup(scale, 0.0, true, true);
                    element5.RenderTransformOrigin = new Point(0.5, 0.0);
                }
            }
        }

        void TransformColumnStack(double scale, List<PlotElement> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                PlotElement element = list[i];
                element.RenderTransform = CreatTransformGroup(scale, -1.0, false, true);
                element.RenderTransformOrigin = new Point(0.5, 0.0);
            }
        }

        void TransformDataPointElement(PlotElement dataPointElement)
        {
            double scale = 1.0;
            double gapWidth = GetGapWidth();
            int maxDataPointCount = GetMaxDataPointCount();
            int dataSeriseCount = -1;
            if (SpreadChartUtility.IsBarChart(SpreadChart.ChartType))
            {
                double num5 = ((!double.IsInfinity(SpreadChart.AxisY.MajorUnit) && !double.IsNaN(SpreadChart.AxisY.MajorUnit)) && (!double.IsPositiveInfinity(SpreadChart.AxisY.MajorUnit) && !double.IsPositiveInfinity(SpreadChart.AxisY.MajorUnit))) ? SpreadChart.AxisY.MajorUnit : 1.0;
                if (SpreadChart.AxisY.AxisType == Dt.Cells.Data.AxisType.Date)
                {
                    if (SpreadChart.AxisY.MajorTimeUnit == AxisTimeUnit.Years)
                    {
                        num5 *= 365.0;
                    }
                    else if (SpreadChart.AxisY.MajorTimeUnit == AxisTimeUnit.Months)
                    {
                        num5 *= 30.0;
                    }
                    else
                    {
                        num5 *= 1.0;
                    }
                }
                else
                {
                    Dt.Cells.Data.AxisType axisType = SpreadChart.AxisY.AxisType;
                }
                dataSeriseCount = (int)(((SpreadChart.AxisY.Max - SpreadChart.AxisY.Min) / num5) + 1);
            }
            else
            {
                double num6 = ((!double.IsInfinity(SpreadChart.AxisX.MajorUnit) && !double.IsNaN(SpreadChart.AxisX.MajorUnit)) && (!double.IsPositiveInfinity(SpreadChart.AxisX.MajorUnit) && !double.IsPositiveInfinity(SpreadChart.AxisX.MajorUnit))) ? SpreadChart.AxisX.MajorUnit : 1.0;
                if (SpreadChart.AxisX.AxisType == Dt.Cells.Data.AxisType.Date)
                {
                    if (SpreadChart.AxisX.MajorTimeUnit == AxisTimeUnit.Years)
                    {
                        num6 *= 365.0;
                    }
                    else if (SpreadChart.AxisX.MajorTimeUnit == AxisTimeUnit.Months)
                    {
                        num6 *= 30.0;
                    }
                    else
                    {
                        num6 *= 1.0;
                    }
                }
                else
                {
                    Dt.Cells.Data.AxisType type2 = SpreadChart.AxisX.AxisType;
                }
                dataSeriseCount = (int)(((SpreadChart.AxisX.Max - SpreadChart.AxisX.Min) / num6) + 1);
            }
            if ((((SpreadChart.ChartType == SpreadChartType.ColumnClustered) || (SpreadChart.ChartType == SpreadChartType.ColumnStacked)) || (SpreadChart.ChartType == SpreadChartType.ColumnStacked100pc)) && (C1Chart.View.AxisX.AxisLine != null))
            {
                double lineLength = C1Chart.View.AxisX.AxisLine.X2 - C1Chart.View.AxisX.AxisLine.X1;
                if ((lineLength > 0.0) && (gapWidth > 0.0))
                {
                    scale = CalculateScale(lineLength, dataSeriseCount, maxDataPointCount, gapWidth, dataPointElement.ActualWidth);
                    TransformColumnChart(scale, C1Chart.View.AxisX.Reversed);
                }
            }
            if ((((SpreadChart.ChartType == SpreadChartType.BarClustered) || (SpreadChart.ChartType == SpreadChartType.BarStacked)) || (SpreadChart.ChartType == SpreadChartType.BarStacked100pc)) && (C1Chart.View.AxisY.AxisLine != null))
            {
                double num8 = C1Chart.View.AxisY.AxisLine.X2 - C1Chart.View.AxisY.AxisLine.X1;
                if ((num8 > 0.0) && (gapWidth > 0.0))
                {
                    scale = CalculateScale(num8, dataSeriseCount, maxDataPointCount, gapWidth, dataPointElement.ActualHeight);
                    TransformBarChart(scale, C1Chart.View.AxisY.Reversed);
                }
            }
        }

        double? TryDouble(object val, bool useCulture = true)
        {
            double num;
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            if (val is bool)
            {
                return new double?(((bool)val) ? 1.0 : 0.0);
            }
            if (val == null)
            {
                return 0.0;
            }
            if (IsNumber(val))
            {
                if (val is DateTime)
                {
                    try
                    {
                        return new double?(DateTimeExtension.ToOADate((DateTime)val));
                    }
                    catch
                    {
                        return null;
                    }
                }
                if (val is TimeSpan)
                {
                    TimeSpan span = (TimeSpan)val;
                    return new double?(span.TotalDays);
                }
                try
                {
                    return new double?(Convert.ToDouble(val, (IFormatProvider)info));
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
            if ((val is string) && double.TryParse((string)(val as string), (NumberStyles)NumberStyles.Any, (IFormatProvider)info, out num))
            {
                return new double?(num);
            }
            return null;
        }

        List<object> UpdateAxisItems(Dt.Cells.Data.Axis axis)
        {
            double num = AxisValueUtility.CalculateMaximum(axis.Min, axis.Max, axis.LogBase, false);
            List<object> list = new List<object>();
            double y = 0.0;
            double num3 = 0.0;
            while (num3 <= num)
            {
                list.Add((double)num3);
                num3 = Math.Pow(axis.LogBase, y);
                y++;
            }
            return list;
        }

        internal override void UpdateLayoutsOnMeasure(Size size)
        {
            base.UpdateLayoutsOnMeasure(size);
            if (((C1Chart != null) && (C1Chart.View.AxisX != null)) && ((C1Chart.View.AxisX.Title != null) && (C1Chart.View.AxisX.Title is FrameworkElement)))
            {
                (C1Chart.View.AxisX.Title as FrameworkElement).MaxWidth = SpreadChart.Size.Width;
                (C1Chart.View.AxisX.Title as FrameworkElement).MaxHeight = SpreadChart.Size.Height;
            }
            if (((C1Chart != null) && (C1Chart.View.AxisY != null)) && ((C1Chart.View.AxisY.Title != null) && (C1Chart.View.AxisY.Title is FrameworkElement)))
            {
                (C1Chart.View.AxisY.Title as FrameworkElement).MaxWidth = SpreadChart.Size.Height;
                (C1Chart.View.AxisY.Title as FrameworkElement).MaxHeight = SpreadChart.Size.Width;
            }
        }

        /// <summary>
        /// Gets the c1 chart.
        /// </summary>
        /// <value>
        /// The c1 chart.
        /// </value>
        public Chart C1Chart
        {
            get { return (C1ChartControl as Chart); }
        }

        /// <summary>
        /// Gets the spread chart.
        /// </summary>
        /// <value>
        /// The spread chart.
        /// </value>
        public Dt.Cells.Data.SpreadChart SpreadChart
        {
            get { return (_spreadChartContent as Dt.Cells.Data.SpreadChart); }
        }

        internal class AxisOriginalItemsCollection : SeriesDataCollection<object>
        {
            protected override object ConvertValue(int valueIndex, object obj)
            {
                return obj;
            }
        }
    }
}

