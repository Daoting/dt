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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public class PieRenderer : BasePieRenderer, IView2DRenderer, IRenderer
    {
        int _ncols;
        int _nrows;
        Size _sz = new Size();
        object DefSymbol = new PieSlice();

        public PieRenderer()
        {
            base.Symbol = DefSymbol;
        }

        UIElement[] IView2DRenderer.Generate()
        {
            int nser = _dataInfo.nser;
            int npts = _dataInfo.npts;
            if ((nser == 0) || (npts == 0))
            {
                return null;
            }
            List<UIElement> objects = new List<UIElement>();
            int num3 = 0;
            int num4 = 0;
            double width = _sz.Width;
            double height = _sz.Height;
            double num7 = width / ((double) _ncols);
            double num8 = height / ((double) _nrows);
            double num9 = 0.5 * Math.Min((double) (width / ((double) _ncols)), (double) (height / ((double) _nrows)));
            double naN = double.NaN;
            double startingAngle = 0.0;
            SweepDirection clockwise = SweepDirection.Clockwise;
            bool flag = false;
            Chart chart = base.Chart;
            double d = 0.0;
            if (chart != null)
            {
                startingAngle = PieOptions.GetStartingAngle(chart);
                clockwise = PieOptions.GetDirection(chart);
                flag = base.Chart.ChartType == ChartType.PieStacked;
                naN = PieOptions.GetOffset(chart);
                d = PieOptions.GetInnerRadius(chart);
            }
            if (double.IsNaN(naN))
            {
                naN = base.Offset;
            }
            if (double.IsNaN(d) || (d == 0.0))
            {
                d = base.InnerRadius;
            }
            for (int i = 0; i < nser; i++)
            {
                Point point = new Point();
                double radiusX = num9 * base.Radius;
                double inner = d;
                if (flag)
                {
                    inner = (((double) i) / ((double) nser)) * radiusX;
                    radiusX *= ((double) (i + 1)) / ((double) nser);
                    inner /= radiusX;
                }
                point = new Point((num4 - (0.5 * _ncols)) + 0.5, (num3 - (0.5 * _nrows)) + 0.5) {
                    X = (num7 * point.X) + (0.5 * width),
                    Y = (num8 * point.Y) + (0.5 * height)
                };
                IDataSeriesInfo info = Series[i];
                DataSeries series = info as DataSeries;
                series.Children.Clear();
                objects.Add(series);
                double num16 = _dataInfo.SumsAbs[i];
                double[,] values = info.GetValues();
                if (!string.IsNullOrEmpty(series.Label))
                {
                    DataTemplate seriesLabelTemplate = null;
                    if (chart != null)
                    {
                        seriesLabelTemplate = PieOptions.GetSeriesLabelTemplate(chart);
                    }
                    UIElement el = null;
                    if (seriesLabelTemplate != null)
                    {
                        el = seriesLabelTemplate.LoadContent() as UIElement;
                        FrameworkElement element2 = el as FrameworkElement;
                        if (element2 != null)
                        {
                            element2.DataContext = series;
                        }
                    }
                    if (el == null)
                    {
                        TextBlock block = new TextBlock();
                        block.Text = series.Label;
                        IRenderer renderer = this;
                        Control visual = renderer.Visual as Control;
                        if (visual != null)
                        {
                            block.FontFamily = visual.FontFamily;
                            block.FontSize = visual.FontSize;
                            block.FontStretch = visual.FontStretch;
                            block.FontStyle = visual.FontStyle;
                            block.FontWeight = visual.FontWeight;
                        }
                        el = block;
                    }
                    Size size = Utils.GetSize(el);
                    Canvas.SetLeft(el, point.X - (0.5 * size.Width));
                    if ((size.Height + radiusX) > (0.5 * num8))
                    {
                        radiusX = (0.5 * num8) - size.Height;
                    }
                    if (flag)
                    {
                        Canvas.SetTop(el, (point.Y - (radiusX * inner)) - size.Height);
                    }
                    else
                    {
                        Canvas.SetTop(el, (point.Y - radiusX) - size.Height);
                    }
                    objects.Add(el);
                }
                double angle = startingAngle;
                double num18 = 0.0;
                if (naN > 0.0)
                {
                    num18 = naN * radiusX;
                    radiusX *= 1.0 - naN;
                }
                int num19 = (values != null) ? values.GetLength(1) : 0;
                for (int j = 0; j < num19; j++)
                {
                    double num21 = values[0, j];
                    if (!double.IsNaN(num21) && (num21 != 0.0))
                    {
                        ShapeStyle shapeStyle = StyleGen.GetStyle(j);
                        double sweep = (Math.Abs(num21) / num16) * 360.0;
                        if (sweep != 0.0)
                        {
                            if (clockwise == SweepDirection.Counterclockwise)
                            {
                                sweep = -sweep;
                            }
                            Point center = point;
                            if ((num18 > 0.0) && (sweep != 360.0))
                            {
                                double num23 = 0.017453292519943295 * (angle + (0.5 * sweep));
                                center.X += num18 * Math.Cos(num23);
                                center.Y += num18 * Math.Sin(num23);
                            }
                            PieRenderContext rc = new PieRenderContext();
                            if (sweep < 0.0)
                            {
                                rc.PieInfo = new PieInfo(center, radiusX, radiusX, inner, angle + sweep, -sweep, 0.0, 0.0);
                            }
                            else
                            {
                                rc.PieInfo = new PieInfo(center, radiusX, radiusX, inner, angle, sweep, 0.0, 0.0);
                            }
                            DataPoint dp = series.CreateDataPoint(i, j);
                            IPlotElement pe = null;
                            if ((series.Symbol is PlotElement) && ((IPlotElement) series.Symbol).IsCompatible(this))
                            {
                                pe = ((PlotElement) series.Symbol).Clone() as IPlotElement;
                            }
                            if ((pe == null) && (Symbol is PlotElement))
                            {
                                pe = ((PlotElement) Symbol).Clone() as IPlotElement;
                            }
                            RenderElement(objects, pe, series, rc, shapeStyle, dp);
                            if (series.ReadLocalValue(DataSeries.SymbolStrokeProperty) != DependencyProperty.UnsetValue)
                            {
                                ((PlotElement) pe).Stroke = series.SymbolStroke;
                            }
                            if (series.ReadLocalValue(DataSeries.SymbolStrokeThicknessProperty) != DependencyProperty.UnsetValue)
                            {
                                ((PlotElement) pe).StrokeThickness = series.SymbolStrokeThickness;
                            }
                            angle += sweep;
                        }
                    }
                }
                if (!flag)
                {
                    num4++;
                    if (num4 >= _ncols)
                    {
                        num4 = 0;
                        num3++;
                    }
                }
            }
            return objects.ToArray();
        }

        Rect IView2DRenderer.Measure(Size sz)
        {
            if (_sz != sz)
            {
                _sz = sz;
            }
            Rect rect = new Rect();
            AnalyzeData(false);
            _nrows = 0;
            _ncols = 0;
            int nser = _dataInfo.nser;
            int npts = _dataInfo.npts;
            if (((nser > 0) && (npts > 0)) && PerformLayout(nser, sz))
            {
                rect.X = -0.5 * _ncols;
                rect.Width = _ncols;
                rect.Y = -0.5 * _nrows;
                rect.Height = _nrows;
            }
            return rect;
        }

        bool PerformLayout(int nPies, Size sz)
        {
            _ncols = nPies;
            _nrows = 1;
            if (nPies < 1)
            {
                return false;
            }
            if ((sz.Height <= 0.0) || (sz.Width <= 0.0))
            {
                return false;
            }
            if (Chart.ChartType == ChartType.PieStacked)
            {
                nPies = 1;
            }
            double num = 0.0;
            for (int i = 1; i <= nPies; i++)
            {
                int num3 = (nPies + (i - 1)) / i;
                double num4 = sz.Width / ((double) i);
                double num5 = sz.Height / ((double) num3);
                if (num5 < num4)
                {
                    num4 = num5;
                }
                if (num4 > num)
                {
                    num = num4;
                    _ncols = i;
                    _nrows = num3;
                }
            }
            return true;
        }

        AxisStyle IView2DRenderer.Axis
        {
            get { return  AxisStyle.None; }
        }
    }
}

