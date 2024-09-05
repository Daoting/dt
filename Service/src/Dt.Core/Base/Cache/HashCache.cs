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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// ֵΪ��ֵ�Լ��ϵĻ������
    /// </summary>
    public class HashCache : BaseCache
    {
        /// <summary>
        /// ֵΪ��ֵ�Լ��ϵĻ������
        /// </summary>
        /// <param name="p_keyPrefix">�����ǰ׺����"ur:u"���÷ֺŸ�����</param>
        public HashCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }

        /// <summary>
        /// ���ݼ���ѯ�������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_key">����ǰ׺�ļ�</param>
        /// <returns>�������</returns>
        public async Task<T> Get<T>(object p_key)
            where T : class
        {
            var hashVal = await _db.HashGetAllAsync(GetFullKey(p_key));
            return FromHashEntry<T>(hashVal);
        }

        /// <summary>
        /// ���ݼ���ѯ����field-value����
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public Task<HashEntry[]> GetAll(object p_key)
        {
            return _db.HashGetAllAsync(GetFullKey(p_key));
        }

        /// <summary>
        /// ��������ӵ�����
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_key">����ǰ׺�ļ����б�</param>
        /// <param name="p_value">���������</param>
        /// <param name="p_expiry">����ʱ��</param>
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
        /// ������������ѯ�������
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_keys">����ǰ׺�ļ����б�</param>
        /// <returns>��������б�</returns>
        public Task<List<T>> BatchGet<T>(IEnumerable<object> p_keys)
            where T : class
        {
            // if (p_keys == null || p_keys.Count() == 0)
            //     return null;
            throw new NotImplementedException();
        }

        /// <summary>
        /// ��ȡָ��������hash��field��Ӧ��value
        /// </summary>
        /// <typeparam name="T">field����</typeparam>
        /// <param name="p_key">����ǰ׺�ļ���</param>
        /// <param name="p_field">hash�е�field����Сд����</param>
        /// <returns>field��Ӧ��value</returns>
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
        /// ����ָ��������hash��field��Ӧ��value
        /// </summary>
        /// <param name="p_key">����ǰ׺�ļ���</param>
        /// <param name="p_field">hash�е�field����Сд����</param>
        /// <param name="p_value">field��Ӧ��value</param>
        /// <returns></returns>
        public Task SetField(object p_key, string p_field, object p_value)
        {
            if (string.IsNullOrEmpty(p_field))
                return Task.CompletedTask;

            return _db.HashSetAsync(GetFullKey(p_key), new HashEntry[] { new HashEntry(p_field, p_value == null ? null : p_value.ToString()) });
        }

        /// <summary>
        /// ɾ��ָ������hash�е�field
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