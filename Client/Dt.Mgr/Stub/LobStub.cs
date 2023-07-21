#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Dt.Mgr.Module;
using Dt.Mgr.Rbac;
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

        #region 用户相关回调
        /// <summary>
        /// cookie自动登录
        /// </summary>
        /// <param name="p_showWarning">是否显示警告信息</param>
        /// <returns></returns>
        protected override Task<bool> LoginByCookie(bool p_showWarning)
        {
            return LoginDs.LoginByCookie(p_showWarning);
        }

        const string _perVerKey = "PermissionVersion";
        static bool _initPer = false;

        /// <summary>
        /// 判断当前登录用户是否具有指定权限
        /// </summary>
        /// <param name="p_permission">权限名称</param>
        /// <returns>true 表示有权限</returns>
        protected override async Task<bool> HasPermission(string p_permission)
        {
            if (string.IsNullOrEmpty(p_permission))
                return false;

            if (_initPer)
            {
                return await HasPerInternal(p_permission);
            }

            // 查询当前版本号
            _initPer = true;
            var ver = await AtCm.StringGet(RbacDs.PrefixPer + Kit.UserID);
            if (!string.IsNullOrEmpty(ver))
            {
                var localVer = await CookieX.Get(_perVerKey);
                if (localVer == ver)
                {
                    // 版本号相同，直接取本地数据
                    return await HasPerInternal(p_permission);
                }
            }

            // 更新用户权限，缓存新版本号
            var tbl = await PermissionX.GetUserPermission(Kit.UserID);

            // 清空旧数据
            await AtLob.Exec("delete from UserPermission");

            bool hasPer = false;
            long sum = 0;
            if (tbl != null && tbl.Count > 0)
            {
                List<Dict> dts = new List<Dict>();
                foreach (var row in tbl)
                {
                    var name = row.Str("name");
                    if (p_permission.Equals(name, StringComparison.OrdinalIgnoreCase))
                        hasPer = true;

                    dts.Add(new Dict { { "name", row.Str("name") } });
                    sum += row.Long("id");
                }
                var d = new Dict();
                d["text"] = "insert into UserPermission (name) values (@name)";
                d["params"] = dts;
                await AtLob.BatchExec(new List<Dict> { d });
            }

            // redis和本地sqlite都记录版本号
            // 版本号是所有用户权限id的和！
            string newVer = sum.ToString();
            await AtCm.StringSet(RbacDs.PrefixPer + Kit.UserID, newVer);

            await CookieX.DelByID(_perVerKey, true, false);
            await CookieX.Save(_perVerKey, newVer);

            return hasPer;
        }

        static async Task<bool> HasPerInternal(string p_permission)
        {
            return await AtLob.GetScalar<int>($"select count(*) from UserPermission where Name='{p_permission}'") > 0;
        }

        /// <summary>
        /// 根据参数id获取用户参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramID"></param>
        /// <returns></returns>
        protected override Task<T> GetParamByID<T>(long p_paramID)
        {
            return ParamsDs.GetParamByID<T>(p_paramID);
        }

        /// <summary>
        /// 根据参数名称获取用户参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramName"></param>
        /// <returns></returns>
        protected override Task<T> GetParamByName<T>(string p_paramName)
        {
            return ParamsDs.GetParamByName<T>(p_paramName);
        }

        /// <summary>
        /// 保存用户参数值
        /// </summary>
        /// <param name="p_paramID"></param>
        /// <param name="p_value"></param>
        /// <returns></returns>
        protected override Task<bool> SaveParams(string p_paramID, string p_value)
        {
            return ParamsDs.SaveParams(p_paramID, p_value);
        }
        #endregion
    }
}