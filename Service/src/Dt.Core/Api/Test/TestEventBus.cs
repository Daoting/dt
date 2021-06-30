#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(GroupName = "功能测试", AgentMode = AgentMode.Generic)]
    public class TestEventBus
    {
        public void Broadcast(List<string> p_svcs, bool p_isAllSvcInst)
        {
            Kit.RemoteBroadcast(new TestEventData(), p_svcs, p_isAllSvcInst);
        }

        public void Multicast(string p_svcName)
        {
            Kit.RemoteMulticast(new TestEventData(), p_svcName);
        }

        public void Push(string p_svcName)
        {
            Kit.RemotePush(new TestEventData(), p_svcName);
        }

        public void PushFixed(string p_svcID)
        {
            Kit.RemotePushFixed(new TestEventData(), p_svcID);
        }

        public void PushGenericEvent(string p_name)
        {
            Kit.RemoteMulticast(new GenericEvent<EventData>(new EventData { Name = p_name }));
        }

        public void LocalPublish()
        {
            Kit.LocalPublish(new KesEvent());
        }

        public Task<string> LocalCall(string p_name)
        {
            return Kit.LocalCall(new UyEvent { Name = p_name });
        }

        public string TestLoadBalance()
        {
            return Kit.SvcID;
        }
    }

    public class TestEventData : IEvent
    {

    }

    public class TestHandler1 : IRemoteHandler<TestEventData>
    {
        public Task Handle(TestEventData p_event)
        {
            Log.Information($"{GetType().Name}已处理");
            return Task.CompletedTask;
        }
    }

    public class TestHandler2 : IRemoteHandler<TestEventData>
    {
        public Task Handle(TestEventData p_event)
        {
            Log.Information($"{GetType().Name}已处理");
            return Task.CompletedTask;
        }
    }

    public class KesHandler : IRemoteHandler<KesEvent>
    {
        public Task Handle(KesEvent p_event)
        {
            Log.Information($"{GetType().Name}已处理");
            return Task.CompletedTask;
        }
    }

    public class KesLocalHandler : ILocalHandler<KesEvent>
    {
        public Task Handle(KesEvent p_event)
        {
            return Task.CompletedTask;
        }
    }
    public class KesEvent : IEvent
    {
    }

    public class KesLocalHandler2 : ILocalHandler<KesEvent>
    {
        public Task Handle(KesEvent p_event)
        {
            throw new Exception("测试异常");
        }
    }

    public class EventData
    {
        public string Name { get; set; }
    }

    public class GenericEvent<T> : IEvent
    {
        public GenericEvent(T p_entity)
        {
            Entity = p_entity;
        }

        public T Entity { get; set; }
    }

    public abstract class GenericHandler<T> : IRemoteHandler<GenericEvent<T>>
    {
        public virtual Task Handle(GenericEvent<T> p_event)
        {
            Log.Information($"{GetType().Name}已处理");
            return Task.CompletedTask;
        }
    }

    public class GenHandler : GenericHandler<EventData>
    {
        public override Task Handle(GenericEvent<EventData> p_event)
        {
            base.Handle(p_event);
            Log.Information("Name:" + (p_event.Entity.Name ?? "null"));
            return Task.CompletedTask;
        }
    }

    public class UyEvent : IRequest<string>
    {
        public string Name { get; set; }
    }

    public class UyHandler : IRequestHandler<UyEvent, string>
    {
        public Task<string> Handle(UyEvent p_request)
        {
            return Task.FromResult($"Hello {p_request.Name}");
        }
    }
}
