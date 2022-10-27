#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-12-31 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 服务地址
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 调用服务API
        /// </summary>
        /// <typeparam name="T">结果对象的类型</typeparam>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        /// <returns>返回远程调用结果</returns>
        public static Task<T> Rpc<T>(string p_serviceName, string p_methodName, params object[] p_params)
        {
            return new UnaryRpc(
                p_serviceName,
                p_methodName,
                p_params
            ).Call<T>();
        }

        /// <summary>
        /// 客户端发送一个请求，服务端返回数据流响应
        /// </summary>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        /// <returns>返回数据流响应</returns>
        public static Task<ResponseReader> ServerStreamRpc(string p_serviceName, string p_methodName, params object[] p_params)
        {
            return new ServerStreamRpc(
                p_serviceName,
                p_methodName,
                p_params
            ).Call();
        }

        /// <summary>
        /// 客户端发送请求数据流，服务端返回一个响应
        /// </summary>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        /// <returns>返回响应</returns>
        public static Task<RequestWriter> ClientStreamRpc(string p_serviceName, string p_methodName, params object[] p_params)
        {
            return new ClientStreamRpc(
                p_serviceName,
                p_methodName,
                p_params
            ).Call();
        }

        /// <summary>
        /// 客户端发送请求数据流，服务端返回数据流响应
        /// </summary>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        /// <returns>返回数据流响应</returns>
        public static Task<DuplexStream> DuplexStreamRpc(string p_serviceName, string p_methodName, params object[] p_params)
        {
            return new DuplexStreamRpc(
                p_serviceName,
                p_methodName,
                p_params
            ).Call();
        }

        /// <summary>
        /// 获取Rpc的单例HttpClient
        /// </summary>
        public static HttpClient RpcClient => BaseRpc.Client;

        /// <summary>
        /// 是否使用服务
        /// </summary>
        public static bool IsUsingSvc => GetService<IRpcConfig>() != null;

        /// <summary>
        /// 获取服务地址
        /// </summary>
        /// <param name="p_svcName">服务名称，如cm</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetSvcUrl(string p_svcName)
        {
            return GetRequiredService<IRpcConfig>().GetSvcUrl(p_svcName);
        }
    }
}