#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 启动页面：
    /// 1. 显示动画，
    /// 2. 更新打开模型库
    /// 3. 获取加载令牌
    /// 4. 切换到主页或登录页
    /// </summary>
    public partial class Startup : Page
    {
        public Startup()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            // 获取全局参数
            Dict cfg;
            try
            {
                cfg = await AtFw.GetConfig();
            }
            catch (Exception ex)
            {
                ShowError("服务器连接失败！", ex);
                return;
            }

            AtSys.SyncTime(cfg.Date("now"));
            if (await UpdateModel(cfg.Str("ver") + ".db"))
                await UpdateToken();
        }

        /// <summary>
        /// 更新打开模型文件
        /// 1. 与本地不同时下载新模型文件；
        /// 2. 打开模型库；
        /// </summary>
        /// <param name="p_modelFile"></param>
        /// <returns></returns>
        async Task<bool> UpdateModel(string p_modelFile)
        {
            // 更新模型文件
            bool existFile = File.Exists(Path.Combine(AtSys.LocalDbPath, p_modelFile));
            if (!existFile)
            {
                // 关闭模型库，打开时无法删除文件
                AtLocal.CloseModelDb();

                // 删除旧版的模型文件
                foreach (var file in new DirectoryInfo(AtSys.LocalDbPath).GetFiles())
                {
                    if (file.Extension == ".db" && file.Name != "State.db")
                        try { file.Delete(); } catch { }
                }

                try
                {
                    // 下载模型文件，下载地址如 https://localhost/app/cm/.model
                    using(var response = await BaseRpc.Client.GetAsync(AtSys.Stub.ServerUrl.TrimEnd('/') + "/cm/.model"))
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    using (var fs = File.Create(Path.Combine(AtSys.LocalDbPath, p_modelFile), 262140, FileOptions.WriteThrough))
                    {
                        gzipStream.CopyTo(fs);
                        fs.Flush();
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        File.Delete(Path.Combine(AtSys.LocalDbPath, p_modelFile));
                    }
                    catch { }
                    ShowError("下载模型文件失败！", ex);
                    return false;
                }
            }

            // 打开模型库
            try
            {
                AtLocal.OpenModelDb(p_modelFile);
            }
            catch (Exception ex)
            {
                ShowError("打开模型库失败！", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <returns></returns>
        async Task UpdateToken()
        {
            // 未登录
            string token = AtLocal.GetCookie("AccessToken");
            if (string.IsNullOrEmpty(token))
            {
                AuthHelper.Login();
                return;
            }

            // 原token有效
            string expires = AtLocal.GetCookie("TokenExpires");
            if (!string.IsNullOrEmpty(expires)
                && DateTime.TryParse(expires, out DateTime exp)
                && exp > AtSys.Now)
            {
                await AuthHelper.LoadToken(token);
                return;
            }

            // 重新获取token
            token = await AuthHelper.RequestToken(AtLocal.GetCookie("LoginID"), AtLocal.GetCookie("LoginPwd"));
            if (token != null)
            {
                await AuthHelper.LoadToken(token);
            }
            else
            {
                // 获取失败，转到登录页面
                AtLocal.DeleteCookie("AccessToken");
                AtLocal.DeleteCookie("TokenExpires");
                AtLocal.DeleteCookie("LoginID");
                AtLocal.DeleteCookie("LoginPwd");
                AtLocal.DeleteCookie("LoginPhone");
                AuthHelper.Login();
            }
        }

        void ShowError(string p_msg, Exception p_ex)
        {
            _tb.Text = $"{p_msg}：{p_ex.Message}";
        }
    }
}
