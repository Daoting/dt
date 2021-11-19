#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using Windows.Graphics.Display;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a unit manager.
    /// </summary>
    internal static class UnitManager
    {
        /// <summary>
        /// cm - inch
        /// </summary>
        const double CMConstant = 2.539999918;
        const int DefaultDpi = 0x60;
        /// <summary>
        /// the device constant of print device.
        /// </summary>
        const int DeviceConstant = 0x48;
        /// <summary>
        /// 36000 EMUs/cm
        /// </summary>
        const double EMUCmConstant = 36000.0;
        /// <summary>
        /// 91440 EMUs/U.S. inch
        /// </summary>
        const double EMUInchConstant = 91440.0;

        /// <summary>
        /// the dpi
        /// </summary>
        static float _dpi;

        static UnitManager()
        {
            _dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
        }

        /// <summary>
        /// Gets or sets the DPI (dots per inch) value.
        /// </summary>
        /// <value>The DPI value.</value>
        public static float Dpi
        {
            get { return _dpi; }
        }

        /// <summary>
        /// Converts the unit from one type to another.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="from">The unit type to convert from.</param>
        /// <param name="to">The unit type to convert to.</param>
        /// <returns></returns>
        public static double ConvertTo(double value, UnitType from, UnitType to)
        {
            return ConvertTo(value, from, to, _dpi);
        }

        /// <summary>
        /// Converts the unit from one type to another.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="from">The unit type to convert from.</param>
        /// <param name="to">The unit type to convert to.</param>
        /// <param name="dpi">The value, in dots per inch.</param>
        /// <returns>A value in the specified unit type.</returns>
        public static double ConvertTo(double value, UnitType from, UnitType to, float dpi)
        {
            switch (from)
            {
                case UnitType.Pixel:
                    switch (to)
                    {
                        case UnitType.Point:
                            return ((((double) value) / ((double) dpi)) * 72.0);

                        case UnitType.Inch:
                            return (((double) value) / ((double) dpi));

                        case UnitType.Cm:
                            return ((((double) value) / ((double) dpi)) * 2.539999918);

                        case UnitType.CentileInch:
                            return ((((double) value) * 100.0) / ((double) dpi));

                        case UnitType.EMUs:
                            return ((((double) value) / ((double) dpi)) * 91440.0);
                    }
                    return value;

                case UnitType.Point:
                    switch (to)
                    {
                        case UnitType.Pixel:
                            return ((((double) value) * dpi) / 72.0);

                        case UnitType.Point:
                            return value;

                        case UnitType.Inch:
                            return (((double) value) / 72.0);

                        case UnitType.Cm:
                            return ((((double) value) / 72.0) * 2.539999918);

                        case UnitType.CentileInch:
                            return ((((double) value) / 72.0) * 100.0);

                        case UnitType.EMUs:
                            return ((((double) value) / 72.0) * 91440.0);
                    }
                    return value;

                case UnitType.Inch:
                    switch (to)
                    {
                        case UnitType.Pixel:
                            return (((double) value) * dpi);

                        case UnitType.Point:
                            return (((double) value) * 72.0);

                        case UnitType.Inch:
                            return value;

                        case UnitType.Cm:
                            return (((double) value) * 2.539999918);

                        case UnitType.CentileInch:
                            return (((double) value) * 100.0);

                        case UnitType.EMUs:
                            return (((double) value) * 91440.0);
                    }
                    return value;

                case UnitType.Cm:
                    switch (to)
                    {
                        case UnitType.Pixel:
                            return ((((double) value) / 2.539999918) * dpi);

                        case UnitType.Point:
                            return ((((double) value) / 2.539999918) * 72.0);

                        case UnitType.Inch:
                            return (((double) value) / 2.539999918);

                        case UnitType.Cm:
                            return value;

                        case UnitType.CentileInch:
                            return ((((double) value) / 2.539999918) * 100.0);

                        case UnitType.EMUs:
                            return (((double) value) * 36000.0);
                    }
                    return value;

                case UnitType.CentileInch:
                    switch (to)
                    {
                        case UnitType.Pixel:
                            return ((((double) value) * dpi) / 100.0);

                        case UnitType.Point:
                            return ((((double) value) * 72.0) / 100.0);

                        case UnitType.Inch:
                            return (((double) value) / 100.0);

                        case UnitType.Cm:
                            return ((((double) value) * 2.539999918) / 100.0);

                        case UnitType.CentileInch:
                            return value;

                        case UnitType.EMUs:
                            return ((((double) value) / 100.0) * 91440.0);
                    }
                    return value;

                case UnitType.EMUs:
                    switch (to)
                    {
                        case UnitType.Pixel:
                            return ((((double) value) / 91440.0) * dpi);

                        case UnitType.Point:
                            return ((((double) value) / 91440.0) * 72.0);

                        case UnitType.Inch:
                            return (((double) value) / 91440.0);

                        case UnitType.Cm:
                            return (((double) value) / 36000.0);

                        case UnitType.CentileInch:
                            return ((((double) value) / 91440.0) * 100.0);
                    }
                    return value;
            }
            return value;
        }
    }
}

