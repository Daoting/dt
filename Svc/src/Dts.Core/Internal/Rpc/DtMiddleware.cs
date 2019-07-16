#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 系统内置中间件，完成：
    /// JWT格式的本地认证；
    /// 内部特殊路径处理；
    /// 默认页处理；
    /// </summary>
    public class DtMiddleware
    {
        #region 成员变量
        readonly RequestDelegate _next;
        readonly DefaultFilesOptions _options;
        readonly PathString _matchUrl;
        readonly IFileProvider _fileProvider;
        readonly IAuthenticationSchemeProvider _schemes;
        #endregion

        #region 构造方法
        public DtMiddleware(
            RequestDelegate p_next,
            IHostingEnvironment p_hostingEnv,
            IOptions<DefaultFilesOptions> p_options,
            IAuthenticationSchemeProvider p_schemes)
        {
            _next = p_next ?? throw new ArgumentNullException(nameof(p_next));
            _schemes = p_schemes ?? throw new ArgumentNullException(nameof(p_schemes));

            if (p_hostingEnv == null)
                throw new ArgumentNullException(nameof(p_hostingEnv));
            if (p_options == null)
                throw new ArgumentNullException(nameof(p_options));
            _options = p_options.Value;
            _fileProvider = _options.FileProvider ?? ResolveFileProvider(p_hostingEnv);
            _matchUrl = _options.RequestPath;
        }
        #endregion

        public async Task Invoke(HttpContext p_context)
        {
            // 只本地认证(JWT格式)，未处理远程认证及重定向，原中间件见Authentication.txt
            var authScheme = await _schemes.GetDefaultAuthenticateSchemeAsync();
            if (authScheme != null)
            {
                var result = await p_context.AuthenticateAsync(authScheme.Name);
                if (result?.Principal != null)
                    p_context.User = result.Principal;
            }

            // 内部特殊路径格式：/.xxx
            string path = p_context.Request.Path.Value;
            if (path.StartsWith("/."))
            {
                switch (path.Substring(2).ToLower())
                {
                    case "c":
                        await new HttpRpcHandler(p_context).Call();
                        return;
                    case "admin":
                        await PageFileHandler.Admin(p_context);
                        return;
                    case "error":
                        await PageFileHandler.Error(p_context);
                        return;
                }
            }

            // 默认页
            if ((HttpMethods.IsGet(p_context.Request.Method) || HttpMethods.IsHead(p_context.Request.Method))
                && TryMatchPath(p_context, _matchUrl, out var subpath))
            {
                // HTTP请求方式：
                // GET 请求一个文件，
                // POST 发送数据让Web服务器进行处理，
                // HEAD 检查一个对象是否存在
                // PUT 发送数据并存储在Web服务器内部
                // DELETE 从Web服务器上删除一个文件

                var dirContents = _fileProvider.GetDirectoryContents(subpath.Value);
                if (dirContents.Exists || subpath == "/")
                {
                    // 检查是否存在默认页
                    bool exist = false;
                    if (dirContents.Exists)
                    {
                        for (int matchIndex = 0; matchIndex < _options.DefaultFileNames.Count; matchIndex++)
                        {
                            string defaultFile = _options.DefaultFileNames[matchIndex];
                            var file = _fileProvider.GetFileInfo(subpath.Value + defaultFile);
                            // TryMatchPath 已在末尾添加"/"
                            if (file.Exists)
                            {
                                exist = true;
                                // 若默认页存在且原始路径末尾无"/"，添加"/"后重定向
                                if (!PathEndsInSlash(p_context.Request.Path))
                                {
                                    p_context.Response.StatusCode = 301;
                                    p_context.Response.Headers[HeaderNames.Location] = p_context.Request.PathBase + p_context.Request.Path + "/" + p_context.Request.QueryString;
                                    return;
                                }

                                // 重置请求路径，由后续StaticFileMiddleware处理
                                p_context.Request.Path = new PathString(path + defaultFile);
                                break;
                            }
                        }
                    }

                    // 无默认页时返回空白
                    if (!exist)
                        return;
                }
            }

            await _next(p_context);
        }

        static IFileProvider ResolveFileProvider(IHostingEnvironment p_hostingEnv)
        {
            if (p_hostingEnv.WebRootFileProvider == null)
                throw new InvalidOperationException("Missing FileProvider.");
            return p_hostingEnv.WebRootFileProvider;
        }

        static bool TryMatchPath(HttpContext p_context, PathString p_matchUrl, out PathString p_subpath)
        {
            var path = p_context.Request.Path;
            if (!PathEndsInSlash(path))
                path += new PathString("/");

            if (path.StartsWithSegments(p_matchUrl, out p_subpath))
                return true;
            return false;
        }

        static bool PathEndsInSlash(PathString p_path)
        {
            return p_path.Value.EndsWith("/", StringComparison.Ordinal);
        }
    }
}
