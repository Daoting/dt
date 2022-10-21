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
    /// 基类属性
    /// </summary>
    public abstract class AliasAttribute : Attribute
    {
        string _alias;

        /// <summary>
        /// 属性名称
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            protected set { _alias = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_alias">名称</param>
        protected AliasAttribute(string p_alias)
        {
            _alias = p_alias;
        }
    }
}
