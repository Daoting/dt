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
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(true, "功能测试", AgentMode.Generic)]
    public class TestEventBus : BaseApi
    {
        public Task Broadcast(bool p_isAllSvcInst)
        {
            _c.Remote.Broadcast(new TestEventData(), p_isAllSvcInst);
            return Task.CompletedTask;
        }

        public Task Multicast(string p_svcName)
        {
            _c.Remote.Multicast(new TestEventData(), p_svcName);
            return Task.CompletedTask;
        }

        public Task Push(string p_svcName)
        {
            _c.Remote.Push(new TestEventData(), p_svcName);
            return Task.CompletedTask;
        }

        public Task PushFixed(string p_svcName, string p_svcID)
        {
            _c.Remote.PushFixed(new TestEventData(), p_svcName, p_svcID);
            return Task.CompletedTask;
        }

        public Task LocalPublish()
        {
            _c.Local.Publish(new KesEvent());
            return Task.CompletedTask;
        }

        public Task<string> LocalCall(string p_name)
        {
            return _c.Local.Call(new UyEvent { Name = p_name });
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
