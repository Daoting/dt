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
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the base line sparkline view information class.
    /// </summary>
    public class LineSparklineViewInfo : BaseSparklineViewInfo
    {
        List<Tuple<Windows.Foundation.Point, Windows.Foundation.Point>> linePos;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.LineSparklineViewInfo" /> class.
        /// </summary>
        /// <param name="info">The sparkline info</param>
        public LineSparklineViewInfo(Sparkline info) : base(info)
        {
            this.linePos = new List<Tuple<Windows.Foundation.Point, Windows.Foundation.Point>>();
        }

        /// <summary>
        /// Gets the clip boundary.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        /// <returns></returns>
        public override Windows.Foundation.Rect? GetClipBounds(Windows.Foundation.Size finalSize)
        {
            double num = this.GetLineWeight() + 1.0;
            double width = ((finalSize.Width - this.LeftSpace) - this.RightSpace) + (num * 2.0);
            double height = ((finalSize.Height - this.TopSpace) - this.BottomSpace) + (num * 2.0);
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Windows.Foundation.Rect(this.LeftSpace - num, this.TopSpace - num, width, height);
            }
            return null;
        }

        /// <summary>
        /// Gets the data point position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="availableSize">The available size.</param>
        /// <returns></returns>
        public override Windows.Foundation.Rect GetDataPointPosition(int index, Windows.Foundation.Size availableSize)
        {
            double lineWeight = this.GetLineWeight();
            if (lineWeight < 2.0)
            {
                lineWeight = 2.0;
            }
            Windows.Foundation.Rect dataPointPosition = base.GetDataPointPosition(index, availableSize);
            dataPointPosition.X += (dataPointPosition.Width - lineWeight) / 2.0;
            double? cachedValue = this.GetCachedValue(index);
            if (cachedValue.HasValue)
            {
                if (cachedValue.Value >= 0.0)
                {
                    dataPointPosition.Y -= lineWeight / 2.0;
                }
                else
                {
                    dataPointPosition.Y = dataPointPosition.Bottom - (lineWeight / 2.0);
                }
                dataPointPosition.Width = lineWeight;
                dataPointPosition.Height = lineWeight;
                return dataPointPosition;
            }
            dataPointPosition.Width = 0.0;
            dataPointPosition.Height = 0.0;
            return dataPointPosition;
        }

        /// <summary>
        /// Gets the line weight.
        /// </summary>
        /// <returns></returns>
        public double GetLineWeight()
        {
            double num = base.SparklineInfo.Setting.LineWeight * base.ZoomFactor;
            if (num < 1.0)
            {
                num = 1.0;
            }
            return num;
        }

        /// <summary>
        /// Gets the color of the marker.
        /// </summary>
        /// <returns></returns>
        public override Color? GetMarkerColor()
        {
            if (base.SparklineInfo.Setting.ShowMarkers)
            {
                return new Color?(base.SparklineInfo.Setting.MarkersColor);
            }
            return null;
        }

        /// <summary>
        /// Measures the line position.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        public void MeasurelinePos(Windows.Foundation.Size availableSize)
        {
            int num = base.CachedIndexMaping.Count - 1;
            if (num < 0)
            {
                num = 0;
            }
            this.linePos = new List<Tuple<Windows.Foundation.Point, Windows.Foundation.Point>>(new Tuple<Windows.Foundation.Point, Windows.Foundation.Point>[num]);
            for (int i = 0; i < num; i++)
            {
                double? nullable3;
                double? nullable = base.CachedValues[base.CachedIndexMaping[i]];
                if (!nullable.HasValue)
                {
                    switch (base.SparklineInfo.Setting.DisplayEmptyCellsAs)
                    {
                        case EmptyValueStyle.Zero:
                            nullable = 0.0;
                            break;
                    }
                }
                if (!nullable.HasValue)
                {
                    continue;
                }
                int num3 = i + 1;
                double? nullable2 = base.CachedValues[base.CachedIndexMaping[num3]];
                if (!nullable2.HasValue)
                {
                    switch (base.SparklineInfo.Setting.DisplayEmptyCellsAs)
                    {
                        case EmptyValueStyle.Zero:
                            nullable2 = 0.0;
                            break;

                        case EmptyValueStyle.Connect:
                            num3 = i + 2;
                            goto Label_0121;
                    }
                }
                goto Label_0125;
            Label_00F5:
                nullable3 = base.CachedValues[base.CachedIndexMaping[num3]];
                if (nullable3.HasValue)
                {
                    nullable2 = nullable3;
                    goto Label_0125;
                }
                num3++;
            Label_0121:
                if (num3 <= num)
                {
                    goto Label_00F5;
                }
            Label_0125:
                if (nullable2.HasValue)
                {
                    Windows.Foundation.Rect dataPointPosition = this.GetDataPointPosition(base.CachedIndexMaping[i], availableSize);
                    Windows.Foundation.Rect rect2 = this.GetDataPointPosition(base.CachedIndexMaping[num3], availableSize);
                    double num4 = dataPointPosition.Width / 2.0;
                    Windows.Foundation.Point point = new Windows.Foundation.Point(dataPointPosition.X + num4, dataPointPosition.Y + num4);
                    Windows.Foundation.Point point2 = new Windows.Foundation.Point(rect2.X + num4, rect2.Y + num4);
                    this.linePos[i] = new Tuple<Windows.Foundation.Point, Windows.Foundation.Point>(point, point2);
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// Gets the bottom space.
        /// </summary>
        public override double BottomSpace
        {
            get { return  ((base.BottomSpace + this.GetLineWeight()) + 1.0); }
        }

        /// <summary>
        /// Gets a value that indicates the left space.
        /// </summary>
        public override double LeftSpace
        {
            get { return  ((base.LeftSpace + this.GetLineWeight()) + 1.0); }
        }

        /// <summary>
        /// Gets the line position.
        /// </summary>
        public List<Tuple<Windows.Foundation.Point, Windows.Foundation.Point>> LinePos
        {
            get { return  this.linePos; }
            set { this.linePos = value; }
        }

        /// <summary>
        /// Gets the right space.
        /// </summary>
        public override double RightSpace
        {
            get { return  ((base.RightSpace + this.GetLineWeight()) + 1.0); }
        }

        /// <summary>
        /// Gets the type of the sparkline.
        /// </summary>
        /// <value>
        /// The type of the sparkline.
        /// </value>
        public override Dt.Cells.Data.SparklineType SparklineType
        {
            get { return  Dt.Cells.Data.SparklineType.Line; }
        }

        /// <summary>
        /// Gets the top space.
        /// </summary>
        public override double TopSpace
        {
            get { return  ((base.TopSpace + this.GetLineWeight()) + 1.0); }
        }
    }
}

