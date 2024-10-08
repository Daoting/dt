using Uno.UI.Runtime.Skia.Wpf;

namespace Demo
{
    public partial class WpfApp : System.Windows.Application
    {
        public WpfApp()
        {
            var host = new WpfHost(Dispatcher, () =>
            {
                var app = new App();
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