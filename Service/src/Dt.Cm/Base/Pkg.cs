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

        public static string WinCerFile { get; private set; }

        public static string ApkFile { get; private set; }

        public static string Win7File { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true 已更新，false 无变化</returns>
        public static bool UpdatePkgVer()
        {
            bool isUpdated = false;
            
            var old = WinAppVer;
            WinAppVer = new Dict
            {
                { "force", Cfg.Config.GetValue("ForceUpdateWinApp", false) },
            };

            DirectoryInfo di = new DirectoryInfo(Path.Combine(Cfg.PackageDir, "win"));
            if (!di.Exists)
            {
                WinAppVer["x64"] = "";
                WinAppVer["arm64"] = "";
                di.Create();
            }
            else
            {
                WinAppVer["x64"] = GetAppLastVer("*_x64.msix", di);
                WinAppVer["arm64"] = GetAppLastVer("*_arm64.msix", di);
            }
            WinCerFile = GetAppLastVer("*.cer", di);
            if (old != null && old.Count == WinAppVer.Count)
            {
                foreach (var item in old)
                {
                    if (old.Str(item.Key) != WinAppVer.Str(item.Key))
                    {
                        isUpdated = true;
                        break;
                    }
                }
            }
            else
            {
                isUpdated = true;
            }
            
            var oldFile = ApkFile;
            di = new DirectoryInfo(Path.Combine(Cfg.PackageDir, "android"));
            if (!di.Exists)
            {
                ApkFile = "";
                di.Create();
            }
            else
            {
                ApkFile = GetAppLastVer("*.apk", di);
            }
            if (!isUpdated)
                isUpdated = oldFile != ApkFile;

            oldFile = Win7File;
            di = new DirectoryInfo(Path.Combine(Cfg.PackageDir, "desktop"));
            if (!di.Exists)
            {
                Win7File = "";
                di.Create();
            }
            else
            {
                Win7File = GetAppLastVer("*.zip", di);
            }
            if (!isUpdated)
                isUpdated = oldFile != Win7File;
            
            return isUpdated;
        }

        static string GetAppLastVer(string p_arch, DirectoryInfo p_di)
        {
            var ver = new Version("0.0.0.0");
            string file = "";
            foreach (var fi in p_di.EnumerateFiles(p_arch))
            {
                // 形如：Demo_1.0.0.1_x64.msix
                var ar = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length).Split('_');
                if (ar.Length < 2)
                    continue;

                var curVer = new Version(ar[1]);
                if (curVer > ver)
                {
                    ver = curVer;
                    file = fi.Name;
                }
            }
            return file;
        }
    }
}
