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
    /// 为类型命名别名的基类标签，一个别名对应一个类型
    /// 别名可以为任意字符串，也可以为空，空时别名为类型名(不包括命名空间)
    /// </summary>
    public abstract class TypeAliasAttribute : Attribute
    {
        protected TypeAliasAttribute(string p_alias)
        {
            Alias = p_alias;
        }

        protected TypeAliasAttribute(Enum p_alias)
        {
            Alias = p_alias.ToString();
        }

        protected TypeAliasAttribute()
        { }

        /// <summary>
        /// 类型别名
        /// </summary>
        public string Alias { get; }
    }
}
