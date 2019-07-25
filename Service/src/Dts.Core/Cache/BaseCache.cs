#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-21 ����
******************************************************************************/
#endregion

#region ��������
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Cache
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
        public Task<bool> Remove(string p_key)
        {
            if (!string.IsNullOrEmpty(p_key))
                return _db.KeyDeleteAsync(GetFullKey(p_key));
            return Task.FromResult(false);
        }

        /// <summary>
        /// ����ɾ���������
        /// </summary>
        /// <param name="p_keys"></param>
        /// <returns></returns>
        public Task BatchRemove(IEnumerable<string> p_keys)
        {
            if (p_keys == null || p_keys.Count() == 0)
                return Task.FromResult(false);

            var keys = p_keys.Select(p => (RedisKey)GetFullKey(p));
            return _db.KeyDeleteAsync(keys.ToArray());
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
        protected string GetFullKey(string p_key)
        {
            if (string.IsNullOrEmpty(_keyPrefix))
                return p_key;
            return $"{_keyPrefix}:{p_key}";
        }
    }
}