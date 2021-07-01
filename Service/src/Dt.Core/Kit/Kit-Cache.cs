#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Caches;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Redis缓存方法
    /// </summary>
    public partial class Kit
    {
        #region String
        /// <summary>
        /// 根据键查询字符串类型的缓存值
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <returns>缓存对象</returns>
        public static Task<T> StringGet<T>(string p_keyPrefix, object p_key = null)
        {
            return new StringCache(p_keyPrefix).Get<T>(p_key);
        }

        /// <summary>
        /// 将对象ToString添加到缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <param name="p_value">待缓存对象</param>
        /// <param name="p_expiry">过期时间</param>
        /// <returns></returns>
        public static Task StringSet<T>(string p_keyPrefix, object p_key, T p_value, TimeSpan? p_expiry = null)
        {
            return new StringCache(p_keyPrefix).Set<T>(p_key, p_value, p_expiry);
        }

        /// <summary>
        /// 按键名批量查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_keys">不带前缀的键名列表</param>
        /// <returns>缓存对象列表</returns>
        public static Task<List<T>> StringBatchGet<T>(string p_keyPrefix, IEnumerable<object> p_keys)
        {
            return new StringCache(p_keyPrefix).BatchGet<T>(p_keys);
        }

        /// <summary>
        /// 计数增加1
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <returns>返回加1后的值</returns>
        public static Task<long> StringIncrement(string p_keyPrefix, object p_key = null)
        {
            return new StringCache(p_keyPrefix).Increment(p_key);
        }

        /// <summary>
        /// 计数减1
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <param name="p_min">最小值</param>
        /// <returns>返回减1后的值</returns>
        public static Task<long> StringDecrement(string p_keyPrefix, object p_key = null, int p_min = 0)
        {
            return new StringCache(p_keyPrefix).Decrement(p_key, p_min);
        }
        #endregion

        #region Hash
        /// <summary>
        /// 根据键查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <returns>缓存对象</returns>
        public static Task<T> HashGet<T>(string p_keyPrefix, object p_key = null)
            where T : class
        {
            return new HashCache(p_keyPrefix).Get<T>(p_key);
        }

        /// <summary>
        /// 将对象添加到缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <param name="p_value">待缓存对象</param>
        /// <param name="p_expiry">过期时间</param>
        /// <returns></returns>
        public static Task HashSet<T>(string p_keyPrefix, object p_key, T p_value, TimeSpan? p_expiry = null)
            where T : class
        {
            return new HashCache(p_keyPrefix).Set<T>(p_key, p_value, p_expiry);
        }

        /// <summary>
        /// 按键名批量查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_keys">不带前缀的键名列表</param>
        /// <returns>缓存对象列表</returns>
        public static Task<List<T>> HashBatchGet<T>(string p_keyPrefix, IEnumerable<object> p_keys)
            where T : class
        {
            return new HashCache(p_keyPrefix).BatchGet<T>(p_keys);
        }

        /// <summary>
        /// 获取指定键名的hash中field对应的value
        /// </summary>
        /// <typeparam name="T">field类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <param name="p_field">hash中的field，大小写敏感</param>
        /// <returns>field对应的value</returns>
        public static Task<T> HashGetField<T>(string p_keyPrefix, object p_key, string p_field)
        {
            return new HashCache(p_keyPrefix).GetField<T>(p_key, p_field);
        }

        /// <summary>
        /// 设置指定键名的hash中field对应的value
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <param name="p_field">hash中的field，大小写敏感</param>
        /// <param name="p_value">field对应的value</param>
        /// <returns></returns>
        public static Task HashSetField(string p_keyPrefix, object p_key, string p_field, object p_value)
        {
            return new HashCache(p_keyPrefix).SetField(p_key, p_field, p_value);
        }

        /// <summary>
        /// 根据键查询所有field-value数组
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <returns></returns>
        public static Task<HashEntry[]> HashGetAll(string p_keyPrefix, object p_key = null)
        {
            return new HashCache(p_keyPrefix).GetAll(p_key);
        }
        #endregion

        #region SortedSet
        /// <summary>
        /// SortedSet中增加指定字符串的权重
        /// </summary>
        /// <param name="p_key">完整键名</param>
        /// <param name="p_name">字符串</param>
        /// <param name="p_stepValue">增量</param>
        /// <returns></returns>
        public static Task<double> SortedSetIncrement(string p_key, string p_name, double p_stepValue = 1)
        {
            return new SortedSetCache(p_key).Increment(p_name, p_stepValue);
        }

        /// <summary>
        /// SortedSet中减少指定字符串的权重
        /// </summary>
        /// <param name="p_key">完整键名</param>
        /// <param name="p_name">字符串</param>
        /// <param name="p_stepValue">减量</param>
        /// <returns></returns>
        public static Task<double> SortedSetDecrement(string p_key, string p_name, double p_stepValue = 1)
        {
            return new SortedSetCache(p_key).Decrement(p_name, p_stepValue);
        }

        /// <summary>
        /// 获取权重值最小的字符串
        /// </summary>
        /// <param name="p_key">完整键名</param>
        /// <returns></returns>
        public static Task<string> SortedSetGetMin(string p_key)
        {
            return new SortedSetCache(p_key).GetMin();
        }

        /// <summary>
        /// 获取权重值最大的字符串
        /// </summary>
        /// <param name="p_key">完整键名</param>
        /// <returns></returns>
        public static Task<string> SortedSetGetMax(string p_key)
        {
            return new SortedSetCache(p_key).GetMax();
        }
        #endregion

        #region List
        /// <summary>
        /// 在尾部添加元素
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，null时键前缀为完整键</param>
        /// <param name="p_value">待缓存对象</param>
        /// <returns></returns>
        public static Task<long> ListRightPush<T>(string p_keyPrefix, object p_key, T p_value)
        {
            return new ListCache<T>(p_keyPrefix).RightPush(p_key, p_value);
        }

        /// <summary>
        /// 返回名称为key的list中start至end之间的元素
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，null时键前缀为完整键</param>
        /// <param name="p_start"></param>
        /// <param name="p_stop">-1表示最后一个元素</param>
        /// <returns></returns>
        public static Task<List<T>> ListRange<T>(string p_keyPrefix, object p_key, long p_start = 0, long p_stop = -1)
        {
            return new ListCache<T>(p_keyPrefix).GetRange(p_key, p_start, p_stop);
        }
        #endregion

        #region 公共
        /// <summary>
        /// 删除缓存对象
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_key">不带前缀的键，非null时完整键形如"prefix:key"，null时前缀作为完整键</param>
        /// <returns></returns>
        public static Task<bool> DeleteCache(string p_keyPrefix, object p_key = null)
        {
            Throw.IfNullOrEmpty(p_keyPrefix);
            string key;
            if (p_key != null)
                key = $"{p_keyPrefix}:{p_key}";
            else
                key = p_keyPrefix;
            return Redis.Db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 批量删除缓存对象
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，非空</param>
        /// <param name="p_keys">不带前缀的键名列表</param>
        /// <returns></returns>
        public static Task BatchDeleteCache(string p_keyPrefix, IEnumerable<string> p_keys)
        {
            Throw.IfNullOrEmpty(p_keyPrefix);
            if (p_keys == null || p_keys.Count() == 0)
                return Task.FromResult(false);

            IEnumerable<RedisKey> keys = p_keys.Select(p => (RedisKey)$"{p_keyPrefix}:{p}");
            return Redis.Db.KeyDeleteAsync(keys.ToArray());
        }
        #endregion
    }
}
