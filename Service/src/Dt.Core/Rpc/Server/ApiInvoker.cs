#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 统一处理json格式的http rpc请求
    /// </summary>
    class ApiInvoker
    {
        #region 成员变量
        const string _errParse = "反序列化请求参数时异常";
        ILogger _logger;
        #endregion

        #region 构造方法
        public ApiInvoker(HttpContext p_context)
        {
            Context = p_context;
            // 内容标志
            Context.Response.ContentType = "application/dt";

            if (long.TryParse(Context.Request.Headers["uid"], out var id))
                UserID = id;
        }
        #endregion

        #region 属性
        /// <summary>
        /// http请求上下文
        /// </summary>
        public HttpContext Context { get; }

        /// <summary>
        /// 获取当前用户标识
        /// </summary>
        public long UserID { get; } = -1;

        /// <summary>
        /// 日志对象
        /// </summary>
        public ILogger Log
        {
            get
            {
                if (_logger == null)
                    _logger = Serilog.Log.ForContext(new ApiLogEnricher(this));
                return _logger;
            }
        }

        /// <summary>
        /// 当前Api方法
        /// </summary>
        public ApiMethod Api { get; private set; }

        /// <summary>
        /// 调用的Api名称
        /// </summary>
        public string ApiName { get; private set; }

        /// <summary>
        /// Api方法参数
        /// </summary>
        public object[] Args { get; private set; }
        #endregion

        /// <summary>
        /// 处理http rpc请求
        /// </summary>
        /// <returns></returns>
        public async Task Handle()
        {
            // 解析rpc参数
            if (!await ParseParams())
                return;

            // 校验授权
            if (!await IsAuthenticated())
            {
                // 未授权
                Context.Response.StatusCode = 401;
                return;
            }

            // 流模式先返回心跳帧，心跳帧为第一帧，避免客户端认为连接超时
            if (Api.CallMode != ApiCallMode.Unary)
                await RpcServerKit.WriteHeartbeat(Context.Response.BodyWriter);

            // 创建整个http请求期间有效的数据包，提供不同位置共享对象
            Bag bag = new Bag(this);
            bool isSuc = true;
            switch (Api.CallMode)
            {
                case ApiCallMode.Unary:
                    isSuc = await new UnaryHandler(this).Call();
                    break;
                case ApiCallMode.ServerStream:
                    isSuc = await new ServerStreamHandler(this).Call();
                    break;
                case ApiCallMode.ClientStream:
                    isSuc = await new ClientStreamHandler(this).Call();
                    break;
                case ApiCallMode.DuplexStream:
                    isSuc = await new DuplexStreamHandler(this).Call();
                    break;
            }
            // Api调用结束后释放资源
            await bag.Close(isSuc);
        }

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
                return RpcServerKit.WriteFrame(Context.Response.BodyWriter, data, compress);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "向客户端输出信息时异常！");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 反序列化json格式的调用参数，请求的第一帧
        /// </summary>
        /// <returns></returns>
        async Task<bool> ParseParams()
        {
            try
            {
                byte[] data = await RpcServerKit.ReadFrame(Context.Request.BodyReader);
                DoParse(data);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, _errParse);
                await Response(ApiResponseType.Error, 0, _errParse);
                return false;
            }
        }

        void DoParse(byte[] p_data)
        {
            // Utf8JsonReader不能用在异步方法内！
            Utf8JsonReader reader = new Utf8JsonReader(p_data);

            // [
            reader.Read();
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

        /// <summary>
        /// 校验授权
        /// </summary>
        /// <returns></returns>
        Task<bool> IsAuthenticated()
        {
            // 不检查授权情况，所有访问都可通过
            if (Api.Auth == null)
                return Task.FromResult(true);

            // 固定特权标识，内部服务之间调用时或admin页面使用该标识
            if (UserID == 110)
                return Task.FromResult(true);

            // 外部自定义校验授权方法
            if (Api.Auth.CustomAuthType != null)
            {
                var custom = Activator.CreateInstance(Api.Auth.CustomAuthType) as ICustomAuth;
                return custom.IsAuthenticated(Context);
            }

            // 所有登录用户
            return Task.FromResult(UserID != -1);
        }
    }
}
