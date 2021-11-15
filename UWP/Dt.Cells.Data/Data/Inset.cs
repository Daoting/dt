#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the dimensions of borders and margins.
    /// </summary>
    public sealed class Inset
    {
        /// <summary>
        /// the bottom position.
        /// </summary>
        double bottom;
        /// <summary>
        /// the left position.
        /// </summary>
        double left;
        /// <summary>
        /// the right position.
        /// </summary>
        double right;
        /// <summary>
        /// the top position.
        /// </summary>
        double top;
        /// <summary>
        /// the unit type for bottom.
        /// </summary>
        UnitType unitTypeBottom;
        /// <summary>
        /// the unit type for left.
        /// </summary>
        UnitType unitTypeLeft;
        /// <summary>
        /// the unit type for right.
        /// </summary>
        UnitType unitTypeRight;
        /// <summary>
        /// the unit type for top.
        /// </summary>
        UnitType unitTypeTop;

        /// <summary>
        /// Creates an inset for the specified side.
        /// </summary>
        /// <param name="sideInset">The left, top, right, or bottom inset.</param>
        public Inset(double sideInset) : this(sideInset, sideInset)
        {
        }

        /// <summary>
        /// Creates an inset with the specified <i>x</i> and <i>y</i> values.
        /// </summary>
        /// <param name="xInset">The left and right inset.</param>
        /// <param name="yInset">The top and bottom inset.</param>
        public Inset(double xInset, double yInset) : this(xInset, yInset, xInset, yInset, UnitType.Pixel)
        {
        }

        /// <summary>
        /// Creates an inset with the four specified sides and the specified unit of measure.
        /// </summary>
        /// <param name="leftInset">The left inset.</param>
        /// <param name="topInset">The top inset.</param>
        /// <param name="rightInset">The right inset.</param>
        /// <param name="bottomInset">The bottom inset.</param>
        /// <param name="unitType">The unit of measure for the left, top, right, and bottom insets.</param>
        public Inset(double leftInset, double topInset, double rightInset, double bottomInset, UnitType unitType) : this(leftInset, topInset, rightInset, bottomInset, unitType, unitType, unitType, unitType)
        {
        }

        /// <summary>
        /// Creates an inset with the four specified sides and the specified units for each side.
        /// </summary>
        /// <param name="leftInset">The left inset.</param>
        /// <param name="topInset">The top inset.</param>
        /// <param name="rightInset">The right inset.</param>
        /// <param name="bottomInset">The bottom inset.</param>
        /// <param name="unitTypeLeft">The unit of measure for the left inset.</param>
        /// <param name="unitTypeTop">The unit of measure for the top inset.</param>
        /// <param name="unitTypeRight">The unit of measure for the right inset.</param>
        /// <param name="unitTypeBottom">The unit of measure for the bottom inset.</param>
        public Inset(double leftInset, double topInset, double rightInset, double bottomInset, UnitType unitTypeLeft, UnitType unitTypeTop, UnitType unitTypeRight, UnitType unitTypeBottom)
        {
            this.left = leftInset;
            this.top = topInset;
            this.right = rightInset;
            this.bottom = bottomInset;
            this.unitTypeLeft = unitTypeLeft;
            this.unitTypeTop = unitTypeTop;
            this.unitTypeRight = unitTypeRight;
            this.unitTypeBottom = unitTypeBottom;
        }

        /// <summary>
        /// Gets the bottom inset using the specified unit type.
        /// </summary>
        /// <param name="unitType">The unit of measure.</param>
        /// <param name="dpi">The value, in dots per inch.</param>
        /// <returns>The bottom inset in the specified unit type.</returns>
        public double GetBottom(UnitType unitType, int dpi)
        {
            return UnitManager.ConvertTo(this.bottom, this.unitTypeBottom, unitType, (float) dpi);
        }

        /// <summary>
        /// Gets the left inset using the specified unit type.
        /// </summary>
        /// <param name="unitType">The unit of measure.</param>
        /// <param name="dpi">The value, in dots per inch.</param>
        /// <returns>The left inset in the specified unit type.</returns>
        public double GetLeft(UnitType unitType, int dpi)
        {
            return UnitManager.ConvertTo(this.left, this.unitTypeLeft, unitType, (float) dpi);
        }

        /// <summary>
        /// Gets the right inset using the specified unit type.
        /// </summary>
        /// <param name="unitType">The unit of measure.</param>
        /// <param name="dpi">The value, in dots per inch.</param>
        /// <returns>The right inset in the specified unit type.</returns>
        public double GetRight(UnitType unitType, int dpi)
        {
            return UnitManager.ConvertTo(this.right, this.unitTypeRight, unitType, (float) dpi);
        }

        /// <summary>
        /// Gets the top inset using the specified unit type.
        /// </summary>
        /// <param name="unitType">The unit of measure.</param>
        /// <param name="dpi">The value, in dots per inch.</param>
        /// <returns>The top inset in the specified unit type.</returns>
        public double GetTop(UnitType unitType, int dpi)
        {
            return UnitManager.ConvertTo(this.top, this.unitTypeTop, unitType, (float) dpi);
        }

        /// <summary>
        /// Gets the bottom inset.
        /// </summary>
        /// <value>The bottom inset.</value>
        [Description("Gets the inset from the bottom.")]
        public double Bottom
        {
            get { return  this.bottom; }
        }

        /// <summary>
        /// Gets the left inset.
        /// </summary>
        /// <value>The left inset.</value>
        [Description("Gets the inset from the left.")]
        public double Left
        {
            get { return  this.left; }
        }

        /// <summary>
        /// Gets the right inset.
        /// </summary>
        /// <value>The right inset.</value>
        [Description("Gets the inset from the right.")]
        public double Right
        {
            get { return  this.right; }
        }

        /// <summary>
        /// Gets the top inset.
        /// </summary>
        /// <value>The top inset.</value>
        [Description("Gets the inset from the top.")]
        public double Top
        {
            get { return  this.top; }
        }

        /// <summary>
        /// Gets the bottom inset's unit type.
        /// </summary>
        /// <value>The unit of measure for the bottom inset.</value>
        [Description("Gets the bottom inset's unit type.")]
        public UnitType UnitTypeBottom
        {
            get { return  this.unitTypeBottom; }
        }

        /// <summary>
        /// Gets the left inset's unit type.
        /// </summary>
        /// <value>The unit of measure for the left inset.</value>
        [Description("Gets the left inset's unit type.")]
        public UnitType UnitTypeLeft
        {
            get { return  this.unitTypeLeft; }
        }

        /// <summary>
        /// Gets the right inset's unit type.
        /// </summary>
        /// <value>The unit of measure for the right inset.</value>
        [Description("Gets the right inset's unit type.")]
        public UnitType UnitTypeRight
        {
            get { return  this.unitTypeRight; }
        }

        /// <summary>
        /// Gets the top inset's unit type.
        /// </summary>
        /// <value>The unit of measure for the top inset.</value>
        [Description("Gets the top inset's unit type.")]
        public UnitType UnitTypeTop
        {
            get { return  this.unitTypeTop; }
        }
    }
}

