#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
using static Dts.Core.Rpc.ServiceRpc;
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
            return Task.FromResult(Glb.GetCfg("rabbitmq", ""));
            //return AtTestRpc.GetString();
        }

        public Task<bool> SetString(string p_str)
        {
            return AtTestRpc.SetString(p_str);
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
            return Call<string>(
                "cm",
                "TestSerialize.GetString"
            );
        }

        /// <summary>
        /// 字符串参数
        /// </summary>
        /// <param name="p_str"></param>
        public static Task<bool> SetString(string p_str)
        {
            return Call<bool>(
                "cm",
                "TestSerialize.SetString",
                p_str
            );
        }
    }
}
