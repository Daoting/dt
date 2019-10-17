#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 实体对象的缓存方式
    /// </summary>
    public enum EntityCacheMode
    {
        /// <summary>
        /// 不缓存
        /// </summary>
        None,

        /// <summary>
        /// 本地内存缓存
        /// </summary>
        Memory,

        /// <summary>
        /// 缓存在全局Redis中
        /// </summary>
        Redis
    }
}
