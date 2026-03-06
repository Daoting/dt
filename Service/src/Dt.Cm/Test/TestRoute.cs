#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
#endregion

namespace Dt.Cm
{
    [Route("/.test/1/普通路由")]
    public class TestRoute0 : RouteApi
    {
        protected override Task<string> Handle(string p_body, IQueryCollection p_query)
        {
            return Task.FromResult($"请求内容：{p_body} \n查询参数：{p_query.Count}个\n当前路由：{_route}\n处理类名：TestRoute0");
        }
    }
    
    [Route("/.test/2/多路由统一处理1")]
    [Route("/.test/3/多路由统一处理2")]
    public class TestRoute1 : RouteApi
    {
        protected override Task<string> Handle(string p_body, IQueryCollection p_query)
        {
            return Task.FromResult($"当前路由：{_route}，处理类：TestRoute1");
        }
    }
    
    [Route("/.test/4/路由处理类异常")]
    public class TestRoute2 : RouteApi
    {
        public TestRoute2()
        {
            throw new NotImplementedException();
        }
        
        protected override async Task<string> Handle(string p_body, IQueryCollection p_query)
        {
            return null;
        }
    }

    [Route("/.test/5/业务异常")]
    public class TestRoute3 : RouteApi
    {
        protected override Task<string> Handle(string p_body, IQueryCollection p_query)
        {
            throw new KnownException("这是一个业务异常");
        }
    }

    [Route("/.test/6/内部异常")]
    public class TestRoute4 : RouteApi
    {
        protected override Task<string> Handle(string p_body, IQueryCollection p_query)
        {
            throw new Exception("内部异常测试");
        }
    }

    [Route("/.test/7/查询数据")]
    public class TestRoute5 : RouteApi
    {
        protected override async Task<string> Handle(string p_body, IQueryCollection p_query)
        {
            var tbl = await _da.Query("select * from cm_user");
            return Kit.Serialize(tbl);
        }
    }

    [Route("/.test/8/自定义输出")]
    public class TestRoute6 : RouteApi
    {
        protected override async Task<string> Handle(string p_body, IQueryCollection p_query)
        {
            var tbl = await _da.Query("select * from cm_user");
            var str = Kit.Serialize(tbl);
            _context.Response.ContentType = "application/json";
            await _context.Response.WriteAsync(str);
            return null;
        }
    }
}
