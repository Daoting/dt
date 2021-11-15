#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the invalid data.
    /// </summary>
    public partial class InvalidDataPresenter : Control
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:InvalidDataPresenter" /> class.
        /// </summary>
        public InvalidDataPresenter()
        {
            base.IsHitTestVisible = false;
            base.DefaultStyleKey = typeof(InvalidDataPresenter);
        }
    }
}

