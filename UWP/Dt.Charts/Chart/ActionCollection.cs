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
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Charts
{
    public class ActionCollection : ObservableCollection<Action>
    {
        Action _activeAction;
        ActionAdorner _adorner;
        Chart _chart;
        Point _pt0 = new Point();
        Rect _rect = new Rect();
        double _scalex;
        double _scaley;
        ActionType _state;
        Storyboard _stb;
        bool _stb_finished;
        double _updateDelay = 40.0;
        double _valx;
        double _valy;
        const double minScale = 1E-05;

        internal ActionCollection(Chart chart)
        {
            _chart = chart;
        }

        void _stb_Completed(object sender, object e)
        {
            UpdateAxesDirect();
            _stb_finished = true;
        }

        ActionType FindAction(PointerRoutedEventArgs e)
        {
            _activeAction = null;
            ActionType none = ActionType.None;
            int num = base.Count;
            Point position = GetPosition(e);
            if (Chart.View.PlotRect.Contains(position))
            {
                if ((num == 0) && ((Chart.View.AxisX.ScrollBar != null) || (Chart.View.AxisY.ScrollBar != null)))
                {
                    return ActionType.Translate;
                }
                for (int i = 0; i < num; i++)
                {
                    Action action = base[i];
                    if (e.KeyModifiers == action.Modifiers)
                    {
                        none = action.ActionType;
                        _activeAction = action;
                        return none;
                    }
                }
            }
            return none;
        }

        internal void FireEnter()
        {
            if (Chart != null)
            {
                Chart.FireActionEnter(_activeAction, EventArgs.Empty);
            }
        }

        internal void FireLeave()
        {
            if (Chart != null)
            {
                Chart.FireActionLeave(_activeAction, EventArgs.Empty);
            }
        }

        internal Action GetAction(ActionType atype)
        {
            foreach (Action action in this)
            {
                if (action.ActionType == atype)
                {
                    return action;
                }
            }
            return null;
        }

        Point GetPosition(PointerRoutedEventArgs e)
        {
            return e.GetCurrentPoint(Chart.View.Viewport).Position;
        }

        internal void OnMouseDown(PointerRoutedEventArgs e)
        {
            if ((State == ActionType.None) && (Chart.Data.Renderer is Renderer2D))
            {
                ActionType zoom = FindAction(e);
                if (Chart.GestureSlide == GestureSlideAction.Zoom)
                {
                    zoom = ActionType.Zoom;
                }
                if (zoom != ActionType.None)
                {
                    _pt0 = GetPosition(e);
                    if (Chart.CapturePointer(e.Pointer))
                    {
                        State = zoom;
                        _valx = Chart.View.AxisX.Value;
                        _valy = Chart.View.AxisY.Value;
                        _scalex = Chart.View.AxisX.Scale;
                        _scaley = Chart.View.AxisY.Scale;
                        Adorner.Shape.Width = 0.0;
                        Adorner.Shape.Height = 0.0;
                        if (!Chart.View.Children.Contains(Adorner.Shape))
                        {
                            Chart.View.Children.Add(Adorner.Shape);
                        }
                        if (zoom == ActionType.Zoom)
                        {
                            ZoomAction action = GetAction(ActionType.Zoom) as ZoomAction;
                            if (action != null)
                            {
                                if (action.Fill != null)
                                {
                                    Adorner.Shape.Fill = action.Fill;
                                }
                                if (action.Stroke != null)
                                {
                                    Adorner.Shape.Stroke = action.Stroke;
                                }
                            }
                        }
                        FireEnter();
                    }
                }
            }
        }

        internal void OnMouseMove(PointerRoutedEventArgs e)
        {
            if ((State == ActionType.None) || (State == ActionType.Pinch))
            {
                if (Chart.View.Children.Contains(Adorner.Shape))
                {
                    Chart.View.Children.Remove(Adorner.Shape);
                }
            }
            else
            {
                Point position = GetPosition(e);
                switch (State)
                {
                    case ActionType.Translate:
                        PerformTranslate(_pt0, position);
                        _pt0 = position;
                        break;

                    case ActionType.Scale:
                        PerformScale(_pt0, position);
                        _pt0 = position;
                        break;
                }
                double x = Math.Min(_pt0.X, position.X);
                Point point2 = new Point(x, Math.Min(_pt0.Y, position.Y));
                double introduced11 = Math.Max(_pt0.X, position.X);
                Point point3 = new Point(introduced11, Math.Max(_pt0.Y, position.Y));
                if ((point2.X != point3.X) && (point2.Y != point3.Y))
                {
                    Rect plotRect = Chart.View.PlotRect;
                    if (Chart.View.AxisX.MinScale == Chart.View.AxisX.Scale)
                    {
                        point2.X = plotRect.Left;
                        point3.X = plotRect.Right;
                    }
                    if (Chart.View.AxisY.MinScale == Chart.View.AxisY.Scale)
                    {
                        point2.Y = plotRect.Top;
                        point3.Y = plotRect.Bottom;
                    }
                    Rect = new Rect(point2.X, point2.Y, point3.X - point2.X, point3.Y - point2.Y);
                    Rect rect = Rect;
                    if (rect.X < plotRect.X)
                    {
                        rect.Width = Math.Max((double) 0.0, (double) (rect.Width - (plotRect.X - rect.X)));
                        rect.X = plotRect.X;
                    }
                    if (rect.Y < plotRect.Y)
                    {
                        rect.Height = Math.Max((double) 0.0, (double) (rect.Height - (plotRect.Y - rect.Y)));
                        rect.Y = plotRect.Y;
                    }
                    if (rect.Right > plotRect.Right)
                    {
                        rect.Width = Math.Max((double) 0.0, (double) (rect.Width - (rect.Right - plotRect.Right)));
                    }
                    if (rect.Bottom > plotRect.Bottom)
                    {
                        rect.Height = Math.Max((double) 0.0, (double) (rect.Height - (rect.Bottom - plotRect.Bottom)));
                    }
                    Rect = rect;
                    Adorner.Shape.Width = Rect.Width;
                    Adorner.Shape.Height = Rect.Height;
                    Canvas.SetLeft(Adorner.Shape, Rect.Left);
                    Canvas.SetTop(Adorner.Shape, Rect.Top);
                }
            }
        }

        internal void OnMouseUp(PointerRoutedEventArgs e)
        {
            if (State != ActionType.None)
            {
                Chart.CapturePointer(e.Pointer);
                if (State == ActionType.Zoom)
                {
                    Point position = GetPosition(e);
                    if ((position.X != _pt0.X) && (position.Y != _pt0.Y))
                    {
                        PerformZoom(_pt0, position);
                    }
                    _pt0 = position;
                }
                if (Chart.View.Children.Contains(Adorner.Shape))
                {
                    Chart.View.Children.Remove(Adorner.Shape);
                }
                State = ActionType.None;
                FireLeave();
            }
        }

        internal void PerformScale(Point pt)
        {
            Axis axisX = Chart.View.AxisX;
            Axis axisY = Chart.View.AxisY;
            _valx = Chart.View.AxisX.Value;
            _valy = Chart.View.AxisY.Value;
            _scalex = Chart.View.AxisX.Scale;
            _scaley = Chart.View.AxisY.Scale;
            pt = Chart.View.PointToData(pt);
            double valx = (pt.X - axisX.ActualMin) / (axisX.ActualMax - axisX.ActualMin);
            double valy = (pt.Y - axisY.ActualMin) / (axisY.ActualMax - axisY.ActualMin);
            double num3 = 0.5;
            double scalex = _scalex * num3;
            if (scalex > 1.0)
            {
                scalex = 1.0;
            }
            if (scalex < 1E-05)
            {
                scalex = 1E-05;
            }
            double scaley = _scaley * num3;
            if (scaley > 1.0)
            {
                scaley = 1.0;
            }
            if (scaley < 1E-05)
            {
                scaley = 1E-05;
            }
            UpdateAxes(scalex, valx, scaley, valy, false);
        }

        void PerformScale(ScaleAction sa, double delta)
        {
            if (sa != null)
            {
                Axis axisX = Chart.View.AxisX;
                Axis axisY = Chart.View.AxisY;
                double num = 1.0 - sa.MouseWheelFactor;
                if (num >= 1.0)
                {
                    num = 0.95;
                }
                else if (num <= 0.0)
                {
                    num = 0.05;
                }
                double scale = axisX.Scale;
                double valx = (scale == 1.0) ? 0.5 : axisX.Value;
                if ((sa.MouseWheelDirection & MouseWheelDirection.X) > MouseWheelDirection.None)
                {
                    if (delta > 0.0)
                    {
                        scale *= num;
                    }
                    else
                    {
                        scale /= num;
                    }
                }
                double scaley = axisY.Scale;
                double valy = (scaley == 1.0) ? 0.5 : axisY.Value;
                if ((sa.MouseWheelDirection & MouseWheelDirection.Y) > MouseWheelDirection.None)
                {
                    if (delta > 0.0)
                    {
                        scaley *= num;
                    }
                    else
                    {
                        scaley /= num;
                    }
                }
                UpdateAxes(scale, valx, scaley, valy, true);
            }
        }

        void PerformScale(Point pt0, Point pt1)
        {
            Axis axisX = Chart.View.AxisX;
            Axis axisY = Chart.View.AxisY;
            Rect plotRect = Chart.View.PlotRect;
            double num1 = (pt1.X - pt0.X) / plotRect.Width;
            double y = (pt1.Y - pt0.Y) / plotRect.Height;
            if (y != 0.0)
            {
                double num2 = Math.Pow(10.0, y);
                double scalex = _scalex * num2;
                if (scalex > 1.0)
                {
                    scalex = 1.0;
                }
                if (scalex < 1E-05)
                {
                    scalex = 1E-05;
                }
                double scaley = _scaley * num2;
                if (scaley > 1.0)
                {
                    scaley = 1.0;
                }
                if (scaley < 1E-05)
                {
                    scaley = 1E-05;
                }
                UpdateAxes(scalex, axisX.Value, scaley, axisY.Value, false);
            }
        }

        internal void PerformScale(Point pt, double xfactor, double yfactor)
        {
            Axis axisX = Chart.View.AxisX;
            Axis axisY = Chart.View.AxisY;
            _valx = Chart.View.AxisX.Value;
            _valy = Chart.View.AxisY.Value;
            _scalex = Chart.View.AxisX.Scale;
            _scaley = Chart.View.AxisY.Scale;
            pt = Chart.View.PointToData(pt);
            double valx = (pt.X - axisX.ActualMin) / (axisX.ActualMax - axisX.ActualMin);
            double valy = (pt.Y - axisY.ActualMin) / (axisY.ActualMax - axisY.ActualMin);
            double scalex = _scalex / xfactor;
            if (scalex > 1.0)
            {
                scalex = 1.0;
            }
            if (scalex < 1E-05)
            {
                scalex = 1E-05;
            }
            double scaley = _scaley / yfactor;
            if (scaley > 1.0)
            {
                scaley = 1.0;
            }
            if (scaley < 1E-05)
            {
                scaley = 1E-05;
            }
            UpdateAxes(scalex, valx, scaley, valy, false);
        }

        void PerformTranslate(TranslateAction ta, double delta)
        {
            if (ta != null)
            {
                Axis axisX = Chart.View.AxisX;
                Axis axisY = Chart.View.AxisY;
                double mouseWheelFactor = ta.MouseWheelFactor;
                double scale = axisX.Scale;
                double valx = axisX.Value;
                if (((ta.MouseWheelDirection & MouseWheelDirection.X) > MouseWheelDirection.None) && (scale < 1.0))
                {
                    double num4 = axisX.Reversed ? -delta : delta;
                    if (num4 > 0.0)
                    {
                        valx -= mouseWheelFactor * scale;
                    }
                    else
                    {
                        valx += mouseWheelFactor * scale;
                    }
                }
                double scaley = axisY.Scale;
                double valy = axisY.Value;
                if (((ta.MouseWheelDirection & MouseWheelDirection.Y) > MouseWheelDirection.None) && (scaley < 1.0))
                {
                    double num7 = axisY.Reversed ? -delta : delta;
                    if (num7 > 0.0)
                    {
                        valy += mouseWheelFactor * scaley;
                    }
                    else
                    {
                        valy -= mouseWheelFactor * scaley;
                    }
                }
                UpdateAxes(scale, valx, scaley, valy, true);
            }
        }

        internal void PerformTranslate(Point pt0, Point pt1)
        {
            Axis axisX = Chart.View.AxisX;
            Axis axisY = Chart.View.AxisY;
            Rect plotRect = Chart.View.PlotRect;
            double num = (pt1.X - pt0.X) / plotRect.Width;
            double delta = (pt1.Y - pt0.Y) / plotRect.Height;
            if ((num != 0.0) || (delta != 0.0))
            {
                if (num != 0.0)
                {
                    _valx = TranslateUpdateAxis(axisX, -num, _valx);
                }
                if (delta != 0.0)
                {
                    _valy = TranslateUpdateAxis(axisY, delta, _valy);
                }
                UpdateAxes(axisX.Scale, _valx, axisY.Scale, _valy, false);
            }
        }

        internal void PerformZoom(Point pt0, Point pt1)
        {
            Axis axisX = Chart.View.AxisX;
            Axis axisY = Chart.View.AxisY;
            Rect plotRect = Chart.View.PlotRect;
            pt0 = Chart.View.PointToData(pt0);
            pt1 = Chart.View.PointToData(pt1);
            Point point = new Point(Math.Min(pt0.X, pt1.X), Math.Min(pt0.Y, pt1.Y));
            Point point2 = new Point(Math.Max(pt0.X, pt1.X), Math.Max(pt0.Y, pt1.Y));
            if (double.IsNaN(point.X))
            {
                point.X = axisX.ActualMin;
            }
            else
            {
                point.X = Math.Max(point.X, axisX.ActualMin);
            }
            if (double.IsNaN(point2.X))
            {
                point2.X = axisX.ActualMax;
            }
            else
            {
                point2.X = Math.Min(point2.X, axisX.ActualMax);
            }
            if (double.IsNaN(point.Y))
            {
                point.Y = axisY.ActualMin;
            }
            else
            {
                point.Y = Math.Max(point.Y, axisY.ActualMin);
            }
            if (double.IsNaN(point2.Y))
            {
                point2.Y = axisY.ActualMax;
            }
            else
            {
                point2.Y = Math.Min(point2.Y, axisY.ActualMax);
            }
            Chart.BeginUpdate();
            ZoomUpdateAxis(axisX, point.X, point2.X);
            ZoomUpdateAxis(axisY, point.Y, point2.Y);
            Chart.EndUpdate();
            _valx = axisX.Value;
            _valy = axisY.Value;
            _scalex = axisX.Scale;
            _scaley = axisY.Scale;
        }

        static double TranslateUpdateAxis(Axis ax, double delta, double val)
        {
            if (ax.Scale == 1.0)
            {
                return 0.0;
            }
            double num = (ax.ActualMax - ax.ActualMin) * ax.Scale;
            double num2 = (val * ((ax.ActualMax - ax.ActualMin) - num)) + ax.ActualMin;
            double num3 = num2 + (num * delta);
            double num4 = (num3 - ax.ActualMin) / ((ax.ActualMax - ax.ActualMin) - num);
            if (num4 < 0.0)
            {
                return 0.0;
            }
            if (num4 > 1.0)
            {
                num4 = 1.0;
            }
            return num4;
        }

        void UpdateAxes(double scalex, double valx, double scaley, double valy, bool immediate)
        {
            _scalex = scalex;
            _valx = valx;
            _scaley = scaley;
            _valy = valy;
            if (immediate || (UpdateDelay == 0.0))
            {
                UpdateAxesDirect();
            }
            else
            {
                if (_stb == null)
                {
                    _stb = new Storyboard();
                    _stb.Duration = new Windows.UI.Xaml.Duration(TimeSpan.FromMilliseconds(UpdateDelay));
                    Storyboard storyboard = _stb;
                    storyboard.Completed += _stb_Completed;
                    _stb_finished = true;
                }
                if (_stb_finished)
                {
                    _stb_finished = false;
                    _stb.Begin();
                }
            }
        }

        void UpdateAxesDirect()
        {
            Axis axisX = Chart.View.AxisX;
            Axis axisY = Chart.View.AxisY;
            Chart.BeginUpdate();
            axisX.Scale = _scalex;
            axisX.Value = _valx;
            axisY.Scale = _scaley;
            axisY.Value = _valy;
            Chart.EndUpdate();
        }

        static void ZoomUpdateAxis(Axis ax, double min, double max)
        {
            if (ax.ActualMin != ax.ActualMax)
            {
                double num = (max - min) / (ax.ActualMax - ax.ActualMin);
                if (num < 1E-05)
                {
                    num = 1E-05;
                }
                double num2 = (ax.ActualMax - ax.ActualMin) * num;
                double num3 = 0.0;
                if (num != 1.0)
                {
                    num3 = (min - ax.ActualMin) / ((ax.ActualMax - ax.ActualMin) - num2);
                }
                ax.Scale = num;
                ax.Value = num3;
            }
        }

        ActionAdorner Adorner
        {
            get
            {
                if (_adorner == null)
                {
                    _adorner = new ActionAdorner(this);
                }
                return _adorner;
            }
        }

        internal Chart Chart
        {
            get { return  _chart; }
        }

        internal Rect Rect
        {
            get { return  _rect; }
            set { _rect = value; }
        }

        internal ActionType State
        {
            get { return  _state; }
            set { _state = value; }
        }

        [DefaultValue(40)]
        internal double UpdateDelay
        {
            get { return  _updateDelay; }
            set
            {
                if (_updateDelay != value)
                {
                    if (value < 0.0)
                    {
                        throw new ArgumentException("The value must be nonnegative.");
                    }
                    _updateDelay = value;
                    if (_stb != null)
                    {
                        _stb.Duration = new Windows.UI.Xaml.Duration(TimeSpan.FromMilliseconds(UpdateDelay));
                    }
                }
            }
        }
    }
}

