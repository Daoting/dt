#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;

#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 刻度线及刻度值
    /// </summary>
    public class Tick
    {
        /// <summary>
        /// 
        /// </summary>
        public RulerSegment Segment { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public double Value { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segmentWidth"></param>
        /// <param name="segmentHeight"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        internal Point GetLinePoint(double segmentWidth, double segmentHeight, out TickAlignment align)
        {
            double num;
            double num2;
            double num3;
            double num4;
            ArrangeTick(out num, out num2, out align);
            if (align == TickAlignment.LeftOrTop)
            {
                num3 = num;
                num4 = num + num2;
            }
            else if (align == TickAlignment.RightOrBottom)
            {
                num3 = segmentHeight - num;
                num4 = segmentHeight - (num + num2);
            }
            else
            {
                num3 = (segmentHeight - num2) / 2.0;
                num4 = segmentHeight - ((segmentHeight - num2) / 2.0);
            }
            return new Point(num3, num4);
        }

        void ArrangeTick(out double start, out double length, out TickAlignment align)
        {
            align = TickAlignment.RightOrBottom;
            start = 0.0;
            if ((this.Value % this.Segment.PxSegmentWidth) == 0.0)
            {
                length = this.Segment.Ruler.Thickness;
            }
            else
            {
                length = this.Segment.Ruler.Thickness * 0.3;
            }
        }
    }

    /// <summary>
    /// 刻度对齐方式
    /// </summary>
    public enum TickAlignment
    {
        LeftOrTop,
        RightOrBottom
    }
}
