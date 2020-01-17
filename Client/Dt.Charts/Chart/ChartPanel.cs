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
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public partial class ChartPanel : Panel, IChartLayer
    {
        Chart _chart;
        ChartPanelObjectCollection _children;
        ChartPanelObject moving;
        Point offset = new Point();

        public ChartPanel()
        {
            _children = new ChartPanelObjectCollection(this);
            Background = new SolidColorBrush(Colors.Transparent);
            PointerMoved += ChartPanel_MouseMove;
        }

        void _dragHelper_DragCompleted(object sender, C1DragCompletedEventArgs e)
        {
            if (moving != null)
            {
                moving = null;
            }
        }

        void _dragHelper_DragDelta(object sender, C1DragDeltaEventArgs e)
        {
            if (moving != null)
            {
                Point position = e.GetPosition(Chart.View.Viewport);
                position.X -= offset.X;
                position.Y -= offset.Y;
                if (moving.Attach != ChartPanelAttach.None)
                {
                    MeasureOption x = MeasureOption.X;
                    if (moving.Attach == ChartPanelAttach.DataY)
                    {
                        x = MeasureOption.Y;
                    }
                    else if (moving.Attach == ChartPanelAttach.DataXY)
                    {
                        x = MeasureOption.XY;
                    }
                    int seriesIndex = -1;
                    int pointIndex = -1;
                    DataDistanceFromPoint(position, x, out seriesIndex, out pointIndex);
                    if ((seriesIndex >= 0) && (pointIndex >= 0))
                    {
                        position = Chart.View.DataIndexToPoint(seriesIndex, pointIndex);
                    }
                }
                position = Chart.View.PointToData(moving.AxisX, moving.AxisY, position);
                moving.SetPoint(position);
                base.InvalidateArrange();
            }
        }

        void _dragHelper_DragStarted(object sender, C1DragStartedEventArgs e)
        {
            ChartPanelObject relativeTo = sender as ChartPanelObject;
            if (relativeTo != null)
            {
                moving = relativeTo;
                offset = e.GetPosition(relativeTo);
                FrameworkElement element = relativeTo;
                if (element != null)
                {
                    if (element.HorizontalAlignment == HorizontalAlignment.Center)
                    {
                        offset.X -= 0.5 * element.ActualWidth;
                    }
                    if (element.VerticalAlignment == VerticalAlignment.Center)
                    {
                        offset.Y -= 0.5 * element.ActualHeight;
                    }
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_chart != null)
            {
                int num = Children.Count;
                for (int i = 0; i < num; i++)
                {
                    ChartPanelObject obj2 = Children[i];
                    double x = obj2.DataPoint.X;
                    double y = obj2.DataPoint.Y;
                    Point pt = new Point(x, y);
                    pt = Chart.View.PointFromData(obj2.AxisX, obj2.AxisY, pt);
                    if (double.IsNaN(x))
                    {
                        pt.X = 0.0;
                    }
                    else
                    {
                        pt.X -= Chart.View.PlotRect.X;
                    }
                    if (double.IsNaN(y))
                    {
                        pt.Y = 0.0;
                    }
                    else
                    {
                        pt.Y -= Chart.View.PlotRect.Y;
                    }
                    FrameworkElement element = obj2;
                    double width = obj2.DesiredSize.Width;
                    double height = obj2.DesiredSize.Height;
                    if (element != null)
                    {
                        if ((double.IsNaN(element.Width) && (element.HorizontalAlignment == HorizontalAlignment.Stretch)) && (finalSize.Width > pt.X))
                        {
                            width = finalSize.Width - pt.X;
                        }
                        if ((double.IsNaN(element.Height) && (element.VerticalAlignment == VerticalAlignment.Stretch)) && (finalSize.Height > pt.Y))
                        {
                            height = finalSize.Height - pt.Y;
                        }
                    }
                    if (!double.IsNaN(pt.X) && !double.IsNaN(pt.Y))
                    {
                        if (element.HorizontalAlignment == HorizontalAlignment.Center)
                        {
                            pt.X -= 0.5 * width;
                        }
                        else if (element.HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            pt.X -= width;
                        }
                        if (element.VerticalAlignment == VerticalAlignment.Center)
                        {
                            pt.Y -= 0.5 * height;
                        }
                        else if (element.VerticalAlignment == VerticalAlignment.Bottom)
                        {
                            pt.Y -= height;
                        }
                        Rect rect = new Rect(pt.X, pt.Y, width, height);
                        if (((rect.Right < 0.0) || (rect.Bottom < 0.0)) || ((rect.Left > finalSize.Width) || (rect.Top > finalSize.Height)))
                        {
                            Rect rect4 = new Rect();
                            obj2.Arrange(rect4);
                        }
                        else
                        {
                            obj2.Arrange(rect);
                        }
                    }
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        internal void AttachEvents(ChartPanelObject obj, ChartPanelAction action)
        {
            ClearDragHelper(obj);
            C1DragHelper helper = null;
            if (action == ChartPanelAction.LeftMouseButtonDrag)
            {
                helper = new C1DragHelper(obj, C1DragHelperMode.TranslateXY, 1.0, true, false, false, false);
            }
            else if (action == ChartPanelAction.RightMouseButtonDrag)
            {
                helper = new C1DragHelper(obj, C1DragHelperMode.TranslateXY, 1.0, true, false, false, true);
            }
            if (helper != null)
            {
                helper.DragStarted += new EventHandler<C1DragStartedEventArgs>(_dragHelper_DragStarted);
                helper.DragDelta += new EventHandler<C1DragDeltaEventArgs>(_dragHelper_DragDelta);
                helper.DragCompleted += new EventHandler<C1DragCompletedEventArgs>(_dragHelper_DragCompleted);
                obj.DragHelper = helper;
            }
        }
        
        void ClearDragHelper(ChartPanelObject obj)
        {
            C1DragHelper dragHelper = obj.DragHelper;
            if (dragHelper != null)
            {
                dragHelper.Complete();
                dragHelper.FinalizeDrag();
                dragHelper.DragStarted -= new EventHandler<C1DragStartedEventArgs>(_dragHelper_DragStarted);
                dragHelper.DragDelta -= new EventHandler<C1DragDeltaEventArgs>(_dragHelper_DragDelta);
                dragHelper.DragCompleted -= new EventHandler<C1DragCompletedEventArgs>(_dragHelper_DragCompleted);
                obj.DragHelper = null;
            }
        }

        void ChartPanel_MouseMove(object sender, PointerRoutedEventArgs e)
        {
            int num = Children.Count;
            bool flag = false;
            for (int i = 0; i < num; i++)
            {
                ChartPanelObject obj2 = Children[i];
                if (obj2.Action == ChartPanelAction.MouseMove)
                {
                    Point position = e.GetPosition(Chart.View.Viewport);
                    if (obj2.Attach != ChartPanelAttach.None)
                    {
                        int num3;
                        int num4;
                        MeasureOption x = MeasureOption.X;
                        if (obj2.Attach == ChartPanelAttach.DataY)
                        {
                            x = MeasureOption.Y;
                        }
                        else if (obj2.Attach == ChartPanelAttach.DataXY)
                        {
                            x = MeasureOption.XY;
                        }
                        DataDistanceFromPoint(position, x, out num3, out num4);
                        if ((num3 >= 0) && (num4 >= 0))
                        {
                            position = Chart.View.DataIndexToPoint(num3, num4);
                        }
                    }
                    position = Chart.View.PointToData(obj2.AxisX, obj2.AxisY, position);
                    obj2.SetPoint(position);
                    flag = true;
                }
            }
            if (flag)
            {
                base.InvalidateArrange();
            }
        }

        double DataDistanceFromPoint(Point pt, MeasureOption measureOption, out int seriesIndex, out int pointIndex)
        {
            int num = Chart.Data.Children.Count;
            double naN = double.NaN;
            seriesIndex = -1;
            pointIndex = -1;
            for (int i = 0; i < num; i++)
            {
                double distance = 0.0;
                int num5 = Chart.View.DataIndexFromPoint(pt, i, measureOption, out distance);
                if (double.IsNaN(naN) || (distance < naN))
                {
                    naN = distance;
                    seriesIndex = i;
                    pointIndex = num5;
                }
            }
            return naN;
        }

        internal void DetachEvents(ChartPanelObject obj, ChartPanelAction action)
        {
            ClearDragHelper(obj);
        }

        public Point GetDataCoordinates(PointerRoutedEventArgs args)
        {
            if (Chart != null)
            {
                Point position = args.GetPosition(Chart.View.Viewport);
                return Chart.View.PointToData(position);
            }
            return new Point(double.NaN, double.NaN);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_chart != null)
            {
                int num = Children.Count;
                for (int i = 0; i < num; i++)
                {
                    Children[i].Measure(availableSize);
                }
            }
            return base.MeasureOverride(availableSize);
        }

        internal UIElementCollection BaseChildren
        {
            get { return  base.Children; }
        }

        public Chart Chart
        {
            get { return  _chart; }
            set
            {
                if (_chart != value)
                {
                    _chart = value;
                }
            }
        }

        public new ChartPanelObjectCollection Children
        {
            get { return  _children; }
        }
    }
}

