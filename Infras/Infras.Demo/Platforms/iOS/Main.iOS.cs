using UIKit;
using Uno.UI.Hosting;
using Infras.Demo;

var host = UnoPlatformHostBuilder.Create()
    .App(() => new App())
    .UseAppleUIKit()
    .Build();

host.Run();
