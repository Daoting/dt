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
    /// 实体类映射表标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
#if SERVER
    public class TblAttribute : Attribute
    {
        public TblAttribute(string p_tblName)
#else
    public class TblAttribute : TypeAliasAttribute
    {
        public TblAttribute(string p_tblName)
            : base(p_tblName)
#endif
        {
            Name = p_tblName;
        }

        /// <summary>
        /// 实体类对应的表名
        /// </summary>
        public string Name { get; }
    }

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

    /// <summary>
    /// 定义实体视图名的标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityViewAttribute : Attribute
    {
        public EntityViewAttribute(string p_viewName)
        {
            Name = p_viewName;
        }

        /// <summary>
        /// 视图名
        /// </summary>
        public string Name { get; }
    }

    /// <summary>
    /// 1号视图标签
    /// </summary>
    public class View1Attribute : EntityViewAttribute
    {
        public View1Attribute(string p_viewName)
            :base(p_viewName)
        { }
    }

    /// <summary>
    /// 2号视图标签
    /// </summary>
    public class View2Attribute : EntityViewAttribute
    {
        public View2Attribute(string p_viewName)
            : base(p_viewName)
        { }
    }

    /// <summary>
    /// 3号视图标签
    /// </summary>
    public class View3Attribute : EntityViewAttribute
    {
        public View3Attribute(string p_viewName)
            : base(p_viewName)
        { }
    }

    /// <summary>
    /// 4号视图标签
    /// </summary>
    public class View4Attribute : EntityViewAttribute
    {
        public View4Attribute(string p_viewName)
            : base(p_viewName)
        { }
    }

    /// <summary>
    /// 5号视图标签
    /// </summary>
    public class View5Attribute : EntityViewAttribute
    {
        public View5Attribute(string p_viewName)
            : base(p_viewName)
        { }
    }
}