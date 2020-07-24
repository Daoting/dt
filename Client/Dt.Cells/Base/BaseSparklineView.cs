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
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the base sparkline view class.
    /// </summary>
    public abstract partial class BaseSparklineView : Panel, IThemeContextSupport
    {
        private BaseSparklineViewInfo _viewInfo;
        private Windows.Foundation.Rect? _cachedClip;
        private List<UIElement> _dataPoints;

        internal BaseSparklineView(BaseSparklineViewInfo viewInfo)
        {
            this._viewInfo = viewInfo;
        }

        internal void ArrangeAxis(Windows.Foundation.Size finalSize)
        {
            if (this._viewInfo.AxisLine != null)
            {
                this._viewInfo.AxisLine.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            }
        }

        internal void ArrangeDataPoints(Windows.Foundation.Size finalSize)
        {
            if (this._dataPoints != null)
            {
                double actualMinValue = this._viewInfo.GetActualMinValue();
                double actualMaxValue = this._viewInfo.GetActualMaxValue();
                for (int i = 0; i < this._viewInfo.CachedIndexMaping.Count; i++)
                {
                    Windows.Foundation.Rect rect;
                    int valueIndexInValueCache = this._viewInfo.CachedIndexMaping[i];
                    double? cachedValue = this._viewInfo.GetCachedValue(valueIndexInValueCache);
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
                                goto Label_0099;
                            }
                        }
                        if (!this.DrawDataPointsBeyondMaxAndMin())
                        {
                            continue;
                        }
                    }
                Label_0099:
                    rect = this._viewInfo.GetDataPointPosition(valueIndexInValueCache, finalSize);
                    if ((rect.Width > 0.0) && (rect.Height > 0.0))
                    {
                        this._dataPoints[i].Arrange(rect);
                    }
                }
            }
        }

        /// <summary>
        /// Positions child elements and determines the size when overridden in a derived class.
        /// </summary>
        /// <param name="finalSize"> The final area within the parent that this element uses to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            this.SetClipToBounds(finalSize);
            this.ArrangeAxis(finalSize);
            this.ArrangeDataPoints(finalSize);
            return finalSize;
        }

        private double CalcItemWidth(Windows.Foundation.Size availableSize)
        {
            double num = DateTimeExtension.ToOADate(this._viewInfo.GetMinDatetime());
            double num2 = DateTimeExtension.ToOADate(this._viewInfo.GetMaxDatetime());
            List<double> list = new List<double>();
            for (int i = 0; i < this.CachedIndexMaping.Count; i++)
            {
                int num4 = this.CachedIndexMaping[i];
                DateTime? nullable = this.CachedDatetimes[num4];
                if ((nullable.HasValue && this.CachedDatetimesVisible[num4]) && (nullable.Value != DateTime.MinValue))
                {
                    double num5 = DateTimeExtension.ToOADate(nullable.Value);
                    list.Add(num5);
                }
            }
            list.Sort();
            if ((list.Count <= 1) || (num == num2))
            {
                return (((availableSize.Width - this.LeftSpace) - this.RightSpace) / 2.0);
            }
            double maxValue = double.MaxValue;
            double num7 = 0.0;
            for (int j = 1; j < list.Count; j++)
            {
                double num10 = list[j] - list[j - 1];
                if ((num10 < maxValue) && (num10 > 0.0))
                {
                    maxValue = num10;
                }
                num7 += num10;
            }
            double num11 = ((((availableSize.Width - this.LeftSpace) - this.RightSpace) * maxValue) / num7) / 2.0;
            if (num11 < 2.0)
            {
                num11 = 2.0;
            }
            return num11;
        }

        private List<UIElement> CreateDataPoints(Windows.Foundation.Size availableSize)
        {
            List<UIElement> list = new List<UIElement>();
            foreach (int num in this._viewInfo.CachedIndexMaping)
            {
                UIElement dataPoint = this.GetDataPoint(num, availableSize);
                Canvas.SetZIndex(dataPoint, this.DataPointZIndex);
                list.Add(dataPoint);
            }
            return list;
        }

        internal virtual bool DrawDataPointsBeyondMaxAndMin()
        {
            return true;
        }

        internal abstract UIElement GetDataPoint(int indexInValueCache, Windows.Foundation.Size availableSize);
        internal Windows.UI.Color GetDataPointColor(int indexInValueCache)
        {
            Windows.UI.Color? markerColor = null;
            double? cachedValue = this._viewInfo.GetCachedValue(indexInValueCache);
            if (cachedValue.HasValue)
            {
                if ((this._viewInfo.CahcedMinValue == double.MaxValue) || (this._viewInfo.CachedMaxValue == double.MinValue))
                {
                    this._viewInfo.GetMaxMinValue();
                }
                double cahcedMinValue = this._viewInfo.CahcedMinValue;
                double? nullable3 = cachedValue;
                double num5 = cahcedMinValue;
                if (((((double) nullable3.GetValueOrDefault()) == num5) && nullable3.HasValue) && this.SparklineInfo.Setting.ShowLow)
                {
                    markerColor = new Windows.UI.Color?(this._viewInfo.SparklineInfo.Setting.LowMarkerColor);
                }
                if (!markerColor.HasValue)
                {
                    double cachedMaxValue = this._viewInfo.CachedMaxValue;
                    double? nullable4 = cachedValue;
                    double num6 = cachedMaxValue;
                    if (((((double) nullable4.GetValueOrDefault()) == num6) && nullable4.HasValue) && this.SparklineInfo.Setting.ShowHigh)
                    {
                        markerColor = new Windows.UI.Color?(this._viewInfo.SparklineInfo.Setting.HighMarkerColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    if (this.SparklineInfo.DisplayDateAxis)
                    {
                        if ((this.CachedIndexMaping.IndexOf(indexInValueCache) == 0) && this.SparklineInfo.Setting.ShowFirst)
                        {
                            markerColor = new Windows.UI.Color?(this.SparklineInfo.Setting.FirstMarkerColor);
                        }
                    }
                    else if ((indexInValueCache == 0) && this.SparklineInfo.Setting.ShowFirst)
                    {
                        markerColor = new Windows.UI.Color?(this.SparklineInfo.Setting.FirstMarkerColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    if (this.SparklineInfo.DisplayDateAxis)
                    {
                        if ((this.CachedIndexMaping.IndexOf(indexInValueCache) == (this.CachedIndexMaping.Count - 1)) && this.SparklineInfo.Setting.ShowLast)
                        {
                            markerColor = new Windows.UI.Color?(this.SparklineInfo.Setting.LastMarkerColor);
                        }
                    }
                    else if ((indexInValueCache == (this.CachedValues.Count - 1)) && this.SparklineInfo.Setting.ShowLast)
                    {
                        markerColor = new Windows.UI.Color?(this.SparklineInfo.Setting.LastMarkerColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    double? nullable5 = cachedValue;
                    if (((((double) nullable5.GetValueOrDefault()) < 0.0) && nullable5.HasValue) && this.SparklineInfo.Setting.ShowNegative)
                    {
                        markerColor = new Windows.UI.Color?(this.SparklineInfo.Setting.NegativeColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    markerColor = this.SparklineViewInfo.GetMarkerColor();
                }
            }
            if (!markerColor.HasValue)
            {
                return Colors.Transparent;
            }
            return markerColor.Value;
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

#if IOS
        new
#endif
        internal void Init()
        {
            this._dataPoints = null;
            this._cachedClip = null;
            this._viewInfo.Reset();
        }

        internal void MeasureAxis(Windows.Foundation.Size availableSize)
        {
            this._viewInfo.MeasureAxis(availableSize);
            if (this._viewInfo.AxisLine != null)
            {
                if (!base.Children.Contains(this._viewInfo.AxisLine))
                {
                    base.Children.Add(this._viewInfo.AxisLine);
                    Canvas.SetZIndex(this._viewInfo.AxisLine, this.AxisZIndex);
                }
                this._viewInfo.AxisLine.Measure(availableSize);
            }
        }

        internal void MeasureDataPoints(Windows.Foundation.Size availableSize)
        {
            if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
            {
                if (this._dataPoints == null)
                {
                    this._dataPoints = this.CreateDataPoints(availableSize);
                    foreach (UIElement element in this._dataPoints)
                    {
                        if (element != null)
                        {
                            base.Children.Add(element);
                        }
                    }
                }
                if (this._dataPoints != null)
                {
                    foreach (UIElement element2 in this._dataPoints)
                    {
                        if (element2 != null)
                        {
                            element2.Measure(availableSize);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Measures the layout size required
        /// for child elements and determines a size for the <see cref="M:System.Windows.FrameworkElement" /> derived class when overridden in a derived class.
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
                this.MeasureDataPoints(availableSize);
                this.MeasureAxis(availableSize);
                return availableSize;
            }
            return base.MeasureOverride(availableSize);
        }

        internal void SetClipToBounds(Windows.Foundation.Size finalSize)
        {
            Windows.Foundation.Rect? clipBounds = this._viewInfo.GetClipBounds(finalSize);
            Windows.Foundation.Rect? cachedClip = this._cachedClip;
            Windows.Foundation.Rect? nullable3 = clipBounds;
            if ((cachedClip.HasValue != nullable3.HasValue) || (cachedClip.HasValue && (cachedClip.GetValueOrDefault() != nullable3.GetValueOrDefault())))
            {
                this._cachedClip = clipBounds;
                if (this._cachedClip.HasValue)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = this._cachedClip.Value;
                    base.Clip = geometry;
                }
                else
                {
                    base.ClearValue(UIElement.ClipProperty);
                }
            }
        }

        /// <summary>
        /// Occurs when overridden in a derived class and used to update the sparkline in the view.
        /// </summary>
        /// <param name="size">The update size.</param>
        /// <param name="zoomfactor">The zoom factor to update.</param>
        public virtual void Update(Windows.Foundation.Size size, double zoomfactor)
        {
            if (this._viewInfo.AxisLine != null)
            {
                base.Children.Remove(this._viewInfo.AxisLine);
            }
            if (this._dataPoints != null)
            {
                foreach (UIElement element in this._dataPoints)
                {
                    if (element != null)
                    {
                        base.Children.Remove(element);
                    }
                }
            }
            this.Init();
            this.ZoomFactor = zoomfactor;
            if (this.SparklineInfo.Setting.RightToLeft)
            {
                base.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.0);
                ScaleTransform transform = new ScaleTransform();
                transform.ScaleX = -1.0;
                base.RenderTransform = transform;
            }
            else
            {
                base.ClearValue(UIElement.RenderTransformOriginProperty);
                base.ClearValue(UIElement.RenderTransformProperty);
            }
            base.InvalidateMeasure();
        }

        internal virtual int AxisZIndex
        {
            get { return  100; }
        }

        internal double BottomSpace
        {
            get { return  this._viewInfo.BottomSpace; }
        }

        internal List<DateTime?> CachedDatetimes
        {
            get { return  this._viewInfo.CachedDatetimes; }
        }

        internal List<bool> CachedDatetimesVisible
        {
            get { return  this._viewInfo.CachedDatetimesVisible; }
        }

        internal List<int> CachedIndexMaping
        {
            get { return  this._viewInfo.CachedIndexMaping; }
        }

        internal List<double?> CachedValues
        {
            get { return  this._viewInfo.CachedValues; }
        }

        internal virtual int DataPointZIndex
        {
            get { return  90; }
        }

        internal double LeftSpace
        {
            get { return  this._viewInfo.LeftSpace; }
        }

        internal double RightSpace
        {
            get { return  this._viewInfo.RightSpace; }
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
        /// Gets the type of the sparkline.
        /// </summary>
        /// <value>
        /// The type of the sparkline.
        /// </value>
        public Dt.Cells.Data.SparklineType SparklineType
        {
            get { return  this._viewInfo.SparklineType; }
        }

        /// <summary>
        /// Gets the sparkline view info.
        /// </summary>
        public BaseSparklineViewInfo SparklineViewInfo
        {
            get { return  this._viewInfo; }
        }

        internal double TopSpace
        {
            get { return  this._viewInfo.TopSpace; }
        }

        internal double ZoomFactor
        {
            get { return  this._viewInfo.ZoomFactor; }
            set
            {
                if (this._viewInfo.ZoomFactor != value)
                {
                    this._viewInfo.ZoomFactor = value;
                    base.InvalidateMeasure();
                    base.InvalidateArrange();
                }
            }
        }
    }
}

