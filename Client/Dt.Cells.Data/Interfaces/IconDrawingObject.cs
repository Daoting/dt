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
    /// Represents an icon object for drawing.
    /// </summary>
    public sealed class IconDrawingObject : DrawingObject
    {
        Dt.Cells.Data.IconSetType iconSetType;
        int indexOfIcon;
        bool showIconOnly;

        /// <summary>
        /// Constructs an icon object with the specified row index, column index, icon set type, icon collection index, and icon or icon and data.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="iconSetType">The icon set type.</param>
        /// <param name="indexOfIcon">The index of the icon collection.</param>
        /// <param name="showIconOnly">
        /// <c>true</c>Show bar only; otherwise, <c>false</c>
        /// </param>
        public IconDrawingObject(int rowIndex, int columnIndex, Dt.Cells.Data.IconSetType iconSetType, int indexOfIcon, bool showIconOnly) : base(rowIndex, columnIndex)
        {
            this.iconSetType = iconSetType;
            this.indexOfIcon = indexOfIcon;
            this.showIconOnly = showIconOnly;
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
            IconDrawingObject obj2 = obj as IconDrawingObject;
            if (obj2 == null)
            {
                return base.Equals(obj);
            }
            return (((this.iconSetType == obj2.iconSetType) && (this.indexOfIcon == obj2.indexOfIcon)) && (this.showIconOnly == obj2.showIconOnly));
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code. </returns>
        public override int GetHashCode()
        {
            return ((this.iconSetType.GetHashCode() | this.indexOfIcon) | this.showIconOnly.GetHashCode());
        }

        /// <summary>
        /// Gets the icon set type of the icon object.
        /// </summary>
        /// <value>
        /// A value that specifies the scale value type.
        /// </value>&gt;
        public Dt.Cells.Data.IconSetType IconSetType
        {
            get { return  this.iconSetType; }
        }

        /// <summary>
        /// Gets the index of the icon object in the icon set type.
        /// </summary>
        /// <value> The index of the icon. </value>
        public int IndexOfIcon
        {
            get { return  this.indexOfIcon; }
        }

        /// <summary>
        /// Gets or sets whether to only show the icon.
        /// </summary>
        public bool ShowIconOnly
        {
            get { return  this.showIconOnly; }
        }
    }
}

