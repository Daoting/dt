#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// 值为键值对集合的缓存基类
    /// </summary>
    public class HashCache : BaseCache
    {
        /// <summary>
        /// 值为键值对集合的缓存基类
        /// </summary>
        /// <param name="p_keyPrefix">缓存键前缀，如"ur:u"，用分号隔开段</param>
        public HashCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }

        /// <summary>
        /// 根据键查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_key">不带前缀的键</param>
        /// <returns>缓存对象</returns>
        public async Task<T> Get<T>(object p_key)
            where T : class
        {
            var hashVal = await _db.HashGetAllAsync(GetFullKey(p_key));
            return FromHashEntry<T>(hashVal);
        }

        /// <summary>
        /// 根据键查询所有field-value数组
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public Task<HashEntry[]> GetAll(object p_key)
        {
            return _db.HashGetAllAsync(GetFullKey(p_key));
        }

        /// <summary>
        /// 将对象添加到缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_key">不带前缀的键名列表</param>
        /// <param name="p_value">待缓存对象</param>
        /// <param name="p_expiry">过期时间</param>
        /// <returns></returns>
        public Task Set<T>(object p_key, T p_value, TimeSpan? p_expiry = null)
            where T : class
        {
            if (p_value == null)
                return Task.CompletedTask;

            HashEntry[] val = ToHashEntry(p_value);
            return _db.HashSetAsync(GetFullKey(p_key), val);
        }

        /// <summary>
        /// 按键名批量查询缓存对象
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="p_keys">不带前缀的键名列表</param>
        /// <returns>缓存对象列表</returns>
        public Task<List<T>> BatchGet<T>(IEnumerable<object> p_keys)
            where T : class
        {
            // if (p_keys == null || p_keys.Count() == 0)
            //     return null;
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定键名的hash中field对应的value
        /// </summary>
        /// <typeparam name="T">field类型</typeparam>
        /// <param name="p_key">不带前缀的键名</param>
        /// <param name="p_field">hash中的field，大小写敏感</param>
        /// <returns>field对应的value</returns>
        public async Task<T> GetField<T>(object p_key, string p_field)
        {
            if (string.IsNullOrEmpty(p_field))
                return default;

            var hashVal = await _db.HashGetAsync(GetFullKey(p_key), p_field);
            if (hashVal.HasValue)
            {
                try
                {
                    return (T)Convert.ChangeType((string)hashVal, typeof(T));
                }
                catch { }
            }
            return default;
        }

        /// <summary>
        /// 设置指定键名的hash中field对应的value
        /// </summary>
        /// <param name="p_key">不带前缀的键名</param>
        /// <param name="p_field">hash中的field，大小写敏感</param>
        /// <param name="p_value">field对应的value</param>
        /// <returns></returns>
        public Task SetField(object p_key, string p_field, object p_value)
        {
            if (string.IsNullOrEmpty(p_field))
                return Task.CompletedTask;

            return _db.HashSetAsync(GetFullKey(p_key), new HashEntry[] { new HashEntry(p_field, p_value == null ? null : p_value.ToString()) });
        }

        /// <summary>
        /// 删除指定键名hash中的field
        /// </summary>
        /// <param name="p_key"></param>
        /// <param name="p_field"></param>
        /// <returns></returns>
        public Task<bool> DeleteField(object p_key, string p_field)
        {
            if (string.IsNullOrEmpty(p_field))
                return Task.FromResult(false);
            return _db.HashDeleteAsync(GetFullKey(p_key), p_field);
        }

        T FromHashEntry<T>(HashEntry[] p_hashVal)
        {
            if (p_hashVal == null || p_hashVal.Length == 0)
                return default;

            Type tp = typeof(T);
            var tgt = Activator.CreateInstance(tp);
            var props = tp.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            foreach (var en in p_hashVal)
            {
                var prop = (from item in props
                            where item.Name.Equals(en.Name, StringComparison.OrdinalIgnoreCase)
                            select item).FirstOrDefault();
                if (prop != null && en.Value.HasValue)
                {
                    try
                    {
                        prop.SetValue(tgt, Convert.ChangeType((string)en.Value, prop.PropertyType));
                    }
                    catch { }
                }
            }
            return (T)tgt;
        }


        HashEntry[] ToHashEntry(object p_value)
        {
            List<HashEntry> ls = new List<HashEntry>();
            foreach (var prop in p_value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty))
            {
                object obj = prop.GetValue(p_value);
                if (obj != null)
                    ls.Add(new HashEntry(prop.Name, obj.ToString()));
            }
            return ls.ToArray();
        }
    }
}