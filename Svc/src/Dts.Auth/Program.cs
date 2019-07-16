using Dts.Core;

namespace Dts.Auth
{
    public class Program
    {
        public static void Main(string[] p_args)
        {
            Launcher.Run(new Stub(p_args));
        }
    }
}