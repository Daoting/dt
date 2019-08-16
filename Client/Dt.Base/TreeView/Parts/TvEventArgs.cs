#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-15 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 加载子节点事件参数
    /// </summary>
    public class LoadingChildArgs : AsyncEventArgs
    {
        public LoadingChildArgs(TvItem p_item)
        {
            CurrentItem = p_item;
        }

        /// <summary>
        /// 当前节点
        /// </summary>
        public TvItem CurrentItem { get; }

        /// <summary>
        /// 获取设置子节点集合
        /// </summary>
        public IEnumerable<object> Children { get; set; }
    }
}
