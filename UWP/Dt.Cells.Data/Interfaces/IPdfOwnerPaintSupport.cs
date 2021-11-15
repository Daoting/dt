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
    /// Interface that supports printing to PDF.
    /// </summary>
    internal interface IPdfOwnerPaintSupport
    {
        /// <summary>
        /// Paints (prints) the cell to PDF when not in edit mode.
        /// </summary>
        /// <param name="g">PDF graphics device interface</param>
        /// <param name="rect">Bounding rectangle to paint</param>
        /// <param name="styleInfo">StyleInfo of the cell</param>
        /// <param name="value">The value to paint.</param>
        void PaintCell(Graphics g, Windows.Foundation.Rect rect, StyleInfo styleInfo, object value);
    }
}

