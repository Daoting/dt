using UIKit;
using Uno.UI.Hosting;
using Demo;

var host = UnoPlatformHostBuilder.Create()
    .App(() => new App())
    .UseAppleUIKit()
    .Build();

host.Run();
