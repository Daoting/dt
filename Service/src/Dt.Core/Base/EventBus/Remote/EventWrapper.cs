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
    /// Զ���¼�����İ�װ��
    /// </summary>
    internal class EventWrapper
    {
        /// <summary>
        /// �¼���������
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// �¼�����
        /// </summary>
        public string Data { get; set; }
    }
}