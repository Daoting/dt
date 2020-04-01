#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 处理服务器推送
    /// </summary>
    internal class PushHandler
    {
        #region 静态内容
        static readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        // 连接推送的重试次数
        static int _retryTimes;

        /// <summary>
        /// 服务器推送连接断开后的重连策略
        /// </summary>
        public static PushRetryState RetryState = PushRetryState.Enable;

        /// <summary>
        /// 处理服务器推送
        /// </summary>
        /// <returns></returns>
        public static async Task Register()
        {
            try
            {
                var reader = await AtMsg.Register((int)AtSys.System);
                _retryTimes = 0;
                while (await reader.MoveNext())
                {
                    new PushHandler().Call(reader.Val<string>());
                }
            }
            catch { }

            // 未停止接收推送时重连
            if (RetryState == PushRetryState.Enable && _retryTimes < 5)
            {
                _retryTimes++;
                _ = Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, _retryTimes))).ContinueWith((t) => Register());
            }
        }
        #endregion

        public void Call(string p_msg)
        {
            if (string.IsNullOrEmpty(p_msg))
            {
                Log.Warning("服务器推送内容为空");
                return;
            }

            string method = "";
            MethodInfo mi = null;
            object[] args = null;
            try
            {
                var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(p_msg));
                // [
                reader.Read();
                method = reader.ReadAsString();

                if (string.IsNullOrEmpty(method) || (mi = GetMethod(method.ToLower())) == null)
                {
                    Log.Warning($"服务器推送方法 {method} 不存在");
                    return;
                }

                var pars = mi.GetParameters();
                if (pars.Length > 0)
                {
                    // 确保和Api的参数个数、类型相同
                    // 类型不同时 执行类型转换 或 直接创建派生类实例！如Row -> User, Table -> Table<User>
                    int index = 0;
                    args = new object[pars.Length];
                    while (index < pars.Length && reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        // 参数支持派生类型！
                        args[index] = JsonRpcSerializer.Deserialize(ref reader, pars[index].ParameterType);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "解析服务器推送时异常");
            }

            AtKit.Trace(TraceOutType.ServerPush, $"{method}—推送", AtSys.TraceRpc ? p_msg : null);

            try
            {
                object tgt = Activator.CreateInstance(mi.DeclaringType);
                AtKit.RunAsync(() => mi.Invoke(tgt, args));
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"调用推送处理方法{method}时异常");
            }
        }

        MethodInfo GetMethod(string _method)
        {
            MethodInfo mi;
            if (_methods.TryGetValue(_method, out mi))
                return mi;

            Type tp;
            string[] arr = _method.Split('.');
            if (arr.Length == 2 && AtSys.Stub.PushHandlers.TryGetValue(arr[0], out tp))
            {
                mi = tp.GetMethod(arr[1], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (mi != null)
                    _methods[_method] = mi;
                return mi;
            }
            return null;
        }
    }

    /// <summary>
    /// 服务器推送连接断开后的重连策略
    /// </summary>
    enum PushRetryState
    {
        /// <summary>
        /// 允许重连
        /// </summary>
        Enable,

        /// <summary>
        /// 停止重连
        /// </summary>
        Stop,

        /// <summary>
        /// 禁止重连
        /// </summary>
        Disable
    }
}
