#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-21 ����
******************************************************************************/
#endregion

#region ��������
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
    }
}