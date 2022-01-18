#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
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
    /// 处理接收到的 RabbitMQ Rpc消息
    /// </summary>
    class HttpApiInvoker : ApiInvoker
    {
        readonly HttpContext _context;

        public HttpApiInvoker(HttpContext p_context)
        {
            _context = p_context;
            // 内容标志
            _context.Response.ContentType = "application/dt";

            if (long.TryParse(_context.Request.Headers["uid"], out var id))
                UserID = id;
        }

        /// <summary>
        /// 取消请求的令牌
        /// </summary>
        public override CancellationToken RequestAborted => _context.RequestAborted;

        public HttpContext Context => _context;

        /// <summary>
        /// 处理http rpc请求
        /// </summary>
        /// <returns></returns>
        public async Task Handle()
        {
            // 读取一帧，已自动解压
            byte[] data = await RpcServerKit.ReadFrame(_context.Request.BodyReader);

            // 解析rpc参数
            if (!await ParseParams(data))
                return;

            // 校验授权
            if (!await IsAuthenticated())
            {
                // 未授权
                _context.Response.StatusCode = 401;
                return;
            }

            // 流模式先返回心跳帧，心跳帧为第一帧，避免客户端认为连接超时
            if (Api.CallMode != ApiCallMode.Unary)
                await RpcServerKit.WriteHeartbeat(_context.Response.BodyWriter);

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


        protected override Task WriteResponse(byte[] p_data, bool p_compress)
        {
            return RpcServerKit.WriteFrame(_context.Response.BodyWriter, p_data, p_compress);
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
                return custom.IsAuthenticated(_context);
            }

            // 所有登录用户
            return Task.FromResult(UserID != -1);
        }
    }
}