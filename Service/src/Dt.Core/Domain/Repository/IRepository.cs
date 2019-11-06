#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
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
    /// 包含"ID"主键的实体类仓库接口
    /// </summary>
    /// <typeparam name="TEntity">聚合根类型</typeparam>
    /// <typeparam name="TKey">聚合根主键类型</typeparam>
    public interface IRepository<TEntity, TKey> : IRepository
        where TEntity : class, IRoot<TKey>
    {
        /// <summary>
        /// 根据主键获得实体对象，不存在时返回null
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_loadDetails">是否加载附加数据，默认false</param>
        /// <returns>返回实体对象或null</returns>
        Task<TEntity> Get(TKey p_id, bool p_loadDetails = false);

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体列表，未加载实体的附加数据！
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        Task<List<TEntity>> GetList(string p_keyOrSql, object p_params = null);

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体枚举，未加载实体的附加数据！高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体枚举</returns>
        Task<IEnumerable<TEntity>> ForEach(string p_keyOrSql, object p_params = null);

        /// <summary>
        /// 插入实体对象
        /// </summary>
        /// <param name="p_entity">待插入的实体</param>
        /// <returns>true 成功</returns>
        Task<bool> Insert(TEntity p_entity);

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="p_entity">待更新的实体</param>
        /// <returns>true 成功</returns>
        Task<bool> Update(TEntity p_entity);

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="p_entity">待删除的实体</param>
        /// <returns>true 删除成功</returns>
        Task<bool> Delete(TEntity p_entity);

        /// <summary>
        /// 根据主键删除实体对象
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>true 删除成功</returns>
        Task<bool> Delete(TKey p_id);
    }
}
