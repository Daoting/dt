namespace Dt.Agent
{
    /// <summary>
    /// 指令消息Api
    /// </summary>
    public partial class AtMsg
    {
        /// <summary>
        /// 向某用户的客户端推送指令信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns>true 在线推送</returns>
        public static Task<bool> SendCmd(long p_userID, MsgInfo p_msg)
        {
            return Kit.Rpc<bool>(
                "msg",
                "CmdMsg.SendCmd",
                p_userID,
                p_msg
            );
        }

        /// <summary>
        /// 向用户列表的所有客户端推送指令信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <param name="p_checkReplica">多副本实例时是否检查其他副本</param>
        /// <returns>在线推送列表</returns>
        public static Task<List<long>> BatchSendCmd(List<long> p_userIDs, MsgInfo p_msg, bool p_checkReplica = true)
        {
            return Kit.Rpc<List<long>>(
                "msg",
                "CmdMsg.BatchSendCmd",
                p_userIDs,
                p_msg,
                p_checkReplica
            );
        }
    }
}
