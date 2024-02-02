#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using StackExchange.Redis;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// 值为按权重参数排序的字符串有序集合的缓存基类
    /// 该类只管理一个键的SortedSet，键名为KeyPrefix
    /// </summary>
    public class SortedSetCache : BaseCache
    {
        public SortedSetCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }

        /// <summary>
        /// 增加指定字符串的权重
        /// </summary>
        /// <param name="p_name">字符串</param>
        /// <param name="p_stepValue">增量</param>
        /// <returns></returns>
        public async Task<double> Increment(string p_name, double p_stepValue = 1)
        {
            if (string.IsNullOrEmpty(p_name))
                return -1;
            return await _db.SortedSetIncrementAsync(_keyPrefix, p_name, p_stepValue);
        }

        /// <summary>
        /// 减少指定字符串的权重
        /// </summary>
        /// <param name="p_name">字符串</param>
        /// <param name="p_stepValue">减量</param>
        /// <returns></returns>
        public async Task<double> Decrement(string p_name, double p_stepValue = 1)
        {
            if (string.IsNullOrEmpty(p_name))
                return -1;
            return await _db.SortedSetDecrementAsync(_keyPrefix, p_name, p_stepValue);
        }

        /// <summary>
        /// 获取权重值最小的字符串
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetMin()
        {
            var arr = await _db.SortedSetRangeByScoreAsync(_keyPrefix, take: 1);
            if (arr != null && arr.Length == 1)
                return arr[0];
            return null;
        }

        /// <summary>
        /// 获取权重值最大的字符串
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetMax()
        {
            var arr = await _db.SortedSetRangeByScoreAsync(_keyPrefix, order: Order.Descending, take: 1);
            if (arr != null && arr.Length == 1)
                return arr[0];
            return null;
        }
    }
}