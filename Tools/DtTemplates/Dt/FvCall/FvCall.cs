using System;
using System.Collections.Generic;
using System.IO;

namespace Dt.Fv
{
    class FvCall : IAutoRun
    {
        public void Run()
        {
            string name = "FvCall" + new Random().Next(10, 99).ToString();
            var dt = new Dictionary<string, string>
                {
                    {"$rootnamespace$", Kit.GetNamespace() },
                    {"$clsname$", name },
                    {"$time$", DateTime.Now.ToString("yyyy-MM-dd") },
                    {"$username$", Environment.UserName },
                };
            var path = Path.Combine(Kit.GetFolderPath(), $"{name}.cs");
            Kit.WritePrjFile(path, "Dt.FvCall.FvCallTemp.cs", dt);
            Kit.OpenFile(path);
        }
    }
}
