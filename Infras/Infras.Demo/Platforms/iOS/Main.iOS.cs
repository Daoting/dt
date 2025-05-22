using UIKit;
using Uno.UI.Hosting;
using UnoApp1;

App.InitializeLogging();

var host = UnoPlatformHostBuilder.Create()
    .App(() => new App())
    .UseAppleUIKit()
    .Build();

host.Run();
