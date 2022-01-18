#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Serilog;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Rpc相关
    /// </summary>
    public partial class Kit
    {
        static readonly Dictionary<string, string> _svcUrls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 调用服务API(RabbitMQ Rpc)，单体服务时本地直接调用
        /// </summary>
        /// <typeparam name="T">结果对象的类型</typeparam>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        /// <returns>返回远程调用结果</returns>
        /// <exception cref="Exception"></exception>
        public static Task<T> Rpc<T>(string p_serviceName, string p_methodName, params object[] p_params)
        {
            if (!IsSingletonSvc && !Stubs[0].SvcName.Equals(p_serviceName, StringComparison.OrdinalIgnoreCase))
            {
                // 非单体且非本服务时，远程调用
                return new RabbitMQRpc().Call<T>(
                    p_serviceName,
                    null,
                    p_methodName,
                    p_params
                );
            }

            // 单体服务，本地直接调用
            return new NativeApiInvoker().Call<T>(p_methodName, p_params);
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
        /// 获取服务地址
        /// </summary>
        /// <param name="p_svcName">服务名称，如cm</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetSvcUrl(string p_svcName)
        {
            if (_svcUrls.TryGetValue(p_svcName, out var url))
                return url;

            throw new Exception($"[{p_svcName}]服务地址不存在！");
        }

        /// <summary>
        /// 加载global.json中的所有微服务地址，修改后动态更新
        /// </summary>
        static void LoadSvcUrls()
        {
            foreach (var item in _config.GetSection("AllSvcUrls").GetChildren())
            {
                _svcUrls[item.Key] = item.Value;
            }
        }
    }
}
