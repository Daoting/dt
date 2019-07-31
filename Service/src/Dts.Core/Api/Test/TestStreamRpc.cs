#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dts.Core.Rpc;
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
        public Task OnServerStream(string p_title, ResponseWriter p_writer)
        {
            return Task.CompletedTask;
        }

        public Task OnClientStream(string p_title, RequestReader p_reader)
        {
            return Task.CompletedTask;
        }

        public Task OnDuplexStream(string p_title, RequestReader p_reader, ResponseWriter p_writer)
        {
            return Task.CompletedTask;
        }
    }
}
