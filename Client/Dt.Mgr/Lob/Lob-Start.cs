#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 启动过程
    /// </summary>
    public partial class Lob
    {
        /// <summary>
        /// 按默认流程开始运行
        /// <para>1. 已登录过，先自动登录</para>
        /// <para>2. 未登录或登录失败时，根据 p_loginFirst 显示登录页或主页</para>
        /// </summary>
        /// <param name="p_svcUrl">cm服务地址，如：https://10.10.1.16/dt-cm </param>
        /// <param name="p_loginFirst">是否强制先登录，默认true</param>
        /// <returns></returns>
        public static async Task StartRun(string p_svcUrl, bool p_loginFirst = true)
        {
            if (string.IsNullOrEmpty(p_svcUrl))
                throw new ArgumentNullException(nameof(p_svcUrl));

            if (Stub.Inst is not LobStub)
                throw new Exception("");

            var rpc = Kit.GetRequiredService<IRpcConfig>() as LobRpcConfig;
            if (rpc == null)
                throw new Exception("");

            rpc.SvcUrl = p_svcUrl.TrimEnd('/');
            await InitConfig();

            string phone = AtState.GetCookie("LoginPhone");
            string pwd = AtState.GetCookie("LoginPwd");
            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
            {
                // 自动登录
                var result = await AtCm.LoginByPwd<LoginResult>(phone, pwd);

                // 登录成功
                if (result.IsSuc)
                {
                    InitUser(result);
                    // 切换到主页
                    ShowHome();
                    // 接收服务器推送
                    PushHandler.Register();
                    return;
                }
            }

            // 未登录或登录失败
            if (p_loginFirst)
            {
                // 强制先登录
                ShowLogin(false);
            }
            else
            {
                // 未登录先显示主页
                ShowHome();
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
            var rpc = Kit.GetRequiredService<IRpcConfig>() as LobRpcConfig;
            if (rpc != null)
                rpc.InitSvcUrls(cfg[1]);

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
    }
}