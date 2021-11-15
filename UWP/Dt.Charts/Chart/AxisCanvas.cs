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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
    public partial class AxisCanvas : Canvas, IAxis
    {
        UIElement[] _albls;
        Size _annoSize = new Size();
        internal Dt.Charts.Axis _axis;
        Chart _chart;
        Size _desiredSize = Size.Empty;
        List<object> _lbls = new List<object>();
        internal double _max = double.NaN;
        double _max0 = double.NaN;
        internal double _min = double.NaN;
        double _min0 = double.NaN;
        internal Rect _plot = new Rect();
        Rect _r = Extensions.EmptyRect;
        double _scale = 1.0;
        TextBlock _tbtitle;
        internal UIElement[] _ticks;
        Size _titleDesiredSize = Size.Empty;
        List<double> _vals = new List<double>();
        const string fmt100pc = "0'%'";
        const string fmtDate = "d";

        internal AxisCanvas(Dt.Charts.Axis axis)
        {
            _axis = axis;
            if (axis != null)
            {
                axis.iax = this;
            }
        }

        void IAxis.AddLabels(double[] vals, object[] lbls)
        {
            if (_axis.ItemsSource != null)
            {
                CreateItemsLabels(_axis.ItemsSource, false);
            }
            else
            {
                _vals.AddRange(vals);
                _lbls.AddRange(lbls);
            }
        }

        void IAxis.ClearLabels()
        {
            _vals.Clear();
            _lbls.Clear();
        }

        string IAxis.Format(double val)
        {
            if ((((IAxis)this).LogBase == 2.7182818284590451) && string.IsNullOrEmpty(_axis.AnnoFormat))
            {
                return ("e" + Format(Math.Log(val), "0.##"));
            }
            return Format(val, ((IAxis)this).AnnoFormat);
        }

        string IAxis.Format(object val)
        {
            if (!string.IsNullOrEmpty(((IAxis)this).AnnoFormat))
            {
                IFormattable formattable = val as IFormattable;
                if (formattable != null)
                {
                    return formattable.ToString(((IAxis)this).AnnoFormat, (IFormatProvider)null);
                }
            }
            return val.ToString();
        }

        double IAxis.FromData(double val)
        {
            return ConvertEx(val);
        }

        int IAxis.GetAnnoNumber()
        {
            if ((AnnoSize.Width <= 0.0) || (base.Width <= 0.0))
            {
                return 10;
            }
            int num = (int)(base.Width / AnnoSize.Width);
            if (num <= 0)
            {
                num = 1;
            }
            return num;
        }

        double IAxis.ToData(double val)
        {
            double logBase = _axis.LogBase;
            if (double.IsNaN(logBase))
            {
                if (ReversedInternal)
                {
                    if (_axis.AxisType == AxisType.Y)
                    {
                        return (_min + (((_max - _min) * (val - _r.Y)) / base.Width));
                    }
                    return (_min + ((_max - _min) * (1.0 - ((val - _r.X) / base.Width))));
                }
                if (_axis.AxisType == AxisType.Y)
                {
                    return (_min + ((_max - _min) * (1.0 - ((val - _r.Y) / base.Width))));
                }
                return (_min + (((_max - _min) * (val - _r.X)) / base.Width));
            }
            double num2 = 0.0;
            if (ReversedInternal)
            {
                if (_axis.AxisType == AxisType.Y)
                {
                    num2 = (val - _r.X) / base.Width;
                }
                else
                {
                    num2 = 1.0 - ((val - _r.Y) / base.Width);
                }
            }
            else if (_axis.AxisType == AxisType.Y)
            {
                num2 = 1.0 - ((val - _r.Y) / base.Width);
            }
            else
            {
                num2 = (val - _r.X) / base.Width;
            }
            return Math.Pow(logBase, Math.Log(_min, logBase) + ((Math.Log(_max, logBase) - Math.Log(_min, logBase)) * num2));
        }

        internal void ClearLabelsAndTicks()
        {
            List<UIElement> list = new List<UIElement>();
            int num = base.Children.Count;
            for (int i = 0; i < num; i++)
            {
                list.Add((UIElement)Children[i]);
            }
            for (int j = 0; j < num; j++)
            {
                if ((list[j] != TitleInternal) && (list[j] != ScrollBarInternal))
                {
                    base.Children.Remove(list[j]);
                }
            }
        }

        double Convert(double val)
        {
            if (_max == _min)
            {
                return 0.0;
            }
            double logBase = _axis.LogBase;
            if (double.IsNaN(logBase))
            {
                if (ReversedInternal)
                {
                    if (_axis.AxisType == AxisType.Y)
                    {
                        return (((val - _min) / (_max - _min)) * base.Width);
                    }
                    return (base.Width - (((val - _min) / (_max - _min)) * base.Width));
                }
                if (_axis.AxisType == AxisType.Y)
                {
                    return (base.Width - (((val - _min) / (_max - _min)) * base.Width));
                }
                return (((val - _min) / (_max - _min)) * base.Width);
            }
            if (val <= 0.0)
            {
                return double.NaN;
            }
            if (ReversedInternal)
            {
                if (_axis.AxisType == AxisType.Y)
                {
                    return ((Math.Log(val / _min, logBase) / Math.Log(_max / _min, logBase)) * base.Width);
                }
                return (base.Width - ((Math.Log(val / _min, logBase) / Math.Log(_max / _min, logBase)) * base.Width));
            }
            if (_axis.AxisType == AxisType.Y)
            {
                return (base.Width - ((Math.Log(val / _min, logBase) / Math.Log(_max / _min, logBase)) * base.Width));
            }
            return ((Math.Log(val / _min, logBase) / Math.Log(_max / _min, logBase)) * base.Width);
        }

        internal double ConvertEx(double val)
        {
            if (_axis.AxisType == AxisType.Y)
            {
                return (_r.Y + Convert(val));
            }
            return (_r.X + Convert(val));
        }

        internal virtual UIElement CreateAnno(double val, object lbl)
        {
            if (lbl is UIElement)
            {
                return (UIElement)lbl;
            }
            UIElement el = null;
            if (AnnoTemplateInternal is DataTemplate)
            {
                DataTemplate annoTemplateInternal = (DataTemplate)AnnoTemplateInternal;
                el = annoTemplateInternal.LoadContent() as UIElement;
                if (el != null)
                {
                    AxisPoint point = new AxisPoint(Axis, val, lbl);
                    FrameworkElement element2 = el as FrameworkElement;
                    if (element2 != null)
                    {
                        element2.DataContext = point;
                    }
                }
            }
            if (el == null)
            {
                TextBlock block = new TextBlock();
                if (_axis.AnnoTemplate is TextBlock)
                {
                    TextBlock annoTemplate = (TextBlock)_axis.AnnoTemplate;
                    block.Foreground = Utils.Clone(annoTemplate.Foreground);
                }
                else if (_chart != null)
                {
                    block.Foreground = _axis.ForegroundInternal;
                }
                string str = "";
                if (lbl is double)
                {
                    str = ((IAxis)this).Format((double)((double)lbl));
                }
                else if (lbl is DateTime)
                {
                    str = ((IAxis)this).Format(((DateTime)lbl).ToOADate());
                }
                else
                {
                    str = (lbl != null) ? lbl.ToString() : ((IAxis)this).Format(val);
                }
                block.Text = str;
                el = block;
            }
            InitElement(el);
            return el;
        }

        internal void CreateAnnosAndTicks(bool isRadar)
        {
            if (_r.IsEmptyRect() || !_axis.Visible)
                return;

            double[] vals = _vals.ToArray();
            object[] itemsAsObjects = GetItemsAsObjects();
            int num = (vals != null) ? vals.Length : 0;
            int num2 = (itemsAsObjects != null) ? itemsAsObjects.Length : 0;
            int num3 = Math.Min(num, num2);
            _albls = new UIElement[num3];
            _ticks = new UIElement[num3];
            CreateMinorTicksAndGrid(vals);
            bool isNear = IsNear;
            double majorTickOverlap = Axis.MajorTickOverlap;
            bool flag2 = !double.IsNaN(_axis.LogBase);
            Brush majorGridStroke = _axis.MajorGridStroke;
            bool inner = (Axis.Position & AxisPosition.Inner) > AxisPosition.Near;
            double tickOffset = GetTickOffset(inner);
            int num6 = 0;
            Point[] pointArray = null;

            for (int i = num6; i < num3; i++)
            {
                if (vals[i] < _min || vals[i] > _max)
                    continue;

                if (flag2 && (vals[i] <= 0.0))
                    continue;

                double d = Convert(vals[i]);
                if (double.IsInfinity(d) || double.IsNaN(d))
                    continue;

                // 刻度值TextBlock
                UIElement el = (isRadar || (_axis.AnnoPosition == AnnoPosition.None)) ? null : CreateAnno(vals[i], itemsAsObjects[i]);
                Size size = new Size();
                if (el != null)
                {
                    size = Utils.GetSize(el);
                    double annoAngleInternal = AnnoAngleInternal;
                    double num10 = 0.0;
                    double num11 = 0.0;
                    Size size2 = size;
                    double num12 = 0.0;
                    double num13 = 1.0;
                    bool flag4 = (inner && isNear) || (!inner && !isNear);
                    if (annoAngleInternal != 0.0)
                    {
                        double a = annoAngleInternal * 0.017453292519943295;
                        num12 = Math.Abs(Math.Sin(a));
                        num13 = Math.Abs(Math.Cos(a));
                        double width = size.Width;
                        double height = size.Height;
                        double num17 = 0.5 * Math.Abs((double)((width * num13) + (height * num12)));
                        double num18 = 0.5 * Math.Abs((double)((width * num12) + (height * num13)));
                        num10 = num18 - (0.5 * height);
                        if (flag4)
                        {
                            num10 = -num10;
                        }
                        num11 = num17 - (0.5 * width);
                        size.Width = 2.0 * num17;
                        size.Height = 2.0 * num18;
                    }
                    double length = 0.0;
                    if (inner)
                    {
                        length = isNear ? (-((size.Height + tickOffset) + num10) * _scale) : (((_r.Height + num10) + tickOffset) * _scale);
                    }
                    else
                    {
                        length = isNear ? ((tickOffset + num10) * _scale) : (_r.Height - (((tickOffset + size.Height) + num10) * _scale));
                    }
                    Canvas.SetTop(el, length);
                    double num20 = d + num11;
                    if (_axis.AnnoPosition == AnnoPosition.Auto)
                    {
                        if ((Math.Abs(num12) > 0.01) && (_axis.AnnoAngle != 0.0))
                        {
                            double num21 = annoAngleInternal * 0.017453292519943295;
                            num12 = Math.Sin(num21);
                            if (_axis.AxisType == AxisType.X)
                            {
                                if (flag4)
                                {
                                    num12 = -num12;
                                }
                                if (num12 > 0.0)
                                {
                                    num20 -= ((0.5 * size2.Height) * _scale) * num12;
                                }
                                else
                                {
                                    num20 -= size.Width * _scale;
                                    num20 -= ((0.5 * size2.Height) * _scale) * num12;
                                }
                            }
                            else if (_axis.AxisType == AxisType.Y)
                            {
                                num12 = Math.Abs(num12);
                                if (!flag4)
                                {
                                    if (_axis.AnnoAngle > 0.0)
                                    {
                                        num20 -= size.Height * _scale;
                                        num20 += ((0.5 * size2.Height) * _scale) * num12;
                                    }
                                    else
                                    {
                                        num20 -= ((0.5 * size2.Height) * _scale) * num12;
                                    }
                                }
                                else if (_axis.AnnoAngle < 0.0)
                                {
                                    num20 -= size.Height * _scale;
                                    num20 += ((0.5 * size2.Height) * _scale) * num12;
                                }
                                else
                                {
                                    num20 -= ((0.5 * size2.Height) * _scale) * num12;
                                }
                            }
                        }
                        else
                        {
                            num20 -= (0.5 * size.Width) * _scale;
                        }
                    }
                    if (_axis.AnnoPosition == AnnoPosition.Far)
                    {
                        num20 -= size.Width * _scale;
                    }
                    Canvas.SetLeft(el, num20);
                    if (_scale != 1.0)
                    {
                        double num28;
                        ScaleTransform transform = new ScaleTransform();
                        transform.ScaleY = num28 = _scale;
                        transform.ScaleX = num28;
                        el.RenderTransform = transform;
                    }
                    if (annoAngleInternal != 0.0)
                    {
                        RotateTransform transform2 = new RotateTransform();
                        transform2.Angle = annoAngleInternal;
                        transform2.CenterX = (0.5 * size2.Width) * _scale;
                        transform2.CenterY = (0.5 * size2.Height) * _scale;
                        el.RenderTransform = transform2;
                    }
                    if (inner && ((num20 < 0.0) || ((num20 + size.Width) > _r.Width)))
                    {
                        el = null;
                    }
                    if (((Axis.Position & AxisPosition.DisableLastLabelOverflow) > AxisPosition.Near) && ((num20 + size.Width) > _r.Width))
                    {
                        el = null;
                    }
                    Point[] pointArray2 = null;
                    if ((Axis.AnnoVisibility == AnnoVisibility.HideOverlapped) && (el != null))
                    {
                        Point point = new Point(num20 + ((0.5 * size2.Width) * _scale), length + ((0.5 * size2.Height) * _scale));
                        pointArray2 = new Point[] { new Point(point.X - (0.5 * size2.Width), point.Y - (0.5 * size2.Height)), new Point(point.X + (0.5 * size2.Width), point.Y - (0.5 * size2.Height)), new Point(point.X + (0.5 * size2.Width), point.Y + (0.5 * size2.Height)), new Point(point.X - (0.5 * size2.Width), point.Y + (0.5 * size2.Height)) };
                        if (annoAngleInternal != 0.0)
                        {
                            RotateTransform transform4 = new RotateTransform();
                            transform4.Angle = annoAngleInternal;
                            transform4.CenterX = point.X;
                            transform4.CenterY = point.Y;
                            RotateTransform transform3 = transform4;
                            for (int j = 0; j < 4; j++)
                            {
                                pointArray2[j] = transform3.TransformPoint(pointArray2[j]);
                            }
                        }
                        if ((pointArray != null) && Intersect(pointArray, pointArray2))
                        {
                            el = null;
                        }
                    }
                    if (el != null)
                    {
                        AnnoCreatedEventArgs args = new AnnoCreatedEventArgs(this, el, i, vals[i]);
                        Axis.FireAnnoCreated(args);
                        if (args.Cancel)
                        {
                            el = null;
                        }
                        else
                        {
                            el = args.Label;
                        }
                    }
                    if ((el != null) && !base.Children.Contains(el))
                    {
                        base.Children.Add(el);
                    }
                    if (el != null)
                    {
                        pointArray = pointArray2;
                    }
                    _albls[i] = el;
                }

                // 刻度线
                Line line = new Line();
                line.Stroke = _axis.MajorTickStroke;
                if (line.Stroke == null)
                {
                    line.Stroke = _axis.ForegroundInternal;
                }
                if (line.Stroke == null)
                {
                    if (_chart != null)
                    {
                        line.Stroke = _chart.Foreground;
                    }
                    else
                    {
                        line.Stroke = new SolidColorBrush(Colors.Black);
                    }
                }
                line.StrokeThickness = _axis.MajorTickThickness;
                line.X2 = d;
                line.X1 = d;
                double num23 = MajorTickHeightInternal * _scale;
                if (isNear)
                {
                    line.Y1 = -num23 * majorTickOverlap;
                    line.Y2 = num23 * (1.0 - majorTickOverlap);
                }
                else
                {
                    line.Y1 = _r.Height - (num23 * (1.0 - majorTickOverlap));
                    line.Y2 = _r.Height + (num23 * majorTickOverlap);
                }
                base.Children.Add(line);
                _ticks[i] = line;

                // 网格区间背景
                if (_axis.MajorGridFill != null
                    && (i % 2) == 1
                    && vals[i - 1] >= _min
                    && vals[i - 1] <= _max)
                {
                    double num24 = 0.0;
                    double num25 = 0.0;
                    double num26 = 0.0;
                    double num27 = 0.0;
                    if (_axis.AxisType == AxisType.X)
                    {
                        num24 = Convert(Math.Max(vals[i - 1], _min));
                        num25 = Convert(Math.Min(_max, vals[i]));
                        num26 = _plot.Top - _r.Top;
                        num27 = _plot.Bottom - _r.Top;
                    }
                    else if (_axis.AxisType == AxisType.Y)
                    {
                        num24 = Convert(vals[i - 1]);
                        num25 = Convert(Math.Min(_max, vals[i]));
                        num26 = _r.Left - _plot.Width;
                        num27 = num26 + _plot.Width;
                    }
                    Rectangle element = new Rectangle();
                    element.Width = Math.Abs((double)(num25 - num24));
                    element.Height = Math.Abs((double)(num27 - num26));
                    element.Fill = Utils.Clone(_axis.MajorGridFill);
                    Canvas.SetLeft(element, Math.Min(num24, num25));
                    Canvas.SetTop(element, Math.Min(num26, num27));
                    base.Children.Add(element);
                }

                // 网格线
                if (!isRadar && majorGridStroke != null)
                {
                    DrawGridLine(majorGridStroke, _axis.MajorGridStrokeThickness, vals[i], isNear, _axis.MajorGridStrokeDashes);
                }
            }
        }

        internal void CreateAnnosAndTicksRadar(RadarView rv, AxisCanvas ay)
        {
            if (_axis.Visible)
            {
                double[] numArray = _vals.ToArray();
                object[] itemsAsObjects = GetItemsAsObjects();
                int num = (numArray != null) ? numArray.Length : 0;
                int num2 = (itemsAsObjects != null) ? itemsAsObjects.Length : 0;
                int num3 = Math.Min(num, num2);
                if (num3 > 0)
                {
                    _albls = new UIElement[num3];
                    UIElement[] elementArray1 = ay._ticks;
                    double angle = (6.2831853071795862 * (numArray[num3 - 1] - _min)) / (_max - _min);
                    angle = rv.GetAngle(angle);
                    List<Point> list = new List<Point>();
                    bool isPolar = rv.IsPolar;
                    Brush majorGridStroke = ay.Axis.MajorGridStroke;
                    if ((isPolar && (majorGridStroke != null)) && (ay._vals != null))
                    {
                        Path path2 = new Path();
                        path2.Stroke = majorGridStroke;
                        path2.StrokeThickness = ay.Axis.MajorGridStrokeThickness;
                        path2.StrokeDashArray = Utils.Clone(ay._axis.MajorGridStrokeDashes);
                        Path path = path2;
                        GeometryGroup group = new GeometryGroup();
                        for (int j = 0; j < ay._vals.Count; j++)
                        {
                            double num6 = (rv.Radius * (ay._vals[j] - ay._min)) / (ay._max - ay._min);
                            if (num6 <= rv.Radius)
                            {
                                EllipseGeometry geometry = new EllipseGeometry();
                                geometry.Center = rv.Center;
                                geometry.RadiusX = num6;
                                geometry.RadiusY = num6;
                                group.Children.Add(geometry);
                            }
                        }
                        path.Data = group;
                        base.Children.Add(path);
                    }
                    for (int i = 0; i < num3; i++)
                    {
                        double num8 = (6.2831853071795862 * (numArray[i] - _min)) / (_max - _min);
                        num8 = rv.GetAngle(num8);
                        double cos = Math.Cos(num8);
                        double sin = Math.Sin(num8);
                        bool haslabel = false;
                        if (ay._axis.RadarLabelVisibility == RadarLabelVisibility.All)
                        {
                            haslabel = true;
                        }
                        else if ((ay._axis.RadarLabelVisibility == RadarLabelVisibility.First) && (i == 0))
                        {
                            haslabel = true;
                        }
                        CreateYAxis(ay, rv, haslabel, !isPolar, num8, angle, cos, sin);
                        Dt.Charts.Axis axis = GetAxis(i);
                        haslabel = false;
                        if (axis != null)
                        {
                            if (axis.RadarLabelVisibility == RadarLabelVisibility.All)
                            {
                                haslabel = true;
                            }
                            else if (axis.RadarLabelVisibility == RadarLabelVisibility.First)
                            {
                                haslabel = (axis.RadarPointIndices != null) && (axis.RadarPointIndices[0] == i);
                            }
                            CreateYAxis((AxisCanvas)axis.iax, rv, haslabel, false, num8, angle, cos, sin);
                        }
                        if ((numArray[i] >= _min) && (numArray[i] < _max))
                        {
                            Line line = Utils.Clone(Axis.AxisLine);
                            line.X1 = rv.Center.X;
                            line.Y1 = rv.Center.Y;
                            double x = rv.Center.X + (rv.Radius * cos);
                            double y = rv.Center.Y + (rv.Radius * sin);
                            list.Add(new Point(x, y));
                            line.X2 = x;
                            line.Y2 = y;
                            base.Children.Add(line);
                            UIElement el = CreateAnno(numArray[i], itemsAsObjects[i]);
                            if (el != null)
                            {
                                base.Children.Add(el);
                                PositionLabel(el, x, y, num8);
                            }
                            if (axis != null)
                            {
                                CreateYAxisTicks(rv, (AxisCanvas)axis.iax, sin, cos);
                            }
                            else
                            {
                                CreateYAxisTicks(rv, ay, sin, cos);
                            }
                        }
                        angle = num8;
                    }
                    if (isPolar)
                    {
                        Path path4 = new Path();
                        path4.Stroke = _axis.ForegroundInternal;
                        path4.StrokeThickness = ay.Axis.AxisLine.StrokeThickness;
                        Path path3 = path4;
                        GeometryGroup group2 = new GeometryGroup();
                        EllipseGeometry geometry2 = new EllipseGeometry();
                        geometry2.Center = rv.Center;
                        geometry2.RadiusX = rv.Radius;
                        geometry2.RadiusY = rv.Radius;
                        group2.Children.Add(geometry2);
                        path3.Data = group2;
                        base.Children.Add(path3);
                    }
                    else if (list.Count > 2)
                    {
                        Path path5 = new Path();
                        PathGeometry geometry3 = new PathGeometry();
                        PathFigure figure = new PathFigure();
                        figure.IsClosed = true;
                        figure.StartPoint = list[0];
                        for (int k = 1; k < list.Count; k++)
                        {
                            LineSegment segment = new LineSegment();
                            segment.Point = list[k];
                            figure.Segments.Add(segment);
                        }
                        geometry3.Figures.Add(figure);
                        path5.Data = geometry3;
                        path5.Stroke = _axis.ForegroundInternal;
                        base.Children.Add(path5);
                    }
                }
            }
        }

        internal void CreateItemsLabels(IEnumerable source, bool skipConversion)
        {
            IList<KeyValuePair<object, double>> list = source as IList<KeyValuePair<object, double>>;
            if (list != null)
            {
                int num = list.Count;
                for (int i = 0; i < num; i++)
                {
                    _lbls.Add(list[i].Key);
                    _vals.Add(list[i].Value);
                }
            }
            else
            {
                IList<KeyValuePair<object, DateTime>> list2 = source as IList<KeyValuePair<object, DateTime>>;
                if (list2 != null)
                {
                    int num3 = list2.Count;
                    for (int j = 0; j < num3; j++)
                    {
                        _lbls.Add(list2[j].Key);
                        _vals.Add(list2[j].Value.ToOADate());
                    }
                }
                else
                {
                    int num5 = 0;
                    DataBindingProxy proxy = null;
                    if (_axis.ItemsLabelBinding != null)
                    {
                        proxy = new DataBindingProxy();
                    }
                    DataBindingProxy proxy2 = null;
                    if (_axis.ItemsValueBinding != null)
                    {
                        proxy2 = new DataBindingProxy();
                    }
                    foreach (object obj2 in source)
                    {
                        double naN = double.NaN;
                        if (proxy2 != null)
                        {
                            proxy2.DataContext = obj2;
                            if (_axis.IsTimeInternal(_chart))
                            {
                                naN = DataSeries.ConvertObjectDateTime(proxy2.GetValue(_axis.ItemsValueBinding), 2147483647.0);
                            }
                            else
                            {
                                naN = DataSeries.ConvertObjectNumber(proxy2.GetValue(_axis.ItemsValueBinding), 2147483647.0);
                            }
                        }
                        else if (_axis.IsTimeInternal(_chart))
                        {
                            naN = DataSeries.ConvertObjectDateTime(obj2, 2147483647.0);
                        }
                        else
                        {
                            naN = DataSeries.ConvertObjectNumber(obj2, 2147483647.0);
                        }
                        if (((naN == 2147483647.0) || double.IsNaN(naN)) || skipConversion)
                        {
                            naN = double.NaN;
                            _vals.Add((double)num5);
                        }
                        else
                        {
                            _vals.Add(naN);
                        }
                        if (proxy != null)
                        {
                            proxy.DataContext = obj2;
                            _lbls.Add(proxy.GetValue(_axis.ItemsLabelBinding));
                        }
                        else if (double.IsNaN(naN))
                        {
                            _lbls.Add(obj2);
                        }
                        else
                        {
                            _lbls.Add(((IAxis)this).Format(naN));
                        }
                        num5++;
                    }
                }
            }
        }

        internal void CreateLabels(IEnumerable source)
        {
            CreateLabels(source, 0.0);
        }

        internal void CreateLabels(IEnumerable source, double delta)
        {
            if (_axis.ItemsSource != null)
            {
                CreateItemsLabels(_axis.ItemsSource, false);
            }
            else if (source != null)
            {
                CreateItemsLabels(source, true);
            }
            else if (!double.IsNaN(_min) && !double.IsNaN(_max))
            {
                ValueLabels labels = ValueLabels.Create(_min, _max, this, delta);
                ((IAxis)this).AddLabels(labels.Vals, labels.Lbls);
            }
        }

        void CreateMinorTick(double val, bool near)
        {
            double num3;
            Line line = new Line();
            line.Stroke = _axis.MinorTickStroke;
            if (line.Stroke == null)
            {
                line.Stroke = _axis.ForegroundInternal;
            }
            if (line.Stroke == null)
            {
                if (_chart != null)
                {
                    line.Stroke = Utils.Clone(_chart.Foreground);
                }
                else
                {
                    line.Stroke = new SolidColorBrush(Colors.Black);
                }
            }
            line.StrokeThickness = _axis.MinorTickThickness;
            line.X2 = num3 = Convert(val);
            line.X1 = num3;
            double num = MinorTickHeightInternal * _scale;
            double minorTickOverlap = Axis.MinorTickOverlap;
            if (near)
            {
                line.Y1 = -num * minorTickOverlap;
                line.Y2 = num * (1.0 - minorTickOverlap);
            }
            else
            {
                line.Y1 = _r.Height - (num * (1.0 - minorTickOverlap));
                line.Y2 = _r.Height + (num * minorTickOverlap);
            }
            base.Children.Add(line);
            Brush minorGridStroke = _axis.MinorGridStroke;
            if (minorGridStroke != null)
            {
                DrawGridLine(minorGridStroke, _axis.MinorGridStrokeThickness, val, near, _axis.MinorGridStrokeDashes);
            }
        }

        void CreateMinorTicksAndGrid(double[] vals)
        {
            if ((Axis.ItemsSource == null) || !double.IsNaN(_axis.MinorUnit))
            {
                int length = vals.Length;
                bool isNear = IsNear;
                if (length >= 1)
                {
                    double unit = 0.0;
                    if (double.IsNaN(_axis.LogBase))
                    {
                        if (length > 1)
                        {
                            if (double.IsNaN(_axis.MinorUnit) || (_axis.MinorUnit <= 0.0))
                            {
                                unit = Math.Round((double)(0.5 * (vals[1] - vals[0])), 15);
                            }
                            else
                            {
                                unit = (double)_axis.MinorUnit;
                            }
                            if (unit > 0.0)
                            {
                                double num3 = vals[0];
                                while (num3 > _min)
                                {
                                    num3 -= unit;
                                }
                                double num1 = (_max - num3) / unit;
                                for (double i = num3; i < _max; i = Math.Round((double)(i + unit), 14))
                                {
                                    if (i < _min)
                                    {
                                        continue;
                                    }
                                    bool flag2 = true;
                                    double[] numArray2 = vals;
                                    for (int j = 0; j < numArray2.Length; j++)
                                    {
                                        double num5 = (double)numArray2[j];
                                        if (Math.Abs((double)(num5 - i)) < (0.01 * unit))
                                        {
                                            flag2 = false;
                                            break;
                                        }
                                    }
                                    if (flag2)
                                    {
                                        CreateMinorTick(i, isNear);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (double.IsNaN(_axis.MinorUnit) || (_axis.MinorUnit <= 0.0))
                        {
                            if (double.IsNaN(((IAxis)this).MajorUnit))
                            {
                                unit = 0.5 * _axis.LogBase;
                            }
                            else
                            {
                                unit = 0.5 * ((IAxis)this).MajorUnit;
                            }
                        }
                        else
                        {
                            unit = _axis.MinorUnit;
                        }
                        double[] numArray = ValueLabels.CreateLogarithmicValues(_min, _max, unit, this, false);
                        if (numArray != null)
                        {
                            int num6 = numArray.Length;
                            for (int k = 0; k < num6; k++)
                            {
                                if ((Array.BinarySearch<double>(vals, numArray[k]) < 0) && (numArray[k] >= _min))
                                {
                                    CreateMinorTick(numArray[k], isNear);
                                }
                            }
                        }
                    }
                }
            }
        }

        void CreateYAxis(AxisCanvas ay, RadarView rv, bool haslabel, bool hasGrid, double angle, double angle0, double cos, double sin)
        {
            if (ay._vals != null)
            {
                for (int i = 0; i < ay._vals.Count; i++)
                {
                    if ((ay._vals[i] >= ay._min) && (ay._vals[i] <= ay._max))
                    {
                        double d = (rv.Radius * (ay._vals[i] - ay._min)) / (ay._max - ay._min);
                        if (!double.IsNaN(d))
                        {
                            Point point = new Point(rv.Center.X + (d * cos), rv.Center.Y + (d * sin));
                            Point point2 = new Point(rv.Center.X + (d * Math.Cos(angle0)), rv.Center.Y + (d * Math.Sin(angle0)));
                            if (((d > 0.0) && (d < rv.Radius)) && (hasGrid && (ay.Axis.MajorGridStroke != null)))
                            {
                                Line line = new Line();
                                line.Stroke = Utils.Clone(ay._axis.MajorGridStroke);
                                line.StrokeThickness = ay._axis.MajorGridStrokeThickness;
                                line.StrokeDashArray = Utils.Clone(ay._axis.MajorGridStrokeDashes);
                                line.X1 = point2.X;
                                line.Y1 = point2.Y;
                                line.X2 = point.X;
                                line.Y2 = point.Y;
                                base.Children.Add(line);
                            }
                            if (haslabel)
                            {
                                UIElement el = ay.CreateAnno(ay._vals[i], ay._lbls[i]);
                                if (el != null)
                                {
                                    base.Children.Add(el);
                                    PositionLabel(el, point.X, point.Y, angle + 1.5707963267948966);
                                }
                            }
                        }
                    }
                }
            }
        }

        void CreateYAxisTicks(RadarView rv, AxisCanvas ay, double sin, double cos)
        {
            UIElement[] elementArray = ay._ticks;
            if (elementArray != null)
            {
                for (int i = 0; i < elementArray.Length; i++)
                {
                    Line line = elementArray[i] as Line;
                    if (line != null)
                    {
                        Line line2 = Utils.Clone(line);
                        double num2 = Math.Abs((double)(rv.Radius - line.X1));
                        double num3 = rv.Center.X + (num2 * cos);
                        double num4 = rv.Center.Y + (num2 * sin);
                        double num5 = ay.MajorTickHeightInternal * sin;
                        double num6 = ay.MajorTickHeightInternal * cos;
                        line2.X1 = num3 - num5;
                        line2.Y1 = num4 + num6;
                        line2.X2 = num3 + num5;
                        line2.Y2 = num4 - num6;
                        base.Children.Add(line2);
                    }
                }
            }
        }

        void DrawGridLine(Brush stroke, double thickness, double val, bool near, DoubleCollection dashes)
        {
            double left = 0.0;
            double top = 0.0;
            double right = 0.0;
            double bottom = 0.0;

            if (_axis.AxisType == AxisType.X)
            {
                left = right = Convert(val);
                top = _plot.Top - _r.Top;
                bottom = _plot.Bottom - _r.Top;
            }
            else if (_axis.AxisType == AxisType.Y)
            {
                left = right = Convert(val);
                top = _r.Left - _plot.Width;
                bottom = top + _plot.Width;
            }

            if ((dashes != null) && (dashes.Count > 0))
            {
                Line line2 = new Line();
                line2.X1 = left;
                line2.Y1 = top;
                line2.X2 = right;
                line2.Y2 = bottom;
                Line line = line2;
                line.Stroke = stroke;
                line.StrokeThickness = thickness;
                line.StrokeDashArray = Utils.Clone(dashes);
                base.Children.Add(line);
            }
            else
            {
                Rectangle rectangle2 = new Rectangle();
                rectangle2.Fill = stroke;
                Rectangle element = rectangle2;
                if (_axis.AxisType == AxisType.X)
                {
                    element.Width = thickness;
                    element.Height = Math.Round(_plot.Height);
                    Canvas.SetLeft(element, Math.Round((double)(left - (0.5 * thickness))));
                    Canvas.SetTop(element, Math.Round(top));
                }
                else if (_axis.AxisType == AxisType.Y)
                {
                    element.Width = thickness;
                    element.Height = Math.Round(_plot.Width);
                    Canvas.SetLeft(element, Math.Round((double)(left - (0.5 * thickness))));
                    Canvas.SetTop(element, top);
                }
                base.Children.Add(element);
            }
        }

        internal string Format(double val, string fmt)
        {
            if (_axis.IsTimeInternal(_chart))
            {
                DateTime time = val.FromOADate();
                if (!string.IsNullOrEmpty(fmt) && _axis.IsValidTimeFmt)
                {
                    return time.ToString(fmt, (IFormatProvider)CultureInfo.CurrentCulture);
                }
                return time.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            }
            if (!string.IsNullOrEmpty(fmt) && _axis.IsValidFmt)
            {
                return ((double)val).ToString(fmt, (IFormatProvider)CultureInfo.CurrentCulture);
            }
            return ((double)val).ToString((IFormatProvider)CultureInfo.CurrentCulture);
        }

        Dt.Charts.Axis GetAxis(int pointIndex)
        {
            foreach (Dt.Charts.Axis axis in Chart.View.Axes)
            {
                if (((axis != Chart.View.AxisX) && (axis != Chart.View.AxisY)) && (axis.AxisType == AxisType.Y))
                {
                    IList<int> radarPointIndices = axis.RadarPointIndices;
                    if ((radarPointIndices != null) && radarPointIndices.Contains(pointIndex))
                    {
                        return axis;
                    }
                }
            }
            return null;
        }

        object[] GetItemsAsObjects()
        {
            return _lbls.ToArray();
        }

        static Size GetMaxSize(params UIElement[] els)
        {
            Size size = new Size();
            int length = els.Length;
            for (int i = 0; i < length; i++)
            {
                UIElement el = els[i];
                if (el != null)
                {
                    Size size2 = Utils.GetSize(el);
                    if (size2.Width > size.Width)
                    {
                        size.Width = size2.Width;
                    }
                    if (size2.Height > size.Height)
                    {
                        size.Height = size2.Height;
                    }
                }
            }
            return size;
        }

        Rect GetRect(UIElement ui)
        {
            Size size = Utils.GetSize(ui);
            return new Rect(Canvas.GetLeft(ui), Canvas.GetTop(ui), size.Width, size.Height);
        }

        internal Size GetSize(IEnumerable items, bool isradar)
        {
            Size size = new Size();
            _annoSize = new Size();
            _titleDesiredSize = new Size();
            if (_axis.Visible)
            {
                if (TitleInternal != null)
                {
                    if (!base.Children.Contains(TitleInternal))
                    {
                        base.Children.Add(TitleInternal);
                    }
                    FrameworkElement titleInternal = TitleInternal as FrameworkElement;
                    if ((titleInternal != null) && (titleInternal.HorizontalAlignment == HorizontalAlignment.Stretch))
                    {
                        titleInternal.Width = double.NaN;
                    }
                    _titleDesiredSize = size = Utils.GetSize(TitleInternal);
                }
                UIElement scrollBarInternal = ScrollBarInternal;
                Size size2 = new Size();
                if ((scrollBarInternal != null) && !isradar)
                {
                    if (!base.Children.Contains(scrollBarInternal))
                    {
                        base.Children.Add(scrollBarInternal);
                    }
                    size2 = Utils.GetSize(scrollBarInternal);
                    size.Height += size2.Height;
                }
                Line axisLine = _axis.AxisLine;
                if (axisLine == null)
                {
                    axisLine = new Line();
                    _axis.SetAxisLine(axisLine);
                }
                axisLine.Stroke = _axis.ForegroundInternal;
                if (axisLine.Stroke == null)
                {
                    axisLine.Stroke = new SolidColorBrush(Colors.Black);
                }
                if (!isradar && !base.Children.Contains(axisLine))
                {
                    base.Children.Add(axisLine);
                }
                UpdateAnnosize(items);
                if ((_axis.Position & AxisPosition.Inner) == AxisPosition.Near)
                {
                    size.Height += AnnoSize.Height * AnnoRowNum;
                }
                size.Height += GetTickOffset(false);
                if (_axis.AxisType == AxisType.Y)
                {
                    size.Height += 4.0;
                }
            }
            _desiredSize = size;
            return size;
        }

        double GetTickOffset(bool inner = false)
        {
            double num = 0.0;
            if (inner)
            {
                num = Math.Max((double)(MinorTickHeightInternal * Axis.MinorTickOverlap), (double)(MajorTickHeightInternal * Axis.MajorTickOverlap));
            }
            else
            {
                num = Math.Max((double)(MinorTickHeightInternal * (1.0 - Axis.MinorTickOverlap)), (double)(MajorTickHeightInternal * (1.0 - Axis.MajorTickOverlap)));
            }
            if (_axis.AxisType == AxisType.Y)
            {
                num += 2.0;
            }
            return num;
        }

        void InitElement(UIElement el)
        {
            if ((_axis != null) && (_chart != null))
            {
                TextBlock block = el as TextBlock;
                FontFamily family = _axis.FontFamily ?? _chart.FontFamily;
                double num = !double.IsNaN(_axis.FontSize) ? _axis.FontSize : _chart.FontSize;
                if (block != null)
                {
                    SetProperty(block, TextBlock.FontFamilyProperty, family);
                    SetProperty(block, TextBlock.FontSizeProperty, (double)num);
                }
                else
                {
                    Control control = el as Control;
                    if (control != null)
                    {
                        SetProperty(control, Control.FontFamilyProperty, family);
                        SetProperty(control, Control.FontSizeProperty, (double)num);
                    }
                }
            }
        }

        bool Intersect(Point[] pts1, Point[] pts2)
        {
            return ((Utils.FindIntersection(pts1[0], pts1[1], pts2[1], pts2[2]) != 0) || ((Utils.FindIntersection(pts1[2], pts1[3], pts2[1], pts2[2]) != 0) || ((Utils.FindIntersection(pts1[0], pts1[1], pts2[0], pts2[3]) != 0) || ((Utils.FindIntersection(pts1[2], pts1[3], pts2[0], pts2[3]) != 0) || ((Utils.FindIntersection(pts1[1], pts1[2], pts2[0], pts2[1]) != 0) || ((Utils.FindIntersection(pts1[0], pts1[3], pts2[0], pts2[1]) != 0) || ((Utils.FindIntersection(pts1[1], pts1[2], pts2[2], pts2[3]) != 0) || (Utils.FindIntersection(pts1[0], pts1[3], pts2[2], pts2[3]) != 0))))))));
        }

        internal void Layout(Rect r)
        {
            if (double.IsNaN(r.Width) || double.IsNaN(r.Height))
                return;

            if ((Axis.Position & AxisPosition.OverData) > AxisPosition.Near)
                Canvas.SetZIndex(this, 1);
            else
                Canvas.SetZIndex(this, 0);

            _r = r;
            Width = r.Width;
            Height = r.Height;
            if (!_axis.Visible || r.IsEmptyRect())
            {
                if (TitleInternal != null)
                {
                    TitleInternal.Visibility = Utils.VisHidden;
                }
                if (_axis.AxisLine != null)
                {
                    _axis.AxisLine.Visibility = Utils.VisHidden;
                }
                return;
            }

            Visibility = Visibility.Visible;
            Canvas.SetTop(this, r.Y);
            if (_axis.AxisType == AxisType.Y)
            {
                Canvas.SetLeft(this, r.X + r.Height);
                RenderTransform = new RotateTransform { Angle = 90.0 };
            }
            else
            {
                Canvas.SetLeft(this, r.X);
            }

            // 坐标轴线
            bool isNear = IsNear;
            Line axisLine = _axis.AxisLine;
            if (axisLine != null)
            {
                axisLine.Visibility = Visibility.Visible;
                axisLine.X1 = 0.0;
                axisLine.X2 = r.Width;
                if (isNear)
                {
                    double num3;
                    axisLine.Y2 = num3 = 0.0;
                    axisLine.Y1 = num3;
                }
                else
                {
                    double num4;
                    axisLine.Y2 = num4 = r.Height;
                    axisLine.Y1 = num4;
                }
            }

            Size size = new Size();
            if (TitleInternal != null)
            {
                size = _titleDesiredSize;
            }

            double tickOffset = GetTickOffset(false);
            _scale = 1.0;
            FrameworkElement scrollBarInternal = ScrollBarInternal as FrameworkElement;
            Size size2 = new Size();
            if (scrollBarInternal != null)
            {
                Windows.UI.Xaml.Thickness scrollBarMargin = _axis.ScrollBar.ScrollBarMargin;
                if (isNear)
                {
                    Canvas.SetTop(scrollBarInternal, tickOffset + AnnoSize.Height);
                }
                else
                {
                    Canvas.SetTop(scrollBarInternal, size.Height);
                }
                Canvas.SetLeft(scrollBarInternal, scrollBarMargin.Left);
                scrollBarInternal.Width = (r.Width - scrollBarMargin.Left) - scrollBarMargin.Right;
                size2 = Utils.GetSize(scrollBarInternal);
            }

            if (TitleInternal != null)
            {
                TitleInternal.Visibility = Visibility.Visible;
                if ((_axis.AxisType == AxisType.Y) && IsNear)
                {
                    RotateTransform transform = new RotateTransform();
                    transform.Angle = 180.0;
                    TitleInternal.RenderTransform = transform;
                    if (TitleInternal is TextBlock)
                    {
                        transform.CenterX = 0.5 * size.Width;
                    }
                    else
                    {
                        TitleInternal.RenderTransformOrigin = new Point(0.5, 0.5);
                    }
                }
                else
                {
                    TitleInternal.RenderTransform = null;
                }
                FrameworkElement titleInternal = TitleInternal as FrameworkElement;
                if (titleInternal != null)
                {
                    switch (titleInternal.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            Canvas.SetLeft(TitleInternal, 0.0);
                            break;

                        case HorizontalAlignment.Center:
                            Canvas.SetLeft(TitleInternal, 0.5 * (r.Width - size.Width));
                            break;

                        case HorizontalAlignment.Right:
                            Canvas.SetLeft(TitleInternal, r.Width - size.Width);
                            break;

                        case HorizontalAlignment.Stretch:
                            {
                                double width = r.Width;
                                width -= titleInternal.Margin.Left + titleInternal.Margin.Right;
                                Canvas.SetLeft(TitleInternal, 0.0);
                                titleInternal.Width = width;
                                break;
                            }
                    }
                }
                else
                {
                    Canvas.SetLeft(TitleInternal, 0.5 * (r.Width - size.Width));
                }

                if (isNear)
                {
                    if ((_axis.AxisType == AxisType.Y) && (TitleInternal is TextBlock))
                    {
                        Canvas.SetTop(TitleInternal, ((tickOffset + AnnoSize.Height) + size.Height) + size2.Height);
                    }
                    else
                    {
                        Canvas.SetTop(TitleInternal, (tickOffset + AnnoSize.Height) + size2.Height);
                    }
                }
                else
                {
                    Canvas.SetTop(TitleInternal, 0.0);
                }
            }
        }

        void PositionLabel(UIElement el, double x, double y, double angle)
        {
            Size size = Utils.GetSize(el);
            Math.Atan2(size.Width, size.Height);
            double num = 0.65 * size.Width;
            double num2 = 0.65 * size.Height;
            double num3 = num * Math.Cos(angle + 3.1415926535897931);
            double num4 = num2 * Math.Sin(angle + 3.1415926535897931);
            x -= 0.5 * size.Width;
            y -= 0.5 * size.Height;
            Canvas.SetLeft(el, x - num3);
            Canvas.SetTop(el, y - num4);
        }

        internal void ResetLimits()
        {
            if (_axis.AutoMin)
            {
                Min0 = _min = double.NaN;
            }
            else
            {
                Min0 = _min = _axis.Min;
            }
            if (_axis.AutoMax)
            {
                Max0 = _max = double.NaN;
            }
            else
            {
                Max0 = _max = _axis.Max;
            }
        }

        internal void RoundLimits()
        {
            if (!double.IsNaN(Max0) && !double.IsNaN(Min0))
            {
                if (Max0 < Min0)
                {
                    double num = Max0;
                    Max0 = _axis.Max = _max = Min0;
                    Min0 = _axis.Min = _min = num;
                }
                if (!double.IsNaN(_axis.LogBase) && (Min0 <= 0.0))
                {
                    Min0 = 0.01;
                }
                if (double.IsNegativeInfinity(Min0))
                {
                    Min0 = -3.4028234663852886E+38;
                }
                if (double.IsPositiveInfinity(Max0))
                {
                    Max0 = 3.4028234663852886E+38;
                }
                if (!Axis.IsDependent && !Axis.UseExactLimits)
                {
                    if (_axis.IsTimeInternal(_chart))
                    {
                        TimeSpan ts = TimeSpan.FromDays((Max0 - Min0) / ((double)((IAxis)this).GetAnnoNumber()));
                        TimeSpan span2 = double.IsNaN(((IAxis)this).MajorUnit) ? TimeAxis.NiceTimeSpan(ts, ((IAxis)this).AnnoFormat) : TimeSpan.FromDays(((IAxis)this).MajorUnit);
                        double totalDays = span2.TotalDays;
                        if (_axis.AutoMin)
                        {
                            double d = TimeAxis.RoundTime(Min0, totalDays, false);
                            if ((!double.IsInfinity(d) && !double.IsNaN(d)) && (d < Min0))
                            {
                                Min0 = d;
                            }
                        }
                        if (_axis.AutoMax)
                        {
                            double num4 = TimeAxis.RoundTime(Max0, totalDays, true);
                            if (num4 < Max0)
                            {
                                num4 += totalDays;
                            }
                            if ((!double.IsInfinity(num4) && !double.IsNaN(num4)) && (num4 > Max0))
                            {
                                Max0 = num4;
                            }
                        }
                    }
                    else
                    {
                        int num5 = Utils.NicePrecision(Max0 - Min0);
                        if (_axis.AutoMin && double.IsNaN(_axis.LogBase))
                        {
                            double num6 = Utils.PrecFloor(-num5, Min0);
                            if (!double.IsInfinity(num6) && !double.IsNaN(num6))
                            {
                                Min0 = num6;
                            }
                        }
                        if (_axis.AutoMax && double.IsNaN(_axis.LogBase))
                        {
                            double num7 = Utils.PrecCeil(-num5, Max0);
                            if (!double.IsInfinity(num7) && !double.IsNaN(num7))
                            {
                                Max0 = num7;
                            }
                        }
                    }
                }
                if (_axis.Scale < 1.0)
                {
                    double num8 = (Max0 - Min0) * _axis.Scale;
                    _min = Min0 + (_axis.Value * ((Max0 - Min0) - num8));
                    _max = _min + num8;
                }
                else
                {
                    _min = Min0;
                    _max = Max0;
                }
            }
        }

        internal void SetLimits(double dataMin, double dataMax)
        {
            if (double.IsNaN(_min))
            {
                Min0 = _min = dataMin;
            }
            if (double.IsNaN(_max))
            {
                double num = dataMax;
                if (num < _min)
                {
                    num = _min + (_min - num);
                }
                Max0 = _max = num;
            }
            else if ((_max < _min) && double.IsNaN(Axis.Min))
            {
                Min0 = _min = _max - (_min - _max);
            }
        }

        internal void SetLimitsAux(double dataMin, double dataMax)
        {
            if (double.IsNaN(Min0))
            {
                Min0 = _min = dataMin;
            }
            else
            {
                _min = Min0;
            }
            if (double.IsNaN(Max0))
            {
                double num = dataMax;
                if (num < _min)
                {
                    num = (_min + _min) - num;
                }
                Max0 = _max = num;
            }
            else
            {
                _max = Max0;
                if ((_max < _min) && double.IsNaN(Axis.Min))
                {
                    Min0 = _min = _max - (_min - _max);
                }
            }
        }

        static void SetProperty(DependencyObject obj, DependencyProperty prop, object value)
        {
            if (obj.ReadLocalValue(prop) == DependencyProperty.UnsetValue)
            {
                obj.SetValue(prop, value);
            }
        }

        void UpdateAnnosize(IEnumerable items)
        {
            if (_axis.ItemsSource != null)
            {
                items = _axis.ItemsSource;
            }
            if (items != null)
            {
                List<object> list = new List<object>();
                foreach (object obj2 in items)
                {
                    list.Add(obj2);
                }
                int num = list.Count;
                Size size = new Size(0.0, 0.0);
                for (int i = 0; i < num; i++)
                {
                    object lbl = list[i];
                    if (lbl is KeyValuePair<object, double>)
                    {
                        lbl = ((KeyValuePair<object, double>)lbl).Key;
                    }
                    else if (lbl is KeyValuePair<object, DateTime>)
                    {
                        lbl = ((KeyValuePair<object, DateTime>)lbl).Key;
                    }
                    Size size2 = Utils.GetSize(CreateAnno(0.0, lbl));
                    size.Width = Math.Max(size.Width, size2.Width);
                    size.Height = Math.Max(size.Height, size2.Height);
                }
                AnnoSize = size;
            }
            else if (!double.IsNaN(_min) && !double.IsNaN(_max))
            {
                UIElement element2;
                UIElement element3;
                double range = _max - _min;
                int digits = Utils.NicePrecision(range);
                if ((digits < 0) || (digits > 15))
                {
                    digits = 0;
                }
                double num5 = 0.1 * range;
                if (!double.IsNaN(_axis.LogBase))
                {
                    element2 = CreateAnno(Math.Pow(_axis.LogBase, Math.Ceiling(Math.Log(_min, _axis.LogBase))), null);
                    element3 = CreateAnno(Math.Pow(_axis.LogBase, Math.Floor(Math.Log(_max, _axis.LogBase))), null);
                }
                else
                {
                    num5 = Utils.GetMajorUnit(range, this, 0.0);
                    element2 = CreateAnno(Math.Round((double)(_min - num5), digits), null);
                    element3 = CreateAnno(Math.Round((double)(_max + num5), digits), null);
                }
                UIElement element4 = (_min != Min0) ? CreateAnno(Math.Round((double)(Min0 - num5), digits), null) : null;
                UIElement element5 = (_max != Max0) ? CreateAnno(Math.Round((double)(Max0 + num5), digits), null) : null;
                AnnoSize = GetMaxSize(new UIElement[] { element2, element3, element4, element5 });
            }
        }

        internal double AnnoAngleInternal
        {
            get
            {
                if (_axis.AxisType != AxisType.Y)
                {
                    return _axis.AnnoAngle;
                }
                if (IsNear)
                {
                    return (_axis.AnnoAngle - 90.0);
                }
                return (_axis.AnnoAngle - 90.0);
            }
        }

        internal virtual int AnnoRowNum
        {
            get { return 1; }
        }

        internal Size AnnoSize
        {
            get { return _annoSize; }
            set
            {
                _annoSize = value;
                double annoAngleInternal = AnnoAngleInternal;
                if (annoAngleInternal != 0.0)
                {
                    annoAngleInternal *= 0.017453292519943295;
                    double num2 = Math.Abs(Math.Sin(annoAngleInternal));
                    double num3 = Math.Abs(Math.Cos(annoAngleInternal));
                    double width = _annoSize.Width;
                    double height = _annoSize.Height;
                    _annoSize.Width = (width * num3) + (height * num2);
                    _annoSize.Height = (width * num2) + (height * num3);
                }
                if (ScrollBarInternal != null)
                {
                    Windows.UI.Xaml.Thickness scrollBarMargin = _axis.ScrollBar.ScrollBarMargin;
                    _annoSize.Width = Math.Max(_annoSize.Width, -scrollBarMargin.Left * 2.0);
                }
            }
        }

        internal object AnnoTemplateInternal
        {
            get { return _axis.AnnoTemplate; }
        }

        internal Dt.Charts.Axis Axis
        {
            get { return _axis; }
            set
            {
                _axis = value;
                if (_axis != null)
                {
                    _axis.iax = this;
                }
            }
        }

        internal string AxisName
        {
            get { return _axis.Name; }
        }

        internal Rect AxisRect
        {
            get
            {
                Rect rect = _r;
                if (!rect.IsEmptyRect() && (_axis.AxisType == AxisType.Y))
                {
                    return new Rect(rect.X, rect.Y, rect.Height, rect.Width);
                }
                return rect;
            }
        }

        string IAxis.AnnoFormat
        {
            get
            {
                string annoFormat = _axis.AnnoFormat;
                if (string.IsNullOrEmpty(annoFormat) && (_chart != null))
                {
                    if ((_axis.AxisType == AxisType.Y) && (((_chart.ChartType == ChartType.ColumnStacked100pc) || (_chart.ChartType == ChartType.AreaStacked100pc)) || ((_chart.ChartType == ChartType.LineStacked100pc) || (_chart.ChartType == ChartType.LineSymbolsStacked100pc))))
                    {
                        annoFormat = "0'%'";
                    }
                    else if ((_axis.AxisType == AxisType.X) && (_chart.ChartType == ChartType.BarStacked100pc))
                    {
                        annoFormat = "0'%'";
                    }
                }
                if (string.IsNullOrEmpty(annoFormat) && _axis.IsTimeInternal(_chart))
                {
                    annoFormat = TimeAxis.GetTimeDefaultFormat(_max, _min);
                }
                if (string.IsNullOrEmpty(annoFormat) && !double.IsNaN(_axis.LogBase))
                {
                    annoFormat = "G3";
                }
                _axis.AnnoFormatInternal = annoFormat;
                return annoFormat;
            }
        }

        AxisType IAxis.AxisType
        {
            get { return _axis.AxisType; }
        }

        bool IAxis.IsTime
        {
            get { return _axis.IsTimeInternal(_chart); }
        }

        double IAxis.LogBase
        {
            get { return _axis.LogBase; }
        }

        double IAxis.MajorUnit
        {
            get
            {
                double majorUnit = (double)_axis.MajorUnit;
                if (majorUnit <= 0.0)
                {
                    majorUnit = double.NaN;
                }
                return majorUnit;
            }
        }

        double IAxis.Max
        {
            get { return _max; }
            set { _max = value; }
        }

        double IAxis.Min
        {
            get { return _min; }
            set { _min = value; }
        }

        bool IAxis.Visible
        {
            get { return _axis.Visible; }
            set { _axis.Visible = value; }
        }

        internal Chart Chart
        {
            get { return _chart; }
            set { _chart = value; }
        }

        new internal Size DesiredSize
        {
            get { return _desiredSize; }
            set { _desiredSize = value; }
        }

        internal bool IsFar
        {
            get { return ((PositionInternal & AxisPosition.Far) != AxisPosition.Near); }
        }

        internal bool IsNear
        {
            get { return ((PositionInternal & AxisPosition.Far) == AxisPosition.Near); }
        }

        internal double MajorTickHeightInternal
        {
            get { return _axis.MajorTickHeight; }
        }

        internal double Max0
        {
            get
            {
                if (!_axis.AutoMax)
                {
                    return _axis.Max;
                }
                return _max0;
            }
            set
            {
                _max0 = value;
                _axis.ActualMax = value;
            }
        }

        internal double Min0
        {
            get
            {
                if (!_axis.AutoMin)
                {
                    return _axis.Min;
                }
                return _min0;
            }
            set
            {
                _min0 = value;
                _axis.ActualMin = value;
            }
        }

        internal double MinorTickHeightInternal
        {
            get { return _axis.MinorTickHeight; }
        }

        internal AxisPosition PositionInternal
        {
            get { return _axis.Position; }
        }

        internal bool ReversedInternal
        {
            get { return _axis.Reversed; }
        }

        internal UIElement ScrollBarInternal
        {
            get
            {
                IAxisScrollBar scrollBar = _axis.ScrollBar;
                if ((scrollBar != null) && (scrollBar.ScrollBarPosition != AxisScrollBarPosition.None))
                {
                    return (scrollBar as UIElement);
                }
                return null;
            }
        }

        internal UIElement TitleInternal
        {
            get
            {
                UIElement title = _axis.Title as UIElement;
                if (title != null)
                {
                    return title;
                }
                if (_axis.Title != null)
                {
                    string str = _axis.Title.ToString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (_tbtitle == null)
                        {
                            TextBlock block = new TextBlock();
                            block.HorizontalAlignment = HorizontalAlignment.Center;
                            _tbtitle = block;
                            InitElement(_tbtitle);
                        }
                        _tbtitle.Text = str;
                        return _tbtitle;
                    }
                }
                return null;
            }
        }
    }
}

