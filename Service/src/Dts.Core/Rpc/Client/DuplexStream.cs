#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    public class DuplexStream
    {
        public DuplexStream(RequestWriter p_requestWriter, ResponseReader p_responseReader)
        {
            RequestWriter = p_requestWriter;
            ResponseReader = p_responseReader;
        }

        public RequestWriter RequestWriter { get; }

        public ResponseReader ResponseReader { get; }
    }
}
