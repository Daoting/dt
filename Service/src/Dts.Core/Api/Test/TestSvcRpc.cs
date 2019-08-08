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
using System.Threading.Tasks;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(true, "功能测试", AgentMode.Generic)]
    public class TestSvcRpc : BaseApi
    {
        public Task<string> GetString()
        {
            return AtTestRpc.GetString();
        }

        public Task<bool> SetString(string p_str)
        {
            return AtTestRpc.SetString(p_str);
        }

        public async Task OnServerStream(string p_title)
        {
            var reader = await AtTestRpc.OnServerStream(p_title);
            while (await reader.MoveNext())
            {
                Log.Information("客户端读取：" + reader.GetOriginalVal());
            }
        }

        public async Task OnClientStream(string p_title = "hello")
        {
            var writer = await AtTestRpc.OnClientStream(p_title);
            for (int i = 0; i < 30; i++)
            {
                var msg = $"{p_title} {i}";
                await writer.Write(msg);
                Log.Information("客户端写入：" + msg);
                //await Task.Delay(100);
            }
        }
    }

    internal static class AtTestRpc
    {
        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns></returns>
        public static Task<string> GetString()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetString"
            ).Call<string>();
        }

        /// <summary>
        /// 字符串参数
        /// </summary>
        /// <param name="p_str"></param>
        public static Task<bool> SetString(string p_str)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetString",
                p_str
            ).Call<bool>();
        }

        public static Task<ResponseReader> OnServerStream(string p_title)
        {
            return new ServerStreamRpc(
                "cm",
                "TestStreamRpc.OnServerStream",
                p_title
            ).Call();
        }

        public static Task<RequestWriter> OnClientStream(string p_title)
        {
            return new ClientStreamRpc(
                "cm",
                "TestStreamRpc.OnClientStream",
                p_title
            ).Call();
        }

        //public static DuplexStream OnDuplexStream(string p_title)
        //{
        //    return new StreamRpc(
        //        "cm",
        //        "TestStreamRpc.OnDuplexStream",
        //        p_title
        //    ).StartDuplexStream();
        //}
    }
}
