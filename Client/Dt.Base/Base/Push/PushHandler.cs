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
using Xamarin.Essentials;
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
        static bool _connected;

        /// <summary>
        /// 连接推送的重试次数
        /// </summary>
        public static int RetryTimes;

        /// <summary>
        /// 服务器推送连接断开后的是否停止重连
        /// </summary>
        public static bool StopRetry;

        /// <summary>
        /// 处理服务器推送
        /// </summary>
        /// <returns></returns>
        public static async Task Register()
        {
            if (_connected)
            {
                //AtKit.Msg("已连接");
                return;
            }

            _connected = true;
#if WASM
            Dict dt = new Dict
            {
                { "model", "wasm" },
                { "name", "Chrome" },
                { "platform", "Browser" },
                { "version", "11.0" },
            };
#else
            Dict dt = new Dict
            {
                { "model", DeviceInfo.Model },
                { "name", DeviceInfo.Name },
                { "platform", DeviceInfo.Platform.ToString() },
                { "version", DeviceInfo.VersionString },
            };
#endif

            try
            {
                var reader = await AtMsg.Register(dt);
                RetryTimes = 0;
                while (await reader.MoveNext())
                {
                    new PushHandler().Call(reader.Val<string>());
                }
            }
            catch { }
            finally
            {
                _connected = false;
            }

            // 未停止接收推送时重连
            if (!StopRetry && RetryTimes < 5)
            {
                RetryTimes++;
                //AtKit.Msg($"第{RetryTimes}次重连");
                _ = Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, RetryTimes))).ContinueWith((t) => Register());
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
}
