#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Dt.Core.EventBus;
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
            await Online.Unregister(_c.UserID);

            // 记录会话信息，登记会话所属服务id
            ClientInfo ci = new ClientInfo(_c, (ClientSystem)p_clientSys, p_writer);
            Online.All[_c.UserID] = ci;
            await Cache.StringSet(Online.PrefixKey, _c.UserID.ToString(), Glb.ID);
            Cache.Increment(Online.OnlineCountKey);
            Log.Debug($"用户{_c.UserID}注册推送");

            // 推送
            while (await ci.SendMsg())
            { }
        }

        /// <summary>
        /// 注销指定用户客户端的在线推送
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public Task Unregister(long p_userID)
        {
            return Online.Unregister(p_userID);
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

            if (p_msg.PushMode == MsgPushMode.Offline)
            {
                Offline.Send(p_userIDs, p_msg);
                return null;
            }

            List<long> offlines = new List<long>();
            string onlineMsg = p_msg.GetOnlineMsg();
            foreach (var id in p_userIDs)
            {
                // 在线推送
                if (p_msg.PushMode != MsgPushMode.Offline)
                {
                    // 会话在当前服务
                    if (Online.All.TryGetValue(id, out var ci))
                    {
                        ci.AddMsg(onlineMsg);
                        break;
                    }

                    // 查询会话所属的服务副本ID
                    string svcID = await Cache.StringGet<string>(Online.PrefixKey, id.ToString());
                    if (!string.IsNullOrEmpty(svcID))
                    {
                        //Glb.GetSvc<RemoteEventBus>().PushFixed(new OnlinePushEvent { UserID = id, Msg = onlineMsg }, Glb.SvcName, svcID);
                        break;
                    }
                }

                // 离线推送
                if (p_msg.PushMode != MsgPushMode.Online)
                    offlines.Add(id);
            }

            if (offlines.Count > 0)
                Offline.Send(offlines, p_msg);
            return null;
        }
    }
}
