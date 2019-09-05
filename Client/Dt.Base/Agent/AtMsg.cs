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
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 系统参数管理类
    /// </summary>
    public static class AtMsg
    {
        #region InstantMsg
        public static Task<ResponseReader> Register(int p_clientSys)
        {
            return new ServerStreamRpc(
                "msg",
                "PushMsg.Register",
                p_clientSys
            ).Call();
        }

        /// <summary>
        /// 向指定会话推送信息，门户之间调用推送信息
        /// </summary>
        /// <param name="p_userID">会话列表，*表全部</param>
        /// <param name="p_content">推送内容</param>
        public static Task PushMsg(Int64 p_userID, string p_content)
        {
            return new UnaryRpc(
                "msg",
                "PushMsg.SendMsg",
                p_userID,
                p_content
            ).Call<object>();
        }
        #endregion
    }
}
