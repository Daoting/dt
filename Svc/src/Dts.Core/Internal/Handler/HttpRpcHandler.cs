#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// Http Rpc请求处理类
    /// </summary>
    public class HttpRpcHandler : RpcHandler
    {
        public HttpRpcHandler(HttpContext p_context)
            : base(p_context)
        {
            _context.Response.ContentType = "text/plain";
            _context.Response.Headers["content-encoding"] = "gzip";
        }

        /// <summary>
        /// 执行Http Rpc调用
        /// </summary>
        /// <returns></returns>
        public async Task Call()
        {
            // 解析调用参数
            using (StreamReader sr = new StreamReader(_context.Request.Body))
            {
                ParseParams(sr);
                if (!string.IsNullOrEmpty(_error))
                {
                    Answer(false);
                    return;
                }
            }

            // 调用目标方法
            await CallMethod();
            if (!string.IsNullOrEmpty(_error))
            {
                Answer(_isMessage);
                return;
            }

            #region 输出结果
            try
            {
                using (GZipStream output = new GZipStream(_context.Response.Body, CompressionMode.Compress))
                using (StreamWriter sr = new StreamWriter(output))
                using (JsonWriter writer = new JsonTextWriter(sr))
                {
                    writer.WriteStartArray();
                    // 0成功，1错误，2警告提示
                    writer.WriteValue(0);
                    // 耗时
                    writer.WriteValue(_elapsed);
                    // 内容
                    JsonRpcSerializer.Serialize(_result, writer);
                    writer.WriteEndArray();
                }
            }
            catch (Exception ex)
            {
                _context.Response.Clear();
                _error = "序列化调用结果时异常";
                _lc.Log.Error(ex, _error);
                Answer(false);
            }
            #endregion
        }

        /// <summary>
        /// 向Http Rpc输出错误或警告信息
        /// </summary>
        /// <param name="p_isMessage"></param>
        void Answer(bool p_isMessage)
        {
            using (GZipStream output = new GZipStream(_context.Response.Body, CompressionMode.Compress))
            using (StreamWriter sr = new StreamWriter(output))
            using (JsonWriter writer = new JsonTextWriter(sr))
            {
                writer.WriteStartArray();
                // 0成功，1错误，2警告提示
                writer.WriteValue(p_isMessage ? 2 : 1);
                // 耗时
                writer.WriteValue(0);
                // 内容
                writer.WriteValue(_error);
                writer.WriteEndArray();
            }
        }
    }
}