#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents color used in Excel
    /// </summary>
    public class ExcelColor : IExcelColor, IEquatable<IExcelColor>
    {
        private uint _color;
        private ExcelColorType _colorType;
        private double _tint;
        /// <summary>
        /// Represents an empty color.
        /// </summary>
        public static ExcelColor EmptyColor = new ExcelColor(new GcColor());

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelColor" /> class.
        /// </summary>
        /// <param name="paletteColor">The color palette index</param>
        public ExcelColor(ExcelPaletteColor paletteColor) : this(ExcelColorType.Indexed, (uint) paletteColor, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelColor" /> class.
        /// </summary>
        /// <param name="color">The RGB color.</param>
        public ExcelColor(GcColor color) : this(ExcelColorType.RGB, color.ToArgb(), 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelColor" /> class.
        /// </summary>
        /// <param name="colorType">Type of the color.</param>
        /// <param name="color">The value of the color</param>
        /// <param name="tint">The tint applied to the color</param>
        public ExcelColor(ExcelColorType colorType, uint color, double tint = 0.0)
        {
            if ((((colorType == ExcelColorType.Theme) && (color > 11)) && ((color != 0xf1) && (color != 0xf2))) && (((color != 0xf3) && (color != 0xf4)) && (color != 0xff)))
            {
                throw new ArgumentOutOfRangeException(ResourceHelper.GetResourceString("themeColorIndexError"));
            }
            if ((tint > 1.0) || (tint < -1.0))
            {
                throw new ArgumentOutOfRangeException(ResourceHelper.GetResourceString("colorTintError"));
            }
            if ((colorType == ExcelColorType.Indexed) && (color == 0x7fff))
            {
                this.IsAutoColor = true;
            }
            this._colorType = colorType;
            this._color = color;
            this._tint = tint;
        }

        /// <summary>
        /// Determines whether the current instance is equals to the specific instance.
        /// </summary>
        /// <param name="other">The other <see cref="T:Dt.Xls.IExcelColor" /> instance used to compare</param>
        /// <returns></returns>
        public bool Equals(IExcelColor other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (other == null)
            {
                return false;
            }
            return ((((this.ColorType == other.ColorType) && (this.Value == other.Value)) && (this.Tint == other.Tint)) && (this.IsAutoColor == other.IsAutoColor));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals((IExcelColor) (obj as ExcelColor));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ((((uint) this._color).GetHashCode() << 8) ^ this._colorType.GetHashCode());
        }

        /// <summary>
        /// Gets the type of the color.
        /// </summary>
        /// <value>The type of the color.</value>
        public ExcelColorType ColorType
        {
            get { return  this._colorType; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is auto color.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is auto color; otherwise, <see langword="false" />.
        /// </value>
        public bool IsAutoColor { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is indexed color.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is indexed color; otherwise, <see langword="false" />.
        /// </value>
        public bool IsIndexedColor
        {
            get { return  (this.ColorType == ExcelColorType.Indexed); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is RGB color.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is RGB color; otherwise, <see langword="false" />.
        /// </value>
        public bool IsRGBColor
        {
            get { return  (this.ColorType == ExcelColorType.RGB); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is theme color.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is theme color; otherwise, <see langword="false" />.
        /// </value>
        public bool IsThemeColor
        {
            get { return  (this.ColorType == ExcelColorType.Theme); }
        }

        /// <summary>
        /// Gets the tint applied to the color.
        /// </summary>
        /// <value>The tint applied to the color</value>
        /// <remarks>
        /// If tint is supplied, then it is applied to the RGB value of the color to determine the final
        /// color applied. The tint value is stored as a double from -1.0 .. 1.0, where -1.0 means 100% darken and
        /// 1.0 means 100% lighten. Also, 0.0 means no change.
        /// </remarks>
        public double Tint
        {
            get { return  this._tint; }
        }

        /// <summary>
        /// Gets the value of the color
        /// </summary>
        /// <value>The value of the color</value>
        public uint Value
        {
            get { return  this._color; }
        }
    }
}

