using System;
using Uno.UI.Runtime.Skia.Linux.FrameBuffer;

namespace Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new FrameBufferHost(() => new App());
            host.Run();
        }
    }
}