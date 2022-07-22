#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Color object for Pdf
    /// </summary>
    public class PdfColor
    {
        private int blue;
        public static PdfColor Empty = new PdfColor();
        private int green;
        private int red;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColor" /> class.
        /// </summary>
        public PdfColor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfColor" /> class.
        /// </summary>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        public PdfColor(int red, int green, int blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        /// <summary>
        /// Equalses the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public bool Equals(PdfColor obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }
            return (object.ReferenceEquals(this, obj) || (((obj.red == this.red) && (obj.green == this.green)) && (obj.blue == this.blue)));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(PdfColor))
            {
                return false;
            }
            return this.Equals((PdfColor) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            int num = (this.red * 0x18d) ^ this.green;
            return ((num * 0x18d) ^ this.blue);
        }

        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns></returns>
        public float[] ToArray()
        {
            return new float[] { this.RedF, this.GreenF, this.BlueF };
        }

        /// <summary>
        /// Gets or sets the blue.
        /// </summary>
        /// <value>The blue.</value>
        public int Blue
        {
            get { return  this.blue; }
            set { this.blue = value; }
        }

        /// <summary>
        /// Gets the blue F.
        /// </summary>
        /// <value>The blue F.</value>
        public float BlueF
        {
            get { return  (((float) this.blue) / 255f); }
        }

        /// <summary>
        /// Gets or sets the green.
        /// </summary>
        /// <value>The green.</value>
        public int Green
        {
            get { return  this.green; }
            set { this.green = value; }
        }

        /// <summary>
        /// Gets the green F.
        /// </summary>
        /// <value>The green F.</value>
        public float GreenF
        {
            get { return  (((float) this.green) / 255f); }
        }

        /// <summary>
        /// Gets or sets the red.
        /// </summary>
        /// <value>The red.</value>
        public int Red
        {
            get { return  this.red; }
            set { this.red = value; }
        }

        /// <summary>
        /// Gets the red F.
        /// </summary>
        /// <value>The red F.</value>
        public float RedF
        {
            get { return  (((float) this.red) / 255f); }
        }
    }
}

