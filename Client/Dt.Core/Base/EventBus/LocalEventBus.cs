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
    /// �����¼�����
    /// </summary>
    public sealed class LocalEventBus
    {
        public LocalEventBus()
        {
        }

        /// <summary>
        /// ��Ϊ�¼��������ƣ�ֵΪILocalHandler����
        /// </summary>
        internal static readonly Dictionary<string, Type> NoticeEvents = new Dictionary<string, Type>();

        /// <summary>
        /// ��Ϊ�¼����ͣ�ֵΪHandler����
        /// </summary>
        internal static readonly Dictionary<Type, Type> RequestEvents = new Dictionary<Type, Type>();

        /// <summary>
        /// ���������¼������ȴ�
        /// </summary>
        /// <param name="p_event">�¼�����</param>
        public async void Publish(IEvent p_event)
        {
            Type tp;
            if (p_event == null || !NoticeEvents.TryGetValue(p_event.GetType().Name, out tp))
                return;

            var mi = tp.GetMethod("Handle");
            foreach (var h in Kit.GetServices(tp))
            {
                try
                {
                    // ��˳�����
                    await (Task)mi.Invoke(h, new object[] { p_event });
                }
                catch (Exception e)
                {
                    //_log.LogWarning(e, $"{h.GetType().Name}�������¼�ʱ�쳣��");
                }
            }
        }
    }
}