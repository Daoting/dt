#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-08-08 创建
******************************************************************************/
#endregion

#if WIN
#region 引用命名
using Dt.Core.Rpc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Win版应用自动更新，https://learn.microsoft.com/zh-cn/windows/msix/non-store-developer-updates
    /// </summary>
    public static class WinPkgUpdate
    {
        public static async void CheckUpdate(bool p_alertNoUpdate = false)
        {
            if (At.Framework != AccessType.Service)
                return;

            var pkg = Package.Current;
            if (pkg.IsDevelopmentMode)
            {
                if (p_alertNoUpdate)
                    Kit.Msg("开发版无需更新！");
                return;
            }

            //Dict dt = new Dict { { "force", true }, { "x64", "Demo_1.0.0.1_x64.msix" }, { "arm64", "Demo_1.0.0.1_arm64.msix" } };
            var dt = await new UnaryRpc(
                    "cm",
                    "SysKernel.GetWinAppVer"
                ).Call<Dict>();

            var file = dt.Str(pkg.Id.Architecture.ToString().ToLower());
            var ar = file.Split('_');
            if (ar.Length != 3)
                return;

            var newVer = new Version(ar[1]);
            var pkgVer = pkg.Id.Version;
            var curVer = new Version(string.Format("{0}.{1}.{2}.{3}", pkgVer.Major, pkgVer.Minor, pkgVer.Build, pkgVer.Revision));

            if (newVer.CompareTo(curVer) <= 0)
            {
                if (p_alertNoUpdate)
                    Kit.Msg("当前已是最新版本！");
                return;
            }

            // 强制更新 或 确认更新
            if (dt.Bool("force")
                || await Kit.Confirm("已发现新版本应用，要升级吗？"))
            {
                Kit.RunAsync(async () => await UpdatePkg(file));
            }
        }

        static async Task UpdatePkg(string p_fileName)
        {
            var dlg = new Dlg
            {
                Resizeable = false,
                HideTitleBar = true,
                IsPinned = true,
                ShowVeil = true,
                PhonePlacement = DlgPlacement.CenterScreen,
                WinPlacement = DlgPlacement.CenterScreen,
            };

            Grid grid = new Grid { Padding = new Thickness(20) };

            var sp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            sp.Children.Add(new ProgressRing { Height = 60, Width = 60, IsActive = true, HorizontalAlignment = HorizontalAlignment.Center });
            sp.Children.Add(new TextBlock
            {
                Text = "发现新版本，正在升级...\r\n　完成后自动重启",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10),
            });
            grid.Children.Add(sp);

            dlg.Content = grid;
            dlg.Show();

            try
            {
                RegisterApplicationRestart(null, RestartFlags.NONE);
                var pm = new PackageManager();
                var res = await pm.AddPackageAsync(
                    // https时会校验证书，无效时安装失败！！！
                    new Uri($"{At.GetSvcUrl("cm")}/pkg/win/{p_fileName}"),
                    null,
                    DeploymentOptions.ForceApplicationShutdown
                );

                if (res != null && !string.IsNullOrEmpty(res.ErrorText))
                {
                    Kit.Warn("更新失败！\r\n" + res.ErrorText);
                }
            }
            catch (Exception ex)
            {
                Kit.Warn("更新失败！\r\n" + ex.Message);
            }
            finally
            {
                dlg.Close();
            }
        }


        #region Restart Manager Methods
        /// <summary>
        /// Registers the active instance of an application for restart.
        /// </summary>
        /// <param name="pwzCommandLine">
        /// A pointer to a Unicode string that specifies the command-line arguments for the application when it is restarted.
        /// The maximum size of the command line that you can specify is RESTART_MAX_CMD_LINE characters. Do not include the name of the executable
        /// in the command line; this function adds it for you.
        /// If this parameter is NULL or an empty string, the previously registered command line is removed. If the argument contains spaces,
        /// use quotes around the argument.
        /// </param>
        /// <param name="dwFlags">One of the options specified in RestartFlags</param>
        /// <returns>
        /// This function returns S_OK on success or one of the following error codes:
        /// E_FAIL for internal error.
        /// E_INVALIDARG if rhe specified command line is too long.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern uint RegisterApplicationRestart(string pwzCommandLine, RestartFlags dwFlags);
        #endregion Restart Manager Methods

        #region Restart Manager Enums
        /// <summary>
        /// Flags for the RegisterApplicationRestart function
        /// </summary>
        [Flags]
        internal enum RestartFlags
        {
            /// <summary>None of the options below.</summary>
            NONE = 0,

            /// <summary>Do not restart the process if it terminates due to an unhandled exception.</summary>
            RESTART_NO_CRASH = 1,
            /// <summary>Do not restart the process if it terminates due to the application not responding.</summary>
            RESTART_NO_HANG = 2,
            /// <summary>Do not restart the process if it terminates due to the installation of an update.</summary>
            RESTART_NO_PATCH = 4,
            /// <summary>Do not restart the process if the computer is restarted as the result of an update.</summary>
            RESTART_NO_REBOOT = 8
        }
        #endregion Restart Manager Enums
    }
}
#endif