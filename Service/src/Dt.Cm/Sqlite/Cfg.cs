#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 配置相关
    /// </summary>
    public static class Cfg
    {
        public static IConfiguration Config { get; private set; }

        /// <summary>
        /// 初始化服务配置
        /// </summary>
        public static void Init()
        {
            if (!File.Exists(Path.Combine(Kit.PathBase, "etc/config/cm.json")))
            {
                var ex = new Exception("缺少 cm.json 文件！");
                Log.Fatal(ex, "cm服务启动出错");
                throw ex;
            }

            try
            {
                Config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Kit.PathBase, "etc/config"))
                    .AddJsonFile("cm.json", false, false)
                    .Build();
                Log.Information("读取 cm.json 成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "读取 cm.json 失败！");
                throw;
            }

            // 客户端实体存储使用的默认服务名
            SysKernel.Config["EntitySvcName"] = Config.GetValue("EntitySvcName", "cm");
            if (Kit.IsSingletonSvc)
            {
                // 单体服务时，只需一个地址
                SysKernel.Config["IsSingletonSvc"] = true;
                return;
            }

            SysKernel.Config["IsSingletonSvc"] = false;
            
            Dict dt = new Dict();
            foreach (var item in Config.GetSection("Urls").GetChildren())
            {
                dt[item.Key] = item.Value;
            }
            SysKernel.Config["SvcUrls"] = dt;
            Log.Information("加载所有服务的地址信息");
        }
    }
}
