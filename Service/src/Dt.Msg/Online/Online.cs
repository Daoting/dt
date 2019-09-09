#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Dt.Core.EventBus;
using System.Collections.Concurrent;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 在线推送类
    /// </summary>
    public static class Online
    {
        /// <summary>
        /// 当前服务的所有在线用户
        /// </summary>
        public static readonly ConcurrentDictionary<long, ClientInfo> All = new ConcurrentDictionary<long, ClientInfo>();

        /// <summary>
        /// 在线客户端前缀
        /// </summary>
        public const string PrefixKey = "cli";

        /// <summary>
        /// 在线总数
        /// </summary>
        public const string OnlineCountKey = "cli:cnt";

        /// <summary>
        /// 注销指定用户的客户端推送
        /// Unregister(Api) -> Pusher.Unregister -> ClientInfo.Close 或 UnregisterPushHandler.Handle
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public static async Task Unregister(long p_userID)
        {
            // 查询会话所属的服务副本ID
            string svcID = await Cache.StringGet<string>(PrefixKey, p_userID.ToString());
            if (string.IsNullOrEmpty(svcID))
                return;

            // 删除会话位置记录
            await Cache.Remove(PrefixKey, p_userID.ToString());

            if (svcID == Glb.ID)
            {
                // 会话在当前服务
                if (All.TryRemove(p_userID, out var ci))
                    ci.Close();
            }
            else
            {
                // 会话在其它服务副本时通过EventBus通知
                //Glb.GetSvc<RemoteEventBus>().PushFixed(new UnregisterPushEvent { UserID = p_userID }, Glb.SvcName, svcID);
            }
        }
    }
}
