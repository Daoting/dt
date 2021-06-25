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

            Log.Debug("{0} 离线", ci.UserID);
        }

        /// <summary>
        /// 注销客户端
        /// 1. 早期版本在客户端关闭时会造成多个无关的ClientInfo收到Abort，只能从服务端Abort，升级到.net 5.0后不再出现该现象！！！
        /// 2. 使用客户端 response.Dispose() 主动关闭时，不同平台现象不同，服务端能同步收到uwp关闭消息，但android ios上不行，
        ///    直到再次推送时才发现客户端已关闭，为了保证客户端状态正确，主动关闭后需要调用该方法！！！
        /// </summary>
        /// <returns></returns>
        public bool Unregister(long p_userID)
        {
            ClientInfo ci = Online.GetClient(p_userID);
            if (ci != null)
            {
                ci.Close();
                return true;
            }

            // 查询所有其他副本
            if (MsgKit.IsMultipleReplicas)
            {

            }
            return false;
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
