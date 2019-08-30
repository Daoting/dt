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
        static readonly ConcurrentDictionary<long, BlockingCollection<string>> _sessions = new ConcurrentDictionary<long, BlockingCollection<string>>();

        public async Task Register(ResponseWriter p_writer)
        {
            BlockingCollection<string> bc = new BlockingCollection<string>();
            _sessions[_c.UserID] = bc;
            while (bc.TryTake(out var msg))
            {
                if (!await p_writer.Write(msg))
                    break;

            }

            _sessions.TryRemove(_c.UserID, out bc);

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
