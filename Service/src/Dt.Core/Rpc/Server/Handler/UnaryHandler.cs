#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Diagnostics;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 请求/响应模式的处理类
    /// </summary>
    class UnaryHandler : RpcHandler
    {
        public UnaryHandler(ApiInvoker p_invoker)
            : base(p_invoker)
        { }

        /// <summary>
        /// 调用服务方法
        /// </summary>
        /// <returns></returns>
        protected override async Task<bool> CallMethod()
        {
            object result = null;
            UnaryResult resultType = UnaryResult.Success;
            ApiResponseType responseType = ApiResponseType.Success;
            string error = null;

            // 输出耗时
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var mi = _invoker.Api.Method;
                if (mi.ReturnType == typeof(Task))
                {
                    // 异步无返回值时
                    var task = (Task)mi.Invoke(_tgt, _invoker.Args);
                    task.Wait(_invoker.Context.RequestAborted);
                }
                else if (typeof(Task).IsAssignableFrom(mi.ReturnType))
                {
                    // 异步有返回值
                    var task = (Task)mi.Invoke(_tgt, _invoker.Args);
                    task.Wait(_invoker.Context.RequestAborted);
                    result = task.GetType().GetProperty("Result").GetValue(task);
                }
                else
                {
                    // 调用同步方法
                    result = mi.Invoke(_tgt, _invoker.Args);
                }
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
                        error = $"调用{_invoker.ApiName}出错";
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

            if (resultType != UnaryResult.Cancel)
            {
                if (TraceRpc)
                    _invoker.Log.Information("{0} — {1}ms", _invoker.ApiName, stopwatch.ElapsedMilliseconds);

                await _invoker.Response(responseType, stopwatch.ElapsedMilliseconds, error == null ? result : error);
            }
            return resultType == UnaryResult.Success;
        }
    }

    enum UnaryResult
    {
        /// <summary>
        /// 调用成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 调用过程中出错
        /// </summary>
        Error = 1,

        /// <summary>
        /// 取消调用
        /// </summary>
        Cancel = 2,
    }
}