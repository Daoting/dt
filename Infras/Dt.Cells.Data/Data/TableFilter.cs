#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.ComponentModel;
#endregion

namespace Dt.Cells.Data
{
    internal class TableFilter : HideRowFilter
    {
        SheetTable table;

        /// <summary>
        /// This method is reserved only for XML serialization, 
        /// Do Not use it in any other cases.
        /// </summary>
        [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
        public TableFilter()
        {
        }

        public TableFilter(SheetTable owner)
        {
            this.table = owner;
        }

        protected override void OnAddRows(int row, int count)
        {
            base.OnAddRows(row, count);
            base.SetRangeInternal(this.table.DataRange);
        }

        protected override void OnRemoveRows(int row, int count)
        {
            base.OnRemoveRows(row, count);
            base.SetRangeInternal(this.table.DataRange);
        }

        public override CellRange Range
        {
            get { return  base.Range; }
            set { throw new NotSupportedException(ResourceStrings.TableResizeRangeError); }
        }

        internal SheetTable Table
        {
            get { return  this.table; }
            set
            {
                if (this.table != value)
                {
                    this.table = value;
                }
            }
        }
    }
}

