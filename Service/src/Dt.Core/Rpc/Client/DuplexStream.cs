#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-09 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 双工流
    /// </summary>
    public class DuplexStream
    {
        internal DuplexStream(RequestWriter p_requestWriter, ResponseReader p_responseReader)
        {
            RequestWriter = p_requestWriter;
            ResponseReader = p_responseReader;
        }

        /// <summary>
        /// 向服务器的写入流
        /// </summary>
        public RequestWriter RequestWriter { get; }

        /// <summary>
        /// 读取服务器的返回流
        /// </summary>
        public ResponseReader ResponseReader { get; }
    }
}
