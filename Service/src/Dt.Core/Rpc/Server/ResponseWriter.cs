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
        readonly ApiInvoker _invoker;

        internal ResponseWriter(ApiInvoker p_invoker)
        {
            _invoker = p_invoker;
        }

        /// <summary>
        /// 向客户端写入一帧
        /// </summary>
        /// <param name="p_message">支持序列化的对象</param>
        /// <returns></returns>
        public async Task<bool> Write(object p_message)
        {
            // 请求已关闭，无法写入
            if (_invoker.Context.RequestAborted.IsCancellationRequested)
                return false;

            try
            {
                await RpcServerKit.WriteFrame(_invoker.Context.Response.BodyWriter, p_message);
                return true;
            }
            catch { }

            return false;
        }
    }
}
