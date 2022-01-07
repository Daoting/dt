namespace Dt.Agent
{
    /// <summary>
    /// 公共测试的Api
    /// </summary>
    public abstract class SvcTestAgent<TSvc>
    {
        #region TestEventBus
        public static Task Broadcast(List<string> p_svcs, bool p_isAllSvcInst)
        {
            return Kit.Rpc<object>(
                typeof(TSvc).Name,
                "TestEventBus.Broadcast",
                p_svcs,
                p_isAllSvcInst
            );
        }

        public static Task Multicast(string p_svcName)
        {
            return Kit.Rpc<object>(
                typeof(TSvc).Name,
                "TestEventBus.Multicast",
                p_svcName
            );
        }

        public static Task Push(string p_svcName)
        {
            return Kit.Rpc<object>(
                typeof(TSvc).Name,
                "TestEventBus.Push",
                p_svcName
            );
        }

        public static Task PushFixed(string p_svcID)
        {
            return Kit.Rpc<object>(
                typeof(TSvc).Name,
                "TestEventBus.PushFixed",
                p_svcID
            );
        }

        public static Task PushGenericEvent(string p_name)
        {
            return Kit.Rpc<object>(
                typeof(TSvc).Name,
                "TestEventBus.PushGenericEvent",
                p_name
            );
        }

        public static Task LocalPublish()
        {
            return Kit.Rpc<object>(
                typeof(TSvc).Name,
                "TestEventBus.LocalPublish"
            );
        }

        public static Task<string> LocalCall(string p_name)
        {
            return Kit.Rpc<string>(
                typeof(TSvc).Name,
                "TestEventBus.LocalCall",
                p_name
            );
        }

        public static Task<string> TestLoadBalance()
        {
            return Kit.Rpc<string>(
                typeof(TSvc).Name,
                "TestEventBus.TestLoadBalance"
            );
        }
        #endregion

        #region TestLog
        /// <summary>
        /// 记录普通日志
        /// </summary>
        /// <param name="p_msg"></param>
        public static Task LogInfo(string p_msg)
        {
            return Kit.Rpc<object>(
                typeof(TSvc).Name,
                "TestLog.LogInfo",
                p_msg
            );
        }

        /// <summary>
        /// 记录警告信息
        /// </summary>
        public static Task LogWarning()
        {
            return Kit.Rpc<object>(
                typeof(TSvc).Name,
                "TestLog.LogWarning"
            );
        }

        /// <summary>
        /// 记录出错信息
        /// </summary>
        public static Task LogError()
        {
            return Kit.Rpc<object>(
                typeof(TSvc).Name,
                "TestLog.LogError"
            );
        }
        #endregion

        #region TestRpc
        public static Task<string> GetRpcString()
        {
            return Kit.Rpc<string>(
                typeof(TSvc).Name,
                "TestRpc.GetRpcString"
            );
        }

        public static Task<bool> SetRpcString(string p_str)
        {
            return Kit.Rpc<bool>(
                typeof(TSvc).Name,
                "TestRpc.SetRpcString",
                p_str
            );
        }

        public static Task<ResponseReader> OnServerStream(string p_title)
        {
            return Kit.ServerStreamRpc(
                typeof(TSvc).Name,
                "TestRpc.OnServerStream",
                p_title
            );
        }

        public static Task<RequestWriter> OnClientStream(string p_title)
        {
            return Kit.ClientStreamRpc(
                typeof(TSvc).Name,
                "TestRpc.OnClientStream",
                p_title
            );
        }

        public static Task<DuplexStream> OnDuplexStream(string p_title)
        {
            return Kit.DuplexStreamRpc(
                typeof(TSvc).Name,
                "TestRpc.OnDuplexStream",
                p_title
            );
        }
        #endregion
    }
}
