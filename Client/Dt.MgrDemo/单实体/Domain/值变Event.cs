#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.单实体
{
    public class 值变Event : IEvent
    {
        public string OriginalVal { get; set; }

        public string NewVal { get; set; }
    }

    [EventHandler]
    public class 值变Handler: IEventHandler<值变Event>
    {
        public Task Handle(值变Event p_event)
        {
            Kit.Msg($"原值：{p_event.OriginalVal}\r\n新值：{p_event.NewVal}");
            return Task.CompletedTask;
        }
    }
}
