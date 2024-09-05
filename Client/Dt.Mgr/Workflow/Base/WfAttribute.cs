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

namespace Dt.Mgr
{
    /// <summary>
    /// 流程表单的别名标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WfFormAttribute : TypeAliasAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_alias">流程名称</param>
        public WfFormAttribute(string p_alias)
            : base(p_alias)
        {
        }
    }
}
