#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace $rootnamespace$
{
    public class $clsname$Event : IEvent
    {
        
    }

    [EventHandler]
    public class $clsname$Handler: IEventHandler<$clsname$Event>
    {
        public Task Handle($clsname$Event p_event)
        {
            return Task.CompletedTask;
        }
    }
}
