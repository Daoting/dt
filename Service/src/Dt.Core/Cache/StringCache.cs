#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-21 ����
******************************************************************************/
#endregion

#region ��������
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
    /// ֵΪ�ַ����Ļ������
    /// </summary>
    public class StringCache : BaseCache
    {
        /// <summary>
        /// ֵΪ�ַ����Ļ������
        /// </summary>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        public StringCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }

        /// <summary>
        /// ���ݼ���ѯ�������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_key">����ǰ׺�ļ�</param>
        /// <returns>�������</returns>
        public async Task<T> Get<T>(string p_key)
        {
            if (string.IsNullOrEmpty(p_key))
                return default;

            RedisKey key = GetFullKey(p_key);
            var val = await _db.StringGetAsync(key);
            if (!val.HasValue)
                return default;

            // ������ֱ��ת��
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

            // �����л�
            return JsonSerializer.Deserialize<T>(val, JsonOptions.UnsafeSerializer);
        }

        /// <summary>
        /// ��������ӵ�����
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_key">����ǰ׺�ļ����б�</param>
        /// <param name="p_value">���������</param>
        /// <param name="p_expiry">����ʱ��</param>
        /// <returns></returns>
        public async Task Set<T>(string p_key, T p_value, TimeSpan? p_expiry = null)
        {
            if (string.IsNullOrEmpty(p_key) || p_value == null)
                return;

            RedisKey key = GetFullKey(p_key);
            Type tp = typeof(T);
            if (tp == typeof(string) || !tp.IsClass)
                await _db.StringSetAsync(key, p_value.ToString(), p_expiry);
            else
                await _db.StringSetAsync(key, JsonSerializer.Serialize(p_value, JsonOptions.UnsafeSerializer), p_expiry);
        }

        /// <summary>
        /// ������������ѯ�������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_keys">����ǰ׺�ļ����б�</param>
        /// <returns>��������б�</returns>
        public async Task<List<T>> BatchGet<T>(IEnumerable<string> p_keys)
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

                // ������ֱ��ת��
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
                // �����л�
                res.Add(JsonSerializer.Deserialize<T>(val, JsonOptions.UnsafeSerializer));
            }
            return res;
        }

        //public async Task BatchSet<T>(IEnumerable<KeyValuePair<string, object>> p_pairs, TimeSpan? p_expiry = null)
        //{
        //    _db.StringSetAsync()
        //}
    }
}