#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 指令消息Api
    /// </summary>
    [Api]
    public class CmdMsg
    {
        /// <summary>
        /// 向某用户的客户端推送指令信息
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_msg"></param>
        /// <returns>true 在线推送</returns>
        public async Task<bool> SendCmd(long p_userID, MsgInfo p_msg)
        {
            var result = await MsgKit.Push(new List<long> { p_userID }, p_msg);
            return result.Count > 0;
        }

        /// <summary>
        /// 向用户列表的所有客户端推送指令信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        /// <returns>在线推送列表</returns>
        public Task<List<long>> BatchSendCmd(List<long> p_userIDs, MsgInfo p_msg)
        {
            return MsgKit.Push(p_userIDs, p_msg);
        }
    }
}
