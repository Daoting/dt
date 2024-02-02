#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
#endregion

namespace Dt.Charts
{
    [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never)]
    public class RenderContext
    {
        Axis _ax;
        Axis _ay;
        Rect _bounds;
        Rect _clipBounds;
        Point _cur;
        Point _curView;
        Axis _defax;
        Axis _defay;
        IDataSeriesInfo _ds;
        double[] _dvals;
        bool _excludeHoles;
        bool _isStacked;
        string[] _names;
        int _pi;
        Point _prev;
        List<Point> _pts;
        Rect _rect;
        IRenderer _rendrerer;
        List<double> _stacked;
        internal Chart Chart;
        internal int ClusterPlotAreaIndex;
        internal bool hasNan;
        internal bool XReversed;
        internal bool YReversed;

        internal RenderContext()
        {
            _cur = new Point();
            _prev = new Point();
            _curView = new Point();
            _bounds = new Rect();
            _stacked = new List<double>();
            _excludeHoles = true;
            _pts = new List<Point>();
            _rect = Extensions.EmptyRect;
            _clipBounds = Extensions.EmptyRect;
        }

        internal RenderContext(IRenderer renderer)
        {
            _cur = new Point();
            _prev = new Point();
            _curView = new Point();
            _bounds = new Rect();
            _stacked = new List<double>();
            _excludeHoles = true;
            _pts = new List<Point>();
            _rect = Extensions.EmptyRect;
            _clipBounds = Extensions.EmptyRect;
            _rendrerer = renderer;
        }

        internal RenderContext(IRenderer renderer, IDataSeriesInfo ds, int npts)
        {
            _cur = new Point();
            _prev = new Point();
            _curView = new Point();
            _bounds = new Rect();
            _stacked = new List<double>();
            _excludeHoles = true;
            _pts = new List<Point>();
            _rect = Extensions.EmptyRect;
            _clipBounds = Extensions.EmptyRect;
            _rendrerer = renderer;
            _ds = ds;
            _pts.Capacity = npts;
            if (_ds != null)
            {
                _names = _ds.GetDataNames();
            }
            Chart visual = renderer.Visual as Chart;
            Chart = visual;
            BaseRenderer renderer2 = renderer as BaseRenderer;
            _isStacked = (renderer2 != null) && renderer2.IsStacked;
            hasNan = renderer2._dataInfo.hasNaN;
            DataSeries series = ds as DataSeries;
            if (series != null)
            {
                Renderer2D rendererd = renderer as Renderer2D;
                if ((((byte)(series.Display & SeriesDisplay.ShowNaNGap)) != 0) && (rendererd != null))
                {
                    _excludeHoles = false;
                }
                if (rendererd != null)
                {
                    string axisX = series.AxisX;
                    string axisY = series.AxisY;
                    bool flag = !string.IsNullOrEmpty(axisX);
                    bool flag2 = !string.IsNullOrEmpty(axisY);
                    if (visual != null)
                    {
                        ChartViewport2D viewElement = visual.View.Viewport;
                        if (viewElement != null)
                        {
                            if (flag || flag2)
                            {
                                foreach (Axis axis in viewElement.Axes)
                                {
                                    if (flag && (axis.AxisName == axisX))
                                    {
                                        _ax = axis;
                                        hasNan = true;
                                    }
                                    if (flag2 && (axis.AxisName == axisY))
                                    {
                                        _ay = axis;
                                        hasNan = true;
                                    }
                                }
                            }
                            _defax = viewElement._ax;
                            _defay = viewElement._ay;
                        }
                    }
                }
            }
            if (visual.View != null)
            {
                ChartView view = visual.View;
                if (_ax != null)
                {
                    XReversed = _ax.ReversedInternal;
                }
                else if (visual.View.AxisX != null)
                {
                    XReversed = visual.View.AxisX.Reversed;
                }
                if (_ay != null)
                {
                    YReversed = _ay.ReversedInternal;
                }
                else if (visual.View.AxisY != null)
                {
                    YReversed = visual.View.AxisY.Reversed;
                }
                Axis axis2 = visual.View.GetAxisX(series);
                if ((axis2 != null) && !double.IsNaN(axis2.LogBase))
                {
                    hasNan = true;
                }
                Axis axis3 = visual.View.GetAxisY(series);
                if ((axis3 != null) && !double.IsNaN(axis3.LogBase))
                {
                    hasNan = true;
                }
                if ((renderer2 != null) && renderer2.Inverted)
                {
                    if ((axis2 != null) && (axis2.PlotAreaIndex != 0))
                    {
                        ClusterPlotAreaIndex = axis2.PlotAreaIndex;
                    }
                }
                else if ((axis3 != null) && (axis3.PlotAreaIndex != 0))
                {
                    ClusterPlotAreaIndex = axis3.PlotAreaIndex;
                }
            }
        }

        void CalcClipBounds()
        {
            _clipBounds = new Rect(Bounds.X, Bounds.Y, Bounds2D.Width, Bounds2D.Height);
            Axis axis = _ax;
            if (axis == null)
            {
                axis = _defax;
            }
            if (axis != null)
            {
                Rect axisRect = axis.AxisRect;
                if (!axisRect.IsEmptyRect())
                {
                    _clipBounds.X = axisRect.X;
                    _clipBounds.Width = axisRect.Width;
                }
            }
            Axis axis2 = _ay;
            if (axis2 == null)
            {
                axis2 = _defay;
            }
            if (axis2 != null)
            {
                Rect rect = axis2.AxisRect;
                if (!rect.IsEmptyRect())
                {
                    _clipBounds.Y = rect.Y;
                    _clipBounds.Height = rect.Height;
                }
            }
        }

        public double ConvertX(double x)
        {
            if (_ax != null)
            {
                return _ax.ConvertEx(x);
            }
            if ((_rendrerer != null) && (_rendrerer.CoordConverter != null))
            {
                return _rendrerer.CoordConverter.ConvertX(x);
            }
            return double.NaN;
        }

        public double ConvertY(double y)
        {
            if (_ay != null)
            {
                return _ay.ConvertEx(y);
            }
            if ((_rendrerer != null) && (_rendrerer.CoordConverter != null))
            {
                return _rendrerer.CoordConverter.ConvertY(y);
            }
            return double.NaN;
        }

        internal double GetMaxX(Rect dataBounds)
        {
            if (_ax != null)
            {
                return _ax.Max0;
            }
            return dataBounds.Right;
        }

        internal double GetMaxY(Rect dataBounds)
        {
            if (_ay != null)
            {
                return _ay.Max0;
            }
            return dataBounds.Bottom;
        }

        internal double GetMinX(Rect dataBounds)
        {
            if (_ax != null)
            {
                return _ax.Min0;
            }
            return dataBounds.Left;
        }

        internal double GetMinY(Rect dataBounds)
        {
            if (_ay != null)
            {
                return _ay.Min0;
            }
            return dataBounds.Top;
        }

        internal void SetPoint(int pi, double x, double y)
        {
            _pi = pi;
            _dvals = null;
            Point point = new Point(x, y);
            Current = point;
            if (_excludeHoles)
            {
                if (!double.IsNaN(x) && !double.IsNaN(y))
                {
                    _pts.Add(point);
                }
            }
            else
            {
                if (!hasNan)
                {
                    hasNan = double.IsNaN(x) || double.IsNaN(y);
                }
                _pts.Add(point);
            }
        }

        internal void SetPrevious(double value)
        {
            if (double.IsNaN(value))
            {
                value = 0.0;
            }
            _stacked.Add(value);
        }

        public Rect Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                CalcClipBounds();
            }
        }

        internal Rect Bounds2D
        {
            get { return _bounds; }
        }

        internal Rect ClipBounds
        {
            get { return _clipBounds; }
        }

        public Point Current
        {
            get { return _cur; }
            set { _cur = value; }
        }

        public Point CurrentView
        {
            get { return _curView; }
            set { _curView = value; }
        }

        public double[] Data
        {
            get
            {
                if ((_dvals == null) && (_ds != null))
                {
                    double[,] values = _ds.GetValues();
                    if (values != null)
                    {
                        int length = values.GetLength(1);
                        int pointIndex = PointIndex;
                        if ((pointIndex >= 0) && (pointIndex < length))
                        {
                            int num3 = values.GetLength(0);
                            _dvals = new double[num3];
                            for (int i = 0; i < num3; i++)
                            {
                                _dvals[i] = values[i, PointIndex];
                            }
                        }
                    }
                }
                return _dvals;
            }
        }

        public IDataSeriesInfo DataSeries
        {
            get { return _ds; }
            internal set { _ds = value; }
        }

        internal bool IsCustomClipping
        {
            get
            {
                bool flag = false;
                if (Chart != null)
                {
                    switch (LineAreaOptions.GetClipping(Chart))
                    {
                        case Clipping.Auto:
                            {
                                BaseRenderer renderer = _rendrerer as BaseRenderer;
                                if ((renderer != null) && (renderer.CoordConverter != null))
                                {
                                    Rect dataBounds = renderer.CoordConverter.DataBounds;
                                    Rect rc = new Rect(GetMinX(dataBounds), GetMinY(dataBounds), GetMaxX(dataBounds) - GetMinX(dataBounds), GetMaxY(dataBounds) - GetMinY(dataBounds));
                                    flag = renderer.IsCustomClipping(rc);
                                }
                                return flag;
                            }
                        case Clipping.Custom:
                            return true;
                    }
                }
                return flag;
            }
        }

        public double this[string name]
        {
            get
            {
                double naN = double.NaN;
                if (_names != null)
                {
                    double[] data = Data;
                    if (data == null)
                    {
                        return naN;
                    }
                    int length = _names.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (name == _names[i])
                        {
                            if (i < data.Length)
                            {
                                naN = data[i];
                            }
                            return naN;
                        }
                    }
                }
                return naN;
            }
        }

        internal double OptimizationRadius
        {
            get
            {
                DataSeries chart = _ds as DataSeries;
                if (chart != null)
                {
                    double optimizationRadius = LineAreaOptions.GetOptimizationRadius(chart);
                    if (!double.IsNaN(optimizationRadius))
                    {
                        return optimizationRadius;
                    }
                }
                if (Chart != null)
                {
                    return LineAreaOptions.GetOptimizationRadius(Chart);
                }
                return double.NaN;
            }
        }

        internal Dt.Charts.OptimizationRadiusScope OptimizationRadiusScope
        {
            get
            {
                if (Chart != null)
                {
                    return LineAreaOptions.GetOptimizationRadiusScope(Chart);
                }
                return Dt.Charts.OptimizationRadiusScope.Lines;
            }
        }

        public int PointIndex
        {
            get { return _pi; }
        }

        public Point[] Points
        {
            get
            {
                if (_pts.Count <= 0)
                {
                    return null;
                }
                RadarRenderer renderer = Renderer as RadarRenderer;
                if ((renderer != null) && !renderer.IsPolar)
                {
                    _pts.Add(_pts[0]);
                }
                return _pts.ToArray();
            }
        }

        public Point Previous
        {
            get { return _prev; }
            set { _prev = value; }
        }

        public double[] PreviousValues
        {
            get
            {
                if (_stacked.Count > 0)
                {
                    return _stacked.ToArray();
                }
                return null;
            }
        }

        public Rect Rect
        {
            get { return _rect; }
            internal set { _rect = value; }
        }

        public IRenderer Renderer
        {
            get { return _rendrerer; }
        }
    }
}

