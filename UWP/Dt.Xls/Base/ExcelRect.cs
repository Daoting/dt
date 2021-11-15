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
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class implements interface <see cref="T:Dt.Xls.IExcelRect" />, used to represents the client area of the window
    /// </summary>
    public class ExcelRect : IExcelRect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelRect" /> class.
        /// </summary>
        public ExcelRect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelRect" /> class.
        /// </summary>
        /// <param name="left">The horizontal position of the window</param>
        /// <param name="top">The vertical position of the window</param>
        /// <param name="width">The width of the window</param>
        /// <param name="height">The height of the window</param>
        public ExcelRect(double left, double top, double width, double height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Specifies whether this Dt.Xls.ExcelRect contains the same coordinates as the specified Dt.Xls.ExcelRect
        /// </summary>
        /// <param name="value">A Dt.Xls.ExcelRect object to compare</param>
        /// <returns>rue if obj is a Dt.Xls.ExcelRect and has the same coordinates as this Dt.Xls.ExcelRect</returns>
        public bool Equals(ExcelRect value)
        {
            if (value == null)
            {
                return false;
            }
            return ((((this.Left == value.Left) && (this.Top == value.Top)) && (this.Width == value.Width)) && (this.Height == value.Height));
        }

        /// <summary>
        /// Specifies whether this Dt.Xls.ExcelRect contains the same coordinates as the specified System.Object
        /// </summary>
        /// <param name="obj">A System.Object object to compare</param>
        /// <returns>rue if obj is a Dt.Xls.ExcelRect and has the same coordinates as this  System.Object</returns>
        public override bool Equals(object obj)
        {
            return (((obj != null) && (obj is ExcelRect)) && this.Equals((ExcelRect) obj));
        }

        /// <summary>
        /// Returns the hash code for this Dt.Xls.ExcelRect object 
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this object</returns>
        public override int GetHashCode()
        {
            return (((((double) this.Left).GetHashCode() ^ ((double) this.Top).GetHashCode()) ^ ((double) this.Width).GetHashCode()) ^ ((double) this.Height).GetHashCode());
        }

        /// <summary>
        /// Height of the window.
        /// </summary>
        /// <value>The height of the window.</value>
        public double Height { get; set; }

        /// <summary>
        /// The horizontal position of the window. The value is relative to the logical left edge of the client area of the window.
        /// </summary>
        /// <value>The horizontal position of the window</value>
        public double Left { get; set; }

        /// <summary>
        /// The vertical position, in twips, of the window. The value is relative to the top edge of the client area of the window.
        /// </summary>
        /// <value>The vertical position of the window</value>
        public double Top { get; set; }

        /// <summary>
        /// Width of the window.
        /// </summary>
        /// <value>The width of the window</value>
        public double Width { get; set; }
    }
}

