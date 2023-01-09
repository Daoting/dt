using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Dt.MySqlBak
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {

#if (!DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new MySqlBakService() };
            ServiceBase.Run(ServicesToRun);
#else
            MySqlBakService service = new MySqlBakService();
            service.Start(null);
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#endif

        }
    }
}
