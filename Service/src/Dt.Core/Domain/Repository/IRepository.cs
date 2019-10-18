#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 标志一个类型为仓库类
    /// </summary>
    public interface IRepository
    { }

    /// <summary>
    /// 通用实体类仓库接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> : IRepository
        where TEntity : class, IEntity
    {
        /// <summary>
        /// 插入实体对象
        /// </summary>
        /// <param name="p_entity">待插入的实体</param>
        /// <returns></returns>
        Task<bool> Insert(TEntity p_entity);

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="p_entity">实体</param>
        /// <returns></returns>
        Task<bool> Update(TEntity p_entity);

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="p_entity">待删除的实体</param>
        /// <returns>true 删除成功</returns>
        Task<bool> Delete(TEntity p_entity);
    }

    /// <summary>
    /// 包含"ID"主键的实体类仓库接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TEntity, TKey> : IRepository
        where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// 根据主键获得实体对象，不存在时返回null
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_includeChildren">是否包含所有的子实体</param>
        /// <returns>返回实体对象或null</returns>
        Task<TEntity> Get(TKey p_id, bool p_includeChildren = true);

        /// <summary>
        /// 根据主键删除实体对象
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>true 删除成功</returns>
        Task<bool> Delete(TKey p_id);
    }
}
