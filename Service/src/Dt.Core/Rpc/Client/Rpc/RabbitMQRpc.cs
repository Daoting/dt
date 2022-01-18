#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.RabbitMQ;
using RabbitMQ.Client.Events;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text.Json;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 基于RabbitMQ的远程调用
    /// </summary>
    class RabbitMQRpc
    {
        /// <summary>
        /// 正在等待结果的远程调用
        /// </summary>
        public static readonly ConcurrentDictionary<string, TaskCompletionSource<BasicDeliverEventArgs>> RunningCalls = new ConcurrentDictionary<string, TaskCompletionSource<BasicDeliverEventArgs>>();

        /// <summary>
        /// 向RabbitMQ发送json格式的Rpc远程调用，服务名称或服务实例id只能指定一个，提供id时发送给固定副本
        /// </summary>
        /// <typeparam name="T">结果对象的类型</typeparam>
        /// <param name="p_svcName">服务名称</param>
        /// <param name="p_svcID">服务实例id</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        /// <returns>返回远程调用结果</returns>
        public async Task<T> Call<T>(string p_svcName, string p_svcID, string p_methodName, object[] p_params)
        {
            if (string.IsNullOrEmpty(p_methodName))
                throw new InvalidOperationException("Rpc调用时需要指定API名称！");

            // 服务名称或服务实例id只能指定一个
            if ((string.IsNullOrEmpty(p_svcName) && string.IsNullOrEmpty(p_svcID))
                || (!string.IsNullOrEmpty(p_svcName) && !string.IsNullOrEmpty(p_svcID)))
                throw new InvalidOperationException("Rpc调用时需要指定服务名称 或 服务实例id！");

            var data = GetRpcData(p_methodName, p_params);

            var id = Kit.NewGuid;
            var tcs = new TaskCompletionSource<BasicDeliverEventArgs>();
            RunningCalls[id] = tcs;

            var mq = Kit.GetObj<RabbitMQCenter>();
            if (!string.IsNullOrEmpty(p_svcName))
            {
                // 发送给某个服务，服务有多个副本时采用均衡算法将消息投递给其中一个
                mq.Publish(
                    data,
                    $"{Kit.AppName}.{p_svcName.ToLower()}",
                    false,
                    id,
                    $".{Kit.SvcID}");
            }
            else
            {
                // 发送给固定副本
                mq.Publish(
                    data,
                    $".{p_svcID}",
                    true,
                    id,
                    $".{Kit.SvcID}");
            }

            var args = await tcs.Task;
            return ParseResult<T>(args);
        }

        /// <summary>
        /// 序列化RPC调用，按需压缩
        /// </summary>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数</param>
        /// <returns></returns>
        byte[] GetRpcData(string p_methodName, ICollection<object> p_params)
        {
            bool _isCompressed = false;
            byte[] data = RpcKit.GetCallBytes(p_methodName, p_params);

            // 超过长度限制时执行压缩
            if (data.Length > RpcKit.MinCompressLength)
            {
                _isCompressed = true;
                var ms = new MemoryStream();
                using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress))
                {
                    zs.Write(data, 0, data.Length);
                }
                
                data = ms.ToArray();
            }

            byte[] result = new byte[data.Length + RpcKit.HeaderSize];
            // Frame头：1字节压缩标志 + 4字节内容长度
            result[0] = _isCompressed ? (byte)1 : (byte)0;
            BinaryPrimitives.WriteUInt32BigEndian(result.AsSpan(1), (uint)data.Length);
            data.CopyTo(result, 5);
            return result;
        }

        /// <summary>
        /// 解析结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_args"></param>
        /// <returns></returns>
        T ParseResult<T>(BasicDeliverEventArgs p_args)
        {
            // 读取RabbitMQ消息内容，已自动解压
            byte[] data = RpcServerKit.ReadRabbitMQMessage(p_args);

            T val = default(T);
            RpcResult result = new RpcResult();
            Utf8JsonReader reader = new Utf8JsonReader(data);

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

            if (result.ResultType == RpcResultType.Value)
                return val;

            throw new ServerException("服务器异常", result.Info);
        }
    }
}