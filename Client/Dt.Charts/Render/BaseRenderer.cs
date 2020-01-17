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
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public abstract class BaseRenderer : IRenderer, IAxisController
    {
        bool _analyzed;
        Dt.Charts.ColorScheme _clrScheme;
        object _conn;
        ICoordConverter _coordConverter;
        internal DataInfo _dataInfo;
        bool _dirty;
        bool _inverted;
        object[] _itemnames;
        List<string> _optbag;
        List<IDataSeriesInfo> _series;
        double _sizeX = 0.7;
        double _sizeY = 0.7;
        StackedOptions _stacked;
        StyleGenerator _stgen;
        ResourceDictionary _styles;
        object _symbol;
        PlotElement _symbolCash;
        UIElement _visual;
        bool clear = true;
        const char delim = ';';
        const char pair_delim = '=';

        event EventHandler IRenderer.Changed
        {
            add
            {
                Changed += value;
            }
            remove
            {
                Changed -= value;
            }
        }

        event EventHandler IRenderer.Rendered
        {
            add
            {
                Rendered += value;
            }
            remove
            {
                Rendered -= value;
            }
        }

        internal event EventHandler Changed;

        internal event EventHandler Rendered;

        protected BaseRenderer()
        {
        }

        internal virtual void AdjustAxisInternal(IAxis ax, double delta)
        {
        }

        void AnalyseNDimNPts()
        {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < _dataInfo.nser; i++)
            {
                IDataSeriesInfo info = Series[i];
                if (info is HighLowSeries)
                {
                    if (Chart.ChartType == Dt.Charts.ChartType.Gantt)
                    {
                        ((HighLowSeries) info).IsGantt = true;
                        ((HighLowSeries) info).Index = i;
                    }
                    else
                    {
                        ((HighLowSeries) info).IsGantt = false;
                    }
                }
                double[,] values = info.GetValues();
                if (values != null)
                {
                    num2 = Math.Max(num2, values.GetLength(1));
                    num = Math.Max(num, values.GetLength(0));
                }
            }
            _dataInfo.npts = num2;
            _dataInfo.ndim = num;
        }

        protected void AnalyzeData(bool isStacked)
        {
            if (!_analyzed)
            {
                int num = Series.Count;
                _dataInfo.nser = num;
                _dataInfo.npts = 0;
                _dataInfo.ndim = 0;
                _dataInfo.ClearLimits();
                _dataInfo.SymbolMinSize = Size.Empty;
                _dataInfo.SymbolMaxSize = Size.Empty;
                _dataInfo.incX = false;
                _dataInfo.hasNaN = false;
                bool flag = false;
                bool flag2 = IsStacked100;
                if (num > 0)
                {
                    AnalyseNDimNPts();
                    int ndim = _dataInfo.ndim;
                    int npts = _dataInfo.npts;
                    double[] numArray = new double[ndim];
                    double[] numArray2 = new double[ndim];
                    double[] numArray3 = new double[num];
                    double[] numArray4 = new double[num];
                    int nsg = NStackGroups();
                    StackedSum sum = isStacked ? new StackedSum(nsg) : null;
                    StackedSum sum2 = isStacked ? new StackedSum(nsg) : null;
                    StackedSum sum3 = isStacked ? new StackedSum(nsg) : null;
                    StackedSum sum4 = flag2 ? new StackedSum(nsg) : null;
                    double num5 = 0.0;
                    double num6 = 0.0;
                    double naN = double.NaN;
                    Size size = new Size();
                    for (int i = 0; i < num; i++)
                    {
                        DataSeries ds = (DataSeries) Series[i];
                        bool flag3 = ds.IsStacked;
                        bool isClustered = ds.GetIsClustered(true);
                        int stackGroup = BarColumnOptions.GetStackGroup(ds);
                        if (ds is BubbleSeries)
                        {
                            Chart chart = _visual as Chart;
                            if (chart != null)
                            {
                                Size maxSize = ((BubbleSeries) ds).GetMaxSize(_dataInfo);
                                if (size.Width < maxSize.Width)
                                {
                                    size.Width = maxSize.Width;
                                }
                                if (size.Height < maxSize.Height)
                                {
                                    size.Height = maxSize.Height;
                                }
                                if (_dataInfo.SymbolMaxSize.IsEmpty)
                                {
                                    _dataInfo.SymbolMaxSize = BubbleOptions.GetMaxSize(chart);
                                }
                                if (_dataInfo.SymbolMinSize.IsEmpty)
                                {
                                    _dataInfo.SymbolMinSize = BubbleOptions.GetMinSize(chart);
                                }
                            }
                        }
                        double[,] values = ds.GetValues();
                        if (values != null)
                        {
                            int length = values.GetLength(0);
                            int num11 = values.GetLength(1);
                            if (((num11 >= 2) && (length >= 2)) && isClustered)
                            {
                                double num12 = 0.0;
                                for (int m = 0; m < (num11 - 1); m++)
                                {
                                    if (!double.IsNaN(values[1, m]) && !double.IsNaN(values[1, m + 1]))
                                    {
                                        num12 = Math.Abs((double) (values[1, m + 1] - values[1, m]));
                                        if (double.IsNaN(naN) || (num12 < naN))
                                        {
                                            naN = num12;
                                        }
                                    }
                                }
                            }
                            ValueCoordinate[] valueCoordinates = ds.GetValueCoordinates(false);
                            for (int j = 0; j < valueCoordinates.Length; j++)
                            {
                                if ((valueCoordinates[j] == ValueCoordinate.X) && IsValidAuxAxis(ds.AxisX))
                                {
                                    valueCoordinates[j] = ValueCoordinate.None;
                                }
                                else if ((valueCoordinates[j] == ValueCoordinate.Y) && IsValidAuxAxis(ds.AxisY))
                                {
                                    valueCoordinates[j] = ValueCoordinate.None;
                                }
                            }
                            for (int k = 0; k < length; k++)
                            {
                                if ((k < valueCoordinates.Length) && (valueCoordinates[k] != ValueCoordinate.None))
                                {
                                    for (int n = 0; n < num11; n++)
                                    {
                                        double x = (length > 1) ? values[1, n] : ((double) n);
                                        if (double.IsNaN(values[k, n]))
                                        {
                                            _dataInfo.hasNaN = true;
                                        }
                                        else
                                        {
                                            if ((i == 0) && (n == 0))
                                            {
                                                numArray[k] = numArray2[k] = values[k, n];
                                            }
                                            else
                                            {
                                                if (values[k, n] > numArray[k])
                                                {
                                                    numArray[k] = values[k, n];
                                                }
                                                if (values[k, n] < numArray2[k])
                                                {
                                                    numArray2[k] = values[k, n];
                                                }
                                            }
                                            if (k == 0)
                                            {
                                                numArray3[i] += values[0, n];
                                                numArray4[i] += Math.Abs(values[0, n]);
                                            }
                                            else if ((flag && (k == 1)) && ((n > 0) && ((values[1, n] - values[1, n - 1]) < 0.0)))
                                            {
                                                flag = false;
                                            }
                                            if (((k == 0) && isStacked) && flag3)
                                            {
                                                if (flag2)
                                                {
                                                    sum.Add(stackGroup, x, Math.Abs(values[0, n]));
                                                    sum4.Add(stackGroup, x, values[0, n]);
                                                }
                                                else
                                                {
                                                    sum.Add(stackGroup, x, values[0, n]);
                                                    num5 = Math.Min(num5, sum[stackGroup, (double) n]);
                                                    num6 = Math.Max(num6, sum[stackGroup, (double) n]);
                                                }
                                                if (values[0, n] > 0.0)
                                                {
                                                    sum2.Add(stackGroup, x, values[0, n]);
                                                }
                                                else
                                                {
                                                    sum3.Add(stackGroup, (double) n, values[0, n]);
                                                }
                                            }
                                        }
                                    }
                                    _dataInfo.UpdateLimits(valueCoordinates[k], numArray2[k], numArray[k]);
                                }
                            }
                            if (length < 2)
                            {
                                _dataInfo.UpdateLimits(ValueCoordinate.X, 0.0, (double) (num11 - 1));
                            }
                        }
                    }
                    if (isStacked && (npts > 0))
                    {
                        if (flag2)
                        {
                            numArray2[0] = 0.0;
                            numArray[0] = 0.0;
                            for (int num18 = 0; num18 < npts; num18++)
                            {
                                for (int num19 = 0; num19 < nsg; num19++)
                                {
                                    if (sum[num19, (double) num18] != 0.0)
                                    {
                                        double num20 = sum2[num19, (double) num18] / sum[num19, (double) num18];
                                        double num21 = sum3[num19, (double) num18] / sum[num19, (double) num18];
                                        if (num20 > numArray[0])
                                        {
                                            numArray[0] = num20;
                                        }
                                        if (num21 < numArray2[0])
                                        {
                                            numArray2[0] = num21;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            numArray[0] = 0.0;
                            numArray2[0] = 0.0;
                            for (int num22 = 0; num22 < nsg; num22++)
                            {
                                double[] ys = sum2.GetYs(num22);
                                double[] numArray7 = sum3.GetYs(num22);
                                for (int num23 = 0; num23 < ys.Length; num23++)
                                {
                                    if (ys[num23] > numArray[0])
                                    {
                                        numArray[0] = ys[num23];
                                    }
                                }
                                for (int num24 = 0; num24 < numArray7.Length; num24++)
                                {
                                    if (numArray7[num24] < numArray2[0])
                                    {
                                        numArray2[0] = numArray7[num24];
                                    }
                                }
                            }
                        }
                    }
                    _dataInfo.MaxVals = numArray;
                    _dataInfo.MinVals = numArray2;
                    _dataInfo.Sums = numArray3;
                    _dataInfo.SumsAbs = numArray4;
                    _dataInfo.Stacked = sum;
                    _dataInfo.DeltaX = (double.IsNaN(naN) || (naN == 0.0)) ? 1.0 : naN;
                    _dataInfo.SymbolSize = size;
                    _dataInfo.incX = flag;
                }
                _analyzed = true;
            }
        }

        protected void AttachTooltip(DataSeries ds, PlotElement ple)
        {
            if ((ple != null) && (ToolTipService.GetToolTip(ple) == null))
            {
                ToolTip tooltip = null;
                if (ds.PointTooltipTemplate != null)
                {
                    object obj2 = ds.PointTooltipTemplate.LoadContent();
                    if (obj2 is ToolTip)
                    {
                        tooltip = (ToolTip) obj2;
                    }
                    else
                    {
                        ToolTip tip = new ToolTip();
                        tip.Content = obj2;
                        tooltip = tip;
                    }
                    ToolTipService.SetToolTip(ple, tooltip);
                    tooltip.PlacementTarget = ple;
                    tooltip.Opened += (sender,e)=>
                    {
                        tooltip.DataContext = ple.DataPoint;
                    };
                }
            }
        }

        void IAxisController.AdjustAxis(IAxis ax, double delta)
        {
            AdjustAxisInternal(ax, delta);
        }

        void IRenderer.AddSeries(IDataSeriesInfo seriesInfo)
        {
            Series.Add(seriesInfo);
            ((DataSeries) seriesInfo).Dirty = true;
        }

        void IRenderer.Clear()
        {
            Series.Clear();
            _analyzed = false;
        }

        internal IPlotElement CreateElement(object element, Style style)
        {
            IPlotElement element2 = null;
            DataTemplate template = element as DataTemplate;
            if (template != null)
            {
                return (template.LoadContent() as IPlotElement);
            }
            PlotElement element3 = element as PlotElement;
            if (element3 != null)
            {
                element2 = element3.Clone() as IPlotElement;
            }
            return element2;
        }

        protected virtual Shape CreateLineShape(DataSeries ds)
        {
            Shape sh = null;
            IPlotElement connectionElement = ds.GetConnectionElement(this, clear);
            if (connectionElement != null)
            {
                sh = connectionElement.LegendShape;
            }
            if (sh == null)
            {
                Rectangle rectangle = new Rectangle();
                sh = rectangle;
            }
            if (ds.ConnectionShape != null)
            {
                ds.ConnectionShape.Apply(sh);
            }
            return sh;
        }

        protected virtual Shape CreateSymbolShape(DataSeries ds)
        {
            Shape sh = null;
            IPlotElement symbolElement = ds.GetSymbolElement(this, clear);
            if (symbolElement != null)
            {
                sh = symbolElement.LegendShape;
            }
            if (sh == null)
            {
                double num;
                Rectangle rectangle = new Rectangle();
                rectangle.RadiusY = num = 3.0;
                rectangle.RadiusX = num;
                sh = rectangle;
            }
            if (ds.SymbolShape != null)
            {
                ds.SymbolShape.Apply(sh);
            }
            if ((sh != null) && (sh.StrokeThickness > 2.0))
            {
                if ((sh.StrokeThickness > 10.0) && (sh.Stroke != null))
                {
                    sh.Fill = sh.Stroke;
                }
                sh.StrokeThickness = 2.0;
            }
            return sh;
        }

        protected virtual Shape CreateSymbolShape(ShapeStyle shapeStyle)
        {
            double num;
            Rectangle sh = new Rectangle();
            sh.RadiusY = num = 3.0;
            sh.RadiusX = num;
            if (shapeStyle != null)
            {
                shapeStyle.Apply(sh);
            }
            return sh;
        }

        internal void FireChanged(object sender, EventArgs e)
        {
            _dirty = true;
            clear = true;
            if (Changed != null)
            {
                Changed(sender, e);
            }
        }

        internal void FireRendered(object sender, EventArgs e)
        {
            if (Rendered != null)
            {
                Rendered(sender, e);
            }
        }

        internal virtual double GetClusterWidth()
        {
            if (_visual is Chart)
            {
                double size = BarColumnOptions.GetSize((Chart) _visual);
                if (size > 0.0)
                {
                    return size;
                }
            }
            if (!Inverted)
            {
                return SizeX;
            }
            return SizeY;
        }

        internal void GetMinMaxY(int pointIndex, ref double ymin, ref double ymax)
        {
            using (List<IDataSeriesInfo>.Enumerator enumerator = Series.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    double[,] values = enumerator.Current.GetValues();
                    if (values != null)
                    {
                        int length = values.GetLength(0);
                        int num2 = values.GetLength(1);
                        if (((length > 0) && (pointIndex >= 0)) && (pointIndex < num2))
                        {
                            double num3 = values[0, pointIndex];
                            if ((num3 < ymin) || double.IsNaN(ymin))
                            {
                                ymin = num3;
                            }
                            if ((num3 > ymax) || double.IsNaN(ymax))
                            {
                                ymax = num3;
                            }
                        }
                    }
                }
            }
        }

        internal double GetOrigin()
        {
            if (SymbolCash is ISupportOrigin)
            {
                return 0.0;
            }
            return double.NaN;
        }

        internal Style GetStyle(DataSeries ds, int si, int pi)
        {
            if (ds.SymbolStyle != null)
            {
                return ds.SymbolStyle;
            }
            if (Styles != null)
            {
                string elementName = ds.GetElementName(si, pi);
                return (Styles[elementName] as Style);
            }
            return null;
        }

        internal virtual string GetValue(string name)
        {
            switch (name)
            {
                case "Inverted":
                    return Inverted.ToString();

                case "Stacked":
                    return Stacked.ToString();
            }
            return "";
        }

        protected virtual void InitOptions()
        {
            OptionsBag.Add("Inverted");
            OptionsBag.Add("Stacked");
        }

        internal bool IsCustomClipping(Rect rc)
        {
            return true;
        }

        internal virtual bool IsValidAuxAxis(string axname)
        {
            return false;
        }

        internal virtual int[] NClustered(bool defval)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            int num = 0;
            ChartView view = ((Chart != null) && (Chart.View != null)) ? Chart.View : null;
            foreach (DataSeries series in Series)
            {
                if (view != null)
                {
                    if (Inverted)
                    {
                        Axis axis = view.Axes[series.AxisX];
                        if (((axis == null) || (axis.PlotAreaIndex == 0)) || !series.GetIsClustered(defval))
                        {
                            goto Label_0135;
                        }
                        if (dictionary.ContainsKey(axis.PlotAreaIndex))
                        {
                            Dictionary<int, int> dictionary2;
                            int num5;
                            (dictionary2 = dictionary)[num5 = axis.PlotAreaIndex] = dictionary2[num5] + 1;
                        }
                        else
                        {
                            dictionary[axis.PlotAreaIndex] = 1;
                        }
                        continue;
                    }
                    Axis axis2 = view.Axes[series.AxisY];
                    if (((axis2 != null) && (axis2.PlotAreaIndex != 0)) && series.GetIsClustered(defval))
                    {
                        if (dictionary.ContainsKey(axis2.PlotAreaIndex))
                        {
                            Dictionary<int, int> dictionary3;
                            int num6;
                            (dictionary3 = dictionary)[num6 = axis2.PlotAreaIndex] = dictionary3[num6] + 1;
                        }
                        else
                        {
                            dictionary[axis2.PlotAreaIndex] = 1;
                        }
                        continue;
                    }
                }
            Label_0135:
                if (series.GetIsClustered(defval))
                {
                    num++;
                }
            }
            if (dictionary.Count == 0)
            {
                return new int[] { num };
            }
            int num2 = 1;
            foreach (int num3 in dictionary.Keys)
            {
                if ((num3 + 1) > num2)
                {
                    num2 = num3 + 1;
                }
            }
            int[] numArray = new int[num2];
            foreach (int num4 in dictionary.Keys)
            {
                numArray[num4] = dictionary[num4];
            }
            numArray[0] = num;
            return numArray;
        }

        internal int NStackGroups()
        {
            int num = 0;
            foreach (DataSeries series in Series)
            {
                num = Math.Max(num, BarColumnOptions.GetStackGroup(series));
            }
            return (num + 1);
        }

        protected PlotElement RenderConnection(DataSeries ds, RenderContext rc, int i)
        {
            PlotElement element = null;
            if (ds.Connection is PlotElement)
            {
                element = ((PlotElement) ds.Connection).Clone() as PlotElement;
            }
            if ((element == null) && ds.ChartType.HasValue)
            {
                ChartSubtype subtype = ChartTypes.GetSubtype(ds.ChartType.ToString());
                if ((subtype != null) && (subtype.Connection is PlotElement))
                {
                    element = ((PlotElement) subtype.Connection).Clone() as PlotElement;
                }
            }
            if ((!ds.ChartType.HasValue && ds.IsDefaultConnection) && ((element == null) && (Connection is PlotElement)))
            {
                element = ((PlotElement) Connection).Clone() as PlotElement;
            }
            if ((element != null) && ((IPlotElement) element).Render(rc))
            {
                element.DataPoint = new DataPoint(ds, i, -1, null);
                element.PlotRect = rc.Bounds;
                ds.ConnectionShape = StyleGen.GetStyle2(i);
                ((IPlotElement) element).SetShape(ds.ConnectionShape);
            }
            return element;
        }

        protected UIElement RenderElement(List<UIElement> objects, IPlotElement pe, DataSeries ds, RenderContext rc, ShapeStyle shapeStyle, DataPoint dp)
        {
            DataTemplate pointLabelTemplate = ds.PointLabelTemplate;
            PlotElement element = pe as PlotElement;
            if (((ItemNames != null) && (dp.PointIndex < ItemNames.Length)) && (ItemNames[dp.PointIndex] != null))
            {
                dp.Name = ItemNames[dp.PointIndex].ToString();
            }
            FrameworkElement lbl = null;
            Line line = null;
            if (pointLabelTemplate != null)
            {
                lbl = pointLabelTemplate.LoadContent() as FrameworkElement;
                lbl.DataContext = dp;
                if (Canvas.GetZIndex(lbl) == 0)
                {
                    Canvas.SetZIndex(lbl, 1);
                }
                if (pe != null)
                {
                    pe.Label = lbl;
                }
                else
                {
                    PlotElement.UpdateLabelPosition(element, lbl);
                }
                line = PlotElement.GetLabelLine(lbl);
            }
            if (pe == null)
            {
                return null;
            }
            Size size = ds.GetSymbolSize(dp.PointIndex, _dataInfo, rc.Chart);
            if (!size.IsEmpty && (element != null))
            {
                element.Size = size;
            }
            if (!pe.Render(rc))
            {
                return null;
            }
            if (lbl != null)
            {
                objects.Add(lbl);
                if (line != null)
                {
                    lbl.Loaded += (sender, e) =>
                    {
                        Canvas parent = (Canvas)lbl.Parent;
                        if (parent != null)
                        {
                            parent.Children.Add(line);
                        }
                    };
                }
            }
            if ((element.Center.X == 0.0) && (element.Center.Y == 0.0))
            {
                element.Center = rc.Current;
            }
            element.PlotRect = rc.Bounds;
            PlotElement.UpdateLabelPosition(element, pe.Label as FrameworkElement);
            if (element.DataContext == null)
            {
                element.DataContext = dp;
            }
            element.DataPoint = dp;
            AttachTooltip(ds, element);
            pe.SetShape(shapeStyle);
            ds.Children.Add((UIElement) pe);
            return (UIElement) pe;
        }

        internal virtual void SetValue(string name, string value)
        {
            string str = name;
            if (str != null)
            {
                if (str != "Inverted")
                {
                    if (str != "Stacked")
                    {
                        return;
                    }
                }
                else
                {
                    Inverted = bool.Parse(value);
                    return;
                }
                Stacked = (StackedOptions) Enum.Parse( typeof(StackedOptions), value, true);
            }
        }

        internal virtual void UpdateLegend(IList<LegendItem> litems)
        {
            using (IEnumerator<LegendItem> enumerator = litems.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Clear();
                }
            }
            litems.Clear();
            if (ColorScheme == Dt.Charts.ColorScheme.Point)
            {
                AnalyzeData(IsStacked);
                int nser = _dataInfo.nser;
                int npts = _dataInfo.npts;
                if (nser > 0)
                {
                    DataSeries series = Series[0] as DataSeries;
                    for (int i = 0; i < npts; i++)
                    {
                        ShapeStyle shapeStyle = StyleGen.GetStyle(i);
                        string lbl = null;
                        object item = null;
                        if ((ItemNames != null) && (i < ItemNames.Length))
                        {
                            item = ItemNames[i];
                            if (item != null)
                            {
                                lbl = item.ToString();
                            }
                        }
                        else
                        {
                            lbl = "Item " + ((int) i).ToString((IFormatProvider) CultureInfo.CurrentCulture);
                        }
                        Shape sym = CreateSymbolShape(shapeStyle);
                        if (series.ReadLocalValue(DataSeries.SymbolStrokeProperty) != DependencyProperty.UnsetValue)
                        {
                            sym.Stroke = series.SymbolStroke;
                        }
                        if (series.ReadLocalValue(DataSeries.SymbolStrokeThicknessProperty) != DependencyProperty.UnsetValue)
                        {
                            sym.StrokeThickness = Math.Min(2.0, series.SymbolStrokeThickness);
                        }
                        LegendItem item2 = new LegendItem(sym, null, lbl, item);
                        litems.Add(item2);
                    }
                }
            }
            else
            {
                int num4 = Series.Count;
                for (int j = 0; j < num4; j++)
                {
                    DataSeries series2 = Series[j] as DataSeries;
                    if (series2 != null)
                    {
                        if ((series2.SymbolShape == null) || !_stgen.List.Contains(series2.SymbolShape))
                        {
                            series2.SymbolShape = _stgen.Next();
                        }
                        if (series2.ConnectionShape == null)
                        {
                            series2.ConnectionShape = _stgen.List2[_stgen.List.Count - 1];
                        }
                        if (((byte) (series2.Display & SeriesDisplay.HideLegend)) == 0)
                        {
                            LegendItem item3;
                            object symbol = Symbol;
                            object connection = Connection;
                            if (series2.ChartType.HasValue)
                            {
                                ChartSubtype subtype = ChartTypes.GetSubtype(series2.ChartType.ToString());
                                symbol = subtype.Symbol;
                                connection = subtype.Connection;
                            }
                            bool flag = (((symbol != null) && series2.IsDefaultSymbol) || (series2.Symbol != null)) || (series2.SymbolMarker != Marker.None);
                            bool flag2 = ((connection != null) && series2.IsDefaultConnection) || (series2.Connection != null);
                            IPlotElement connectionElement = series2.GetConnectionElement(this, clear);
                            if ((!flag && flag2) && (connectionElement is Area))
                            {
                                item3 = new LegendItem(flag2 ? CreateLineShape(series2) : null, null, series2.Label, series2);
                            }
                            else
                            {
                                item3 = new LegendItem(flag ? CreateSymbolShape(series2) : null, flag2 ? CreateLineShape(series2) : null, series2.Label, series2);
                            }
                            litems.Add(item3);
                        }
                    }
                }
                clear = false;
            }
        }

        internal virtual Dt.Charts.AxisView AxisView
        {
            get { return  Dt.Charts.AxisView.None; }
        }

        bool IRenderer.Dirty
        {
            get { return  _dirty; }
            set { _dirty = value; }
        }

        string IRenderer.Options
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (string str in OptionsBag)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(';');
                    }
                    builder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", new object[] { str, (char) '=', GetValue(str) });
                }
                return builder.ToString();
            }
            set
            {
                string str = value;
                if (str != null)
                {
                    string[] strArray = str.Split(new char[] { ';' });
                    if ((strArray != null) && (strArray.Length > 0))
                    {
                        int length = strArray.Length;
                        for (int i = 0; i < length; i++)
                        {
                            string[] strArray2 = strArray[i].Split(new char[] { '=' });
                            if (((strArray2 != null) && (strArray2.Length == 2)) && OptionsBag.Contains(strArray2[0]))
                            {
                                SetValue(strArray2[0], strArray2[1]);
                            }
                        }
                    }
                }
            }
        }

        UIElement IRenderer.Visual
        {
            get { return  _visual; }
            set { _visual = value; }
        }

        internal Chart Chart
        {
            get { return _visual as Chart; }
        }

        public Dt.Charts.ColorScheme ColorScheme
        {
            get { return  _clrScheme; }
            set
            {
                _clrScheme = value;
                FireChanged(this, EventArgs.Empty);
            }
        }

        public object Connection
        {
            get { return  _conn; }
            set
            {
                _conn = value;
                FireChanged(this, EventArgs.Empty);
            }
        }

        [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
        public virtual ICoordConverter CoordConverter
        {
            get { return  _coordConverter; }
            set { _coordConverter = value; }
        }

        public bool Inverted
        {
            get { return  _inverted; }
            set
            {
                _inverted = value;
                FireChanged(this, EventArgs.Empty);
            }
        }

        internal bool IsStacked
        {
            get { return  (Stacked != StackedOptions.None); }
        }

        internal bool IsStacked100
        {
            get { return  (Stacked == StackedOptions.Stacked100pc); }
        }

        internal object[] ItemNames
        {
            get { return  _itemnames; }
            set { _itemnames = value; }
        }

        internal List<string> OptionsBag
        {
            get
            {
                if (_optbag == null)
                {
                    _optbag = new List<string>();
                    InitOptions();
                }
                return _optbag;
            }
        }

        internal List<IDataSeriesInfo> Series
        {
            get
            {
                if (_series == null)
                {
                    _series = new List<IDataSeriesInfo>();
                }
                return _series;
            }
        }

        public double SizeX
        {
            get { return  _sizeX; }
            set
            {
                _sizeX = value;
                FireChanged(this, EventArgs.Empty);
            }
        }

        public double SizeY
        {
            get { return  _sizeY; }
            set
            {
                _sizeY = value;
                FireChanged(this, EventArgs.Empty);
            }
        }

        public StackedOptions Stacked
        {
            get { return  _stacked; }
            set
            {
                _stacked = value;
                FireChanged(this, EventArgs.Empty);
            }
        }

        internal StyleGenerator StyleGen
        {
            get { return  _stgen; }
            set { _stgen = value; }
        }

        internal ResourceDictionary Styles
        {
            get { return  _styles; }
            set { _styles = value; }
        }

        public object Symbol
        {
            get { return  _symbol; }
            set
            {
                _symbol = value;
                FireChanged(this, EventArgs.Empty);
            }
        }

        internal PlotElement SymbolCash
        {
            get
            {
                if ((_symbolCash == null) && (Symbol is DataTemplate))
                {
                    _symbolCash = ((DataTemplate) Symbol).LoadContent() as PlotElement;
                }
                return _symbolCash;
            }
        }
    }
}

