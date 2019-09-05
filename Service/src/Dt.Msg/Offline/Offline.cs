#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
#endregion

namespace Dt.Msg
{
    /// <summary>
    /// 离线推送类
    /// </summary>
    public static class Offline
    {
        static WnsHandler _wns;

        /// <summary>
        /// 向用户列表的所有客户端推送信息
        /// </summary>
        /// <param name="p_userIDs">用户列表</param>
        /// <param name="p_msg">待推送信息</param>
        public static void Send(List<long> p_userIDs, MsgInfo p_msg)
        {
            // 参数不再重复检查
            Task.Run(() =>
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
            });

        }

        /// <summary>
        /// 初始化离线推送
        /// </summary>
        internal static void Init()
        {
            //_wns = new WnsHandler();
        }
    }
}
