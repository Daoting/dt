#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 支持组合主键的实体基类
    /// </summary>
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// 输出组合主键
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[ENTITY: {GetType().Name}] Keys = {string.Join(", ", GetKeys())}";
        }

        public abstract object[] GetKeys();
    }

    /// <summary>
    /// 只包含"Id"主键的实体基类
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public abstract class Entity<TKey> : Entity, IEntity<TKey>
    {
        /// <summary>
        /// 实体的唯一主键
        /// </summary>
        public virtual TKey Id { get; set; }

        protected Entity()
        {
        }

        protected Entity(TKey id)
        {
            Id = id;
        }

        /// <summary>
        /// 判断两实体是否相同
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TKey>))
            {
                return false;
            }

            // 相同实例
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            // 两个临时对象(默认主键)不相同
            var other = (Entity<TKey>)obj;
            if (EntityHelper.HasDefaultId(this) && EntityHelper.HasDefaultId(other))
            {
                return false;
            }

            // 类型不同且无继承关系的不相同
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.IsAssignableFrom(typeOfOther) && !typeOfOther.IsAssignableFrom(typeOfThis))
            {
                return false;
            }
            return Id.Equals(other.Id);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            if (Id == null)
            {
                return 0;
            }

            return Id.GetHashCode();
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

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }

        /// <summary>
        /// 输出实体描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[ENTITY: {GetType().Name}] Id = {Id}";
        }
    }
}
