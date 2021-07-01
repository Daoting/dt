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
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 订阅消息Api
    /// </summary>
    [Api]
    public class SubscribeMsg
    {
        /// <summary>
        /// 发布订阅信息
        /// </summary>
        /// <param name="p_subscribeID">订阅号标识</param>
        /// <param name="p_msg">信息内容</param>
        /// <param name="p_offlineTip">离线推送时的提示信息</param>
        /// <returns>在线收到的人数</returns>
        public async Task<int> Publish(long p_subscribeID, string p_msg, string p_offlineTip)
        {
            string key = "msg:Subscribe:" + p_subscribeID.ToString();
            List<long> users = await Kit.ListRange<long>("msg:Subscribe", p_subscribeID);
            if (users == null || users.Count == 0)
                return 0;

            var mi = new MsgInfo
            {
                MethodName = "",
                Params = new List<object> { p_subscribeID, p_msg },
                Title = "订阅信息",
                Content = p_offlineTip
            };
            var result = await MsgKit.Push(users, mi);
            return result.Count;
        }
    }
}
