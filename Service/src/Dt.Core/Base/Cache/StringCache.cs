#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// 值为字符串的缓存基类
    /// </summary>
    public class StringCache : BaseCache
    {
        /// <summary>
        /// 值为字符串的缓存基类
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        public StringCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }

        /// <summary>
        /// 根据键查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_key">不带前缀的键，null时键前缀为完整键</param>
        /// <returns>缓存对象</returns>
        public async Task<T> Get<T>(object p_key)
        {
            RedisKey key = GetFullKey(p_key);
            var val = await _db.StringGetAsync(key);
            if (!val.HasValue)
                return default;

            // 简单类型直接转换
            Type tp = typeof(T);
            if (tp == typeof(string) || !tp.IsClass)
            {
                try
                {
                    return (T)Convert.ChangeType(val, tp);
                }
                catch
                {
                    return default;
                }
            }

            // 反序列化
            return JsonSerializer.Deserialize<T>(val, JsonOptions.UnsafeSerializer);
        }

        /// <summary>
        /// 将对象添加到缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_key">不带前缀的键，null时键前缀为完整键</param>
        /// <param name="p_value">待缓存对象</param>
        /// <param name="p_expiry">过期时间</param>
        /// <returns></returns>
        public async Task Set<T>(object p_key, T p_value, TimeSpan? p_expiry = null)
        {
            if (p_value == null)
                return;

            RedisKey key = GetFullKey(p_key);
            Type tp = typeof(T);
            if (tp == typeof(string) || !tp.IsClass)
                await _db.StringSetAsync(key, p_value.ToString(), p_expiry);
            else
                await _db.StringSetAsync(key, JsonSerializer.Serialize(p_value, JsonOptions.UnsafeSerializer), p_expiry);
        }

        /// <summary>
        /// 按键名批量查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keys">不带前缀的键名列表</param>
        /// <returns>缓存对象列表</returns>
        public async Task<List<T>> BatchGet<T>(IEnumerable<object> p_keys)
        {
            if (p_keys == null || p_keys.Count() == 0)
                return null;

            List<RedisKey> keys = new List<RedisKey>();
            foreach (var key in p_keys)
            {
                keys.Add(GetFullKey(key));
            }
            var vals = await _db.StringGetAsync(keys.ToArray());

            List<T> res = new List<T>();
            foreach (var val in vals)
            {
                if (!val.HasValue)
                {
                    res.Add(default);
                    continue;
                }

                // 简单类型直接转换
                Type tp = typeof(T);
                if (tp == typeof(string) || !tp.IsClass)
                {
                    try
                    {
                        res.Add((T)Convert.ChangeType(val, tp));
                    }
                    catch
                    {
                        res.Add(default);
                    }
                }
                // 反序列化
                res.Add(JsonSerializer.Deserialize<T>(val, JsonOptions.UnsafeSerializer));
            }
            return res;
        }

        /// <summary>
        /// 计数增加1
        /// </summary>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <returns>返回加1后的值</returns>
        public Task<long> Increment(object p_key)
        {
            return _db.StringIncrementAsync(GetFullKey(p_key));
        }

        /// <summary>
        /// 计数减1
        /// </summary>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <param name="p_min">最小值</param>
        /// <returns>返回减1后的值</returns>
        public async Task<long> Decrement(object p_key, int p_min = 0)
        {
            RedisKey key = GetFullKey(p_key);
            long cnt = await _db.StringDecrementAsync(key);
            if (cnt < p_min)
            {
                cnt = p_min;
                await _db.StringSetAsync(key, cnt);
            }
            return cnt;
        }

        //public async Task BatchSet<T>(IEnumerable<KeyValuePair<string, object>> p_pairs, TimeSpan? p_expiry = null)
        //{
        //    _db.StringSetAsync()
        //}
    }
}