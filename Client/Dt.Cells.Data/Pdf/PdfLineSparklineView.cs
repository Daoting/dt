#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the line sparkline view.
    /// </summary>
    internal class PdfLineSparklineView : PdfBaseSparklineView
    {
        List<Windows.UI.Xaml.Shapes.Line> lines;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PdfLineSparklineView" /> class.
        /// </summary>
        /// <param name="viewInfo">The view info.</param>
        public PdfLineSparklineView(LineSparklineViewInfo viewInfo) : base(viewInfo)
        {
        }

        internal override bool DrawDataPointsBeyondMaxAndMin()
        {
            return false;
        }

        internal List<Windows.UI.Xaml.Shapes.Line> GetLines(Windows.Foundation.Size size)
        {
            this.MeasureLines(size);
            if (this.lines != null)
            {
                Windows.UI.Xaml.Shapes.Line axisLine = base.GetAxisLine(size);
                if (axisLine != null)
                {
                    this.lines.Insert(0, axisLine);
                }
            }
            return this.lines;
        }

        internal double GetLineWeight()
        {
            double num = base.SparklineInfo.Setting.LineWeight * 1.0;
            if (num < 1.0)
            {
                num = 1.0;
            }
            return num;
        }

        void MeasureLines(Windows.Foundation.Size availableSize)
        {
            LineSparklineViewInfo sparklineViewInfo = base.SparklineViewInfo as LineSparklineViewInfo;
            if (sparklineViewInfo != null)
            {
                sparklineViewInfo.MeasurelinePos(availableSize);
            }
            if ((sparklineViewInfo.LinePos != null) && (sparklineViewInfo.LinePos.Count > 0))
            {
                if (this.lines == null)
                {
                    this.lines = new List<Windows.UI.Xaml.Shapes.Line>();
                }
                for (int i = 0; i < sparklineViewInfo.LinePos.Count; i++)
                {
                    Windows.UI.Xaml.Shapes.Line line;
                    Windows.Foundation.Point p1;
                    Windows.Foundation.Point p2;
                    Tuple<Windows.Foundation.Point, Windows.Foundation.Point> tuple = sparklineViewInfo.LinePos[i];
                    if (tuple != null)
                    {
                        line = null;
                        if (i >= this.lines.Count)
                        {
                            line = new Windows.UI.Xaml.Shapes.Line();
                            line.StrokeStartLineCap = PenLineCap.Round;
                            line.StrokeEndLineCap = PenLineCap.Round;
                            line.Stroke = new SolidColorBrush(this.SparklineInfo.Setting.SeriesColor);
                            double lineWeight = this.GetLineWeight();
                            line.StrokeThickness = lineWeight;
                            this.lines.Add(line);
                        }
                        else
                        {
                            line = this.lines[i];
                        }
                        p1 = tuple.Item1;
                        p2 = tuple.Item2;
                        line.X1 = p1.X;
                        line.X2 = p2.X;
                        line.Y1 = p1.Y;
                        line.Y2 = p2.Y;
                    }
                }
            }
        }

        internal override void Paint(Graphics gs, Windows.Foundation.Size avilableSize)
        {
            List<Windows.UI.Xaml.Shapes.Line> lines = this.GetLines(avilableSize);
            if (lines != null)
            {
                using (List<Windows.UI.Xaml.Shapes.Line>.Enumerator enumerator = lines.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Windows.UI.Xaml.Shapes.Line item = enumerator.Current;
                        base.TransFormLine(item, avilableSize);
                        double num1 = item.X1;
                        double num3 = item.Y1;
                        double width = Math.Abs((double)(item.X2 - item.X1));
                        double height = Math.Abs((double)(item.Y2 - item.Y1));
                        if ((width >= 0.0) && (height >= 0.0))
                        {
                            gs.SaveState();
                            gs.MoveTo(new Windows.Foundation.Point(item.X1, item.Y1));
                            gs.LineTo(new Windows.Foundation.Point(item.X2, item.Y2));
                            gs.ApplyFillEffect(item.Stroke, new Windows.Foundation.Rect(item.X1, item.Y1, width, height), true, false);
                            gs.SetLineWidth(item.StrokeThickness);
                            gs.Stroke();
                            gs.RestoreState();
                        }
                    }
                }
            }
            List<DrawRectInfo> drawRects = base.GetDrawRects(avilableSize);
            if (drawRects != null)
            {
                foreach (DrawRectInfo info in drawRects)
                {
                    if (((info.brush != null) && (info.rect.Height > 0.0)) && (info.rect.Height > 0.0))
                    {
                        gs.SaveState();
                        Windows.Foundation.Rect rect = base.TransFormRect(info.rect, avilableSize);
                        gs.Translate(rect.Left + (rect.Width / 2.0), rect.Top + (rect.Height / 2.0));
                        gs.Rotate(-45.0);
                        rect = new Windows.Foundation.Rect(-rect.Width / 2.0, -rect.Height / 2.0, rect.Width, rect.Height);
                        gs.FillRectangle(rect, info.brush);
                        gs.RestoreState();
                    }
                }
            }
        }

        internal override double BottomSpace
        {
            get { return  ((base.BottomSpace + this.GetLineWeight()) + 1.0); }
        }

        internal override double LeftSpace
        {
            get { return  ((base.LeftSpace + this.GetLineWeight()) + 1.0); }
        }

        internal override double RightSpace
        {
            get { return  ((base.RightSpace + this.GetLineWeight()) + 1.0); }
        }

        internal override double TopSpace
        {
            get { return  ((base.TopSpace + this.GetLineWeight()) + 1.0); }
        }
    }
}

