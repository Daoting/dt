﻿#region 文件描述
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
    public class DelCrudEvent : IEvent
    {
        public CrudX Tgt { get; set; }
    }

    [EventHandler]
    public class DelCrudHandler: IEventHandler<DelCrudEvent>
    {
        public Task Handle(DelCrudEvent p_event)
        {
            Log.Debug("被删实体 Name：" + p_event.Tgt.Name);
            return Task.CompletedTask;
        }
    }
}
