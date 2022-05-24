#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO.Compression;
using System.Text.Json;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 启动入口
    /// </summary>
    public abstract partial class DefaultStub : Stub
    {
        /// <summary>
        /// 启动入口
        /// App.OnLaunched -> DefaultStub.OnLaunched -> DefaultStub.Launch
        /// </summary>
        /// <param name="p_launchArgs"></param>
        /// <param name="p_shareInfo"></param>
        /// <returns></returns>
        async Task Launch(string p_launchArgs = null, ShareInfo p_shareInfo = null)
        {
            if (!string.IsNullOrEmpty(p_launchArgs))
            {
                try
                {
                    // 带参数启动
                    _autoStartOnce = JsonSerializer.Deserialize<AutoStartInfo>(p_launchArgs);
                }
                catch { }
            }

            // 非null表示app已启动过
            if (Kit.Stub != null)
            {
                // 带参数启动
                if (_autoStartOnce != null)
                    ShowAutoStartOnce();
                Kit.MainWin.Activate();

                if (p_shareInfo != null)
                    OnReceiveShare(p_shareInfo);
                return;
            }

            try
            {
                // 系统初始化，内含创建窗口及整个系统可视树
                await Kit.Init(this);

                // 连接cm服务，获取全局参数，更新/打开模型库
                if (Kit.IsUsingDtSvc)
                    await InitConfig();

                // 由外部控制启动过程
                await OnStartup();

                // 接收分享
                if (p_shareInfo != null)
                    OnReceiveShare(p_shareInfo);

                // 注册后台任务
                if (EnableBgTask)
                    BgJob.Register();

                InputManager.Init();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// 连接cm服务，获取全局参数，更新打开模型库
        /// </summary>
        /// <returns></returns>
        static async Task InitConfig()
        {
            // 获取全局参数：服务器时间、所有服务地址、模型文件版本号
            List<object> cfg;
            try
            {
                cfg = await AtCm.GetConfig();
            }
            catch
            {
                throw new Exception("服务器连接失败！");
            }

            if (cfg == null || cfg.Count != 3)
                throw new Exception("获取参数失败！");

            // 服务器时间、初始化服务地址
            Kit.SyncTime((DateTime)cfg[0]);
            Kit.InitSvcUrls(cfg[1]);

            // 更新打开模型库
            await OpenModelDb(cfg[2] as string);
        }

        /// <summary>
        /// 更新打开模型文件
        /// 1. 与本地不同时下载新模型文件；
        /// 2. 打开模型库；
        /// </summary>
        /// <param name="p_ver"></param>
        /// <returns></returns>
        static async Task OpenModelDb(string p_ver)
        {
            // 更新模型文件
            string modelVer = Path.Combine(Kit.DataPath, $"model-{p_ver}.ver");
            if (!File.Exists(modelVer))
            {
                string modelFile = Path.Combine(Kit.DataPath, "model.db");

                // 删除旧版的模型文件和版本号文件
                try { File.Delete(modelFile); } catch { }
                foreach (var file in new DirectoryInfo(Kit.DataPath).GetFiles($"model-*.ver"))
                {
                    try { file.Delete(); } catch { }
                }

                try
                {
                    // 下载模型文件，下载地址如 https://localhost/app-cm/.model
                    using (var response = await BaseRpc.Client.GetAsync($"{Kit.GetSvcUrl("cm")}/.model"))
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    using (var fs = File.Create(modelFile, 262140, FileOptions.WriteThrough))
                    {
                        gzipStream.CopyTo(fs);
                        fs.Flush();
                    }

                    // 版本号文件
                    File.Create(modelVer);
                }
                catch (Exception ex)
                {
                    try
                    {
                        File.Delete(modelFile);
                    }
                    catch { }
                    throw new Exception("下载模型文件失败！" + ex.Message);
                }
            }

            // 打开模型库
            try
            {
                AtModel.OpenDb();
            }
            catch (Exception ex)
            {
                throw new Exception("打开模型库失败！" + ex.Message);
            }
        }

        /// <summary>
        /// 启动过程中显示错误信息，此时未加载任何UI
        /// </summary>
        /// <param name="p_error"></param>
        static void ShowError(string p_error)
        {
            var dlg = new Dlg { IsPinned = true, Resizeable = false, HideTitleBar = true, ShowVeil = false, Background = Res.主蓝 };
            if (!Kit.IsPhoneUI)
            {
                dlg.WinPlacement = DlgPlacement.CenterScreen;
                dlg.MinWidth = 300;
                dlg.MaxWidth = Kit.ViewWidth / 4;
                dlg.BorderThickness = new Thickness(0);
            }
            var pnl = new StackPanel { Margin = new Thickness(40), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            pnl.Children.Add(new TextBlock { Text = "\uE037", FontFamily = Res.IconFont, Foreground = Res.WhiteBrush, FontSize = 40, Margin = new Thickness(0, 0, 0, 10), HorizontalAlignment = HorizontalAlignment.Center });
            pnl.Children.Add(new TextBlock { Text = p_error, Foreground = Res.WhiteBrush, FontSize = 20, TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center });
            dlg.Content = pnl;
            dlg.Show();
        }

        void ShowAutoStartOnce()
        {
            Win win = AutoStartKit.CreateAutoStartWin(_autoStartOnce);
            if (win != null)
            {
                if (Kit.IsPhoneUI)
                    win.NaviToHome();
                else
                    Desktop.Inst.ShowNewWin(win);
            }
            _autoStartOnce = null;
        }
    }
}