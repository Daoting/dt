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
                var ex = "缺少 cm.json 文件！";
                Log.Fatal(ex);
                throw new Exception(ex);
            }

            try
            {
                Config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Kit.PathBase, "etc/config"))
                    .AddJsonFile("cm.json", false, false)
                    .Build();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "读取 cm.json 失败！");
                throw;
            }

            if (Kit.IsSingletonSvc)
            {
                // 单体服务时，只需一个地址
                SysKernel.Config["IsSingletonSvc"] = true;
                Log.Information("cm：单体服务模式");
                return;
            }

            SysKernel.Config["IsSingletonSvc"] = false;

            Dict dt = new Dict();
            foreach (var item in Config.GetSection("Urls").GetChildren())
            {
                dt[item.Key] = item.Value;
            }
            SysKernel.Config["SvcUrls"] = dt;
            Log.Information("cm：微服务模式");
        }
    }
}
