#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Cache
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
        public Task<bool> Remove(string p_key)
        {
            if (!string.IsNullOrEmpty(p_key))
                return _db.KeyDeleteAsync(GetFullKey(p_key));
            return Task.FromResult(false);
        }

        /// <summary>
        /// 批量删除缓存对象
        /// </summary>
        /// <param name="p_keys"></param>
        /// <returns></returns>
        public Task BatchRemove(IEnumerable<string> p_keys)
        {
            if (p_keys == null || p_keys.Count() == 0)
                return Task.FromResult(false);

            var keys = p_keys.Select(p => (RedisKey)GetFullKey(p));
            return _db.KeyDeleteAsync(keys.ToArray());
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
        protected string GetFullKey(string p_key)
        {
            if (string.IsNullOrEmpty(_keyPrefix))
                return p_key;
            return $"{_keyPrefix}:{p_key}";
        }
    }
}