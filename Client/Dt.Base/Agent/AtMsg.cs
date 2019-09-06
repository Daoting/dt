#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 系统参数管理类
    /// </summary>
    public static class AtMsg
    {
        #region PushMsg
        /// <summary>
        /// 客户端注册在线推送
        /// </summary>
        /// <param name="p_clientSys">客户端系统</param>
        /// <param name="p_writer"></param>
        /// <returns></returns>
        public static Task<ResponseReader> Register(int p_clientSys)
        {
            return new ServerStreamRpc(
                "msg",
                "PushMsg.Register",
                p_clientSys
            ).Call();
        }

        /// <summary>
        /// 注销指定用户客户端的在线推送
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public static Task Unregister(long p_userID)
        {
            return new UnaryRpc(
                "msg",
                "PushMsg.Unregister",
                p_userID
            ).Call<object>();
        }

        /// <summary>
        /// 向某用户的客户端推送信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns></returns>
        public static Task<string> Send(long p_userID, MsgInfo p_msg)
        {
            return new UnaryRpc(
                "msg",
                "PushMsg.Send",
                p_userID,
                p_msg
            ).Call<string>();
        }

        /// <summary>
        /// 向用户列表的所有客户端推送信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <returns></returns>
        public static Task<string> BatchSend(List<long> p_userIDs, MsgInfo p_msg)
        {
            return new UnaryRpc(
                "msg",
                "PushMsg.BatchSend",
                p_userIDs,
                p_msg
            ).Call<string>();
        }
        #endregion
    }
}
