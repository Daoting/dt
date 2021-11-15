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
        BaseSparklineViewInfo _viewInfo;
        Rect? _cachedClip;
        List<UIElement> _dataPoints;

        internal BaseSparklineView(BaseSparklineViewInfo viewInfo)
        {
            _viewInfo = viewInfo;
        }

        internal void ArrangeAxis(Size finalSize)
        {
            if (_viewInfo.AxisLine != null)
            {
                _viewInfo.AxisLine.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            }
        }

        internal void ArrangeDataPoints(Size finalSize)
        {
            if (_dataPoints != null)
            {
                double actualMinValue = _viewInfo.GetActualMinValue();
                double actualMaxValue = _viewInfo.GetActualMaxValue();
                for (int i = 0; i < _viewInfo.CachedIndexMaping.Count; i++)
                {
                    Rect rect;
                    int valueIndexInValueCache = _viewInfo.CachedIndexMaping[i];
                    double? cachedValue = _viewInfo.GetCachedValue(valueIndexInValueCache);
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
                        if (!DrawDataPointsBeyondMaxAndMin())
                        {
                            continue;
                        }
                    }
                Label_0099:
                    rect = _viewInfo.GetDataPointPosition(valueIndexInValueCache, finalSize);
                    if ((rect.Width > 0.0) && (rect.Height > 0.0))
                    {
                        _dataPoints[i].Arrange(rect);
                    }
                }
            }
        }

        /// <summary>
        /// Positions child elements and determines the size when overridden in a derived class.
        /// </summary>
        /// <param name="finalSize"> The final area within the parent that this element uses to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            SetClipToBounds(finalSize);
            ArrangeAxis(finalSize);
            ArrangeDataPoints(finalSize);
            return finalSize;
        }

        double CalcItemWidth(Size availableSize)
        {
            double num = DateTimeExtension.ToOADate(_viewInfo.GetMinDatetime());
            double num2 = DateTimeExtension.ToOADate(_viewInfo.GetMaxDatetime());
            List<double> list = new List<double>();
            for (int i = 0; i < CachedIndexMaping.Count; i++)
            {
                int num4 = CachedIndexMaping[i];
                DateTime? nullable = CachedDatetimes[num4];
                if ((nullable.HasValue && CachedDatetimesVisible[num4]) && (nullable.Value != DateTime.MinValue))
                {
                    double num5 = DateTimeExtension.ToOADate(nullable.Value);
                    list.Add(num5);
                }
            }
            list.Sort();
            if ((list.Count <= 1) || (num == num2))
            {
                return (((availableSize.Width - LeftSpace) - RightSpace) / 2.0);
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
            double num11 = ((((availableSize.Width - LeftSpace) - RightSpace) * maxValue) / num7) / 2.0;
            if (num11 < 2.0)
            {
                num11 = 2.0;
            }
            return num11;
        }

        List<UIElement> CreateDataPoints(Size availableSize)
        {
            List<UIElement> list = new List<UIElement>();
            foreach (int num in _viewInfo.CachedIndexMaping)
            {
                UIElement dataPoint = GetDataPoint(num, availableSize);
                Canvas.SetZIndex(dataPoint, DataPointZIndex);
                list.Add(dataPoint);
            }
            return list;
        }

        internal virtual bool DrawDataPointsBeyondMaxAndMin()
        {
            return true;
        }

        internal abstract UIElement GetDataPoint(int indexInValueCache, Size availableSize);
        internal Windows.UI.Color GetDataPointColor(int indexInValueCache)
        {
            Windows.UI.Color? markerColor = null;
            double? cachedValue = _viewInfo.GetCachedValue(indexInValueCache);
            if (cachedValue.HasValue)
            {
                if ((_viewInfo.CahcedMinValue == double.MaxValue) || (_viewInfo.CachedMaxValue == double.MinValue))
                {
                    _viewInfo.GetMaxMinValue();
                }
                double cahcedMinValue = _viewInfo.CahcedMinValue;
                double? nullable3 = cachedValue;
                double num5 = cahcedMinValue;
                if (((((double) nullable3.GetValueOrDefault()) == num5) && nullable3.HasValue) && SparklineInfo.Setting.ShowLow)
                {
                    markerColor = new Windows.UI.Color?(_viewInfo.SparklineInfo.Setting.LowMarkerColor);
                }
                if (!markerColor.HasValue)
                {
                    double cachedMaxValue = _viewInfo.CachedMaxValue;
                    double? nullable4 = cachedValue;
                    double num6 = cachedMaxValue;
                    if (((((double) nullable4.GetValueOrDefault()) == num6) && nullable4.HasValue) && SparklineInfo.Setting.ShowHigh)
                    {
                        markerColor = new Windows.UI.Color?(_viewInfo.SparklineInfo.Setting.HighMarkerColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    if (SparklineInfo.DisplayDateAxis)
                    {
                        if ((CachedIndexMaping.IndexOf(indexInValueCache) == 0) && SparklineInfo.Setting.ShowFirst)
                        {
                            markerColor = new Windows.UI.Color?(SparklineInfo.Setting.FirstMarkerColor);
                        }
                    }
                    else if ((indexInValueCache == 0) && SparklineInfo.Setting.ShowFirst)
                    {
                        markerColor = new Windows.UI.Color?(SparklineInfo.Setting.FirstMarkerColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    if (SparklineInfo.DisplayDateAxis)
                    {
                        if ((CachedIndexMaping.IndexOf(indexInValueCache) == (CachedIndexMaping.Count - 1)) && SparklineInfo.Setting.ShowLast)
                        {
                            markerColor = new Windows.UI.Color?(SparklineInfo.Setting.LastMarkerColor);
                        }
                    }
                    else if ((indexInValueCache == (CachedValues.Count - 1)) && SparklineInfo.Setting.ShowLast)
                    {
                        markerColor = new Windows.UI.Color?(SparklineInfo.Setting.LastMarkerColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    double? nullable5 = cachedValue;
                    if (((((double) nullable5.GetValueOrDefault()) < 0.0) && nullable5.HasValue) && SparklineInfo.Setting.ShowNegative)
                    {
                        markerColor = new Windows.UI.Color?(SparklineInfo.Setting.NegativeColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    markerColor = SparklineViewInfo.GetMarkerColor();
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
            return ((IThemeContextSupport) _viewInfo).GetContext();
        }

        /// <summary>
        /// Sets the theme context.
        /// </summary>
        /// <param name="context">The theme context.</param>
        void IThemeContextSupport.SetContext(IThemeSupport context)
        {
            ((IThemeContextSupport) _viewInfo).SetContext(context);
        }

#if IOS
        new
#endif
        internal void Init()
        {
            _dataPoints = null;
            _cachedClip = null;
            _viewInfo.Reset();
        }

        internal void MeasureAxis(Size availableSize)
        {
            _viewInfo.MeasureAxis(availableSize);
            if (_viewInfo.AxisLine != null)
            {
                if (!base.Children.Contains(_viewInfo.AxisLine))
                {
                    base.Children.Add(_viewInfo.AxisLine);
                    Canvas.SetZIndex(_viewInfo.AxisLine, AxisZIndex);
                }
                _viewInfo.AxisLine.Measure(availableSize);
            }
        }

        internal void MeasureDataPoints(Size availableSize)
        {
            if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
            {
                if (_dataPoints == null)
                {
                    _dataPoints = CreateDataPoints(availableSize);
                    foreach (UIElement element in _dataPoints)
                    {
                        if (element != null)
                        {
                            base.Children.Add(element);
                        }
                    }
                }
                if (_dataPoints != null)
                {
                    foreach (UIElement element2 in _dataPoints)
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
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
            {
                MeasureDataPoints(availableSize);
                MeasureAxis(availableSize);
                return availableSize;
            }
            return base.MeasureOverride(availableSize);
        }

        internal void SetClipToBounds(Size finalSize)
        {
            Rect? clipBounds = _viewInfo.GetClipBounds(finalSize);
            Rect? cachedClip = _cachedClip;
            Rect? nullable3 = clipBounds;
            if ((cachedClip.HasValue != nullable3.HasValue) || (cachedClip.HasValue && (cachedClip.GetValueOrDefault() != nullable3.GetValueOrDefault())))
            {
                _cachedClip = clipBounds;
                if (_cachedClip.HasValue)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = _cachedClip.Value;
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
        public virtual void Update(Size size, double zoomfactor)
        {
            if (_viewInfo.AxisLine != null)
            {
                base.Children.Remove(_viewInfo.AxisLine);
            }
            if (_dataPoints != null)
            {
                foreach (UIElement element in _dataPoints)
                {
                    if (element != null)
                    {
                        base.Children.Remove(element);
                    }
                }
            }
            Init();
            ZoomFactor = zoomfactor;
            if (SparklineInfo.Setting.RightToLeft)
            {
                base.RenderTransformOrigin = new Point(0.5, 0.0);
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
            get { return  _viewInfo.BottomSpace; }
        }

        internal List<DateTime?> CachedDatetimes
        {
            get { return  _viewInfo.CachedDatetimes; }
        }

        internal List<bool> CachedDatetimesVisible
        {
            get { return  _viewInfo.CachedDatetimesVisible; }
        }

        internal List<int> CachedIndexMaping
        {
            get { return  _viewInfo.CachedIndexMaping; }
        }

        internal List<double?> CachedValues
        {
            get { return  _viewInfo.CachedValues; }
        }

        internal virtual int DataPointZIndex
        {
            get { return  90; }
        }

        internal double LeftSpace
        {
            get { return  _viewInfo.LeftSpace; }
        }

        internal double RightSpace
        {
            get { return  _viewInfo.RightSpace; }
        }

        /// <summary>
        /// Gets or sets a value that indicates the sparkline.
        /// </summary>
        public Sparkline SparklineInfo
        {
            get { return  _viewInfo.SparklineInfo; }
            set { _viewInfo.SparklineInfo = value; }
        }

        /// <summary>
        /// Gets the type of the sparkline.
        /// </summary>
        /// <value>
        /// The type of the sparkline.
        /// </value>
        public Dt.Cells.Data.SparklineType SparklineType
        {
            get { return  _viewInfo.SparklineType; }
        }

        /// <summary>
        /// Gets the sparkline view info.
        /// </summary>
        public BaseSparklineViewInfo SparklineViewInfo
        {
            get { return  _viewInfo; }
        }

        internal double TopSpace
        {
            get { return  _viewInfo.TopSpace; }
        }

        internal double ZoomFactor
        {
            get { return  _viewInfo.ZoomFactor; }
            set
            {
                if (_viewInfo.ZoomFactor != value)
                {
                    _viewInfo.ZoomFactor = value;
                    base.InvalidateMeasure();
                    base.InvalidateArrange();
                }
            }
        }
    }
}

