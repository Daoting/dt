#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Text.Json;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 初始化模式处理接收到的消息
    /// </summary>
    class InitModeInvoker
    {
        const string _errParse = "反序列化请求参数时异常";
        readonly HttpContext _context;
        MethodInfo _method;
        object[] _args;

        public InitModeInvoker(HttpContext p_context)
        {
            _context = p_context;
            _context.Response.ContentType = "application/dt";
        }

        /// <summary>
        /// 处理http rpc请求
        /// </summary>
        /// <returns></returns>
        public async Task Handle()
        {
            // 读取一帧，已自动解压
            byte[] data = await RpcServerKit.ReadFrame(_context.Request.BodyReader);

            // 解析参数
            if (!await ParseParams(data))
                return;

            object result = null;
            var responseType = ApiResponseType.Success;
            string error = null;
            var tgt = new InitModeApi();

            try
            {
                if (_method.ReturnType == typeof(Task))
                {
                    // 异步无返回值时
                    var task = (Task)_method.Invoke(tgt, _args);
                    task.Wait(_context.RequestAborted);
                }
                else if (typeof(Task).IsAssignableFrom(_method.ReturnType))
                {
                    // 异步有返回值
                    var task = (Task)_method.Invoke(tgt, _args);
                    task.Wait(_context.RequestAborted);
                    result = task.GetType().GetProperty("Result").GetValue(task);
                }
                else
                {
                    // 调用同步方法
                    result = _method.Invoke(tgt, _args);
                }
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException
                    || ex.InnerException is OperationCanceledException)
                {
                    // 客户端取消请求，不记录日志，不Response
                    return;
                }

                responseType = ApiResponseType.Error;
                error = $"调用{_method.Name}出错\r\n" + ex.Message;
                Log.Fatal(ex, $"调用{_method.Name}出错");
            }

            await Response(responseType, error == null ? result : error);
        }

        /// <summary>
        /// 反序列化json格式的调用参数
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        async Task<bool> ParseParams(byte[] p_data)
        {
            try
            {
                DoParse(p_data);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, _errParse);
                await Response(ApiResponseType.Error, _errParse);
                return false;
            }
        }

        void DoParse(byte[] p_data)
        {
            // Utf8JsonReader不能用在异步方法内！
            Utf8JsonReader reader = new Utf8JsonReader(p_data);

            // [
            reader.Read();
            var apiName = reader.ReadAsString();
            if (string.IsNullOrEmpty(apiName)
                || (_method = typeof(InitModeApi).GetMethod(apiName, BindingFlags.Public | BindingFlags.Instance)) == null)
            {
                // 未找到对应方法
                throw new Exception($"Api方法“{apiName}”不存在！");
            }

            var method = _method.GetParameters();
            if (method.Length > 0)
            {
                // 确保和Api的参数个数、类型相同
                // 类型不同时 执行类型转换 或 直接创建派生类实例！如Row -> User, Table -> Table<User>
                int index = 0;
                _args = new object[method.Length];
                while (index < method.Length && reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    // 参数支持派生类型！
                    _args[index] = JsonRpcSerializer.Deserialize(ref reader, method[index].ParameterType);
                    index++;
                }
            }
        }

        /// <summary>
        /// 向客户端输出响应
        /// </summary>
        /// <param name="p_responseType">结果标志：0成功，1错误，2警告提示</param>
        /// <param name="p_content">内容</param>
        /// <returns></returns>
        Task Response(ApiResponseType p_responseType, object p_content)
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
                        // 内容
                        JsonRpcSerializer.Serialize(p_content, writer);

                        writer.WriteEndArray();
                    }
                    data = stream.ToArray();
                }

                // 写入响应流
                return RpcServerKit.WriteFrame(_context.Response.BodyWriter, data, false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "向客户端输出信息时异常！");
            }
            return Task.CompletedTask;
        }
    }
}