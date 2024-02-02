#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.ComponentModel;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the information related to sorting columns, 
    /// rows, or a range of cells.
    /// </summary>
    public class SortInfo
    {
        /// <summary>
        /// The ascending.
        /// </summary>
        bool ascending;
        /// <summary>
        /// The comparer.
        /// </summary>
        IComparer comparer;
        /// <summary>
        /// The sort index.
        /// </summary>
        int index;

        /// <summary>
        /// Creates a <see cref="T:Dt.Cells.Data.SortInfo" /> object with the specified order that uses the system default comparer for the 
        /// comparison.
        /// </summary>
        /// <param name="index">The index of the column or row on which to sort.</param>
        /// <param name="ascending">If set to <c>true</c>, sort order is ascending.</param>
        public SortInfo(int index, bool ascending)
        {
            this.index = index;
            this.ascending = ascending;
            this.comparer = null;
        }

        /// <summary>
        /// Creates a <see cref="T:Dt.Cells.Data.SortInfo" /> object with the specified order and comparison that uses 
        /// a specified comparer.
        /// </summary>
        /// <param name="index">The index of the column or row on which to sort.</param>
        /// <param name="ascending">If set to <c>true</c>, sort order is ascending.</param>
        /// <param name="comparer">An <see cref="T:System.Collections.IComparer" /> object for the method of comparison.</param>
        public SortInfo(int index, bool ascending, IComparer comparer)
        {
            this.index = index;
            this.ascending = ascending;
            this.comparer = comparer;
        }

        /// <summary>
        /// Determines whether the specified condition is equal to the current SortInfo object.
        /// </summary>
        /// <param name="obj">The SortInfo to compare with the current SortInfo object.</param>
        /// <returns>
        /// True if the specified SortInfo is equal to the current SortInfo object; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            SortInfo info = obj as SortInfo;
            if (info == null)
            {
                return false;
            }
            if (info.ascending != this.ascending)
            {
                return false;
            }
            if (info.index != this.index)
            {
                return false;
            }
            if (!object.Equals(info.comparer, this.comparer))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the sort order is ascending.
        /// </summary>
        /// <value><c>true</c> if the sort order is ascending; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Ascending
        {
            get { return  this.ascending; }
            set { this.ascending = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Collections.IComparer" /> object for the method of comparison.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.IComparer" /> object for the method of comparison.</value>
        [DefaultValue((string) null)]
        public IComparer Comparer
        {
            get { return  this.comparer; }
            set { this.comparer = value; }
        }

        /// <summary>
        /// Gets or sets the index of the column or row on which to sort.
        /// </summary>
        /// <value>The index of the column or row on which to sort.</value>
        [DefaultValue(-1)]
        public int Index
        {
            get { return  this.index; }
            set { this.index = value; }
        }
    }
}

