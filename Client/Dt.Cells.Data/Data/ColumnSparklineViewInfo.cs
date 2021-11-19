#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the base column sparkline view information class.
    /// </summary>
    public class ColumnSparklineViewInfo : BaseSparklineViewInfo
    {
        internal const int MinItemHeight = 2;
        Windows.Foundation.Size theSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ColumnSparklineViewInfo" /> class.
        /// </summary>
        /// <param name="info">The sparkline info</param>
        public ColumnSparklineViewInfo(Sparkline info) : base(info)
        {
            this.theSize = Windows.Foundation.Size.Empty;
        }

        /// <summary>
        /// Gets the height of the item.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override double GetItemHeight(Windows.Foundation.Size availableSize, int index)
        {
            double? nullable = base.CachedValues[index];
            if (!nullable.HasValue && (base.SparklineInfo.Setting.DisplayEmptyCellsAs == EmptyValueStyle.Zero))
            {
                return 0.0;
            }
            double itemHeight = base.GetItemHeight(availableSize, index);
            if ((itemHeight > -2.0) && (itemHeight < 2.0))
            {
                double? nullable2 = nullable;
                if ((((double) nullable2.GetValueOrDefault()) > 0.0) && nullable2.HasValue)
                {
                    return (itemHeight + 2.0);
                }
                double? nullable3 = nullable;
                if ((((double) nullable3.GetValueOrDefault()) < 0.0) && nullable3.HasValue)
                {
                    return (itemHeight - 2.0);
                }
            }
            return itemHeight;
        }

        /// <summary>
        /// Gets the height of the visible item.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override double GetItemVisibleHeight(Windows.Foundation.Size availableSize, int index)
        {
            double itemVisibleHeight = base.GetItemVisibleHeight(availableSize, index);
            if ((itemVisibleHeight <= -2.0) || (itemVisibleHeight >= 2.0))
            {
                return itemVisibleHeight;
            }
            double? cachedValue = this.GetCachedValue(index);
            double num2 = cachedValue.HasValue ? ((double) cachedValue.GetValueOrDefault()) : 0.0;
            if (num2 == 0.0)
            {
                return itemVisibleHeight;
            }
            if (num2 > 0.0)
            {
                return (itemVisibleHeight + 2.0);
            }
            return (itemVisibleHeight - 2.0);
        }

        /// <summary>
        /// Gets the color of the marker.
        /// </summary>
        /// <returns></returns>
        public override Color? GetMarkerColor()
        {
            return new Color?(base.SparklineInfo.Setting.SeriesColor);
        }

        /// <summary>
        /// Gets the type of the sparkline.
        /// </summary>
        /// <value>
        /// The type of the sparkline.
        /// </value>
        public override Dt.Cells.Data.SparklineType SparklineType
        {
            get { return  Dt.Cells.Data.SparklineType.Column; }
        }
    }
}

