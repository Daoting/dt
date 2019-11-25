#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Repo扩展方法
    /// </summary>
    public static class RepoEx
    {
        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，不存在时返回null，仅支持单主键
        /// </summary>
        /// <param name="p_repo"></param>
        /// <param name="p_id">主键</param>
        /// <param name="p_loadDetails">是否加载附加数据，默认false</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByKey<TEntity>(this Repo<TEntity> p_repo, long p_id, bool p_loadDetails = false)
            where TEntity : Entity
        {
            return p_repo.GetByKey(p_id.ToString(), p_loadDetails);
        }

        /// <summary>
        /// 根据主键删除实体对象，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_repo"></param>
        /// <param name="p_id">主键</param>
        /// <returns>实际删除行数</returns>
        public static Task<int> DelByKey<TEntity>(this Repo<TEntity> p_repo, long p_id)
            where TEntity : Entity
        {
            return p_repo.DelByKey(p_id.ToString());
        }
    }
}
