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
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CustomFloatingObject : FloatingObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CustomFloatingObject" /> class.
        /// </summary>
        public CustomFloatingObject() : base(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CustomFloatingObject" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CustomFloatingObject(string name) : base(name, 0.0, 0.0, 200.0, 200.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CustomFloatingObject" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public CustomFloatingObject(string name, double x, double y, double width, double height) : base(name, x, y, width, height)
        {
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public abstract FrameworkElement Content { get; }
    }
}

