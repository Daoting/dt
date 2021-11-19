
namespace Dt.Core
{
    /// <summary>
    /// 输出信息的种类
    /// </summary>
    public enum TraceOutType
    {
        /// <summary>
        /// 普通信息
        /// </summary>
        Normal,

        /// <summary>
        /// 远程调用请求信息
        /// </summary>
        RpcCall,

        /// <summary>
        /// 远程调用响应信息
        /// </summary>
        RpcRecv,

        /// <summary>
        /// WebSocket远程调用
        /// </summary>
        WsCall,

        /// <summary>
        /// WebSocket响应信息
        /// </summary>
        WsRecv,

        /// <summary>
        /// 服务器推送
        /// </summary>
        ServerPush,

        /// <summary>
        /// 远程调用异常信息
        /// </summary>
        RpcException,

        /// <summary>
        /// 未处理异常信息
        /// </summary>
        UnhandledException
    }
}
