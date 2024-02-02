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
    public class SqliteNameChangedEvent : IEvent
    {
        public string OriginalVal { get; set; }

        public string NewVal { get; set; }
    }

    [EventHandler]
    public class SqliteNameChangedHandler : IEventHandler<SqliteNameChangedEvent>
    {
        public Task Handle(SqliteNameChangedEvent p_event)
        {
            Kit.Msg($"原值：{p_event.OriginalVal}，\r\n新值：{p_event.NewVal}");
            return Task.CompletedTask;
        }
    }
}
