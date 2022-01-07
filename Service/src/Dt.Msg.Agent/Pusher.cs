namespace Dt.Agent
{
    /// <summary>
    /// 消息推送Api
    /// </summary>
    public partial class AtMsg
    {
        /// <summary>
        /// 客户端注册在线推送
        /// </summary>
        /// <param name="p_deviceInfo">客户端设备信息</param>
        /// <returns></returns>
        public static Task<ResponseReader> Register(Dict p_deviceInfo)
        {
            return Kit.ServerStreamRpc(
                "msg",
                "Pusher.Register",
                p_deviceInfo
            );
        }

        /// <summary>
        /// 注销客户端
        /// 1. 早期版本在客户端关闭时会造成多个无关的ClientInfo收到Abort，只能从服务端Abort，升级到.net 5.0后不再出现该现象！！！
        /// 2. 使用客户端 response.Dispose() 主动关闭时，不同平台现象不同，服务端能同步收到uwp关闭消息，但android ios上不行，
        /// 直到再次推送时才发现客户端已关闭，为了保证客户端在线状态实时更新，客户端只能调用该方法！！！
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_sessionID">会话标识，区分同一账号多个登录的情况</param>
        /// <returns></returns>
        public static Task<bool> Unregister(long p_userID, string p_sessionID)
        {
            return Kit.Rpc<bool>(
                "msg",
                "Pusher.Unregister",
                p_userID,
                p_sessionID
            );
        }

        /// <summary>
        /// 判断用户是否在线，查询所有副本
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>false 不在线</returns>
        public static Task<bool> IsOnline(long p_userID)
        {
            return Kit.Rpc<bool>(
                "msg",
                "Pusher.IsOnline",
                p_userID
            );
        }

        /// <summary>
        /// 查询所有副本，获取某账号的所有会话信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>会话信息列表</returns>
        public static Task<List<Dict>> GetAllSessions(long p_userID)
        {
            return Kit.Rpc<List<Dict>>(
                "msg",
                "Pusher.GetAllSessions",
                p_userID
            );
        }

        /// <summary>
        /// 实时获取所有副本的在线用户总数
        /// </summary>
        /// <returns>Dict结构：key为副本id，value为副本会话总数</returns>
        public static Task<Dict> GetOnlineCount()
        {
            return Kit.Rpc<Dict>(
                "msg",
                "Pusher.GetOnlineCount"
            );
        }
    }
}
