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

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 请求/响应模式的处理类
    /// </summary>
    public class UnaryHandler : RpcHandler
    {
        public UnaryHandler(LobContext p_lc)
            :base(p_lc)
        { }

        /// <summary>
        /// 调用服务方法
        /// </summary>
        /// <returns></returns>
        protected override async Task CallMethod()
        {
            object result = null;
            ApiResponseType responseType = ApiResponseType.Success;
            string error = null;

            // 输出耗时
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var mi = _lc.Api.Method;
                if (mi.ReturnType == typeof(Task))
                {
                    // 异步无返回值时
                    await (Task)mi.Invoke(_tgt, _lc.Args);
                }
                else if (typeof(Task).IsAssignableFrom(mi.ReturnType))
                {
                    // 异步有返回值
                    var task = (Task)mi.Invoke(_tgt, _lc.Args);
                    await task;
                    result = task.GetType().GetProperty("Result").GetValue(task);
                }
                else
                {
                    // 调用同步方法
                    result = mi.Invoke(_tgt, _lc.Args);
                }
            }
            catch (Exception ex)
            {
                // 将异常记录日志
                RpcException rpcEx = ex.InnerException as RpcException;
                if (rpcEx != null)
                {
                    // 业务异常，在客户端作为提示消息，不记日志
                    responseType = ApiResponseType.Warning;
                    error = rpcEx.Message;
                }
                else
                {
                    // 程序执行过程的错误
                    responseType = ApiResponseType.Error;
                    error = $"调用{_lc.ApiName}出错";
                    if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                        _lc.Log.Error(ex.InnerException, error);
                    else
                        _lc.Log.Error(ex, error);
                }
            }
            finally
            {
                stopwatch.Stop();
            }

            if (TraceRpc)
                _lc.Log.Information($"{_lc.ApiName} — {stopwatch.ElapsedMilliseconds}ms");

            await _lc.Response(responseType, stopwatch.ElapsedMilliseconds, error == null ? result : error);
        }
    }
}