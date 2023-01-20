#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-14 ����
******************************************************************************/
#endregion

#region ��������
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Զ���¼�����ӿ�
    /// </summary>
    /// <typeparam name="TEvent">��ֹ�¼�Ϊ����</typeparam>
    public interface IRemoteEventHandler<TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// �¼�����
        /// </summary>
        /// <param name="p_event">�¼����󣬽�ֹ�¼�Ϊ����</param>
        /// <returns></returns>
        Task Handle(TEvent p_event);
    }
}