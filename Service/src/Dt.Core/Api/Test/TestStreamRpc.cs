#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(GroupName = "功能测试", AgentMode = AgentMode.Generic)]
    public class TestStreamRpc : BaseApi
    {
        public async Task OnServerStream(string p_title, ResponseWriter p_writer)
        {
            for (int i = 0; i < 50; i++)
            {
                var msg = $"{p_title} {i}";
                await p_writer.Write(msg);
                _log.Information("服务端写入：" + msg);
                await Task.Delay(1000);
            }
        }

        public async Task OnClientStream(string p_title, RequestReader p_reader)
        {
            while (await p_reader.MoveNext())
            {
                _log.Information("服务端读取：" + p_reader.Val<string>());
            }
        }

        public async Task OnDuplexStream(string p_title, RequestReader p_reader, ResponseWriter p_writer)
        {
            while (await p_reader.MoveNext())
            {
                _log.Information("服务端读取：" + p_reader.Val<string>());
                var msg = "++" + p_reader.Val<string>();
                await p_writer.Write(msg);
                _log.Information("服务端写入：" + msg);
            }
        }
    }
}
