#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-14 ����
******************************************************************************/
#endregion

#region ��������
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// �����з��񸱱������鲥ʱ�ų�ָ������
    /// </summary>
    public class ExcludeEvent : IEvent
    {
        /// <summary>
        /// �ų��ķ��񸱱�ID
        /// </summary>
        public virtual string ExcludeSvcID { get; } = Kit.SvcID;
    }
}