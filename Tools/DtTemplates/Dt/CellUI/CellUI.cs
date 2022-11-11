using System;
using System.Collections.Generic;
using System.IO;

namespace Dt.Cells
{
    class CellUI : IAutoRun
    {
        public void Run()
        {
            string name = "UI" + new Random().Next(10, 99).ToString();
            var dt = new Dictionary<string, string>
                {
                    {"$rootnamespace$", Kit.GetNamespace() },
                    {"$clsname$", name },
                    {"$time$", DateTime.Now.ToString("yyyy-MM-dd") },
                    {"$username$", Environment.UserName },
                };
            var path = Path.Combine(Kit.GetFolderPath(), $"{name}.cs");
            Kit.WritePrjFile(path, "Dt.CellUI.CellUITemp.cs", dt);
            Kit.OpenFile(path);
        }
    }
}
