using Uno.UI.Runtime.Skia.Wpf;

namespace Dt.MgrDemo
{
    public partial class WpfApp : System.Windows.Application
    {
        public WpfApp()
        {
            var host = new WpfHost(Dispatcher, () =>
            {
                var app = new Dt.Demo.App();
                DispatcherUnhandledException += (s, e) =>
                {
                    e.Handled = true;
                    app.OnUnhandledException(e.Exception);
                };
                return app;
            });
            host.Run();
        }
    }
}