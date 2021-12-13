#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents an ARGB (alpha, red, green, blue) color.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GcColor
    {
        private GcARGBColor _gcARGBColor;
        /// <summary>
        /// The alpha component value of this Dt.Xls.GcColor
        /// </summary>
        public byte A
        {
            get { return  this._gcARGBColor.a; }
        }
        /// <summary>
        /// The red component value of this Dt.Xls.GcColor
        /// </summary>
        public byte R
        {
            get { return  this._gcARGBColor.r; }
        }
        /// <summary>
        /// The green component value of this Dt.Xls.GcColor
        /// </summary>
        public byte G
        {
            get { return  this._gcARGBColor.g; }
        }
        /// <summary>
        /// The blue component value of this Dt.Xls.GcColor
        /// </summary>
        public byte B
        {
            get { return  this._gcARGBColor.b; }
        }
        /// <summary>
        /// Create a Dt.Xls.GcColor structure from the specified 8-bit color values (red,green and blue)
        /// The alpha value is implicitly 255 (fully opaque).Although this method allows a 32-bit value to be passed for each color component,
        /// the value of each component is limited to 8 bits.
        /// </summary>
        /// <param name="red">The red component value for the new Dt.Xls.GcColor Valid values are 0 through 255</param>
        /// <param name="green">The green component value for the new Dt.Xls.GcColor Valid values are 0 through 255</param>
        /// <param name="blue">The blue component value for the new Dt.Xls.GcColor Valid values are 0 through 255</param>
        /// <returns>The Dt.Xls.GcColor that this method creates.</returns>
        public static GcColor FromArgb(int red, int green, int blue)
        {
            return FromArgb(0xff, red, green, blue);
        }

        /// <summary>
        /// Create a Dt.Xls.GcColor structure from the specified 8-bit color values (red,green and blue)
        /// Although this method allows a 32-bit value to be passed for each color component,
        /// the value of each component is limited to 8 bits.
        /// </summary>
        /// <param name="alpha">The alpha component value for the new Dt.Xls.GcColor Valid values are 0 through 255</param>
        /// <param name="red">The red component value for the new Dt.Xls.GcColor Valid values are 0 through 255</param>
        /// <param name="green">The green component value for the new Dt.Xls.GcColor Valid values are 0 through 255</param>
        /// <param name="blue">The blue component value for the new Dt.Xls.GcColor Valid values are 0 through 255</param>
        /// <returns>
        /// The Dt.Xls.GcColor that this method creates.
        /// </returns>
        public static GcColor FromArgb(int alpha, int red, int green, int blue)
        {
            GcColor color = new GcColor();
            color._gcARGBColor.a = (byte) alpha;
            color._gcARGBColor.r = (byte) red;
            color._gcARGBColor.g = (byte) green;
            color._gcARGBColor.b = (byte) blue;
            return color;
        }

        /// <summary>
        /// Gets the 32-bit ARGB value of this structure.
        /// </summary>
        /// <returns> The 32-bit ARGB value of this Dt.Xls.GcColor</returns>
        public uint ToArgb()
        {
            return uint.Parse(((byte) this.A).ToString("X2") + ((byte) this.R).ToString("X2") + ((byte) this.G).ToString("X2") + ((byte) this.B).ToString("X2"), (NumberStyles) NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compares two Dt.Xls.GcColors for exact equality.
        /// </summary>
        /// <param name="color1"> The first Dt.Xls.GcColors to compare.</param>
        /// <param name="color2"> The second Dt.Xls.GcColors to compare.</param>
        /// <returns>true if the Dt.Xls.GcColors have the same ARGB value; otherwise, false.</returns>
        public static bool operator ==(GcColor color1, GcColor color2)
        {
            return ((((color1.A == color2.A) && (color1.R == color2.R)) && (color1.G == color2.G)) && (color1.B == color2.B));
        }

        /// <summary>
        /// Compares two Dt.Xls.GcColors for exact inequality.
        /// </summary>
        /// <param name="color1"> The first Dt.Xls.GcColors to compare.</param>
        /// <param name="color2"> The second Dt.Xls.GcColors to compare.</param>
        /// <returns>true if the two Dt.Xls.GcColors have the different ARGB value; otherwise, false.</returns>
        public static bool operator !=(GcColor color1, GcColor color2)
        {
            return !(color1 == color2);
        }

        /// <summary>
        /// Indicates whether the specified Dt.Xls.GcColors are equal.
        /// </summary>
        /// <param name="color1"> The first Dt.Xls.GcColors to compare.</param>
        /// <param name="color2"> The second Dt.Xls.GcColors to compare.</param>
        /// <returns>true if the  Dt.Xls.GcColor have the sameARGB value; otherwise, false.</returns>
        public static bool Equals(GcColor color1, GcColor color2)
        {
            return (color1 == color2);
        }

        /// <summary>
        /// Indicates whether the specified Dt.Xls.GcColor is equal to the current Dt.Xls.GcColor.
        /// </summary>
        /// <param name="o"> The object to compare to the current Dt.Xls.GcColor..</param>
        /// <returns>true if the Dt.Xls.GcColor have the same ARGB value; otherwise, false.</returns>
        public override bool Equals(object o)
        {
            if ((o == null) || !(o is GcColor))
            {
                return false;
            }
            GcColor color = (GcColor) o;
            return Equals(this, color);
        }

        /// <summary>
        /// Indicates whether the specified Dt.Xls.GcColor is equal to the current Dt.Xls.GcColor.
        /// </summary>
        /// <param name="value"> The Dt.Xls.GcColor to compare to the current rectangle.</param>
        /// <returns>true if the Dt.Xls.GcColors have the same ARGB value; otherwise, false.</returns>
        public bool Equals(GcColor value)
        {
            return Equals(this, value);
        }

        /// <summary>
        /// Returns a hash code for this Dt.Xls.GcColor structure.
        /// </summary>
        /// <returns> An integer value that specifies the hash code for this Dt.Xls.GcColor structure</returns>
        public override int GetHashCode()
        {
            return ((uint) this.ToArgb()).GetHashCode();
        }

        /// <summary>
        /// Convert this Dt.Xls.GcColor to a human-readable string
        /// </summary>
        /// <returns> A string that consists of the ARGB component names and their values.</returns>
        public override string ToString()
        {
            return (((byte) this.A).ToString("X2") + ((byte) this.R).ToString("X2") + ((byte) this.G).ToString("X2") + ((byte) this.B).ToString("X2"));
        }

        internal static GcColor FromArgb(uint argb)
        {
            GcColor color = new GcColor();
            color._gcARGBColor.a = (byte) ((argb >> 0x18) & 0xff);
            color._gcARGBColor.r = (byte) ((argb >> 0x10) & 0xff);
            color._gcARGBColor.g = (byte) ((argb >> 8) & 0xff);
            color._gcARGBColor.b = (byte) (argb & 0xff);
            return color;
        }
    }
}

