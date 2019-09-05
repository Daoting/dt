#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-09-02 ����
******************************************************************************/
#endregion

#region ��������
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// Redis�������̬��
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// ���ݼ���ѯ�ַ������͵Ļ���ֵ
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        /// <param name="p_key">����ǰ׺�ļ�</param>
        /// <returns>�������</returns>
        public static Task<T> StringGet<T>(string p_keyPrefix, string p_key)
        {
            return new StringCache(p_keyPrefix).Get<T>(p_key);
        }

        /// <summary>
        /// ������ToString��ӵ�����
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        /// <param name="p_key">����ǰ׺�ļ�</param>
        /// <param name="p_value">���������</param>
        /// <param name="p_expiry">����ʱ��</param>
        /// <returns></returns>
        public static Task StringSet<T>(string p_keyPrefix, string p_key, T p_value, TimeSpan? p_expiry = null)
        {
            return new StringCache(p_keyPrefix).Set<T>(p_key, p_value, p_expiry);
        }

        /// <summary>
        /// ������������ѯ�������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        /// <param name="p_keys">����ǰ׺�ļ����б�</param>
        /// <returns>��������б�</returns>
        public static Task<List<T>> StringBatchGet<T>(string p_keyPrefix, IEnumerable<string> p_keys)
        {
            return new StringCache(p_keyPrefix).BatchGet<T>(p_keys);
        }

        /// <summary>
        /// ���ݼ���ѯ�������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        /// <param name="p_key">����ǰ׺�ļ�</param>
        /// <returns>�������</returns>
        public static Task<T> HashGet<T>(string p_keyPrefix, string p_key)
            where T : class
        {
            return new HashCache(p_keyPrefix).Get<T>(p_key);
        }

        /// <summary>
        /// ��������ӵ�����
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        /// <param name="p_key">����ǰ׺�ļ�</param>
        /// <param name="p_value">���������</param>
        /// <param name="p_expiry">����ʱ��</param>
        /// <returns></returns>
        public static Task HashSet<T>(string p_keyPrefix, string p_key, T p_value, TimeSpan? p_expiry = null)
            where T : class
        {
            return new HashCache(p_keyPrefix).Set<T>(p_key, p_value, p_expiry);
        }

        /// <summary>
        /// ������������ѯ�������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        /// <param name="p_keys">����ǰ׺�ļ����б�</param>
        /// <returns>��������б�</returns>
        public static Task<List<T>> HashBatchGet<T>(string p_keyPrefix, IEnumerable<string> p_keys)
            where T : class
        {
            return new HashCache(p_keyPrefix).BatchGet<T>(p_keys);
        }

        /// <summary>
        /// ��ȡָ��������hash��field��Ӧ��value
        /// </summary>
        /// <typeparam name="T">field����</typeparam>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        /// <param name="p_key">����ǰ׺�ļ���</param>
        /// <param name="p_field">hash�е�field����Сд����</param>
        /// <returns>field��Ӧ��value</returns>
        public static Task<T> HashGetField<T>(string p_keyPrefix, string p_key, string p_field)
        {
            return new HashCache(p_keyPrefix).GetField<T>(p_key, p_field);
        }

        /// <summary>
        /// ����ָ��������hash��field��Ӧ��value
        /// </summary>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        /// <param name="p_key">����ǰ׺�ļ���</param>
        /// <param name="p_field">hash�е�field����Сд����</param>
        /// <param name="p_value">field��Ӧ��value</param>
        /// <returns></returns>
        public static Task HashSetField(string p_keyPrefix, string p_key, string p_field, object p_value)
        {
            return new HashCache(p_keyPrefix).SetField(p_key, p_field, p_value);
        }

        /// <summary>
        /// ɾ���������
        /// </summary>
        /// <param name="p_keyPrefix">�����ǰ׺</param>
        /// <param name="p_key">����ǰ׺�ļ���</param>
        /// <returns></returns>
        public static Task<bool> Remove(string p_keyPrefix, string p_key)
        {
            if (string.IsNullOrEmpty(p_key))
                return Task.FromResult(false);

            string key;
            if (string.IsNullOrEmpty(p_keyPrefix))
                key = p_key;
            else
                key = $"{p_keyPrefix}:{p_key}";
            return Redis.Db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// ����ɾ���������
        /// </summary>
        /// <param name="p_keyPrefix">�����ǰ׺</param>
        /// <param name="p_keys">����ǰ׺�ļ����б�</param>
        /// <returns></returns>
        public static Task BatchRemove(string p_keyPrefix, IEnumerable<string> p_keys)
        {
            if (p_keys == null || p_keys.Count() == 0)
                return Task.FromResult(false);

            IEnumerable<RedisKey> keys;
            if (string.IsNullOrEmpty(p_keyPrefix))
                keys = p_keys.Select(p => (RedisKey)p);
            else
                keys = p_keys.Select(p => (RedisKey)$"{p_keyPrefix}:{p}");
            return Redis.Db.KeyDeleteAsync(keys.ToArray());
        }

        /// <summary>
        /// ��������1
        /// </summary>
        /// <param name="p_key">��������</param>
        /// <returns>���ؼ�1���ֵ</returns>
        public static long Increment(string p_key)
        {
            return Redis.Db.StringIncrement(p_key);
        }

        /// <summary>
        /// ������1
        /// </summary>
        /// <param name="p_key">��������</param>
        /// <param name="p_min">��Сֵ</param>
        /// <returns>���ؼ�1���ֵ</returns>
        public static long Decrement(string p_key, int p_min = 0)
        {
            var db = Redis.Db;
            long cnt = db.StringDecrement(p_key);
            if (cnt < p_min)
            {
                cnt = p_min;
                db.StringSet(p_key, cnt);
            }
            return cnt;
        }
    }
}