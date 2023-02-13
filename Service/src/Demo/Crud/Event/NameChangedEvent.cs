#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-02 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Crud
{
    public class NameChangedEvent : IEvent
    {
        public string OriginalVal { get; set; }

        public string NewVal { get; set; }
    }

    [EventHandler]
    public class NameChangedHandler : IEventHandler<NameChangedEvent>
    {
        public Task Handle(NameChangedEvent p_event)
        {
            Log.Debug($"原值：{p_event.OriginalVal}，\r\n新值：{p_event.NewVal}");
            return Task.CompletedTask;
        }
    }
}
