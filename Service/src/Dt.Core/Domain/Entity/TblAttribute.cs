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
    public class TblAttribute : Attribute
    {
        public TblAttribute(string p_tblName, string p_svcName)
        {
            Name = p_tblName;
            Svc = p_svcName;
        }

        /// <summary>
        /// 实体类对应的表名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 实体所属的服务，客户端用
        /// </summary>
        public string Svc { get; }
    }

    ///// <summary>
    ///// 子实体类标签
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    //public class ChildTblAttribute : Attribute
    //{
    //    public ChildTblAttribute(string p_parentID)
    //    {
    //        ParentID = p_parentID;
    //    }

    //    /// <summary>
    //    /// 父表外键字段名
    //    /// </summary>
    //    public string ParentID { get; }
    //}
}
