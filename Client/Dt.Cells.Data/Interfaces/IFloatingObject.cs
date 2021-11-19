#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the interface for the floating object.
    /// </summary>
    public interface IFloatingObject
    {
        /// <summary>
        /// Gets or sets a value that indicates whether this floating object can print.
        /// </summary>
        /// <value>
        /// <c>true</c> if this floating object can print; otherwise, <c>false</c>.
        /// </value>
        bool CanPrint { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this floating object is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this floating object is selected; otherwise, <c>false</c>.
        /// </value>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the location of a floating object.
        /// </summary>
        /// <value>
        /// The location of a floating object.
        /// </value>
        Windows.Foundation.Point Location { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this floating object is locked.
        /// </summary>
        /// <value>
        /// <c>true</c> if locked; otherwise, <c>false</c>.
        /// </value>
        bool Locked { get; set; }

        /// <summary>
        /// Gets the name of the floating object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the size of a floating object.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        Windows.Foundation.Size Size { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this floating object is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        bool Visible { get; set; }
    }
}

