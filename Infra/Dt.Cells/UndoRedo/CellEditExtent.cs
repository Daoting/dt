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
using System.Text;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents a cell edit action extent that supports editing a cell on the sheet.
    /// </summary>
    public class CellEditExtent
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.CellEditExtent" /> class.
        /// </summary>
        /// <param name="row">The edit row index.</param>
        /// <param name="column">The edit column index.</param>
        /// <param name="newValue">The edit cell's new value to apply.</param>
        public CellEditExtent(int row, int column, string newValue)
        {
            RowIndex = row;
            ColumnIndex = column;
            NewValue = newValue;
        }

        string GetCoord(int row, int column)
        {
            return (IndexToLetters(column) + ((int) row));
        }

        string IndexToLetters(int index)
        {
            StringBuilder builder = new StringBuilder();
            while (index > 0)
            {
                builder.Append((char) (0x41 + ((index - 1) % 0x1a)));
                index = (index - 1) / 0x1a;
            }
            for (int i = 0; i < (builder.Length / 2); i++)
            {
                char ch = builder[i];
                builder[i] = builder[(builder.Length - i) - 1];
                builder[(builder.Length - i) - 1] = ch;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (NewValue != null)
            {
                return string.Concat((string[]) new string[] { string.Format(ResourceStrings.undoActionTypingInCell, (object[]) new object[] { NewValue.ToString(), GetCoord(RowIndex + 1, ColumnIndex + 1) }) });
            }
            return ResourceStrings.undoActionEditingCell;
        }

        /// <summary>
        /// Gets the edit cell column index.
        /// </summary>
        public int ColumnIndex { get; private set; }

        /// <summary>
        /// Gets the new value of the edit cell.
        /// </summary>
        public string NewValue { get; private set; }

        /// <summary>
        /// Gets the edit cell row index.
        /// </summary>
        public int RowIndex { get; private set; }
    }
}

