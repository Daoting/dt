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
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public class Renderer2D : BaseRenderer, IView2DRenderer, IRenderer
    {
        Rect _rect = new Rect();
        static double cosPi6 = Math.Cos(0.52359877559829882);
        static object DefSymbol = new DotSymbol();
        static double sinPi6 = Math.Sin(0.52359877559829882);
        Brush TransparentBrush = new SolidColorBrush(Colors.Transparent);
        internal string[] ValidAuxAxes;

        public Renderer2D()
        {
            base.Symbol = DefSymbol;
        }

        internal override void AdjustAxisInternal(IAxis ax, double delta)
        {
            ax.Visible = true;
            switch (ax.AxisType)
            {
                case AxisType.X:
                {
                    if (_rect.Width <= 0.0)
                    {
                        break;
                    }
                    ValueLabels labels = ValueLabels.Create(ax.Min, ax.Max, ax);
                    if ((base.ItemNames == null) || base.Inverted)
                    {
                        ax.AddLabels(labels.Vals, labels.Lbls);
                        return;
                    }
                    ax.AddLabels(labels.Vals, base.ItemNames);
                    return;
                }
                case AxisType.Y:
                {
                    if (_rect.Height <= 0.0)
                    {
                        break;
                    }
                    ValueLabels labels2 = ValueLabels.Create(ax.Min, ax.Max, ax);
                    if ((base.ItemNames == null) || !base.Inverted)
                    {
                        ax.AddLabels(labels2.Vals, labels2.Lbls);
                        break;
                    }
                    ax.AddLabels(labels2.Vals, base.ItemNames);
                    return;
                }
                default:
                    return;
            }
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
            Rect dataBounds = CoordConverter.DataBounds;
            bool isStacked = base.IsStacked;
            StackedSum sum = null;
            StackedSum sum2 = null;
            StackedSum sum3 = null;
            int nsg = 1;
            if (isStacked)
            {
                nsg = base.NStackGroups();
                sum = new StackedSum(nsg);
                sum2 = new StackedSum(nsg);
                sum3 = new StackedSum(nsg);
            }

            double num4 = GetClusterWidth() * _dataInfo.DeltaX;
            IPlotElement defel = null;
            if (base.Symbol is PlotElement)
            {
                defel = ((PlotElement) base.Symbol).Clone() as IPlotElement;
            }

            bool defval = (defel != null) ? defel.IsClustered : false;
            bool inverted = base.Inverted;
            int num5 = 0;
            int[] numArray = null;
            int[] numArray2 = null;
            if (isStacked)
            {
                num5 = base.NStackGroups();
            }
            else
            {
                numArray = NClustered(defval);
                numArray2 = new int[numArray.Length];
                num5 = numArray[0];
            }

            double d = 0.0;
            double origin = 0.0;
            double seriesOverlap = 0.0;
            Chart visual = Chart;
            string xformat = null;
            string yformat = null;
            bool flag4 = false;
            bool flag5 = false;
            if (visual != null)
            {
                origin = BarColumnOptions.GetOrigin(visual);
                xformat = visual.View.AxisX.AnnoFormatInternal;
                yformat = visual.View.AxisY.AnnoFormatInternal;
                flag4 = !double.IsNaN(visual.View.AxisX.LogBase);
                flag5 = !double.IsNaN(visual.View.AxisY.LogBase);
                seriesOverlap = BarColumnOptions.GetSeriesOverlap(visual);
                if (seriesOverlap > 1.0)
                {
                    seriesOverlap = 1.0;
                }
                else if (seriesOverlap < -1.0)
                {
                    seriesOverlap = -1.0;
                }
            }

            int si = 0;
            int num10 = 0;
            while (si < nser)
            {
                IDataSeriesInfo ds = base.Series[si];
                DataSeries series = (DataSeries) base.Series[si];
                series.Children.Clear();
                objects.Add(series);
                series.SetDefaultFormat(xformat, yformat);
                int stackGroup = 0;
                if (isStacked)
                {
                    stackGroup = BarColumnOptions.GetStackGroup(series);
                }
                if (series.SymbolShape == null)
                {
                    series.SymbolShape = base.StyleGen.Next();
                }
                ShapeStyle symbolShape = series.SymbolShape;
                bool? nullable = null;
                double[,] values = ds.GetValues();
                if (values != null)
                {
                    int length = values.GetLength(1);
                    int num13 = values.GetLength(0);
                    RenderContext rc = new RenderContext(this, ds, length) {
                        Bounds = CoordConverter.ViewBounds
                    };
                    Rect clipBounds = rc.ClipBounds;
                    clipBounds = new Rect(clipBounds.X - 1.0, clipBounds.Y - 1.0, clipBounds.Width + 2.0, clipBounds.Height + 2.0);
                    double naN = double.NaN;
                    if ((rc.OptimizationRadiusScope & OptimizationRadiusScope.Symbols) > ((OptimizationRadiusScope) 0))
                    {
                        naN = rc.OptimizationRadius;
                    }
                    bool isClustered = defval;
                    bool flag7 = series.IsStacked;
                    int num15 = 0;
                    int num16 = length;
                    if (_dataInfo.incX && !inverted)
                    {
                        Rect rect2 = CoordConverter.DataBounds2D;
                        for (int j = 0; j < length; j++)
                        {
                            double num18 = (num13 >= 2) ? values[1, j] : ((double) j);
                            if (num18 >= rect2.Left)
                            {
                                num15 = Math.Max(0, j - 2);
                                break;
                            }
                        }
                        for (int k = num15; k < length; k++)
                        {
                            double num20 = (num13 >= 2) ? values[1, k] : ((double) k);
                            if (num20 > rect2.Right)
                            {
                                num16 = k;
                                break;
                            }
                        }
                        num16 = Math.Min(length, num16 + 2);
                    }
                    Point point = new Point();
                    for (int i = num15; i < num16; i++)
                    {
                        double num22 = values[0, i];
                        double x = (num13 >= 2) ? values[1, i] : ((double) i);
                        double y = x;
                        Style style = null;
                        IPlotElement pe = null;
                        bool? nullable2 = nullable;
                        if (!nullable2.GetValueOrDefault() || !nullable2.HasValue)
                        {
                            if (base.ColorScheme == ColorScheme.Point)
                            {
                                symbolShape = base.StyleGen.GetStyle(i);
                            }
                            style = base.GetStyle(series, si, i);
                            pe = base.CreateElement(series.Symbol, style);
                            if (pe == null)
                            {
                                pe = PlotElement.SymbolFromMarker(series.SymbolMarker);
                                if ((pe != null) && (style != null))
                                {
                                    pe.Style = style;
                                }
                            }
                            if ((pe == null) && series.ChartType.HasValue)
                            {
                                ChartSubtype subtype = ChartTypes.GetSubtype(series.ChartType.ToString());
                                if (subtype != null)
                                {
                                    pe = base.CreateElement(subtype.Symbol, style);
                                }
                            }
                            else if (pe == null)
                            {
                                if (series.IsDefaultSymbol && (defel != null))
                                {
                                    pe = defel;
                                    pe.Style = style;
                                    defel = ((PlotElement) base.Symbol).Clone() as IPlotElement;
                                }
                            }
                            else
                            {
                                isClustered = pe.IsClustered;
                            }
                            if (pe == null)
                            {
                                isClustered = false;
                                if ((series.PointLabelTemplate != null) || (series.PointTooltipTemplate != null))
                                {
                                    DotSymbol symbol = new DotSymbol();
                                    symbol.Fill = TransparentBrush;
                                    symbol.Size = new Size(5.0, 5.0);
                                    symbol.Stroke = TransparentBrush;
                                    pe = symbol;
                                }
                                else
                                {
                                    nullable = true;
                                }
                            }
                            else
                            {
                                isClustered = pe.IsClustered;
                            }
                        }
                        bool flag8 = (flag4 && inverted) || (flag5 && !inverted);
                        if (!flag8)
                        {
                            if (nsg > 1)
                            {
                                if (flag7)
                                {
                                    x += ((-0.5 * num4) + ((0.5 * num4) / ((double) num5))) + ((stackGroup * num4) / ((double) num5));
                                }
                            }
                            else if (isClustered)
                            {
                                int clusterPlotAreaIndex = rc.ClusterPlotAreaIndex;
                                if (clusterPlotAreaIndex == 0)
                                {
                                    if (num5 > 1)
                                    {
                                        x += ((-0.5 * num4) + ((0.5 * num4) / ((double) num5))) + ((num10 * num4) / ((double) num5));
                                    }
                                }
                                else if (numArray[clusterPlotAreaIndex] > 1)
                                {
                                    x += ((-0.5 * num4) + ((0.5 * num4) / ((double) numArray[clusterPlotAreaIndex]))) + ((numArray2[clusterPlotAreaIndex] * num4) / ((double) numArray[clusterPlotAreaIndex]));
                                }
                            }
                        }
                        if (isStacked && flag7)
                        {
                            if (double.IsNaN(num22))
                            {
                                num22 = 0.0;
                            }
                            if (base.IsStacked100)
                            {
                                if (_dataInfo.Stacked[stackGroup, x] != 0.0)
                                {
                                    num22 = ((num22 * 100.0) / _dataInfo.Stacked[stackGroup, x]) + sum[stackGroup, x];
                                }
                            }
                            else
                            {
                                num22 += sum[stackGroup, x];
                            }
                        }
                        double num26 = x;
                        if (inverted)
                        {
                            double num27 = x;
                            x = num22;
                            num22 = num27;
                        }
                        double num28 = rc.ConvertX(x);
                        double num29 = rc.ConvertY(num22);
                        if (((isStacked && flag7) && (si > 0)) && ((inverted && !double.IsNaN(num29)) || (!inverted && !double.IsNaN(num28))))
                        {
                            rc.SetPrevious(sum[stackGroup, num26]);
                        }
                        nullable2 = nullable;
                        if (!nullable2.GetValueOrDefault() || !nullable2.HasValue)
                        {
                            if (isClustered)
                            {
                                int num30 = num5;
                                if (rc.ClusterPlotAreaIndex > 0)
                                {
                                    num30 = numArray[rc.ClusterPlotAreaIndex];
                                }
                                if (num30 > 0)
                                {
                                    if (inverted)
                                    {
                                        d = Math.Abs((double) (CoordConverter.ConvertY((num4 / ((double) num30)) + num22) - CoordConverter.ConvertY(num22)));
                                    }
                                    else
                                    {
                                        d = Math.Abs((double) (CoordConverter.ConvertX((num4 / ((double) num30)) + x) - CoordConverter.ConvertX(x)));
                                    }
                                }
                            }
                            else if (inverted)
                            {
                                d = Math.Abs((double) (CoordConverter.ConvertY(num4 + num22) - CoordConverter.ConvertY(num22)));
                            }
                            else
                            {
                                d = Math.Abs((double) (CoordConverter.ConvertX(num4 + x) - CoordConverter.ConvertX(x)));
                            }
                            if (double.IsNaN(d))
                            {
                                d = inverted ? (base.SizeY * 20.0) : (base.SizeX * 20.0);
                            }
                        }
                        if (flag8)
                        {
                            double num31 = 0.0;
                            double num32 = d * num5;
                            if (nsg > 1)
                            {
                                if (flag7)
                                {
                                    num31 = ((-0.5 * num32) + ((0.5 * num32) / ((double) num5))) + ((stackGroup * num32) / ((double) num5));
                                }
                            }
                            else if ((num5 > 1) && isClustered)
                            {
                                num31 = ((-0.5 * num32) + ((0.5 * num32) / ((double) num5))) + ((num10 * num32) / ((double) num5));
                            }
                            if (num31 != 0.0)
                            {
                                if (inverted)
                                {
                                    num29 += num31;
                                }
                                else
                                {
                                    num28 += num31;
                                }
                            }
                        }
                        if (isStacked && flag7)
                        {
                            StackedSum sum4;
                            int num63;
                            double num64;
                            double num33 = values[0, i];
                            if (double.IsNaN(num33))
                            {
                                num33 = 0.0;
                            }
                            double num34 = (num33 >= 0.0) ? sum2[stackGroup, num26] : sum3[stackGroup, num26];
                            double num35 = (num33 >= 0.0) ? (sum2[stackGroup, num26] + num33) : (sum3[stackGroup, num26] + num33);
                            if (base.IsStacked100)
                            {
                                num34 = (num34 * 100.0) / _dataInfo.Stacked[stackGroup, num26];
                                num35 = (num35 * 100.0) / _dataInfo.Stacked[stackGroup, num26];
                            }
                            if (inverted)
                            {
                                double num36;
                                double num37;
                                double num38;
                                if (flag4)
                                {
                                    double minX = rc.GetMinX(CoordConverter.DataBounds2D);
                                    num36 = CoordConverter.ConvertX(Math.Max(num35, minX));
                                    num37 = CoordConverter.ConvertX(Math.Max(num34, minX));
                                }
                                else
                                {
                                    num36 = CoordConverter.ConvertX(num35);
                                    num37 = CoordConverter.ConvertX(num34);
                                }
                                if (flag5)
                                {
                                    num38 = base.SizeY * 20.0;
                                }
                                else
                                {
                                    num38 = Math.Abs((double) (CoordConverter.ConvertY(num4 / ((double) nsg)) - CoordConverter.ConvertY(0.0)));
                                }
                                double width = Math.Abs((double) (num37 - num36));
                                rc.Rect = new Rect(Math.Min(num36, num37), num29 - (0.5 * num38), width, num38);
                                sum[stackGroup, num26] = x;
                            }
                            else
                            {
                                double num41;
                                double num42;
                                double num43;
                                if (flag5)
                                {
                                    double minY = rc.GetMinY(CoordConverter.DataBounds2D);
                                    num41 = CoordConverter.ConvertY(Math.Max(num35, minY));
                                    num42 = CoordConverter.ConvertY(Math.Max(num34, minY));
                                }
                                else
                                {
                                    num41 = CoordConverter.ConvertY(num35);
                                    num42 = CoordConverter.ConvertY(num34);
                                }
                                if (flag4)
                                {
                                    num43 = base.SizeX * 20.0;
                                }
                                else
                                {
                                    num43 = Math.Abs((double) (CoordConverter.ConvertX(num4 / ((double) nsg)) - CoordConverter.ConvertX(0.0)));
                                }
                                double height = Math.Abs((double) (num42 - num41));
                                rc.Rect = new Rect(num28 - (0.5 * num43), Math.Min(num41, num42), num43, height);
                                sum[stackGroup, num26] = num22;
                            }
                            if (num33 >= 0.0)
                            {
                                (sum4 = sum2)[num63 = stackGroup, num64 = num26] = sum4[num63, num64] + num33;
                            }
                            else
                            {
                                (sum4 = sum3)[num63 = stackGroup, num64 = num26] = sum4[num63, num64] + num33;
                            }
                        }
                        else
                        {
                            nullable2 = nullable;
                            if (!nullable2.GetValueOrDefault() || !nullable2.HasValue)
                            {
                                double num46 = origin;
                                if (inverted)
                                {
                                    double num47 = rc.GetMinX(CoordConverter.DataBounds2D);
                                    double maxX = rc.GetMaxX(CoordConverter.DataBounds2D);
                                    if (num46 < num47)
                                    {
                                        num46 = num47;
                                    }
                                    else if (num46 > maxX)
                                    {
                                        num46 = maxX;
                                    }
                                    double num49 = rc.ConvertX(num46);
                                    double num50 = d;
                                    double num51 = Math.Abs((double) (num49 - num28));
                                    if (seriesOverlap != 0.0)
                                    {
                                        double num52 = rc.ConvertY(y);
                                        double num53 = d * num5;
                                        if (seriesOverlap > 0.0)
                                        {
                                            num50 = num53 / ((num5 - (num5 * seriesOverlap)) + seriesOverlap);
                                        }
                                        else
                                        {
                                            num50 *= 1.0 + seriesOverlap;
                                        }
                                        double num54 = (num53 - num50) / ((double) (num5 - 1));
                                        rc.Rect = new Rect(Math.Min(num28, num49), (num52 - (0.5 * num53)) + (((num5 - num10) - 1) * num54), num51, num50);
                                    }
                                    else
                                    {
                                        rc.Rect = new Rect(Math.Min(num28, num49), num29 - (0.5 * num50), num51, num50);
                                    }
                                }
                                else
                                {
                                    double num55 = rc.GetMinY(CoordConverter.DataBounds2D);
                                    double maxY = rc.GetMaxY(CoordConverter.DataBounds2D);
                                    if (num46 < num55)
                                    {
                                        num46 = num55;
                                    }
                                    else if (num46 > maxY)
                                    {
                                        num46 = maxY;
                                    }
                                    double num57 = rc.ConvertY(num46);
                                    double num58 = d;
                                    double num59 = Math.Abs((double) (num57 - num29));
                                    if (seriesOverlap != 0.0)
                                    {
                                        double num60 = rc.ConvertX(y);
                                        double num61 = d * num5;
                                        if (seriesOverlap > 0.0)
                                        {
                                            num58 = num61 / ((num5 - (num5 * seriesOverlap)) + seriesOverlap);
                                        }
                                        else
                                        {
                                            num58 *= 1.0 + seriesOverlap;
                                        }
                                        double num62 = (num61 - num58) / ((double) (num5 - 1));
                                        rc.Rect = new Rect((num60 - (0.5 * num61)) + (num10 * num62), Math.Min(num29, num57), num58, num59);
                                    }
                                    else
                                    {
                                        rc.Rect = new Rect(num28 - (0.5 * num58), Math.Min(num29, num57), num58, num59);
                                    }
                                }
                            }
                        }
                        rc.SetPoint(i, num28, num29);
                        if (pe != null)
                        {
                            Point point2 = new Point(num28, num29);
                            if ((pe is ICustomClipping) || clipBounds.Contains(point2))
                            {
                                if (double.IsNaN(naN))
                                {
                                    DataPoint dp = series.CreateDataPoint(si, i);
                                    base.RenderElement(objects, pe, series, rc, symbolShape, dp);
                                }
                                else if (((i == 0) || (Math.Abs((double) (point2.X - point.X)) > naN)) || ((Math.Abs((double) (point2.Y - point.Y)) > naN) || (i == (num16 - 1))))
                                {
                                    DataPoint point4 = series.CreateDataPoint(si, i);
                                    base.RenderElement(objects, pe, series, rc, symbolShape, point4);
                                    point = point2;
                                }
                            }
                        }
                    }

                    PlotElement element3 = base.RenderConnection(series, rc, si);
                    if (element3 != null)
                    {
                        series.Children.Insert(0, element3);
                    }

                    if (isClustered)
                    {
                        if (rc.ClusterPlotAreaIndex == 0)
                        {
                            num10++;
                        }
                        else
                        {
                            numArray2[rc.ClusterPlotAreaIndex]++;
                        }
                    }
                }
                si++;
            }
            return objects.ToArray();
        }

        Rect IView2DRenderer.Measure(Size sz)
        {
            base.AnalyzeData(base.IsStacked);
            Rect rect = new Rect();
            double num = (Enumerable.Sum(NClustered(Utils.GetIsClustered(false, base.Symbol))) > 0) ? ((0.5 * GetClusterWidth()) * _dataInfo.DeltaX) : 0.0;
            double num2 = 0.0;
            if (num == 0.0)
            {
                num2 = Math.Max(GetMaxSymbolWidth(), _dataInfo.SymbolSize.Width);
            }
            double height = _dataInfo.SymbolSize.Height;
            if ((_dataInfo.nser > 0) && (_dataInfo.npts > 0))
            {
                double minY;
                double maxY;
                if (_dataInfo.ndim >= 2)
                {
                    rect.X = _dataInfo.MinX;
                    rect.Width = _dataInfo.MaxX - _dataInfo.MinX;
                }
                else
                {
                    rect.X = 0.0;
                    rect.Width = _dataInfo.npts - 1;
                }
                double num4 = base.Inverted ? sz.Height : sz.Width;
                if ((num2 > 0.0) && (num2 < num4))
                {
                    double num5 = (num4 / (num4 - num2)) - 1.0;
                    num = (num5 * rect.Width) * 0.6;
                }
                rect.X -= num;
                rect.Width += 2.0 * num;
                if (rect.Width == 0.0)
                {
                    rect.X -= 0.5;
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
                    if ((Chart.ChartType == ChartType.Column) || (Chart.ChartType == ChartType.Bar))
                    {
                        Chart visual = Chart;
                        if (visual != null)
                        {
                            double origin = BarColumnOptions.GetOrigin(visual);
                            if (((minY > origin) && (visual.View != null)) && (visual.View.AxisY != null))
                            {
                                if (double.IsNaN(visual.View.AxisY.LogBase))
                                {
                                    minY = origin;
                                }
                                else if (origin > 0.0)
                                {
                                    minY = origin;
                                }
                            }
                        }
                    }
                }
                if (minY == maxY)
                {
                    double num9 = (minY == 0.0) ? 0.5 : Math.Abs((double) (minY * 0.1));
                    minY -= num9;
                    maxY += num9;
                }
                rect.Y = minY;
                rect.Height = maxY - minY;
                double num10 = 0.0;
                double num11 = base.Inverted ? sz.Width : sz.Height;
                if ((height > 0.0) && (height < num11))
                {
                    double num12 = (num11 / (num11 - height)) - 1.0;
                    num10 = (num12 * rect.Height) * 0.6;
                }
                rect.Y -= num10;
                rect.Height += 2.0 * num10;
            }
            _rect = base.Inverted ? new Rect(rect.Y, rect.X, rect.Height, rect.Width) : rect;
            return _rect;
        }

        double GetMaxSymbolWidth()
        {
            double num = 0.0;
            foreach (DataSeries series in base.Series)
            {
                if (series is IMeasureSymbolWidth)
                {
                    DataTemplate symbol = series.Symbol as DataTemplate;
                    if (symbol == null)
                    {
                        symbol = base.Symbol as DataTemplate;
                    }
                    if (symbol != null)
                    {
                        PlotElement element = symbol.LoadContent() as PlotElement;
                        if (element != null)
                        {
                            num = Math.Max(num, element.Size.Width);
                        }
                    }
                    else if (base.Symbol is PlotElement)
                    {
                        num = Math.Max(num, ((PlotElement) base.Symbol).Size.Width);
                    }
                }
            }
            return num;
        }
        
        internal static bool IsLine(ChartType? ct)
        {
            return (ct.HasValue && ct.ToString().StartsWith("Line"));
        }

        internal override bool IsValidAuxAxis(string axname)
        {
            if (ValidAuxAxes != null)
            {
                for (int i = 0; i < ValidAuxAxes.Length; i++)
                {
                    if (ValidAuxAxes[i] == axname)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        internal override Dt.Charts.AxisView AxisView
        {
            get { return  Dt.Charts.AxisView.Cartseian; }
        }

        AxisStyle IView2DRenderer.Axis
        {
            get { return  AxisStyle.Cartesian; }
        }
    }
}

