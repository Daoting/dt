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
    public class InsertCrudEvent : IEvent
    {
        public long ID { get; set; }
    }

    [EventHandler]
    public class InsertCrudHandler: IEventHandler<InsertCrudEvent>
    {
        public Task Handle(InsertCrudEvent p_event)
        {
            Log.Debug("新增ID：" + p_event.ID.ToString());
            return Task.CompletedTask;
        }
    }
}
