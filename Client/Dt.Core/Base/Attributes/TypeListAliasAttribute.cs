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
    /// 类型别名标签，一个别名对应一个类型列表
    /// </summary>
    public abstract class TypeListAliasAttribute : Attribute
    {
        protected TypeListAliasAttribute(string p_alias)
        {
            Alias = p_alias;
        }

        protected TypeListAliasAttribute()
        { }

        /// <summary>
        /// 类型别名
        /// </summary>
        public string Alias { get; }
    }
}
