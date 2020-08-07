#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a drawing object displayed in a cell.
    /// </summary>
    public abstract class CustomDrawingObject : DrawingObject
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:CustomDrawingObject" /> class.
        /// </summary>
        /// <param name="anchorRow">The anchor row index to draw the object.</param>
        /// <param name="anchorColumn">The anchor column index to draw the object.</param>
        public CustomDrawingObject(int anchorRow, int anchorColumn) : base(anchorRow, anchorColumn)
        {
        }

        /// <summary>
        /// Gets the root element of the drawing object on its visual tree.
        /// </summary>
        public abstract FrameworkElement RootElement { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether to show only the drawing object.
        /// </summary>
        /// <value><c>true</c> to only show the drawing object; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// If this property is set to true, the text of the cell is not shown, but, any other drawing objects (such as conditional format icons) are still shown.
        /// </remarks>
        [DefaultValue(false)]
        public bool ShowDrawingObjectOnly { get; set; }
    }
}

