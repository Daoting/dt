#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents an adapter class for converting between view and model index.
    /// </summary>
    internal abstract class SortedIndexAdapter
    {
        protected SortedIndexAdapter()
        {
        }

        /// <summary>
        /// Gets the model index from the view index.
        /// </summary>
        /// <param name="index">The view index.</param>
        /// <returns></returns>
        public abstract int GetModelIndexFromViewIndex(int index);
        /// <summary>
        /// Gets the view index from the model index.
        /// </summary>
        /// <param name="index">The model index.</param>
        /// <returns></returns>
        public abstract int GetViewIndexFromModelIndex(int index);

        /// <summary>
        /// Gets a value that indicates whether these items are sorted.
        /// </summary>
        /// <value><c>true</c> if items are sorted; otherwise, <c>false</c></value>
        public abstract bool IsSorted { get; }

        /// <summary>
        /// Represents an adapter class for converting between view and model index.
        /// </summary>
        internal class SortedColumnIndexAdapter : SortedIndexAdapter
        {
            Worksheet worksheet;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SortedIndexAdapter.SortedColumnIndexAdapter" /> class.
            /// </summary>
            /// <param name="worksheet">The sheet.</param>
            public SortedColumnIndexAdapter(Worksheet worksheet)
            {
                if (worksheet == null)
                {
                    throw new ArgumentException("sheet");
                }
                this.worksheet = worksheet;
            }

            /// <summary>
            /// Gets the model index from the view index.
            /// </summary>
            /// <param name="index">The view index.</param>
            /// <returns></returns>
            public override int GetModelIndexFromViewIndex(int index)
            {
                return this.worksheet.GetModelColumnFromViewColumn(index);
            }

            /// <summary>
            /// Gets the view index from the model index.
            /// </summary>
            /// <param name="index">The model index.</param>
            /// <returns></returns>
            public override int GetViewIndexFromModelIndex(int index)
            {
                return this.worksheet.GetViewColumnFromModelColumn(index);
            }

            /// <summary>
            /// Gets or sets a value that indicates whether the items are sorted.
            /// </summary>
            /// <value><c>true</c> if items are sorted; otherwise, <c>false</c>.</value>
            public override bool IsSorted
            {
                get { return  this.worksheet.IsColumnSorted; }
            }
        }

        /// <summary>
        /// Represents an adapter class for converting between view and model index.
        /// </summary>
        internal class SortedRowIndexAdapter : SortedIndexAdapter
        {
            Worksheet worksheet;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SortedIndexAdapter.SortedColumnIndexAdapter" /> class.
            /// </summary>
            /// <param name="worksheet">The sheet.</param>
            public SortedRowIndexAdapter(Worksheet worksheet)
            {
                if (worksheet == null)
                {
                    throw new ArgumentException("sheet");
                }
                this.worksheet = worksheet;
            }

            /// <summary>
            /// Gets the model index from the view index.
            /// </summary>
            /// <param name="index">The view index.</param>
            /// <returns></returns>
            public override int GetModelIndexFromViewIndex(int index)
            {
                return this.worksheet.GetModelRowFromViewRow(index);
            }

            /// <summary>
            /// Gets the view index from the model index.
            /// </summary>
            /// <param name="index">The model index.</param>
            /// <returns></returns>
            public override int GetViewIndexFromModelIndex(int index)
            {
                return this.worksheet.GetViewRowFromModelRow(index);
            }

            /// <summary>
            /// Gets a value that indicates whether the items are sorted.
            /// </summary>
            /// <value><c>true</c> if items are sorted; otherwise, <c>false</c>.</value>
            public override bool IsSorted
            {
                get { return  this.worksheet.IsRowSorted; }
            }
        }
    }
}

