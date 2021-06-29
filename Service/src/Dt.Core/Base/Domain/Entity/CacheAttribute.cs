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

namespace Dt.Core
{
    /// <summary>
    /// 实体缓存配置标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CacheAttribute : Attribute
    {
        /// <summary>
        /// 缓存键前缀，不可为空
        /// </summary>
        public string PrefixKey { get; set; }

        /// <summary>
        /// 除主键以外其它作为缓存键的属性名，缓存多个时属性名之间逗号隔开，确保该列数据不重复
        /// </summary>
        public string OtherKey { get; set; }

        /// <summary>
        /// 缓存有效小时数，0表示始终有效
        /// </summary>
        public double ExpiryHour { get; set; }
    }
}
