#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-01-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 排序描述类
    /// </summary>
    public class SortDescription
    {
        public SortDescription()
        {
        }

        public SortDescription(string p_propertyName, ListSortDirection p_direction)
        {
            ID = p_propertyName;
            Direction = p_direction;
        }

        /// <summary>
        /// 排序列名
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 排序方向
        /// </summary>
        public ListSortDirection Direction { get; set; }
    }

    public class GroupDataList : KeyedCollection<string, GroupData<object>>
    {
        /// <summary>
        /// 分组值作为键值
        /// </summary>
        /// <param name="p_item">当前数据项</param>
        /// <returns></returns>
        protected override string GetKeyForItem(GroupData<object> p_item)
        {
            if (p_item != null)
                return p_item.Title;
            throw new Exception("列表中不可插入空的分组！");
        }
    }
}
