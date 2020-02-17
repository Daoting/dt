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
    /// 消息服务Api代理类（自动生成）
    /// </summary>
    public static class AtMsg
    {
        #region Pusher
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
                "Pusher.Register",
                p_clientSys
            ).Call();
        }

        /// <summary>
        /// 实时获取所有在线用户，测试用
        /// </summary>
        /// <returns></returns>
        public static Task<string> GetAllOnline()
        {
            return new UnaryRpc(
                "msg",
                "Pusher.GetAllOnline"
            ).Call<string>();
        }
        #endregion
    }
}
