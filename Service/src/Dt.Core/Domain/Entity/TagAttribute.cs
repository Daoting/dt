#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 实体类标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TagAttribute : Attribute
    {
        /// <summary>
        /// 实体类对应的表名
        /// </summary>
        public string TblName { get; set; }

        /// <summary>
        /// 实体对象的缓存方式
        /// </summary>
        public EntityCacheMode CacheMode { get; set; }
    }
}
