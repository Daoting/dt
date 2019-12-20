#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Used to represent a cell range in the Excel. If you selected a range and provide a name for it. you can then use 
    /// that name in your formulas instead of using the raw range expression.
    /// in excel.
    /// </summary>
    public class NamedCellRange : IName
    {
        internal Tuple<int, byte[]> DefinitionBits;
        internal Tuple<int, byte[]> ExtraDefinitionBits;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.NamedCellRange" /> class.
        /// </summary>
        /// <param name="name">The name of the cell range</param>
        /// <param name="sheet">The index indicate the range scope, -1 means if a global cell range.</param>
        public NamedCellRange(string name, int sheet = -1)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            this.Name = name;
            this.Index = sheet;
        }

        /// <summary>
        /// Get or set the comment associated with this defined name
        /// </summary>
        public string Comment { get; internal set; }

        /// <summary>
        /// Get or set the zero-base index of Worksheet that the defined name belongs
        /// </summary>
        /// <remarks>
        /// If the value is -1, it means it's  workbook defined name (global name)
        /// </remarks>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets or sets the property which determines whether this defined name is hidden to the user.
        /// </summary>
        public bool IsHidden { get; internal set; }

        /// <summary>
        /// Get or set the name of the defined name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the formula which the defined name refers to.
        /// </summary>
        public string RefersTo { get; internal set; }

        /// <summary>
        /// Gets or sets the formula which the defined name refers to in R1C1 notation.
        /// </summary>
        public string RefersToR1C1 { get; internal set; }
    }
}

