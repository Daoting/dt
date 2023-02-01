#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Xaml;
using System.Text;
#endregion

namespace Dt.UIDemo
{
    public partial class LocalEventDemo : Win
    {
        public LocalEventDemo()
        {
            InitializeComponent();
        }

        void OnPub(object sender, RoutedEventArgs e)
        {
            _ = Kit.PublishEvent(new TestEventData { Name = new Random().Next().ToString() });
        }

        void OnPubMulti(object sender, RoutedEventArgs e)
        {
            _ = Kit.PublishEvent(new KesEvent { Name = new Random().Next().ToString() });
        }
    }

    public class TestEventData : IEvent
    {
        public string Name { get; set; }
    }

    [EventHandler]
    public class TestHandler2 : IEventHandler<TestEventData>
    {
        public Task Handle(TestEventData p_event)
        {
            Log.Information($"{GetType().Name}已处理，Name：{p_event.Name}");
            return Task.CompletedTask;
        }
    }

    public class KesEvent : IEvent
    {
        public string Name { get; set; }
    }

    [EventHandler]
    public class KesLocalHandler1 : IEventHandler<KesEvent>
    {
        public Task Handle(KesEvent p_event)
        {
            Log.Information($"{GetType().Name}已处理，Name：{p_event.Name}");
            return Task.CompletedTask;
        }
    }

    [EventHandler]
    public class KesLocalHandler2 : IEventHandler<KesEvent>
    {
        public Task Handle(KesEvent p_event)
        {
            Log.Information($"{GetType().Name}已处理，Name：{p_event.Name}");
            return Task.CompletedTask;
            //throw new Exception("测试异常");
        }
    }
}