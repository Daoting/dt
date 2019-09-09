#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 消息推送Api
    /// </summary>
    [Api]
    public class PushMsg : BaseApi
    {
        /// <summary>
        /// 客户端注册在线推送
        /// </summary>
        /// <param name="p_clientSys">客户端系统</param>
        /// <param name="p_writer"></param>
        /// <returns></returns>
        public async Task Register(int p_clientSys, ResponseWriter p_writer)
        {
            // 通知已注册的客户端退出推送
            Online.Unregister(_c.UserID);

            // 记录会话信息，登记会话所属服务id
            ClientInfo ci = new ClientInfo(_c, (ClientSystem)p_clientSys, p_writer);
            Online.All[_c.UserID] = ci;
            Log.Debug($"用户{_c.UserID}注册推送");

            // 推送
            while (await ci.SendMsg())
            { }
        }

        /// <summary>
        /// 向某用户的客户端推送信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns></returns>
        public Task<string> Send(long p_userID, MsgInfo p_msg)
        {
            return BatchSend(new List<long> { p_userID }, p_msg);
        }

        /// <summary>
        /// 向用户列表的所有客户端推送信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <returns></returns>
        public async Task<string> BatchSend(List<long> p_userIDs, MsgInfo p_msg)
        {
            if (p_userIDs == null
                || p_userIDs.Count == 0
                || p_msg == null)
                return "待推送的用户或信息不可为空！";

            string error = p_msg.Validate();
            if (!string.IsNullOrEmpty(error))
                return error;

            // 只离线推送
            if (p_msg.PushMode == MsgPushMode.Offline)
            {
                Offline.Send(p_userIDs, p_msg);
                return null;
            }

            // 在线推送
            List<long> offlines = await Online.Send(p_userIDs, p_msg);

            // 离线推送
            if (offlines.Count > 0)
                Offline.Send(offlines, p_msg);
            return null;
        }
    }
}
