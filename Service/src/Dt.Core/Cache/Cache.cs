#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// Redis缓存管理静态类
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// 根据键查询字符串类型的缓存值
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        /// <param name="p_key">不带前缀的键</param>
        /// <returns>缓存对象</returns>
        public static Task<T> StringGet<T>(string p_keyPrefix, string p_key)
        {
            return new StringCache(p_keyPrefix).Get<T>(p_key);
        }

        /// <summary>
        /// 将对象ToString添加到缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        /// <param name="p_key">不带前缀的键</param>
        /// <param name="p_value">待缓存对象</param>
        /// <param name="p_expiry">过期时间</param>
        /// <returns></returns>
        public static Task StringSet<T>(string p_keyPrefix, string p_key, T p_value, TimeSpan? p_expiry = null)
        {
            return new StringCache(p_keyPrefix).Set<T>(p_key, p_value, p_expiry);
        }

        /// <summary>
        /// 按键名批量查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        /// <param name="p_keys">不带前缀的键名列表</param>
        /// <returns>缓存对象列表</returns>
        public static Task<List<T>> StringBatchGet<T>(string p_keyPrefix, IEnumerable<string> p_keys)
        {
            return new StringCache(p_keyPrefix).BatchGet<T>(p_keys);
        }

        /// <summary>
        /// 根据键查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        /// <param name="p_key">不带前缀的键</param>
        /// <returns>缓存对象</returns>
        public static Task<T> HashGet<T>(string p_keyPrefix, string p_key)
            where T : class
        {
            return new HashCache(p_keyPrefix).Get<T>(p_key);
        }

        /// <summary>
        /// 将对象添加到缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        /// <param name="p_key">不带前缀的键</param>
        /// <param name="p_value">待缓存对象</param>
        /// <param name="p_expiry">过期时间</param>
        /// <returns></returns>
        public static Task HashSet<T>(string p_keyPrefix, string p_key, T p_value, TimeSpan? p_expiry = null)
            where T : class
        {
            return new HashCache(p_keyPrefix).Set<T>(p_key, p_value, p_expiry);
        }

        /// <summary>
        /// 按键名批量查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        /// <param name="p_keys">不带前缀的键名列表</param>
        /// <returns>缓存对象列表</returns>
        public static Task<List<T>> HashBatchGet<T>(string p_keyPrefix, IEnumerable<string> p_keys)
            where T : class
        {
            return new HashCache(p_keyPrefix).BatchGet<T>(p_keys);
        }

        /// <summary>
        /// 获取指定键名的hash中field对应的value
        /// </summary>
        /// <typeparam name="T">field类型</typeparam>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        /// <param name="p_key">不带前缀的键名</param>
        /// <param name="p_field">hash中的field，大小写敏感</param>
        /// <returns>field对应的value</returns>
        public static Task<T> HashGetField<T>(string p_keyPrefix, string p_key, string p_field)
        {
            return new HashCache(p_keyPrefix).GetField<T>(p_key, p_field);
        }

        /// <summary>
        /// 设置指定键名的hash中field对应的value
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        /// <param name="p_key">不带前缀的键名</param>
        /// <param name="p_field">hash中的field，大小写敏感</param>
        /// <param name="p_value">field对应的value</param>
        /// <returns></returns>
        public static Task HashSetField(string p_keyPrefix, string p_key, string p_field, object p_value)
        {
            return new HashCache(p_keyPrefix).SetField(p_key, p_field, p_value);
        }
    }
}