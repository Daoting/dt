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
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public class RadarRenderer : BaseRenderer, IView2DRenderer, IRenderer
    {
        Brush TransparentBrush = new SolidColorBrush(Colors.Transparent);

        UIElement[] IView2DRenderer.Generate()
        {
            int nser = _dataInfo.nser;
            int npts = _dataInfo.npts;
            if ((nser == 0) || (npts == 0))
            {
                return null;
            }
            IPlotElement element = null;
            if (base.Symbol is PlotElement)
            {
                element = ((PlotElement) base.Symbol).Clone() as IPlotElement;
            }
            List<UIElement> objects = new List<UIElement>();
            for (int i = 0; i < nser; i++)
            {
                DataSeries ds = (DataSeries) base.Series[i];
                ds.Children.Clear();
                objects.Add(ds);
                if (ds.SymbolShape == null)
                {
                    ds.SymbolShape = base.StyleGen.Next();
                }
                ShapeStyle symbolShape = ds.SymbolShape;
                double[,] values = ds.GetValues();
                if (values != null)
                {
                    int length = values.GetLength(1);
                    int num5 = values.GetLength(0);
                    RenderContext rc = new RenderContext(this, ds, length) {
                        Bounds = CoordConverter.ViewBounds
                    };
                    for (int j = 0; j < length; j++)
                    {
                        if (base.ColorScheme == ColorScheme.Point)
                        {
                            symbolShape = base.StyleGen.GetStyle(j);
                        }
                        double y = values[0, j];
                        double x = (num5 >= 2) ? values[1, j] : ((double) j);
                        bool flag = false;
                        Style style = base.GetStyle(ds, i, j);
                        IPlotElement pe = base.CreateElement(ds.Symbol, style);
                        if (pe == null)
                        {
                            pe = PlotElement.SymbolFromMarker(ds.SymbolMarker);
                            if ((pe != null) && (style != null))
                            {
                                pe.Style = style;
                            }
                        }
                        if ((pe == null) && ds.ChartType.HasValue)
                        {
                            ChartSubtype subtype = ChartTypes.GetSubtype(ds.ChartType.ToString());
                            if (subtype != null)
                            {
                                pe = base.CreateElement(subtype.Symbol, style);
                            }
                        }
                        else if (((pe == null) && ds.IsDefaultSymbol) && (element != null))
                        {
                            pe = element;
                            pe.Style = style;
                            element = ((PlotElement) base.Symbol).Clone() as IPlotElement;
                        }
                        if ((pe == null) && ((ds.PointLabelTemplate != null) || (ds.PointTooltipTemplate != null)))
                        {
                            DotSymbol symbol = new DotSymbol();
                            symbol.Fill = TransparentBrush;
                            symbol.Size = new Size(5.0, 5.0);
                            symbol.Stroke = TransparentBrush;
                            pe = symbol;
                            flag = true;
                        }
                        Point point = CoordConverter.ConvertPoint(new Point(x, y));
                        Axis auxAxis = GetAuxAxis(j);
                        if (auxAxis != null)
                        {
                            ChartViewport2D coordConverter = CoordConverter as ChartViewport2D;
                            if (coordConverter != null)
                            {
                                point = coordConverter.ConvertPoint(new Point(x, y), base.Chart.View.AxisX, auxAxis);
                            }
                        }
                        rc.SetPoint(j, point.X, point.Y);
                        if (!double.IsNaN(point.X) && !double.IsNaN(point.Y))
                        {
                            DataPoint dp = ds.CreateDataPoint(i, j);
                            UIElement element3 = base.RenderElement(objects, pe, ds, rc, symbolShape, dp);
                            if ((element3 != null) && flag)
                            {
                                Canvas.SetZIndex(element3, 2);
                            }
                        }
                    }
                    PlotElement element4 = base.RenderConnection(ds, rc, i);
                    if (element4 != null)
                    {
                        element4.DataPoint = ds.CreateDataPoint(i, -1);
                        ds.Children.Insert(0, element4);
                    }
                }
            }
            return objects.ToArray();
        }

        Rect IView2DRenderer.Measure(Size sz)
        {
            base.AnalyzeData(base.IsStacked);
            Rect rect = new Rect();
            if ((_dataInfo.nser > 0) && (_dataInfo.npts > 0))
            {
                double minY;
                double maxY;
                if (IsPolar)
                {
                    rect.X = 0.0;
                    rect.Width = 360.0;
                }
                else if (_dataInfo.ndim >= 2)
                {
                    rect.X = _dataInfo.MinX;
                    rect.Width = _dataInfo.MaxX - _dataInfo.MinX;
                }
                else
                {
                    rect.X = 0.0;
                    rect.Width = _dataInfo.npts;
                }
                if (rect.Width == 0.0)
                {
                    rect.Width = 1.0;
                }
                if (base.IsStacked)
                {
                    if (base.IsStacked100)
                    {
                        minY = _dataInfo.MinVals[0] * 100.0;
                        maxY = _dataInfo.MaxVals[0] * 100.0;
                        if (minY == maxY)
                        {
                            minY = 0.0;
                            maxY = 100.0;
                        }
                    }
                    else
                    {
                        minY = _dataInfo.MinVals[0];
                        maxY = _dataInfo.MaxVals[0];
                        if (maxY < 0.0)
                        {
                            maxY = 0.0;
                        }
                        if (minY > 0.0)
                        {
                            minY = 0.0;
                        }
                    }
                }
                else
                {
                    minY = _dataInfo.MinY;
                    maxY = _dataInfo.MaxY;
                }
                if (minY == maxY)
                {
                    double num3 = (minY == 0.0) ? 0.5 : Math.Abs((double) (minY * 0.1));
                    minY -= num3;
                    maxY += num3;
                }
                rect.Y = minY;
                rect.Height = maxY - minY;
            }
            return rect;
        }

        Axis GetAuxAxis(int pointIndex)
        {
            foreach (Axis axis in base.Chart.View.Axes)
            {
                if ((axis != base.Chart.View.AxisY) && (axis.AxisType == AxisType.Y))
                {
                    IList<int> radarPointIndices = axis.RadarPointIndices;
                    if (radarPointIndices != null)
                    {
                        using (IEnumerator<int> enumerator2 = radarPointIndices.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                if (enumerator2.Current == pointIndex)
                                {
                                    return axis;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal override string GetValue(string name)
        {
            if (name == "IsPolar")
            {
                return IsPolar.ToString();
            }
            return base.GetValue(name);
        }

        protected override void InitOptions()
        {
            base.InitOptions();
            base.OptionsBag.Add("IsPolar");
        }

        internal override void SetValue(string name, string value)
        {
            string str;
            if (((str = name) != null) && (str == "IsPolar"))
            {
                IsPolar = bool.Parse(value);
            }
            base.SetValue(name, value);
        }

        AxisStyle IView2DRenderer.Axis
        {
            get { return  AxisStyle.Radar; }
        }

        [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
        public bool IsPolar { get; set; }
    }
}

