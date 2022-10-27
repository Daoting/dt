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
    /// 为类型命名别名的基类标签
    /// </summary>
    public abstract class TypeAliasAttribute : Attribute
    {
        protected TypeAliasAttribute(string p_alias)
        {
            Alias = p_alias;
        }

        protected TypeAliasAttribute()
        { }

        /// <summary>
        /// 类型别名
        /// </summary>
        public string Alias { get; }
    }
}
