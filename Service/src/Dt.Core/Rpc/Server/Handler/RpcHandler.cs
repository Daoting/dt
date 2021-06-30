#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
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
        protected object _tgt;
        #endregion

        public RpcHandler(ApiInvoker p_invoker)
        {
            _invoker = p_invoker;
        }

        /// <summary>
        /// 执行Http Rpc调用
        /// </summary>
        /// <returns></returns>
        public Task<bool> Call()
        {
            // 创建服务实例
            _tgt = Kit.GetObj(_invoker.Api.Method.DeclaringType);
            if (_tgt == null)
            {
                var msg = $"无法创建服务实例，类型{_invoker.Api.Method.DeclaringType.Name}！";
                _invoker.Log.Warning(msg);
                _ = _invoker.Response(ApiResponseType.Error, 0, msg);
                return Task.FromResult(false);
            }
            return CallMethod();
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