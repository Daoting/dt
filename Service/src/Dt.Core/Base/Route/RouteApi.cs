#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 路由处理的抽象基类
    /// </summary>
    public abstract class RouteApi
    {
        #region 成员变量
        RouteInvoker _invoker;
        IDataAccess _daInternal;
        #endregion

        #region 属性
        /// <summary>
        /// 获取数据访问对象
        /// </summary>
        protected IDataAccess _da
        {
            get
            {
                if (_daInternal == null)
                {
                    _daInternal = Kit.NewDataAccess(_invoker.Attribute.DbKey);
                    // 不自动关闭连接
                    _daInternal.AutoClose = false;
                }
                return _daInternal;
            }
        }

        /// <summary>
        /// 日志对象，日志中比静态Log类多出路由 和 Api名称
        /// </summary>
        protected ILogger _log => _invoker.Log;

        /// <summary>
        /// 当前路由
        /// </summary>
        protected string _route => _invoker.Attribute.Path;

        /// <summary>
        /// 当前http请求上下文
        /// </summary>
        protected HttpContext _context => _invoker.Context;
        #endregion

        #region 内部方法
        internal void Init(RouteInvoker p_routeInvoker)
        {
            _invoker = p_routeInvoker;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true 自动提交事务，false回滚事务</returns>
        internal async Task<bool> CallHandler()
        {
            string result = null;
            UnaryResult resultType = UnaryResult.Success;
            ApiResponseType responseType = ApiResponseType.Success;
            string error = null;

            // 输出耗时
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                using var reader = new StreamReader(_invoker.Context.Request.Body);
                var body = await reader.ReadToEndAsync();
                result = await Handle(body, _invoker.Context.Request.Query);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException
                    || ex.InnerException is OperationCanceledException)
                {
                    // 客户端取消请求，不记录日志，不Response
                    resultType = UnaryResult.Cancel;
                }
                else
                {
                    resultType = UnaryResult.Error;
                    KnownException rpcEx = ex.InnerException as KnownException;
                    if (rpcEx == null)
                        rpcEx = ex as KnownException;

                    if (rpcEx != null)
                    {
                        // 业务异常，在客户端作为提示消息，不记日志
                        responseType = ApiResponseType.Warning;
                        error = rpcEx.Message;
                    }
                    else
                    {
                        // 程序执行过程的错误，将异常记录日志
                        responseType = ApiResponseType.Error;
                        error = $"路由{_route}出错";
                        if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                        {
                            _invoker.Log.Error(ex.InnerException, error);
                            error += "\r\n" + ex.InnerException.Message;
                        }
                        else
                        {
                            _invoker.Log.Error(ex, error);
                            error += "\r\n" + ex.Message;
                        }
                    }
                }
            }
            finally
            {
                stopwatch.Stop();
            }

            // 取消请求
            if (resultType == UnaryResult.Cancel)
                return false;

            // 输出日志
            if (RpcHandler.TraceRpc)
                _invoker.Log.Information("{0} — {1}ms", _route, stopwatch.ElapsedMilliseconds);

            var response = _invoker.Context.Response;
            if (responseType == ApiResponseType.Success)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    response.ContentType = "text/plain; charset=utf-8";
                    await response.WriteAsync(result);
                }
            }
            else
            {
                response.StatusCode = responseType == ApiResponseType.Warning ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
                if (!string.IsNullOrEmpty(error))
                {
                    response.ContentType = "text/plain; charset=utf-8";
                    await response.WriteAsync(error);
                }
            }
            return resultType == UnaryResult.Success;
        }

        /// <summary>
        /// 调用结束后释放资源，提交或回滚事务、关闭数据库连接
        /// </summary>
        /// <param name="p_suc"></param>
        /// <returns></returns>
        internal Task Close(bool p_suc)
        {
            if (_daInternal != null)
                return _daInternal.Close(p_suc);
            return Task.CompletedTask;
        }
        #endregion

        /// <summary>
        /// 路由处理，不需要捕获异常
        /// </summary>
        /// <param name="p_body">请求内容</param>
        /// <param name="p_query">查询参数</param>
        /// <returns>有内容时自动写入Response；自定义响应内容请直接使用 _context.Response </returns>
        protected abstract Task<string> Handle(string p_body, IQueryCollection p_query);
    }
}