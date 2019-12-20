#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent the collection of <see cref="T:Dt.Xls.IExcelWorksheet" /> instances.
    /// </summary>
    public class ExcelWorksheetCollection : IEnumerable<IExcelWorksheet>, IEnumerable
    {
        private IExcelWorkbook _owner;
        private List<IExcelWorksheet> _sheets = new List<IExcelWorksheet>();

        /// <summary>
        /// Initialize a new <see cref="T:Dt.Xls.ExcelWorksheetCollection" /> with specified <see cref="T:Dt.Xls.IExcelWorkbook" /> owner.
        /// </summary>
        /// <param name="owner"></param>
        public ExcelWorksheetCollection(IExcelWorkbook owner)
        {
            this._owner = owner;
        }

        /// <summary>
        /// Adds an <see cref="T:Dt.Xls.IExcelWorksheet" /> to the end of the <see cref="T:Dt.Xls.ExcelWorksheetCollection" />.
        /// </summary>
        /// <param name="worksheet">
        /// The  <see cref="T:Dt.Xls.IExcelWorksheet" /> will be added to the collection
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="worksheet" /> is <see langword="null" /></exception>
        public void Add(IExcelWorksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("worksheet");
            }
            worksheet.ExtendedFormats = this._owner.ExcelCellFormats;
            this._sheets.Add(worksheet);
        }

        /// <summary>
        /// Remove all ExcelWorksheet from the collection.
        /// </summary>
        public void Clear()
        {
            this._sheets.Clear();
        }

        /// <summary>
        /// return an enumerator that iterates through the underlying collection 
        /// </summary>
        /// <returns>a <see cref="T:System.Collections.Generic.List`1.Enumerator" /> for the collection</returns>
        public IEnumerator<IExcelWorksheet> GetEnumerator()
        {
            return (IEnumerator<IExcelWorksheet>) this._sheets.GetEnumerator();
        }

        /// <summary>
        /// searches for the specified object and returns the zero-based index of the first occurrence within the entire collection
        /// </summary>
        /// <param name="worksheet">The object to locate in the collection</param>
        /// <returns>The zero based index of the first occurrence of ExcelWorksheet within the entire collection, if found; otherwise, –1.</returns>
        public int IndexOf(IExcelWorksheet worksheet)
        {
            return this._sheets.IndexOf(worksheet);
        }

        /// <summary>
        /// Add an <see cref="T:Dt.Xls.IExcelWorksheet" /> instance at the specified index
        /// </summary>
        /// <param name="index">the zero based index at which item should be inserted.</param>
        /// <param name="worksheet">
        /// The  <see cref="T:Dt.Xls.IExcelWorksheet" /> will be added to the collection
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="worksheet" /> is <see langword="null" /></exception>
        public void Insert(int index, IExcelWorksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("worksheet");
            }
            worksheet.ExtendedFormats = this._owner.ExcelCellFormats;
            this._sheets.Insert(index, worksheet);
        }

        /// <summary>
        /// Removes the element at the specified index
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            this._sheets.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (IExcelWorksheet iteratorVariable0 in this._sheets)
            {
                yield return iteratorVariable0;
            }
        }

        /// <summary>
        /// Gets the number of elements actually contained in the collection
        /// </summary>
        public int Count
        {
            get { return  this._sheets.Count; }
        }

        /// <summary>
        /// Get the item at the specified index
        /// </summary>
        /// <param name="index">&gt;the zero based index of the item to get</param>
        /// <returns></returns>
        public IExcelWorksheet this[int index]
        {
            get { return  this._sheets[index]; }
        }
    }
}

