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
    /// 标准服务存根
    /// </summary>
    public partial class LobStub : DefaultStub
    {
        /// <summary>
        /// 设置cm服务地址，末尾无/，如：https://10.10.1.16/dt-cm
        /// <para>请在构造方法中设置</para>
        /// </summary>
        public string SvcUrl
        {
            get { return (Kit.GetRequiredService<IRpcConfig>() as LobRpcConfig).SvcUrl; }
            protected set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    (Kit.GetRequiredService<IRpcConfig>() as LobRpcConfig).SvcUrl = value.TrimEnd('/');
            }
        }

        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected override void ConfigureServices(IServiceCollection p_svcs)
        {
            base.ConfigureServices(p_svcs);
            p_svcs.AddSingleton<IRpcConfig, LobRpcConfig>();
            p_svcs.AddSingleton<IModelCallback, ModelCallback>();
        }

        /// <summary>
        /// 连接cm服务，获取全局参数，更新打开模型库
        /// </summary>
        /// <returns></returns>
        protected sealed override async Task InitConfig()
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

            // 更新模型库
            await UpdateModelDb(cfg[2] as string);
        }

        /// <summary>
        /// 更新模型文件，与本地不同时下载新模型文件；
        /// </summary>
        /// <param name="p_ver"></param>
        /// <returns></returns>
        async Task UpdateModelDb(string p_ver)
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
                    using (var response = await Kit.RpcClient.GetAsync($"{Kit.GetSvcUrl("cm")}/.model"))
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
        }
    }
}