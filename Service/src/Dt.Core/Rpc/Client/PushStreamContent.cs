#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    internal class PushStreamContent : HttpContent
    {
        private readonly Func<Stream, Task> _onStreamAvailable;

        public PushStreamContent(Func<Stream, Task> onStreamAvailable)
        {
            _onStreamAvailable = onStreamAvailable;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            // 此处返回的Task未结束前一直可以写入流，实现客户端推送流功能！
            return _onStreamAvailable(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            // 设置内容长度未知
            length = -1;
            return false;
        }
    }
}
