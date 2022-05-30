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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewportFloatingObjectHitTestInformation
    {
        /// <summary>
        /// Creates a new set of viewport hit test information.
        /// </summary>
        internal ViewportFloatingObjectHitTestInformation()
        {
        }

        /// <summary>
        /// Gets the spread chart.
        /// </summary>
        public Dt.Cells.Data.FloatingObject FloatingObject { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether to resize the chart shape in the bottom NESW area.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in bottom NESW resize chart shape]; otherwise, <c>false</c>.
        /// </value>
        public bool InBottomNESWResize { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether to resize the chart shape in the bottom NS area.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in bottom NS resize chart shape]; otherwise, <c>false</c>.
        /// </value>
        public bool InBottomNSResize { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether to resize the chart shape area in the bottom NWSE area.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in bottom NWSE resize chart shape]; otherwise, <c>false</c>.
        /// </value>
        public bool InBottomNWSEResize { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether to resize the chart shape in the left WE area.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in left WE resize chart shape]; otherwise, <c>false</c>.
        /// </value>
        public bool InLeftWEResize { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether the chart shape is being moved.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in moving chart shape]; otherwise, <c>false</c>.
        /// </value>
        public bool InMoving { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether to resize the chart shape in the right WE area.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in right WE resize chart shape]; otherwise, <c>false</c>.
        /// </value>
        public bool InRightWEResize { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether to resize the chart shape in the top NESW area.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in top NESW resize chart shape]; otherwise, <c>false</c>.
        /// </value>
        public bool InTopNESWResize { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether to resize the chart shape in the top NS area.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in top NS resize chart shape]; otherwise, <c>false</c>.
        /// </value>
        public bool InTopNSResize { get; internal set; }

        /// <summary>
        /// Gets a value that indicates whether to resize the chart shape in the top NWSE area.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in top NWSE resize chart shape]; otherwise, <c>false</c>.
        /// </value>
        public bool InTopNWSEResize { get; internal set; }
    }
}

