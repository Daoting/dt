#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Text;
#endregion

namespace Dt.Fsm;

/// <summary>
/// 服务存根
/// </summary>
public class SvcStub : Stub
{
    /// <summary>
    /// 当前微服务http post请求的最大长度，0时采用默认28.6M
    /// </summary>
    public override long MaxRequestBodySize => 1073741824;

    /// <summary>
    /// 定义全局服务
    /// </summary>
    /// <param name="p_services"></param>
    public override void ConfigureServices(IServiceCollection p_services)
    {
        // 解决Multipart body length limit 134217728 exceeded
        p_services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = int.MaxValue;
            x.MultipartBodyLengthLimit = int.MaxValue;
        });

        // 增加浏览目录功能
        p_services.AddDirectoryBrowser();
    }

    /// <summary>
    /// 自定义请求处理或定义请求管道的中间件
    /// </summary>
    /// <param name="p_app"></param>
    /// <param name="p_handlers">注册根路由处理</param>
    public override void Configure(IApplicationBuilder p_app, IDictionary<string, RequestDelegate> p_handlers)
    {
        Cfg.Init();

        // 注册请求路径处理
        p_handlers["/.u"] = (p_context) => new Uploader(p_context).Handle();
        p_handlers["/.d"] = (p_context) => new Downloader(p_context).Handle();

        // 设置可浏览目录的根目录
        var fileProvider = new PhysicalFileProvider(Cfg.Root);
        p_app.UseDirectoryBrowser(new DirectoryBrowserOptions
        {
            FileProvider = fileProvider,
            RequestPath = "/drv"
        });

        // drive下的所有文件作为网站静态文件，映射到虚拟目录drv，和wwwroot区分
        p_app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = fileProvider,
            RequestPath = "/drv"
        });
    }

    /// <summary>
    /// 获取服务的描述信息，启动时输出到日志和控制台
    /// </summary>
    /// <returns></returns>
    public override string GetDesc()
    {
        StringBuilder sb = new();
        if (Cfg.FixedVolumes?.Count > 0)
        {
            sb.Append("固定卷(");
            foreach (var volume in Cfg.FixedVolumes)
            {
                sb.Append(" ");
                sb.Append(volume);
            }
            sb.Append(" )，");
        }

        sb.Append("普通卷(");
        foreach (var volume in Cfg.Volumes)
        {
            sb.Append(" ");
            sb.Append(volume);
        }
        sb.Append(" )");
        return sb.ToString();
    }
}