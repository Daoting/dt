#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dts.Core.Rpc;
using Serilog;
using System;
using System.Threading.Tasks;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(true, "功能测试", AgentMode.Generic)]
    public class TestStreamRpc : BaseApi
    {
        public async Task OnServerStream(string p_title, ResponseWriter p_writer)
        {
            for (int i = 0; i < 100; i++)
            {
                var msg = $"{p_title} {i}";
                await p_writer.Write(msg);
                Log.Information("服务端写入：" + msg);
                await Task.Delay(1000);
            }
        }

        public async Task OnClientStream(string p_title, RequestReader p_reader)
        {
            while (await p_reader.MoveNext())
            {
                Log.Information("服务端读取：" + p_reader.GetOriginalVal());
            }
        }

        public Task OnDuplexStream(string p_title, RequestReader p_reader, ResponseWriter p_writer)
        {
            return Task.CompletedTask;
        }
    }
}
