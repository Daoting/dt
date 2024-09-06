#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-02 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base.Sqlite
{
    public class Sqlite插入Event : IEvent
    {
        public long ID { get; set; }
    }

    [EventHandler]
    public class Sqlite插入Handler : IEventHandler<Sqlite插入Event>
    {
        public Task Handle(Sqlite插入Event p_event)
        {
            Kit.Msg("新增ID：" + p_event.ID.ToString());
            return Task.CompletedTask;
        }
    }
}
