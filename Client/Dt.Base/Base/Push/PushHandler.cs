#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 处理服务器推送
    /// </summary>
    internal class PushHandler
    {
        static readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// 该账户从其它位置登录时停止接收推送
        /// </summary>
        public static bool StopPush;

        /// <summary>
        /// 连接推送的重试次数
        /// </summary>
        public static int RetryTimes;

        string _method;
        List<object> _params;

        public PushHandler(string p_msg)
        {
            try
            {
                using (var sr = new StringReader(p_msg))
                using (var jr = new JsonTextReader(sr))
                {
                    // Json Rpc格式错误
                    if (jr.Read()
                        && jr.TokenType == JsonToken.StartArray
                        && !string.IsNullOrEmpty(_method = jr.ReadAsString()))
                    {
                        _method = _method.ToLower();
                        _params = new List<object>();
                        while (jr.Read() && jr.TokenType != JsonToken.EndArray)
                        {
                            _params.Add(JsonRpcSerializer.Deserialize(jr));
                        }
                        AtKit.Trace(TraceOutType.ServerPush, $"{_method}—推送", AtSys.TraceRpc ? p_msg : null);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "解析服务器推送时异常");
            }
        }

        public void Call()
        {
            MethodInfo mi;
            if (string.IsNullOrEmpty(_method) || (mi = GetMethod()) == null)
                return;

            try
            {
                object tgt = Activator.CreateInstance(mi.DeclaringType);
                AtKit.RunAsync(() => mi.Invoke(tgt, _params.ToArray()));
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"调用推送处理方法{_method}时异常");
            }
        }

        MethodInfo GetMethod()
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
