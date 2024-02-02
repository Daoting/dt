#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-02 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.CrudSqlite
{
    public class SqliteDelCrudEvent : IEvent
    {
        public CrudX Tgt { get; set; }
    }

    [EventHandler]
    public class SqliteDelCrudHandler : IEventHandler<SqliteDelCrudEvent>
    {
        public Task Handle(SqliteDelCrudEvent p_event)
        {
            Kit.Msg("被删实体 Name：" + p_event.Tgt.Name);
            return Task.CompletedTask;
        }
    }
}
