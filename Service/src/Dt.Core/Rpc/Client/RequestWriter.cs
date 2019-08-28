#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 向服务器的写入流
    /// </summary>
    public class RequestWriter
    {
        readonly ClientStreamRpc _rpc;

        internal RequestWriter(ClientStreamRpc p_rpc)
        {
            _rpc = p_rpc;
        }

        /// <summary>
        /// 向服务端写入一帧
        /// </summary>
        /// <param name="p_message">支持序列化的对象</param>
        /// <returns></returns>
        public async Task<bool> Write(object p_message)
        {
            // 请求流已关闭
            if (_rpc.RequestStream == null || _rpc.RequestCompleted)
            {
                _rpc.FinishRequest();
                return false;
            }

            try
            {
                await RpcClientKit.WriteFrame(_rpc.RequestStream, p_message);
                return true;
            }
            catch { }

            _rpc.FinishRequest();
            return false;
        }

        /// <summary>
        /// 结束请求流
        /// </summary>
        /// <returns></returns>
        public void Complete()
        {
            _rpc.FinishRequest();
        }
    }
}
