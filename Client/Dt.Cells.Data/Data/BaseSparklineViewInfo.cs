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
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the base sparkline view information class.
    /// </summary>
    public abstract class BaseSparklineViewInfo : IThemeContextSupport
    {
        double _zoomfactor = 1.0;
        Windows.UI.Xaml.Shapes.Line axisLine;
        List<DateTime?> cachedDatetimes;
        List<bool> cachedDatetimeVisible;
        List<int> cachedIndexMapping;
        DateTime cachedMaxDatetime = DateTime.MinValue;
        double cachedMaxValue = double.MinValue;
        DateTime cachedMinDatetime = DateTime.MaxValue;
        List<double?> cachedValues;
        double cahcedMinValue = double.MaxValue;
        Sparkline info;

        /// <summary>
        /// Creates a new instance of  the base sparkline view information.
        /// </summary>
        /// <param name="info">The sparkline information.</param>
        public BaseSparklineViewInfo(Sparkline info)
        {
            this.info = info;
            this.Reset();
        }

        double CalcItemWidth(Windows.Foundation.Size availableSize)
        {
            double num = this.GetMinDatetime().ToOADate();
            double num2 = this.GetMaxDatetime().ToOADate();
            List<double> list = new List<double>();
            for (int i = 0; i < this.CachedIndexMaping.Count; i++)
            {
                int num4 = this.CachedIndexMaping[i];
                DateTime? nullable = this.CachedDatetimes[num4];
                if (nullable.HasValue && this.CachedDatetimesVisible[num4] && (nullable.Value != DateTime.MinValue))
                {
                    double num5 = nullable.Value.ToOADate();
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

        /// <summary>
        /// Creates the axis line.
        /// </summary>
        /// <param name="avalibleSize">The available size.</param>
        /// <returns></returns>
        public Windows.UI.Xaml.Shapes.Line CreateAxisLine(Windows.Foundation.Size avalibleSize)
        {
            if (this.SparklineInfo.Setting.DisplayXAxis && this.HasAxis)
            {
                Windows.UI.Xaml.Shapes.Line line = new Windows.UI.Xaml.Shapes.Line();
                line.X1 = this.LeftSpace;
                line.X2 = avalibleSize.Width - this.RightSpace;
                line.Y1 = this.GetAxisY(avalibleSize);
                line.Y2 = line.Y1;
                line.Stroke = new SolidColorBrush(this.SparklineInfo.Setting.AxisColor);
                line.StrokeThickness = 1.0;
                Canvas.SetZIndex(line, this.AxisZIndex);
                return line;
            }
            return null;
        }

        /// <summary>
        /// Gets the actual maximum value.
        /// </summary>
        /// <returns></returns>
        public double GetActualMaxValue()
        {
            if (this.cachedMaxValue == double.MinValue)
            {
                this.GetMaxMinValue();
            }
            switch (this.info.Setting.MaxAxisType)
            {
                case SparklineAxisMinMax.Individual:
                    return this.cachedMaxValue;

                case SparklineAxisMinMax.Group:
                    return this.info.Setting.GetGroupMaxValue();

                case SparklineAxisMinMax.Custom:
                    return this.info.Setting.ManualMax;
            }
            return this.cachedMaxValue;
        }

        /// <summary>
        /// Gets the actual minimum value.
        /// </summary>
        /// <returns></returns>
        public double GetActualMinValue()
        {
            if (this.cahcedMinValue == double.MaxValue)
            {
                this.GetMaxMinValue();
            }
            switch (this.info.Setting.MinAxisType)
            {
                case SparklineAxisMinMax.Individual:
                    return this.cahcedMinValue;

                case SparklineAxisMinMax.Group:
                    return this.info.Setting.GetGroupMinValue();

                case SparklineAxisMinMax.Custom:
                    return this.info.Setting.ManualMin;
            }
            return this.cahcedMinValue;
        }

        /// <summary>
        /// Gets the y-axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns></returns>
        public virtual double GetAxisY(Windows.Foundation.Size availableSize)
        {
            Windows.Foundation.Size canvasSize = this.GetCanvasSize(availableSize);
            double actualMaxValue = this.GetActualMaxValue();
            double actualMinValue = this.GetActualMinValue();
            if ((actualMaxValue == double.MinValue) && (actualMinValue == double.MaxValue))
            {
                return (availableSize.Height / 2.0);
            }
            double num3 = actualMaxValue - actualMinValue;
            if (actualMaxValue == actualMinValue)
            {
                if (actualMaxValue == 0.0)
                {
                    return (availableSize.Height / 2.0);
                }
                num3 = actualMaxValue;
                if (actualMaxValue < 0.0)
                {
                    actualMaxValue = 0.0;
                }
            }
            double num4 = canvasSize.Height / num3;
            return (this.TopSpace + (actualMaxValue * num4));
        }

        /// <summary>
        /// Gets the cached value.
        /// </summary>
        /// <param name="valueIndexInValueCache">The index in the cache.</param>
        /// <returns></returns>
        public virtual double? GetCachedValue(int valueIndexInValueCache)
        {
            double? nullable = this.CachedValues[valueIndexInValueCache];
            if (!nullable.HasValue && (this.SparklineInfo.Setting.DisplayEmptyCellsAs == EmptyValueStyle.Zero))
            {
                nullable = 0.0;
            }
            return nullable;
        }

        /// <summary>
        /// Gets the size of the canvas.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns></returns>
        public Windows.Foundation.Size GetCanvasSize(Windows.Foundation.Size availableSize)
        {
            double num = (availableSize.Width - this.LeftSpace) - this.RightSpace;
            num = Math.Max(num, 0.0);
            double num2 = (availableSize.Height - this.TopSpace) - this.BottomSpace;
            return new Windows.Foundation.Size(num, Math.Max(num2, 0.0));
        }

        /// <summary>
        /// Gets the clip boundary.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        /// <returns></returns>
        public virtual Windows.Foundation.Rect? GetClipBounds(Windows.Foundation.Size finalSize)
        {
            double width = (finalSize.Width - this.LeftSpace) - this.RightSpace;
            double height = (finalSize.Height - this.TopSpace) - this.BottomSpace;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Windows.Foundation.Rect(this.LeftSpace, this.TopSpace, width, height);
            }
            return null;
        }

        /// <summary>
        /// Gets the color of the data point.
        /// </summary>
        /// <param name="indexInValueCache">The index in the cache.</param>
        /// <returns></returns>
        public Windows.UI.Color GetDataPointColor(int indexInValueCache)
        {
            Windows.UI.Color? markerColor = null;
            double? cachedValue = this.GetCachedValue(indexInValueCache);
            if (cachedValue.HasValue)
            {
                if ((this.cahcedMinValue == double.MaxValue) || (this.cachedMaxValue == double.MinValue))
                {
                    this.GetMaxMinValue();
                }
                double cahcedMinValue = this.cahcedMinValue;
                double? nullable3 = cachedValue;
                double num5 = cahcedMinValue;
                if (((((double) nullable3.GetValueOrDefault()) == num5) && nullable3.HasValue) && this.SparklineInfo.Setting.ShowLow)
                {
                    markerColor = new Windows.UI.Color?(this.info.Setting.LowMarkerColor);
                }
                if (!markerColor.HasValue)
                {
                    double cachedMaxValue = this.cachedMaxValue;
                    double? nullable4 = cachedValue;
                    double num6 = cachedMaxValue;
                    if (((((double) nullable4.GetValueOrDefault()) == num6) && nullable4.HasValue) && this.SparklineInfo.Setting.ShowHigh)
                    {
                        markerColor = new Windows.UI.Color?(this.info.Setting.HighMarkerColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    if (this.SparklineInfo.DisplayDateAxis)
                    {
                        if ((this.CachedIndexMaping.IndexOf(indexInValueCache) == 0) && this.SparklineInfo.Setting.ShowFirst)
                        {
                            markerColor = new Windows.UI.Color?(this.info.Setting.FirstMarkerColor);
                        }
                    }
                    else if ((indexInValueCache == 0) && this.SparklineInfo.Setting.ShowFirst)
                    {
                        markerColor = new Windows.UI.Color?(this.info.Setting.FirstMarkerColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    if (this.SparklineInfo.DisplayDateAxis)
                    {
                        if ((this.CachedIndexMaping.IndexOf(indexInValueCache) == (this.CachedIndexMaping.Count - 1)) && this.SparklineInfo.Setting.ShowLast)
                        {
                            markerColor = new Windows.UI.Color?(this.info.Setting.LastMarkerColor);
                        }
                    }
                    else if ((indexInValueCache == (this.CachedValues.Count - 1)) && this.SparklineInfo.Setting.ShowLast)
                    {
                        markerColor = new Windows.UI.Color?(this.info.Setting.LastMarkerColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    double? nullable5 = cachedValue;
                    if (((((double) nullable5.GetValueOrDefault()) < 0.0) && nullable5.HasValue) && this.SparklineInfo.Setting.ShowNegative)
                    {
                        markerColor = new Windows.UI.Color?(this.info.Setting.NegativeColor);
                    }
                }
                if (!markerColor.HasValue)
                {
                    markerColor = this.GetMarkerColor();
                }
            }
            if (!markerColor.HasValue)
            {
                return Colors.Transparent;
            }
            return markerColor.Value;
        }

        /// <summary>
        /// Gets the data point position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="availableSize">The available size.</param>
        /// <returns></returns>
        public virtual Windows.Foundation.Rect GetDataPointPosition(int index, Windows.Foundation.Size availableSize)
        {
            double itemWidth = this.GetItemWidth(availableSize);
            double itemX = this.GetItemX(availableSize, index);
            if (itemWidth < 0.0)
            {
                itemWidth = 0.0;
            }
            itemWidth = Math.Floor(itemWidth);
            if ((itemWidth % 2.0) == 1.0)
            {
                itemWidth++;
            }
            double itemHeight = this.GetItemHeight(availableSize, index);
            double axisY = this.GetAxisY(availableSize);
            double actualMaxValue = this.GetActualMaxValue();
            double actualMinValue = this.GetActualMinValue();
            double y = 0.0;
            if ((actualMaxValue < 0.0) && (actualMinValue < 0.0))
            {
                y = Math.Max(this.TopSpace, axisY);
            }
            else
            {
                y = axisY;
                if (itemHeight >= 0.0)
                {
                    y = axisY - itemHeight;
                }
            }
            double itemVisibleHeight = this.GetItemVisibleHeight(availableSize, index);
            Windows.Foundation.Rect rect = new Windows.Foundation.Rect(itemX, y, itemWidth, Math.Abs(itemVisibleHeight));
            if (itemHeight != 0.0)
            {
                if ((rect.Y < this.TopSpace) && (rect.Bottom < (this.TopSpace + 1.0)))
                {
                    rect.Height = Math.Floor((double) (rect.Height + 1.0));
                    return rect;
                }
                double num9 = availableSize.Height - this.BottomSpace;
                if ((rect.Bottom > num9) && (rect.Y > (num9 - 1.0)))
                {
                    rect.Y--;
                    rect.Height++;
                }
            }
            return rect;
        }

        /// <summary>
        /// Gets the height of the item.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public virtual double GetItemHeight(Windows.Foundation.Size availableSize, int index)
        {
            Windows.Foundation.Size canvasSize = this.GetCanvasSize(availableSize);
            double actualMaxValue = this.GetActualMaxValue();
            double actualMinValue = this.GetActualMinValue();
            double num3 = actualMaxValue - actualMinValue;
            if (actualMaxValue == actualMinValue)
            {
                if (actualMaxValue == 0.0)
                {
                    return 0.0;
                }
                num3 = Math.Abs(actualMaxValue);
            }
            double? cachedValue = this.GetCachedValue(index);
            double num4 = cachedValue.HasValue ? ((double) cachedValue.GetValueOrDefault()) : 0.0;
            double num5 = canvasSize.Height / num3;
            return (num4 * num5);
        }

        /// <summary>
        /// Gets the height of the visible item.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public virtual double GetItemVisibleHeight(Windows.Foundation.Size availableSize, int index)
        {
            Windows.Foundation.Size canvasSize = this.GetCanvasSize(availableSize);
            double actualMaxValue = this.GetActualMaxValue();
            double actualMinValue = this.GetActualMinValue();
            double num3 = actualMaxValue - actualMinValue;
            if (actualMaxValue == actualMinValue)
            {
                if (actualMaxValue == 0.0)
                {
                    return 0.0;
                }
                num3 = actualMaxValue;
            }
            double num4 = canvasSize.Height / num3;
            double? cachedValue = this.GetCachedValue(index);
            double num5 = cachedValue.HasValue ? ((double) cachedValue.GetValueOrDefault()) : 0.0;
            if ((actualMaxValue == actualMinValue) || ((actualMaxValue * actualMinValue) <= 0.0))
            {
                return (num5 * num4);
            }
            if (num5 >= 0.0)
            {
                return ((num5 - actualMinValue) * num4);
            }
            return ((num5 - actualMaxValue) * num4);
        }

        /// <summary>
        /// Gets the width of the item.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns></returns>
        public virtual double GetItemWidth(Windows.Foundation.Size availableSize)
        {
            if (this.SparklineInfo.DisplayDateAxis)
            {
                return this.CalcItemWidth(availableSize);
            }
            int num = this.CachedIndexMaping.Count;
            return (((availableSize.Width - this.LeftSpace) - this.RightSpace) / ((double) num));
        }

        /// <summary>
        /// Gets the x-item.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public double GetItemX(Windows.Foundation.Size availableSize, int index)
        {
            if (this.SparklineInfo.DisplayDateAxis)
            {
                double num = this.GetItemWidth(availableSize);
                DateTime maxDatetime = this.GetMaxDatetime();
                DateTime minDatetime = this.GetMinDatetime();
                if (maxDatetime == minDatetime)
                {
                    return (this.LeftSpace + (num / 2.0));
                }
                DateTime? nullable = this.CachedDatetimes[index];
                if (!nullable.HasValue)
                {
                    return 0.0;
                }
                double num2 = (availableSize.Width - this.LeftSpace) - this.RightSpace;
                num2 -= num;
                TimeSpan span = (TimeSpan) (maxDatetime - minDatetime);
                double totalDays = span.TotalDays;
                TimeSpan span2 = nullable.Value - minDatetime;
                return (this.LeftSpace + Math.Floor((double) ((span2.TotalDays / totalDays) * num2)));
            }
            double itemWidth = this.GetItemWidth(availableSize);
            int num5 = this.CachedIndexMaping.IndexOf(index);
            double d = this.LeftSpace + (itemWidth * num5);
            return Math.Floor(d);
        }

        /// <summary>
        /// Gets the color of the marker.
        /// </summary>
        /// <returns></returns>
        public abstract Windows.UI.Color? GetMarkerColor();
        /// <summary>
        /// Gets the maximum datetime.
        /// </summary>
        /// <returns></returns>
        public DateTime GetMaxDatetime()
        {
            if (this.cachedMaxDatetime == DateTime.MinValue)
            {
                this.GetMaxMinDatetimes();
            }
            return this.cachedMaxDatetime;
        }

        void GetMaxMinDatetimes()
        {
            for (int i = 0; i < this.CachedIndexMaping.Count; i++)
            {
                int valueIndexInValueCache = this.CachedIndexMaping[i];
                DateTime? nullable = this.CachedDatetimes[valueIndexInValueCache];
                if (this.cachedDatetimeVisible[valueIndexInValueCache])
                {
                    double? cachedValue = this.GetCachedValue(valueIndexInValueCache);
                    if (!cachedValue.HasValue || !double.IsNaN(cachedValue.Value))
                    {
                        DateTime? nullable3 = nullable;
                        DateTime cachedMaxDatetime = this.cachedMaxDatetime;
                        if (nullable3.HasValue ? (nullable3.GetValueOrDefault() > cachedMaxDatetime) : false)
                        {
                            this.cachedMaxDatetime = nullable.Value;
                        }
                        DateTime? nullable4 = nullable;
                        DateTime cachedMinDatetime = this.cachedMinDatetime;
                        if (nullable4.HasValue ? (nullable4.GetValueOrDefault() < cachedMinDatetime) : false)
                        {
                            this.cachedMinDatetime = nullable.Value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the maximum and minimum value.
        /// </summary>
        public void GetMaxMinValue()
        {
            for (int i = 0; i < this.CachedValues.Count; i++)
            {
                double? cachedValue = this.GetCachedValue(i);
                if ((!cachedValue.HasValue || !double.IsNaN(cachedValue.Value)) && cachedValue.HasValue)
                {
                    double? nullable2 = cachedValue;
                    double cahcedMinValue = this.cahcedMinValue;
                    if ((((double) nullable2.GetValueOrDefault()) < cahcedMinValue) && nullable2.HasValue)
                    {
                        this.cahcedMinValue = cachedValue.Value;
                    }
                    double? nullable3 = cachedValue;
                    double cachedMaxValue = this.cachedMaxValue;
                    if ((((double) nullable3.GetValueOrDefault()) > cachedMaxValue) && nullable3.HasValue)
                    {
                        this.cachedMaxValue = cachedValue.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the minimum datetime.
        /// </summary>
        /// <returns></returns>
        public DateTime GetMinDatetime()
        {
            if (this.cachedMinDatetime == DateTime.MaxValue)
            {
                this.GetMaxMinDatetimes();
            }
            return this.cachedMinDatetime;
        }

        /// <summary>
        /// Gets the theme context.
        /// </summary>
        /// <returns>The theme context.</returns>
        IThemeSupport IThemeContextSupport.GetContext()
        {
            return ((IThemeContextSupport) this.info).GetContext();
        }

        /// <summary>
        /// Sets the theme context.
        /// </summary>
        /// <param name="context">The theme context.</param>
        void IThemeContextSupport.SetContext(IThemeSupport context)
        {
            ((IThemeContextSupport) this.info).SetContext(context);
        }

        /// <summary>
        /// Measures the axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        public void MeasureAxis(Windows.Foundation.Size availableSize)
        {
            if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
            {
                if (this.axisLine == null)
                {
                    this.axisLine = this.CreateAxisLine(availableSize);
                }
                if (this.axisLine != null)
                {
                    this.axisLine.X1 = this.LeftSpace;
                    this.axisLine.X2 = availableSize.Width - this.RightSpace;
                    double axisY = this.GetAxisY(availableSize);
                    this.axisLine.Y1 = Math.Floor(axisY) + 0.5;
                    double actualMinValue = this.GetActualMinValue();
                    double actualMaxValue = this.GetActualMaxValue();
                    if ((actualMaxValue == actualMinValue) && (actualMaxValue >= 0.0))
                    {
                        this.axisLine.Y1 = Math.Floor(axisY) - 0.5;
                    }
                    this.axisLine.Y2 = this.axisLine.Y1;
                    double zoomFactor = this.ZoomFactor;
                    if (zoomFactor < 1.0)
                    {
                        zoomFactor = 1.0;
                    }
                    this.axisLine.StrokeThickness = zoomFactor;
                }
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.cachedMaxValue = double.MinValue;
            this.cahcedMinValue = double.MaxValue;
            this.cachedValues = null;
            this.cachedDatetimes = null;
            this.cachedIndexMapping = null;
            this.cachedMaxDatetime = DateTime.MinValue;
            this.cachedMinDatetime = DateTime.MaxValue;
            this.axisLine = null;
            this._zoomfactor = 1.0;
        }

        /// <summary>
        /// Gets the axis line.
        /// </summary>
        public Windows.UI.Xaml.Shapes.Line AxisLine
        {
            get { return  this.axisLine; }
        }

        /// <summary>
        /// Gets a value that indicates the axis z-index.
        /// </summary>
        public virtual int AxisZIndex
        {
            get { return  100; }
        }

        /// <summary>
        /// Gets the bottom space.
        /// </summary>
        public virtual double BottomSpace
        {
            get { return  3.0; }
        }

        /// <summary>
        /// Gets the cached dates and times.
        /// </summary>
        public List<DateTime?> CachedDatetimes
        {
            get
            {
                if (this.cachedDatetimes == null)
                {
                    this.cachedDatetimeVisible = new List<bool>();
                    this.cachedDatetimes = new List<DateTime?>();
                    int count = this.SparklineInfo.DateAxisData.Count;
                    for (int i = 0; i < count; i++)
                    {
                        object obj2 = this.SparklineInfo.DateAxisData.GetValue(i);
                        if (obj2 != null)
                        {
                            if (double.IsNaN((double) ((double) obj2)))
                            {
                                this.cachedDatetimes.Add(new DateTime?(DateTime.MinValue));
                                this.cachedDatetimeVisible.Add(false);
                            }
                            else
                            {
                                this.cachedDatetimes.Add(new DateTime?(DateTimeExtension.FromOADate((double) ((double) obj2))));
                                this.cachedDatetimeVisible.Add(true);
                            }
                        }
                        else
                        {
                            DateTime? nullable = null;
                            this.cachedDatetimes.Add(nullable);
                            this.cachedDatetimeVisible.Add(true);
                        }
                    }
                }
                return this.cachedDatetimes;
            }
        }

        /// <summary>
        /// Gets the visible, cached dates and times.
        /// </summary>
        public List<bool> CachedDatetimesVisible
        {
            get { return  this.cachedDatetimeVisible; }
        }

        /// <summary>
        /// Gets the cached index mapping.
        /// </summary>
        public List<int> CachedIndexMaping
        {
            get
            {
                if (this.cachedIndexMapping == null)
                {
                    if (this.SparklineInfo.DisplayDateAxis)
                    {
                        int num = this.CachedValues.Count;
                        int num2 = this.CachedDatetimes.Count;
                        int num3 = Math.Min(num, num2);
                        List<DateTime?> range = this.CachedDatetimes.GetRange(0, num3);
                        range.Sort();
                        this.cachedIndexMapping = new List<int>();
                        for (int i = 0; i < range.Count; i++)
                        {
                            DateTime? nullable = range[i];
                            if (nullable.HasValue)
                            {
                                int index = this.CachedDatetimes.IndexOf(nullable);
                                while (this.cachedIndexMapping.Contains(index))
                                {
                                    index = this.CachedDatetimes.IndexOf(nullable, index + 1);
                                }
                                if (this.cachedDatetimeVisible[index])
                                {
                                    double? nullable2 = this.CachedValues[index];
                                    if (!nullable2.HasValue || !double.IsNaN(nullable2.Value))
                                    {
                                        this.cachedIndexMapping.Add(index);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        this.cachedIndexMapping = new List<int>();
                        for (int j = 0; j < this.CachedValues.Count; j++)
                        {
                            double? nullable3 = this.CachedValues[j];
                            if (!nullable3.HasValue || !double.IsNaN(nullable3.Value))
                            {
                                this.cachedIndexMapping.Add(j);
                            }
                        }
                    }
                }
                return this.cachedIndexMapping;
            }
        }

        /// <summary>
        /// Gets the cached maximum value.
        /// </summary>
        public double CachedMaxValue
        {
            get { return  this.cachedMaxValue; }
        }

        /// <summary>
        /// Gets the cached values.
        /// </summary>
        public List<double?> CachedValues
        {
            get
            {
                if (this.cachedValues == null)
                {
                    this.cachedValues = new List<double?>();
                    for (int i = 0; i < this.info.Data.Count; i++)
                    {
                        object obj2 = this.info.Data.GetValue(i);
                        if (obj2 != null)
                        {
                            this.cachedValues.Add(new double?((double) ((double) obj2)));
                        }
                        else
                        {
                            double? nullable = null;
                            this.cachedValues.Add(nullable);
                        }
                    }
                }
                return this.cachedValues;
            }
        }

        /// <summary>
        /// Gets the cached minimum value.
        /// </summary>
        public double CahcedMinValue
        {
            get { return  this.cahcedMinValue; }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance has an axis.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has an axis; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasAxis
        {
            get
            {
                if (!this.info.Setting.DisplayXAxis)
                {
                    return false;
                }
                double actualMaxValue = this.GetActualMaxValue();
                if (actualMaxValue != double.MinValue)
                {
                    double actualMinValue = this.GetActualMinValue();
                    if (actualMinValue != double.MaxValue)
                    {
                        if (actualMaxValue != actualMinValue)
                        {
                            return ((actualMaxValue * actualMinValue) <= 0.0);
                        }
                        return true;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets  a value that indicates the left space.
        /// </summary>
        public virtual double LeftSpace
        {
            get { return  3.0; }
        }

        /// <summary>
        /// Gets the right space.
        /// </summary>
        public virtual double RightSpace
        {
            get { return  3.0; }
        }

        /// <summary>
        /// Gets or sets a value that indicates the sparkline.
        /// </summary>
        public Sparkline SparklineInfo
        {
            get { return  this.info; }
            set { this.info = value; }
        }

        /// <summary>
        /// Gets the type of the sparkline.
        /// </summary>
        /// <value>
        /// The type of the sparkline.
        /// </value>
        public abstract Dt.Cells.Data.SparklineType SparklineType { get; }

        /// <summary>
        /// Gets the top space.
        /// </summary>
        public virtual double TopSpace
        {
            get { return  3.0; }
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>
        /// The zoom factor.
        /// </value>
        public double ZoomFactor
        {
            get { return  this._zoomfactor; }
            set
            {
                if (value < 0.1)
                {
                    value = 0.1;
                }
                else if (value > 4.0)
                {
                    value = 4.0;
                }
                if (this._zoomfactor != value)
                {
                    this._zoomfactor = value;
                }
            }
        }
    }
}

