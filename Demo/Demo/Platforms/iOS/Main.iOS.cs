using UIKit;
using Uno.UI.Hosting;
using TestSdk;

var host = UnoPlatformHostBuilder.Create()
    .App(() => new App())
    .UseAppleUIKit()
    .Build();

host.Run();
