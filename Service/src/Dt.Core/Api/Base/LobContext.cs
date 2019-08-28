#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using Dt.Core.Rpc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 业务线处理上下文
    /// </summary>
    public class LobContext
    {
        #region 成员变量
        const string _lcName = "LobContext";
        const string _errParse = "反序列化请求参数时异常";
        ILogger _logger;
        Db _defaultDb;
        Dictionary<string, Db> _dbs;
        bool _intercepted;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_context"></param>
        internal LobContext(HttpContext p_context)
        {
            Context = p_context;
            // 在服务中通过静态Current取出
            Context.Items[_lcName] = this;
            // 内容标志
            Context.Response.ContentType = "application/dt";
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前业务线上下文
        /// </summary>
        public static LobContext Current => (LobContext)Glb.HttpContext.Items[_lcName];

        /// <summary>
        /// http请求上下文
        /// </summary>
        public HttpContext Context { get; }

        /// <summary>
        /// 获取mysql默认库
        /// </summary>
        public Db Db
        {
            get
            {
                if (_defaultDb == null)
                {
                    _defaultDb = new Db(false);
                    if (Api.IsTransactional)
                        _defaultDb.BeginTrans().Wait();
                }
                return _defaultDb;
            }
        }

        /// <summary>
        /// 日志对象
        /// </summary>
        public ILogger Log
        {
            get
            {
                if (_logger == null)
                    _logger = Serilog.Log.ForContext(new LobLogEnricher(this, Glb.HttpContext.User.FindFirst("uid")?.Value));
                return _logger;
            }
        }

        /// <summary>
        /// 本地事件总线
        /// </summary>
        public LocalEventBus Local
        {
            get { return Glb.GetSvc<LocalEventBus>(); }
        }

        /// <summary>
        /// 远程事件总线
        /// </summary>
        public RemoteEventBus Remote
        {
            get { return Glb.GetSvc<RemoteEventBus>(); }
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
        /// 当前拦截状态
        /// </summary>
        public InterceptStatus Status { get; private set; }

        internal object[] Args { get; private set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 获取当前用户标识
        /// </summary>
        public long GetUserID()
        {
            long id = -1;
            var claim = Glb.HttpContext.User.FindFirst("uid");
            if (claim != null)
                long.TryParse(claim.Value, out id);
            return id;
        }

        /// <summary>
        /// 根据键名获取Db对象
        /// </summary>
        /// <param name="p_dbKey">数据源键名，在json配置DbList节</param>
        /// <returns></returns>
        public Db GetDbByKey(string p_dbKey)
        {
            Check.NotNull(p_dbKey);
            if (_dbs == null)
                _dbs = new Dictionary<string, Db>();

            Db db;
            if (!_dbs.TryGetValue(p_dbKey, out db))
            {
                db = new Db(p_dbKey, false);
                if (Api.IsTransactional)
                    db.BeginTrans().Wait();
                _dbs[p_dbKey] = db;
            }
            return db;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 是否已被拦截过，确保只拦截一次
        /// </summary>
        /// <returns></returns>
        internal bool IsIntercepted()
        {
            if (_intercepted)
                return true;

            _intercepted = true;
            return false;
        }

        /// <summary>
        /// 拦截结束，提交或回滚事务、关闭数据库连接
        /// </summary>
        /// <param name="p_suc"></param>
        /// <returns></returns>
        internal async Task Complete(bool p_suc)
        {
            if (Status != InterceptStatus.Intercepting)
                return;

            Status = p_suc ? InterceptStatus.Successful : InterceptStatus.Failed;
            if (_defaultDb != null)
            {
                await _defaultDb.Close(p_suc);
                _defaultDb = null;
            }

            if (_dbs != null && _dbs.Count > 0)
            {
                foreach (var db in _dbs.Values)
                {
                    await db.Close(p_suc);
                }
                _dbs.Clear();
            }
        }

        /// <summary>
        /// 处理http rpc请求
        /// </summary>
        /// <returns></returns>
        internal async Task Handle()
        {
            // 解析rpc参数
            if (!await ParseParams())
                return;

            // 获取Api
            Api = Silo.GetMethod(ApiName);
            if (Api == null)
            {
                // 未找到对应方法
                var msg = $"Api方法“{ApiName}”不存在！";
                Log.Warning(msg);
                await Response(ApiResponseType.Error, 0, msg);
                return;
            }

            // 校验授权
            if (!IsAuthenticated())
            {
                await Response(ApiResponseType.Error, 0, "未经授权");
                return;
            }

            switch (Api.CallMode)
            {
                case ApiCallMode.Unary:
                    await new UnaryHandler(this).Call();
                    break;
                case ApiCallMode.ServerStream:
                    await new ServerStreamHandler(this).Call();
                    break;
                case ApiCallMode.ClientStream:
                    // 返回心跳帧为第一帧，可能仅响应一帧
                    await RpcServerKit.WriteHeartbeat(Context.Response.BodyWriter);
                    await new ClientStreamHandler(this).Call();
                    break;
                case ApiCallMode.DuplexStream:
                    await new DuplexStreamHandler(this).Call();
                    break;
            }
        }

        /// <summary>
        /// 向客户端输出响应
        /// </summary>
        /// <param name="p_responseType">结果标志：0成功，1错误，2警告提示</param>
        /// <param name="p_elapsed">耗时</param>
        /// <param name="p_content">内容</param>
        /// <returns></returns>
        internal Task Response(ApiResponseType p_responseType, long p_elapsed, object p_content)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sr = new StringWriter(sb))
                using (JsonWriter writer = new JsonTextWriter(sr))
                {
                    writer.WriteStartArray();

                    // 0成功，1错误，2警告提示
                    writer.WriteValue((int)p_responseType);

                    // 耗时
                    writer.WriteValue(p_elapsed);

                    // 内容
                    if (p_content is string str)
                        writer.WriteValue(str);
                    else
                        JsonRpcSerializer.Serialize(p_content, writer);

                    writer.WriteEndArray();
                    writer.Flush();
                }
                var data = Encoding.UTF8.GetBytes(sb.ToString());
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
                using (MemoryStream ms = new MemoryStream(data))
                using (StreamReader sr = new StreamReader(ms))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    if (!reader.Read()
                        || reader.TokenType != JsonToken.StartArray
                        || !reader.Read()
                        || reader.TokenType != JsonToken.String
                        || string.IsNullOrEmpty(ApiName = (string)reader.Value))
                        throw new Exception("Json Rpc格式错误！");

                    List<object> objs = new List<object>();
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        objs.Add(JsonRpcSerializer.Deserialize(reader));
                    }
                    if (objs.Count > 0)
                        Args = objs.ToArray();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, _errParse);
                await Response(ApiResponseType.Error, 0, _errParse);
                return false;
            }
        }

        /// <summary>
        /// 校验授权
        /// </summary>
        /// <returns></returns>
        bool IsAuthenticated()
        {
            return true;
        }
        #endregion
    }

    /// <summary>
    /// 拦截状态
    /// </summary>
    public enum InterceptStatus
    {
        /// <summary>
        /// 拦截中
        /// </summary>
        Intercepting,

        /// <summary>
        /// 成功
        /// </summary>
        Successful,

        /// <summary>
        /// 过程中异常
        /// </summary>
        Failed
    }
}
