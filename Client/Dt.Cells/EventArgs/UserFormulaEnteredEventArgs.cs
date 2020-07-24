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
    /// Represents the event data for the UserFormulaEntered event for the GcSpreadSheet component; occurs when the user has entered a formula in a cell. 
    /// </summary>
    public class UserFormulaEnteredEventArgs : EventArgs
    {
        internal UserFormulaEnteredEventArgs(int row, int column, string formula)
        {
            Row = row;
            Column = column;
            Formula = formula;
        }

        /// <summary>
        /// Gets the column index of the cell in which the user entered a formula.
        /// </summary>
        /// <value>The column index of the cell in which the user entered a formula.</value>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the formula that the user entered.
        /// </summary>
        /// <value>The formula that the user entered.</value>
        public string Formula { get; private set; }

        /// <summary>
        /// Gets the row index of the cell in which the user entered a formula. 
        /// </summary>
        /// <value>The row index of the cell in which the user entered a formula.</value>
        public int Row { get; private set; }
    }
}

