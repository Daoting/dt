#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent the data for supporting external workbook.
    /// </summary>
    internal class ExcelSupBook
    {
        /// <summary>
        /// 
        /// </summary>
        internal byte[] Buffer;
        /// <summary>
        /// 
        /// </summary>
        internal List<Tuple<int, byte[]>> ExternNameBuffers;
        /// <summary>
        /// Path and file name of the <see cref="T:Dt.Xls.ExcelSupBook" />.
        /// </summary>
        internal string FileName;
        /// <summary>
        /// Flag indicate whether the supbook used in add-in functions
        /// </summary>
        internal bool IsAddInReferencedSupBook;
        /// <summary>
        /// Flag indicate whether the supbook refers to the current worksheet
        /// </summary>
        internal bool IsCurrentSheetSupBook;
        /// <summary>
        /// Determines whether this is the current workbook.
        /// </summary>
        internal bool IsSelfReferenced;
        /// <summary>
        /// Number of sheets in the workbook.
        /// </summary>
        internal int SheetCount;
        /// <summary>
        /// Array list of sheet names in the workbook.
        /// </summary>
        internal List<string> SheetNames;

        /// <summary>
        /// Creates a <see cref="T:Dt.Xls.ExcelSupBook" />.
        /// </summary>
        internal ExcelSupBook()
        {
            this.SheetNames = new List<string>();
        }

        /// <summary>
        /// Creates a new <see cref="T:Dt.Xls.ExcelSupBook" /> and specifies whether it is the current workbook.
        /// </summary>
        /// <param name="isSelfReferenced">Whether it is the current workbook</param>
        internal ExcelSupBook(bool isSelfReferenced)
        {
            this.SheetNames = new List<string>();
            this.IsSelfReferenced = isSelfReferenced;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int num = ((this.IsAddInReferencedSupBook.GetHashCode() ^ (this.IsCurrentSheetSupBook.GetHashCode() << 4)) ^ (this.IsSelfReferenced.GetHashCode() << 8)) ^ (((int) this.SheetCount).GetHashCode() << 0x10);
            if (this.FileName != null)
            {
                num ^= this.FileName.GetHashCode() << 0x10;
            }
            return num;
        }
    }
}

