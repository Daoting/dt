#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 提供不同长度单位之间的转换
    /// </summary>
    public class LengthUnit
    {
        public const double Centimeters = 37.795275590551185;
        public const double DPI = 96.0;
        public const double Foot = 1152.0;
        public const double Inch = 96.0;
        public const double Kilometers = 377952.75590551185;
        public const double Meters = 3779.5275590551187;
        public const double Miles = 6082560.0;
        public const double Millimeters = 3.7795275590551185;
        public const double Yard = 3456.0;

        LengthUnits _source;
        LengthUnits _target;
        LengthUnits _unit = LengthUnits.Pixels;

        public LengthUnits Unit
        {
            get { return _unit; }
            set
            {
                if (_unit != value)
                {
                    _source = _unit;
                    _unit = value;
                    _target = _unit;
                }
            }
        }

        public double ToPixel(double unit)
        {
            return ToPixel(unit, Unit);
        }

        public double ToUnit(double pixel)
        {
            return ToUnit(pixel, Unit);
        }

        public double Convert(double value, LengthUnits from, LengthUnits to)
        {
            return ToUnit(ToPixel(value, from), to);
        }

        static double ToPixel(double unit, LengthUnits type)
        {
            switch (type)
            {
                case LengthUnits.Inches:
                    return (unit * 96.0);

                case LengthUnits.Feets:
                    return (unit * 1152.0);

                case LengthUnits.Yards:
                    return (unit * 3456.0);

                case LengthUnits.Miles:
                    return (unit * 6082560.0);

                case LengthUnits.Millimeters:
                    return (unit * 3.7795275590551185);

                case LengthUnits.Centimeters:
                    return (unit * 37.795275590551185);

                case LengthUnits.Meters:
                    return (unit * 3779.5275590551187);

                case LengthUnits.Kilometers:
                    return (unit * 377952.75590551185);
            }
            return unit;
        }

        static double ToUnit(double pixel, LengthUnits type)
        {
            switch (type)
            {
                case LengthUnits.Inches:
                    return (pixel / 96.0);

                case LengthUnits.Feets:
                    return (pixel / 1152.0);

                case LengthUnits.Yards:
                    return (pixel / 3456.0);

                case LengthUnits.Miles:
                    return (pixel / 6082560.0);

                case LengthUnits.Millimeters:
                    return (pixel / 3.7795275590551185);

                case LengthUnits.Centimeters:
                    return (pixel / 37.795275590551185);

                case LengthUnits.Meters:
                    return (pixel / 3779.5275590551187);

                case LengthUnits.Kilometers:
                    return (pixel / 377952.75590551185);
            }
            return pixel;
        }
    }

    /// <summary>
    /// 长度单位类别
    /// </summary>
    public enum LengthUnits
    {
        Pixels,
        Inches,
        Feets,
        Yards,
        Miles,
        Millimeters,
        Centimeters,
        Meters,
        Kilometers
    }
}
