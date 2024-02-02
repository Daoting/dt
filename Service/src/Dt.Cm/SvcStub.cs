﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 服务存根
    /// </summary>
    public class SvcStub : Stub
    {
        /// <summary>
        /// 服务名称，小写
        /// </summary>
        public override string SvcName
        {
            get => "cm";
            set => throw new Exception("cm服务名称不可修改");
        }

        /// <summary>
        /// 是否允许单体服务模式
        /// </summary>
        public override bool AllowSingleton => false;

        /// <summary>
        /// 定义全局服务
        /// </summary>
        /// <param name="p_services"></param>
        public override void ConfigureServices(IServiceCollection p_services)
        {
            p_services.AddSingleton<SqliteFileHandler>();
        }

        /// <summary>
        /// 自定义请求处理或定义请求管道的中间件
        /// </summary>
        /// <param name="p_app"></param>
        /// <param name="p_handlers">注册自定义请求处理</param>
        public override void Configure(IApplicationBuilder p_app, IDictionary<string, RequestDelegate> p_handlers)
        {
            Kit.GetService<SqliteFileHandler>().Init(p_handlers);
            LoadSvcUrls();
        }

        void LoadSvcUrls()
        {
            // 客户端实体存储使用的默认服务名
            SysKernel.Config["EntitySvcName"] = Kit.Config.GetValue("EntitySvcName", "cm");
            if (Kit.IsSingletonSvc)
            {
                // 单体服务时，只需一个地址
                SysKernel.Config["IsSingletonSvc"] = true;
                return;
            }

            SysKernel.Config["IsSingletonSvc"] = false;
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "etc/config/url.json")))
            {
                Log.Fatal("缺少url.json文件，无法获取所有服务的地址信息");
                return;
            }

            var config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "etc/config"))
                    .AddJsonFile("url.json", false, true)
                    .Build();

            Dict dt = new Dict();
            foreach (var item in config.GetChildren())
            {
                dt[item.Key] = item.Value;
            }
            SysKernel.Config["SvcUrls"] = dt;
            Log.Information("加载所有服务的地址信息");
        }
    }
}
