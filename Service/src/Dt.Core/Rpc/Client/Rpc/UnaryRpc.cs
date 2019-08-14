#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.IO;
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
            try
            {
                using (var request = CreateRequestMessage())
                using (var content = new PushStreamContent((ws) => RpcKit.WriteFrame(ws, _data, _isCompressed)))
                {
                    request.Content = content;
                    var response = await _client.SendAsync(request).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var stream = await response.Content.ReadAsStreamAsync();
                    data = await RpcKit.ReadFrame(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"调用【{_methodName}】时服务器连接失败！\r\n{ex.Message}");
            }

            // 解析结果
            RpcResult result = new RpcResult();
            using (MemoryStream ms = new MemoryStream(data))
            using (StreamReader sr = new StreamReader(ms))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                try
                {
                    // [
                    reader.Read();
                    // 0成功，1错误，2警告提示
                    result.ResultType = (RpcResultType)reader.ReadAsInt32();
                    // 耗时，非调试状态为0
                    result.Elapsed = reader.ReadAsString();
                    if (result.ResultType == RpcResultType.Value)
                    {
                        reader.Read();
                        result.Value = JsonRpcSerializer.Deserialize(reader);
                    }
                    else
                    {
                        // 错误或提示信息
                        result.Info = reader.ReadAsString();
                    }
                }
                catch
                {
                    result.ResultType = RpcResultType.Error;
                    result.Info = "返回Json内容结构不正确！";
                }
            }

            // 返回结果
            T val = default(T);
            if (result.ResultType == RpcResultType.Value)
            {
                if (result.Value == null)
                {
                    // 空值
                }
                else if (typeof(T) == result.Value.GetType())
                {
                    // 结果对象与给定对象类型相同时
                    val = (T)result.Value;
                }
                else
                {
                    // 特殊处理结果对象与给定对象类型不相同时
                    try
                    {
                        val = (T)Convert.ChangeType(result.Value, typeof(T));
                    }
                    catch (Exception convExp)
                    {
                        throw new Exception($"调用【{_methodName}】对返回结果类型转换时异常：\r\n {result.Value.GetType()}-->{typeof(T)}：{convExp.Message}");
                    }
                }
            }
            else
            {
                throw new Exception($"调用【{_methodName}】异常：\r\n{result.Info}");
            }
            return val;
        }
    }
}