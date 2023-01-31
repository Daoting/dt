#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-19 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// 含两个一对一实体的虚拟实体
    /// </summary>
    /// <typeparam name="TEntity1"></typeparam>
    /// <typeparam name="TEntity2"></typeparam>
    public class VirObj<TEntity1, TEntity2> : EntityX<VirObj<TEntity1, TEntity2>>, IVirEntity
        where TEntity1 : Entity
        where TEntity2 : Entity
    {
        public VirObj()
        {
            E1 = To<TEntity1>();
            E2 = To<TEntity2>();
        }

        /// <summary>
        /// 第一个实体对象
        /// </summary>
        public TEntity1 E1 { get; }

        /// <summary>
        /// 第二个实体对象
        /// </summary>
        public TEntity2 E2 { get; }

        public List<Entity> GetEntities()
        {
            return new List<Entity> { E1, E2 };
        }
    }

    /// <summary>
    /// 含三个一对一实体的虚拟实体
    /// </summary>
    /// <typeparam name="TEntity1"></typeparam>
    /// <typeparam name="TEntity2"></typeparam>
    /// <typeparam name="TEntity3"></typeparam>
    public class VirObj<TEntity1, TEntity2, TEntity3> : EntityX<VirObj<TEntity1, TEntity2, TEntity3>>, IVirEntity
        where TEntity1 : Entity
        where TEntity2 : Entity
        where TEntity3 : Entity
    {
        public VirObj()
        {
            E1 = To<TEntity1>();
            E2 = To<TEntity2>();
            E3 = To<TEntity3>();
        }

        /// <summary>
        /// 第一个实体对象
        /// </summary>
        public TEntity1 E1 { get; }

        /// <summary>
        /// 第二个实体对象
        /// </summary>
        public TEntity2 E2 { get; }

        /// <summary>
        /// 第三个实体对象
        /// </summary>
        public TEntity3 E3 { get; }

        public List<Entity> GetEntities()
        {
            return new List<Entity> { E1, E2, E3 };
        }
    }
}
