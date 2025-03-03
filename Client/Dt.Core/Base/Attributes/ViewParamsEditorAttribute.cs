#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 视图参数编辑器的别名标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewParamsEditorAttribute : TypeAliasAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_alias">别名，推荐 string Enum 类型</param>
        public ViewParamsEditorAttribute(object p_alias)
            : base(p_alias == null ? null : p_alias.ToString())
        {
        }

        public ViewParamsEditorAttribute() { }
    }
}
