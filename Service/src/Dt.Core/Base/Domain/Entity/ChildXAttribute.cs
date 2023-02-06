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
    /// 子实体类标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ChildXAttribute : Attribute
    {
        public ChildXAttribute(string p_parentID)
        {
            ParentID = p_parentID;
        }

        /// <summary>
        /// 父表外键字段名
        /// </summary>
        public string ParentID { get; }
    }
}
