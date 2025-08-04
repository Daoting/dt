
public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Contains("-RegisterForBGTaskServer"))
        {
            // ����ʱ��"����λ��"ѡ��"TimeTriggeredTask"�󣬽��뵱ǰλ��
            var exitEvent = Dt.Tasks.BackgroundTaskServer.Register();
            exitEvent.WaitOne();
        }
        else
        {
            // App.g.i.cs
            global::WinRT.ComWrappersSupport.InitializeComWrappers();
            global::Microsoft.UI.Xaml.Application.Start((p) => {
                var context = new global::Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(global::Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
                global::System.Threading.SynchronizationContext.SetSynchronizationContext(context);
                new Demo.App();
            });
        }
    }
}
