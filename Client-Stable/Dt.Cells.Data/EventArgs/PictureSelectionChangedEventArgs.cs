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
    /// Provides data for the PictureSelectionChanged event.
    /// </summary>
    public class PictureSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PictureSelectionChangedEventArgs" /> class.
        /// </summary>
        /// <param name="picture">The picture.</param>
        public PictureSelectionChangedEventArgs(Dt.Cells.Data.Picture picture)
        {
            this.Picture = picture;
        }

        /// <summary>
        /// Gets the picture.
        /// </summary>
        /// <value>
        /// The picture.
        /// </value>
        public Dt.Cells.Data.Picture Picture { get; private set; }
    }
}

