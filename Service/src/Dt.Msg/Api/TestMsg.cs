#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021/6/25 9:07:41 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#endregion

namespace Dt.Msg.Api
{
    /// <summary>
    /// 
    /// </summary>
    [Api(IsTest = true)]
    public class TestMsg : BaseApi
    {
        public async Task<int> CloseAllOnline()
        {
            // 单副本
            int cnt = Online.TotalCount;
            var ls = Online.All.Values.ToList();
            foreach (var item in ls)
            {
                foreach (var ci in item)
                {
                    await ci.Close();
                }
            }
            return cnt;
        }

        public Task<string> CallCmGetString()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestSerialize.GetString"
            );
        }

        public async Task<int> CallAllReplica(string p_msg, bool p_checkReplica = true)
        {
            Log.Information($"{Kit.SvcID}收到：{p_msg}");
            int total = 1;

            // 查询所有其他副本
            if (p_checkReplica)
            {
                int cnt = Kit.GetSvcReplicaCount();
                if (cnt > 1)
                {
                    foreach (var svcID in Kit.GetOtherReplicaIDs())
                    {
                        await Kit.RpcInst<int>(svcID, "TestMsg.CallAllReplica", p_msg, false);
                        total++;
                    }
                }
            }
            return total;
        }
    }
}