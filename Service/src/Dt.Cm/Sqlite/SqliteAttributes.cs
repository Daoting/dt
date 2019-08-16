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
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public TableAttribute()
        { }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 标识状态库类型表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StateTableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute
    {
    }

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

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : IndexedAttribute
    {
        public override bool Unique
        {
            get { return true; }
            set { /* throw?  */ }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLengthAttribute : Attribute
    {
        public int Value { get; set; }

        public MaxLengthAttribute(int length)
        {
            Value = length;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CollationAttribute : Attribute
    {
        public string Value { get; set; }

        public CollationAttribute(string collation)
        {
            Value = collation;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultAttribute : Attribute
    {
        public DefaultAttribute(object value = null)
        {
            Value = value;
        }

        public object Value { get; set; }
    }
}
