#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides data for the PictureChanged event.
    /// </summary>
    public class PictureChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PictureChangedEventArgs" /> class.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <param name="property">The property.</param>
        public PictureChangedEventArgs(Dt.Cells.Data.Picture picture, string property)
        {
            this.Picture = picture;
            this.Property = property;
        }

        /// <summary>
        /// Gets the picture.
        /// </summary>
        /// <value>
        /// The picture.
        /// </value>
        public Dt.Cells.Data.Picture Picture { get; private set; }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public string Property { get; private set; }
    }
}

