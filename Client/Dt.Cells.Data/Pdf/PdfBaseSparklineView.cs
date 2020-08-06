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
    /// Represents the base sparkline view class.
    /// </summary>
    internal abstract class PdfBaseSparklineView : IThemeContextSupport
    {
        BaseSparklineViewInfo _viewInfo;

        internal PdfBaseSparklineView(BaseSparklineViewInfo viewInfo)
        {
            this._viewInfo = viewInfo;
            this.Init();
        }

        internal virtual bool DrawDataPointsBeyondMaxAndMin()
        {
            return true;
        }

        internal Windows.UI.Xaml.Shapes.Line GetAxisLine(Windows.Foundation.Size availableSize)
        {
            this._viewInfo.ZoomFactor = 1.0;
            this._viewInfo.MeasureAxis(availableSize);
            return this._viewInfo.AxisLine;
        }

        internal Windows.Foundation.Rect? GetClipBounds(Windows.Foundation.Size finalSize)
        {
            return this._viewInfo.GetClipBounds(finalSize);
        }

        internal List<DrawRectInfo> GetDrawRects(Windows.Foundation.Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                return null;
            }
            List<DrawRectInfo> list = new List<DrawRectInfo>();
            double actualMinValue = this._viewInfo.GetActualMinValue();
            double actualMaxValue = this._viewInfo.GetActualMaxValue();
            foreach (int num4 in this._viewInfo.CachedIndexMaping)
            {
                Brush brush;
                double? cachedValue = this._viewInfo.GetCachedValue(num4);
                if (cachedValue.HasValue)
                {
                    double? nullable2 = cachedValue;
                    double num5 = actualMinValue;
                    if (!((((double) nullable2.GetValueOrDefault()) < num5) && nullable2.HasValue))
                    {
                        double? nullable3 = cachedValue;
                        double num6 = actualMaxValue;
                        if (!((((double) nullable3.GetValueOrDefault()) > num6) && nullable3.HasValue))
                        {
                            goto Label_00C0;
                        }
                    }
                    if (!this.DrawDataPointsBeyondMaxAndMin())
                    {
                        continue;
                    }
                }
            Label_00C0:
                brush = this.GetFillBrush(num4, availableSize);
                Windows.Foundation.Rect dataPointPosition = this._viewInfo.GetDataPointPosition(num4, availableSize);
                DrawRectInfo info = new DrawRectInfo {
                    brush = brush,
                    rect = dataPointPosition
                };
                list.Add(info);
            }
            return list;
        }

        internal Brush GetFillBrush(int index, Windows.Foundation.Size availableSize)
        {
            return new SolidColorBrush(this._viewInfo.GetDataPointColor(index));
        }

        /// <summary>
        /// Gets the theme context.
        /// </summary>
        /// <returns>The theme context.</returns>
        IThemeSupport IThemeContextSupport.GetContext()
        {
            return ((IThemeContextSupport) this._viewInfo).GetContext();
        }

        /// <summary>
        /// Sets the theme context.
        /// </summary>
        /// <param name="context">The theme context.</param>
        void IThemeContextSupport.SetContext(IThemeSupport context)
        {
            ((IThemeContextSupport) this._viewInfo).SetContext(context);
        }

        internal void Init()
        {
            this._viewInfo.Reset();
        }

        internal virtual void Paint(Graphics gs, Windows.Foundation.Size avilableSize)
        {
            List<DrawRectInfo> drawRects = this.GetDrawRects(avilableSize);
            if (drawRects != null)
            {
                foreach (DrawRectInfo info in drawRects)
                {
                    if (info.brush != null)
                    {
                        Windows.Foundation.Rect rect = info.rect;
                        Windows.Foundation.Rect rect2 = this.TransFormRect(new Windows.Foundation.Rect(rect.X + (rect.Width / 4.0), rect.Y, rect.Width / 2.0, rect.Height), avilableSize);
                        gs.SaveState();
                        gs.FillRectangle(rect2, info.brush);
                        gs.RestoreState();
                    }
                }
            }
            Windows.UI.Xaml.Shapes.Line axisLine = this.GetAxisLine(avilableSize);
            if (axisLine != null)
            {
                double num1 = axisLine.X1;
                double num3 = axisLine.Y1;
                double width = Math.Abs((double) (axisLine.X2 - axisLine.X1));
                double height = Math.Abs((double) (axisLine.Y2 - axisLine.Y1));
                if ((width >= 0.0) && (height >= 0.0))
                {
                    gs.SaveState();
                    gs.MoveTo(new Windows.Foundation.Point(axisLine.X1, axisLine.Y1));
                    gs.LineTo(new Windows.Foundation.Point(axisLine.X2, axisLine.Y2));
                    gs.ApplyFillEffect(axisLine.Stroke, new Windows.Foundation.Rect(axisLine.X1, axisLine.Y1, width, height), true, false);
                    gs.SetLineWidth(axisLine.StrokeThickness);
                    gs.Stroke();
                    gs.RestoreState();
                }
            }
        }

        internal void TransFormLine(Windows.UI.Xaml.Shapes.Line line, Windows.Foundation.Size availableSize)
        {
            if (this.SparklineInfo.Setting.RightToLeft && (line != null))
            {
                line.X1 = availableSize.Width - line.X1;
                line.X2 = availableSize.Width - line.X2;
            }
        }

        internal Windows.Foundation.Rect TransFormRect(Windows.Foundation.Rect rect, Windows.Foundation.Size availableSize)
        {
            Windows.Foundation.Rect rect2 = rect;
            if (this.SparklineInfo.Setting.RightToLeft)
            {
                rect2.X = (availableSize.Width - rect2.X) - rect2.Width;
            }
            return rect2;
        }

        internal virtual double BottomSpace
        {
            get { return  UnitManager.ConvertTo(3.0, UnitType.Pixel, UnitType.CentileInch, 96f); }
        }

        internal List<DateTime?> CachedDatetimes
        {
            get { return  this._viewInfo.CachedDatetimes; }
        }

        internal List<int> CachedIndexMaping
        {
            get { return  this._viewInfo.CachedIndexMaping; }
        }

        internal List<double?> CachedValues
        {
            get { return  this._viewInfo.CachedValues; }
        }

        internal virtual double LeftSpace
        {
            get { return  UnitManager.ConvertTo(3.0, UnitType.Pixel, UnitType.CentileInch, 96f); }
        }

        internal virtual double RightSpace
        {
            get { return  UnitManager.ConvertTo(3.0, UnitType.Pixel, UnitType.CentileInch, 96f); }
        }

        /// <summary>
        /// Gets or sets a value that indicates the sparkline.
        /// </summary>
        public Sparkline SparklineInfo
        {
            get { return  this._viewInfo.SparklineInfo; }
            set { this._viewInfo.SparklineInfo = value; }
        }

        /// <summary>
        /// Gets the sparkline view info.
        /// </summary>
        public BaseSparklineViewInfo SparklineViewInfo
        {
            get { return  this._viewInfo; }
        }

        internal virtual double TopSpace
        {
            get { return  UnitManager.ConvertTo(3.0, UnitType.Pixel, UnitType.CentileInch, 96f); }
        }
    }
}

