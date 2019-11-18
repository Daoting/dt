#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// 缓存项管理类
    /// </summary>
    /// <typeparam name="TCacheItem">缓存项类型</typeparam>
    public class Cache<TCacheItem>
        where TCacheItem : class, ICacheItem
    {
        static readonly CacheHandler _cacheHandler;

        static Cache()
        {
            _cacheHandler = new CacheHandler(typeof(TCacheItem));
        }

        /// <summary>
        /// 获取缓存项
        /// </summary>
        /// <param name="p_keyVal">属性值</param>
        /// <param name="p_keyName">属性名</param>
        /// <returns>返回缓存项或null</returns>
        public static Task<TCacheItem> Get(string p_keyVal, string p_keyName = "ID")
        {
            return _cacheHandler.Get<TCacheItem>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 增加缓存项
        /// </summary>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        public static Task Add(TCacheItem p_entity)
        {
            return _cacheHandler.Cache(p_entity);
        }

        /// <summary>
        /// 删除缓存项
        /// </summary>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        public static Task Remove(TCacheItem p_entity)
        {
            return _cacheHandler.Remove(p_entity);
        }

        /// <summary>
        /// 删除缓存项
        /// </summary>
        /// <param name="p_id">缓存项主键值</param>
        /// <returns></returns>
        public static Task Remove(string p_id)
        {
            return _cacheHandler.RemoveByID(p_id);
        }
    }
}
