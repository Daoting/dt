#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the line sparkline view.
    /// </summary>
    public partial class LineSparklineView : BaseSparklineView
    {
        private List<Windows.UI.Xaml.Shapes.Line> lines;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="viewInfo">The line sparkline view information.</param>
        public LineSparklineView(LineSparklineViewInfo viewInfo) : base(viewInfo)
        {
        }

        private void ArrangeLines(Windows.Foundation.Size finalSize)
        {
            if (this.lines != null)
            {
                using (List<Windows.UI.Xaml.Shapes.Line>.Enumerator enumerator = this.lines.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Windows.Foundation.Point location = new Windows.Foundation.Point();
                        enumerator.Current.Arrange(new Windows.Foundation.Rect(location, finalSize));
                    }
                }
            }
        }

        /// <summary>
        /// Positions child elements and determines the size when overridden in a derived class.
        /// </summary>
        /// <param name="finalSize"> The final area within the parent that this element uses to arrange itself and its children.</param>
        /// <returns> The actual size used.</returns>
        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            this.ArrangeLines(finalSize);
            return finalSize;
        }

        internal override bool DrawDataPointsBeyondMaxAndMin()
        {
            return false;
        }

        internal override UIElement GetDataPoint(int index, Windows.Foundation.Size availableSize)
        {
            Rectangle r = new Rectangle();
            Windows.UI.Color color = base.GetDataPointColor(index);
            Dt.Cells.Data.UIAdaptor.InvokeSync(delegate {
                r.Fill = new SolidColorBrush(color);
            });
            r.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            RotateTransform transform = new RotateTransform();
            transform.Angle = 45.0;
            r.RenderTransform = transform;
            return r;
        }

        private void MeasureLines(Windows.Foundation.Size availableSize)
        {
            LineSparklineViewInfo sparklineViewInfo = base.SparklineViewInfo as LineSparklineViewInfo;
            if (sparklineViewInfo != null)
            {
                List<Tuple<Windows.Foundation.Point, Windows.Foundation.Point>> linePos = sparklineViewInfo.LinePos;
                sparklineViewInfo.MeasurelinePos(availableSize);
                if ((linePos != null) && (linePos.Count != sparklineViewInfo.LinePos.Count))
                {
                    this.RemoveLines();
                }
            }
            if ((sparklineViewInfo.LinePos != null) && (sparklineViewInfo.LinePos.Count > 0))
            {
                if (this.lines == null)
                {
                    this.lines = new List<Windows.UI.Xaml.Shapes.Line>();
                }
                for (int i = 0; i < sparklineViewInfo.LinePos.Count; i++)
                {
                    Action action = null;
                    Windows.UI.Xaml.Shapes.Line line;
                    Tuple<Windows.Foundation.Point, Windows.Foundation.Point> tuple = sparklineViewInfo.LinePos[i];
                    if (tuple != null)
                    {
                        line = null;
                        if (i >= this.lines.Count)
                        {
                            line = new Windows.UI.Xaml.Shapes.Line();
                            line.StrokeStartLineCap = PenLineCap.Round;
                            line.StrokeEndLineCap = PenLineCap.Round;
                            if (action == null)
                            {
                                action = delegate {
                                    line.Stroke = new SolidColorBrush(this.SparklineInfo.Setting.SeriesColor);
                                };
                            }
                            Dt.Cells.Data.UIAdaptor.InvokeSync(action);
                            double lineWeight = (base.SparklineViewInfo as LineSparklineViewInfo).GetLineWeight();
                            line.StrokeThickness = lineWeight;
                            Canvas.SetZIndex(line, this.LineZIndex);
                            base.Children.Add(line);
                            this.lines.Add(line);
                        }
                        else
                        {
                            line = this.lines[i];
                        }
                        Windows.Foundation.Point point = tuple.Item1;
                        Windows.Foundation.Point point2 = tuple.Item2;
                        line.X1 = point.X;
                        line.X2 = point2.X;
                        line.Y1 = point.Y;
                        line.Y2 = point2.Y;
                        line.Measure(availableSize);
                    }
                }
            }
        }

        /// <summary>
        /// Measures the layout size required
        /// for child elements and determines a size for the FrameworkElement's derived class when overridden in a derived class.
        /// </summary>
        /// <param name="availableSize"> 
        /// The available size that this element can give to child elements.
        /// </param>
        /// <returns> 
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
            {
                this.MeasureLines(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        private void RemoveLines()
        {
            if (this.lines != null)
            {
                foreach (Windows.UI.Xaml.Shapes.Line line in this.lines)
                {
                    if (line != null)
                    {
                        base.Children.Remove(line);
                    }
                }
            }
            this.lines = null;
        }

        /// <summary>
        /// Updates the sparkline in the view when overridden in a derived class.
        /// </summary>
        /// <param name="size">The updated size.</param>
        /// <param name="zoomfactor">The zoom factor used for the update.</param>
        public override void Update(Windows.Foundation.Size size, double zoomfactor)
        {
            this.RemoveLines();
            this.lines = null;
            (base.SparklineViewInfo as LineSparklineViewInfo).LinePos = null;
            base.Update(size, zoomfactor);
        }

        internal override int AxisZIndex
        {
            get { return  90; }
        }

        internal override int DataPointZIndex
        {
            get { return  110; }
        }

        internal virtual int LineZIndex
        {
            get { return  100; }
        }
    }
}

