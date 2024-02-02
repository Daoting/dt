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
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// 值为值为按插入顺序排序的字符串列表的缓存基类
    /// </summary>
    /// <typeparam name="TCacheItem">缓存类型，可以为任意类型</typeparam>
    public class ListCache<TCacheItem> : BaseCache
    {
        public ListCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }

        /// <summary>
        /// 在尾部添加元素
        /// </summary>
        /// <param name="p_key">不带前缀的键，null时键前缀为完整键</param>
        /// <param name="p_value">待缓存对象</param>
        /// <returns></returns>
        public Task<long> RightPush(object p_key, TCacheItem p_value)
        {
            Throw.If(p_value == null);
            RedisKey key = GetFullKey(p_key);
            if (!NeedSerialize)
                return _db.ListRightPushAsync(key, p_value.ToString());

            return _db.ListRightPushAsync(key, JsonSerializer.Serialize(p_value, JsonOptions.UnsafeSerializer));
        }

        /// <summary>
        /// 返回名称为key的list中start至end之间的元素
        /// </summary>
        /// <param name="p_key">不带前缀的键，null时键前缀为完整键</param>
        /// <param name="p_start"></param>
        /// <param name="p_stop">-1表示最后一个元素</param>
        /// <returns></returns>
        public async Task<List<TCacheItem>> GetRange(object p_key, long p_start = 0, long p_stop = -1)
        {
            RedisKey key = GetFullKey(p_key);
            var arr = await _db.ListRangeAsync(key, p_start, p_stop);
            if (arr == null || arr.Length == 0)
                return default(List<TCacheItem>);

            List<TCacheItem> ls = new List<TCacheItem>();
            if (NeedSerialize)
            {
                foreach (var val in arr)
                {
                    var item = JsonSerializer.Deserialize<TCacheItem>(val, JsonOptions.UnsafeSerializer);
                    ls.Add(item);
                }
            }
            else
            {
                Type tp = typeof(TCacheItem);
                foreach (var val in arr)
                {
                    ls.Add((TCacheItem)Convert.ChangeType(val, tp));
                }
            }
            return ls;
        }

        bool NeedSerialize
        {
            get
            {
                Type tp = typeof(TCacheItem);
                return tp != typeof(string) && tp.IsClass;
            }
        }
    }
}