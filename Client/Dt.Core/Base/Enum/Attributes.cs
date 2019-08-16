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

    /// <summary>
    /// 视图类属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ViewAttribute : AliasAttribute
    {
        /// <summary>
        /// 构造视图类属性
        /// </summary>
        /// <param name="p_alias">名称</param>
        public ViewAttribute(string p_alias)
            : base(p_alias)
        {
        }
    }

    /// <summary>
    /// 流程表单类属性
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple=false)]
    public class WfFormAttribute : AliasAttribute
    {
        /// <summary>
        /// 构造脚本类属性
        /// </summary>
        /// <param name="p_alias">名称</param>
        public WfFormAttribute(string p_alias)
            : base(p_alias)
        {
        }
    }

    /// <summary>
    /// 流程业务数据列表类属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WfSheetAttribute : AliasAttribute
    {
        /// <summary>
        /// 构造自定义查找类属性
        /// </summary>
        /// <param name="p_alias">名称</param>
        public WfSheetAttribute(string p_alias)
            : base(p_alias)
        {
        }
    }

    /// <summary>
    /// 可序列化类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class JsonObjAttribute : AliasAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Type Type { get; internal set; }

        /// <summary>
        /// 构造可序列化类型
        /// </summary>
        /// <param name="p_alias">名称</param>
        public JsonObjAttribute(string p_alias)
            : base(p_alias)
        {
        }
    }

    /// <summary>
    /// rpc时需要序列化属性的标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RpcMemberAttribute : AliasAttribute
    {
        /// <summary>
        /// 构造rpc类型属性
        /// </summary>
        /// <param name="p_alias">名称</param>
        public RpcMemberAttribute(string p_alias)
            : base(p_alias)
        {
        }
    }

    /// <summary>
    /// 可远程调用的类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RpcClassAttribute : Attribute
    {
    }

    /// <summary>
    /// 可远程调用方法的标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RpcMethodAttribute : Attribute
    {
    }

    /// <summary>
    /// 忽略标志，Sqlite.Net中为IgnoreAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoredAttribute : Attribute
    {
    }
}
