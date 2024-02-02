#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Tools;
using Microsoft.UI.Xaml;
using System.Text;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.Storage;
#endregion

namespace Dt.Base.Tools
{
    public class PackageKit
    {
        public static Nl<LogPathInfo> GetLogPaths()
        {
            PackageManager pkgMgr = new PackageManager();

            try
            {
                // 等于 %UserProfile%
                var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                // 当前app包名
                var curName = Package.Current.Id.FamilyName;

                var ls = new Nl<LogPathInfo>();
                var pkgs = pkgMgr.FindPackages();
                foreach (var pk in pkgs)
                {
                    if (pk.IsFramework || pk.IsOptional || pk.IsBundle || pk.IsResourcePackage || pk.IsStub)
                        continue;

                    // 是Dt应用 且 不是当前应用
                    if (!File.Exists(Path.Combine(pk.InstalledPath, "Dt.Core.dll"))
                        || pk.Id.FamilyName == curName)
                        continue;

                    var path = $"{profile}\\AppData\\Local\\Packages\\{pk.Id.FamilyName}\\LocalState\\.log";
                    ls.Add(new LogPathInfo { AppName = pk.DisplayName, Path = path });
                }

                return ls;
            }
            catch (UnauthorizedAccessException)
            {
                Kit.Warn("当前无权访问其它App信息，请“以管理员身份运行”当前应用！");
            }
            catch (Exception ex)
            {
                Kit.Warn("访问其它应用包异常：\r\n" + ex.Message);
            }
            return null;
        }

        public class LogPathInfo
        {
            public string Path { get; set; }

            public string AppName { get; set; }
        }
    }
}