#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    internal class RadarView
    {
        public double Angle;
        public Point Center;
        public SweepDirection Direction = SweepDirection.Clockwise;
        public bool IsPolar;
        public double Radius;

        public double GetAngle(double angle)
        {
            if (Direction == SweepDirection.Counterclockwise)
            {
                angle = -angle;
            }
            return ((angle + ((Angle * 3.1415926535897931) / 180.0)) - 1.5707963267948966);
        }

        public Rect GetAxisYRect(double h)
        {
            return new Rect(Center.X - h, Center.Y - Radius, Radius, h);
        }

        public void Init(Rect rc)
        {
            Radius = 0.5 * Math.Min(rc.Width, rc.Height);
            Center = new Point(rc.X + (0.5 * rc.Width), rc.Y + (0.5 * rc.Height));
        }
    }
}

