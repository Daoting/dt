#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据表中的列集合，可以按索引或列名获取Column对象
    /// </summary>
    public class ColumnList : KeyedCollection<string, Column>
    {
        Table _owner;

        /// <summary>
        /// 构造方法，键比较时忽略大小写
        /// </summary>
        public ColumnList(Table p_owner)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            _owner = p_owner;
        }

        /// <summary>
        /// 添加列，同步到所有Row
        /// </summary>
        /// <param name="p_col">列</param>
        new public void Add(Column p_col)
        {
            base.Add(p_col);
            if (_owner.Count > 0)
            {
                foreach (var row in _owner)
                {
                    new Cell(row, p_col.ID, p_col.Type);
                }
            }
        }

        /// <summary>
        /// 删除列，同步到所有Row
        /// </summary>
        /// <param name="p_colName"></param>
        /// <returns></returns>
        new public bool Remove(string p_colName)
        {
            bool success = false;
            if (this.Contains(p_colName))
            {
                success = base.Remove(p_colName);
                RemoveColumnData(p_colName);
            }
            return success;
        }

        /// <summary>
        /// 删除列，同步到所有Row
        /// </summary>
        /// <param name="p_col"></param>
        /// <returns></returns>
        new public bool Remove(Column p_col)
        {
            bool success = false;
            if (this.Contains(p_col))
            {
                success = base.Remove(p_col);
                RemoveColumnData(p_col.ID);
            }
            return success;
        }

        /// <summary>
        /// 删除列，同步到Row
        /// </summary>
        /// <param name="p_index"></param>
        new public void RemoveAt(int p_index)
        {
            if (p_index < 0 || p_index >= Count)
                return;

            Column col = this[p_index];
            base.RemoveAt(p_index);
            RemoveColumnData(col.ID);
        }

        /// <summary>
        /// 删除列数据
        /// </summary>
        /// <param name="p_colName"></param>
        void RemoveColumnData(string p_colName)
        {
            if (_owner.Count > 0)
            {
                foreach (var row in _owner)
                {
                    row.Cells.Remove(p_colName);
                }
            }
        }

        /// <summary>
        /// 根据数据列获得列字段名
        /// </summary>
        /// <param name="item">数据列</param>
        /// <returns>列字段名</returns>
        protected override string GetKeyForItem(Column item)
        {
            return item.ID;
        }
    }
}
