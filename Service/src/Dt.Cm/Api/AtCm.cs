#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cm;
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Dt.Agent
{
    /// <summary>
    /// 发布服务Api
    /// </summary>
    public static class AtCm
    {
        #region Entry
        /// <summary>
        /// 密码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_pwd">密码</param>
        /// <returns></returns>
        public static Task<Row> LoginByPwd(string p_phone, string p_pwd)
        {
            return Kit.Rpc<Row>(
                "cm",
                "Entry.LoginByPwd",
                p_phone,
                p_pwd
            );
        }

        /// <summary>
        /// 验证码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_code">验证码</param>
        /// <returns></returns>
        public static Task<Row> LoginByCode(string p_phone, string p_code)
        {
            return Kit.Rpc<Row>(
                "cm",
                "Entry.LoginByCode",
                p_phone,
                p_code
            );
        }

        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="p_phone"></param>
        /// <returns></returns>
        public static Task<string> CreateVerificationCode(string p_phone)
        {
            return Kit.Rpc<string>(
                "cm",
                "Entry.CreateVerificationCode",
                p_phone
            );
        }
        #endregion

        #region TestStreamRpc
        public static Task<ResponseReader> OnServerStream(string p_title)
        {
            return Kit.ServerStreamRpc(
                "cm",
                "TestStreamRpc.OnServerStream",
                p_title
            );
        }

        public static Task<RequestWriter> OnClientStream(string p_title)
        {
            return Kit.ClientStreamRpc(
                "cm",
                "TestStreamRpc.OnClientStream",
                p_title
            );
        }

        public static Task<DuplexStream> OnDuplexStream(string p_title)
        {
            return Kit.DuplexStreamRpc(
                "cm",
                "TestStreamRpc.OnDuplexStream",
                p_title
            );
        }
        #endregion
    }
}
