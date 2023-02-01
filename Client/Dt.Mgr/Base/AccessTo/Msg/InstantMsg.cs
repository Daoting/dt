namespace Dt.Mgr
{
    /// <summary>
    /// 即时消息Api
    /// </summary>
    public partial class AtMsg
    {
        /// <summary>
        /// 向某用户的客户端推送系统消息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns>true 在线推送</returns>
        public static Task<bool> SendMsg(long p_userID, string p_msg)
        {
            return Kit.Rpc<bool>(
                "msg",
                "InstantMsg.SendMsg",
                p_userID,
                p_msg
            );
        }

        /// <summary>
        /// 向用户列表的所有客户端推送系统消息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <returns>在线推送列表</returns>
        public static Task<List<long>> BatchSendMsg(List<long> p_userIDs, string p_msg)
        {
            return Kit.Rpc<List<long>>(
                "msg",
                "InstantMsg.BatchSendMsg",
                p_userIDs,
                p_msg
            );
        }

        /// <summary>
        /// 向所有副本的所有在线用户广播信息
        /// </summary>
        /// <param name="p_msg"></param>
        /// <param name="p_checkReplica">多副本实例时是否检查其他副本</param>
        public static Task<T> SendMsgToOnline<T>(string p_msg, bool p_checkReplica = true)
            where T : class
        {
            return Kit.Rpc<T>(
                "msg",
                "InstantMsg.SendMsgToOnline",
                p_msg,
                p_checkReplica
            );
        }

        /// <summary>
        /// 向用户列表的所有客户端推送聊天信息，可通过指定LetterInfo.LetterType为Undo撤回信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_letter">聊天信息</param>
        /// <returns>在线推送列表</returns>
        public static Task<List<long>> BatchSendLetter(List<long> p_userIDs, LetterInfo p_letter)
        {
            return Kit.Rpc<List<long>>(
                "msg",
                "InstantMsg.BatchSendLetter",
                p_userIDs,
                p_letter
            );
        }
    }
}
