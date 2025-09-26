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
    /// 安装包相关
    /// </summary>
    public static class Pkg
    {
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

        public static void UpdatePkgVer()
        {
            var prefix = Cfg.Config.GetValue("WinApp:File", "Dt");
            WinAppVer = new Dict
            {
                { "file", prefix },
                { "force", Cfg.Config.GetValue("WinApp:ForceUpdate", false) },
            };

            var ver = GetAppLastVer(prefix, "x64");
            WinAppVer["x64"] = ver != null ? ver.ToString(4) : "";

            ver = GetAppLastVer(prefix, "arm64");
            WinAppVer["arm64"] = ver != null ? ver.ToString(4) : "";
        }
        
        static Version GetAppLastVer(string p_prefix, string p_arch)
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Cfg.PackageDir, "Win"));
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
