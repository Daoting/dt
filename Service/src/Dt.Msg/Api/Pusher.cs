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
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 消息推送Api
    /// </summary>
    [Api]
    public class Pusher
    {
        /// <summary>
        /// 客户端注册在线推送
        /// </summary>
        /// <param name="p_deviceInfo">客户端设备信息</param>
        /// <param name="p_writer"></param>
        /// <returns></returns>
        public async Task Register(Dict p_deviceInfo, ResponseWriter p_writer)
        {
            var ci = new ClientInfo(p_deviceInfo, p_writer);
            // 注册新会话，注销同一用户的旧会话，向新会话发送离线信息
            await Online.Register(ci);

            // 推送
            while (await ci.SendMsg())
            { }

            // 推送结束，删除会话
            var curClient = Online.GetClient(ci.UserID);
            if (curClient != null && curClient == ci)
            {
                // 必须比较Online中该用户的会话是否与当前会话相同！
                // 注册新会话时，若需要注销旧会话，存在删除新会话的可能！
                Online.RemoveClient(ci.UserID);
            }

            Log.Debug("离线：" + ci.UserID);
        }

        /// <summary>
        /// 注销客户端，因客户端直接关闭app时会造成http2连接关闭，该连接下的所有Register推送都结束！！！只能从服务端Abort
        /// </summary>
        /// <returns></returns>
        public Task Unregister()
        {
            ClientInfo ci = Online.GetClient(Bag.UserID);
            if (ci != null)
            {
                ci.Close();
                return Task.CompletedTask;
            }

            // 查询所有其他副本
            if (MsgKit.IsMultipleReplicas)
            {

                return Task.Delay(50);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 判断用户是否在线，查询所有副本
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>null 不在线</returns>
        public async Task<Dict> IsOnline(long p_userID)
        {
            ClientInfo ci = Online.GetClient(p_userID);
            if (ci != null)
            {
                return new Dict
                {
                    { "userid", ci.UserID },
                    { "svcid", Glb.ID },
                    { "starttime", ci.StartTime.ToString() },
                    { "platform", ci.Platform },
                    { "version", ci.Version },
                    { "devicename", ci.DeviceName },
                    { "devicemodel", ci.DeviceModel },
                };
            }

            // 查询所有其他副本
            if (MsgKit.IsMultipleReplicas)
            {
                string key = $"ci:{p_userID}:{Guid.NewGuid().ToString().Substring(0, 6)}";
                Glb.GetSvc<RemoteEventBus>().Multicast(new ClientInfoEvent { CacheKey = key, UserID = p_userID }, Glb.SvcName);
                // 等待收集
                await Task.Delay(500);

                var db = Redis.Db;
                var hash = db.HashGetAll(key);
                if (hash != null)
                {
                    db.KeyDelete(key);
                    return hash.ToDict();
                }
            }
            return null;
        }

        /// <summary>
        /// 实时获取所有副本的在线用户总数
        /// </summary>
        /// <returns>Dict结构：key为副本id，value为副本会话总数</returns>
        public async Task<Dict> GetOnlineCount()
        {
            if (MsgKit.IsMultipleReplicas)
            {
                string key = "cicount:" + Guid.NewGuid().ToString().Substring(0, 6);
                Glb.GetSvc<RemoteEventBus>().Multicast(new OnlineCountEvent { CacheKey = key }, Glb.SvcName);
                // 等待收集
                await Task.Delay(500);

                var db = Redis.Db;
                var hash = db.HashGetAll(key);
                if (hash != null)
                {
                    db.KeyDelete(key);
                    return hash.ToDict();
                }
            }

            // 单副本
            return new Dict { { Glb.ID, Online.All.Count } };
        }
    }
}
