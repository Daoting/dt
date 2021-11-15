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

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// 类型和sqlite表的映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SqliteAttribute : Attribute
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_dbName">sqlite库文件名，不包括扩展名</param>
        public SqliteAttribute(string p_dbName)
        {
            Throw.IfNullOrEmpty(p_dbName);
            DbName = p_dbName.ToLower();
        }

        /// <summary>
        /// sqlite库文件名，小写不包括扩展名
        /// </summary>
        public string DbName { get; }
    }

    /// <summary>
    /// 主键标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    { }

    /// <summary>
    /// 字段自增标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute
    { }

    /// <summary>
    /// 索引列标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexedAttribute : Attribute
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public virtual bool Unique { get; set; }

        public IndexedAttribute()
        {
        }

        public IndexedAttribute(string name, int order)
        {
            Name = name;
            Order = order;
        }
    }

    /// <summary>
    /// 此属性不映射字段标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    { }
}
