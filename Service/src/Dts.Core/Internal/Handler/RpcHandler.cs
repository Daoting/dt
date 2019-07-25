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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 请求处理基类，异步、高性能、健壮！
    /// </summary>
    public abstract class RpcHandler
    {
        #region 成员变量
        // 是否输出所有调用的Api名称
        internal static bool TraceRpc;

        protected readonly HttpContext _context;
        protected readonly LobContext _lc;
        protected object[] _args;
        protected object _result;
        protected bool _isMessage;
        protected string _error;
        protected long _elapsed;
        #endregion

        public RpcHandler(HttpContext p_context)
        {
            _context = p_context;
            _lc = new LobContext();
            _context.Items["lc"] = _lc;
        }

        /// <summary>
        /// 调用服务方法
        /// </summary>
        /// <returns></returns>
        protected async Task CallMethod()
        {
            _isMessage = false;
            _lc.Api = Silo.GetMethod(_lc.ApiName);
            if (_lc.Api == null)
            {
                // 未找到对应方法
                _error = $"Api方法“{_lc.ApiName}”不存在！";
                _lc.Log.Warning(_error);
                return;
            }

            // 输出耗时
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            BaseApi tgt = null;
            try
            {
                tgt = Glb.GetSvc(_lc.Api.Method.DeclaringType) as BaseApi;
                if (tgt == null)
                    throw new Exception($"类型{_lc.Api.Method.DeclaringType.Name}未继承BaseApi！");

                if (_lc.Api.Usage == ApiMethodUsage.AsyncResult)
                {
                    // 异步有返回值
                    var task = (Task)_lc.Api.Method.Invoke(tgt, _args);
                    await task;
                    _result = task.GetType().GetProperty("Result").GetValue(task);
                }
                else if (_lc.Api.Usage == ApiMethodUsage.SyncMethod)
                {
                    // 调用同步方法
                    _result = _lc.Api.Method.Invoke(tgt, _args);
                }
                else
                {
                    // 异步无返回值时
                    await (Task)_lc.Api.Method.Invoke(tgt, _args);
                }
            }
            catch (Exception ex)
            {
                // 将异常记录日志
                RpcException rpcEx = ex.InnerException as RpcException;
                if (rpcEx != null)
                {
                    // 业务异常，在客户端作为提示消息，不记日志
                    _isMessage = true;
                    _error = rpcEx.Message;
                }
                else
                {
                    // 程序执行过程的错误
                    _error = $"调用{_lc.ApiName}出错";
                    if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                        _lc.Log.Error(ex.InnerException, _error);
                    else
                        _lc.Log.Error(ex, _error);
                }
            }
            finally
            {
                stopwatch.Stop();
                _elapsed = stopwatch.ElapsedMilliseconds;
                if (TraceRpc)
                    _lc.Log.Information($"{_lc.ApiName} — {_elapsed}ms");
            }
        }

        /// <summary>
        /// 反序列化json格式的调用参数
        /// </summary>
        /// <param name="p_tr"></param>
        protected void ParseParams(TextReader p_tr)
        {
            try
            {
                using (JsonReader reader = new JsonTextReader(p_tr))
                {
                    if (!reader.Read()
                        || reader.TokenType != JsonToken.StartArray
                        || !reader.Read()
                        || reader.TokenType != JsonToken.String
                        || string.IsNullOrEmpty(_lc.ApiName = (string)reader.Value))
                        throw new Exception("Json Rpc格式错误！");

                    List<object> objs = new List<object>();
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        objs.Add(JsonRpcSerializer.Deserialize(reader));
                    }
                    if (objs.Count > 0)
                        _args = objs.ToArray();
                }
            }
            catch (Exception ex)
            {
                _error = "反序列化请求参数时异常";
                _lc.Log.Error(ex, _error);
            }
        }

        /// <summary>
        /// 获取向调用方返回的结果，json格式
        /// </summary>
        /// <returns></returns>
        protected string GetResult()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                using (StringWriter sw = new StringWriter(sb))
                using (JsonWriter writer = new JsonTextWriter(sw))
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
                _error = "序列化调用结果时异常";
                _lc.Log.Error(ex, _error);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取向调用方返回的错误或警告信息，json格式
        /// </summary>
        /// <param name="p_isMessage"></param>
        /// <returns></returns>
        protected string GetFaultResult(bool p_isMessage)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sw))
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
            return sb.ToString();
        }
    }
}