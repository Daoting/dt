﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-02 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.Crud
{
    public class NameChangedEvent : IEvent
    {
        public CrudX Tgt { get; set; }
    }

    [EventHandler]
    public class NameChangedHandler : IEventHandler<NameChangedEvent>
    {
        public Task Handle(NameChangedEvent p_event)
        {
            return Task.CompletedTask;
        }
    }
}
