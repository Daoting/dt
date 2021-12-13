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
    /// Represents the event data for the DragFillBlockCompleted event for the GcSpreadSheet component; occurs when the dragging and filling of a range of cells is completed. 
    /// </summary>
    public class DragFillBlockCompletedEventArgs : EventArgs
    {
        internal DragFillBlockCompletedEventArgs(CellRange fillRange, Dt.Cells.Data.FillDirection fillDirection, Dt.Cells.Data.AutoFillType autoFillType)
        {
            FillRange = fillRange;
            AutoFillType = autoFillType;
            FillDirection = fillDirection;
        }

        /// <summary>
        /// Gets the AutoFillType value used for the fill operation.  
        /// </summary>
        /// <value>The AutoFillType value used for the fill operation.</value>
        public Dt.Cells.Data.AutoFillType AutoFillType { get; private set; }

        /// <summary>
        /// Gets the direction of the fill.  
        /// </summary>
        /// <value>The direction of the fill.</value>
        public Dt.Cells.Data.FillDirection FillDirection { get; private set; }

        /// <summary>
        /// Gets the range used for the fill operation.  
        /// </summary>
        /// <value>The range used for the fill operation.</value>
        public CellRange FillRange { get; private set; }
    }
}

