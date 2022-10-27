namespace Dt.Agent
{
    /// <summary>
    /// 订阅消息Api
    /// </summary>
    public partial class AtMsg
    {
        /// <summary>
        /// 发布订阅信息
        /// </summary>
        /// <param name="p_subscribeID">订阅号标识</param>
        /// <param name="p_msg">信息内容</param>
        /// <param name="p_offlineTip">离线推送时的提示信息</param>
        /// <returns>在线收到的人数</returns>
        public static Task<int> Publish(long p_subscribeID, string p_msg, string p_offlineTip)
        {
            return Kit.Rpc<int>(
                "msg",
                "SubscribeMsg.Publish",
                p_subscribeID,
                p_msg,
                p_offlineTip
            );
        }
    }
}
