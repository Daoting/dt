#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Configuration;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 配置相关
    /// </summary>
    public static class Cfg
    {
        public static IConfiguration Config { get; private set; }

        /// <summary>
        /// 安装包存放目录
        /// </summary>
        public static string PackageDir { get; private set; }

        /// <summary>
        /// wasm网站静态文件存放目录
        /// </summary>
        public static string WasmDir { get; private set; }

        /// <summary>
        /// wasm的虚拟目录
        /// </summary>
        public const string WasmVirPath = "/wasm";

        /// <summary>
        /// 安装包的虚拟目录
        /// </summary>
        public const string PackageVirPath = "/pkg";

        /// <summary>
        /// windows应用msix安装包的版本信息
        /// </summary>
        public static Dict WinAppVer { get; private set; }


        public static string WinX64File
        {
            get
            {
                if (WinAppVer != null && WinAppVer.Str("x64") != "")
                {
                    return WinAppVer.Str("file") + "_" + WinAppVer.Str("x64") + "_x64.msix";
                }
                return null;
            }
        }

        public static string WinArm64File
        {
            get
            {
                if (WinAppVer != null && WinAppVer.Str("arm64") != "")
                {
                    return WinAppVer.Str("file") + "_" + WinAppVer.Str("arm64") + "_arm64.msix";
                }
                return null;
            }
        }
        
        /// <summary>
        /// 初始化服务配置
        /// </summary>
        public static void Init()
        {
            if (!File.Exists(Path.Combine(Kit.PathBase, "etc/config/cm.json")))
            {
                var ex = "缺少 cm.json 文件！";
                Log.Fatal(ex);
                throw new Exception(ex);
            }

            try
            {
                Config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Kit.PathBase, "etc/config"))
                    .AddJsonFile("cm.json", false, false)
                    .Build();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "读取 cm.json 失败！");
                throw;
            }

            if (Kit.IsSingletonSvc)
            {
                // 单体服务时，只需一个地址
                SysKernel.Config["IsSingletonSvc"] = true;
            }
            else
            {
                SysKernel.Config["IsSingletonSvc"] = false;

                Dict dt = new Dict();
                foreach (var item in Config.GetSection("Urls").GetChildren())
                {
                    dt[item.Key] = item.Value;
                }
                SysKernel.Config["SvcUrls"] = dt;
            }

            PackageDir = Config.GetValue<string>("PackagePath", "package");
            if (!Path.IsPathRooted(PackageDir))
            {
                // 相对路径
                PackageDir = Path.Combine(Kit.PathBase, PackageDir);
            }
            if (!Directory.Exists(PackageDir))
                Directory.CreateDirectory(PackageDir);

            WasmDir = Config.GetValue<string>("WasmPath", "wasm");
            if (!Path.IsPathRooted(WasmDir))
            {
                WasmDir = Path.Combine(Kit.PathBase, WasmDir);
            }
            if (!Directory.Exists(WasmDir))
                Directory.CreateDirectory(WasmDir);

            UpdatePkgVer();
        }

        static void UpdatePkgVer()
        {
            var prefix = Config.GetValue("WinApp:File", "Dt");
            WinAppVer = new Dict
            {
                { "file", prefix },
                { "force", Config.GetValue("WinApp:ForceUpdate", false) },
            };

            var ver = GetAppLastVer(prefix, "x64");
            WinAppVer["x64"] = ver != null ? ver.ToString(4) : "";

            ver = GetAppLastVer(prefix, "arm64");
            WinAppVer["arm64"] = ver != null ? ver.ToString(4) : "";
        }
        
        static Version GetAppLastVer(string p_prefix, string p_arch)
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(PackageDir, "Win"));
            if (!di.Exists)
                return null;
                
            bool exist = false;
            var ver = new Version("0.0.0.0");
            foreach (var fi in di.EnumerateFiles($"{p_prefix}_*_{p_arch}.msix"))
            {
                var ar = fi.Name.Split('_');
                if (ar.Length != 3)
                    continue;

                var curVer = new Version(ar[1]);
                if (curVer > ver)
                {
                    ver = curVer;
                    exist = true;
                }
            }
            return exist? ver : null;
        }
    }
}
