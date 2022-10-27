namespace Dt.Core
{
    /// <summary>
    /// 为类型命名别名的基类标签
    /// </summary>
    public abstract class TypeAliasAttribute : Attribute
    {
        protected TypeAliasAttribute(string p_alias)
        {
            Alias = p_alias;
        }

        protected TypeAliasAttribute()
        { }

        /// <summary>
        /// 类型别名
        /// </summary>
        public string Alias { get; }
    }

    /// <summary>
    /// 视图类型的别名标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewAttribute : TypeAliasAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_alias">别名</param>
        public ViewAttribute(string p_alias)
            : base(p_alias)
        {
        }
    }
}
