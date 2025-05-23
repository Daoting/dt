using Uno.UI.Hosting;
using Infras.Demo;

var host = UnoPlatformHostBuilder.Create()
    .App(() => new App())
    .UseX11()
    .UseLinuxFrameBuffer()
    .UseWin32()
    .Build();

host.Run();
