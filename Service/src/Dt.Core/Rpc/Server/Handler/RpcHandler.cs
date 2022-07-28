#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 请求处理基类
    /// </summary>
    abstract class RpcHandler
    {
        #region 成员变量
        // 是否输出所有调用的Api名称
        internal static bool TraceRpc;

        protected readonly ApiInvoker _invoker;
        protected BaseApi _tgt;
        #endregion

        public RpcHandler(ApiInvoker p_invoker)
        {
            _invoker = p_invoker;
        }

        /// <summary>
        /// 执行Rpc调用
        /// </summary>
        /// <returns></returns>
        public async Task Call()
        {
            // 创建服务实例
            _tgt = Kit.GetService(_invoker.Api.Method.DeclaringType) as BaseApi;
            if (_tgt != null)
            {
                _tgt.UserID = _invoker.UserID;
                _tgt.IsTransactional = _invoker.Api.IsTransactional;

                bool suc = await CallMethod();
                // Api调用结束后释放资源
                await _tgt.Close(suc);
            }
            else
            {
                var msg = $"无法创建服务实例，类型{_invoker.Api.Method.DeclaringType.Name}！";
                _invoker.Log.Warning(msg);
                await _invoker.Response(ApiResponseType.Error, 0, msg);
            }
        }

        /// <summary>
        /// 调用服务方法
        /// </summary>
        /// <returns></returns>
        protected abstract Task<bool> CallMethod();

        /// <summary>
        /// 记录调用过程的错误日志
        /// </summary>
        /// <param name="p_ex"></param>
        protected void LogCallError(Exception p_ex)
        {
            string error = $"调用{_invoker.ApiName}出错";
            if (p_ex.InnerException != null && !string.IsNullOrEmpty(p_ex.InnerException.Message))
                _invoker.Log.Error(p_ex.InnerException, error);
            else
                _invoker.Log.Error(p_ex, error);
        }
    }
}