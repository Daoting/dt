#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base.Sqlite
{
    public class Sqlite值变Event : IEvent
    {
        public string OriginalVal { get; set; }

        public string NewVal { get; set; }
    }

    [EventHandler]
    public class Sqlite值变Handler : IEventHandler<Sqlite值变Event>
    {
        public Task Handle(Sqlite值变Event p_event)
        {
            Kit.Msg($"原值：{p_event.OriginalVal}\r\n新值：{p_event.NewVal}");
            return Task.CompletedTask;
        }
    }
}
