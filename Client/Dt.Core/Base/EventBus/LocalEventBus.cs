#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-14 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// 本地事件总线
    /// </summary>
    public sealed class LocalEventBus
    {
        public LocalEventBus()
        {
        }

        /// <summary>
        /// 键为事件类型名称，值为ILocalHandler泛型
        /// </summary>
        internal static readonly Dictionary<string, Type> NoticeEvents = new Dictionary<string, Type>();

        /// <summary>
        /// 键为事件类型，值为Handler类型
        /// </summary>
        internal static readonly Dictionary<Type, Type> RequestEvents = new Dictionary<Type, Type>();

        /// <summary>
        /// 发布本地事件，不等待
        /// </summary>
        /// <param name="p_event">事件内容</param>
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
                    // 按顺序调用
                    await (Task)mi.Invoke(h, new object[] { p_event });
                }
                catch (Exception e)
                {
                    //_log.LogWarning(e, $"{h.GetType().Name}处理本地事件时异常！");
                }
            }
        }
    }
}