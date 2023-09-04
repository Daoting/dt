#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Tools;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
#endregion

namespace Dt.UIDemo
{
    public sealed partial class TestDemo1 : Win
    {
        public TestDemo1()
        {
            InitializeComponent();
        }

        void OnTest1(object sender, RoutedEventArgs e)
        {
            //Kit.RunAsync(() => _sp.Children.Add(new TextBlock { Text = "主线程 Kit.RunAsync" }));
            Windows.Management.Deployment.PackageManager packageManager = new Windows.Management.Deployment.PackageManager();

            try
            {
                // % UserProfile %\AppData\Local\Packages\{ }\LocalState\
                IEnumerable<Windows.ApplicationModel.Package> packages = (IEnumerable<Windows.ApplicationModel.Package>)packageManager.FindPackages("19855Modern.91683A99ECB1_a93adc3rz58km");
                string names = "";
                int packageCount = 0;

                var pkg = packages.FirstOrDefault();
                if (pkg != null)
                {
                    var pro = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    var path = $"{pro}\\AppData\\Local\\Packages\\{pkg.Id.FamilyName}\\LocalState\\";
                    DirectoryInfo di = new DirectoryInfo(path);
                    Kit.Msg(di.FullName);
                    if (di.Exists)
                        Kit.Msg("存在");
                    SysTrace.CopyToClipboard(path);
                }
                //foreach (var pkg in packages)
                //{
                //    //DisplayPackageInfo(package);
                //    //DisplayPackageUsers(packageManager, package);
                //    //Kit.Msg(package.InstalledLocation.DisplayName);
                //    //Console.WriteLine();
                //    packageCount += 1;
                //    names += pkg.EffectivePath;
                //    names += ",";
                //}
                //Kit.Msg(names);
                //Kit.Msg(packageCount.ToString());
                //if (packageCount < 1)
                //{
                //    Console.WriteLine("No packages were found.");
                //}

            }
            catch (UnauthorizedAccessException)
            {
                Kit.Warn("packageManager.FindPackages() failed because access was denied. This program must be run from an elevated command prompt.");

            }
            catch (Exception ex)
            {
                Kit.Warn("packageManager.FindPackages() failed, error message: " + ex.Message);
                //Kit.Msg("Full Stacktrace: {0}", ex.ToString());

            }
        }

        void OnTest2(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Kit.RunAsync(() => _sp.Children.Add(new TextBlock { Text = "Task中 Kit.RunAsync" }));
            });
        }

        void OnTest3(object sender, RoutedEventArgs e)
        {
            Kit.RunSync(() => _sp.Children.Add(new TextBlock { Text = "主线程 Kit.RunSync" }));
        }

        void OnTest4(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Kit.RunSync(() => _sp.Children.Add(new TextBlock { Text = "Task中 Kit.RunSync" }));
            });
        }
    }
}