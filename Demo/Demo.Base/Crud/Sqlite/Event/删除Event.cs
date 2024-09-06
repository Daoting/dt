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
    public class Sqlite删除Event : IEvent
    {
        public 基础X Tgt { get; set; }
    }

    [EventHandler]
    public class Sqlite删除Handler : IEventHandler<Sqlite删除Event>
    {
        public Task Handle(Sqlite删除Event p_event)
        {
            Kit.Msg("被删实体：" + p_event.Tgt.名称);
            return Task.CompletedTask;
        }
    }
}
