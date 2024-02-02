#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-21 ����
******************************************************************************/
#endregion

#region ��������
using StackExchange.Redis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// ����Redis�Ļ���ӿ�
    /// </summary>
    public abstract class BaseCache
    {
        /// <summary>
        /// Redis����
        /// </summary>
        protected readonly IDatabase _db = Redis.Db;

        /// <summary>
        /// �����ǰ׺����"ur:u"���÷ֺŸ�����
        /// </summary>
        protected readonly string _keyPrefix;

        public BaseCache(string p_keyPrefix)
        {
            Throw.IfEmpty(p_keyPrefix);
            _keyPrefix = p_keyPrefix;
        }

        /// <summary>
        /// ͳ�Ʊ����ͻ��������
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _db.KeyCount(_keyPrefix);
        }

        /// <summary>
        /// ɾ���������
        /// </summary>
        /// <param name="p_key">����ǰ׺�ļ���</param>
        /// <returns></returns>
        public Task<bool> Delete(object p_key)
        {
            return _db.KeyDeleteAsync(GetFullKey(p_key));
        }

        /// <summary>
        /// ����ɾ���������
        /// </summary>
        /// <param name="p_keys"></param>
        /// <returns></returns>
        public Task BatchDelete(IList p_keys)
        {
            if (p_keys == null || p_keys.Count == 0)
                return Task.FromResult(false);

            RedisKey[] keys = new RedisKey[p_keys.Count];
            for (int i = 0; i < p_keys.Count; i++)
            {
                var obj = p_keys[i];
                if (obj != null)
                    keys[i] = GetFullKey(obj);
            }
            return _db.KeyDeleteAsync(keys);
        }

        /// <summary>
        /// ��ձ����ͻ���
        /// </summary>
        public void Clear()
        {
            _db.KeyDeleteWithPrefix(_keyPrefix);
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        protected string GetFullKey(object p_key)
        {
            if (p_key != null)
                return $"{_keyPrefix}:{p_key}";
            return _keyPrefix;
        }
    }
}