#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog;
using System.IO.Compression;
using System.Text.Json;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 统一处理json格式的rpc请求
    /// </summary>
    abstract class ApiInvoker
    {
        #region 成员变量
        protected const string _errParse = "反序列化请求参数时异常";
        ILogger _logger;
        #endregion

        #region 属性
        /// <summary>
        /// 日志对象
        /// </summary>
        public ILogger Log
        {
            get
            {
                if (_logger == null)
                    _logger = Serilog.Log.ForContext(new ApiLogEnricher(ApiName, UserID));
                return _logger;
            }
        }

        /// <summary>
        /// 当前Api方法
        /// </summary>
        public ApiMethod Api { get; protected set; }

        /// <summary>
        /// 调用的Api名称
        /// </summary>
        public string ApiName { get; protected set; }

        /// <summary>
        /// Api方法参数
        /// </summary>
        public object[] Args { get; protected set; }

        /// <summary>
        /// 获取当前用户标识，UI客户端rpc为实际登录用户ID
        /// <para>特殊标识：110为admin页面，111为RabbitMQ rpc，112为本地调用</para>
        /// </summary>
        public virtual long UserID { get; }

        /// <summary>
        /// 获取取消令牌
        /// </summary>
        public virtual CancellationToken RequestAborted { get; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string SvcName { get; private set; }
        #endregion

        /// <summary>
        /// 向客户端输出响应
        /// </summary>
        /// <param name="p_responseType">结果标志：0成功，1错误，2警告提示</param>
        /// <param name="p_elapsed">耗时</param>
        /// <param name="p_content">内容</param>
        /// <returns></returns>
        public Task Response(ApiResponseType p_responseType, long p_elapsed, object p_content)
        {
            try
            {
                byte[] data;
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
                    {
                        writer.WriteStartArray();

                        // 0成功，1错误，2警告提示
                        writer.WriteNumberValue((int)p_responseType);
                        // 耗时
                        writer.WriteNumberValue(p_elapsed);
                        // 内容
                        JsonRpcSerializer.Serialize(p_content, writer);

                        writer.WriteEndArray();
                    }
                    data = stream.ToArray();
                }
                bool compress = data.Length > RpcKit.MinCompressLength;

                // 超过长度限制时执行压缩
                if (compress)
                {
                    var ms = new MemoryStream();
                    using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress))
                    {
                        zs.Write(data, 0, data.Length);
                    }
                    data = ms.ToArray();
                }

                // 写入响应流
                return WriteResponse(data, compress);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "向客户端输出信息时异常！");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 输出到客户端
        /// </summary>
        /// <param name="p_data"></param>
        /// <param name="p_compress"></param>
        /// <returns></returns>
        protected abstract Task WriteResponse(byte[] p_data, bool p_compress);

        /// <summary>
        /// 反序列化json格式的调用参数
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        protected async Task<bool> ParseParams(byte[] p_data)
        {
            try
            {
                DoParse(p_data);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, _errParse);
                await Response(ApiResponseType.Error, 0, _errParse);
                return false;
            }
        }

        protected void DoParse(byte[] p_data)
        {
            // Utf8JsonReader不能用在异步方法内！
            Utf8JsonReader reader = new Utf8JsonReader(p_data);

            // [
            reader.Read();
            SvcName = reader.ReadAsString();
            ApiName = reader.ReadAsString();
            if (string.IsNullOrEmpty(ApiName) || (Api = Silo.GetMethod(ApiName)) == null)
            {
                // 未找到对应方法
                throw new Exception($"Api方法“{ApiName}”不存在！");
            }

            var method = Api.Method.GetParameters();
            if (method.Length > 0)
            {
                // 确保和Api的参数个数、类型相同
                // 类型不同时 执行类型转换 或 直接创建派生类实例！如Row -> User, Table -> Table<User>
                int index = 0;
                Args = new object[method.Length];
                while (index < method.Length && reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    // 参数支持派生类型！
                    Args[index] = JsonRpcSerializer.Deserialize(ref reader, method[index].ParameterType);
                    index++;
                }
            }
        }
    }
}
