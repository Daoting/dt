#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 离线推送类
    /// </summary>
    public static class Offline
    {
        static readonly BlockingCollection<OfflineItem> _queue;
        //static readonly WnsHandler _wns;

        static Offline()
        {
            // 初始化WNS
            //_wns = new WnsHandler();

            _queue = new BlockingCollection<OfflineItem>();
            Task.Run(PushMsg);
        }

        /// <summary>
        /// 增加待推送信息
        /// </summary>
        /// <param name="p_userIDs"></param>
        /// <param name="p_msg"></param>
        public static bool Add(List<long> p_userIDs, MsgInfo p_msg)
        {
            // 不再重复校验
            return _queue.TryAdd(new OfflineItem { Users = p_userIDs, Msg = p_msg });
        }

        /// <summary>
        /// 后台推送任务
        /// </summary>
        /// <returns></returns>
        static void PushMsg()
        {
            try
            {
                while (_queue.TryTake(out var item))
                {
                    PushHandler(item);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "离线推送后台任务已停止");
            }
        }

        /// <summary>
        /// 执行推送
        /// </summary>
        /// <param name="p_item"></param>
        static void PushHandler(OfflineItem p_item)
        {
            // 提取允许推送的用户列表


            try
            {
                //byte[] data = p_msg.GetToastMsg();
                //foreach (var id in p_userIDs)
                //{
                //    bool valid = _wns.PostToWns(db.HashGet(key, "channel"), data);
                //    // uri无效时清空
                //    if (!valid)
                //    {
                //        db.HashSet(key, "channel", "");
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Toast多设备推送异常");
            }
        }
    }
}
