#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
#endregion

namespace Dt.Charts
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PieInfo
    {
        public Point Center;
        public double RadiusX;
        public double RadiusY;
        public double Angle;
        public double Sweep;
        public double Height;
        public double InnerRadius;
        public double Offset;
        public PieInfo(Point center, double radiusX, double radiusY, double inner, double angle, double sweep, double height, double offset = 0.0)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
            InnerRadius = inner;
            Angle = angle;
            Sweep = sweep;
            Height = height;
            if (InnerRadius > 1.0)
            {
                InnerRadius = 1.0;
            }
            else if (InnerRadius < 0.0)
            {
                InnerRadius = 0.0;
            }
            Offset = offset;
        }

        public override int GetHashCode()
        {
            return ((((((Center.GetHashCode() ^ ((double) Angle).GetHashCode()) ^ ((double) Height).GetHashCode()) ^ ((double) InnerRadius).GetHashCode()) ^ ((double) RadiusX).GetHashCode()) ^ ((double) RadiusY).GetHashCode()) ^ ((double) Sweep).GetHashCode());
        }

        public static bool operator ==(PieInfo pie1, PieInfo pie2)
        {
            return (((((pie1.Angle == pie2.Angle) && (pie1.Center == pie2.Center)) && ((pie1.Height == pie2.Height) && (pie1.InnerRadius == pie2.InnerRadius))) && ((pie1.RadiusX == pie2.RadiusX) && (pie1.RadiusY == pie2.RadiusY))) && (pie1.Sweep == pie2.Sweep));
        }

        public static bool operator !=(PieInfo pie1, PieInfo pie2)
        {
            return !(pie1 == pie2);
        }

        public bool Equals(PieInfo value)
        {
            return (this == value);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is PieInfo))
            {
                return false;
            }
            PieInfo info = (PieInfo) obj;
            return (this == info);
        }

        internal Point GetRadiusCenter()
        {
            double d = 0.017453292519943295 * (Angle + (0.5 * Sweep));
            double x = RadiusX * Math.Cos(d);
            double num3 = RadiusY * Math.Sin(d);
            x += Center.X;
            return new Point(x, num3 + Center.Y);
        }
    }
}

