#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Charts
{
    public partial class ChartPanelObject : ContentControl
    {
        static Point EmptyPoint = new Point(double.NaN, double.NaN);

        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register("Action", typeof(ChartPanelAction), typeof(ChartPanelObject), new PropertyMetadata(ChartPanelAction.None, new PropertyChangedCallback(ChartPanelObject.OnActionChanged)));
        public static readonly DependencyProperty AttachProperty = DependencyProperty.Register("Attach", typeof(ChartPanelAttach), typeof(ChartPanelObject), new PropertyMetadata(ChartPanelAttach.None));
        public static readonly DependencyProperty AxisXProperty = DependencyProperty.Register("AxisX", typeof(string), typeof(ChartPanelObject), new PropertyMetadata("", new PropertyChangedCallback(ChartPanelObject.OnAxisXChanged)));
        public static readonly DependencyProperty AxisYProperty = DependencyProperty.Register("AxisY", typeof(string), typeof(ChartPanelObject), new PropertyMetadata("", new PropertyChangedCallback(ChartPanelObject.OnAxisYChanged)));
        public static readonly DependencyProperty DataPointProperty = DependencyProperty.Register("DataPoint", typeof(Point), typeof(ChartPanelObject), new PropertyMetadata(EmptyPoint, new PropertyChangedCallback(ChartPanelObject.OnDataPointChanged)));
        public static readonly DependencyProperty UseAxisLimitsProperty = DependencyProperty.Register("UseAxisLimits", typeof(bool), typeof(ChartPanelObject), new PropertyMetadata((bool)true));

        C1DragHelper _dragHelper;
        ChartPanel _panel;
        bool notify = true;
        
        public event EventHandler DataPointChanged;

        Point AdjustPoint(Point pt)
        {
            if ((Panel != null) && (Panel.Chart != null))
            {
                double actualMin;
                double actualMax;
                double num4;
                double num5;
                ChartView view = Panel.Chart.View;
                if (view == null)
                {
                    return pt;
                }
                Axis axisX = null;
                Axis axisY = null;
                if (!string.IsNullOrEmpty(AxisX))
                {
                    axisX = view.Axes[AxisX];
                }
                if (axisX == null)
                {
                    axisX = view.AxisX;
                }
                if (!string.IsNullOrEmpty(AxisY))
                {
                    axisY = view.Axes[AxisY];
                }
                if (axisY == null)
                {
                    axisY = view.AxisY;
                }
                if (axisX.Scale < 1.0)
                {
                    double num3 = (axisX.ActualMax - axisX.ActualMin) * axisX.Scale;
                    actualMin = axisX.ActualMin + (axisX.Value * ((axisX.ActualMax - axisX.ActualMin) - num3));
                    actualMax = actualMin + num3;
                }
                else
                {
                    actualMin = axisX.ActualMin;
                    actualMax = axisX.ActualMax;
                }
                if (axisY.Scale < 1.0)
                {
                    double num6 = (axisY.ActualMax - axisY.ActualMin) * axisY.Scale;
                    num4 = axisY.ActualMin + (axisY.Value * ((axisY.ActualMax - axisY.ActualMin) - num6));
                    num5 = num4 + num6;
                }
                else
                {
                    num4 = axisY.ActualMin;
                    num5 = axisY.ActualMax;
                }
                if (!double.IsNaN(pt.X))
                {
                    if (pt.X < actualMin)
                    {
                        pt.X = actualMin;
                    }
                    else if (pt.X > actualMax)
                    {
                        pt.X = actualMax;
                    }
                }
                if (double.IsNaN(pt.Y))
                {
                    return pt;
                }
                if (pt.Y < num4)
                {
                    pt.Y = num4;
                    return pt;
                }
                if (pt.Y > num5)
                {
                    pt.Y = num5;
                }
            }
            return pt;
        }

        static void OnActionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartPanelObject obj2 = (ChartPanelObject) obj;
            if (obj2.Panel != null)
            {
                obj2.Panel.DetachEvents(obj2, (ChartPanelAction) args.OldValue);
                obj2.Panel.AttachEvents(obj2, (ChartPanelAction) args.NewValue);
            }
        }

        static void OnAxisXChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ChartPanelObject) obj).InvalidateMeasure();
        }

        static void OnAxisYChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ChartPanelObject) obj).InvalidateMeasure();
        }

        protected virtual void OnDataPointChanged(DependencyPropertyChangedEventArgs args)
        {
            if (DataPointChanged != null)
            {
                DataPointChanged(this, EventArgs.Empty);
            }
            if (notify && (_panel != null))
            {
                _panel.InvalidateArrange();
            }
        }

        static void OnDataPointChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ChartPanelObject) obj).OnDataPointChanged(args);
        }

        internal void SetPoint(Point pt)
        {
            double x = DataPoint.X;
            if (!double.IsNaN(x))
            {
                x = pt.X;
            }
            double y = DataPoint.Y;
            if (!double.IsNaN(y))
            {
                y = pt.Y;
            }
            if (UseAxisLimits)
            {
                pt = AdjustPoint(new Point(x, y));
            }
            if (double.IsNaN(pt.X))
            {
                pt.X = DataPoint.X;
            }
            if (double.IsNaN(pt.Y))
            {
                pt.Y = DataPoint.Y;
            }
            notify = false;
            DataPoint = pt;
            notify = true;
        }

        public ChartPanelAction Action
        {
            get { return  (ChartPanelAction) base.GetValue(ActionProperty); }
            set { base.SetValue(ActionProperty, value); }
        }

        public ChartPanelAttach Attach
        {
            get { return  (ChartPanelAttach) base.GetValue(AttachProperty); }
            set { base.SetValue(AttachProperty, value); }
        }

        public string AxisX
        {
            get { return  (string) ((string) base.GetValue(AxisXProperty)); }
            set { base.SetValue(AxisXProperty, value); }
        }

        public string AxisY
        {
            get { return  (string) ((string) base.GetValue(AxisYProperty)); }
            set { base.SetValue(AxisYProperty, value); }
        }

        public Point DataPoint
        {
            get { return  (Point) base.GetValue(DataPointProperty); }
            set { base.SetValue(DataPointProperty, value); }
        }

        internal C1DragHelper DragHelper
        {
            get { return _dragHelper; }
            set { _dragHelper = value; }
        }

        internal ChartPanel Panel
        {
            get { return  _panel; }
            set
            {
                if (_panel != value)
                {
                    if (_panel != null)
                    {
                        _panel.DetachEvents(this, Action);
                    }
                    _panel = value;
                    if (_panel != null)
                    {
                        _panel.AttachEvents(this, Action);
                    }
                }
            }
        }

        public bool UseAxisLimits
        {
            get { return  (bool) ((bool) base.GetValue(UseAxisLimitsProperty)); }
            set { base.SetValue(UseAxisLimitsProperty, (bool) value); }
        }
    }
}

