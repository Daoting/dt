#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using StackExchange.Redis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// 基于Redis的缓存接口
    /// </summary>
    public abstract class BaseCache
    {
        /// <summary>
        /// Redis访问
        /// </summary>
        protected readonly IDatabase _db = Redis.Db;

        /// <summary>
        /// 缓存键前缀，如"ur:u"，用分号隔开段
        /// </summary>
        protected readonly string _keyPrefix;

        public BaseCache(string p_keyPrefix)
        {
            Throw.IfNullOrEmpty(p_keyPrefix);
            _keyPrefix = p_keyPrefix;
        }

        /// <summary>
        /// 统计本类型缓存的行数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _db.KeyCount(_keyPrefix);
        }

        /// <summary>
        /// 删除缓存对象
        /// </summary>
        /// <param name="p_key">不带前缀的键名</param>
        /// <returns></returns>
        public Task<bool> Delete(object p_key)
        {
            return _db.KeyDeleteAsync(GetFullKey(p_key));
        }

        /// <summary>
        /// 批量删除缓存对象
        /// </summary>
        /// <param name="p_keys"></param>
        /// <returns></returns>
        public Task BatchDelete(IList p_keys)
        {
            if (p_keys == null || p_keys.Count == 0)
                return Task.FromResult(false);

            RedisKey[] keys = new RedisKey[p_keys.Count];
            for (int i = 0; i < p_keys.Count; i++)
            {
                var obj = p_keys[i];
                if (obj != null)
                    keys[i] = GetFullKey(obj);
            }
            return _db.KeyDeleteAsync(keys);
        }

        /// <summary>
        /// 清空本类型缓存
        /// </summary>
        public void Clear()
        {
            _db.KeyDeleteWithPrefix(_keyPrefix);
        }

        /// <summary>
        /// 获取完整键名
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        protected string GetFullKey(object p_key)
        {
            if (p_key != null)
                return $"{_keyPrefix}:{p_key}";
            return _keyPrefix;
        }
    }
}