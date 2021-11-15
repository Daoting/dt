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
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    internal partial class ChartViewport2D : Canvas, ICoordConverter
    {
        readonly ChartView _view;
        AxisStyle _ast;
        internal Axis _ax;
        AxisCanvasCollection _axes = new AxisCanvasCollection();
        internal Axis _ay;
        Canvas _intCanvas = new Canvas();
        Rect _plot = new Rect();
        Rect _plot0 = new Rect();
        RadarView _radarView = new RadarView();
        int _startDataIdx = -1;

        internal ChartViewport2D(ChartView view)
        {
            _view = view;
            Children.Add(_intCanvas);

            _ax = view.AxisX;
            _ay = view.AxisY;
            _axes.Add(_ax);
            _axes.Add(_ay);
            InternalChildren.Add(_ax);
            InternalChildren.Add(_ay);
        }

        #region 测量布局
        /*********************************************************************************************************/
        // MeasureOverride中尽可能不增删Children元素，uno中每增删一个元素会重复一次MeasureOverride，严重时死循环！！！
        // UWP和uno的调用顺序不同！
        // UWP：MeasureOverride > _owner.SizeChanged > SizeChanged > Loaded
        // uno：Loaded > MeasureOverride > SizeChanged > _owner.SizeChanged
        /*********************************************************************************************************/

        /// <summary>
        /// 当前视口大小
        /// </summary>
        internal Size CurrentSize { get; set; }

        internal void Refresh()
        {
            if (CurrentSize.Width == 0 || CurrentSize.Height == 0)
                return;

            _ax.Axis = _view.AxisX;
            _ay.Axis = _view.AxisY;
            List<string> list = new List<string>();
            foreach (Axis axis in _axes)
            {
                if ((axis.PositionInternal & AxisPosition.OverData) > AxisPosition.Near)
                {
                    Canvas.SetZIndex(axis, 1);
                }
                else
                {
                    Canvas.SetZIndex(axis, 0);
                }
                ((IAxis)axis).ClearLabels();
                axis.ResetLimits();
                axis.Chart = _view.Chart;
                if (((axis != _ax) && (axis != _ay)) && !string.IsNullOrEmpty(axis.AxisName))
                {
                    list.Add(axis.AxisName);
                }
            }

            if (_view.Renderer != null)
            {
                IView2DRenderer renderer = _view.Renderer as IView2DRenderer;
                if (renderer == null)
                    return;

                _ast = renderer.Axis;
                Size sz = new Size(CurrentSize.Width, CurrentSize.Height);
                Renderer2D rendererd = renderer as Renderer2D;
                if (rendererd != null)
                {
                    rendererd.ValidAuxAxes = list.ToArray();
                    if (_plot.Width > 0.0)
                    {
                        sz.Width = _plot.Width;
                    }
                    if (_plot.Height > 0.0)
                    {
                        sz.Height = _plot.Height;
                    }
                }

                Rect rect = renderer.Measure(sz);
                if (rect.Width > 0.0)
                {
                    _ax.SetLimits(rect.X, rect.X + rect.Width);
                }
                if (rect.Height > 0.0)
                {
                    _ay.SetLimits(rect.Y, rect.Y + rect.Height);
                }
            }

            switch (_ast)
            {
                case AxisStyle.None:
                    if ((base.Width > 10.0) && (base.Height > 10.0))
                    {
                        _plot = new Rect(5.0, 5.0, base.Width - 10.0, base.Height - 10.0);
                    }
                    using (List<Axis>.Enumerator enumerator3 = _axes.GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            enumerator3.Current.Visibility = Utils.VisHidden;
                        }
                    }
                    break;

                case AxisStyle.Cartesian:
                    UpdateLayoutCartesian();
                    break;

                case AxisStyle.Radar:
                    using (List<Axis>.Enumerator enumerator2 = _axes.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            enumerator2.Current.Visibility = Visibility.Collapsed;
                        }
                    }
                    UpdateLayoutRadar();
                    break;
            }

            Shape plotShape = _view.PlotShape;
            if (plotShape != null)
            {
                Canvas.SetLeft(plotShape, _view.PlotRect.Left);
                Canvas.SetTop(plotShape, _view.PlotRect.Top);
                plotShape.Width = _view.PlotRect.Width;
                plotShape.Height = _view.PlotRect.Height;
                if (_ast == AxisStyle.None)
                {
                    plotShape.Visibility = Visibility.Collapsed;
                }
                else
                {
                    plotShape.Visibility = Visibility.Visible;
                }
            }

            if (_view.Chart.UpdateCount <= 0 && _view.Renderer != null)
            {
                CreateDataObjects();
                _view.Renderer.Dirty = false;
            }
        }

        void UpdateLayoutCartesian()
        {
            PrepareAxes();
            if (_view.HasPlotAreas)
            {
                PerformPlotAreaLayout();
                return;
            }

            int num = _axes.Count;
            PlotArea area = _view.UpdateMainPlotArea();
            Renderer2D renderer = _view.Renderer as Renderer2D;
            double xLeft = 0.0;
            double xTop = 0.0;
            double yLeft = 0.0;
            double yTop = 0.0;
            double width = CurrentSize.Width;
            double height = CurrentSize.Height;
            double right = CurrentSize.Width;
            double bottom = CurrentSize.Height;

            for (int i = 0; i < num; i++)
            {
                Axis ax = _axes[i];
                double origin = ax.Axis.Origin;
                if (ax.AxisType == AxisType.X)
                {
                    Size size = new Size(CurrentSize.Width, 0.3 * CurrentSize.Height);
                    Size size3 = ax.GetSize(GetItems(renderer, ax), false);
                    if (size3.Height > size.Height)
                    {
                        size3.Height = size.Height;
                    }
                    size3.Width = size.Width;
                    ax.DesiredSize = size3;
                    bool flag2 = (!double.IsNaN(origin) && (origin <= _ay.Max0)) && (origin >= _ay.Min0);
                    if (ax.IsNear)
                    {
                        xLeft = Math.Max(xLeft, ax.AnnoSize.Width * 0.5);
                        width = Math.Min(width, CurrentSize.Width - (ax.AnnoSize.Width * 0.5));
                        if (flag2)
                        {
                            double num12 = ConvertY(origin, xTop, bottom);
                            bottom -= Math.Max((double)0.0, (double)((num12 + size3.Height) - bottom));
                        }
                        else
                        {
                            bottom -= size3.Height;
                        }
                    }
                    else if (ax.IsFar)
                    {
                        xLeft = Math.Max(xLeft, ax.AnnoSize.Width * 0.5);
                        width = Math.Min(width, CurrentSize.Width - (ax.AnnoSize.Width * 0.5));
                        if (flag2)
                        {
                            double num13 = ConvertY(origin, xTop, bottom);
                            xTop += Math.Max((double)0.0, (double)(xTop - (num13 - size3.Height)));
                        }
                        else
                        {
                            xTop += size3.Height;
                        }
                    }
                }
                else if (ax.AxisType == AxisType.Y)
                {
                    Size size2 = new Size(CurrentSize.Height, 0.3 * CurrentSize.Width);
                    Size size4 = ax.GetSize(GetItems(renderer, ax), false);
                    if (size4.Height > size2.Height)
                    {
                        size4.Height = size2.Height;
                    }
                    size4.Width = size2.Width;
                    ax.DesiredSize = size4;
                    bool flag3 = (!double.IsNaN(origin) && (origin <= _ax.Max0)) && (origin >= _ax.Min0);
                    if (ax.IsNear)
                    {
                        yTop = Math.Max(yTop, ax.AnnoSize.Width * 0.5);
                        height = Math.Min(height, CurrentSize.Height - (ax.AnnoSize.Width * 0.5));
                        if (flag3)
                        {
                            double num14 = ConvertX(origin, yLeft, right);
                            yLeft += Math.Max((double)0.0, (double)(yLeft - (num14 - size4.Height)));
                        }
                        else
                        {
                            yLeft += size4.Height;
                        }
                    }
                    else if (ax.IsFar)
                    {
                        yTop = Math.Max(yTop, ax.AnnoSize.Width * 0.5);
                        height = Math.Min(height, CurrentSize.Height - (ax.AnnoSize.Width * 0.5));
                        if (flag3)
                        {
                            double num15 = ConvertX(origin, yLeft, right);
                            right -= Math.Max((double)0.0, (double)((num15 + size4.Height) - right));
                        }
                        else
                        {
                            right -= size4.Height;
                        }
                    }
                }
            }

            double w = 0.0;
            double h = 0.0;
            AdjustMargins(ref xLeft, ref width, ref yTop, ref height, ref w, ref h, ref yLeft, ref right, ref xTop, ref bottom);
            _plot = _plot0 = new Rect(xLeft, yTop, w, h);
            if (area != null)
            {
                area.SetPlotX(_plot.X, _plot.Width);
                area.SetPlotY(_plot.Y, _plot.Height);
                UIElement uIElement = area.UIElement;
                if ((uIElement != null) && !InternalChildren.Contains(uIElement))
                {
                    InternalChildren.Insert(0, uIElement);
                }
            }

            for (int j = 0; j < num; j++)
            {
                Axis axis2 = _axes[j];
                axis2._plot = _plot0;
                double d = axis2.Axis.Origin;
                if (axis2.AxisType == AxisType.X)
                {
                    Rect r = new Rect();
                    if ((double.IsNaN(d) || (d > _ay.Max0)) || (d < _ay.Min0))
                    {
                        if (axis2.IsNear)
                        {
                            r = new Rect(xLeft, bottom, w, axis2.DesiredSize.Height);
                            bottom += axis2.DesiredSize.Height;
                        }
                        else if (axis2.IsFar)
                        {
                            r = new Rect(xLeft, xTop - axis2.DesiredSize.Height, w, axis2.DesiredSize.Height);
                            xTop -= axis2.DesiredSize.Height;
                        }
                    }
                    else
                    {
                        double y = ConvertY(d, _plot.Top, _plot.Bottom);
                        if (axis2.IsNear)
                        {
                            r = new Rect(xLeft, y, w, axis2.DesiredSize.Height);
                            bottom += Math.Max((double)0.0, (double)(r.Bottom - _plot.Bottom));
                        }
                        else if (axis2.IsFar)
                        {
                            r = new Rect(xLeft, y - axis2.DesiredSize.Height, w, axis2.DesiredSize.Height);
                            xTop -= Math.Max((double)0.0, (double)(_plot.Top - r.Top));
                        }
                    }
                    axis2.Layout(r);
                }
                else if (axis2.AxisType == AxisType.Y)
                {
                    Rect rect2 = new Rect();
                    if ((double.IsNaN(d) || (d > _ax.Max0)) || (d < _ax.Min0))
                    {
                        if (axis2.IsNear)
                        {
                            rect2 = new Rect(yLeft - axis2.DesiredSize.Height, yTop, h, axis2.DesiredSize.Height);
                            yLeft -= axis2.DesiredSize.Height;
                        }
                        else if (axis2.IsFar)
                        {
                            rect2 = new Rect(right, yTop, h, axis2.DesiredSize.Height);
                            right += axis2.DesiredSize.Height;
                        }
                    }
                    else
                    {
                        double x = ConvertX(d, _plot.Left, _plot.Right);
                        if (axis2.IsNear)
                        {
                            rect2 = new Rect(x - axis2.DesiredSize.Height, yTop, h, axis2.DesiredSize.Height);
                            yLeft -= axis2.DesiredSize.Height;
                        }
                        else if (axis2.IsFar)
                        {
                            rect2 = new Rect(x, yTop, h, axis2.DesiredSize.Height);
                            right += axis2.DesiredSize.Height;
                        }
                    }
                    axis2.Layout(rect2);
                }
                if (((axis2 == _ax) && (renderer != null)) && !renderer.Inverted)
                {
                    axis2.CreateLabels(renderer.ItemNames);
                }
                else if (((axis2 == _ay) && (renderer != null)) && renderer.Inverted)
                {
                    axis2.CreateLabels(renderer.ItemNames);
                }
                else
                {
                    axis2.CreateLabels(null);
                }
                axis2.CreateAnnosAndTicks(false);
            }
        }

        void UpdateLayoutRadar()
        {
            RadarRenderer renderer = _view.Renderer as RadarRenderer;
            List<int> list = new List<int>();
            foreach (Axis axis in _axes)
            {
                if (((axis != _ax) && (axis != _ay)) && ((axis.AxisType == AxisType.Y) && (axis.Axis.RadarPointIndices != null)))
                {
                    double naN = double.NaN;
                    double ymax = double.NaN;
                    foreach (int num3 in axis.Axis.RadarPointIndices)
                    {
                        if (!list.Contains(num3))
                        {
                            list.Add(num3);
                        }
                        renderer.GetMinMaxY(num3, ref naN, ref ymax);
                    }
                    if (double.IsNaN(axis._min))
                    {
                        axis.Min0 = axis._min = naN;
                    }
                    if (double.IsNaN(axis._max))
                    {
                        axis.Max0 = axis._max = ymax;
                    }
                    axis.RoundLimits();
                }
            }
            if (list.Count > 0)
            {
                double ymin = double.NaN;
                double num5 = double.NaN;
                int npts = ((BaseRenderer)_view.Renderer)._dataInfo.npts;
                for (int i = 0; i < npts; i++)
                {
                    if (!list.Contains(i))
                    {
                        renderer.GetMinMaxY(i, ref ymin, ref num5);
                    }
                }
                if (double.IsNaN(_ay.Axis.Min))
                {
                    _ay.Min0 = _ay._min = ymin;
                }
                if (double.IsNaN(_ay.Axis.Max))
                {
                    _ay.Max0 = _ay._max = num5;
                }
                _ay.RoundLimits();
            }
            Canvas.SetLeft(_ay, 0.0);
            Canvas.SetTop(_ay, 0.0);
            Canvas.SetLeft(_ax, 0.0);
            Canvas.SetTop(_ax, 0.0);
            using (List<Axis>.Enumerator enumerator3 = _axes.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.Children.Clear();
                }
            }
            Size size = _ay.GetSize(null, false);
            _ax.GetSize(renderer.ItemNames, true);
            double width = base.Width;
            double height = base.Height;
            if (double.IsNaN(width))
            {
                width = CurrentSize.Width;
            }
            if (double.IsNaN(height))
            {
                height = CurrentSize.Height;
            }
            Size annoSize = _ax.AnnoSize;
            _plot = new Rect(1.1 * annoSize.Width, 1.1 * annoSize.Height, Math.Max((double)8.0, (double)(width - (2.2 * annoSize.Width))), Math.Max((double)8.0, (double)(height - (2.2 * annoSize.Height))));
            Chart chart = _view.Chart;
            if (chart != null)
            {
                _radarView.Angle = PolarRadarOptions.GetStartingAngle(chart);
                _radarView.Direction = PolarRadarOptions.GetDirection(chart);
            }
            _radarView.IsPolar = renderer.IsPolar;
            _radarView.Init(_plot);
            _ay.Layout(_radarView.GetAxisYRect(size.Height));
            _ay.CreateLabels(null);
            _ay.CreateAnnosAndTicks(true);
            _ay.Visibility = Visibility.Collapsed;
            foreach (Axis axis3 in _axes)
            {
                if ((axis3 != _ay) && (axis3.AxisType == AxisType.Y))
                {
                    axis3.Layout(_radarView.GetAxisYRect(size.Height));
                    axis3.CreateLabels(null);
                    axis3.CreateAnnosAndTicks(true);
                    axis3.Visibility = Visibility.Collapsed;
                }
            }
            _ax.Visibility = Visibility.Visible;
            _ax.CreateLabels(renderer.ItemNames, renderer.IsPolar ? ((double)0x2d) : ((double)0));
            _ax.CreateAnnosAndTicksRadar(_radarView, _ay);
            _plot0 = _plot = new Rect(0.1 * annoSize.Width, 0.1 * annoSize.Height, Math.Max((double)8.0, (double)(width - (0.2 * annoSize.Width))), Math.Max((double)8.0, (double)(height - (0.2 * annoSize.Height))));
        }

        void CreateDataObjects()
        {
            if (_startDataIdx >= 0)
            {
                int num = InternalChildren.Count - _startDataIdx;
                for (int j = 0; j < num; j++)
                {
                    PlotElement sender = InternalChildren[_startDataIdx] as PlotElement;
                    InternalChildren.RemoveAt(_startDataIdx);
                    if (((sender != null) && (sender.DataPoint != null)) && (sender.DataPoint.Series != null))
                    {
                        sender.DataPoint.Series.FirePlotElementUnloaded(sender, EventArgs.Empty);
                    }
                }
            }

            Shape plotShape = _view.PlotShape;
            if ((plotShape != null) && !InternalChildren.Contains(plotShape))
            {
                InternalChildren.Insert(0, plotShape);
            }
            _startDataIdx = InternalChildren.Count;

            IView2DRenderer renderer = _view.Renderer as IView2DRenderer;
            if (renderer != null)
            {
                renderer.CoordConverter = this;
                UIElement[] elementArray = renderer.Generate();

                if (_view.Renderer is BaseRenderer br)
                    br.FireRendered(this, EventArgs.Empty);

                if (elementArray == null)
                {
                    while (Children.Count > 1)
                    {
                        Children.RemoveAt(Children.Count - 1);
                    }
                }
                else
                {
                    int length = elementArray.Length;
                    for (int k = 0; k < length; k++)
                    {
                        var elem = elementArray[k];
                        if (elem == null || InternalChildren.Contains(elem))
                            continue;

                        if (elem is FrameworkElement fe && fe.Parent is Panel pnl)
                            pnl.Children.Remove(elem);
                        InternalChildren.Add(elem);
                    }

                    while (Children.Count > 1)
                    {
                        Children.RemoveAt(Children.Count - 1);
                    }
                }
            }

            int num5 = _view.Layers.Count;
            for (int i = 0; i < num5; i++)
            {
                FrameworkElement element = _view.Layers[i] as FrameworkElement;
                if (element != null)
                {
                    Canvas.SetLeft(element, PlotRect.X);
                    Canvas.SetTop(element, PlotRect.Y);
                    element.Width = PlotRect.Width;
                    element.Height = PlotRect.Height;
                    Children.Add(element);
                }
            }

            foreach (UIElement element5 in _view.Children)
            {
                if (!Children.Contains(element5))
                {
                    Children.Add(element5);
                }
            }
        }
        #endregion

        internal void AddAxis(Axis ax)
        {
            Axis axis = ax;
            _axes.Add(axis);
            if (_startDataIdx >= 0)
            {
                InternalChildren.Insert(_startDataIdx++, axis);
            }
            else
            {
                InternalChildren.Add(axis);
            }
        }

        void AdjustMargins(
            ref double left,
            ref double right,
            ref double top,
            ref double bottom,
            ref double w,
            ref double h,
            ref double l0,
            ref double r0,
            ref double t0,
            ref double b0)
        {
            Windows.UI.Xaml.Thickness thickness = _view.Margin;
            l0 = left = (thickness.Left != 0.0) ? thickness.Left : Math.Max(left, l0);
            r0 = right = (thickness.Right != 0.0) ? (CurrentSize.Width - thickness.Right) : Math.Min(right, r0);
            t0 = top = (thickness.Top != 0.0) ? thickness.Top : Math.Max(top, t0);
            b0 = bottom = (thickness.Bottom != 0.0) ? (CurrentSize.Height - thickness.Bottom) : Math.Min(bottom, b0);
            w = right - left;
            h = bottom - top;
            if (w < 1.0)
            {
                w = 1.0;
            }
            if (h < 1.0)
            {
                h = 1.0;
            }
        }

        internal void ClearAxes()
        {
            foreach (AxisCanvas canvas in _axes)
            {
                if ((canvas.Axis != _ax.Axis) && (canvas.Axis != _ay.Axis))
                {
                    _view.Axes.Remove(canvas.Axis);
                }
            }
            _axes.Clear();
            _axes.Add(_ax);
            _axes.Add(_ay);
        }

        public double ConvertBackX(double x)
        {
            return ((IAxis)_ax).ToData(x);
        }

        public double ConvertBackY(double y)
        {
            return ((IAxis)_ay).ToData(y);
        }

        public Point ConvertPoint(Point pt)
        {
            if (_ast == AxisStyle.Radar)
            {
                double radius = _radarView.Radius;
                Point center = _radarView.Center;
                double angle = (6.2831853071795862 * (pt.X - _ax.Axis.ActualMin)) / (_ax.Axis.ActualMax - _ax.Axis.ActualMin);
                angle = _radarView.GetAngle(angle);
                double num3 = (radius * (pt.Y - _ay.Axis.ActualMin)) / (_ay.Axis.ActualMax - _ay.Axis.ActualMin);
                return new Point(center.X + (num3 * Math.Cos(angle)), center.Y + (num3 * Math.Sin(angle)));
            }
            return new Point(_ax.ConvertEx(pt.X), _ay.ConvertEx(pt.Y));
        }

        public Point ConvertPoint(Point pt, Axis ax, Axis ay)
        {
            double radius = _radarView.Radius;
            Point center = _radarView.Center;
            double angle = (6.2831853071795862 * (pt.X - ax.ActualMin)) / (ax.ActualMax - ax.ActualMin);
            angle = _radarView.GetAngle(angle);
            if ((pt.Y > ay.ActualMax) || (pt.Y < ay.ActualMin))
            {
                return new Point(double.NaN, double.NaN);
            }
            double num3 = (radius * (pt.Y - ay.ActualMin)) / (ay.ActualMax - ay.ActualMin);
            return new Point(center.X + (num3 * Math.Cos(angle)), center.Y + (num3 * Math.Sin(angle)));
        }

        public double ConvertX(double x)
        {
            return _ax.ConvertEx(x);
        }

        double ConvertX(double x, double left, double right)
        {
            if (_ax.ReversedInternal)
            {
                return (right - (((right - left) * (x - _ax.Min0)) / (_ax.Max0 - _ax.Min0)));
            }
            return left + ((right - left) * (x - _ax.Min0) / (_ax.Max0 - _ax.Min0));
        }

        public double ConvertY(double y)
        {
            return _ay.ConvertEx(y);
        }

        double ConvertY(double y, double top, double bottom)
        {
            if (_ay.ReversedInternal)
            {
                return (top + (((bottom - top) * (y - _ay.Min0)) / (_ay.Max0 - _ay.Min0)));
            }
            return (bottom - (((bottom - top) * (y - _ay.Min0)) / (_ay.Max0 - _ay.Min0)));
        }

        public double ConvertZ(double z)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        object[] GetItems(Renderer2D rend, AxisCanvas ax)
        {
            if (rend != null)
            {
                if (rend.Inverted)
                {
                    if (ax == _ay)
                    {
                        return rend.ItemNames;
                    }
                }
                else if (ax == _ax)
                {
                    return rend.ItemNames;
                }
            }
            return null;
        }

        void PerformPlotAreaLayout()
        {
            List<AreaDef> list = new List<AreaDef>();
            List<AreaDef> list2 = new List<AreaDef>();
            int num = _view.Axes.Count;
            for (int i = 0; i < num; i++)
            {
                Axis axis = _view.Axes[i];
                if (axis.AxisType == AxisType.X)
                {
                    while (list.Count <= axis.PlotAreaIndex)
                    {
                        list.Add(new AreaDef());
                    }
                    list[axis.PlotAreaIndex].Axes.Add(axis);
                }
                else if (axis.AxisType == AxisType.Y)
                {
                    while (list2.Count <= axis.PlotAreaIndex)
                    {
                        list2.Add(new AreaDef());
                    }
                    list2[axis.PlotAreaIndex].Axes.Add(axis);
                }
            }
            int ncols = list.Count;
            int nrows = list2.Count;
            Renderer2D renderer = _view.Renderer as Renderer2D;
            Size size = new Size(CurrentSize.Width, 0.3 * CurrentSize.Height);
            Size size2 = new Size(CurrentSize.Height, 0.3 * CurrentSize.Width);
            double left = 0.0;
            double top = 0.0;
            double width = CurrentSize.Width;
            double height = CurrentSize.Height;
            for (int j = 0; j < ncols; j++)
            {
                AreaDef def = list[j];
                def.Right = CurrentSize.Width;
                def.Bottom = CurrentSize.Height;
                for (int num10 = 0; num10 < def.Axes.Count; num10++)
                {
                    AxisCanvas iax = def.Axes[num10].iax as AxisCanvas;
                    Size size3 = iax.GetSize(GetItems(renderer, iax), false);
                    if (size3.Height > size.Height)
                    {
                        size3.Height = size.Height;
                    }
                    size3.Width = size.Width;
                    iax.DesiredSize = size3;
                    if (j == 0)
                    {
                        def.Left = Math.Max(def.Left, iax.AnnoSize.Width * 0.5);
                    }
                    if (j == (ncols - 1))
                    {
                        def.Right = Math.Min(def.Right, CurrentSize.Width - (iax.AnnoSize.Width * 0.5));
                    }
                    if (iax.IsNear)
                    {
                        def.Bottom -= size3.Height;
                    }
                    else if (iax.IsFar)
                    {
                        def.Top += size3.Height;
                    }
                }
            }
            for (int k = 0; k < nrows; k++)
            {
                AreaDef def2 = list2[k];
                def2.Right = CurrentSize.Width;
                def2.Bottom = CurrentSize.Height;
                for (int num12 = 0; num12 < def2.Axes.Count; num12++)
                {
                    AxisCanvas ax = def2.Axes[num12].iax as AxisCanvas;
                    Size size4 = ax.GetSize(GetItems(renderer, ax), false);
                    if (size4.Height > size2.Height)
                    {
                        size4.Height = size2.Height;
                    }
                    size4.Width = size2.Width;
                    ax.DesiredSize = size4;
                    if (k == 0)
                    {
                        def2.Top = Math.Max(def2.Top, ax.AnnoSize.Width * 0.5);
                    }
                    if (k == (nrows - 1))
                    {
                        def2.Bottom = Math.Min(def2.Bottom, CurrentSize.Height - (ax.AnnoSize.Width * 0.5));
                    }
                    if (ax.IsNear)
                    {
                        def2.Left += size4.Height;
                    }
                    else if (ax.IsFar)
                    {
                        def2.Right -= size4.Height;
                    }
                }
            }
            double num13 = 0.0;
            double num14 = 0.0;
            double num15 = CurrentSize.Width;
            double num16 = CurrentSize.Height;
            for (int m = 0; m < ncols; m++)
            {
                AreaDef def3 = list[m];
                num13 = Math.Max(num13, def3.Left);
                num14 = Math.Max(num14, def3.Top);
                num15 = Math.Min(num15, def3.Right);
                num16 = Math.Min(num16, def3.Bottom);
            }
            for (int n = 0; n < nrows; n++)
            {
                AreaDef def4 = list2[n];
                num13 = Math.Max(num13, def4.Left);
                num14 = Math.Max(num14, def4.Top);
                num15 = Math.Min(num15, def4.Right);
                num16 = Math.Min(num16, def4.Bottom);
            }
            double w = 0.0;
            double h = 0.0;
            AdjustMargins(ref left, ref width, ref top, ref height, ref w, ref h, ref num13, ref num15, ref num14, ref num16);
            _plot = _plot0 = new Rect(left, top, w, h);
            double x = left;
            double[] numArray = _view.PlotAreas.CalculateWidths(w, ncols);
            int num22 = InternalChildren.Count - 1;
            for (int num23 = 0; num23 < ncols; num23++)
            {
                num16 = height;
                num14 = top;
                AreaDef def5 = list[num23];
                double num24 = numArray[num23];
                for (int num25 = 0; num25 < def5.Axes.Count; num25++)
                {
                    AxisCanvas canvas3 = def5.Axes[num25].iax as AxisCanvas;
                    canvas3._plot = new Rect(x, _plot0.Top, num24, _plot0.Height);
                    Rect r = new Rect();
                    if (canvas3.IsNear)
                    {
                        r = new Rect(x, num16, num24, canvas3.DesiredSize.Height);
                        num16 += canvas3.DesiredSize.Height;
                    }
                    else if (canvas3.IsFar)
                    {
                        r = new Rect(x, num14 - canvas3.DesiredSize.Height, num24, canvas3.DesiredSize.Height);
                        num14 -= canvas3.DesiredSize.Height;
                    }
                    canvas3.Layout(r);
                    if (((canvas3 == _ax) && (renderer != null)) && !renderer.Inverted)
                    {
                        canvas3.CreateLabels(renderer.ItemNames);
                    }
                    else
                    {
                        canvas3.CreateLabels(null);
                    }
                    canvas3.CreateAnnosAndTicks(false);
                }
                foreach (PlotArea area in _view.PlotAreas)
                {
                    if (area.Column == num23)
                    {
                        area.SetPlotX(x, num24);
                    }
                }
                x += num24;
            }
            double y = height;
            double[] numArray2 = _view.PlotAreas.CalculateHeights(h, nrows);
            for (int num27 = 0; num27 < nrows; num27++)
            {
                num13 = left;
                num15 = width;
                AreaDef def6 = list2[num27];
                double num28 = numArray2[num27];
                y -= num28;
                for (int num29 = 0; num29 < def6.Axes.Count; num29++)
                {
                    AxisCanvas canvas4 = def6.Axes[num29].iax as AxisCanvas;
                    canvas4._plot = new Rect(_plot0.Left, y, _plot0.Width, num28);
                    Rect rect2 = new Rect();
                    if (canvas4.IsNear)
                    {
                        rect2 = new Rect(num13 - canvas4.DesiredSize.Height, y, num28, canvas4.DesiredSize.Height);
                        num13 -= canvas4.DesiredSize.Height;
                    }
                    else if (canvas4.IsFar)
                    {
                        rect2 = new Rect(num15, y, num28, canvas4.DesiredSize.Height);
                        num15 += canvas4.DesiredSize.Height;
                    }
                    canvas4.Layout(rect2);
                    if (((canvas4 == _ay) && (renderer != null)) && renderer.Inverted)
                    {
                        canvas4.CreateLabels(renderer.ItemNames);
                    }
                    else
                    {
                        canvas4.CreateLabels(null);
                    }
                    canvas4.CreateAnnosAndTicks(false);
                }
                foreach (PlotArea area2 in _view.PlotAreas)
                {
                    if (area2.Row == num27)
                    {
                        area2.SetPlotY(y, num28);
                    }
                }
            }
            using (IEnumerator<PlotArea> enumerator = _view.PlotAreas.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    UIElement uIElement = enumerator.Current.UIElement;
                    if ((uIElement != null) && !InternalChildren.Contains(uIElement))
                    {
                        InternalChildren.Insert(num22++, uIElement);
                    }
                }
            }
        }

        void PrepareAxes()
        {
            Renderer2D renderer = _view.Renderer as Renderer2D;
            int num = _axes.Count;
            for (int i = 0; i < num; i++)
            {
                Axis axis = _axes[i];
                if ((axis != _ax) && (axis != _ay))
                {
                    Point min = new Point(double.NaN, double.NaN);
                    Point max = new Point(double.NaN, double.NaN);
                    if (renderer != null)
                    {
                        foreach (DataSeries series in renderer.Series)
                        {
                            bool flag = series.AxisX == axis.AxisName;
                            bool flag2 = series.AxisY == axis.AxisName;
                            if (flag || flag2)
                            {
                                series.GetMinMax(ref min, ref max);
                            }
                        }
                        if (renderer.Inverted)
                        {
                            double x = min.X;
                            min.X = min.Y;
                            min.Y = x;
                            x = max.X;
                            max.X = max.Y;
                            max.Y = x;
                        }
                    }
                    switch (axis.AxisType)
                    {
                        case AxisType.X:
                            axis.SetLimitsAux(min.X, max.X);
                            break;

                        case AxisType.Y:
                            axis.SetLimitsAux(min.Y, max.Y);
                            break;
                    }
                }
                axis.RoundLimits();
                if (axis.Axis.IsDependent)
                {
                    double scale = 1.0;
                    double num5 = 0.0;
                    Func<double, double> dependentAxisConverter = axis.Axis.DependentAxisConverter;
                    if (axis.AxisType == AxisType.X)
                    {
                        if (dependentAxisConverter != null)
                        {
                            double num6 = dependentAxisConverter(_ax.Min0);
                            double num7 = dependentAxisConverter(_ax.Max0);
                            if (num6 <= num7)
                            {
                                axis.Min0 = axis._min = num6;
                                axis.Max0 = axis._max = num7;
                            }
                            else
                            {
                                axis.Min0 = axis._min = num7;
                                axis.Max0 = axis._max = num6;
                            }
                        }
                        else
                        {
                            axis.Min0 = axis._min = _ax.Min0;
                            axis.Max0 = axis._max = _ax.Max0;
                        }
                        num5 = _ax.Axis.Value;
                        scale = _ax.Axis.Scale;
                    }
                    else if (axis.AxisType == AxisType.Y)
                    {
                        if (dependentAxisConverter != null)
                        {
                            double num8 = dependentAxisConverter(_ay.Min0);
                            double num9 = dependentAxisConverter(_ay.Max0);
                            if (num8 <= num9)
                            {
                                axis.Min0 = axis._min = num8;
                                axis.Max0 = axis._max = num9;
                            }
                            else
                            {
                                axis.Min0 = axis._min = num9;
                                axis.Max0 = axis._max = num8;
                            }
                        }
                        else
                        {
                            axis.Min0 = axis._min = _ay.Min0;
                            axis.Max0 = axis._max = _ay.Max0;
                        }
                        num5 = _ay.Axis.Value;
                        scale = _ay.Axis.Scale;
                    }
                    if (scale < 1.0)
                    {
                        double num10 = (axis.Max0 - axis.Min0) * scale;
                        axis._min = axis.Min0 + (num5 * ((axis.Max0 - axis.Min0) - num10));
                        axis._max = axis._min + num10;
                    }
                }
                axis.ClearLabelsAndTicks();
            }
        }

        internal void RemoveAxis(Axis ax)
        {
            foreach (Axis axis in _axes)
            {
                if (axis.Axis == ax)
                {
                    _axes.Remove(axis);
                    InternalChildren.Remove(axis);
                    _startDataIdx--;
                    break;
                }
            }
        }

        internal void Reset()
        {
        }

        internal void SetAxisX(Axis ax)
        {
            int index = InternalChildren.IndexOf(_ax);
            int num2 = _axes.IndexOf(_ax);
            InternalChildren.Remove(_ax);
            _axes.Remove(_ax);
            _ax = ax;
            InternalChildren.Insert(index, _ax);
            _axes.Insert(num2, _ax);
        }

        internal void SetAxisY(Axis ay)
        {
            int index = InternalChildren.IndexOf(_ay);
            int num2 = _axes.IndexOf(_ay);
            InternalChildren.Remove(_ay);
            _axes.Remove(_ay);
            _ay = ay;
            InternalChildren.Insert(index, _ay);
            _axes.Insert(num2, _ay);
        }

        internal AxisCanvasCollection Axes
        {
            get { return _axes; }
        }

        public Rect DataBounds
        {
            get { return new Rect(_ax._min, _ay._min, _ax._max - _ax._min, _ay._max - _ay._min); }
        }

        public Rect DataBounds2D
        {
            get { return new Rect(_ax._min, _ay._min, _ax._max - _ax._min, _ay._max - _ay._min); }
        }

        internal UIElementCollection InternalChildren
        {
            get { return _intCanvas.Children; }
        }

        internal Rect PlotRect
        {
            get { return _plot; }
        }

        public Rect ViewBounds
        {
            get { return new Rect(_plot.X, _plot.Y, _plot.Width, _plot.Height); }
        }

        class AreaDef
        {
            List<Axis> _axes = new List<Axis>();
            public double Bottom;
            public double Left;
            public double Right;
            public double Top;

            public List<Axis> Axes
            {
                get { return _axes; }
            }
        }
    }
}

