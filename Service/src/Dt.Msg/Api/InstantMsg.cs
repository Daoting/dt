#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Cache;
using Dt.Core.Rpc;
using Dt.Core.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 即时通信Api
    /// </summary>
    [Api]
    public class InstantMsg : BaseApi
    {
        static readonly ConcurrentDictionary<long, ClientInfo> _sessions = new ConcurrentDictionary<long, ClientInfo>();

        public async Task Register(ClientSystem p_clientSys, ResponseWriter p_writer)
        {
            string svcID = await new StringCache("").Get<string>(_c.UserID.ToString());
            ClientInfo ci;
            if (_sessions.TryGetValue(_c.UserID, out ci))
                ci.Logout();

            ci = new ClientInfo(_c.UserID, p_clientSys);
            _sessions[_c.UserID] = ci;
            MsgInfo msg;
            while ((msg = ci.Take()) != null)
            {
                if (msg.Type == MsgType.Logout)
                    return;


                if (!await msg.Write(p_writer))
                    break;

            }

            _sessions.TryRemove(_c.UserID, out ci);

        }

        /// <summary>
        /// 向在线用户广播信息
        /// </summary>
        /// <param name="p_msg"></param>
        public void PushToAll(string p_msg)
        {

        }

        /// <summary>
        /// 向指定会话推送信息，门户之间调用推送信息
        /// </summary>
        /// <param name="p_sessionIDs">会话列表，*表全部</param>
        /// <param name="p_content">推送内容</param>
        public void PushMsg(string p_sessionIDs, string p_content)
        {

        }
    }
}
