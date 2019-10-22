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
        /// 是否将实体缓存在Redis中
        /// </summary>
        public bool IsCached { get; set; }

        /// <summary>
        /// 缓存在Redis中的键对应的属性名
        /// <para>默认ID</para>
        /// <para>缓存时需要多个属性名作为键值时属性名之间逗号隔开</para>
        /// </summary>
        public string CacheKey { get; set; }
    }
}
