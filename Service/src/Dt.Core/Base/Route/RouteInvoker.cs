#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using Polly;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 路由处理类的包装类
    /// </summary>
    class RouteInvoker
    {
        Type _handler;
        RouteAttribute _attribute;
        ILogger _logger;

        public RouteInvoker(Type p_handler, RouteAttribute p_attribute)
        {
            _handler = p_handler;
            _attribute = p_attribute;
        }

        public ILogger Log
        {
            get
            {
                if (_logger == null)
                {
                    _logger = Serilog.Log
                        .ForContext("route", _attribute.Path)
                        .ForContext("handler", _handler.Name);
                }
                return _logger;
            }
        }

        public HttpContext Context { get; private set; }
        
        public RouteAttribute Attribute => _attribute;

        public async Task Handle(HttpContext p_context)
        {
            try
            {
                Context = p_context;
                var api = ((RouteApi)Activator.CreateInstance(_handler));
                api.Init(this);

                bool suc = await api.CallHandler();

                // Api调用结束后释放资源
                await api.Close(suc);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{_handler.Name}] 路由处理异常，路径：{p_context.Request.Path}");
                p_context.Response.StatusCode = 500;
            }
        }
    }
}
