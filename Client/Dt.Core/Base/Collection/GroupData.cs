#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 分组数据源集合类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupData<T> : List<T>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public GroupData()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_title">组名</param>
        /// <param name="p_items">数据集合</param>
        public GroupData(string p_title, IEnumerable<T> p_items = null)
        {
            Title = p_title;
            if (p_items != null)
                AddRange(p_items);
        }

        /// <summary>
        /// 获取设置数据源集合的组名
        /// </summary>
        public string Title { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Title))
                return Title;
            return base.ToString();
        }
    }
}