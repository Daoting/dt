#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 返回客户端的响应流
    /// </summary>
    public class ResponseWriter
    {
        LobContext _lc;

        internal ResponseWriter(LobContext p_lc)
        {
            _lc = p_lc;
        }

        /// <summary>
        /// 向客户端写入一帧
        /// </summary>
        /// <param name="p_message">支持序列化的对象</param>
        /// <returns></returns>
        public async Task<bool> Write(object p_message)
        {
            // 请求已关闭，无法写入
            if (_lc.Context.RequestAborted.IsCancellationRequested)
                return false;

            try
            {
                await RpcServerKit.WriteFrame(_lc.Context.Response.BodyWriter, p_message);
                return true;
            }
            catch { }

            return false;
        }
    }
}
