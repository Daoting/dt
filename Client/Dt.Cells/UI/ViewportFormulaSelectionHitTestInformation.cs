#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewportFormulaSelectionHitTestInformation
    {
        internal bool CanMove
        {
            get
            {
                if (((Position != PositionInFormulaSelection.Left) && (Position != PositionInFormulaSelection.Right)) && (Position != PositionInFormulaSelection.Top))
                {
                    return (Position == PositionInFormulaSelection.Bottom);
                }
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PositionInFormulaSelection Position { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public int SelectionIndex { get; internal set; }
    }
}

