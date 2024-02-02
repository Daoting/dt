#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 共享类型的别名标签，被标记的所有类型都会被记录在全局字典 Stub._aliasTypes 中
    /// 用在两个无引用关系的dll之间的互相访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ShareAttribute : TypeAliasAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_alias">别名，推荐 string Enum 类型</param>
        public ShareAttribute(object p_alias)
            : base(p_alias == null ? null : p_alias.ToString())
        {
        }

        public ShareAttribute() { }
    }
}
