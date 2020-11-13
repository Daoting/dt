#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Serilog;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端与服务之间Rpc功能测试
    /// </summary>
    [Api(GroupName = "功能测试", AgentMode = AgentMode.Generic)]
    public class TestRpc
    {
        public string GetString()
        {
            return "字符串结果";
        }

        public bool SetString(string p_str)
        {
            return !string.IsNullOrEmpty(p_str);
        }

        public async Task OnServerStream(string p_title, ResponseWriter p_writer)
        {
            int i = 0;
            while (true)
            {
                var msg = $"{p_title} {i++}";
                if (!await p_writer.Write(msg) || i > 50)
                    break;
                Log.Information("服务端写入：" + msg);
                await Task.Delay(1000);
            }
            Log.Information("写入结束");
        }

        public async Task OnClientStream(string p_title, RequestReader p_reader)
        {
            while (await p_reader.MoveNext())
            {
                Log.Information("服务端读取：" + p_reader.Val<string>());
            }
        }

        public async Task OnDuplexStream(string p_title, RequestReader p_reader, ResponseWriter p_writer)
        {
            while (await p_reader.MoveNext())
            {
                Log.Information("服务端读取：" + p_reader.Val<string>());
                var msg = "++" + p_reader.Val<string>();
                await p_writer.Write(msg);
                Log.Information("服务端写入：" + msg);
            }
        }
    }
}
