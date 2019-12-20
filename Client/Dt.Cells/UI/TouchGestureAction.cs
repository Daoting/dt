#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    internal class TouchGestureAction : GestureAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UI.TouchGestureAction" /> class.
        /// </summary>
        /// <param name="gestureElement">The UI element.</param>
        public TouchGestureAction(UIElement gestureElement) : base(gestureElement)
        {
        }

        /// <summary>
        /// Gets the implementation-specific move-threshold.
        /// </summary>
        /// <value>The move threshold.</value>
        protected override double MoveThreshold
        {
            get { return  1.0; }
        }
    }
}

