#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-21 ����
******************************************************************************/
#endregion

#region ��������
using StackExchange.Redis;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Caches
{
    /// <summary>
    /// ֵΪ��Ȩ�ز���������ַ������򼯺ϵĻ������
    /// ����ֻ����һ������SortedSet������ΪKeyPrefix
    /// </summary>
    public class SortedSetCache : BaseCache
    {
        public SortedSetCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }

        /// <summary>
        /// ����ָ���ַ�����Ȩ��
        /// </summary>
        /// <param name="p_name">�ַ���</param>
        /// <param name="p_stepValue">����</param>
        /// <returns></returns>
        public async Task<double> Increment(string p_name, double p_stepValue = 1)
        {
            if (string.IsNullOrEmpty(p_name))
                return -1;
            return await _db.SortedSetIncrementAsync(_keyPrefix, p_name, p_stepValue);
        }

        /// <summary>
        /// ����ָ���ַ�����Ȩ��
        /// </summary>
        /// <param name="p_name">�ַ���</param>
        /// <param name="p_stepValue">����</param>
        /// <returns></returns>
        public async Task<double> Decrement(string p_name, double p_stepValue = 1)
        {
            if (string.IsNullOrEmpty(p_name))
                return -1;
            return await _db.SortedSetDecrementAsync(_keyPrefix, p_name, p_stepValue);
        }

        /// <summary>
        /// ��ȡȨ��ֵ��С���ַ���
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
        /// ��ȡȨ��ֵ�����ַ���
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