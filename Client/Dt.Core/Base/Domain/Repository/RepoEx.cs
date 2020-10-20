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
        /// 根据主键获得实体对象(包含所有列值)，主键列名id，仅支持单主键
        /// </summary>
        /// <param name="p_repo"></param>
        /// <param name="p_id">主键</param>
        /// <param name="p_loadDetails">是否加载附加数据，默认false</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByID<TEntity>(this Repo<TEntity> p_repo, long p_id, bool p_loadDetails = false)
            where TEntity : Entity
        {
            return p_repo.GetByID(p_id.ToString(), p_loadDetails);
        }

        /// <summary>
        /// 根据主键删除实体对象，主键列名id，仅支持单主键
        /// 依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_repo"></param>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelByID<TEntity>(this Repo<TEntity> p_repo, long p_id, bool p_isNotify = true)
            where TEntity : Entity
        {
            return p_repo.DelByID(p_id.ToString(), p_isNotify);
        }

        /// <summary>
        /// 调用服务端Repo删除实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_repo"></param>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelRowByKey<TEntity>(this Repo<TEntity> p_repo, long p_id, bool p_isNotify = true)
            where TEntity : Entity
        {
            return p_repo.DelRowByKey(p_id.ToString(), p_isNotify);
        }
    }
}
