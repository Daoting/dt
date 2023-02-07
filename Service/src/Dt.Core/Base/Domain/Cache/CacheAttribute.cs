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
        /// 构造方法
        /// </summary>
        /// <param name="p_keys">除主键以外其它作为缓存键的属性名(字段名)，该列通常为唯一索引</param>
        public CacheAttribute(params string[] p_keys)
        {
            Keys = p_keys;
        }

        /// <summary>
        /// 除主键以外其它作为缓存键的属性名，该列通常为唯一索引，确保该列数据不重复
        /// </summary>
        public string[] Keys { get; }
    }
}
