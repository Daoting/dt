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

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 请求处理基类
    /// </summary>
    public abstract class RpcHandler
    {
        #region 成员变量
        // 是否输出所有调用的Api名称
        internal static bool TraceRpc;

        protected readonly LobContext _lc;
        protected BaseApi _tgt;
        #endregion

        public RpcHandler(LobContext p_lc)
        {
            _lc = p_lc;
        }

        /// <summary>
        /// 执行Http Rpc调用
        /// </summary>
        /// <returns></returns>
        public Task Call()
        {
            // 校验授权
            if (!IsAuthenticated())
                return _lc.Response(ApiResponseType.Error, 0, "未经授权");

            // 创建服务实例
            _tgt = Glb.GetSvc(_lc.Api.Method.DeclaringType) as BaseApi;
            if (_tgt == null)
            {
                var msg = $"类型{_lc.Api.Method.DeclaringType.Name}未继承BaseApi！";
                _lc.Log.Warning(msg);
                return _lc.Response(ApiResponseType.Error, 0, msg);
            }

            return CallMethod();
        }

        /// <summary>
        /// 调用服务方法
        /// </summary>
        /// <returns></returns>
        protected abstract Task CallMethod();

        /// <summary>
        /// 记录调用过程的错误日志
        /// </summary>
        /// <param name="p_ex"></param>
        protected void LogCallError(Exception p_ex)
        {
            string error = $"调用{_lc.ApiName}出错";
            if (p_ex.InnerException != null && !string.IsNullOrEmpty(p_ex.InnerException.Message))
                _lc.Log.Error(p_ex.InnerException, error);
            else
                _lc.Log.Error(p_ex, error);
        }

        bool IsAuthenticated()
        {
            return true;
        }
    }
}