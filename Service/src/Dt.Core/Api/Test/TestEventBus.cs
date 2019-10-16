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
    public class TestEventBus : BaseApi
    {
        public Task Broadcast(List<string> p_svcs, bool p_isAllSvcInst)
        {
            _.Remote.Broadcast(new TestEventData(), p_svcs, p_isAllSvcInst);
            return Task.CompletedTask;
        }

        public Task Multicast(string p_svcName)
        {
            _.Remote.Multicast(new TestEventData(), p_svcName);
            return Task.CompletedTask;
        }

        public Task Push(string p_svcName)
        {
            _.Remote.Push(new TestEventData(), p_svcName);
            return Task.CompletedTask;
        }

        public Task PushFixed(string p_svcID)
        {
            _.Remote.PushFixed(new TestEventData(), p_svcID);
            return Task.CompletedTask;
        }

        public Task LocalPublish()
        {
            _.Local.Publish(new KesEvent());
            return Task.CompletedTask;
        }

        public Task<string> LocalCall(string p_name)
        {
            return _.Local.Call(new UyEvent { Name = p_name });
        }

        public string TestLoadBalance()
        {
            return Glb.ID;
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
