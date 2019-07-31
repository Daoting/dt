#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 请求处理基类
    /// </summary>
    public abstract class RpcHandler
    {
        #region 成员变量
        // 是否输出所有调用的Api名称
        internal static bool TraceRpc;

        protected readonly LobContext _lc;
        protected BaseApi _tgt;
        #endregion

        public RpcHandler(LobContext p_lc)
        {
            _lc = p_lc;
        }

        /// <summary>
        /// 执行Http Rpc调用
        /// </summary>
        /// <returns></returns>
        public async Task Call()
        {
            // 校验授权
            if (!IsAuthenticated())
            {
                await _lc.Response(ApiResponseType.Error, 0, "未经授权");
                return;
            }

            // 创建服务实例
            _tgt = Glb.GetSvc(_lc.Api.Method.DeclaringType) as BaseApi;
            if (_tgt == null)
            {
                var msg = $"类型{_lc.Api.Method.DeclaringType.Name}未继承BaseApi！";
                _lc.Log.Warning(msg);
                await _lc.Response(ApiResponseType.Error, 0, msg);
                return;
            }

            await CallMethod();
        }

        /// <summary>
        /// 调用服务方法
        /// </summary>
        /// <returns></returns>
        protected abstract Task CallMethod();

        /// <summary>
        /// 记录调用过程的错误日志
        /// </summary>
        /// <param name="p_ex"></param>
        protected void LogCallError(Exception p_ex)
        {
            string error = $"调用{_lc.ApiName}出错";
            if (p_ex.InnerException != null && !string.IsNullOrEmpty(p_ex.InnerException.Message))
                _lc.Log.Error(p_ex.InnerException, error);
            else
                _lc.Log.Error(p_ex, error);
        }

        protected async Task EndResponse()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sr = new StringWriter(sb))
                using (JsonWriter writer = new JsonTextWriter(sr))
                {
                    writer.WriteStartArray();
                    // 0成功，1错误，2警告提示
                    writer.WriteValue(0);
                    // 耗时
                    writer.WriteValue(0);
                    // 内容
                    writer.WriteNull();
                    writer.WriteEndArray();
                    writer.Flush();
                }

                // 压缩内容
                var ms = new MemoryStream();
                using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress))
                {
                    var data = Encoding.UTF8.GetBytes(sb.ToString());
                    zs.Write(data, 0, data.Length);
                }

                var response = _lc.Context.Response;
                if (!response.HasStarted)
                {
                    response.ContentType = "text/plain";
                    response.Headers["content-encoding"] = "gzip";
                    await response.StartAsync();
                }

                await response.BodyWriter.WriteAsync(ms.ToArray());
                await response.BodyWriter.FlushAsync();
            }
            catch (Exception ex)
            {
                _lc.Log.Error(ex, "向客户端输出信息时异常！");
            }
        }

        bool IsAuthenticated()
        {
            return true;
        }
    }
}