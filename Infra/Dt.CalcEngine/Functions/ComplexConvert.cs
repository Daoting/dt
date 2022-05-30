#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
#endregion

namespace Dt.CalcEngine.Functions
{
    internal class ComplexConvert
    {
        /// <summary>
        /// Converts the specified value to a complex number.
        /// </summary>
        public static Complex ToComplex(object value)
        {
            Complex complex;
            try
            {
                if (value == null)
                {
                    return new Complex(0.0, 0.0);
                }
                if (value is double)
                {
                    return new Complex((double) ((double) value), 0.0);
                }
                if (value is float)
                {
                    return new Complex((double) ((float) value), 0.0);
                }
                if (value is decimal)
                {
                    return new Complex((double) ((double) ((decimal) value)), 0.0);
                }
                if (value is long)
                {
                    return new Complex((double) ((long) value), 0.0);
                }
                if (value is int)
                {
                    return new Complex((double) ((int) value), 0.0);
                }
                if (value is short)
                {
                    return new Complex((double) ((short) value), 0.0);
                }
                if (value is sbyte)
                {
                    return new Complex((double) ((sbyte) value), 0.0);
                }
                if (value is ulong)
                {
                    return new Complex((double) ((ulong) value), 0.0);
                }
                if (value is uint)
                {
                    return new Complex((double) ((uint) value), 0.0);
                }
                if (value is ushort)
                {
                    return new Complex((double) ((ushort) value), 0.0);
                }
                if (value is byte)
                {
                    return new Complex((double) ((byte) value), 0.0);
                }
                if (!(value is string))
                {
                    throw new InvalidCastException();
                }
                complex = Complex.Parse((string) ((string) value));
            }
            catch
            {
                throw new InvalidCastException();
            }
            return complex;
        }

        /// <summary>
        /// Converts a double to an object.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Object value equivalent to the specified value</returns>
        public static object ToResult(Complex value)
        {
            return ToResult(value, "i");
        }

        /// <summary>
        /// Converts a double to an object.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="suffix">String suffix</param>
        /// <returns>Object value equivalent to the specified value</returns>
        public static object ToResult(Complex value, string suffix)
        {
            if ((!double.IsNaN(value.Real) && !double.IsInfinity(value.Real)) && (!double.IsNaN(value.Imag) && !double.IsInfinity(value.Imag)))
            {
                return value.ToString(suffix);
            }
            return CalcErrors.Number;
        }
    }
}

