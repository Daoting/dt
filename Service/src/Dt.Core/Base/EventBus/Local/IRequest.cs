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
    /// ����/��Ӧģʽ���¼�
    /// </summary>
    /// <typeparam name="TResponse">����ֵ����</typeparam>
    public interface IRequest<out TResponse> : IEvent
    { }
}