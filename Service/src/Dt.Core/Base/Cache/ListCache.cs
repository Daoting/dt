#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-21 ����
******************************************************************************/
#endregion

#region ��������
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// ֵΪֵΪ������˳��������ַ����б�Ļ������
    /// </summary>
    /// <typeparam name="TCacheItem">�������ͣ�����Ϊ��������</typeparam>
    public class ListCache<TCacheItem> : BaseCache
    {
        public ListCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }

        /// <summary>
        /// ��β�����Ԫ��
        /// </summary>
        /// <param name="p_key">����ǰ׺�ļ���nullʱ��ǰ׺Ϊ������</param>
        /// <param name="p_value">���������</param>
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
        /// ��������Ϊkey��list��start��end֮���Ԫ��
        /// </summary>
        /// <param name="p_key">����ǰ׺�ļ���nullʱ��ǰ׺Ϊ������</param>
        /// <param name="p_start"></param>
        /// <param name="p_stop">-1��ʾ���һ��Ԫ��</param>
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