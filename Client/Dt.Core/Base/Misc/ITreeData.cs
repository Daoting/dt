#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 树状数据接口
    /// </summary>
    public interface ITreeData
    {
        /// <summary>
        /// 获取树根节点数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> GetTreeRoot();

        /// <summary>
        /// 获取指定节点的子节点数据
        /// </summary>
        /// <param name="p_parent"></param>
        /// <returns></returns>
        IEnumerable<object> GetTreeItemChildren(object p_parent);
    }
}