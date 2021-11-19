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
    /// Represents the winloss line sparkline view information class.
    /// </summary>
    public class WinLossSparklineViewInfo : BaseSparklineViewInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.WinLossSparklineViewInfo" /> class.
        /// </summary>
        /// <param name="info">The sparkline info</param>
        public WinLossSparklineViewInfo(Sparkline info) : base(info)
        {
        }

        /// <summary>
        /// Gets the y-axis.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns></returns>
        public override double GetAxisY(Windows.Foundation.Size availableSize)
        {
            return (availableSize.Height / 2.0);
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
            double num = nullable.HasValue ? ((double) nullable.GetValueOrDefault()) : 0.0;
            if (num == 0.0)
            {
                return 0.0;
            }
            Windows.Foundation.Size canvasSize = base.GetCanvasSize(availableSize);
            if (num >= 0.0)
            {
                return (canvasSize.Height / 2.0);
            }
            return (-canvasSize.Height / 2.0);
        }

        /// <summary>
        /// Gets the height of the visible item.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override double GetItemVisibleHeight(Windows.Foundation.Size availableSize, int index)
        {
            return this.GetItemHeight(availableSize, index);
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
        /// Gets a value that indicates whether this instance has an axis.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has an axis; otherwise, <c>false</c>.
        /// </value>
        public override bool HasAxis
        {
            get
            {
                bool hasAxis = base.HasAxis;
                if (!hasAxis && (base.CachedIndexMaping.Count > 0))
                {
                    foreach (int num in base.CachedIndexMaping)
                    {
                        if (base.CachedValues[num].HasValue)
                        {
                            return true;
                        }
                    }
                }
                return hasAxis;
            }
        }

        /// <summary>
        /// Gets the type of the sparkline.
        /// </summary>
        /// <value>
        /// The type of the sparkline.
        /// </value>
        public override Dt.Cells.Data.SparklineType SparklineType
        {
            get { return  Dt.Cells.Data.SparklineType.Winloss; }
        }
    }
}

