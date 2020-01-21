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
    /// 内部维护数据项列表类
    /// </summary>
#if !SERVER
    [Windows.UI.Xaml.Data.Bindable]
#endif
    public class CellList : KeyedCollection<string, Cell>
    {
        /// <summary>
        /// 构造方法，键比较时忽略大小写
        /// </summary>
        public CellList()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// 通过列名获取数据项
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <returns></returns>
        new public Cell this[string p_colName]
        {
            get
            {
                if (string.IsNullOrEmpty(p_colName))
                    throw new Exception("Row中列名不可为空！");
                Cell cell;
                if (Dictionary.TryGetValue(p_colName, out cell))
                    return cell;
                throw new Exception($"Row中缺少【{p_colName}】列！");
            }
        }

        /// <summary>
        /// 屏蔽外部直接添加
        /// </summary>
        /// <param name="p_cell"></param>
        new internal void Add(Cell p_cell)
        {
            base.Add(p_cell);
        }

        /// <summary>
        /// 屏蔽外部直接删除
        /// </summary>
        /// <param name="p_name"></param>
        new internal bool Remove(string p_name)
        {
            return base.Remove(p_name);
        }

        /// <summary>
        /// 屏蔽外部直接删除
        /// </summary>
        /// <param name="p_cell"></param>
        new internal bool Remove(Cell p_cell)
        {
            return base.Remove(p_cell);
        }

        /// <summary>
        /// 屏蔽外部直接删除
        /// </summary>
        /// <param name="p_index"></param>
        new internal void RemoveAt(int p_index)
        {
            if (p_index >= 0 && p_index < Count)
            {
                base.RemoveAt(p_index);
            }
        }

        /// <summary>
        /// 列名作为键值
        /// </summary>
        /// <param name="p_item">当前数据项</param>
        /// <returns></returns>
        protected override string GetKeyForItem(Cell p_item)
        {
            if (p_item != null && !string.IsNullOrEmpty(p_item.ID))
                return p_item.ID;
            throw new Exception("数据项列表中不可插入为空或无字段名的数据项！");
        }
    }
}
