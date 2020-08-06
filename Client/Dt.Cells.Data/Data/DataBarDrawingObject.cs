#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a data bar object for drawing.
    /// </summary>
    public sealed class DataBarDrawingObject : DrawingObject
    {
        Windows.UI.Color axisColor;
        double axisLocation;
        Windows.UI.Color borderColor;
        BarDirection direction;
        Windows.UI.Color fillColor;
        bool isGradient;
        double scale;
        bool showBarOnly;
        bool showBorder;

        /// <summary>
        /// Constructs a data bar object with the specified row index, column index, color, scale, and data bar or data bar and data.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="fillColor">The fill color of the data bar object.</param>
        /// <param name="borderColor">The border color of the data bar object.</param>
        /// <param name="showBorder">Whether to show the border of the data bar object.</param>
        /// <param name="axisColor">The axis color of the data bar object.</param>
        /// <param name="isGradient">The fill type of the data bar object.</param>
        /// <param name="direction">The direction the data bar object</param>
        /// <param name="axisLocation">The axis location the data bar object</param>
        /// <param name="scale">The scale of the data bar object.</param>
        /// <param name="showBarOnly">
        /// <c>true</c>Show bar only; otherwise, <c>false</c>.
        /// </param>
        public DataBarDrawingObject(int rowIndex, int columnIndex, Windows.UI.Color fillColor, Windows.UI.Color borderColor, bool showBorder, Windows.UI.Color axisColor, bool isGradient, BarDirection direction, double axisLocation, double scale, bool showBarOnly) : base(rowIndex, columnIndex)
        {
            this.fillColor = fillColor;
            this.borderColor = borderColor;
            this.showBorder = showBorder;
            this.axisColor = axisColor;
            this.isGradient = isGradient;
            this.direction = direction;
            this.axisLocation = axisLocation;
            this.scale = scale;
            this.showBarOnly = showBarOnly;
        }

        /// <summary>
        /// Compares whether the current object is equal to the specified object.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>
        /// <c>true</c> The current object is equal to the specified object; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                DataBarDrawingObject obj2 = obj as DataBarDrawingObject;
                if (obj2 != null)
                {
                    return (((((this.fillColor == obj2.fillColor) && (this.borderColor == obj2.borderColor)) && ((this.axisColor == obj2.axisColor) && (this.isGradient == obj2.isGradient))) && (((this.direction == obj2.direction) && (this.axisLocation == obj2.axisLocation)) && ((this.scale == obj2.scale) && (this.showBarOnly == obj2.showBarOnly)))) && (this.showBorder == obj2.showBorder));
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return ((((((((base.GetHashCode() | this.fillColor.GetHashCode()) | this.borderColor.GetHashCode()) | this.axisColor.GetHashCode()) | this.isGradient.GetHashCode()) | this.direction.GetHashCode()) | ((double) this.axisLocation).GetHashCode()) | ((double) this.scale).GetHashCode()) | this.showBarOnly.GetHashCode());
        }

        /// <summary>
        /// Gets the axis color. 
        /// </summary>
        public Windows.UI.Color AxisColor
        {
            get { return  this.axisColor; }
        }

        /// <summary>
        /// Gets the border color. 
        /// </summary>
        public Windows.UI.Color BorderColor
        {
            get { return  this.borderColor; }
        }

        /// <summary>
        /// Gets the postive fill color. 
        /// </summary>
        public Windows.UI.Color Color
        {
            get { return  this.fillColor; }
        }

        /// <summary>
        /// Gets the data bar axis position; 
        /// the value is from 0 to 1.
        /// </summary>
        public double DataBarAxisPosition
        {
            get { return  this.axisLocation; }
        }

        /// <summary>
        /// Gets the data bar direction.
        /// </summary>
        public BarDirection DataBarDirection
        {
            get { return  this.direction; }
        }

        /// <summary>
        /// Gets the data bar fill type.
        /// </summary>
        public bool Gradient
        {
            get { return  this.isGradient; }
        }

        /// <summary>
        /// Gets the scale of the data bar object; 
        /// the positive value represents the drawing object from left to right and 
        /// the negative value represents the drawing object from right to left.
        /// The value is from 0 to 1.
        /// </summary>
        public double Scale
        {
            get { return  this.scale; }
        }

        /// <summary>
        /// Gets whether to only show the bar.
        /// </summary>
        public bool ShowBarOnly
        {
            get { return  this.showBarOnly; }
        }

        /// <summary>
        /// Gets whether to show the border. 
        /// </summary>
        public bool ShowBorder
        {
            get { return  this.showBorder; }
        }
    }
}

