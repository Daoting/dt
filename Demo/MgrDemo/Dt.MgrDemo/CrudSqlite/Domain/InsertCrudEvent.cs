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
    public class SqliteInsertCrudEvent : IEvent
    {
        public long ID { get; set; }
    }

    [EventHandler]
    public class SqliteInsertCrudHandler : IEventHandler<SqliteInsertCrudEvent>
    {
        public Task Handle(SqliteInsertCrudEvent p_event)
        {
            Kit.Msg("新增ID：" + p_event.ID.ToString());
            return Task.CompletedTask;
        }
    }
}
