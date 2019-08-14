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

namespace Dt.Core.EventBus
{
    /// <summary>
    /// �����¼�����ӿ�
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface ILocalHandler<TEvent> : IEventHandler
        where TEvent : IEvent
    {
        /// <summary>
        /// �¼�����
        /// </summary>
        /// <param name="p_event">�¼�����</param>
        /// <returns></returns>
        Task Handle(TEvent p_event);
    }
}