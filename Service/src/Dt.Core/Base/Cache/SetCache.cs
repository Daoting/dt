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
    /// ֵΪ�����ַ������ϵĻ������
    /// </summary>
    /// <typeparam name="TCacheItem">�������ͣ�����Ϊ��������</typeparam>
    public class SetCache<TCacheItem> : BaseCache
    {
        public SetCache(string p_keyPrefix)
            : base(p_keyPrefix)
        {
        }
    }
}