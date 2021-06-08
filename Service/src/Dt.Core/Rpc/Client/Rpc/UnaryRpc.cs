#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 基于Http2的请求/响应模式的远程调用
    /// </summary>
    public class UnaryRpc : BaseRpc
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        public UnaryRpc(string p_serviceName, string p_methodName, params object[] p_params)
            : base(p_serviceName, p_methodName, p_params)
        { }

        /// <summary>
        /// 发送json格式的Http Rpc远程调用
        /// </summary>
        /// <typeparam name="T">结果对象的类型</typeparam>
        /// <returns>返回远程调用结果</returns>
        public async Task<T> Call<T>()
        {
            // 远程请求
            byte[] data = null;
            using (var request = CreateRequestMessage())
            using (var content = new PushStreamContent((ws) => RpcClientKit.WriteFrame(ws, _data, _isCompressed)))
            {
                request.Content = content;
                HttpResponseMessage response;
                try
                {
                    response = await _client.SendAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new ServerException("服务器连接失败", $"调用【{_methodName}】时服务器连接失败！\r\n{ex.Message}");
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
#if !SERVER
                    // 无权限时
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // 已登录则提示无权限
                        if (Kit.IsLogon)
                            throw new KnownException($"⚡对【{_methodName}】无访问权限！");

                        // 跳转到登录页面
                        Kit.Login(true);
                        throw new KnownException("请先登录您的账号！");
                    }
#endif
                    throw new ServerException($"服务器返回状态码：{response.StatusCode}", $"调用【{_methodName}】时返回状态码：{response.StatusCode}");
                }

                var stream = await response.Content.ReadAsStreamAsync();
                data = await RpcClientKit.ReadFrame(stream);
                response.Dispose();
            }
            return ParseResult<T>(data);
        }

        /// <summary>
        /// 解析结果，Utf8JsonReader不能用在异步方法内！
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_data"></param>
        /// <returns></returns>
        T ParseResult<T>(byte[] p_data)
        {
            T val = default(T);
            RpcResult result = new RpcResult();
            Utf8JsonReader reader = new Utf8JsonReader(p_data);

            try
            {
                // [
                reader.Read();

                // 0成功，1错误，2警告提示
                result.ResultType = (RpcResultType)reader.ReadAsInt();

                // 耗时，非调试状态为0
                reader.Read();
                result.Elapsed = reader.GetInt32().ToString();

                reader.Read();
                if (result.ResultType == RpcResultType.Value)
                {
                    // 结果
                    val = JsonRpcSerializer.Deserialize<T>(ref reader);
                }
                else
                {
                    // 错误或提示信息
                    result.Info = reader.GetString();
                }
            }
            catch
            {
                result.ResultType = RpcResultType.Error;
                result.Info = "返回Json内容结构不正确！";
            }

#if !SERVER
            // 输出监视信息
            string content = null;
            if (Kit.TraceRpc || result.ResultType == RpcResultType.Error)
            {
                // 输出详细内容
                content = Encoding.UTF8.GetString(p_data);
            }
            Kit.Trace(TraceOutType.RpcRecv, string.Format("{0}—{1}ms", _methodName, result.Elapsed), content, _svcName);

            // ⚡ 为服务器标志
            if (result.ResultType == RpcResultType.Message)
                throw new KnownException("⚡" + result.Info);
#endif

            if (result.ResultType == RpcResultType.Value)
                return val;
            throw new ServerException("服务器异常", result.Info);
        }
    }
}