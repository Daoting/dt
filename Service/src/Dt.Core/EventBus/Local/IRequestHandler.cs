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
    /// ����/��Ӧģʽ���¼�����ӿ�
    /// </summary>
    /// <typeparam name="TRequest">�¼��������������</typeparam>
    /// <typeparam name="TResponse">�¼�������������</typeparam>
    public interface IRequestHandler<TRequest, TResponse> : IEventHandler
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="p_request">��������</param>
        /// <returns>������Ӧֵ</returns>
        Task<TResponse> Handle(TRequest p_request);
    }
}