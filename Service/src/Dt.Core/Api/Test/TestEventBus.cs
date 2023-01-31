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
    [Api(AgentMode = AgentMode.Generic, IsTest = true)]
    public class TestEventBus : DomainSvc
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

        public void LocalPublish()
        {
            Kit.PublishEvent(new KesEvent());
        }

        public string TestLoadBalance()
        {
            return Kit.SvcID;
        }
    }

    public class TestEventData : IEvent
    {

    }

    [EventHandler]
    public class TestHandler1 : IRemoteEventHandler<TestEventData>
    {
        public Task Handle(TestEventData p_event)
        {
            Log.Information($"{GetType().Name}已处理");
            return Task.CompletedTask;
        }
    }

    [EventHandler]
    public class TestHandler2 : IRemoteEventHandler<TestEventData>
    {
        public Task Handle(TestEventData p_event)
        {
            Log.Information($"{GetType().Name}已处理");
            return Task.CompletedTask;
        }
    }

    [EventHandler]
    public class KesHandler : IRemoteEventHandler<KesEvent>
    {
        public Task Handle(KesEvent p_event)
        {
            Log.Information($"{GetType().Name}已处理");
            return Task.CompletedTask;
        }
    }

    [EventHandler]
    public class KesLocalHandler : IEventHandler<KesEvent>
    {
        public Task Handle(KesEvent p_event)
        {
            Log.Information($"{GetType().Name}已处理");
            return Task.CompletedTask;
        }
    }
    public class KesEvent : IEvent
    {
    }

    [EventHandler]
    public class KesLocalHandler2 : IEventHandler<KesEvent>
    {
        public Task Handle(KesEvent p_event)
        {
            Log.Information($"{GetType().Name}已处理");
            return Task.CompletedTask;
            //throw new Exception("测试异常");
        }
    }
}
