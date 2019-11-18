#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 包含"ID"主键的实体基类
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        /// <summary>
        /// 实体的唯一主键
        /// </summary>
        public virtual TKey ID { get; set; }

        protected Entity()
        { }

        protected Entity(TKey p_id)
        {
            ID = p_id;
        }

        /// <summary>
        /// 判断两实体是否相同
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // 比较的对象为 NULL 或者对象不是派生自 Entity<T> 都视为不相等
            if (obj == null || !(obj is Entity<TKey>))
            {
                return false;
            }

            // 相同实例
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            // 类型不同且无继承关系的不相同
            var other = (Entity<TKey>)obj;
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.IsAssignableFrom(typeOfOther) && !typeOfOther.IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            // 通过泛型的 Equals 方法进行最后的比较
            return ID.Equals(other.ID);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            if (ID == null)
            {
                return 0;
            }

            return ID.GetHashCode();
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// 输出实体描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[Entity: {GetType().Name}] ID = {ID}";
        }
    }

    /// <summary>
    /// 主键ID为long类型的实体基类
    /// </summary>
    public abstract class Entity : Entity<long>
    { }
}
